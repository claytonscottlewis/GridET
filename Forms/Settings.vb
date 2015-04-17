Public Class Settings

#Region "User Input"

    Private Sub Settings_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        For Each Control In Me.Controls
            GetType(Control).InvokeMember("DoubleBuffered", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance Or Reflection.BindingFlags.SetProperty, Nothing, Control, New Object() {True})
        Next

        If IO.File.Exists(ProjectDetailsPath) Then
            ClimateModelDirectory.Text = Global_Variables.ClimateModelDirectory
        Else
            OK_Button.Enabled = False
            FileLocationsGroup.Enabled = False
        End If
    End Sub

    Private Sub ClimateModelSet_Click(sender As System.Object, e As System.EventArgs) Handles ClimateModelSet.Click
        Dim FolderBrowserDialog As New FolderBrowserDialog
        FolderBrowserDialog.Description = "Set climate model location as..."

        If Not FolderBrowserDialog.ShowDialog = Windows.Forms.DialogResult.OK Then Exit Sub

        ClimateModelDirectory.Text = FixDirectory(FolderBrowserDialog.SelectedPath)
    End Sub

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        If Not IO.Directory.Exists(ClimateModelDirectory.Text) Then
            MsgBox("Please specify a valid directory.")
            Exit Sub
        End If

        Global_Variables.ClimateModelDirectory = ClimateModelDirectory.Text

        Main.UpdateSettings()

        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

#End Region

End Class
