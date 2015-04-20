﻿Public Class Main

#Region "Program Initialization"

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
        GDALProcess.SetEnvironmentVariable("GDAL_CACHEMAX", 128)
        GDALProcess.SetEnvironmentVariable("OGR_SQLITE_CACHE", 128)
        GDAL.Gdal.AllRegister()
        OGR.Ogr.RegisterAll()

        SetFormLabel()

        ProjectDirectory = "F:\Temporary Project\Test\" '"F:\UtahET 2.0\Utah-Third Mile"
        ClimateModelDirectory = "F:\UtahET 2.0\Data Sources\"
        SetFormLabel()

        Dim File = IO.Directory.GetFiles(PotentialEvapotranspirationDirectory, "*.db")(0)
        CalculatePeriodAverages({File}, EvapotranspirationTableName.Statistics, IO.Path.Combine(OutputCalculationsDirectory, "1980-2013"), 1980, 2013, Nothing, Nothing)

        'Dim SpatialReferenceSystem As OSR.SpatialReference
        'Using Raster As New Raster("C:\Users\Clayton Lewis\Desktop\Images\Mask.tif")
        '    SpatialReferenceSystem = New OSR.SpatialReference(Raster.Projection)
        'End Using

        'Dim Process As New GDALProcess
        'Dim f = Process.Ogr2Ogr("C:\Users\Clayton Lewis\Desktop\Utah Land Use\StateWideLandUse2013.shp", "C:\Users\Clayton Lewis\Desktop\Utah Land Use\StateWideLandUse2013_2.db", , SpatialReferenceSystem)
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

        If Not FolderBrowserDialog.ShowDialog = Windows.Forms.DialogResult.OK Then Exit Sub

        ProjectDirectory = FolderBrowserDialog.SelectedPath

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
            Catch
                MsgBox("Not a valid project directory.")
                Exit Sub
            End Try
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

    Private Sub PRISMToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles PRISMToolStripMenuItem.Click

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

    Private Sub ActualToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ActualToolStripMenuItem.Click

    End Sub

    Private Sub RasterPeriodAverageToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles RasterPeriodAverageToolStripMenuItem.Click

    End Sub

    Private Sub ExtractByPolygonToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ExtractByPolygonToolStripMenuItem.Click

    End Sub

    Private Sub RunAllToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles RunAllToolStripMenuItem.Click

    End Sub

#End Region

#End Region

    Sub GetRaster()
        Using Connection = CreateConnection(DAYMETPrecipitationPath)
            Connection.Open()

            Using Command = Connection.CreateCommand
                Dim OutputPath = IO.Path.Combine(FileIO.SpecialDirectories.Desktop, "DAYMET")
                Command.CommandText = "SELECT Annual FROM Statistics WHERE YEAR = '1985'"
                IO.File.WriteAllBytes(OutputPath, Command.ExecuteScalar)

                Dim OutputRaster As New Raster(OutputPath)
                OutputRaster.Open(GDAL.Access.GA_ReadOnly)
                Dim RealOutputRaster = CreateNewRaster(OutputPath & ".tif", OutputRaster.XCount, OutputRaster.YCount, OutputRaster.Projection, OutputRaster.GeoTransform, {Single.MinValue})
                RealOutputRaster.Open(GDAL.Access.GA_Update)

                Dim NoDataValue = OutputRaster.BandNoDataValue(0)

                Do Until OutputRaster.BlocksProcessed
                    Dim OPixels = OutputRaster.Read({1})

                    For I = 0 To OPixels.Length - 1
                        If OPixels(I) = NoDataValue Then OPixels(I) = Single.MinValue
                    Next

                    RealOutputRaster.Write({1}, OPixels)

                    RealOutputRaster.AdvanceBlock()
                    OutputRaster.AdvanceBlock()
                Loop

                OutputRaster.Dispose()
                RealOutputRaster.Dispose()
            End Using
        End Using
    End Sub

End Class

'Private Function CalculateShadows(Window8() As Single, ShadowConstants As ShadowConstants) As Single
'    ' First Slope ...
'    Dim X As Double = ((Window8(0) + Window8(3) + Window8(3) + Window8(6)) - (Window8(2) + Window8(5) + Window8(5) + Window8(8))) / ShadowConstants.Xresolution

'    Dim Y As Double = ((Window8(6) + Window8(7) + Window8(7) + Window8(8)) - (Window8(0) + Window8(1) + Window8(1) + Window8(2))) / ShadowConstants.Yresolution

'    Dim XX_Plus_YY As Double = X ^ 2 + Y ^ 2

'    ' ... then aspect...
'    Dim Aspect As Double = Math.Atan2(Y, X)
'    Dim Slope As Double = XX_Plus_YY * ShadowConstants.ScaleSquared

