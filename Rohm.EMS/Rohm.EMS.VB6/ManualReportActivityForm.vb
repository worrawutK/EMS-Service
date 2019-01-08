Public Class ManualReportActivityForm

    Private m_SelectedActivityID As Integer
    Public ReadOnly Property SelectedActivityID() As Integer
        Get
            Return m_SelectedActivityID
        End Get
    End Property

    Public Function GetActivityID(ByVal dataTable As DataTable) As Boolean
        ActivityComboBox.DisplayMember = "Name"
        ActivityComboBox.ValueMember = "ID"
        ActivityComboBox.DataSource = dataTable
        If dataTable.Rows.Count > 0 Then
            ActivityComboBox.SelectedIndex = 0
        End If
        Return Me.ShowDialog() = Windows.Forms.DialogResult.OK
    End Function

    Private Sub ReportButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReportButton.Click
        m_SelectedActivityID = CInt(ActivityComboBox.SelectedValue)
        Me.DialogResult = Windows.Forms.DialogResult.OK
    End Sub

    Private Sub CancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CLButton.Click
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
    End Sub

End Class