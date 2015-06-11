'            Copyright Clayton S. Lewis 2014-2015.
'   Distributed under the Boost Software License, Version 1.0.
'      (See accompanying file GridET License.rtf or copy at
'            http://www.boost.org/LICENSE_1_0.txt)

Public Class Main

#Region "Program Initialization"

    Private ProcessSchedulerForm As Process_Scheduler = Nothing

    Private Sub Form1_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        'Remove Internet Download Connection Limit
        Net.ServicePointManager.DefaultConnectionLimit = Integer.MaxValue

        'Register GDAL
        Dim GDALPath As String = IO.Path.Combine(Application.StartupPath, "Dependencies\GDAL")
        Environment.SetEnvironmentVariable("PATH", GDALPath & ";" & Environment.GetEnvironmentVariable("PATH"))
        GDAL.Gdal.SetConfigOption("PATH", GDALPath)
        GDALProcess.SetEnvironmentVariable("GDAL_DATA", IO.Path.Combine(GDALPath, "Data"))
        GDALProcess.SetEnvironmentVariable("GDAL_DRIVER_PATH", IO.Path.Combine(GDALPath, "Plugins"))
        GDALProcess.SetEnvironmentVariable("PROJ_LIB", IO.Path.Combine(GDALPath, "Projections"))
        'GDALProcess.SetEnvironmentVariable("GDAL_CACHEMAX", 128)
        'GDALProcess.SetEnvironmentVariable("OGR_SQLITE_CACHE", 128)
        GDAL.Gdal.AllRegister()
        OGR.Ogr.RegisterAll()

        If IO.Directory.Exists(My.Settings.LastProjectDirectory) Then
            OpenProjectToolStripMenuItem_Click(Nothing, Nothing)
        Else
            SetFormLabel()
        End If
    End Sub

#End Region

#Region "Menu"

#Region "Project"

    Private Sub NewProjectToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles NewProjectToolStripMenuItem.Click
        Dim Form As New New_Project
        Form.ShowDialog()

        SetFormLabel()
    End Sub

    Private Sub OpenProjectToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles OpenProjectToolStripMenuItem.Click
        Dim FolderBrowserDialog As New FolderBrowserDialog
        FolderBrowserDialog.Description = "Open project location..."

        If sender IsNot Nothing Then
            If Not FolderBrowserDialog.ShowDialog = Windows.Forms.DialogResult.OK Then Exit Sub

            ProjectDirectory = FolderBrowserDialog.SelectedPath
            My.Settings.LastProjectDirectory = ProjectDirectory
        Else
            ProjectDirectory = My.Settings.LastProjectDirectory
        End If


        If Not IO.File.Exists(ProjectDetailsPath) Then
            MsgBox("Not a valid project directory.")
            Exit Sub
        Else
            Try
                'Open Database
                Using Connection = CreateConnection(ProjectDetailsPath, False)
                    Connection.Open()

                    Using Command = Connection.CreateCommand
                        Command.CommandText = "Select Directory FROM ClimateModel WHERE ROWID = 1"
                        ClimateModelDirectory = Command.ExecuteScalar
                    End Using
                End Using
            Catch
                MsgBox("Not a valid project directory.")
                Exit Sub
            End Try
        End If

        SetFormLabel()
    End Sub

    Private Sub CloseProjectToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles CloseProjectToolStripMenuItem.Click
        ProjectDirectory = ""
        ClimateModelDirectory = ""

        SetFormLabel()
    End Sub

    Private Sub SettingsToolStripMenuItem1_Click(sender As System.Object, e As System.EventArgs) Handles SettingsToolStripMenuItem1.Click
        Dim Form As New Settings
        Form.ShowDialog()
    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        Application.Exit()
    End Sub

    Private Sub SetFormLabel()
        Dim Text = "GridET"
        If Not ProjectDirectory = "" Then Text &= " - " & ProjectDirectory
        MapViewer.LoadProjectDirectories()
        Me.Text = Text
    End Sub

    Public Sub UpdateSettings()
        'Open Database
        Using Connection = CreateConnection(ProjectDetailsPath, False)
            Connection.Open()

            Using Command = Connection.CreateCommand
                Command.CommandText = "CREATE TABLE IF NOT EXISTS ClimateModel (Directory TEXT UNIQUE)"
                Command.ExecuteNonQuery()

                Command.CommandText = "SELECT COUNT(Directory) FROM ClimateModel"
                Dim Count As Integer = Command.ExecuteScalar

                If Count = 0 Then
                    Command.CommandText = "INSERT INTO ClimateModel VALUES (@Directory)"
                    Command.Parameters.Add("@Directory", DbType.String).Value = ClimateModelDirectory
                    Command.ExecuteNonQuery()
                Else
                    Command.CommandText = "UPDATE ClimateModel SET Directory = @Directory WHERE ROWID = 1"
                    Command.Parameters.Add("@Directory", DbType.String).Value = ClimateModelDirectory
                    Command.ExecuteNonQuery()
                End If
            End Using

        End Using
    End Sub