'    ' ... then the shade value
'    Dim Cangle As Double = Math.Acos((ShadowConstants.SinAltitude - ShadowConstants.CosAltitudeScaled * Math.Sqrt(XX_Plus_YY) * Math.Sin(Aspect - ShadowConstants.AzimuthInRadians)) / Math.Sqrt(1 + Slope))

'    ' combined shading
'    Cangle = 1 - Cangle * Math.Atan(Math.Sqrt(Slope)) / ShadowConstants.PiSquaredOver4

'    Return CSng(Cangle)
'End Function

'Private Function CalculateShadowData(GeoTransform() As Double, Elevation As Double, Altitude As Double, Azimuth As Double) As Object
'    Dim ShadowConstants As New ShadowConstants

'    ShadowConstants.Yresolution = GeoTransform(5)
'    ShadowConstants.Xresolution = GeoTransform(1)
'    ShadowConstants.AltitudeInRadians = ToRadians(Altitude)
'    ShadowConstants.AzimuthInRadians = ToRadians(Azimuth)
'    ShadowConstants.SinAltitude = Math.Sin(ShadowConstants.AltitudeInRadians)
'    Dim Scale As Double = Elevation / 8
'    ShadowConstants.CosAltitudeScaled = Math.Cos(ShadowConstants.AltitudeInRadians) * Scale
'    ShadowConstants.ScaleSquared = Scale ^ 2
'    ShadowConstants.PiSquaredOver4 = π ^ 2 / 4

'    Return ShadowConstants
'End Function

'Public Class ShadowConstants
'    Public Yresolution As Double
'    Public Xresolution As Double
'    Public SinAltitude As Double
'    Public CosAltitudeScaled As Double
'    Public AzimuthInRadians As Double
'    Public AltitudeInRadians As Double
'    Public ScaleSquared As Double
'    Public PiSquaredOver4 As Double
'End Class

'For Each File In IO.Directory.GetFiles("F:\NLDAS-2", "*", IO.SearchOption.AllDirectories)
'    Dim TableName = IO.Path.GetFileNameWithoutExtension(File).Replace("NLDAS_FORA0125_H.", "").Replace(".", "_")
'    ScaleRaster(File, "F:\Test.tif") '"Rasterlite:" & Directory & DataSources & "NLDAS_2A.db")
'Next


'Using Connection = CreateConnection("H:\Utah ET 2.0\NLDAS_2A.db")
'    Connection.Open()
'    Using Command = Connection.CreateCommand
'        Command.CommandText = "SELECT Image FROM Rasters WHERE Date = '1999-08-16 16:00:00'"
'        Dim Image = Command.ExecuteScalar
'    End Using
'End Using

'Sub LoadCovers()
'    'Dim Covers = IO.File.ReadAllLines("C:\Users\A00578752\Desktop\Covers.csv")
'    'Array.Sort(Covers)
'    'Dim Curves = IO.File.ReadAllLines("C:\Users\A00578752\Desktop\Curves.csv")
'    'Array.Sort(Curves)

'    'Dim CurveNames As New List(Of String)
'    'For Each Curve In Curves
'    '    CurveNames.Add(Curve.Split(",")(0))
'    'Next

'    'Dim CoverProperties As New List(Of CoverProperties)
'    'For Each Cover In Covers
'    '    Dim C = Cover.Split(",")

'    '    Dim Properties As New CoverProperties
'    '    Properties.Name = C(0)
'    '    Properties.InitiationThreshold = C(6)
'    '    Properties.InitiationThresholdType = Properties.ToThresholdType(C(5), C(4))
'    '    Properties.IntermediateThreshold = C(8)
'    '    Properties.IntermediateThresholdType = Properties.ToThresholdType(C(7), C(4))
'    '    Properties.TerminationThreshold = C(10)
'    '    Properties.TerminationThresholdType = Properties.ToThresholdType(C(9), C(4))
'    '    If C(11) = "" Then C(11) = -100
'    '    Properties.SpringFrostTemperature = C(11)
'    '    Properties.KillingFrostTemperature = C(12)

'    '    If C(3).Contains("1st") Then
'    '        Properties.IsCuttingType = True
'    '        ReDim Properties.InitialCurve(2)

'    '        Properties.CuttingIntermediateThreshold = C(14)
'    '        Properties.CuttingIntermediateThresholdType = Properties.ToThresholdType(C(13), C(4))
'    '        Properties.CuttingTerminationThreshold = C(16)
'    '        Properties.CuttingTerminationThresholdType = Properties.ToThresholdType(C(15), C(4))
'    '    End If

