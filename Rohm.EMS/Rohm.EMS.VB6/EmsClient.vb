Imports Rohm.EMS.VB6.EmsServiceReference
Imports System.ServiceModel
Imports System.IO

Public Class EmsClient


    Private m_Log As TextFileLogger

    Private m_Timer As System.Timers.Timer
    Private m_UpdateOutputTimer As System.Timers.Timer

    Private m_ReporterClient As ReporterClient
    Private m_MachineDictionary As Dictionary(Of String, EmsMachineOutputInfo)
    Private m_MachineRegisterInfoDictionary As Dictionary(Of String, EmsMachineRegisterInfo)

    Private m_ProcessName As String
    Public Property ProcessName() As String
        Get
            Return m_ProcessName
        End Get
        Set(ByVal value As String)
            If m_ProcessName <> value Then
                m_ProcessName = value
            End If
        End Set
    End Property

    Public Property UpdateOutputInterval() As Double
        Get
            Return m_UpdateOutputTimer.Interval
        End Get
        Set(ByVal value As Double)
            m_UpdateOutputTimer.Interval = value
        End Set
    End Property

    Private m_Locker As New Object()

    Public Sub New(ByVal processName As String, ByVal serviceEndpointUrl As String)

        'http://stackoverflow.com/questions/3188965/could-not-find-endpoint-element-with-name
        'It turns out that we have an exe that loads a DLL. The DLL contains the WCF client. 
        'When compiled, MyServer.dll.config is generated, 
        'but since the exe is native (not .NET) it does not read in a .config file automatically. 
        'We need to do it manually. This link allowed me to load the config manually and create a CustomChannelFactory<> to solve this question.

        'http://www.paraesthesia.com/archive/2008/11/26/reading-wcf-configuration-from-a-custom-location.aspx/

        Dim binding As BasicHttpBinding = New BasicHttpBinding()
        Dim endpoint As EndpointAddress = New EndpointAddress(serviceEndpointUrl)
        m_ReporterClient = New ReporterClient(binding, endpoint)

        m_MachineDictionary = New Dictionary(Of String, EmsMachineOutputInfo)()
        m_MachineRegisterInfoDictionary = New Dictionary(Of String, EmsMachineRegisterInfo)()

        m_Timer = New System.Timers.Timer()
        m_Timer.Interval = 1000
        AddHandler m_Timer.Elapsed, AddressOf m_Timer_Elapsed
        m_UpdateOutputTimer = New System.Timers.Timer()
        m_UpdateOutputTimer.Interval = 600000
        ' 10 minutes
        AddHandler m_UpdateOutputTimer.Elapsed, AddressOf m_UpdateOutputTimer_Elapsed
        m_ProcessName = processName

        m_Log = New TextFileLogger(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmsClient.log"))
    End Sub

    Private Sub m_UpdateOutputTimer_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs)
        m_UpdateOutputTimer.[Stop]()

        SyncLock m_Locker
            Try
                For Each mc As EmsMachineOutputInfo In m_MachineDictionary.Values
                    m_ReporterClient.SaveOutputInfo(mc)
                Next
            Catch ex As Exception
                m_Log.Log("m_UpdateOutputTimer_Elapsed", "SaveOutputInfo", ex.Message, "")
            End Try
        End SyncLock

        m_UpdateOutputTimer.Start()
    End Sub

    Private Sub m_Timer_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs)
        m_Timer.[Stop]()
        SyncLock m_Locker
            Dim tmp As DateTime = DateTime.Now
            If tmp.Hour = 8 AndAlso tmp.Minute = 0 AndAlso tmp.Second = 0 Then
                Dim co As EmsOutputRecordBLL
                For Each mc As EmsMachineOutputInfo In m_MachineDictionary.Values
                    co = CaptureOutput(mc)

                    mc.CutTotalGood = mc.CurrentTotalGood
                    mc.CutTotalNG = mc.CurrentTotalNG

                    Try
                        'save to db
                        m_ReporterClient.ReportOutput(co)
                    Catch ex As Exception
                        m_Log.Log("m_Timer_Elapsed", "ReportOutput", ex.Message, "")
                    End Try

                    Try
                        m_ReporterClient.SaveOutputInfo(mc)
                    Catch ex As Exception
                        m_Log.Log("m_Timer_Elapsed", "SaveOutputInfo", ex.Message, "")
                    End Try
                Next
            End If
        End SyncLock
        m_Timer.Start()
    End Sub

    Public Sub Start()
        m_Timer.Start()
        m_UpdateOutputTimer.Start()
    End Sub

    Public Sub [Stop]()
        m_UpdateOutputTimer.[Stop]()
        m_Timer.[Stop]()
        For Each mc As EmsMachineOutputInfo In m_MachineDictionary.Values
            m_ReporterClient.SaveOutputInfo(mc)
        Next
    End Sub

    Protected Overrides Sub Finalize()
        Try
            m_MachineDictionary.Clear()
            m_MachineDictionary = Nothing
            m_MachineRegisterInfoDictionary.Clear()
            m_MachineRegisterInfoDictionary = Nothing
            m_ReporterClient = Nothing
            m_Timer.[Stop]()
            RemoveHandler m_Timer.Elapsed, AddressOf m_Timer_Elapsed
            m_Timer = Nothing
        Finally
            MyBase.Finalize()
        End Try
    End Sub

    Public Sub Register(ByVal regInfo As EmsMachineRegisterInfo)
        Dim key As String = GetMachineUniqueKey(regInfo.ProcessName, regInfo.MCNo)
        '1.) keep registere  info
        'remove old register data if exists
        If m_MachineRegisterInfoDictionary.ContainsKey(key) Then
            m_MachineRegisterInfoDictionary.Remove(key)
        End If
        'add new register data
        m_MachineRegisterInfoDictionary.Add(key, regInfo)

        PrivateRegister(key, regInfo)
    End Sub

    Public Sub Remove(ByVal mcNo As String)
        Dim oi As EmsMachineOutputInfo = GetEmsMachine(mcNo)
        If oi IsNot Nothing Then
            m_ReporterClient.SaveOutputInfo(oi)
            m_MachineDictionary.Remove(mcNo)
        End If
    End Sub

    Public Sub SetActivity(ByVal mcNo As String, ByVal activityName As String, ByVal category As TmeCategory)
        If String.IsNullOrEmpty(activityName) Then
            Return
        End If

        Try
            Dim errorCode As Integer = m_ReporterClient.SetCurrentActivity(m_ProcessName, mcNo, activityName, category.ToString())
            If errorCode = 1 Then
                'machine not found
                ReRegister(mcNo)
                m_ReporterClient.SetCurrentActivity(m_ProcessName, mcNo, activityName, category.ToString())
            End If
        Catch ex As Exception
            m_Log.Log("SetActivity", "SetCurrentActivity", ex.Message, "")
        End Try
    End Sub

    Public Sub SetCurrentLot(ByVal mcNo As String, ByVal lotNo As String, ByVal standardRpm As Double)
        Dim oi As EmsMachineOutputInfo = TryGetEmsMachine(mcNo)

        If oi IsNot Nothing Then
            oi.CurrentLotNo = lotNo
            oi.CurrentStandardRPM = standardRpm
            oi.CurrentTotalGood = 0
            oi.CurrentTotalNG = 0
            oi.CutTotalGood = 0
            oi.CutTotalNG = 0

            m_ReporterClient.SaveOutputInfo(oi)
        End If
    End Sub

    Public Sub SetOutput(ByVal mcNo As String, ByVal totalGood As Integer, ByVal totalNG As Integer)
        Dim oi As EmsMachineOutputInfo = TryGetEmsMachine(mcNo)
        If oi IsNot Nothing Then
            oi.CurrentTotalGood = totalGood
            oi.CurrentTotalNG = totalNG
        End If
    End Sub

    Public Sub SetLotEnd(ByVal mcNo As String)
        Dim mc As EmsMachineOutputInfo = TryGetEmsMachine(mcNo)
        If mc Is Nothing Then
            Return
        End If

        Dim co As EmsOutputRecordBLL = CaptureOutput(mc)

        mc.CurrentLotNo = ""
        'clear Lot
        mc.CurrentStandardRPM = 0
        mc.CurrentTotalGood = 0
        mc.CurrentTotalNG = 0
        mc.CutTotalGood = 0
        mc.CutTotalNG = 0

        Try
            m_ReporterClient.ReportOutput(co)
        Catch ex As Exception
            m_Log.Log("SetLotEnd", "ReportOutput", ex.Message, "")
        End Try

        Try
            m_ReporterClient.SaveOutputInfo(mc)
        Catch ex As Exception
            m_Log.Log("SetLotEnd", "SaveOutputInfo", ex.Message, "")
        End Try
    End Sub

    Private Function TryGetEmsMachine(ByVal mcNo As String) As EmsMachineOutputInfo
        Dim oi As EmsMachineOutputInfo = Nothing
        Dim key As String = GetMachineUniqueKey(m_ProcessName, mcNo)

        If m_MachineDictionary.ContainsKey(key) Then
            oi = m_MachineDictionary(key)
        Else
            ReRegister(mcNo)
            'check again after re-register
            If m_MachineDictionary.ContainsKey(key) Then
                oi = m_MachineDictionary(key)

            End If
        End If
        Return oi
    End Function

    Private Sub ReRegister(ByVal mcNo As String)
        Dim key As String = GetMachineUniqueKey(m_ProcessName, mcNo)

        '1.) keep registere  info
        If Not m_MachineRegisterInfoDictionary.ContainsKey(key) Then
            Return
        End If
        'add new register data
        Dim regInfo As EmsMachineRegisterInfo = m_MachineRegisterInfoDictionary(key)
        PrivateRegister(key, regInfo)

    End Sub

    Private Sub PrivateRegister(ByVal key As String, ByVal regInfo As EmsMachineRegisterInfo)
        Try
            '2.) register
            Dim registeredMC As EmsMachineOutputInfo = m_ReporterClient.RegisterMachine(regInfo)
            '3.) check exists 
            If m_MachineDictionary.ContainsKey(key) Then
                m_MachineDictionary.Remove(key)
            End If
            m_MachineDictionary.Add(key, registeredMC)
        Catch ex As Exception
            m_Log.Log("PrivateRegister", "RegisterMachine", ex.Message, "")
        End Try
    End Sub

    Private Function GetEmsMachine(ByVal mcNo As String) As EmsMachineOutputInfo
        Dim oi As EmsMachineOutputInfo = Nothing
        Dim key As String = GetMachineUniqueKey(m_ProcessName, mcNo)
        If m_MachineDictionary.ContainsKey(key) Then
            oi = m_MachineDictionary(key)
        End If
        Return oi
    End Function

    Private Function CaptureOutput(ByVal mc As EmsMachineOutputInfo) As EmsOutputRecordBLL
        If String.IsNullOrEmpty(mc.CurrentLotNo) Then
            Return Nothing
        End If

        Dim ret As New EmsOutputRecordBLL()
        ret.LotNo = mc.CurrentLotNo
        ret.MCNo = mc.MCNo
        ret.ProcessName = mc.ProcessName
        ret.RohmDate = DateTime.Now.AddHours(-8).[Date]
        ret.StandardRPM = mc.CurrentStandardRPM
        ret.TotalGood = mc.CurrentTotalGood - mc.CutTotalGood
        ret.TotalNG = mc.CurrentTotalNG - mc.CutTotalNG

        Return ret
    End Function

    Private Function GetMachineUniqueKey(ByVal processName As String, ByVal mcNo As String) As String
        Dim key As String = processName & mcNo
        Return key
    End Function

    '=======================================================
    'Service provided by Telerik (www.telerik.com)
    'Conversion powered by NRefactory.
    'Twitter: @telerik
    'Facebook: facebook.com/telerik
    '=======================================================



End Class

