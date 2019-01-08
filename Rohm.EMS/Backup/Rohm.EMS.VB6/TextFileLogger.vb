Imports System.IO

Public Class TextFileLogger

    Public Property FileName() As String
        Get
            Return m_FileName
        End Get
        Set(ByVal value As String)
            m_FileName = value
        End Set
    End Property
    Private m_FileName As String

    Private m_Locker As Object


    Public Sub New(ByVal fileName As String)
        Me.FileName = fileName
        m_Locker = New Object()
    End Sub

#Region "ILogger Members"

    Public Sub Log(ByVal [function] As String, ByVal position As String, ByVal errMessage As String, ByVal comment As String)
        SyncLock m_Locker
            Using sw As New StreamWriter(Me.FileName, True)
                sw.WriteLine(String.Format("DateTime:{0}, Function:{1}, Position:{2}, ErrorMessage:{3}, Comment:{4}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), [function], position, errMessage, comment))
            End Using
        End SyncLock
    End Sub

#End Region

    '=======================================================
    'Service provided by Telerik (www.telerik.com)
    'Conversion powered by NRefactory.
    'Twitter: @telerik
    'Facebook: facebook.com/telerik
    '=======================================================

End Class
