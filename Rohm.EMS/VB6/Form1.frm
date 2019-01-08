VERSION 5.00
Object = "{72119D5E-D673-4A64-BD57-77AF62994034}#1.0#0"; "mscoree.dll"
Begin VB.Form Form1 
   Caption         =   "Form1"
   ClientHeight    =   6645
   ClientLeft      =   60
   ClientTop       =   345
   ClientWidth     =   5130
   LinkTopic       =   "Form1"
   ScaleHeight     =   6645
   ScaleWidth      =   5130
   StartUpPosition =   3  'Windows Default
   Begin Rohm_EMS_VB6Ctl.EmsReport EmsReport1 
      Height          =   450
      Left            =   4440
      TabIndex        =   12
      Top             =   120
      Width           =   450
      Object.Visible         =   "True"
      Enabled         =   "True"
      ForegroundColor =   "-2147483630"
      BackgroundColor =   "-2147483633"
      BackColor       =   "Control"
      ForeColor       =   "ControlText"
      Location        =   "296, 8"
      MaximumSize     =   "30, 30"
      MinimumSize     =   "30, 30"
      Name            =   "EmsReport"
      Size            =   "30, 30"
      Object.TabIndex        =   "0"
   End
   Begin VB.CommandButton LotEndButton 
      Caption         =   "End"
      Height          =   375
      Left            =   3600
      TabIndex        =   5
      Top             =   2640
      Width           =   1215
   End
   Begin VB.CommandButton SetOutputButton 
      Caption         =   "Set Output"
      Height          =   375
      Left            =   2640
      TabIndex        =   19
      Top             =   2640
      Width           =   975
   End
   Begin VB.TextBox StandardRPMTextBox 
      Height          =   375
      Left            =   1440
      TabIndex        =   3
      Top             =   2040
      Width           =   3015
   End
   Begin VB.Frame Frame1 
      Caption         =   "Frame1"
      Height          =   3015
      Left            =   600
      TabIndex        =   16
      Top             =   3360
      Width           =   3495
      Begin VB.OptionButton Option1 
         Caption         =   "Free Running"
         Height          =   255
         Index           =   4
         Left            =   480
         TabIndex        =   10
         Top             =   1800
         Width           =   2415
      End
      Begin VB.OptionButton Option1 
         Caption         =   "Running"
         Height          =   255
         Index           =   3
         Left            =   480
         TabIndex        =   9
         Top             =   1440
         Width           =   2415
      End
      Begin VB.OptionButton Option1 
         Caption         =   "Alarm"
         Height          =   255
         Index           =   2
         Left            =   480
         TabIndex        =   8
         Top             =   1080
         Width           =   2415
      End
      Begin VB.OptionButton Option1 
         Caption         =   "Stop"
         Height          =   255
         Index           =   1
         Left            =   480
         TabIndex        =   7
         Top             =   720
         Width           =   2415
      End
      Begin VB.OptionButton Option1 
         Caption         =   "Plan Stop"
         Height          =   255
         Index           =   0
         Left            =   480
         TabIndex        =   6
         Top             =   360
         Width           =   2415
      End
      Begin VB.CommandButton ChangeActivityButton 
         Caption         =   "Change Activity"
         Height          =   495
         Left            =   960
         TabIndex        =   11
         Top             =   2280
         Width           =   1215
      End
   End
   Begin VB.TextBox LotNoTextBox 
      Height          =   375
      Left            =   1440
      TabIndex        =   0
      Top             =   600
      Width           =   3015
   End
   Begin VB.TextBox TotalNGTextBox 
      Height          =   375
      Left            =   1440
      TabIndex        =   2
      Top             =   1560
      Width           =   3015
   End
   Begin VB.TextBox TotalGoodTextBox 
      Height          =   375
      Left            =   1440
      TabIndex        =   1
      Top             =   1080
      Width           =   3015
   End
   Begin VB.CommandButton LotStartButton 
      Caption         =   "Start"
      Height          =   375
      Left            =   1560
      TabIndex        =   4
      Top             =   2640
      Width           =   1095
   End
   Begin VB.CommandButton MockupButton 
      Caption         =   "Mock up"
      Height          =   375
      Left            =   600
      TabIndex        =   18
      Top             =   2640
      Width           =   975
   End
   Begin VB.Label Label2 
      Caption         =   "Standard RPM"
      Height          =   375
      Left            =   120
      TabIndex        =   17
      Top             =   2040
      Width           =   1215
   End
   Begin VB.Label Label1 
      Caption         =   "Total NG"
      Height          =   375
      Index           =   2
      Left            =   360
      TabIndex        =   15
      Top             =   1560
      Width           =   855
   End
   Begin VB.Label Label1 
      Caption         =   "Total Good"
      Height          =   375
      Index           =   1
      Left            =   360
      TabIndex        =   14
      Top             =   1080
      Width           =   855
   End
   Begin VB.Label Label1 
      Caption         =   "LotNo"
      Height          =   375
      Index           =   0
      Left            =   360
      TabIndex        =   13
      Top             =   600
      Width           =   855
   End
