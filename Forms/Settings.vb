'            Copyright Clayton S. Lewis 2014-2018.
'   Distributed under the Boost Software License, Version 1.0.
'      (See accompanying file GridET License.rtf or copy at
'            http://www.boost.org/LICENSE_1_0.txt)

Class Settings

#Region "User Input"

    Private Sub Settings_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        For Each Control In Me.Controls
            GetType(Control).InvokeMember("DoubleBuffered", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance Or Reflection.BindingFlags.SetProperty, Nothing, Control, New Object() {True})
        Next

        If IO.File.Exists(ProjectDetailsPath) Then
            ClimateModelBox.Text = Global_Variables.ClimateModelDirectory
        Else
            OK_Button.Enabled = False
            FileLocationsGroup.Enabled = False
        End If
    End Sub

    Private Sub ClimateModelSet_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ClimateModelSet.Click
        Dim FolderBrowserDialog As New FolderBrowserDialog
        FolderBrowserDialog.Description = "Set climate model location as..."

        If Not FolderBrowserDialog.ShowDialog = Windows.Forms.DialogResult.OK Then Exit Sub

        ClimateModelBox.Text = FolderBrowserDialog.SelectedPath
    End Sub

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        If Not IO.Directory.Exists(ClimateModelBox.Text) Then
            MsgBox("Please specify a valid directory.")
            Exit Sub
        End If

        ClimateModelDirectory = ClimateModelBox.Text

        Using Connection = CreateConnection(ProjectDetailsPath, False), Command = Connection.CreateCommand : Connection.Open()

            Command.CommandText = "UPDATE OR IGNORE Settings SET Value = @Directory WHERE Name = 'Climate Model Directory'"
            Command.Parameters.Add("@Directory", DbType.String).Value = ClimateModelDirectory
            Command.ExecuteNonQuery()

        End Using

        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

#End Region

End Class
