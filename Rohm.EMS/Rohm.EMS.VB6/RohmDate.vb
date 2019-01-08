Public Class RohmDate
    Public Shared Function GetCurrent() As Date
        Dim tmp As Date = Now
        Return tmp.AddHours(-8).Date
    End Function
End Class
