<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ManualReportActivityForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.ReportButton = New System.Windows.Forms.Button
        Me.ActivityComboBox = New System.Windows.Forms.ComboBox
        Me.CLButton = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'ReportButton
        '
        Me.ReportButton.Location = New System.Drawing.Point(96, 94)
        Me.ReportButton.Margin = New System.Windows.Forms.Padding(7, 7, 7, 7)
        Me.ReportButton.Name = "ReportButton"
        Me.ReportButton.Size = New System.Drawing.Size(175, 51)
        Me.ReportButton.TabIndex = 0
        Me.ReportButton.Text = "Report"
        Me.ReportButton.UseVisualStyleBackColor = True
        '
        'ComboBox1
        '
        Me.ActivityComboBox.FormattingEnabled = True
        Me.ActivityComboBox.Location = New System.Drawing.Point(32, 20)
        Me.ActivityComboBox.Name = "ComboBox1"
        Me.ActivityComboBox.Size = New System.Drawing.Size(480, 37)
        Me.ActivityComboBox.TabIndex = 1
        '
        'CancelButton
        '
        Me.CLButton.Location = New System.Drawing.Point(281, 94)
        Me.CLButton.Name = "CancelButton"
        Me.CLButton.Size = New System.Drawing.Size(150, 51)
        Me.CLButton.TabIndex = 2
        Me.CLButton.Text = "Cancel"
        Me.CLButton.UseVisualStyleBackColor = True
        '
        'ManualReportActivityForm
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(543, 173)
        Me.ControlBox = False
        Me.Controls.Add(Me.CLButton)
        Me.Controls.Add(Me.ActivityComboBox)
        Me.Controls.Add(Me.ReportButton)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Margin = New System.Windows.Forms.Padding(7, 7, 7, 7)
        Me.Name = "ManualReportActivityForm"
        Me.Text = "ManualReportActivityForm"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ReportButton As System.Windows.Forms.Button
    Friend WithEvents ActivityComboBox As System.Windows.Forms.ComboBox
    Friend WithEvents CLButton As System.Windows.Forms.Button
End Class
