Option Strict On
Imports System.Windows.Forms
Imports System.ComponentModel

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class EmsReport
    Inherits System.Windows.Forms.UserControl

    'InteropUserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Me.picBackground = New System.Windows.Forms.PictureBox
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.SettingToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.PlanStopToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.m_Timer = New System.Windows.Forms.Timer(Me.components)
        Me.BackgroundWorker1 = New System.ComponentModel.BackgroundWorker
        CType(Me.picBackground, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'picBackground
        '
        Me.picBackground.ContextMenuStrip = Me.ContextMenuStrip1
        Me.picBackground.Dock = System.Windows.Forms.DockStyle.Fill
        Me.picBackground.Image = Global.Rohm.EMS.VB6.My.Resources.Resources.ems
        Me.picBackground.Location = New System.Drawing.Point(0, 0)
        Me.picBackground.Name = "picBackground"
        Me.picBackground.Size = New System.Drawing.Size(30, 30)
        Me.picBackground.TabIndex = 0
        Me.picBackground.TabStop = False
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SettingToolStripMenuItem, Me.PlanStopToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(117, 48)
        '
        'SettingToolStripMenuItem
        '
        Me.SettingToolStripMenuItem.Name = "SettingToolStripMenuItem"
        Me.SettingToolStripMenuItem.Size = New System.Drawing.Size(116, 22)
        Me.SettingToolStripMenuItem.Text = "Setting"
        '
        'PlanStopToolStripMenuItem
        '
        Me.PlanStopToolStripMenuItem.Name = "PlanStopToolStripMenuItem"
        Me.PlanStopToolStripMenuItem.Size = New System.Drawing.Size(116, 22)
        Me.PlanStopToolStripMenuItem.Text = "PlanStop"
        '
        'm_Timer
        '
        Me.m_Timer.Interval = 1000
        '
        'BackgroundWorker1
        '
        '
        'EmsReport
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.Controls.Add(Me.picBackground)
        Me.MaximumSize = New System.Drawing.Size(30, 30)
        Me.MinimumSize = New System.Drawing.Size(30, 30)
        Me.Name = "EmsReport"
        Me.Size = New System.Drawing.Size(30, 30)
        CType(Me.picBackground, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents picBackground As System.Windows.Forms.PictureBox
    Friend WithEvents m_Timer As System.Windows.Forms.Timer
    Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents SettingToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PlanStopToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents BackgroundWorker1 As System.ComponentModel.BackgroundWorker

End Class
