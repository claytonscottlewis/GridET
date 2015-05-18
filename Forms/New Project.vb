Public Class New_Project

#Region "User Input"

    Private Sub New_Project_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        For Each Control In Me.Controls
            GetType(Control).InvokeMember("DoubleBuffered", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance Or Reflection.BindingFlags.SetProperty, Nothing, Control, New Object() {True})
        Next
    End Sub

    Private Sub Form_Closing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = BackgroundWorker.IsBusy
        Cancel_Button_Click(Nothing, Nothing)
    End Sub

    Private Sub ProjectSet_Click(sender As System.Object, e As System.EventArgs) Handles ProjectSet.Click
        Dim FolderBrowserDialog As New FolderBrowserDialog
        FolderBrowserDialog.Description = "Set project location as..."

        If Not FolderBrowserDialog.ShowDialog = Windows.Forms.DialogResult.OK Then Exit Sub

        ProjectDirectory.Text = FolderBrowserDialog.SelectedPath
    End Sub

    Private Sub ClimateModelSet_Click(sender As System.Object, e As System.EventArgs) Handles ClimateModelSet.Click
        Dim FolderBrowserDialog As New FolderBrowserDialog
        FolderBrowserDialog.Description = "Set climate model location as..."

        If Not FolderBrowserDialog.ShowDialog = Windows.Forms.DialogResult.OK Then Exit Sub

        ClimateModelDirectory.Text = FolderBrowserDialog.SelectedPath
    End Sub

    Private Sub MaskAdd_Click(sender As System.Object, e As System.EventArgs) Handles MaskAdd.Click
        Dim OpenFileDialog As New OpenFileDialog
        OpenFileDialog.Title = "Location of mask vector file."
        OpenFileDialog.Multiselect = False

        If Not OpenFileDialog.ShowDialog = Windows.Forms.DialogResult.OK Then Exit Sub

        Try
            Using Datasource = OGR.Ogr.OpenShared(OpenFileDialog.FileName, False)
                If Datasource Is Nothing Then
                    MsgBox(String.Format("'{0}' not recognized as a supported file format.", OpenFileDialog.FileName))
                    MaskDatasetPath.Text = ""
                Else
                    ResolutionLabel.Text = String.Format("Project Raster Resolution ({0})", Datasource.GetLayerByIndex(0).GetSpatialRef.GetLinearUnitsName)
                    MaskDatasetPath.Text = OpenFileDialog.FileName
                End If
            End Using
        Catch
            MsgBox(String.Format("'{0}' not recognized as a supported file format.", OpenFileDialog.FileName))
            MaskDatasetPath.Text = ""
        End Try
    End Sub

    Private Sub ElevationAdd_Click(sender As System.Object, e As System.EventArgs) Handles ElevationAdd.Click
        Dim OpenFileDialog As New OpenFileDialog
        OpenFileDialog.Title = "Location of digital elevation model file(s)."
        OpenFileDialog.Multiselect = True

        If Not OpenFileDialog.ShowDialog = Windows.Forms.DialogResult.OK Then Exit Sub

        Dim UserSelectedUnsupportedFiles As Boolean = False
        For Each FileName In OpenFileDialog.FileNames
            Try
                Using DataSet = GDAL.Gdal.OpenShared(FileName, GDAL.Access.GA_ReadOnly)
                End Using
            Catch Exception As Exception
                MsgBox(Exception.Message)
                UserSelectedUnsupportedFiles = True
            End Try
        Next

        If UserSelectedUnsupportedFiles Then
            ElevationDatasetPaths.Text = ""
        Else
            ElevationDatasetPaths.Text = String.Join(";", OpenFileDialog.FileNames)
        End If
    End Sub

    Private Sub CreateButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CreateButton.Click
        If Cancel_Button.Enabled = False Then
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()
            Exit Sub
        End If

        If ProjectDirectory.Text = "" Or ClimateModelDirectory.Text = "" Or MaskDatasetPath.Text = "" Or ElevationDatasetPaths.Text = "" Then
            MsgBox("Please specify a path(s) for each project creation input.")
            Exit Sub
        End If

        If Not BackgroundWorker.IsBusy Then
            BackgroundWorker.WorkerReportsProgress = True
            BackgroundWorker.WorkerSupportsCancellation = True

            Global_Variables.ProjectDirectory = ProjectDirectory.Text
            Global_Variables.ClimateModelDirectory = ClimateModelDirectory.Text

            FileLocationsGroup.Enabled = False
            AreaOfInterestGroup.Enabled = False
            CreateButton.Enabled = False

            ProgressText.Text = ""
            ProgressBar.Value = 0
            ProgressText.Visible = True
            ProgressBar.Visible = True

            Timer = New Stopwatch
            Timer.Start()

            BackgroundWorker.RunWorkerAsync()
        End If
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        If BackgroundWorker.IsBusy Then
            If BackgroundWorker.WorkerSupportsCancellation = True Then
                ProgressText.Text = "Attempting to cancel..."
                BackgroundWorker.CancelAsync()
                If Not GDALProcess Is Nothing Then If Not GDALProcess.Process.HasExited Then GDALProcess.Process.Kill()
                Cancel_Button.Enabled = False
            End If
        Else
            Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
            RemoveHandler Me.FormClosing, AddressOf Form_Closing
            Me.Close()
        End If
    End Sub

#End Region

#Region "Background Execution"

    WithEvents BackgroundWorker As New System.ComponentModel.BackgroundWorker
    Private Timer As Stopwatch
    Private GDALProcess As GDALProcess

    Private Sub BackgroundWorker_DoWork(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker.DoWork
        InitializeProjectAreas(MaskDatasetPath.Text, Resolution.Value, ElevationDatasetPaths.Text.Split(";"), Scaling.Value, sender, e, GDALProcess)
    End Sub

    Private Sub BackgroundWorker_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorker.ProgressChanged
        ProgressBar.Value = e.ProgressPercentage
        ProgressText.Text = e.UserState
    End Sub

    Private Sub BackgroundWorker_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker.RunWorkerCompleted
        If e.Cancelled Then
            ProgressText.Text = "Project creation cancelled"
        ElseIf e.Error IsNot Nothing Then
            ProgressText.Text = "There was an error in trying to create the project"
        Else
            Timer.Stop()
            ProgressText.Text = "Project datasets successfully were built"
        End If
        ProgressText.Text &= String.Format(" ({0}).", Timer.Elapsed.ToString())

        CreateButton.Enabled = True
        CreateButton.Text = "OK"
        Cancel_Button.Enabled = False
    End Sub

#End Region

End Class
