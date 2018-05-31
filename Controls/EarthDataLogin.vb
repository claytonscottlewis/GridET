Public Class EarthDataLogin

#Region "Login"

    Friend EarthDataClient As EarthDataClient
    Friend Event EarthDataClientLoaded()
    Friend Event EarthDataClientCancelled()

    Private Sub OKButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OKButton.Click
        If Not (String.IsNullOrWhiteSpace(UsernameBox.Text) OrElse String.IsNullOrWhiteSpace(PasswordBox.Text)) Then
            ProgressText.Visible = True
            LoginGroup.Enabled = False
            OKButton.Enabled = False
            CancelButton.Enabled = False

            BackgroundWorker.RunWorkerAsync()
        End If
    End Sub

    Private Sub CancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CancelButton.Click
        RaiseEvent EarthDataClientCancelled()
    End Sub

#End Region

#Region "Background Execution"

    WithEvents BackgroundWorker As New System.ComponentModel.BackgroundWorker

    Private Sub BackgroundWorker_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker.DoWork
        EarthDataClient = New EarthDataClient(UsernameBox.Text, PasswordBox.Text)
    End Sub

    Private Sub BackgroundWorker_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker.RunWorkerCompleted
        RaiseEvent EarthDataClientLoaded()
    End Sub

#End Region

End Class
