Imports Rohm.EMS.VB6.EmsServiceReference
Imports System.ServiceModel
Imports System.IO
Imports System.Xml.Serialization

<ComClass(EmsReport.ClassId, EmsReport.InterfaceId, EmsReport.EventsId)> _
Public Class EmsReport

#Region "VB6 Interop Code"

#If COM_INTEROP_ENABLED Then

#Region "COM Registration"

    ' These  GUIDs provide the COM identity for this class 
    ' and its COM interfaces. If you change them, existing 
    ' clients will no longer be able to access the class.

    Public Const ClassId As String = "e839d227-bd46-4613-aa21-984724d33977"
    Public Const InterfaceId As String = "7c43748e-3604-48f4-addc-1e5f8c427813"
    Public Const EventsId As String = "443dde7b-3579-4cd0-8c18-bd7ae1905892"

    'These routines perform the additional COM registration needed by ActiveX controls
    <EditorBrowsable(EditorBrowsableState.Never)> _
    <ComRegisterFunction()> _
    Private Shared Sub Register(ByVal t As Type)
        ComRegistration.RegisterControl(t)
    End Sub

    <EditorBrowsable(EditorBrowsableState.Never)> _
    <ComUnregisterFunction()> _
    Private Shared Sub Unregister(ByVal t As Type)
        ComRegistration.UnregisterControl(t)
    End Sub

#End Region

#Region "VB6 Events"

    'This section shows some examples of exposing a UserControl's events to VB6.  Typically, you just
    '1) Declare the event as you want it to be shown in VB6
    '2) Raise the event in the appropriate UserControl event.

    Public Shadows Event Click() 'Event must be marked as Shadows since .NET UserControls have the same name.
    Public Event DblClick()

    Private Sub InteropUserControl_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Click, picBackground.Click
        RaiseEvent Click()
    End Sub

    Private Sub InteropUserControl_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.DoubleClick, picBackground.DoubleClick
        RaiseEvent DblClick()
    End Sub

#End Region

#Region "VB6 Properties"

    'The following are examples of how to expose typical form properties to VB6.  
    'You can also use these as examples on how to add additional properties.

    'Must Shadow this property as it exists in Windows.Forms and is not overridable
    Public Shadows Property Visible() As Boolean
        Get
            Return MyBase.Visible
        End Get
        Set(ByVal value As Boolean)
            MyBase.Visible = value
        End Set
    End Property

    Public Shadows Property Enabled() As Boolean
        Get
            Return MyBase.Enabled
        End Get
        Set(ByVal value As Boolean)
            MyBase.Enabled = value
        End Set
    End Property

    Public Shadows Property ForegroundColor() As Integer
        Get
            Return ActiveXControlHelpers.GetOleColorFromColor(MyBase.ForeColor)
        End Get
        Set(ByVal value As Integer)
            MyBase.ForeColor = ActiveXControlHelpers.GetColorFromOleColor(value)
        End Set
    End Property

    Public Shadows Property BackgroundColor() As Integer
        Get
            Return ActiveXControlHelpers.GetOleColorFromColor(MyBase.BackColor)
        End Get
        Set(ByVal value As Integer)
            MyBase.BackColor = ActiveXControlHelpers.GetColorFromOleColor(value)
        End Set
    End Property

    Public Overrides Property BackgroundImage() As System.Drawing.Image
        Get
            Return Nothing
        End Get
        Set(ByVal value As System.Drawing.Image)
            If value IsNot Nothing Then
                MsgBox("Setting the background image of an Interop UserControl is not supported, please use a PictureBox instead.", MsgBoxStyle.Information)
            End If
            MyBase.BackgroundImage = Nothing
        End Set
    End Property

#End Region