#End Region

#Region "Properties"

    Private Sub CoverToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles CoverToolStripMenuItem.Click
        Dim Form As New Cover_Properties
        Form.ShowDialog()
    End Sub

    Private Sub CurveToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles CurveToolStripMenuItem.Click
        Dim Form As New Cover_Curves
        Form.ShowDialog()
    End Sub

    Private Sub ImportToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ImportToolStripMenuItem.Click
        If IO.File.Exists(ProjectDetailsPath) Then
            Dim FolderBrowserDialog As New FolderBrowserDialog
            FolderBrowserDialog.Description = "Import cover and curve project location..."

            If Not FolderBrowserDialog.ShowDialog = Windows.Forms.DialogResult.OK Then Exit Sub

            Dim Path = IO.Path.Combine(FolderBrowserDialog.SelectedPath, "Project Details.db")

            If Not IO.File.Exists(Path) Then
                MsgBox("Not a valid project directory.")
                Exit Sub
            Else
                Try
                    'Open Database
                    Using Connection = CreateConnection(Path, False)
                        Connection.Open()

                        Using Command = Connection.CreateCommand
                            Command.CommandText = String.Format("ATTACH DATABASE '{0}' AS 'CurrentDatabase'", ProjectDetailsPath)
                            Command.ExecuteNonQuery()

                            Command.CommandText = "INSERT OR REPLACE INTO CurrentDatabase.Curve SELECT * FROM main.Curve; INSERT OR REPLACE INTO CurrentDatabase.Cover SELECT * FROM main.Cover"
                            Command.ExecuteNonQuery()
                        End Using
                    End Using

                    MsgBox("Import succeeded.")
                Catch
                    MsgBox("Not a valid project directory.")
                    Exit Sub
                End Try
            End If
        Else
            MsgBox("Please load or create a new project to import cover and curve properties.")
        End If
    End Sub

#End Region

#Region "Process"

    Private Sub DAYMETToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles DAYMETToolStripMenuItem.Click
        Dim Form As New Download_DAYMET
        Form.ShowDialog()
    End Sub

    Private Sub NLDASToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles NLDASToolStripMenuItem.Click
        Dim Form As New Download_NLDAS
        Form.ShowDialog()
    End Sub

    Private Sub ReferenceToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ReferenceToolStripMenuItem.Click
        Dim Form As New Calculate_Reference_Evapotranspiration
        Form.ShowDialog()
    End Sub

    Private Sub PotentialToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles PotentialToolStripMenuItem.Click
        Dim Form As New Calculate_Potential_Evapotranspiration
        Form.ShowDialog()
    End Sub

    Private Sub NetPotentialToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles NetPotentialToolStripMenuItem.Click
        Dim Form As New Calculate_Net_Potential_Evapotranspiration
        Form.ShowDialog()
    End Sub

    Private Sub RasterPeriodAverageToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles RasterPeriodAverageToolStripMenuItem.Click
        Dim Form As New Calculate_Raster_Period_Average
        Form.ShowDialog()
    End Sub

    Private Sub ExtractByPolygonToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ExtractByPolygonToolStripMenuItem.Click
        Dim Form As New Extract_by_Polygon
        Form.ShowDialog()
    End Sub

    Private Sub SchedulerToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles SchedulerToolStripMenuItem.Click
        If ProcessSchedulerForm IsNot Nothing Then
            If ProcessSchedulerForm.DialogResult = Windows.Forms.DialogResult.Cancel Then
                ProcessSchedulerForm = New Process_Scheduler
            End If
        Else
            ProcessSchedulerForm = New Process_Scheduler
        End If

        ProcessSchedulerForm.Show()
    End Sub

#End Region

#Region "Help"

    Private Sub ContentsToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ContentsToolStripMenuItem.Click
        Help.ShowHelp(Me, IO.Path.Combine(Application.StartupPath, "Help File\GridET.chm"), HelpNavigator.TableOfContents)
    End Sub

    Private Sub AboutToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles AboutToolStripMenuItem.Click
        Dim Form As New About
        Form.ShowDialog()
    End Sub

#End Region

#End Region

End Class