End
Attribute VB_Name = "Form1"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False

Private Sub ChangeActivityButton_Click()
    
    If Option1(0).Value Then
        Call EmsReport1.SetActivity(Option1(0).Caption, TmeCategory_PlanStopLoss)
    ElseIf Option1(1).Value Then
        Call EmsReport1.SetActivity(Option1(1).Caption, TmeCategory_StopLoss)
    ElseIf Option1(2).Value Then
        Call EmsReport1.SetActivity(Option1(2).Caption, TmeCategory_ChokotieLoss)
    ElseIf Option1(3).Value Then
        Call EmsReport1.SetActivity(Option1(3).Caption, TmeCategory_RealOperation)
    ElseIf Option1(4).Value Then
        Call EmsReport1.SetActivity(Option1(4).Caption, TmeCategory_FreeRunningLoss)
    End If
        
End Sub

Private Sub Form_Load()
    Call EmsReport1.Init("Z-99", "IFLX", "FL", "PE1-OFFICE", "9999A8888V", 2000, 200, 53.5, "http://10.28.33.254:7777/EmsService/")
End Sub

Private Sub Form_Unload(Cancel As Integer)
    EmsReport1.SaveState
End Sub

Private Sub LotEndButton_Click()
    Call EmsReport1.LotEnd
    LotNoTextBox.Text = ""
    TotalGoodTextBox.Text = "0"
    TotalNGTextBox.Text = "0"
    StandardRPMTextBox.Text = "0"
End Sub

Private Sub LotStartButton_Click()
    Call EmsReport1.SetupLot(LotNoTextBox.Text, CSng(StandardRPMTextBox.Text))
End Sub

Private Sub MockupButton_Click()
    Call EmsReport1.SetupLot("1123A4321V", 55.2)
    Call EmsReport1.SetOutput(1000, 10)
    Call EmsReport1.SetActivity("FreeRunning", TmeCategory_FreeRunningLoss)
    Call EmsReport1.SetActivity("Running", TmeCategory_RealOperation)
    Call EmsReport1.LotEnd
    Call EmsReport1.SetActivity("LotEndStop", TmeCategory_StopLoss)
    
    Call EmsReport1.SetupLot("1123A4987V", 75.2)
    Call EmsReport1.SetOutput(5000, 100)
    Call EmsReport1.SetActivity("FreeRunning", TmeCategory_FreeRunningLoss)
    Call EmsReport1.SetActivity("Running", TmeCategory_RealOperation)
    Call EmsReport1.LotEnd
    Call EmsReport1.SetActivity("LotEndStop", TmeCategory_StopLoss)
End Sub

Private Sub SetOutputButton_Click()
    Call EmsReport1.SetOutput(CLng(TotalGoodTextBox.Text), CLng(TotalNGTextBox.Text))
End Sub
