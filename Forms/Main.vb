'            Copyright Clayton S. Lewis 2014-2018.
'   Distributed under the Boost Software License, Version 1.0.
'      (See accompanying file GridET License.rtf or copy at
'            http://www.boost.org/LICENSE_1_0.txt)

Public Class Main

#Region "Program Initialization"

    Private ProcessSchedulerForm As Process_Scheduler = Nothing

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Remove Internet Download Connection Limit
        Net.ServicePointManager.DefaultConnectionLimit = Integer.MaxValue

        'Register GDAL
        Dim GDALPath As String = IO.Path.Combine(Application.StartupPath, "GDAL")
        Environment.SetEnvironmentVariable("PATH", GDALPath & ";" & Environment.GetEnvironmentVariable("PATH"))
        GDAL.Gdal.SetConfigOption("PATH", GDALPath)
        GDALProcess.SetEnvironmentVariable("GDAL_DATA", IO.Path.Combine(GDALPath, "gdal-data"))
        GDALProcess.SetEnvironmentVariable("GDAL_DRIVER_PATH", IO.Path.Combine(GDALPath, "gdal", "plugins"))
        GDALProcess.SetEnvironmentVariable("PROJ_LIB", IO.Path.Combine(GDALPath, "proj"))
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

    Private Sub NewProjectToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewProjectToolStripMenuItem.Click
        Dim Form As New New_Project
        Form.ShowDialog()

        SetFormLabel()
    End Sub

    Private Sub OpenProjectToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenProjectToolStripMenuItem.Click
        If sender IsNot Nothing Then
            Dim FolderBrowserDialog As New FolderBrowserDialog
            FolderBrowserDialog.Description = "Open project location..."

            If Not FolderBrowserDialog.ShowDialog = Windows.Forms.DialogResult.OK Then Exit Sub

            My.Settings.LastProjectDirectory = FolderBrowserDialog.SelectedPath
        End If

        If Not IO.File.Exists(ProjectDetailsPath) Then
            MsgBox("Not a valid project directory.")
            Exit Sub
        Else
            Try
                'Open Database
                Using Connection = CreateConnection(ProjectDetailsPath), Command = Connection.CreateCommand : Connection.Open()

                    Command.CommandText = "SELECT * FROM Settings WHERE Name IN ('Climate Model Directory', 'Pixel Count')"
                    Using Reader = Command.ExecuteReader
                        While Reader.Read
                            Select Case Reader.GetString(0)
                                Case "Climate Model Directory" : ClimateModelDirectory = Reader.GetString(1)
                                Case "Pixel Count" : PixelCount = Reader.GetInt64(1)
                            End Select
                        End While
                    End Using

                End Using

                Using Raster As New Raster(MaskRasterPath, GDAL.Access.GA_ReadOnly)

                    ReDim ProjectMask(Raster.XCount * Raster.YCount - 1)
                    Raster.Dataset.ReadRaster(0, 0, Raster.XCount, Raster.YCount, ProjectMask, Raster.XCount, Raster.YCount, 1, {1}, 0, 0, 0)

                    ProjectProjection = Raster.Projection
                    ProjectGeoTransform = Raster.GeoTransform
                    ProjectExtent = Raster.Extent
                    ProjectXCount = Raster.XCount
                    ProjectYCount = Raster.YCount

                End Using
            Catch
                MsgBox("Not a valid project directory.")
                Exit Sub
            End Try
        End If

        If Not IO.File.Exists(ClimateModelDirectory) Then
            Dim Climate = IO.Path.GetFileName(ClimateModelDirectory)

            Dim LastDirectory = IO.Path.GetDirectoryName(ProjectDirectory)
            While Not String.IsNullOrWhiteSpace(LastDirectory)
                Dim Directory = IO.Path.Combine(LastDirectory, Climate)

                If IO.Directory.Exists(Directory) Then
                    ClimateModelDirectory = Directory
                    Exit While
                End If

                LastDirectory = IO.Path.GetDirectoryName(LastDirectory)
            End While
        End If

        SetFormLabel()
    End Sub

    Private Sub CloseProjectToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CloseProjectToolStripMenuItem.Click
        My.Settings.LastProjectDirectory = ""
        ClimateModelDirectory = ""

        SetFormLabel()
    End Sub

    Private Sub SettingsToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SettingsToolStripMenuItem1.Click
        Dim Form As New Settings
        Form.ShowDialog()
    End Sub

    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        Application.Exit()
    End Sub

    Private Sub SetFormLabel()
        Dim Text = "GridET"
        If Not ProjectDirectory = "" Then Text &= " - " & ProjectDirectory
        MapViewer.LoadProjectDirectories()
        Me.Text = Text
    End Sub

