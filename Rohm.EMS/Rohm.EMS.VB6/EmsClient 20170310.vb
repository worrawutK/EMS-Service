Imports Rohm.EMS.VB6.EmsServiceReference
Imports System.ServiceModel

Public Class EmsClient

    Private m_Timer As System.Timers.Timer
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

    Public Sub New(ByVal processName As String, ByVal serviceEndpointUrl As String)
        m_MachineDictionary = New Dictionary(Of String, EmsMachineOutputInfo)()
        m_MachineRegisterInfoDictionary = New Dictionary(Of String, EmsMachineRegisterInfo)()
        Dim binding As BasicHttpBinding = New BasicHttpBinding()
        Dim endpoint As EndpointAddress = New EndpointAddress(serviceEndpointUrl)
        m_ReporterClient = New ReporterClient(binding, endpoint)
        m_Timer = New System.Timers.Timer()
        m_Timer.Interval = 1000
        AddHandler m_Timer.Elapsed, New System.Timers.ElapsedEventHandler(AddressOf m_Timer_Elapsed)
        m_ProcessName = processName
    End Sub

    Private Sub m_Timer_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs)
        m_Timer.[Stop]()
        Dim tmp As DateTime = DateTime.Now
        If tmp.Hour = 8 AndAlso tmp.Minute = 0 AndAlso tmp.Second = 0 Then
            For Each mc As EmsMachineOutputInfo In m_MachineDictionary.Values
                CaptureOutput(mc, False)
            Next
        End If
        m_Timer.Start()
    End Sub

    Public Sub Start()
        m_Timer.Start()
    End Sub

    Public Sub [Stop]()

        For Each mc As EmsMachineOutputInfo In m_MachineDictionary.Values
            m_ReporterClient.SaveOutputInfo(mc)
        Next
        m_Timer.[Stop]()
    End Sub

    Protected Overrides Sub Finalize()
        Try
            m_MachineDictionary.Clear()
            m_MachineDictionary = Nothing
            m_MachineRegisterInfoDictionary.Clear()
            m_MachineRegisterInfoDictionary = Nothing
            m_ReporterClient = Nothing
            m_Timer.[Stop]()
            AddHandler m_Timer.Elapsed, New System.Timers.ElapsedEventHandler(AddressOf m_Timer_Elapsed)
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
        If oi Is Nothing Then
            m_MachineDictionary.Remove(mcNo)
            m_ReporterClient.SaveOutputInfo(oi)
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
        Catch generatedExceptionName As Exception
        End Try
    End Sub

    Public Sub SetCurrentLot(ByVal mcNo As String, ByVal lotNo As String, ByVal standardRpm As Double)
        Dim oi As EmsMachineOutputInfo = TryGetEmsMachine(mcNo)
        oi.CurrentLotNo = lotNo
        oi.CurrentStandardRPM = standardRpm
    End Sub

    Public Sub SetOutput(ByVal mcNo As String, ByVal totalGood As Integer, ByVal totalNG As Integer)
        Dim oi As EmsMachineOutputInfo = TryGetEmsMachine(mcNo)
        oi.CurrentTotalGood = totalGood
        oi.CurrentTotalNG = totalNG
    End Sub

    Public Sub SetLotEnd(ByVal mcNo As String)
        Dim oi As EmsMachineOutputInfo = TryGetEmsMachine(mcNo)
        CaptureOutput(oi, True)
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
        'remove old register data if exists
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
        Catch
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

    Private Sub CaptureOutput(ByVal mc As EmsMachineOutputInfo, ByVal lotIsEnd As Boolean)
        If String.IsNullOrEmpty(mc.CurrentLotNo) Then
            Return
        End If

        Dim capturedOutput As New EmsOutputRecordBLL()
        capturedOutput.LotNo = mc.CurrentLotNo
        capturedOutput.MCNo = mc.MCNo
        capturedOutput.ProcessName = mc.ProcessName
        capturedOutput.RohmDate = DateTime.Now.AddHours(-8).[Date]
        capturedOutput.StandardRPM = mc.CurrentStandardRPM
        capturedOutput.TotalGood = mc.CurrentTotalGood - mc.CutTotalGood
        capturedOutput.TotalNG = mc.CurrentTotalNG - mc.CutTotalNG
        If lotIsEnd Then
            mc.CurrentLotNo = ""
            'clear Lot
            mc.CutTotalGood = 0
            mc.CutTotalNG = 0
            mc.CurrentStandardRPM = 0
            mc.CurrentTotalGood = 0
            mc.CurrentTotalNG = 0
        Else
            mc.CutTotalGood = mc.CurrentTotalGood
            mc.CutTotalNG = mc.CurrentTotalNG
        End If

        Try
            'save to db
            m_ReporterClient.ReportOutput(capturedOutput)
        Catch generatedExceptionName As Exception
        End Try
    End Sub

    Private Function GetMachineUniqueKey(ByVal processName As String, ByVal mcNo As String) As String
        Dim key As String = processName & mcNo
        Return key
    End Function


End Class