#Region "VB6 Methods"

    Public Overrides Sub Refresh()
        MyBase.Refresh()
    End Sub

    'Ensures that tabbing across VB6 and .NET controls works as expected
    Private Sub UserControl_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.LostFocus
        ActiveXControlHelpers.HandleFocus(Me)
    End Sub

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.++

        'Out of memory will be raised
        'm_Recorder = New EmsClient(My.Settings.ProcessName, My.Settings.EmsServiceUrl)

        'Raise Load event
        Me.OnCreateControl()
    End Sub

    <SecurityPermission(SecurityAction.LinkDemand, Flags:=SecurityPermissionFlag.UnmanagedCode)> _
    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)

        Const WM_SETFOCUS As Integer = &H7
        Const WM_PARENTNOTIFY As Integer = &H210
        Const WM_DESTROY As Integer = &H2
        Const WM_LBUTTONDOWN As Integer = &H201
        Const WM_RBUTTONDOWN As Integer = &H204

        If m.Msg = WM_SETFOCUS Then
            'Raise Enter event
            Me.OnEnter(New System.EventArgs)

        ElseIf m.Msg = WM_PARENTNOTIFY AndAlso _
            (m.WParam.ToInt32 = WM_LBUTTONDOWN OrElse _
             m.WParam.ToInt32 = WM_RBUTTONDOWN) Then

            If Not Me.ContainsFocus Then
                'Raise Enter event
                Me.OnEnter(New System.EventArgs)
            End If

        ElseIf m.Msg = WM_DESTROY AndAlso Not Me.IsDisposed AndAlso Not Me.Disposing Then
            'Used to ensure that VB6 will cleanup control properly
            Me.Dispose()
        End If

        MyBase.WndProc(m)
    End Sub

    'This event will hook up the necessary handlers
    Private Sub InteropUserControl_ControlAdded(ByVal sender As Object, ByVal e As ControlEventArgs) Handles Me.ControlAdded
        ActiveXControlHelpers.WireUpHandlers(e.Control, AddressOf ValidationHandler)
    End Sub

    'Ensures that the Validating and Validated events fire appropriately
    Friend Sub ValidationHandler(ByVal sender As Object, ByVal e As EventArgs)

        If Me.ContainsFocus Then Return

        'Raise Leave event
        Me.OnLeave(e)

        If Me.CausesValidation Then
            Dim validationArgs As New CancelEventArgs
            Me.OnValidating(validationArgs)

            If validationArgs.Cancel AndAlso Me.ActiveControl IsNot Nothing Then
                Me.ActiveControl.Focus()
            Else
                'Raise Validated event
                Me.OnValidated(e)
            End If
        End If

    End Sub

#End Region

#End If

#End Region

    Private m_SaveFileName As String
    Private m_EMSFolder As String = "EMS"
    Private m_Recorder As EmsClient
    Private m_MCNo As String

    Public Sub Init(ByVal mcNo As String, ByVal machineType As String, ByVal lotNo As String, ByVal totalGood As Integer, ByVal totalNG As Integer, ByVal standardRpm As Double)


        m_Recorder = New EmsClient(My.Settings.ProcessName, My.Settings.EmsServiceUrl)

        If Not Directory.Exists(m_EMSFolder) Then
            Directory.CreateDirectory(m_EMSFolder)
        End If

        m_MCNo = mcNo
        Dim regInfo As EmsMachineRegisterInfo = New EmsMachineRegisterInfo()
        regInfo.MCNo = mcNo
        regInfo.AreaName = My.Settings.AreaName
        regInfo.ProcessName = My.Settings.ProcessName
        regInfo.MachineTypeName = machineType
        regInfo.CurrentLotNo = lotNo
        regInfo.CurrentTotalGood = totalGood
        regInfo.CurrentTotalNG = totalNG
        regInfo.CurrentStandardRPM = standardRpm
        regInfo.CutTotalGood = 0
        regInfo.CutTotalNG = 0
        BackgroundWorker1.RunWorkerAsync(regInfo)

    End Sub


    Private Sub BackgroundWorker1_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Dim regInfo As EmsMachineRegisterInfo = CType(e.Argument, EmsMachineRegisterInfo)
        Try
            m_Recorder.Register(regInfo)
            m_Recorder.Start()
        Catch ex As Exception
            SaveExceptionLog("Init-001", ex)
        End Try
    End Sub

    Public Sub [Stop]()
        Try
            m_Recorder.[Stop]()
        Catch ex As Exception
            SaveExceptionLog("[Stop]", ex)
        End Try

    End Sub

    Private Sub SaveExceptionLog(ByVal errorCode As String, ByVal ex As Exception)
        Using sw As StreamWriter = New StreamWriter(m_EMSFolder & "\EmsService.error.log", True)
            sw.WriteLine("{0} [{1}]=>{2}", Now.ToString("yyyy/MM/dd HH:mm:ss"), errorCode, ex.Message, ex.StackTrace)
        End Using
    End Sub

    Public Sub SetupLot(ByVal lotNo As String, ByVal standardRPM As Double)
        m_Recorder.SetCurrentLot(m_MCNo, lotNo, standardRPM)
    End Sub

    Public Sub SetOutput(ByVal totalGood As Integer, ByVal totalNG As Integer)
        m_Recorder.SetOutput(m_MCNo, totalGood, totalNG)
    End Sub

    Public Sub SetActivity(ByVal activityName As String, ByVal category As TmeCategory)
        m_Recorder.SetActivity(m_MCNo, activityName, category)
        PlanStopToolStripMenuItem.Checked = False
    End Sub

    Public Sub LotEnd()
        m_Recorder.SetLotEnd(m_MCNo)
    End Sub

    Private Sub SettingToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SettingToolStripMenuItem.Click
        Using f As AppSettingForm = New AppSettingForm()
            f.ShowDialog()
        End Using
    End Sub

    Private Sub PlanStopToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PlanStopToolStripMenuItem.Click
        SetActivity("Plan Stop", TmeCategory.PlanStopLoss)
        PlanStopToolStripMenuItem.Checked = True
    End Sub

End Class