'    '    Properties.CurveName = C(3).Replace("1st ", "")
'    '    Dim I = CurveNames.IndexOf(C(3))
'    '    Dim Curve = Curves(I).Split(",")
'    '    Properties.InitiationToIntermediateCurveType = Properties.ToCurveType(Curve(1))
'    '    Properties.IntermediateToTermiationCurveType = Properties.ToCurveType(Curve(2))
'    '    Dim Values As New List(Of Single)
'    '    For J = 3 To Curve.Length - 1
'    '        If Not Curve(J) = "" Then Values.Add(Curve(J))
'    '    Next
'    '    Properties.InitialCurve(0) = Values.ToArray
'    '    If Properties.IsCuttingType Then
'    '        For K = 1 To 2
'    '            Curve = Curves(I + K).Split(",")
'    '            Values = New List(Of Single)
'    '            For J = 3 To Curve.Length - 1
'    '                If Not Curve(J) = "" Then Values.Add(Curve(J))
'    '            Next
'    '            Properties.InitialCurve(K) = Values.ToArray
'    '        Next
'    '    End If

'    '    CoverProperties.Add(Properties)
'    'Next

'    'Using Connection = CreateConnection( ProjectDetails, False)
'    '    Connection.Open()

'    '    Using Command = Connection.CreateCommand

'    '        For Each C In CoverProperties
'    '            'Properties.Append(C.Name & ",")
'    '            Dim Properties As New System.Text.StringBuilder
'    '            Properties.Append(C.CurveName & ";")
'    '            Properties.Append(C.InitiationThresholdType.ToString & ";")
'    '            Properties.Append(C.InitiationThreshold & ";")
'    '            Properties.Append(C.IntermediateThresholdType.ToString & ";")
'    '            Properties.Append(C.IntermediateThreshold & ";")
'    '            Properties.Append(C.TerminationThresholdType.ToString & ";")
'    '            Properties.Append(C.TerminationThreshold & ";")
'    '            Properties.Append(C.CuttingIntermediateThresholdType.ToString & ";")
'    '            Properties.Append(C.CuttingTerminationThresholdType.ToString & ";")
'    '            Properties.Append(C.CuttingIntermediateThreshold & ";")
'    '            Properties.Append(C.CuttingTerminationThreshold & ";")
'    '            Properties.Append(C.SpringFrostTemperature & ";")
'    '            Properties.Append(C.KillingFrostTemperature)

'    '            Command.CommandText = "INSERT OR IGNORE INTO Cover VALUES(@Name, @Properties)"
'    '            Command.Parameters.Add("@Name", DbType.String).Value = C.Name
'    '            Command.Parameters.Add("@Properties", DbType.String).Value = Properties.ToString
'    '            Command.ExecuteNonQuery()
'    '        Next

'    '        For C = 0 To Curves.Length - 1
'    '            Dim Curve1 = Curves(C).Split(",")
'    '            Dim Curve2() As String = Nothing
'    '            Dim Curve3() As String = Nothing
'    '            Dim P As New CoverProperties

'    '            Dim Properties As New System.Text.StringBuilder
'    '            Properties.Append("Reference Evapotranspiration ASCE NLDAS_2A;")
'    '            Dim HasCuttings As Boolean = Curve1(0).Contains("1st")
'    '            If HasCuttings Then
'    '                Curve2 = Curves(C + 1).Split(",")
'    '                Curve3 = Curves(C + 2).Split(",")
'    '                Properties.Append(SeasonalCurveType.Has_Cuttings.ToString & ";")
'    '            Else
'    '                Properties.Append(SeasonalCurveType.Full_Season.ToString & ";")
'    '            End If
'    '            Properties.Append(P.ToCurveType(Curve1(1)).ToString & ";")

'    '            For I = 3 To 13
'    '                Properties.Append(Curve1(I) & ",")
'    '                If HasCuttings Then
'    '                    Properties.Append(Curve2(I) & s"," & Curve3(I) & ",")
'    '                Else
'    '                    Properties.Append("0,0,")
'    '                End If
'    '            Next
'    '            Properties.Remove(Properties.Length - 1, 1)

'    '            Properties.Append(";" & P.ToCurveType(Curve1(2)).ToString & ";")

'    '            For I = 14 To Curve1.Length - 1
'    '                If Curve1(I) = "" Then Exit For
'    '                Properties.Append(Curve1(I) & ",")
'    '                If HasCuttings Then
'    '                    Properties.Append(Curve2(I) & "," & Curve3(I) & ",")
'    '                Else
'    '                    Properties.Append("0,0,")
'    '                End If
'    '            Next
'    '            Properties.Remove(Properties.Length - 1, 1)

'    '            Command.CommandText = "INSERT OR IGNORE INTO Curve VALUES(@Name, @Properties)"
'    '            Command.Parameters.Add("@Name", DbType.String).Value = Curve1(0).Replace("1st ", "")
'    '            Command.Parameters.Add("@Properties", DbType.String).Value = Properties.ToString
'    '            Command.ExecuteNonQuery()

'    '            If HasCuttings Then C += 2
'    '        Next
'    '    End Using

'    'End Using
'End Sub