#End Region

#Region "Properties"

    Private Sub CoverToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CoverToolStripMenuItem.Click
        Dim Form As New Cover_Properties
        Form.ShowDialog()
    End Sub

    Private Sub CurveToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CurveToolStripMenuItem.Click
        Dim Form As New Cover_Curves
        Form.ShowDialog()
    End Sub

    Private Sub ImportToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ImportToolStripMenuItem.Click
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

    Private Sub DAYMETToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DAYMETToolStripMenuItem.Click
        If Not IO.Directory.Exists(ClimateModelDirectory) Then
            MsgBox("Cannot find climate model directory.")
        Else
            Dim Form As New Download_DAYMET
            Form.ShowDialog()
        End If
    End Sub

    Private Sub NLDASToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NLDASToolStripMenuItem.Click
        If Not IO.Directory.Exists(ClimateModelDirectory) Then
            MsgBox("Cannot find climate model directory.")
        Else
            Dim Form As New Download_NLDAS
            Form.ShowDialog()
        End If
    End Sub

    Private Sub ReferenceToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReferenceToolStripMenuItem.Click
        If Not IO.Directory.Exists(ClimateModelDirectory) Then
            MsgBox("Cannot find climate model directory.")
        ElseIf Not String.IsNullOrWhiteSpace(ProjectDetailsPath) AndAlso Not IO.File.Exists(ProjectDetailsPath) Then
            MsgBox("Cannot find project details path.")
        Else
            Dim Form As New Calculate_Reference_Evapotranspiration
            Form.ShowDialog()
        End If
    End Sub

    Private Sub PotentialToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PotentialToolStripMenuItem.Click
        If Not IO.File.Exists(ProjectDetailsPath) Then
            MsgBox("Cannot find project details path.")
        Else
            Dim Form As New Calculate_Potential_Evapotranspiration
            Form.ShowDialog()
        End If
    End Sub

    Private Sub NetPotentialToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NetPotentialToolStripMenuItem.Click
        If Not IO.File.Exists(ProjectDetailsPath) Then
            MsgBox("Cannot find project details path.")
        Else
            Dim Form As New Calculate_Net_Potential_Evapotranspiration
            Form.ShowDialog()
        End If
    End Sub

    Private Sub RasterPeriodAverageToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RasterPeriodAverageToolStripMenuItem.Click
        If Not IO.Directory.Exists(IntermediateCalculationsDirectory) Then
            MsgBox("Cannot find project directory.")
        Else
            Dim Form As New Calculate_Raster_Period_Average
            Form.ShowDialog()
        End If
    End Sub

    Private Sub ExtractByPolygonToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExtractByPolygonToolStripMenuItem.Click
        If Not IO.File.Exists(ProjectDetailsPath) Then
            MsgBox("Cannot find project details path.")
        Else
            Dim Form As New Extract_by_Polygon
            Form.ShowDialog()
        End If
    End Sub

    Private Sub SchedulerToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SchedulerToolStripMenuItem.Click
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

    Private Sub ContentsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ContentsToolStripMenuItem.Click
        Static Form As Help = Nothing

        If Form Is Nothing OrElse Form.IsDisposed Then
            Form = New Help
            Form.LoadFile(IO.Path.Combine(Application.StartupPath, "Help File\GridET Help.html"))
        End If

        Form.Show()
        Form.Focus()
    End Sub

    Private Sub AboutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutToolStripMenuItem.Click
        Dim Form As New About
        Form.ShowDialog()
    End Sub

#End Region

#End Region

End Class
