Module Calculations

#Region "Functions"

#Region "Major Processes"

    Sub InitializeProjectAreas(AreaPolygonPath As String, ProjectRasterResolution As Double, ElevationTilePaths() As String, VerticalUnitScaling As Double, BackgroundWorker As System.ComponentModel.BackgroundWorker, DoWorkEvent As System.ComponentModel.DoWorkEventArgs, ByRef GDALProcess As GDALProcess)
        Try
            If BackgroundWorker.CancellationPending Then : DoWorkEvent.Cancel = True : Exit Sub
            Else : BackgroundWorker.ReportProgress(0, "Rasterizing mask area...") : End If

            'Create Project Sub Directories
            For Each Directory In {CoordinateDirectory, ElevationDirectory, MaskDirectory, InputVariablesDirectory, ReferenceEvapotranspirationDirectory, DateVariablesDirectory, PotentialEvapotranspirationDirectory, OutputCalculationsDirectory, ClimateModelDirectory}
                If Not IO.Directory.Exists(Directory) Then IO.Directory.CreateDirectory(Directory)
            Next

            'Create Template, Snap, or Mask Raster to Represent Project Area
            GDALProcess = New GDALProcess
            GDALProcess.Rasterize(AreaPolygonPath, MaskRasterPath, ProjectRasterResolution, , , 1, 0)

            'Extract Mask Raster Projection, Determine Linear Unit, and Write in Text File
            Using Raster = GDAL.Gdal.Open(MaskRasterPath, GDAL.Access.GA_ReadOnly)
                IO.File.WriteAllText(MaskProjectionPath, Raster.GetProjection)
            End Using

            'Add Mask Raster Pixel Centers to Point Vector File
            RasterPixelsToPoints(MaskRasterPath, MaskPointsPath)

            If BackgroundWorker.CancellationPending Then : DoWorkEvent.Cancel = True : Exit Sub
            Else : BackgroundWorker.ReportProgress(1, "Calculating latitude and longitude raster maps...") : End If

            'Calculate Latitude and Longitude for Mask Raster Pixel Centers
            CreateCoordinateRasters(MaskRasterPath, MaskLatitudeRasterPath, MaskLongitudeRasterPath, , {"COMPRESS=DEFLATE"})

            If BackgroundWorker.CancellationPending Then : DoWorkEvent.Cancel = True : Exit Sub
            Else : BackgroundWorker.ReportProgress(2, "Calculating average elevation in each pixel (this process may take a while)...") : End If

            'Calculate Average Elevation of Pixels in Mask Raster
            Dim OriginalResolutionElevationRaster As String = IO.Path.Combine(ElevationDirectory, "Original Resolution Elevation.tif")
            MergeRasters(ElevationTilePaths, MaskProjectionPath, OriginalResolutionElevationRaster, , , , , VerticalUnitScaling, , GDALProcess)
            Dim ProjectResolutionElevationRaster As String = IO.Path.Combine(ElevationDirectory & "Project Resolution Elevation.tif")
            SnapToRaster(OriginalResolutionElevationRaster, MaskRasterPath, ProjectResolutionElevationRaster, False, False, , , , , , GDAL.DataType.GDT_Float32, GDALProcess)
            SnapToRaster(ProjectResolutionElevationRaster, MaskRasterPath, MaskElevationRasterPath, , , , , , , , GDAL.DataType.GDT_Float32, GDALProcess)

            If BackgroundWorker.CancellationPending Then : DoWorkEvent.Cancel = True : Exit Sub
            Else : BackgroundWorker.ReportProgress(3, "Calculating pixel slope...") : End If

            'Calculate Average Slope of Pixels and Slope Multiplier in Mask Raster
            GDALProcess = New GDALProcess
            Dim IntermediateSlopeRaster As String = MaskSlopeRasterPath & ".temp"
            GDALProcess.DEM(ProjectResolutionElevationRaster, IntermediateSlopeRaster, GDALProcess.DEMDerivative.Slope)
            SnapToRaster(IntermediateSlopeRaster, MaskRasterPath, MaskSlopeRasterPath, , , , , , , , GDAL.DataType.GDT_Float32, GDALProcess)
            GDALProcess.DeleteRaster(IntermediateSlopeRaster)

            Using SlopeRaster As New Raster(MaskSlopeRasterPath)
                SlopeRaster.Open(GDAL.Access.GA_ReadOnly)

                Dim SlopeAreaRaster = CreateNewRaster(MaskSlopeAreaRasterPath, SlopeRaster.XCount, SlopeRaster.YCount, SlopeRaster.Projection, SlopeRaster.GeoTransform, {-9999}, GDAL.DataType.GDT_Float32, {"COMPRESS=DEFLATE"})
                SlopeAreaRaster.Open(GDAL.Access.GA_Update)

                Do Until SlopeRaster.BlocksProcessed
                    Dim SlopePixels = SlopeRaster.Read({1})

                    Dim NoDataValue = SlopeRaster.BandNoDataValue(0)

                    For I = 0 To SlopePixels.Length - 1
                        If SlopePixels(I) = NoDataValue Then
                            SlopePixels(I) = -9999
                        Else
                            SlopePixels(I) = 1 / Math.Cos(ToRadians(SlopePixels(I)))
                        End If
                    Next

                    SlopeAreaRaster.Write({1}, SlopePixels)

                    SlopeAreaRaster.AdvanceBlock()
                    SlopeRaster.AdvanceBlock()
                Loop

                SlopeAreaRaster.Close()
            End Using

            If BackgroundWorker.CancellationPending Then : DoWorkEvent.Cancel = True : Exit Sub
            Else : BackgroundWorker.ReportProgress(4, "Calculating pixel aspect...") : End If

            'Calculate Average Aspect of Pixels in Mask Raster
            Dim IntermediateAspectRaster As String = MaskAspectRasterPath & ".temp"
            GDALProcess.DEM(ProjectResolutionElevationRaster, IntermediateAspectRaster, GDALProcess.DEMDerivative.Aspect)
            SnapToRaster(IntermediateAspectRaster, MaskRasterPath, MaskAspectRasterPath, , , , , , , , GDAL.DataType.GDT_Float32, GDALProcess)
            GDALProcess.DeleteRaster(IntermediateAspectRaster)

            GDALProcess.DeleteRaster(OriginalResolutionElevationRaster)
            GDALProcess.DeleteRaster(ProjectResolutionElevationRaster)

            If BackgroundWorker.CancellationPending Then : DoWorkEvent.Cancel = True : Exit Sub
            Else : BackgroundWorker.ReportProgress(5, "...") : End If

            'Download NLDAS-2A Elevation, Slope, and Aspect Rasters
            DownloadNLDAS_2ATopography()

            Main.UpdateSettings()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Sub CalculateReferenceEvapotranspirationNLDAS_2A(MinDate As DateTime, MaxDate As DateTime, BackgroundWorker As System.ComponentModel.BackgroundWorker, DoWorkEvent As System.ComponentModel.DoWorkEventArgs)
        Try
            'Open Mask Coordinate Rasters and Get Extent (In Degrees)
            Dim LongitudeRaster As New Raster(MaskLongitudeRasterPath)
            LongitudeRaster.Open(GDAL.Access.GA_ReadOnly)

            Dim XMinMax(1) As Double
            Dim LongitudeNoDataValue As Double
            Using Band = LongitudeRaster.Dataset.GetRasterBand(1)
                Band.ComputeRasterMinMax(XMinMax, False)
                Band.GetNoDataValue(LongitudeNoDataValue, False)
            End Using

            Dim LatitudeRaster As New Raster(MaskLatitudeRasterPath)
            LatitudeRaster.Open(GDAL.Access.GA_ReadOnly)

            Dim YMinMax(1) As Double
            Using Band = LatitudeRaster.Dataset.GetRasterBand(1)
                Band.ComputeRasterMinMax(YMinMax, False)
            End Using

            'Open NLDAS-2A Topographical Rasters and Get Masked Extent
            Dim NLDAS_2AElevationRaster As New Raster(NLDAS_2AElevationRasterPath)
            Dim NLDAS_2ASlopeRaster As New Raster(NLDAS_2ASlopeRasterPath)
            Dim NLDAS_2AAspectRaster As New Raster(NLDAS_2AAspectRasterPath)
            NLDAS_2AElevationRaster.Open(GDAL.Access.GA_ReadOnly)
            NLDAS_2ASlopeRaster.Open(GDAL.Access.GA_ReadOnly)
            NLDAS_2AAspectRaster.Open(GDAL.Access.GA_ReadOnly)

            Dim NLDAS_2ANoDataValue As Double
            Using Band = NLDAS_2AElevationRaster.Dataset.GetRasterBand(1)
                Band.GetNoDataValue(NLDAS_2ANoDataValue, False)
            End Using

            Dim Extent = GetMaskedExtent(New Extent(XMinMax(0) - NLDAS_2AElevationRaster.XResolutionHalf, XMinMax(1) + NLDAS_2AElevationRaster.XResolutionHalf, YMinMax(0) - NLDAS_2AElevationRaster.YResolutionHalf, YMinMax(1) + NLDAS_2AElevationRaster.YResolutionHalf), NLDAS_2AElevationRaster)
            Dim NLDAS_2AXOffset As Integer = (Extent.Xmin - NLDAS_2AElevationRaster.Extent.Xmin) / NLDAS_2AElevationRaster.XResolution
            Dim NLDAS_2AYOffset As Integer = (NLDAS_2AElevationRaster.Extent.Ymax - Extent.Ymax) / NLDAS_2AElevationRaster.YResolution
            Dim NLDAS_2AXSize As Integer = (Extent.Xmax - Extent.Xmin) / NLDAS_2AElevationRaster.XResolution
            Dim NLDAS_2AYSize As Integer = (Extent.Ymax - Extent.Ymin) / NLDAS_2AElevationRaster.YResolution

            'Load NLDAS-2A Topographical Raster Values within Masked Extent
            Dim NLDAS_2ATopographyPixels(2)() As Single
            Dim NLDAS_2ALength = NLDAS_2AXSize * NLDAS_2AYSize - 1
            For I = 0 To 2
                ReDim NLDAS_2ATopographyPixels(I)(NLDAS_2ALength)
            Next
            NLDAS_2AElevationRaster.Dataset.ReadRaster(NLDAS_2AXOffset, NLDAS_2AYOffset, NLDAS_2AXSize, NLDAS_2AYSize, NLDAS_2ATopographyPixels(0), NLDAS_2AXSize, NLDAS_2AYSize, 1, {1}, 0, 0, 0)
            NLDAS_2ASlopeRaster.Dataset.ReadRaster(NLDAS_2AXOffset, NLDAS_2AYOffset, NLDAS_2AXSize, NLDAS_2AYSize, NLDAS_2ATopographyPixels(1), NLDAS_2AXSize, NLDAS_2AYSize, 1, {1}, 0, 0, 0)
            NLDAS_2AAspectRaster.Dataset.ReadRaster(NLDAS_2AXOffset, NLDAS_2AYOffset, NLDAS_2AXSize, NLDAS_2AYSize, NLDAS_2ATopographyPixels(2), NLDAS_2AXSize, NLDAS_2AYSize, 1, {1}, 0, 0, 0)

            NLDAS_2AElevationRaster.Close()
            NLDAS_2ASlopeRaster.Close()
            NLDAS_2AAspectRaster.Close()

            Dim ReducedNLDAS_2ARaster As New Raster
            ReducedNLDAS_2ARaster = NLDAS_2AElevationRaster
            ReducedNLDAS_2ARaster.Extent = Extent
            ReducedNLDAS_2ARaster.GeoTransform(0) = Extent.Xmin
            ReducedNLDAS_2ARaster.GeoTransform(3) = Extent.Ymax
            GDAL.Gdal.InvGeoTransform(ReducedNLDAS_2ARaster.GeoTransform, ReducedNLDAS_2ARaster.InverseGeoTransform)
            ReducedNLDAS_2ARaster.XCount = NLDAS_2AXSize
            ReducedNLDAS_2ARaster.YCount = NLDAS_2AYSize

            'Open Project Topographical Rasters
            Dim ElevationRaster As New Raster(MaskElevationRasterPath)
            Dim SlopeRaster As New Raster(MaskSlopeRasterPath)
            Dim AspectRaster As New Raster(MaskAspectRasterPath)
            Dim InputRasters = {LongitudeRaster, LatitudeRaster, ElevationRaster, SlopeRaster, AspectRaster}

            'Create SQLite Raster Storage Database If It Does Not Already Exist
            Dim IntermediateCalculationPaths = {NLDAS_2AMeanAirTemperaturePath, _
                                                 NLDAS_2AMaximumAirTemperaturePath, _
                                                 NLDAS_2AMinimumAirTemperaturePath, _
                                                 NLDAS_2ADewpointTemperaturePath, _
                                                 NLDAS_2ASolarRadiationPath, _
                                                 NLDAS_2AWindSpeedPath, _
                                                 NLDAS_2APrecipitationPath, _
                                                 NLDAS_2AEvapotranspirationASCEPath, _
                                                 NLDAS_2AEvapotranspirationHargreavesPath, _
                                                 NLDAS_2AEvapotranspirationAerodynamicPath, _
                                                 NLDAS_2AGrowingDegreeDays32Path, _
                                                 NLDAS_2AGrowingDegreeDays41Path, _
                                                 NLDAS_2AGrowingDegreeDays8650Path}

            Dim Statistics = {StatisticType.Average, _
                              StatisticType.Average, _
                              StatisticType.Average, _
                              StatisticType.Average, _
                              StatisticType.Average, _
                              StatisticType.Average, _
                              StatisticType.Sum, _
                              StatisticType.Sum, _
                              StatisticType.Sum, _
                              StatisticType.Sum, _
                              StatisticType.Sum, _
                              StatisticType.Sum, _
                              StatisticType.Sum}

            'Open Databases
            Dim ConnectionRasters = CreateConnection(NLDAS_2ARastersPath)
            ConnectionRasters.Open()

            Dim IWeather As Integer = 6
            Dim IEvapotranspiration As Integer = 2
            Dim IGrowingDegreeDays As Integer = 2

            Dim ConnectionsWeather(IWeather) As System.Data.SQLite.SQLiteConnection
            For I = 0 To IWeather
                ConnectionsWeather(I) = CreateConnection(IntermediateCalculationPaths(I), False)
            Next
            Dim ConnectionsEvapotranspiration(IEvapotranspiration) As System.Data.SQLite.SQLiteConnection
            For I = 0 To IEvapotranspiration
                ConnectionsEvapotranspiration(I) = CreateConnection(IntermediateCalculationPaths(I + IWeather + 1), False)
            Next
            Dim ConnectionsGrowingDegreeDays(IGrowingDegreeDays) As System.Data.SQLite.SQLiteConnection
            For I = 0 To IGrowingDegreeDays
                ConnectionsGrowingDegreeDays(I) = CreateConnection(IntermediateCalculationPaths(I + IWeather + IEvapotranspiration + 2), False)
            Next

            Dim OutputConnections = ConnectionsWeather.Concat(ConnectionsEvapotranspiration).Concat(ConnectionsGrowingDegreeDays)
            For Each Connection In OutputConnections
                Connection.Open()
            Next

            Dim CommandRasters = ConnectionRasters.CreateCommand
            Dim CommandsWeather(IWeather) As System.Data.SQLite.SQLiteCommand
            For I = 0 To IWeather
                CommandsWeather(I) = ConnectionsWeather(I).CreateCommand
            Next
            Dim CommandsEvapotranspiration(IEvapotranspiration) As System.Data.SQLite.SQLiteCommand
            For I = 0 To IEvapotranspiration
                CommandsEvapotranspiration(I) = ConnectionsEvapotranspiration(I).CreateCommand
            Next
            Dim CommandsGrowingDegreeDays(IGrowingDegreeDays) As System.Data.SQLite.SQLiteCommand
            For I = 0 To IGrowingDegreeDays
                CommandsGrowingDegreeDays(I) = ConnectionsGrowingDegreeDays(I).CreateCommand
            Next
            Dim OutputCommands = CommandsWeather.Concat(CommandsEvapotranspiration).Concat(CommandsGrowingDegreeDays)

            'Create Tables and Delete Previous Calculations if Needed
            For Each Command As System.Data.SQLite.SQLiteCommand In OutputCommands
                Command.CommandText = "CREATE TABLE IF NOT EXISTS Rasters (Date NUMERIC UNIQUE, Image BLOB)"
                Command.ExecuteNonQuery()
            Next

            'Determine Calculation Hours
            Dim First As Integer = MinDate.Subtract(NLDAS_2AStartDate).TotalHours
            Dim Last As Integer = MaxDate.Subtract(NLDAS_2AStartDate).TotalHours

            For Hour As Integer = First To Last Step 24
                If Hour + 23 <= Last Then
                    'Load NLDAS-2A Grib Files for Each Day Period
                    Dim DayStartDate = NLDAS_2AStartDate.AddHours(Hour)  'New DateTime(2010, 7, 24, 13, 0, 0)
                    CommandRasters.CommandText = "SELECT Image FROM Rasters WHERE Date >= @Date1 and Date <= @Date2"
                    CommandRasters.Parameters.Add("@Date1", DbType.DateTime).Value = DayStartDate
                    CommandRasters.Parameters.Add("@Date2", DbType.DateTime).Value = DayStartDate.AddHours(23)

                    Dim NLDAS_2AWeatherPixels(23)()() As Single
                    Using Reader = CommandRasters.ExecuteReader
                        For H = 0 To 23
                            Reader.Read()

                            Dim InMemoryPath = "/vsimem/Raster" & DayStartDate.AddHours(H).ToString("yyyyMMddHH")
                            GDAL.Gdal.FileFromMemBuffer(InMemoryPath, Reader(0))

                            NLDAS_2AWeatherPixels(H) = ExtractNLDAS_2A(InMemoryPath, NLDAS_2AXOffset, NLDAS_2AYOffset, NLDAS_2AXSize, NLDAS_2AYSize)

                            GDAL.Gdal.Unlink(InMemoryPath)
                        Next
                    End Using

                    'Create Intermediate Output Datasets
                    Dim IntermediateRasterPath = IO.Path.Combine(IO.Path.GetTempPath, "{0} " & DayStartDate.ToString("yyyy-MM-dd") & ".tif")
                    Dim WeatherRasterPaths(IWeather) As String
                    Dim WeatherRasters(IWeather) As Raster
                    For I = 0 To IWeather
                        WeatherRasterPaths(I) = String.Format(IntermediateRasterPath, "Weather " & I)
                        WeatherRasters(I) = CreateNewRaster(WeatherRasterPaths(I), LongitudeRaster.XCount, LongitudeRaster.YCount, LongitudeRaster.Projection, LongitudeRaster.GeoTransform, {Single.MinValue})
                    Next
                    Dim EvapotranspirationRasterPaths(IEvapotranspiration) As String
                    Dim EvapotranspirationRasters(IEvapotranspiration) As Raster
                    For I = 0 To IEvapotranspiration
                        EvapotranspirationRasterPaths(I) = String.Format(IntermediateRasterPath, "Evapotranspiration " & I)
                        EvapotranspirationRasters(I) = CreateNewRaster(EvapotranspirationRasterPaths(I), LongitudeRaster.XCount, LongitudeRaster.YCount, LongitudeRaster.Projection, LongitudeRaster.GeoTransform, {Single.MinValue})
                    Next
                    Dim GrowingDegreeDaysRasterPaths(IGrowingDegreeDays) As String
                    Dim GrowingDegreeDaysRasters(IGrowingDegreeDays) As Raster
                    For I = 0 To IGrowingDegreeDays
                        GrowingDegreeDaysRasterPaths(I) = String.Format(IntermediateRasterPath, "Growing Degree Days " & I)
                        GrowingDegreeDaysRasters(I) = CreateNewRaster(GrowingDegreeDaysRasterPaths(I), LongitudeRaster.XCount, LongitudeRaster.YCount, LongitudeRaster.Projection, LongitudeRaster.GeoTransform, {Single.MinValue})
                    Next
                    Dim OutputPaths = WeatherRasterPaths.Concat(EvapotranspirationRasterPaths).Concat(GrowingDegreeDaysRasterPaths)
                    Dim OutputRasters = WeatherRasters.Concat(EvapotranspirationRasters).Concat(GrowingDegreeDaysRasters)

                    'Calculate Weather Parameters, Evapotranspiration, and Growing Degree Days from Hourly Input
                    For Each Raster In InputRasters
                        Raster.Open(GDAL.Access.GA_ReadOnly)
                    Next
                    For Each Raster In OutputRasters
                        Raster.Open(GDAL.Access.GA_Update)
                    Next

                    'Calculate Solar Positioning for Multiple Time Steps
                    Dim TimeStep = 15 'minutes
                    Dim HourlySteps As Integer = 60 / TimeStep
                    Dim SolarPositions(1440 / TimeStep) As SolarPosition
                    For I = 0 To SolarPositions.Length - 1
                        SolarPositions(I) = CalculateSolarPosition(DayStartDate.AddMinutes(I * TimeStep - 30))
                    Next

                    'Calculate NLDAS-2A Extraterrestrial Radiation
                    Dim NLDAS_2ARaPixels(SolarPositions.Length - 1)() As Single
                    For J = 0 To SolarPositions.Length - 1
                        ReDim NLDAS_2ARaPixels(J)(NLDAS_2ALength)

                        Dim I As Integer = 0
                        For Y = 0 To NLDAS_2AYSize - 1
                            For X = 0 To NLDAS_2AXSize - 1
                                If Not NLDAS_2ATopographyPixels(0)(I) = NLDAS_2ANoDataValue Then
                                    Dim Longitude = ReducedNLDAS_2ARaster.Extent.Xmin + ReducedNLDAS_2ARaster.XResolution * (X + 0.5)
                                    Dim Latitude = ReducedNLDAS_2ARaster.Extent.Ymax - ReducedNLDAS_2ARaster.YResolution * (Y + 0.5)
                                    Dim H = Limit((J - 1) / HourlySteps, 0, 23)
                                    NLDAS_2ARaPixels(J)(I) = CalculateInstantaneousRa(Longitude, Latitude, NLDAS_2ATopographyPixels(0)(I), NLDAS_2ATopographyPixels(1)(I), NLDAS_2ATopographyPixels(2)(I), NLDAS_2AWeatherPixels(H)(Hourly.Air_Temperature)(I), NLDAS_2AWeatherPixels(H)(Hourly.Air_Pressure)(I), SolarPositions(J))
                                End If
                                I += 1
                            Next
                        Next
                    Next

                    If BackgroundWorker.CancellationPending Then : DoWorkEvent.Cancel = True : Exit Sub : End If

                    'Loop Through Raster Blocks
                    Do Until WeatherRasters(0).BlocksProcessed
                        Dim LongitudePixels = LongitudeRaster.Read({1})
                        Dim LatitudePixels = LatitudeRaster.Read({1})
                        Dim ElevationPixels = ElevationRaster.Read({1})
                        Dim SlopePixels = SlopeRaster.Read({1})
                        Dim AspectPixels = AspectRaster.Read({1})
                        Dim WeatherPixels(IWeather)
                        For I = 0 To IWeather
                            WeatherPixels(I) = WeatherRasters(I).GetValuesArray({1})
                        Next
                        Dim EvapotranspirationPixels(IEvapotranspiration)
                        For I = 0 To IEvapotranspiration
                            EvapotranspirationPixels(I) = EvapotranspirationRasters(I).GetValuesArray({1})
                        Next
                        Dim GrowingDegreeDaysPixels(IGrowingDegreeDays)
                        For I = 0 To IGrowingDegreeDays
                            GrowingDegreeDaysPixels(I) = GrowingDegreeDaysRasters(I).GetValuesArray({1})
                        Next
                        Dim OutputPixels = WeatherPixels.Concat(EvapotranspirationPixels).Concat(GrowingDegreeDaysPixels)

                        Threading.Tasks.Parallel.For(0, ElevationPixels.Length, _
                        Sub(I)
                            'For I = 0 To WeatherPixels(0).Length - 1
                            If Not LongitudePixels(I) = LongitudeNoDataValue Then
                                'Extract Surrounding NLDAS-2 Pixel Values
                                Dim RaQuads = ReducedNLDAS_2ARaster.GetPixelQuadFromCoordinate(NLDAS_2ARaPixels, New Point64(LongitudePixels(I), LatitudePixels(I)))
                                Dim MinimumAirTemperature As Single = Single.MaxValue
                                Dim MaximumAirTemperature As Single = Single.MinValue

                                'Sinusoidal Regression Correction Factors
                                Dim TimeVariables(5) As Double
                                TimeVariables(0) = 1
                                Dim FunctionDoY As Double = (DayStartDate.DayOfYear - 1) / 365 * 2 * π
                                TimeVariables(1) = Math.Cos(FunctionDoY)
                                TimeVariables(2) = Math.Sin(FunctionDoY)

                                For H = 0 To 23
                                    Dim WeatherQuads = ReducedNLDAS_2ARaster.GetPixelQuadFromCoordinate(NLDAS_2AWeatherPixels(H), New Point64(LongitudePixels(I), LatitudePixels(I)))
                                    Dim TopographyQuads = ReducedNLDAS_2ARaster.GetPixelQuadFromCoordinate(NLDAS_2ATopographyPixels, New Point64(LongitudePixels(I), LatitudePixels(I)))

                                    'Record Date and Time
                                    Dim Time = DayStartDate.AddHours(H)
                                    Dim FunctionHour As Double = (Time.Hour) / 23 * 2 * π
                                    TimeVariables(3) = Math.Cos(FunctionHour)
                                    TimeVariables(4) = Math.Sin(FunctionHour)

                                    'Correct for Topographical Differences
                                    Dim ΔZBottomLeft = ElevationPixels(I) - TopographyQuads.Band(0).BottomLeft
                                    Dim ΔZBottomRight = ElevationPixels(I) - TopographyQuads.Band(0).BottomRight
                                    Dim ΔZTopLeft = ElevationPixels(I) - TopographyQuads.Band(0).TopLeft
                                    Dim ΔZTopRight = ElevationPixels(I) - TopographyQuads.Band(0).TopRight

                                    '    Air Temperature (F)
                                    WeatherQuads.Band(Hourly.Air_Temperature).BottomLeft += LapseRate * ΔZBottomLeft
                                    WeatherQuads.Band(Hourly.Air_Temperature).BottomRight += LapseRate * ΔZBottomRight
                                    WeatherQuads.Band(Hourly.Air_Temperature).TopLeft += LapseRate * ΔZTopLeft
                                    WeatherQuads.Band(Hourly.Air_Temperature).TopRight += LapseRate * ΔZTopRight
                                    Dim AirTemperature = BilinearInterpolation(WeatherQuads.Band(Hourly.Air_Temperature), WeatherQuads.FractionX, WeatherQuads.FractionY)
                                    TimeVariables(5) = AirTemperature
                                    AirTemperature -= SumProduct(AirTemperatureCorrectionCoefficients, TimeVariables)

                                    WeatherPixels(Weather.MeanAirTemperature)(I) += AirTemperature
                                    If AirTemperature < MinimumAirTemperature Then MinimumAirTemperature = AirTemperature
                                    If AirTemperature > MaximumAirTemperature Then MaximumAirTemperature = AirTemperature

                                    '    Air Pressure (kPa)
                                    WeatherQuads.Band(Hourly.Air_Pressure).BottomLeft = AdjustAirPressure(WeatherQuads.Band(Hourly.Air_Pressure).BottomLeft, WeatherQuads.Band(Hourly.Air_Temperature).BottomLeft, ΔZBottomLeft)
                                    WeatherQuads.Band(Hourly.Air_Pressure).BottomRight = AdjustAirPressure(WeatherQuads.Band(Hourly.Air_Pressure).BottomRight, WeatherQuads.Band(Hourly.Air_Temperature).BottomRight, ΔZBottomRight)
                                    WeatherQuads.Band(Hourly.Air_Pressure).TopLeft = AdjustAirPressure(WeatherQuads.Band(Hourly.Air_Pressure).TopLeft, WeatherQuads.Band(Hourly.Air_Temperature).TopLeft, ΔZTopLeft)
                                    WeatherQuads.Band(Hourly.Air_Pressure).TopRight = AdjustAirPressure(WeatherQuads.Band(Hourly.Air_Pressure).TopRight, WeatherQuads.Band(Hourly.Air_Temperature).TopRight, ΔZTopRight)
                                    Dim AirPressure = BilinearInterpolation(WeatherQuads.Band(Hourly.Air_Pressure), WeatherQuads.FractionX, WeatherQuads.FractionY)

                                    '    Solar Radiation (Langley/hr)
                                    Dim ProjectRa As Double = 0
                                    Dim Start As Integer = H * HourlySteps
                                    For J = Start To Start + HourlySteps
                                        ProjectRa += CalculateInstantaneousRa(LongitudePixels(I), LatitudePixels(I), ElevationPixels(I), SlopePixels(I), AspectPixels(I), AirTemperature, AirPressure, SolarPositions(J))
                                    Next
                                    Dim HourlyAverage As Integer = HourlySteps + 1
                                    ProjectRa /= HourlyAverage

                                    Dim NLDAS_2ARa As New QuadValues
                                    NLDAS_2ARa.BottomLeft = 0
                                    NLDAS_2ARa.BottomRight = 0
                                    NLDAS_2ARa.TopLeft = 0
                                    NLDAS_2ARa.TopRight = 0

                                    For J = Start To Start + HourlySteps
                                        NLDAS_2ARa.BottomLeft += RaQuads.Band(J).BottomLeft
                                        NLDAS_2ARa.BottomRight += RaQuads.Band(J).BottomRight
                                        NLDAS_2ARa.TopLeft += RaQuads.Band(J).TopLeft
                                        NLDAS_2ARa.TopRight += RaQuads.Band(J).TopRight
                                    Next

                                    NLDAS_2ARa.BottomLeft /= HourlyAverage
                                    NLDAS_2ARa.BottomRight /= HourlyAverage
                                    NLDAS_2ARa.TopLeft /= HourlyAverage
                                    NLDAS_2ARa.TopRight /= HourlyAverage

                                    WeatherQuads.Band(Hourly.Solar_Radiation).BottomLeft = TranslateRs(WeatherQuads.Band(Hourly.Solar_Radiation).BottomLeft, NLDAS_2ARa.BottomLeft, ProjectRa, SlopePixels(I))
                                    WeatherQuads.Band(Hourly.Solar_Radiation).BottomRight = TranslateRs(WeatherQuads.Band(Hourly.Solar_Radiation).BottomRight, NLDAS_2ARa.BottomRight, ProjectRa, SlopePixels(I))
                                    WeatherQuads.Band(Hourly.Solar_Radiation).TopLeft = TranslateRs(WeatherQuads.Band(Hourly.Solar_Radiation).TopLeft, NLDAS_2ARa.TopLeft, ProjectRa, SlopePixels(I))
                                    WeatherQuads.Band(Hourly.Solar_Radiation).TopRight = TranslateRs(WeatherQuads.Band(Hourly.Solar_Radiation).TopRight, NLDAS_2ARa.TopRight, ProjectRa, SlopePixels(I))

                                    Dim SolarRadiation = BilinearInterpolation(WeatherQuads.Band(Hourly.Solar_Radiation), WeatherQuads.FractionX, WeatherQuads.FractionY)
                                    WeatherPixels(Weather.SolarRadiation)(I) += SolarRadiation

                                    '    Precipitation (in)
                                    'Dim LinearRegressionCoefficients = CalculateSimpleLinearRegression({TopographyQuads.Band(0).BottomLeft, TopographyQuads.Band(0).BottomRight, TopographyQuads.Band(0).TopLeft, TopographyQuads.Band(0).TopRight}, {WeatherQuads.Band(Hourly.Precipitation).BottomLeft, WeatherQuads.Band(Hourly.Precipitation).BottomRight, WeatherQuads.Band(Hourly.Precipitation).TopLeft, WeatherQuads.Band(Hourly.Precipitation).TopRight})
                                    'WeatherPixels(Weather.Precipitation)(I) += 0.5 * (LinearRegressionCoefficients(0) * ElevationPixels(I) + LinearRegressionCoefficients(1) + BilinearInterpolation(WeatherQuads.Band(Hourly.Precipitation), WeatherQuads.FractionX, WeatherQuads.FractionY))
                                    WeatherPixels(Weather.Precipitation)(I) += BilinearInterpolation(WeatherQuads.Band(Hourly.Precipitation), WeatherQuads.FractionX, WeatherQuads.FractionY)

                                    '    Relative Humidity (%)
                                    Dim RelativeHumitidy = BilinearInterpolation(WeatherQuads.Band(Hourly.Relative_Humidity), WeatherQuads.FractionX, WeatherQuads.FractionY)
                                    TimeVariables(5) = RelativeHumitidy
                                    RelativeHumitidy -= SumProduct(HumidityCorrectionCoefficients, TimeVariables)
                                    If RelativeHumitidy > 100 Then RelativeHumitidy = 100
                                    If RelativeHumitidy < 7 Then RelativeHumitidy = 7

                                    '    Wind Speed (mph)
                                    Dim WindSpeed = (BilinearInterpolation(WeatherQuads.Band(Hourly.Wind_Vector_U), WeatherQuads.FractionX, WeatherQuads.FractionY) ^ 2 + BilinearInterpolation(WeatherQuads.Band(Hourly.Wind_Vector_V), WeatherQuads.FractionX, WeatherQuads.FractionY) ^ 2) ^ 0.5
                                    'WindSpeed -= SumProduct(WindSpeedCorrectionCoefficients, TimeVariables)
                                    If WindSpeed > 5.5 Then WindSpeed = 5.5
                                    WeatherPixels(Weather.Windspeed)(I) += WindSpeed

                                    '    Dewpoint Temperature (F)
                                    Dim DewpointTemperature As Single = Single.MinValue

                                    'Calculate ASCE Reference Evapotranspiration (in)
                                    EvapotranspirationPixels(0)(I) += CalculateHourlyASCEReferenceET(Elevation:=ElevationPixels(I), _
                                                                                        Latitude:=LatitudePixels(I), _
                                                                                        Longitude:=LongitudePixels(I), _
                                                                                        RecordDate:=Time, _
                                                                                        AirTemperature:=AirTemperature, _
                                                                                        AirPressure:=AirPressure, _
                                                                                        RelativeHumidity:=RelativeHumitidy, _
                                                                                        SolarRadiation:=SolarRadiation, _
                                                                                        ExtraterrestrialRadiation:=ProjectRa, _
                                                                                        WindSpeed:=WindSpeed, _
                                                                                        AnemometerHeight:=ToFeet(2), _
                                                                                        ReferenceET:=ReferenceET.LongReference, _
                                                                                        DewpointTemperature:=DewpointTemperature)

                                    'Calculate Aerodynamic Reference Evapotranspiration (in)
                                    EvapotranspirationPixels(2)(I) += CalculateHourlyAerodynamicWaterSurfaceEvaporation(Time, AirTemperature, AirPressure, RelativeHumitidy, WindSpeed)

                                    WeatherPixels(Weather.DewpointTemperature)(I) += DewpointTemperature
                                Next

                                'Set Temperature Pixels (F)
                                WeatherPixels(Weather.MeanAirTemperature)(I) /= 24
                                WeatherPixels(Weather.MaximumAirTemperature)(I) = MaximumAirTemperature
                                WeatherPixels(Weather.MinimumAirTemperature)(I) = MinimumAirTemperature
                                WeatherPixels(Weather.DewpointTemperature)(I) /= 24

                                'Calculate Hargreaves Reference Evapotranspiration (in)
                                EvapotranspirationPixels(1)(I) = CalculateDailyHargreavesReferenceET(MinimumAirTemperature, WeatherPixels(Weather.MeanAirTemperature)(I), MaximumAirTemperature, WeatherPixels(Weather.SolarRadiation)(I))

                                'Calculate Growing Degree Days (F)
                                Dim AverageMaxMinTemperature As Single = (MaximumAirTemperature + MinimumAirTemperature) / 2
                                Dim Degrees As Single = AverageMaxMinTemperature - 32
                                If Degrees < 0 Then Degrees = 0
                                GrowingDegreeDaysPixels(0)(I) = Degrees

                                Degrees = AverageMaxMinTemperature - 41
                                If Degrees < 0 Then Degrees = 0
                                GrowingDegreeDaysPixels(1)(I) = Degrees

                                GrowingDegreeDaysPixels(2)(I) = (Limit(MaximumAirTemperature, 50, 86) + Limit(MinimumAirTemperature, 50, 86)) / 2 - 50

                                ''Convert to Equivalent Horizontal Depth from a Sloped Surface (in)
                                'Dim SlopeFraction = Math.Cos(ToRadians(SlopePixels(I)))
                                'PrecipitationPixels(I) /= SlopeFraction
                                'ASCE_ETPixels(I) /= SlopeFraction
                                'Hargreaves_ETPixels(I) /= SlopeFraction
                            Else
                                For P = 0 To OutputPixels.Count - 1
                                    OutputPixels(P)(I) = Single.MinValue
                                Next
                            End If

                            If BackgroundWorker.CancellationPending Then : DoWorkEvent.Cancel = True : Exit Sub : End If
                            'Next
                        End Sub)

                        'Write Calculations to Output
                        For I = 0 To IWeather
                            WeatherRasters(I).Write({1}, WeatherPixels(I))
                        Next
                        For I = 0 To IEvapotranspiration
                            EvapotranspirationRasters(I).Write({1}, EvapotranspirationPixels(I))
                        Next
                        For I = 0 To IGrowingDegreeDays
                            GrowingDegreeDaysRasters(I).Write({1}, GrowingDegreeDaysPixels(I))
                        Next

                        'Move Position to Next Raster Block
                        For Each Raster In InputRasters.Concat(OutputRasters)
                            Raster.AdvanceBlock()
                        Next
                    Loop

                    For I = 0 To OutputCommands.Count - 1
                        'Close Intermediate Raster
                        OutputRasters(I).Close()

                        'Scale Intermediate Rasters to 16-bit Integers to Conserve Space
                        Dim ScaledRasterPath = OutputRasters(I).Path & ".Scaled.tif"
                        ScaleRaster(OutputRasters(I).Path, ScaledRasterPath, , {"COMPRESS=DEFLATE"})

                        'Store Raster in Database
                        OutputCommands(I).CommandText = "INSERT OR REPLACE INTO Rasters (Date, Image) VALUES (@Date, @Image)"
                        OutputCommands(I).Parameters.Add("@Date", DbType.DateTime).Value = DayStartDate
                        OutputCommands(I).Parameters.Add("@Image", DbType.Object).Value = IO.File.ReadAllBytes(ScaledRasterPath)
                        OutputCommands(I).ExecuteNonQuery()

                        'Delete Intermediate FilesS
                        IO.File.Delete(OutputRasters(I).Path)
                        IO.File.Delete(ScaledRasterPath)
                    Next
                End If

                BackgroundWorker.ReportProgress(0)
            Next

            'Release Memory
            For Each OutputCommand In OutputCommands
                OutputCommand.Dispose()
            Next
            For Each Connection In OutputConnections
                Connection.Dispose()
            Next
            For Each Raster In InputRasters
                Raster.Close()
            Next

            'Calculate Monthly and Annual Sums and Averages
            Threading.Tasks.Parallel.For(0, IntermediateCalculationPaths.Length, _
            Sub(I)
                'For I = 0 To IntermediateCalculationPaths.Length - 1
                CalculateMonthlyAndAnnualStatistics(IntermediateCalculationPaths(I), Statistics(I), , MinDate, MaxDate)
                'Next
            End Sub)
        Catch Exception As Exception
            MsgBox(Exception.Message)
        End Try
    End Sub

    Sub CalculateReferenceEvapotranspirationDAYMET(MinDate As DateTime, MaxDate As DateTime, BackgroundWorker As System.ComponentModel.BackgroundWorker, DoWorkEvent As System.ComponentModel.DoWorkEventArgs)
        'Open Databases 
        Dim ConnectionRasters = CreateConnection(DAYMETRastersPath)
        ConnectionRasters.Open()
        Dim CommandRasters = ConnectionRasters.CreateCommand

        Dim OutputConnection = CreateConnection(DAYMETPrecipitationPath, False)
        OutputConnection.Open()
        Dim OutputCommand = OutputConnection.CreateCommand

        'Create Tables and Delete Previous Calculations if Needed
        OutputCommand.CommandText = "CREATE TABLE IF NOT EXISTS Rasters (Date NUMERIC UNIQUE, Image BLOB)"
        OutputCommand.ExecuteNonQuery()

        'Determine Calculation Hours
        Dim First As Integer = MinDate.Subtract(DAYMETStartDate).TotalDays
        Dim Last As Integer = MaxDate.Subtract(DAYMETStartDate).TotalDays

        For I As Integer = First To Last
            Dim ImageDate = DAYMETStartDate.AddDays(I)

            CommandRasters.CommandText = "SELECT Image FROM Rasters WHERE Date = @Date"
            CommandRasters.Parameters.Add("@Date", DbType.DateTime).Value = ImageDate

            Dim DAYMETRasterPath = IO.Path.Combine(IO.Path.GetTempPath, "DAYMET " & ImageDate.ToString("yyyy-MM-dd") & ".tif")
            IO.File.WriteAllBytes(DAYMETRasterPath, CommandRasters.ExecuteScalar)

            'Currently Precipitation Only
            Dim SnappedRasterPath = DAYMETRasterPath & ".Snapped.tif"
            SnapToRaster(DAYMETRasterPath, MaskRasterPath, SnappedRasterPath, , , , GDALProcess.Compression.NONE, GDALProcess.ResamplingMethod.Bilinear, , , GDAL.DataType.GDT_Float32)

            'Unit Conversion
            Using SnappedRaster As New Raster(SnappedRasterPath)
                SnappedRaster.Open(GDAL.Access.GA_Update)

                Dim NoDataValue = SnappedRaster.BandNoDataValue(0)

                Do Until SnappedRaster.BlocksProcessed
                    Dim SnappedPixels = SnappedRaster.Read({1})

                    For J = 0 To SnappedPixels.Length - 1
                        If SnappedPixels(J) <> NoDataValue Then
                            SnappedPixels(J) = ToInches(SnappedPixels(J))
                        End If
                    Next

                    SnappedRaster.Write({1}, SnappedPixels)

                    SnappedRaster.AdvanceBlock()
                Loop
            End Using

            'Scale Intermediate Rasters to 16-bit Integers to Conserve Space
            Dim ScaledRasterPath = DAYMETRasterPath & ".Scaled.tif"
            ScaleRaster(SnappedRasterPath, ScaledRasterPath, , {"COMPRESS=DEFLATE"})

            'Store Raster in Database
            OutputCommand.CommandText = "INSERT OR REPLACE INTO Rasters (Date, Image) VALUES (@Date, @Image)"
            OutputCommand.Parameters.Add("@Date", DbType.DateTime).Value = ImageDate
            OutputCommand.Parameters.Add("@Image", DbType.Object).Value = IO.File.ReadAllBytes(ScaledRasterPath)
            OutputCommand.ExecuteNonQuery()

            'Delete Intermediate Files
            IO.File.Delete(DAYMETRasterPath)
            IO.File.Delete(SnappedRasterPath)
            IO.File.Delete(ScaledRasterPath)

            If BackgroundWorker.CancellationPending Then : DoWorkEvent.Cancel = True : Exit Sub : End If
            BackgroundWorker.ReportProgress(0)
        Next

        'Release Memory
        OutputCommand.Dispose()
        OutputConnection.Dispose()
        CommandRasters.Dispose()
        ConnectionRasters.Dispose()

        'Calculate Monthly and Annual Sums and Averages
        CalculateMonthlyAndAnnualStatistics(DAYMETPrecipitationPath, StatisticType.Sum, , MinDate, MaxDate)
    End Sub

    Sub CalculatePotentialEvapotranspiration(CoverProperties() As CoverProperties, MinDate As DateTime, MaxDate As DateTime, BackgroundWorker As System.ComponentModel.BackgroundWorker, DoWorkEvent As System.ComponentModel.DoWorkEventArgs)
        Threading.Tasks.Parallel.ForEach(CoverProperties, New Threading.Tasks.ParallelOptions With {.MaxDegreeOfParallelism = 1}, _
        Sub(Cover)
            'Open Mask Raster
            Dim MaskRaster As New Raster(MaskRasterPath)
            Dim MaskNoDataValue = MaskRaster.BandNoDataValue(0)

            'For Each Cover In CoverProperties
            Dim OutputDatabasePath = IO.Path.Combine(PotentialEvapotranspirationDirectory, String.Format(IO.Path.GetFileName(PotentialEvapotranspirationPath), Cover.Name))

            Using Connection = CreateConnection(OutputDatabasePath, False)
                Connection.Open()

                Using Command = Connection.CreateCommand
                    Command.CommandText = "CREATE TABLE IF NOT EXISTS Dates (Year INTEGER UNIQUE, Initiation BLOB, Intermediate BLOB, Termination BLOB, SpringFrost BLOB, KillingFrost BLOB)"
                    Command.ExecuteNonQuery()

                    Command.CommandText = "CREATE TABLE IF NOT EXISTS Rasters (Date NUMERIC UNIQUE, Image BLOB)"
                    Command.ExecuteNonQuery()

                    'Find Start and End Years
                    Dim StartYear As Integer = MinDate.Year
                    Command.CommandText = "SELECT MIN(Year) FROM Dates"
                    Dim Value = Command.ExecuteScalar
                    If Not IsDBNull(Value) Then If Value < StartYear Then StartYear = Value

                    Dim EndYear = MaxDate.AddDays(1).Year - 1

                    'Loop Through Each Year
                    For Year As Integer = StartYear To EndYear
                        Command.CommandText = String.Format("INSERT OR REPLACE INTO Dates (Year) VALUES ('{0}')", Year)
                        Command.ExecuteNonQuery()

                        Dim TemporaryPath = IO.Path.Combine(IO.Path.GetTempPath, IO.Path.GetFileNameWithoutExtension(OutputDatabasePath) & "-" & Year & "_{0}.tif")
                        Dim StartDate = New DateTime(Year, 1, 1).AddHours(13)
                        Dim EndDate = New DateTime(Year, 12, 31).AddHours(13)
                        Dim YearDayCount As Integer = EndDate.DayOfYear

                        'Spring Frost Calculation
                        Using TemperatureConnection = CreateConnection(NLDAS_2AMinimumAirTemperaturePath)
                            TemperatureConnection.Open()

                            Using TemperatureCommand = TemperatureConnection.CreateCommand
                                TemperatureCommand.CommandText = "SELECT Image FROM Rasters WHERE Date >= @StartDate and Date <= @EndDate ORDER BY Date"
                                TemperatureCommand.Parameters.Add("@StartDate", DbType.DateTime).Value = StartDate
                                TemperatureCommand.Parameters.Add("@EndDate", DbType.DateTime).Value = StartDate.AddDays(199)

                                Dim SpringFrostPath = String.Format(TemporaryPath, "Spring Frost")
                                Dim SpringFrostRaster = CreateNewRaster(SpringFrostPath, MaskRaster.XCount, MaskRaster.YCount, MaskRaster.Projection, MaskRaster.GeoTransform, {Int16.MinValue}, GDAL.DataType.GDT_Int16)

                                Using Reader = TemperatureCommand.ExecuteReader
                                    Dim DayOfYear As Integer = 1

                                    Do While Reader.Read
                                        Dim DailyPath = String.Format(TemporaryPath, "Daily")
                                        IO.File.WriteAllBytes(DailyPath, Reader(0))
                                        Dim DailyRaster As New Raster(DailyPath)

                                        DailyRaster.Open(GDAL.Access.GA_ReadOnly)
                                        SpringFrostRaster.Open(GDAL.Access.GA_Update)

                                        Do Until DailyRaster.BlocksProcessed
                                            Dim DailyPixels = DailyRaster.Read({1})
                                            Dim SpringFrostPixels = SpringFrostRaster.Read({1})

                                            Dim NoDataValue = DailyRaster.BandNoDataValue(0)

                                            For I = 0 To DailyPixels.Length - 1
                                                If DailyPixels(I) = NoDataValue Then
                                                    SpringFrostPixels(I) = Int16.MinValue
                                                Else
                                                    'Set Spring Frost Date if Temperature Reached
                                                    If DailyPixels(I) <= Cover.SpringFrostTemperature Then
                                                        SpringFrostPixels(I) = DayOfYear
                                                    End If
                                                End If
                                            Next

                                            SpringFrostRaster.Write({1}, SpringFrostPixels)

                                            SpringFrostRaster.AdvanceBlock()
                                            DailyRaster.AdvanceBlock()
                                        Loop

                                        DailyRaster.Close()
                                        IO.File.Delete(DailyRaster.Path)

                                        DayOfYear += 1

                                        If BackgroundWorker.CancellationPending Then : DoWorkEvent.Cancel = True : Exit Sub : End If
                                    Loop
                                End Using

                                SpringFrostRaster.Close()

                                'Compress SpringFrost Date Raster and Store in Database
                                Dim CompressedRasterPath As String = SpringFrostPath & ".Compressed.tif"
                                Dim Process As New GDALProcess
                                Process.Translate({SpringFrostRaster.Path}, CompressedRasterPath, , GDALProcess.Compression.DEFLATE)

                                Command.CommandText = String.Format("UPDATE Dates SET SpringFrost = @File WHERE Year = '{0}'", Year)
                                Command.Parameters.Add("@File", DbType.Object).Value = IO.File.ReadAllBytes(CompressedRasterPath)
                                Command.ExecuteNonQuery()

                                IO.File.Delete(CompressedRasterPath)
                                IO.File.Delete(SpringFrostRaster.Path)
                            End Using
                        End Using

                        'Killing Frost Calculation
                        Using TemperatureConnection = CreateConnection(NLDAS_2AMinimumAirTemperaturePath)
                            TemperatureConnection.Open()

                            Using TemperatureCommand = TemperatureConnection.CreateCommand
                                TemperatureCommand.CommandText = "SELECT Image FROM Rasters WHERE Date >= @StartDate and Date <= @EndDate ORDER BY Date"
                                TemperatureCommand.Parameters.Add("@StartDate", DbType.DateTime).Value = StartDate.AddDays(200)
                                TemperatureCommand.Parameters.Add("@EndDate", DbType.DateTime).Value = EndDate

                                Dim KillingFrostPath = String.Format(TemporaryPath, "Killing Frost")
                                Dim KillingFrostRaster = CreateNewRaster(KillingFrostPath, MaskRaster.XCount, MaskRaster.YCount, MaskRaster.Projection, MaskRaster.GeoTransform, {Int16.MinValue}, GDAL.DataType.GDT_Int16)

                                Using Reader = TemperatureCommand.ExecuteReader
                                    Dim DayOfYear As Integer = 201

                                    Do While Reader.Read
                                        Dim DailyPath = String.Format(TemporaryPath, "Daily")
                                        IO.File.WriteAllBytes(DailyPath, Reader(0))
                                        Dim DailyRaster As New Raster(DailyPath)

                                        DailyRaster.Open(GDAL.Access.GA_ReadOnly)
                                        KillingFrostRaster.Open(GDAL.Access.GA_Update)

                                        Dim CountRemaining As Integer = 0

                                        Do Until DailyRaster.BlocksProcessed
                                            Dim DailyPixels = DailyRaster.Read({1})
                                            Dim KillingFrostPixels = KillingFrostRaster.Read({1})

                                            Dim NoDataValue = DailyRaster.BandNoDataValue(0)

                                            For I = 0 To DailyPixels.Length - 1
                                                If DailyPixels(I) = NoDataValue Then
                                                    KillingFrostPixels(I) = Int16.MinValue
                                                Else
                                                    'Set Killing Frost Date if Temperature Reached
                                                    If KillingFrostPixels(I) = 0 Then
                                                        If DailyPixels(I) <= Cover.KillingFrostTemperature Then
                                                            KillingFrostPixels(I) = DayOfYear
                                                        Else
                                                            CountRemaining += 1
                                                        End If
                                                    End If
                                                End If
                                            Next

                                            'Set Killing Frost Date to the Last Period Date if Threshold was never Surpassed
                                            If DayOfYear = YearDayCount Then
                                                For I = 0 To DailyPixels.Length - 1
                                                    If DailyPixels(I) <> NoDataValue Then
                                                        If KillingFrostPixels(I) = 0 Then
                                                            KillingFrostPixels(I) = DayOfYear
                                                        End If
                                                    End If
                                                Next

                                                CountRemaining = 0
                                            End If

                                            KillingFrostRaster.Write({1}, KillingFrostPixels)

                                            KillingFrostRaster.AdvanceBlock()
                                            DailyRaster.AdvanceBlock()
                                        Loop

                                        DailyRaster.Close()
                                        IO.File.Delete(DailyRaster.Path)

                                        If CountRemaining = 0 Then Exit Do
                                        DayOfYear += 1

                                        If BackgroundWorker.CancellationPending Then : DoWorkEvent.Cancel = True : Exit Sub : End If
                                    Loop
                                End Using

                                KillingFrostRaster.Close()

                                'Compress KillingFrost Date Raster and Store in Database
                                Dim CompressedRasterPath As String = KillingFrostPath & ".Compressed.tif"
                                Dim Process As New GDALProcess
                                Process.Translate({KillingFrostRaster.Path}, CompressedRasterPath, , GDALProcess.Compression.DEFLATE)

                                Command.CommandText = String.Format("UPDATE Dates SET KillingFrost = @File WHERE Year = '{0}'", Year)
                                Command.Parameters.Add("@File", DbType.Object).Value = IO.File.ReadAllBytes(CompressedRasterPath)
                                Command.ExecuteNonQuery()

                                IO.File.Delete(CompressedRasterPath)
                                IO.File.Delete(KillingFrostRaster.Path)
                            End Using
                        End Using

                        'Initial Date Selection
                        For X = 1 To 1
                            Dim InitiationPath = String.Format(TemporaryPath, "Initiation Date")
                            Dim InitiationRaster = CreateNewRaster(InitiationPath, MaskRaster.XCount, MaskRaster.YCount, MaskRaster.Projection, MaskRaster.GeoTransform, {Int16.MinValue}, GDAL.DataType.GDT_Int16)

                            If Cover.InitiationThresholdType = ThresholdType.Number_of_Days Then
                                'Initiation Type Day of Year
                                InitiationRaster.Open(GDAL.Access.GA_Update)
                                MaskRaster.Open(GDAL.Access.GA_ReadOnly)

                                Do Until InitiationRaster.BlocksProcessed
                                    Dim InitiationPixels = InitiationRaster.Read({1})
                                    Dim MaskPixels = MaskRaster.Read({1})

                                    For I = 0 To InitiationPixels.Length - 1
                                        If MaskPixels(I) = MaskNoDataValue Then
                                            InitiationPixels(I) = Int16.MinValue
                                        Else
                                            InitiationPixels(I) = Cover.InitiationThreshold
                                        End If
                                    Next

                                    InitiationRaster.Write({1}, InitiationPixels)

                                    MaskRaster.AdvanceBlock()
                                    InitiationRaster.AdvanceBlock()
                                Loop

                                MaskRaster.Close()

                                If BackgroundWorker.CancellationPending Then : DoWorkEvent.Cancel = True : Exit Sub : End If
                            Else
                                'Initiation Type Variable Summation
                                Dim ThresholdRaster = CreateNewRaster(String.Format(TemporaryPath, "Initiation Threshold"), MaskRaster.XCount, MaskRaster.YCount, MaskRaster.Projection, MaskRaster.GeoTransform, {Single.MinValue})

                                Dim DateVariablePath As String = NLDAS_2AEvapotranspirationHargreavesPath
                                Select Case Cover.InitiationThresholdType
                                    Case ThresholdType.Growing_Degree_Days_Base_32F : DateVariablePath = NLDAS_2AGrowingDegreeDays32Path
                                    Case ThresholdType.Growing_Degree_Days_Base_41F : DateVariablePath = NLDAS_2AGrowingDegreeDays41Path
                                    Case ThresholdType.Growing_Degree_Days_Base_86F_and_50F : DateVariablePath = NLDAS_2AGrowingDegreeDays8650Path
                                End Select

                                'Loop Through Daily Rasters
                                Using DateVariableConnection = CreateConnection(DateVariablePath)
                                    DateVariableConnection.Open()

                                    Using DateVariableCommand = DateVariableConnection.CreateCommand
                                        DateVariableCommand.CommandText = "SELECT Image FROM Rasters WHERE Date >= @StartDate and Date <= @EndDate ORDER BY Date"
                                        DateVariableCommand.Parameters.Add("@StartDate", DbType.DateTime).Value = StartDate
                                        DateVariableCommand.Parameters.Add("@EndDate", DbType.DateTime).Value = EndDate

                                        Using Reader = DateVariableCommand.ExecuteReader
                                            Dim DayOfYear As Integer = 1

                                            Do While Reader.Read
                                                Dim DailyPath = String.Format(TemporaryPath, "Daily")
                                                IO.File.WriteAllBytes(DailyPath, Reader(0))
                                                Dim DailyRaster As New Raster(DailyPath)

                                                DailyRaster.Open(GDAL.Access.GA_ReadOnly)
                                                ThresholdRaster.Open(GDAL.Access.GA_Update)
                                                InitiationRaster.Open(GDAL.Access.GA_Update)

                                                Dim CountRemaining As Integer = 0

                                                Do Until DailyRaster.BlocksProcessed
                                                    Dim DailyPixels = DailyRaster.Read({1})
                                                    Dim ThresholdPixels = ThresholdRaster.Read({1})
                                                    Dim InitiationPixels = InitiationRaster.Read({1})

                                                    Dim NoDataValue = DailyRaster.BandNoDataValue(0)

                                                    For I = 0 To DailyPixels.Length - 1
                                                        If DailyPixels(I) = NoDataValue Then
                                                            InitiationPixels(I) = Int16.MinValue
                                                        Else
                                                            'Set Initiation Date if Threshold Reached
                                                            If ThresholdPixels(I) < Cover.InitiationThreshold Then
                                                                ThresholdPixels(I) += DailyPixels(I)
                                                                If ThresholdPixels(I) >= Cover.InitiationThreshold Then
                                                                    InitiationPixels(I) = DayOfYear
                                                                Else
                                                                    CountRemaining += 1
                                                                End If
                                                            End If
                                                        End If
                                                    Next

                                                    'Set Initiation Date to the Last Period Date if Threshold was Never Surpassed
                                                    If DayOfYear = YearDayCount Then
                                                        For I = 0 To InitiationPixels.Length - 1
                                                            If InitiationPixels(I) <> Int16.MinValue Then
                                                                If ThresholdPixels(I) < Cover.InitiationThreshold Then
                                                                    InitiationPixels(I) = DayOfYear
                                                                End If
                                                            End If
                                                        Next

                                                        CountRemaining = 0
                                                    End If

                                                    ThresholdRaster.Write({1}, ThresholdPixels)
                                                    InitiationRaster.Write({1}, InitiationPixels)

                                                    ThresholdRaster.AdvanceBlock()
                                                    InitiationRaster.AdvanceBlock()
                                                    DailyRaster.AdvanceBlock()
                                                Loop

                                                DailyRaster.Close()
                                                IO.File.Delete(DailyRaster.Path)

                                                If CountRemaining = 0 Then Exit Do
                                                DayOfYear += 1

                                                If BackgroundWorker.CancellationPending Then : DoWorkEvent.Cancel = True : Exit Sub : End If
                                            Loop
                                        End Using

                                    End Using
                                End Using

                                ThresholdRaster.Close()
                                IO.File.Delete(ThresholdRaster.Path)
                            End If

                            'Set Initiation Date to Last Spring Frost Date if Later 
                            Command.CommandText = String.Format("SELECT SpringFrost FROM Dates WHERE Year = '{0}'", Year)
                            Dim SpringFrostPath = String.Format(TemporaryPath, "Spring Frost")
                            IO.File.WriteAllBytes(SpringFrostPath, Command.ExecuteScalar)
                            Dim SpringFrostRaster As New Raster(SpringFrostPath)
                            SpringFrostRaster.Open(GDAL.Access.GA_ReadOnly)

                            InitiationRaster.Open(GDAL.Access.GA_Update)

                            Do Until InitiationRaster.BlocksProcessed
                                Dim SpringFrostPixels = SpringFrostRaster.Read({1})
                                Dim InitiationPixels = InitiationRaster.Read({1})

                                For I = 0 To InitiationPixels.Length - 1
                                    If InitiationPixels(I) <> Int16.MinValue Then
                                        If SpringFrostPixels(I) > InitiationPixels(I) Then
                                            InitiationPixels(I) = SpringFrostPixels(I)
                                        End If
                                    End If
                                Next

                                InitiationRaster.Write({1}, InitiationPixels)

                                InitiationRaster.AdvanceBlock()
                                SpringFrostRaster.AdvanceBlock()
                            Loop

                            SpringFrostRaster.Close()
                            IO.File.Delete(SpringFrostRaster.Path)

                            InitiationRaster.Close()

                            'Compress Initiation Date Raster and Store in Database
                            Dim CompressedRasterPath As String = InitiationPath & ".Compressed.tif"
                            Dim Process As New GDALProcess
                            Process.Translate({InitiationRaster.Path}, CompressedRasterPath, , GDALProcess.Compression.DEFLATE)

                            Command.CommandText = String.Format("UPDATE Dates SET Initiation = @File WHERE Year = '{0}'", Year)
                            Command.Parameters.Add("@File", DbType.Object).Value = IO.File.ReadAllBytes(CompressedRasterPath)
                            Command.ExecuteNonQuery()

                            IO.File.Delete(CompressedRasterPath)
                            IO.File.Delete(InitiationRaster.Path)
                        Next

                        'Intermediate Date Selection
                        For X = 1 To 1
                            Dim IntermediatePath = String.Format(TemporaryPath, "Intermediate Date")
                            Dim IntermediateRaster = CreateNewRaster(IntermediatePath, MaskRaster.XCount, MaskRaster.YCount, MaskRaster.Projection, MaskRaster.GeoTransform, {Int16.MinValue}, GDAL.DataType.GDT_Int16)

                            Command.CommandText = "SELECT KillingFrost FROM Dates WHERE Year = " & Year
                            Dim KillingFrostPath = String.Format(TemporaryPath, "Killing Frost")
                            IO.File.WriteAllBytes(KillingFrostPath, Command.ExecuteScalar)
                            Dim KillingFrostRaster As New Raster(KillingFrostPath)

                            Command.CommandText = "SELECT Initiation FROM Dates WHERE Year = " & Year
                            Dim InitiationPath = String.Format(TemporaryPath, "Initiation Date")
                            IO.File.WriteAllBytes(InitiationPath, Command.ExecuteScalar)
                            Dim InitiationRaster As New Raster(InitiationPath)

                            If Cover.IntermediateThresholdType = ThresholdType.Number_of_Days Then
                                'Intermediate Type Day of Year
                                IntermediateRaster.Open(GDAL.Access.GA_Update)
                                InitiationRaster.Open(GDAL.Access.GA_ReadOnly)
                                KillingFrostRaster.Open(GDAL.Access.GA_ReadOnly)

                                Do Until IntermediateRaster.BlocksProcessed
                                    Dim IntermediatePixels = IntermediateRaster.Read({1})
                                    Dim InitiationPixels = InitiationRaster.Read({1})
                                    Dim KillingFrostPixels = KillingFrostRaster.Read({1})

                                    Dim InitiationNoDataValue = InitiationRaster.BandNoDataValue(0)

                                    For I = 0 To IntermediatePixels.Length - 1
                                        If InitiationPixels(I) = InitiationNoDataValue Then
                                            IntermediatePixels(I) = Int16.MinValue
                                        Else
                                            IntermediatePixels(I) = Cover.IntermediateThreshold + InitiationPixels(I)
                                            If IntermediatePixels(I) > KillingFrostPixels(I) Then IntermediatePixels(I) = KillingFrostPixels(I)
                                        End If
                                    Next

                                    IntermediateRaster.Write({1}, IntermediatePixels)

                                    KillingFrostRaster.AdvanceBlock()
                                    InitiationRaster.AdvanceBlock()
                                    IntermediateRaster.AdvanceBlock()
                                Loop

                                If BackgroundWorker.CancellationPending Then : DoWorkEvent.Cancel = True : Exit Sub : End If
                            Else
                                'Intermediate Type Variable Summation 
                                Dim ThresholdRaster = CreateNewRaster(String.Format(TemporaryPath, "Intermediate Threshold"), MaskRaster.XCount, MaskRaster.YCount, MaskRaster.Projection, MaskRaster.GeoTransform, {Single.MinValue})

                                Dim DateVariablePath As String = NLDAS_2AEvapotranspirationHargreavesPath
                                Select Case Cover.IntermediateThresholdType
                                    Case ThresholdType.Growing_Degree_Days_Base_32F : DateVariablePath = NLDAS_2AGrowingDegreeDays32Path
                                    Case ThresholdType.Growing_Degree_Days_Base_41F : DateVariablePath = NLDAS_2AGrowingDegreeDays41Path
                                    Case ThresholdType.Growing_Degree_Days_Base_86F_and_50F : DateVariablePath = NLDAS_2AGrowingDegreeDays8650Path
                                End Select

                                'Set Start Date to Earliest Initiation Date
                                Dim EarliestInitiationDate As Integer = YearDayCount
                                InitiationRaster.Open(GDAL.Access.GA_ReadOnly)
                                Do Until InitiationRaster.BlocksProcessed
                                    Dim InitiationPixels = InitiationRaster.Read({1})

                                    Dim InitiationNoDataValue = InitiationRaster.BandNoDataValue(0)

                                    For I = 0 To InitiationPixels.Length - 1
                                        If Not InitiationPixels(I) = InitiationNoDataValue Then
                                            If InitiationPixels(I) < EarliestInitiationDate Then EarliestInitiationDate = InitiationPixels(I)
                                        End If
                                    Next

                                    InitiationRaster.AdvanceBlock()
                                Loop

                                'Loop Through Daily Rasters
                                Using DateVariableConnection = CreateConnection(DateVariablePath)
                                    DateVariableConnection.Open()

                                    Using DateVariableCommand = DateVariableConnection.CreateCommand
                                        DateVariableCommand.CommandText = "SELECT Image FROM Rasters WHERE Date >= @StartDate and Date <= @EndDate ORDER BY Date"
                                        DateVariableCommand.Parameters.Add("@StartDate", DbType.DateTime).Value = StartDate.AddDays(EarliestInitiationDate - 1)
                                        DateVariableCommand.Parameters.Add("@EndDate", DbType.DateTime).Value = EndDate

                                        Using Reader = DateVariableCommand.ExecuteReader
                                            Dim DayOfYear As Integer = EarliestInitiationDate

                                            Do While Reader.Read
                                                Dim DailyPath = String.Format(TemporaryPath, "Daily")
                                                IO.File.WriteAllBytes(DailyPath, Reader(0))
                                                Dim DailyRaster As New Raster(DailyPath)

                                                DailyRaster.Open(GDAL.Access.GA_ReadOnly)
                                                ThresholdRaster.Open(GDAL.Access.GA_Update)
                                                IntermediateRaster.Open(GDAL.Access.GA_Update)
                                                InitiationRaster.Open(GDAL.Access.GA_ReadOnly)
                                                KillingFrostRaster.Open(GDAL.Access.GA_ReadOnly)

                                                Dim CountRemaining As Integer = 0

                                                Do Until DailyRaster.BlocksProcessed
                                                    Dim DailyPixels = DailyRaster.Read({1})
                                                    Dim ThresholdPixels = ThresholdRaster.Read({1})
                                                    Dim IntermediatePixels = IntermediateRaster.Read({1})
                                                    Dim InitiationPixels = InitiationRaster.Read({1})
                                                    Dim KillingFrostPixels = KillingFrostRaster.Read({1})

                                                    Dim NoDataValue = DailyRaster.BandNoDataValue(0)

                                                    For I = 0 To DailyPixels.Length - 1
                                                        If DailyPixels(I) = NoDataValue Then
                                                            IntermediatePixels(I) = Int16.MinValue
                                                        Else
                                                            'Set Intermediate Date if Threshold Reached
                                                            If DayOfYear >= InitiationPixels(I) Then
                                                                If ThresholdPixels(I) < Cover.IntermediateThreshold Then
                                                                    ThresholdPixels(I) += DailyPixels(I)
                                                                    If ThresholdPixels(I) >= Cover.IntermediateThreshold Then
                                                                        IntermediatePixels(I) = DayOfYear
                                                                    ElseIf KillingFrostPixels(I) <= DayOfYear Then
                                                                        IntermediatePixels(I) = DayOfYear
                                                                        ThresholdPixels(I) = Cover.IntermediateThreshold
                                                                    Else
                                                                        CountRemaining += 1
                                                                    End If
                                                                End If
                                                            End If
                                                        End If
                                                    Next

                                                    ThresholdRaster.Write({1}, ThresholdPixels)
                                                    IntermediateRaster.Write({1}, IntermediatePixels)

                                                    KillingFrostRaster.AdvanceBlock()
                                                    InitiationRaster.AdvanceBlock()
                                                    ThresholdRaster.AdvanceBlock()
                                                    IntermediateRaster.AdvanceBlock()
                                                    DailyRaster.AdvanceBlock()
                                                Loop

                                                DailyRaster.Close()
                                                IO.File.Delete(DailyRaster.Path)

                                                If CountRemaining = 0 Then Exit Do
                                                DayOfYear += 1

                                                If BackgroundWorker.CancellationPending Then : DoWorkEvent.Cancel = True : Exit Sub : End If
                                            Loop
                                        End Using

                                    End Using
                                End Using

                                ThresholdRaster.Close()
                                IO.File.Delete(ThresholdRaster.Path)
                            End If

                            KillingFrostRaster.Close()
                            IO.File.Delete(KillingFrostRaster.Path)

                            InitiationRaster.Close()
                            IO.File.Delete(InitiationRaster.Path)

                            IntermediateRaster.Close()

                            'Compress Intermediate Date Raster and Store in Database
                            Dim CompressedRasterPath As String = IntermediatePath & ".Compressed.tif"
                            Dim Process As New GDALProcess
                            Process.Translate({IntermediateRaster.Path}, CompressedRasterPath, , GDALProcess.Compression.DEFLATE)

                            Command.CommandText = String.Format("UPDATE Dates SET Intermediate = @File WHERE Year = '{0}'", Year)
                            Command.Parameters.Add("@File", DbType.Object).Value = IO.File.ReadAllBytes(CompressedRasterPath)
                            Command.ExecuteNonQuery()

                            IO.File.Delete(CompressedRasterPath)
                            IO.File.Delete(IntermediateRaster.Path)
                        Next

                        'Termination Date Selection
                        For X = 1 To 1
                            Dim TerminationPath = String.Format(TemporaryPath, "Termination Date")
                            Dim TerminationRaster = CreateNewRaster(TerminationPath, MaskRaster.XCount, MaskRaster.YCount, MaskRaster.Projection, MaskRaster.GeoTransform, {Int16.MinValue}, GDAL.DataType.GDT_Int16)

                            Command.CommandText = "SELECT KillingFrost FROM Dates WHERE Year = " & Year
                            Dim KillingFrostPath = String.Format(TemporaryPath, "Killing Frost")
                            IO.File.WriteAllBytes(KillingFrostPath, Command.ExecuteScalar)
                            Dim KillingFrostRaster As New Raster(KillingFrostPath)

                            Command.CommandText = "SELECT Intermediate FROM Dates WHERE Year = " & Year
                            Dim IntermediatePath = String.Format(TemporaryPath, "Intermediate Date")
                            IO.File.WriteAllBytes(IntermediatePath, Command.ExecuteScalar)
                            Dim IntermediateRaster As New Raster(IntermediatePath)

                            If Cover.TerminationThresholdType = ThresholdType.Number_of_Days Then
                                'Termination Type Day of Year
                                TerminationRaster.Open(GDAL.Access.GA_Update)
                                IntermediateRaster.Open(GDAL.Access.GA_ReadOnly)
                                KillingFrostRaster.Open(GDAL.Access.GA_ReadOnly)

                                Do Until TerminationRaster.BlocksProcessed
                                    Dim TerminationPixels = TerminationRaster.Read({1})
                                    Dim IntermediatePixels = IntermediateRaster.Read({1})
                                    Dim KillingFrostPixels = KillingFrostRaster.Read({1})

                                    Dim IntermediateNoDataValue = IntermediateRaster.BandNoDataValue(0)

                                    For I = 0 To TerminationPixels.Length - 1
                                        If IntermediatePixels(I) = IntermediateNoDataValue Then
                                            TerminationPixels(I) = Int16.MinValue
                                        Else
                                            TerminationPixels(I) = Cover.TerminationThreshold + IntermediatePixels(I)
                                            If TerminationPixels(I) > KillingFrostPixels(I) Then TerminationPixels(I) = KillingFrostPixels(I)
                                        End If
                                    Next

                                    TerminationRaster.Write({1}, TerminationPixels)

                                    KillingFrostRaster.AdvanceBlock()
                                    IntermediateRaster.AdvanceBlock()
                                    TerminationRaster.AdvanceBlock()
                                Loop

                                If BackgroundWorker.CancellationPending Then : DoWorkEvent.Cancel = True : Exit Sub : End If
                            Else
                                'Termination Type Variable Summation 
                                Dim ThresholdRaster = CreateNewRaster(String.Format(TemporaryPath, "Termination Threshold"), MaskRaster.XCount, MaskRaster.YCount, MaskRaster.Projection, MaskRaster.GeoTransform, {Single.MinValue})

                                Dim DateVariablePath As String = NLDAS_2AEvapotranspirationHargreavesPath
                                Select Case Cover.TerminationThresholdType
                                    Case ThresholdType.Growing_Degree_Days_Base_32F : DateVariablePath = NLDAS_2AGrowingDegreeDays32Path
                                    Case ThresholdType.Growing_Degree_Days_Base_41F : DateVariablePath = NLDAS_2AGrowingDegreeDays41Path
                                    Case ThresholdType.Growing_Degree_Days_Base_86F_and_50F : DateVariablePath = NLDAS_2AGrowingDegreeDays8650Path
                                End Select

                                'Set Start Date to Earliest Intermediate Date
                                Dim EarliestIntermediateDate As Integer = YearDayCount
                                IntermediateRaster.Open(GDAL.Access.GA_ReadOnly)
                                Do Until IntermediateRaster.BlocksProcessed
                                    Dim IntermediatePixels = IntermediateRaster.Read({1})

                                    Dim IntermediateNoDataValue = IntermediateRaster.BandNoDataValue(0)

                                    For I = 0 To IntermediatePixels.Length - 1
                                        If Not IntermediatePixels(I) = IntermediateNoDataValue Then
                                            If IntermediatePixels(I) < EarliestIntermediateDate Then EarliestIntermediateDate = IntermediatePixels(I)
                                        End If
                                    Next

                                    IntermediateRaster.AdvanceBlock()
                                Loop

                                'Loop Through Daily Rasters
                                Using DateVariableConnection = CreateConnection(DateVariablePath)
                                    DateVariableConnection.Open()

                                    Using DateVariableCommand = DateVariableConnection.CreateCommand
                                        DateVariableCommand.CommandText = "SELECT Image FROM Rasters WHERE Date >= @StartDate and Date <= @EndDate ORDER BY Date"
                                        DateVariableCommand.Parameters.Add("@StartDate", DbType.DateTime).Value = StartDate.AddDays(EarliestIntermediateDate - 1)
                                        DateVariableCommand.Parameters.Add("@EndDate", DbType.DateTime).Value = EndDate

                                        Using Reader = DateVariableCommand.ExecuteReader
                                            Dim DayOfYear As Integer = EarliestIntermediateDate

                                            Do While Reader.Read
                                                Dim DailyPath = String.Format(TemporaryPath, "Daily")
                                                IO.File.WriteAllBytes(DailyPath, Reader(0))
                                                Dim DailyRaster As New Raster(DailyPath)

                                                DailyRaster.Open(GDAL.Access.GA_ReadOnly)
                                                ThresholdRaster.Open(GDAL.Access.GA_Update)
                                                TerminationRaster.Open(GDAL.Access.GA_Update)
                                                IntermediateRaster.Open(GDAL.Access.GA_ReadOnly)
                                                KillingFrostRaster.Open(GDAL.Access.GA_ReadOnly)

                                                Dim CountRemaining As Integer = 0

                                                Do Until DailyRaster.BlocksProcessed
                                                    Dim DailyPixels = DailyRaster.Read({1})
                                                    Dim ThresholdPixels = ThresholdRaster.Read({1})
                                                    Dim TerminationPixels = TerminationRaster.Read({1})
                                                    Dim IntermediatePixels = IntermediateRaster.Read({1})
                                                    Dim KillingFrostPixels = KillingFrostRaster.Read({1})

                                                    Dim NoDataValue = DailyRaster.BandNoDataValue(0)

                                                    For I = 0 To DailyPixels.Length - 1
                                                        If DailyPixels(I) = NoDataValue Then
                                                            TerminationPixels(I) = Int16.MinValue
                                                        Else
                                                            'Set Termination Date if Threshold Reached
                                                            If DayOfYear >= IntermediatePixels(I) Then
                                                                If ThresholdPixels(I) < Cover.TerminationThreshold Then
                                                                    ThresholdPixels(I) += DailyPixels(I)
                                                                    If ThresholdPixels(I) >= Cover.TerminationThreshold Then
                                                                        TerminationPixels(I) = DayOfYear
                                                                    ElseIf KillingFrostPixels(I) <= DayOfYear Then
                                                                        TerminationPixels(I) = DayOfYear
                                                                        ThresholdPixels(I) = Cover.TerminationThreshold
                                                                    Else
                                                                        CountRemaining += 1
                                                                    End If
                                                                End If
                                                            End If
                                                        End If
                                                    Next

                                                    ThresholdRaster.Write({1}, ThresholdPixels)
                                                    TerminationRaster.Write({1}, TerminationPixels)

                                                    KillingFrostRaster.AdvanceBlock()
                                                    IntermediateRaster.AdvanceBlock()
                                                    ThresholdRaster.AdvanceBlock()
                                                    TerminationRaster.AdvanceBlock()
                                                    DailyRaster.AdvanceBlock()
                                                Loop

                                                DailyRaster.Close()
                                                IO.File.Delete(DailyRaster.Path)

                                                If CountRemaining = 0 Then Exit Do
                                                DayOfYear += 1

                                                If BackgroundWorker.CancellationPending Then : DoWorkEvent.Cancel = True : Exit Sub : End If
                                            Loop
                                        End Using

                                    End Using
                                End Using

                                ThresholdRaster.Close()
                                IO.File.Delete(ThresholdRaster.Path)
                            End If

                            KillingFrostRaster.Close()
                            IO.File.Delete(KillingFrostRaster.Path)

                            IntermediateRaster.Close()
                            IO.File.Delete(IntermediateRaster.Path)

                            TerminationRaster.Close()

                            'Compress Termination Date Raster and Store in Database
                            Dim CompressedRasterPath As String = TerminationPath & ".Compressed.tif"
                            Dim Process As New GDALProcess
                            Process.Translate({TerminationRaster.Path}, CompressedRasterPath, , GDALProcess.Compression.DEFLATE)

                            Command.CommandText = String.Format("UPDATE Dates SET Termination = @File WHERE Year = '{0}'", Year)
                            Command.Parameters.Add("@File", DbType.Object).Value = IO.File.ReadAllBytes(CompressedRasterPath)
                            Command.ExecuteNonQuery()

                            IO.File.Delete(CompressedRasterPath)
                            IO.File.Delete(TerminationRaster.Path)
                        Next

                        'Evapotranspiration Calculation
                        Using ReferenceConnection = CreateConnection(IO.Directory.GetFiles(IntermediateCalculationsDirectory, "*" & Cover.Variable & ".db", IO.SearchOption.AllDirectories)(0))
                            ReferenceConnection.Open()

                            Using ReferenceCommand = ReferenceConnection.CreateCommand
                                Command.CommandText = "SELECT Initiation FROM Dates WHERE Year = " & Year
                                Dim InitiationPath = String.Format(TemporaryPath, "Initiation Date")
                                IO.File.WriteAllBytes(InitiationPath, Command.ExecuteScalar)
                                Dim InitiationRaster As New Raster(InitiationPath)

                                Command.CommandText = "SELECT Intermediate FROM Dates WHERE Year = " & Year
                                Dim IntermediatePath = String.Format(TemporaryPath, "Intermediate Date")
                                IO.File.WriteAllBytes(IntermediatePath, Command.ExecuteScalar)
                                Dim IntermediateRaster As New Raster(IntermediatePath)

                                Command.CommandText = "SELECT Termination FROM Dates WHERE Year = " & Year
                                Dim TerminationPath = String.Format(TemporaryPath, "Termination Date")
                                IO.File.WriteAllBytes(TerminationPath, Command.ExecuteScalar)
                                Dim TerminationRaster As New Raster(TerminationPath)

                                Command.CommandText = "SELECT KillingFrost FROM Dates WHERE Year = " & Year
                                Dim KillingFrostPath = String.Format(TemporaryPath, "Killing Frost")
                                IO.File.WriteAllBytes(KillingFrostPath, Command.ExecuteScalar)
                                Dim KillingFrostRaster As New Raster(KillingFrostPath)

                                Dim CalculationStartDate As DateTime = StartDate
                                If CalculationStartDate < MinDate Then CalculationStartDate = MinDate
                                Dim CalculationEndDate As DateTime = EndDate
                                If CalculationEndDate > MaxDate Then CalculationEndDate = MaxDate

                                ReferenceCommand.CommandText = "SELECT Image FROM Rasters WHERE Date >= @StartDate and Date <= @EndDate ORDER BY Date"
                                ReferenceCommand.Parameters.Add("@StartDate", DbType.DateTime).Value = CalculationStartDate
                                ReferenceCommand.Parameters.Add("@EndDate", DbType.DateTime).Value = CalculationEndDate

                                Using Reader = ReferenceCommand.ExecuteReader
                                    Dim DayOfYear As Integer = CalculationStartDate.DayOfYear

                                    Do While Reader.Read
                                        Dim CoverPath = String.Format(TemporaryPath, "Cover Date")
                                        Dim CoverRaster = CreateNewRaster(CoverPath, MaskRaster.XCount, MaskRaster.YCount, MaskRaster.Projection, MaskRaster.GeoTransform, {Single.MinValue})

                                        Dim DailyPath = String.Format(TemporaryPath, "Daily")
                                        IO.File.WriteAllBytes(DailyPath, Reader(0))
                                        Dim DailyRaster As New Raster(DailyPath)

                                        CoverRaster.Open(GDAL.Access.GA_Update)
                                        DailyRaster.Open(GDAL.Access.GA_ReadOnly)
                                        InitiationRaster.Open(GDAL.Access.GA_Update)
                                        IntermediateRaster.Open(GDAL.Access.GA_Update)
                                        TerminationRaster.Open(GDAL.Access.GA_Update)
                                        KillingFrostRaster.Open(GDAL.Access.GA_ReadOnly)

                                        Dim CuttingCurve As Integer = 0

                                        Do Until DailyRaster.BlocksProcessed
                                            Dim CoverPixels = CoverRaster.Read({1})
                                            Dim DailyPixels = DailyRaster.Read({1})
                                            Dim InitiationPixels = InitiationRaster.Read({1})
                                            Dim IntermediatePixels = IntermediateRaster.Read({1})
                                            Dim TerminationPixels = TerminationRaster.Read({1})
                                            Dim KillingFrostPixels = KillingFrostRaster.Read({1})

                                            Dim NoDataValue = DailyRaster.BandNoDataValue(0)

                                            For I = 0 To DailyPixels.Length - 1
                                                Dim Kc As Single = 0

                                                Select Case DayOfYear
                                                    Case Is < InitiationPixels(I)
                                                        'No Evapotranspiration
                                                    Case Is <= IntermediatePixels(I)
                                                        'Initiation To Intermediate Curve Interpolation
                                                        Dim EndValue = IntermediatePixels(I)

                                                        Select Case Cover.InitiationToIntermediateCurveType
                                                            Case CurveType.Number_of_Days
                                                                Kc = CurveDaysInterpolation(DayOfYear, InitiationPixels(I), Cover.InitialCurve, CuttingCurve)
                                                            Case CurveType.Percent_Days
                                                                Kc = CurvePercentInterpolation(DayOfYear, InitiationPixels(I), EndValue, Cover.InitialCurve, CuttingCurve)
                                                        End Select
                                                    Case Is <= TerminationPixels(I)
                                                        'Intermediate To Termination Curve Interpolation
                                                        Dim EndValue = TerminationPixels(I)

                                                        Select Case Cover.IntermediateToTerminationCurveType
                                                            Case CurveType.Number_of_Days
                                                                Kc = CurveDaysInterpolation(DayOfYear, IntermediatePixels(I), Cover.FinalCurve, CuttingCurve)
                                                            Case CurveType.Percent_Days
                                                                Kc = CurvePercentInterpolation(DayOfYear, IntermediatePixels(I), EndValue, Cover.FinalCurve, CuttingCurve)
                                                        End Select

                                                        If Cover.SeasonalCurveType = SeasonalCurveType.Has_Cuttings Then
                                                            If DayOfYear = TerminationPixels(I) Then
                                                                'Only Number_of_Days Case Included for Cutting Intermediate and Termination Thresholds
                                                                Select Case KillingFrostPixels(I) - TerminationPixels(I)
                                                                    Case 0
                                                                        'End of Season
                                                                    Case Is > Cover.CuttingIntermediateThreshold + Cover.CuttingTerminationThreshold
                                                                        CuttingCurve = 1
                                                                        InitiationPixels(I) = TerminationPixels(I) + 1
                                                                        IntermediatePixels(I) = InitiationPixels(I) + Cover.CuttingIntermediateThreshold
                                                                        TerminationPixels(I) = IntermediatePixels(I) + Cover.CuttingTerminationThreshold
                                                                    Case Else
                                                                        CuttingCurve = 2
                                                                        InitiationPixels(I) = TerminationPixels(I) + 1
                                                                        IntermediatePixels(I) = InitiationPixels(I) + Cover.CuttingIntermediateThreshold
                                                                        If IntermediatePixels(I) > KillingFrostPixels(I) Then IntermediatePixels(I) = KillingFrostPixels(I)
                                                                        TerminationPixels(I) = IntermediatePixels(I) + Cover.CuttingTerminationThreshold
                                                                        If TerminationPixels(I) > KillingFrostPixels(I) Then TerminationPixels(I) = KillingFrostPixels(I)
                                                                End Select
                                                            End If
                                                        End If
                                                End Select

                                                CoverPixels(I) = Kc * DailyPixels(I)
                                            Next

                                            CoverRaster.Write({1}, CoverPixels)
                                            If Cover.SeasonalCurveType = SeasonalCurveType.Has_Cuttings Then
                                                InitiationRaster.Write({1}, InitiationPixels)
                                                IntermediateRaster.Write({1}, IntermediatePixels)
                                                TerminationRaster.Write({1}, TerminationPixels)
                                            End If

                                            InitiationRaster.AdvanceBlock()
                                            IntermediateRaster.AdvanceBlock()
                                            TerminationRaster.AdvanceBlock()
                                            DailyRaster.AdvanceBlock()
                                        Loop

                                        DailyRaster.Close()
                                        IO.File.Delete(DailyRaster.Path)

                                        'Close Cover Raster
                                        CoverRaster.Close()

                                        'Scale Cover Raster
                                        Dim ScaledRasterPath = CoverRaster.Path & ".Scaled.tif"
                                        ScaleRaster(CoverRaster.Path, ScaledRasterPath, , {"COMPRESS=DEFLATE"})

                                        'Store Raster in Database
                                        Command.CommandText = "INSERT OR REPLACE INTO Rasters (Date, Image) VALUES (@Date, @Image)"
                                        Command.Parameters.Add("@Date", DbType.DateTime).Value = StartDate.AddDays(DayOfYear - 1)
                                        Command.Parameters.Add("@Image", DbType.Object).Value = IO.File.ReadAllBytes(ScaledRasterPath)
                                        Command.ExecuteNonQuery()

                                        'Delete Intermediate Files
                                        IO.File.Delete(CoverRaster.Path)
                                        IO.File.Delete(ScaledRasterPath)

                                        DayOfYear += 1

                                        If BackgroundWorker.CancellationPending Then : DoWorkEvent.Cancel = True : Exit Sub : End If
                                    Loop
                                End Using

                                InitiationRaster.Close()
                                IO.File.Delete(InitiationRaster.Path)
                                IntermediateRaster.Close()
                                IO.File.Delete(IntermediateRaster.Path)
                                TerminationRaster.Close()
                                IO.File.Delete(TerminationRaster.Path)
                                KillingFrostRaster.Close()
                                IO.File.Delete(KillingFrostRaster.Path)
                            End Using
                        End Using
                    Next
                End Using
            End Using

            MaskRaster.Dispose()

            If BackgroundWorker.CancellationPending Then : DoWorkEvent.Cancel = True : Exit Sub : End If

            CalculateMonthlyAndAnnualStatistics(OutputDatabasePath, StatisticType.Sum, , MinDate)

            BackgroundWorker.ReportProgress(0, String.Format("{0} potential evapotranspiration calculated...", Cover.Name))
            'Next
        End Sub)
    End Sub

    Sub CalculateNetPotentialEvapotranspiration(CoverPaths() As String, CoverEffectivePrecipitation() As EffectivePrecipitationType, PrecipitationPath As String, MinDate As DateTime, MaxDate As DateTime, BackgroundWorker As System.ComponentModel.BackgroundWorker, DoWorkEvent As System.ComponentModel.DoWorkEventArgs)
        'Open Database Connections
        Dim Count = CoverPaths.Length - 1
        Dim ConnectionCover(Count) As Data.SQLite.SQLiteConnection
        For I = 0 To Count
            ConnectionCover(I) = CreateConnection(CoverPaths(I), False)
        Next
        Dim ConnectionPrecipitation = CreateConnection(PrecipitationPath)
        For Each Connection In ConnectionCover.Concat({ConnectionPrecipitation})
            Connection.Open()
        Next

        Dim CommandCover(Count) As Data.SQLite.SQLiteCommand
        For I = 0 To Count
            CommandCover(I) = ConnectionCover(I).CreateCommand

        Next
        Dim CommandPrecipitation = ConnectionPrecipitation.CreateCommand

        'Retrieve Selected Years from Statistic Tables
        Dim CommandText = String.Format("SELECT * FROM Statistics WHERE Year >= '{0}' and Year <= '{1}'", MinDate.Year, MaxDate.Year)
        For I = 0 To Count
            CommandCover(I).CommandText = CommandText
        Next
        CommandPrecipitation.CommandText = CommandText
        Dim ReaderPrecipitation As Data.SQLite.SQLiteDataReader = CommandPrecipitation.ExecuteReader

        'Load Slope Area Raster to Account for Pixel Area Increase in Evapotranspiration
        Dim SlopeAreaRaster As New Raster(MaskSlopeAreaRasterPath)

        'Prepare Output Paths
        Dim VariableFileName = "{0} Net Potential Evapotranspiration {1}.tif"
        Dim RasterPath As String = IO.Path.Combine(IO.Path.GetTempPath, VariableFileName)
        While ReaderPrecipitation.Read
            Dim Year As Integer = ReaderPrecipitation(0)
            Dim CoverRasterPaths(Count) As String
            Dim ScaledCoverRasterPaths(Count) As String
            For I = 0 To Count
                CoverRasterPaths(I) = String.Format(RasterPath, IO.Path.GetFileNameWithoutExtension(CoverPaths(I)).Replace(" Potential Evapotranspiration", ""), Year)
                ScaledCoverRasterPaths(I) = CoverRasterPaths(I) & ".Scaled.tif"

                CommandCover(I).CommandText = String.Format("INSERT OR REPLACE INTO Net (Year) VALUES ('{0}')", Year)
                CommandCover(I).ExecuteNonQuery()
            Next
            Dim PrecipitationRasterPath = String.Format(RasterPath, "Precipitation", Year) & " - .tif"

            'Select Start and End Months in Year
            Dim StartMonth As Integer = 1 : If Year = MinDate.Year Then StartMonth = MinDate.Month
            Dim EndMonth As Integer = 12 : If Year = MaxDate.Year Then EndMonth = MaxDate.Month

            'Check if Annual Calculation will be Executed
            Dim DoAnnualCalculation As Boolean = True
            If StartMonth > 1 And EndMonth < 12 Then
                For Month As Integer = 1 To StartMonth - 1
                    If ReaderPrecipitation.IsDBNull(Month) Then
                        DoAnnualCalculation = False
                        Exit For
                    End If
                Next
                For Month As Integer = 12 To EndMonth + 1 Step -1
                    If ReaderPrecipitation.IsDBNull(Month) Then
                        DoAnnualCalculation = False
                        Exit For
                    End If
                Next
            End If

            'For Each Selected Month
            For Month As Integer = StartMonth To EndMonth
                'Write Out Temporary Files for Each Potential Esvapotranspiration and Precipitation Monthly Raster
                For I = 0 To Count
                    CommandCover(I).CommandText = String.Format("SELECT Month{0} FROM Statistics WHERE Year = {1}", Month, Year)
                    IO.File.WriteAllBytes(CoverRasterPaths(I), CommandCover(I).ExecuteScalar)
                Next
                IO.File.WriteAllBytes(PrecipitationRasterPath, ReaderPrecipitation(Month))

                Dim CoverRasters(Count) As Raster
                Dim MonthlyRasterPaths(Count) As String
                Dim MonthlyRasters(Count) As Raster
                For I = 0 To Count
                    CoverRasters(I) = New Raster(CoverRasterPaths(I))
                    CoverRasters(I).Open(GDAL.Access.GA_ReadOnly)
                    MonthlyRasterPaths(I) = CoverRasterPaths(I) & ".Monthly.tif"
                    MonthlyRasters(I) = CreateNewRaster(MonthlyRasterPaths(I), CoverRasters(I).XCount, CoverRasters(I).YCount, CoverRasters(I).Projection, CoverRasters(I).GeoTransform, {Single.MinValue})
                    MonthlyRasters(I).Open(GDAL.Access.GA_Update)
                Next
                Dim PrecipitationRaster As New Raster(PrecipitationRasterPath)
                PrecipitationRaster.Open(GDAL.Access.GA_ReadOnly)
                SlopeAreaRaster.Open(GDAL.Access.GA_ReadOnly)

                'Calculate Net of Potential Evapotranspiration and Effective Precipitation
                Do Until PrecipitationRaster.BlocksProcessed
                    Dim CoverPixels(Count) As Array
                    Dim MonthlyPixels(Count) As Array
                    For I = 0 To Count
                        CoverPixels(I) = CoverRasters(I).Read({1})
                        MonthlyPixels(I) = MonthlyRasters(I).GetValuesArray({1})
                    Next
                    Dim PrecipitationPixels = PrecipitationRaster.Read({1})
                    Dim SlopeAreaPixels = SlopeAreaRaster.Read({1})

                    Dim NoDataValue = PrecipitationRaster.BandNoDataValue(0)

                    Threading.Tasks.Parallel.For(0, Count + 1, _
                    Sub(I)
                        'For I = 0 To Count
                        For J = 0 To PrecipitationPixels.Length - 1
                            If PrecipitationPixels(J) = NoDataValue Then
                                MonthlyPixels(I)(J) = Single.MinValue
                            Else
                                Dim EffectivePrecipitation As Single = 0
                                Select Case CoverEffectivePrecipitation(I)
                                    Case EffectivePrecipitationType.Eighty_Percent
                                        EffectivePrecipitation = 0.8 * PrecipitationPixels(J)
                                    Case EffectivePrecipitationType.One_Hundred_Percent
                                        EffectivePrecipitation = PrecipitationPixels(J)
                                    Case EffectivePrecipitationType.USDA_1970
                                        EffectivePrecipitation = CalculateUSDAEffectivePrecipitation(PrecipitationPixels(J), CoverPixels(I)(J))
                                End Select

                                MonthlyPixels(I)(J) = CoverPixels(I)(J) - EffectivePrecipitation / SlopeAreaPixels(J)
                                If MonthlyPixels(I)(J) < 0 Then MonthlyPixels(I)(J) = 0
                            End If
                        Next
                        'Next
                    End Sub)

                    For I = 0 To Count
                        MonthlyRasters(I).Write({1}, MonthlyPixels(I))
                    Next

                    SlopeAreaRaster.AdvanceBlock()
                    PrecipitationRaster.AdvanceBlock()
                    For I = 0 To Count
                        CoverRasters(I).AdvanceBlock()
                        MonthlyRasters(I).AdvanceBlock()
                    Next
                Loop

                For I = 0 To Count
                    CoverRasters(I).Dispose()
                    MonthlyRasters(I).Dispose()
                Next
                PrecipitationRaster.Dispose()

                'Scale Net Cover Rasters
                For I = 0 To Count
                    ScaleRaster(MonthlyRasterPaths(I), ScaledCoverRasterPaths(I), , {"COMPRESS=DEFLATE"})

                    CommandCover(I).CommandText = String.Format("UPDATE Net SET Month{0} = @Month{0} WHERE Year = '{1}'", Month, Year)
                    CommandCover(I).Parameters.Add("@Month" & Month, DbType.Object).Value = IO.File.ReadAllBytes(ScaledCoverRasterPaths(I))
                    CommandCover(I).ExecuteNonQuery()

                    IO.File.Delete(MonthlyRasterPaths(I))
                Next

                If BackgroundWorker.CancellationPending Then : DoWorkEvent.Cancel = True : Exit Sub : End If
                BackgroundWorker.ReportProgress(0)
            Next

            'Annual Summation
            If DoAnnualCalculation Then
                Threading.Tasks.Parallel.For(0, Count + 1, _
                Sub(I)
                    'For I = 0 To Count
                    Dim MaskRaster As New Raster(MaskRasterPath)
                    Dim AnnualRasterPath As String = CoverRasterPaths(I) & ".Annual.tif"
                    Dim AnnualRaster = CreateNewRaster(AnnualRasterPath, MaskRaster.XCount, MaskRaster.YCount, MaskRaster.Projection, MaskRaster.GeoTransform, {Single.MinValue}, GDAL.DataType.GDT_Float32)
                    MaskRaster.Dispose()

                    For Month As Integer = 1 To 12
                        CommandCover(I).CommandText = String.Format("SELECT Month{0} FROM Net WHERE Year = '{1}'", Month, Year)
                        IO.File.WriteAllBytes(CoverRasterPaths(I), CommandCover(I).ExecuteScalar)
                        Dim MonthlyRaster As New Raster(CoverRasterPaths(I))
                        MonthlyRaster.Open(GDAL.Access.GA_ReadOnly)
                        AnnualRaster.Open(GDAL.Access.GA_Update)

                        Do Until MonthlyRaster.BlocksProcessed
                            Dim MonthlyPixels = MonthlyRaster.Read({1})
                            Dim AnnualPixels = AnnualRaster.Read({1})

                            Dim NoDataValue = MonthlyRaster.BandNoDataValue(0)

                            For J = 0 To MonthlyPixels.Length - 1
                                If MonthlyPixels(J) = NoDataValue Then
                                    AnnualPixels(J) = Single.MinValue
                                Else
                                    AnnualPixels(J) += MonthlyPixels(J)
                                End If
                            Next

                            AnnualRaster.Write({1}, AnnualPixels)

                            AnnualRaster.AdvanceBlock()
                            MonthlyRaster.AdvanceBlock()
                        Loop

                        MonthlyRaster.Dispose()
                    Next
                    AnnualRaster.Dispose()

                    'Scale Annual Raster and Store in Database
                    ScaleRaster(AnnualRasterPath, ScaledCoverRasterPaths(I), , {"COMPRESS=DEFLATE"})

                    CommandCover(I).CommandText = String.Format("UPDATE Net SET Annual = @Annual WHERE Year = '{0}'", Year)
                    CommandCover(I).Parameters.Add("@Annual", DbType.Object).Value = IO.File.ReadAllBytes(ScaledCoverRasterPaths(I))
                    CommandCover(I).ExecuteNonQuery()

                    IO.File.Delete(AnnualRasterPath)
                    'Next
                End Sub)
            End If

            'Delete Temporary Files
            For I = 0 To Count
                IO.File.Delete(CoverRasterPaths(I))
                IO.File.Delete(ScaledCoverRasterPaths(I))
            Next
            IO.File.Delete(PrecipitationRasterPath)

            If BackgroundWorker.CancellationPending Then : DoWorkEvent.Cancel = True : Exit Sub : End If
            For I = 1 To 3 : BackgroundWorker.ReportProgress(0) : Next
        End While

        SlopeAreaRaster.Close()

        'Dispose of Database Objects
        For Each Com In CommandCover.Concat({CommandPrecipitation})
            Com.Dispose()
        Next
        For Each Connection In ConnectionCover.Concat({ConnectionPrecipitation})
            Connection.Dispose()
        Next
    End Sub

    Sub CalculateRasterPeriodAverages(DatabasePaths() As String, TableName() As DatabaseTableName, OutputDirectory As String, MinYear As Integer, MaxYear As Integer, BackgroundWorker As System.ComponentModel.BackgroundWorker, DoWorkEvent As System.ComponentModel.DoWorkEventArgs)
        'Open Database Containing Rasters
        Threading.Tasks.Parallel.For(0, DatabasePaths.Length, _
        Sub(J)
            Using Connection = CreateConnection(DatabasePaths(J), False)
                Connection.Open()

                Using Command = Connection.CreateCommand
                    'Open Mask Raster
                    Dim TemplateRaster As New Raster(MaskRasterPath)

                    'Determine Period
                    Command.CommandText = String.Format("SELECT MIN(Year) FROM {0} WHERE Annual IS NOT NULL", TableName(J).ToString)
                    Dim StartYear As Integer = Command.ExecuteScalar

                    Command.CommandText = String.Format("SELECT MAX(Year) FROM {0} WHERE Annual IS NOT NULL", TableName(J).ToString)
                    Dim EndYear As Integer = Command.ExecuteScalar

                    StartYear = Limit(MinYear, StartYear, EndYear)
                    EndYear = Limit(MaxYear, StartYear, EndYear)

                    Command.CommandText = String.Format("SELECT COUNT(Year) FROM {0} WHERE Year >= '{1}' AND Year <= '{2}' AND Annual IS NOT NULL", TableName(J).ToString, StartYear, EndYear)
                    Dim YearCount As Integer = Command.ExecuteScalar

                    'Prepare Output Raster
                    Dim VariableFileName = IO.Path.GetFileNameWithoutExtension(DatabasePaths(J))
                    If TableName(J) = DatabaseTableName.Net Then VariableFileName = VariableFileName.Insert(VariableFileName.Length - 29, " Net")
                    VariableFileName &= " (" & YearCount & " Year Average, " & StartYear & "-" & EndYear & ").tif"
                    Dim OutputRasterPath = IO.Path.Combine(OutputDirectory, VariableFileName)
                    Dim OutputRaster = CreateNewRaster(OutputRasterPath, TemplateRaster.XCount, TemplateRaster.YCount, TemplateRaster.Projection, TemplateRaster.GeoTransform, {Single.MinValue}, , {"TILED=YES", "COMPRESS=DEFLATE"}, 13)

                    Dim RasterPath As String = IO.Path.Combine(IO.Path.GetTempPath, VariableFileName)
                    For C = 0 To MonthAndAnnualNames.Length - 1
                        'Prepare Output Datasets for Period Average Calculation
                        Dim YearlyRasterPath = RasterPath & " - Yearly"
                        Dim PeriodRasterPath = RasterPath & MonthAndAnnualNames(C)
                        Dim PeriodRaster = CreateNewRaster(PeriodRasterPath, TemplateRaster.XCount, TemplateRaster.YCount, TemplateRaster.Projection, TemplateRaster.GeoTransform, {Single.MinValue})

                        'Extract Yearly Rasters and Sum in Period Dataset
                        Command.CommandText = String.Format("SELECT {0} FROM {1} WHERE Year >= '{2}' AND Year <= '{3}' AND Annual IS NOT NULL", MonthAndAnnualNames(C), TableName(J).ToString, StartYear, EndYear)
                        Using Reader = Command.ExecuteReader
                            Do Until Not Reader.Read
                                IO.File.WriteAllBytes(YearlyRasterPath, Reader(0))

                                Dim YearlyRaster As New Raster(YearlyRasterPath)
                                YearlyRaster.Open(GDAL.Access.GA_ReadOnly)
                                PeriodRaster.Open(GDAL.Access.GA_Update)
                                TemplateRaster.Open(GDAL.Access.GA_ReadOnly)

                                Do Until PeriodRaster.BlocksProcessed
                                    Dim PeriodPixels = PeriodRaster.Read({1})
                                    Dim YearlyPixels = YearlyRaster.Read({1})
                                    Dim TemplatePixels = TemplateRaster.Read({1})

                                    Dim NoDataValue = TemplateRaster.BandNoDataValue(0)

                                    For I = 0 To PeriodPixels.Length - 1
                                        If TemplatePixels(I) = NoDataValue Then
                                            PeriodPixels(I) = Single.MinValue
                                        Else
                                            PeriodPixels(I) += YearlyPixels(I)
                                        End If
                                    Next

                                    PeriodRaster.Write({1}, PeriodPixels)

                                    PeriodRaster.AdvanceBlock()
                                    YearlyRaster.AdvanceBlock()
                                    If BackgroundWorker.CancellationPending Then : DoWorkEvent.Cancel = True : Exit Sub : End If
                                Loop

                                YearlyRaster.Close()
                            Loop
                        End Using

                        'Divide by Number of Represented Years and Store in Output Dataset
                        PeriodRaster.Open(GDAL.Access.GA_ReadOnly)
                        OutputRaster.Open(GDAL.Access.GA_Update)
                        Do Until PeriodRaster.BlocksProcessed
                            Dim PeriodPixels = PeriodRaster.Read({1})

                            For I = 0 To PeriodPixels.Length - 1
                                If PeriodPixels(I) <> Single.MinValue Then PeriodPixels(I) /= YearCount
                            Next

                            OutputRaster.Write({C + 1}, PeriodPixels)

                            OutputRaster.AdvanceBlock()
                            PeriodRaster.AdvanceBlock()
                            If BackgroundWorker.CancellationPending Then : DoWorkEvent.Cancel = True : Exit Sub : End If
                        Loop
                        PeriodRaster.Close()

                        IO.File.Delete(PeriodRasterPath)
                        IO.File.Delete(YearlyRasterPath)

                        BackgroundWorker.ReportProgress(0)
                    Next

                    OutputRaster.AddStatistics()

                    'Set Band Descriptions
                    OutputRaster.Open(GDAL.Access.GA_Update)
                    For I = 1 To 13
                        Using Band = OutputRaster.Dataset.GetRasterBand(I)
                            Band.SetDescription(MonthAndAnnualNames(I - 1))
                        End Using
                    Next
                    OutputRaster.Close()

                    TemplateRaster.Dispose()
                    BackgroundWorker.ReportProgress(0)
                End Using
            End Using
        End Sub)
    End Sub

    Sub CalculateAverageRasterValueInPolygon(RasterPaths() As String, RasterVectorRelations() As List(Of String), VectorDatabasePath As String, VectorTableName As String, RelationField As String, OutputVectorPath As String, VectorFormat As GDALProcess.VectorFormat, BackgroundWorker As System.ComponentModel.BackgroundWorker, DoWorkEvent As System.ComponentModel.DoWorkEventArgs)
        'Open Intermediate Vector Database and Add Monthly Calculation Columns
        Dim CalculationColumns(MonthAndAnnualNames.Length) As String
        For I = 0 To CalculationColumns.Length - 2
            CalculationColumns(I) = "___" & MonthAndAnnualNames(I) & "___"
        Next
        CalculationColumns(MonthAndAnnualNames.Length) = "___Count___"

        Dim MaskRaster As New Raster(MaskRasterPath)
        Dim SpatialReferenceSystem = New OSR.SpatialReference(MaskRaster.Projection)

        Using ConnectionWrite = CreateConnection(VectorDatabasePath, False)
            ConnectionWrite.Open()

            Using CommandWrite = ConnectionWrite.CreateCommand
                Using Transaction = ConnectionWrite.BeginTransaction
                    For I = 0 To CalculationColumns.Length - 1
                        CommandWrite.CommandText = String.Format("ALTER TABLE ""{0}"" ADD COLUMN ""{1}"" FLOAT DEFAULT '0'", VectorTableName, CalculationColumns(I))
                        CommandWrite.ExecuteNonQuery()
                    Next

                    Transaction.Commit()
                End Using

                Dim CalculationColumnString = "(""" & String.Join(""",""", CalculationColumns) & ")"""
                Dim CalculationBands() = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13}

                'For Each Calculation
                For I = 0 To RasterPaths.Length - 1
                    If RasterVectorRelations(I).Count > 0 Then
                        'Rasterize Polygon FID's for Related Variables to Mask Raster Template
                        Dim RasterizedFIDPath = IO.Path.Combine(IO.Path.GetTempPath, String.Format("Extract by Polygon {0} Rasterized FID {1}.tif", VectorTableName, I))

                        Dim WhereExpression As New System.Text.StringBuilder()
                        For J = 0 To RasterVectorRelations(I).Count - 1
                            If J > 0 Then WhereExpression.Append(" OR")
                            WhereExpression.Append(String.Format(" ""{0}"" = '{1}'", RelationField, RasterVectorRelations(I)(J)))
                        Next

                        Dim Process As New GDALProcess
                        Process.Rasterize(VectorDatabasePath, RasterizedFIDPath, VectorTableName, "FID", MaskRaster.Extent, MaskRaster.XCount, MaskRaster.YCount, WhereExpression.ToString, , , Integer.MinValue)

                        'Open Rasterized FID and Calculation Rasters
                        Dim RasterizedFIDRaster As New Raster(RasterizedFIDPath)
                        RasterizedFIDRaster.Open(GDAL.Access.GA_ReadOnly)
                        Dim CalculationRaster As New Raster(RasterPaths(I))
                        CalculationRaster.Open(GDAL.Access.GA_ReadOnly)

                        'Add Rasterized Pixel Values to Polygon Database
                        Using Transaction = ConnectionWrite.BeginTransaction
                            Do Until RasterizedFIDRaster.BlocksProcessed
                                Dim RasterizedFIDPixels = RasterizedFIDRaster.Read({1})
                                Dim CalculationPixels = CalculationRaster.Read(CalculationBands)

                                Dim NoDataValue = RasterizedFIDRaster.BandNoDataValue(0)
                                Dim BandOffset = CalculationRaster.BlockYSize * CalculationRaster.XCount

                                For J = 0 To RasterizedFIDPixels.Length - 1
                                    If RasterizedFIDPixels(J) <> NoDataValue Then
                                        Dim CommandText As New System.Text.StringBuilder(String.Format("UPDATE ""{0}"" SET", VectorTableName))
                                        For K = 0 To CalculationBands.Length - 1
                                            CommandText.Append(String.Format(" ""{0}"" = ""{0}"" + '{1}',", CalculationColumns(K), CalculationPixels(BandOffset * K + J)))
                                        Next
                                        CommandText.Append(String.Format(" ""{0}"" = ""{0}"" + '1' WHERE ""ogc_fid"" = '{1}'", CalculationColumns(MonthAndAnnualNames.Length), RasterizedFIDPixels(J)))

                                        CommandWrite.CommandText = CommandText.ToString
                                        CommandWrite.ExecuteNonQuery()
                                    End If
                                Next

                                If BackgroundWorker.CancellationPending Then : DoWorkEvent.Cancel = True : Exit Sub : End If
                            Loop

                            Transaction.Commit()
                        End Using

                        'Add Interpolated Pixel Values for Polygons not Containing a Raster Pixel
                        Using Transaction = ConnectionWrite.BeginTransaction
                            Using ConnectionRead = CreateConnection(VectorDatabasePath)
                                ConnectionRead.Open()

                                Using CommandRead = ConnectionRead.CreateCommand
                                    CommandRead.CommandText = String.Format("SELECT ""ogc_fid"", ""GEOMETRY"" FROM ""{0}"" WHERE {1} AND ""{2}"" = '0'", VectorTableName, WhereExpression.ToString, CalculationColumns(MonthAndAnnualNames.Length))

                                    Using Reader = CommandRead.ExecuteReader
                                        While Reader.Read
                                            Dim FID = Reader.GetInt64(0)

                                            Using Geometry = OGR.Geometry.CreateFromWkb(Reader.GetValue(1)).Centroid
                                                Dim CentroidLocation = MaskRaster.CoordinateToPixelLocation(New Point64(Geometry.GetX(0), Geometry.GetY(0)))
                                                Dim X = Math.Floor(CentroidLocation.X)
                                                Dim Y = Math.Floor(CentroidLocation.Y)

                                                Dim Values(4 * CalculationRaster.BandCount - 1) As Single
                                                CalculationRaster.Dataset.ReadRaster(X, Y, 2, 2, Values, 2, 2, CalculationBands.Length, CalculationBands, Nothing, Nothing, Nothing)

                                                Dim CommandText As New System.Text.StringBuilder(String.Format("UPDATE ""{0}"" SET", VectorTableName))
                                                For J = 0 To CalculationBands.Length - 1
                                                    Dim Offset As Integer = 4 * J
                                                    Dim Value = BilinearInterpolation(New QuadValues(Values(Offset), Values(Offset + 1), Values(Offset + 2), Values(Offset + 3)), CentroidLocation.X - X, CentroidLocation.Y - Y)
                                                    CommandText.Append(String.Format(" ""{0}"" = ""{0}"" + '{1}',", CalculationColumns(J), Value))
                                                Next
                                                CommandText.Append(String.Format(" ""{0}"" = ""{0}"" + '1' WHERE ""ogc_fid"" = '{1}'", CalculationColumns(MonthAndAnnualNames.Length), FID))

                                                CommandWrite.CommandText = CommandText.ToString
                                                CommandWrite.ExecuteNonQuery()
                                            End Using

                                        End While
                                    End Using
                                End Using
                            End Using

                            Transaction.Commit()
                        End Using

                        'Release Memory
                        CalculationRaster.Close()
                        RasterizedFIDRaster.Close()
                        MaskRaster.Close()

                        IO.File.Delete(RasterizedFIDPath)

                        If BackgroundWorker.CancellationPending Then : DoWorkEvent.Cancel = True : Exit Sub : End If
                        BackgroundWorker.ReportProgress(0)
                    End If
                Next

                'Compute Polygon Average
                Dim ComputeCommandText As New System.Text.StringBuilder(String.Format("UPDATE ""{0}"" SET", VectorTableName))
                For J = 0 To CalculationColumns.Length - 2
                    If J > 0 Then ComputeCommandText.Append(",")
                    ComputeCommandText.Append(String.Format(" ""{0}"" = ""{0}"" / ""{1}""", CalculationColumns(J), CalculationColumns(MonthAndAnnualNames.Length)))
                Next
                ComputeCommandText.Append(String.Format(" WHERE ""{0}"" > '0'", CalculationColumns(MonthAndAnnualNames.Length)))

                CommandWrite.CommandText = ComputeCommandText.ToString
                CommandWrite.ExecuteNonQuery()

                If BackgroundWorker.CancellationPending Then : DoWorkEvent.Cancel = True : Exit Sub : End If
            End Using
        End Using

        'Extract Calculation Columns to Output Vector Dataset
        Dim ExtractCommandText As New System.Text.StringBuilder("SELECT ""ogc_fid"" AS ""INPUT_FID"", ""GEOMETRY"",")
        For I = 0 To CalculationColumns.Length - 1
            If I > 0 Then ExtractCommandText.Append(",")
            ExtractCommandText.Append(String.Format(" ""{0}"" AS ""{1}""", CalculationColumns(I), CalculationColumns(I).Replace("_", "")))
        Next
        ExtractCommandText.Append(String.Format(" FROM ""{0}""", VectorTableName))

        Dim ExtractProcess As New GDALProcess
        ExtractProcess.Ogr2Ogr(VectorDatabasePath, OutputVectorPath, VectorFormat, SpatialReferenceSystem, , True, ExtractCommandText.ToString)

        BackgroundWorker.ReportProgress(0)
    End Sub

#End Region

#Region "Weather Data"

    Function ExtractNLDAS_2A(Path As String, Optional XOffset As Integer = 0, Optional YOffset As Integer = 0, Optional XSize As Integer = -1, Optional YSize As Integer = -1) As Single()()
        ExtractNLDAS_2A = Nothing

        'Open GDAL Dataset 
        Using Dataset = GDAL.Gdal.Open(Path, GDAL.Access.GA_ReadOnly)
            If XSize < 0 Then XSize = Dataset.RasterXSize
            If YSize < 0 Then YSize = Dataset.RasterYSize

            'Extract Relavent Raster Bands
            Dim BandIndex = {NLDAS_2A.Air_Pressure, NLDAS_2A.Air_Temperature, NLDAS_2A.Precipitation, NLDAS_2A.Solar_Radiation, NLDAS_2A.Specific_Humidity, NLDAS_2A.Windspeed_U, NLDAS_2A.Windspeed_V}
            Dim Bands(BandIndex.Count - 1)() As Double
            Dim Length = XSize * YSize - 1
            For B = 0 To BandIndex.Count - 1
                ReDim Bands(B)(Length)
                Dataset.GetRasterBand(BandIndex(B)).ReadRaster(XOffset, YOffset, XSize, YSize, Bands(B), XSize, YSize, 0, 0)
            Next

            'Unit Conversions
            Dim ParameterCount = [Enum].GetNames(GetType(Hourly)).Length - 1
            Dim Converted(ParameterCount)() As Single
            For P = 0 To ParameterCount
                ReDim Converted(P)(Length)
            Next

            Dim U2 As Single = CalculateWindSpeedAdjustmentFactor(ToFeet(10))
            For I = 0 To Length
                If Not Bands(0)(I) = 9999 Then
                    Converted(Hourly.Air_Pressure)(I) = Bands(0)(I) / 1000 'Pa to kPa
                    Converted(Hourly.Air_Temperature)(I) = ToFahrenheit(Bands(1)(I)) 'C to F
                    Converted(Hourly.Precipitation)(I) = ToInches(Bands(2)(I)) 'kg/m^2 to in
                    Converted(Hourly.Relative_Humidity)(I) = CalculateRelativeHumidity(Bands(4)(I), Bands(0)(I), Bands(1)(I)) '%
                    Converted(Hourly.Solar_Radiation)(I) = ToLangleysPerHour(Bands(3)(I)) 'W/m^2 to Langley
                    Converted(Hourly.Wind_Vector_U)(I) = U2 * ToMilesPerHour(Bands(5)(I))  'm/s to mph
                    Converted(Hourly.Wind_Vector_V)(I) = U2 * ToMilesPerHour(Bands(6)(I)) 'm/s to mph
                Else
                    For P = 0 To ParameterCount
                        Converted(P)(I) = -9999
                    Next
                End If
            Next

            ExtractNLDAS_2A = Converted
        End Using
    End Function

    Sub CalculateMonthlyAndAnnualStatistics(DatabasePath As String, StatisticType As StatisticType, Optional Full As Boolean = False, Optional OverwriteMinDate As DateTime = Nothing, Optional OverwriteMaxDate As DateTime = Nothing)
        'Open Database Containing Rasters
        Using Connection = CreateConnection(DatabasePath, False)
            Connection.Open()

            Using Command = Connection.CreateCommand
                'Create Statistics Table (If It Doesn't Exist)
                If Full Then
                    Command.CommandText = "DROP TABLE IF EXISTS Statistics"
                    Command.ExecuteNonQuery()
                End If
                Command.CommandText = "CREATE TABLE IF NOT EXISTS Statistics (Year INTEGER UNIQUE, Month1 BLOB, Month2 BLOB, Month3 BLOB, Month4 BLOB, Month5 BLOB, Month6 BLOB, Month7 BLOB, Month8 BLOB, Month9 BLOB, Month10 BLOB, Month11 BLOB, Month12 BLOB, Annual BLOB)"
                Command.ExecuteNonQuery()

                If OverwriteMinDate <> Nothing And OverwriteMaxDate <> Nothing Then
                    Using Transaction = Connection.BeginTransaction

                        For Year As Integer = OverwriteMinDate.Year To OverwriteMaxDate.Year
                            Dim StartMonth = 1 : If Year = OverwriteMinDate.Year Then StartMonth = OverwriteMinDate.Month
                            Dim EndMonth = 12 : If Year = OverwriteMaxDate.Year Then EndMonth = OverwriteMaxDate.Month

                            For Month As Integer = StartMonth To EndMonth
                                Command.CommandText = String.Format("UPDATE OR IGNORE Statistics SET Month{0} = NULL WHERE Year = '{1}'", Month, Year)
                                Command.ExecuteNonQuery()
                            Next

                            Command.CommandText = String.Format("UPDATE OR IGNORE Statistics SET Annual = NULL WHERE Year = '{0}'", Year)
                            Command.ExecuteNonQuery()
                        Next

                        Transaction.Commit()
                    End Using
                End If

                'Retrieve Minimum and Maximum Image Dates
                Dim StartDate As DateTime
                Dim EndDate As DateTime
                Command.CommandText = "SELECT MIN(Date), MAX(Date) FROM Rasters"
                Using Reader = Command.ExecuteReader
                    Reader.Read()

                    StartDate = Reader(0)
                    If StartDate.Day <> 1 Then StartDate = StartDate.AddDays(DateTime.DaysInMonth(StartDate.Year, StartDate.Month) - (StartDate.Day - 1))
                    EndDate = Reader(1)
                    If EndDate.Day <> DateTime.DaysInMonth(EndDate.Year, EndDate.Month) Then EndDate = EndDate.AddDays(-EndDate.Day)
                End Using

                'Open Mask Raster
                Dim TemplateRaster As New Raster(MaskRasterPath)
                Dim VariableName = IO.Path.GetFileNameWithoutExtension(DatabasePath)

                StartDate = New DateTime(1986, 1, 1)
                EndDate = StartDate

                For Year As Integer = StartDate.Year To EndDate.Year
                    'Insert New Record into Database for Calculation Year
                    Command.CommandText = String.Format("INSERT OR IGNORE INTO Statistics (Year) VALUES ('{0}')", Year)
                    Command.ExecuteNonQuery()

                    Dim ColumnEmpty(12) As Boolean
                    Command.CommandText = String.Format("SELECT COUNT(Month1), COUNT(Month2), COUNT(Month3), COUNT(Month4), COUNT(Month5), COUNT(Month6), COUNT(Month7), COUNT(Month8), COUNT(Month9), COUNT(Month10), COUNT(Month11), COUNT(Month12), COUNT(Annual) FROM Statistics WHERE Year = '{0}'", Year)
                    Using Reader = Command.ExecuteReader
                        Reader.Read()

                        For I = 0 To 12
                            ColumnEmpty(I) = Not CBool(Reader(I))
                        Next
                    End Using

                    'Prepare Output Datasets for Monthly Calculations
                    Dim StartMonth = 1 : If Year = StartDate.Year Then StartMonth = StartDate.Month
                    Dim EndMonth = 12 : If Year = EndDate.Year Then EndMonth = EndDate.Month

                    Dim FileName As String = IO.Path.Combine(IO.Path.GetTempPath, "{0}-" & VariableName & Year & "-{1}.tif")

                    For Month As Integer = StartMonth To EndMonth
                        If ColumnEmpty(Month - 1) Then
                            Dim DailyRasterPath = String.Format(FileName, "Daily", Month)
                            Dim MonthlyRasterPath = String.Format(FileName, "Monthly", Month)
                            Dim MonthlyRaster = CreateNewRaster(MonthlyRasterPath, TemplateRaster.XCount, TemplateRaster.YCount, TemplateRaster.Projection, TemplateRaster.GeoTransform, {Single.MinValue})

                            'Extract Daily Rasters and Sum in Monthly Dataset
                            Command.CommandText = "SELECT IMAGE FROM Rasters WHERE Date >= @Date1 and Date < @Date2"
                            Command.Parameters.Add("@Date1", DbType.DateTime).Value = New DateTime(Year, Month, 1)
                            Command.Parameters.Add("@Date2", DbType.DateTime).Value = New DateTime(Year, Month, 1).AddMonths(1)
                            Dim DayCount As Integer = 0
                            Using Reader = Command.ExecuteReader
                                Do Until Not Reader.Read
                                    IO.File.WriteAllBytes(DailyRasterPath, Reader(0))

                                    Dim DailyRaster As New Raster(DailyRasterPath)
                                    DailyRaster.Open(GDAL.Access.GA_ReadOnly)
                                    MonthlyRaster.Open(GDAL.Access.GA_Update)

                                    Do Until MonthlyRaster.BlocksProcessed
                                        Dim MonthlyPixels = MonthlyRaster.Read({1})
                                        Dim DailyPixels = DailyRaster.Read({1})

                                        Dim NoDataValue = DailyRaster.BandNoDataValue(0)

                                        For I = 0 To MonthlyPixels.Length - 1
                                            If DailyPixels(I) = NoDataValue Then
                                                MonthlyPixels(I) = Single.MinValue
                                            Else
                                                MonthlyPixels(I) += DailyPixels(I)
                                            End If
                                        Next

                                        MonthlyRaster.Write({1}, MonthlyPixels)

                                        MonthlyRaster.AdvanceBlock()
                                        DailyRaster.AdvanceBlock()
                                    Loop

                                    DailyRaster.Close()

                                    DayCount += 1
                                Loop
                            End Using

                            'If Average Selected Divide by Number of Days in Month
                            If StatisticType = Calculations.StatisticType.Average Then
                                MonthlyRaster.Open(GDAL.Access.GA_Update)
                                Do Until MonthlyRaster.BlocksProcessed
                                    Dim MonthlyPixels = MonthlyRaster.Read({1})

                                    For I = 0 To MonthlyPixels.Length - 1
                                        If MonthlyPixels(I) <> Single.MinValue Then MonthlyPixels(I) /= DayCount
                                    Next

                                    MonthlyRaster.Write({1}, MonthlyPixels)

                                    MonthlyRaster.AdvanceBlock()
                                Loop
                            End If

                            MonthlyRaster.Close()

                            'Scale Monthly Output Raster and Insert into Database
                            Dim ScaledRasterPath As String = MonthlyRasterPath & ".Scaled.tif"
                            ScaleRaster(MonthlyRasterPath, ScaledRasterPath, , {"COMPRESS=DEFLATE"})

                            Command.CommandText = String.Format("UPDATE Statistics SET Month{0} = @Month{0} WHERE Year = '{1}'", Month, Year)
                            Command.Parameters.Add("@Month" & Month, DbType.Object).Value = IO.File.ReadAllBytes(ScaledRasterPath)
                            Command.ExecuteNonQuery()

                            IO.File.Delete(MonthlyRasterPath)
                            IO.File.Delete(DailyRasterPath)
                            IO.File.Delete(ScaledRasterPath)
                        End If
                    Next

                    If StartMonth = 1 And EndMonth = 12 Then
                        If ColumnEmpty(12) Then
                            'Prepare Output Datasets for Annual Calculations
                            Dim MonthlyRasterPath = String.Format(FileName, "Monthly", 13)
                            Dim AnnualRasterPath = String.Format(FileName, "Annual", 13)
                            Dim AnnualRaster = CreateNewRaster(AnnualRasterPath, TemplateRaster.XCount, TemplateRaster.YCount, TemplateRaster.Projection, TemplateRaster.GeoTransform, {Single.MinValue})

                            'Extract Monthly Rasters and Sum in Annual Dataset
                            For Month As Integer = 1 To 12
                                Command.CommandText = String.Format("SELECT Month{0} FROM Statistics WHERE Year = '{1}'", Month, Year)
                                IO.File.WriteAllBytes(MonthlyRasterPath, Command.ExecuteScalar)

                                Dim MonthlyRaster As New Raster(MonthlyRasterPath)
                                MonthlyRaster.Open(GDAL.Access.GA_ReadOnly)
                                AnnualRaster.Open(GDAL.Access.GA_Update)

                                Do Until AnnualRaster.BlocksProcessed
                                    Dim AnnualPixels = AnnualRaster.Read({1})
                                    Dim MonthlyPixels = MonthlyRaster.Read({1})

                                    Dim NoDataValue = MonthlyRaster.BandNoDataValue(0)

                                    For I = 0 To AnnualPixels.Length - 1
                                        If MonthlyPixels(I) = NoDataValue Then
                                            AnnualPixels(I) = Single.MinValue
                                        Else
                                            AnnualPixels(I) += MonthlyPixels(I)
                                        End If
                                    Next

                                    AnnualRaster.Write({1}, AnnualPixels)

                                    AnnualRaster.AdvanceBlock()
                                    MonthlyRaster.AdvanceBlock()
                                Loop

                                MonthlyRaster.Close()
                            Next

                            'If Average Selected Divide by Number of Represented Months in the Year
                            If StatisticType = Calculations.StatisticType.Average Then
                                AnnualRaster.Open(GDAL.Access.GA_Update)
                                Do Until AnnualRaster.BlocksProcessed
                                    Dim AnnualPixels = AnnualRaster.Read({1})

                                    For I = 0 To AnnualPixels.Length - 1
                                        If AnnualPixels(I) <> Single.MinValue Then AnnualPixels(I) /= 12
                                    Next

                                    AnnualRaster.Write({1}, AnnualPixels)

                                    AnnualRaster.AdvanceBlock()
                                Loop
                            End If

                            AnnualRaster.Close()

                            'Scale Monthly Output Raster and Insert into Database
                            Dim ScaledRasterPath1 As String = AnnualRasterPath & ".Scaled.tif"
                            ScaleRaster(AnnualRasterPath, ScaledRasterPath1, , {"COMPRESS=DEFLATE"})

                            Command.CommandText = String.Format("UPDATE Statistics SET Annual = @Annual WHERE Year = '{0}'", Year)
                            Command.Parameters.Add("@Annual", DbType.Object).Value = IO.File.ReadAllBytes(ScaledRasterPath1)
                            Command.ExecuteNonQuery()

                            IO.File.Delete(AnnualRasterPath)
                            IO.File.Delete(MonthlyRasterPath)
                            IO.File.Delete(ScaledRasterPath1)
                        End If
                    End If
                Next
            End Using
        End Using
    End Sub

    Public Sub GetMaxAndMinDates(Files() As String, ByRef MaxDate As DateTime, ByRef MinDate As DateTime, Optional TableName As DatabaseTableName = DatabaseTableName.Rasters)
        For Each File In Files
            Try
                Using Connection = CreateConnection(File, False)
                    Connection.Open()

                    Using Command = Connection.CreateCommand
                        Select Case TableName
                            Case DatabaseTableName.Rasters
                                Command.CommandText = "SELECT MAX(Date) FROM " & TableName.ToString
                                Dim Value = Command.ExecuteScalar
                                If Not IsDBNull(Value) Then
                                    If Convert.ToDateTime(Value) < MaxDate Then MaxDate = Value
                                Else
                                    MaxDate = DateTime.MaxValue
                                    MinDate = DateTime.MinValue
                                    Exit Sub
                                End If

                                Command.CommandText = "SELECT MIN(Date) FROM " & TableName.ToString
                                Value = Command.ExecuteScalar
                                If Not IsDBNull(Value) Then If Convert.ToDateTime(Value) > MinDate Then MinDate = Value
                            Case Else
                                Command.CommandText = String.Format("CREATE TABLE IF NOT EXISTS {0} (Year INTEGER UNIQUE, Month1 BLOB, Month2 BLOB, Month3 BLOB, Month4 BLOB, Month5 BLOB, Month6 BLOB, Month7 BLOB, Month8 BLOB, Month9 BLOB, Month10 BLOB, Month11 BLOB, Month12 BLOB, Annual BLOB)", TableName.ToString)
                                Command.ExecuteNonQuery()

                                Command.CommandText = String.Format("SELECT * FROM {0} WHERE (Month1 IS NOT NULL OR Month12 IS NOT NULL) ORDER BY Year LIMIT 1", TableName.ToString)
                                Using Reader = Command.ExecuteReader
                                    Reader.Read()

                                    For Month As Integer = 1 To 12
                                        If Not Reader.IsDBNull(Month) Then
                                            MinDate = New DateTime(Reader(0), Month, 1)
                                            Exit For
                                        End If
                                        If Month = 12 Then
                                            MaxDate = DateTime.MaxValue
                                            MinDate = DateTime.MinValue
                                            Exit Sub
                                        End If
                                    Next
                                End Using

                                Command.CommandText = String.Format("SELECT * FROM {0} WHERE (Month1 IS NOT NULL OR Month12 IS NOT NULL) ORDER BY Year DESC LIMIT 1", TableName.ToString)
                                Using Reader = Command.ExecuteReader
                                    Reader.Read()

                                    For Month As Integer = 12 To 1 Step -1
                                        If Not Reader.IsDBNull(Month) Then
                                            MaxDate = New DateTime(Reader(0), Month, DateTime.DaysInMonth(Reader(0), Month))
                                            Exit For
                                        End If
                                    Next
                                End Using
                        End Select

                    End Using

                End Using
            Catch Exception As Exception : Dim f = Exception.Message : End Try
        Next
    End Sub

    Sub CalculateLastSpringFrostDates(SpringFrostTemperatures() As Single, MinDate As DateTime, MaxDate As DateTime)
        'Open Mask Raster
        Dim TemplateRaster As New Raster(MaskRasterPath)

        'Open Database Containing Rasters
        Dim DatabasePath As String = NLDAS_2ALastSpringFrostPath
        Using Connection = CreateConnection(DatabasePath, False)
            Connection.Open()

            Using Command = Connection.CreateCommand
                'Create Rasters Table (If It Doesn't Exist)
                Command.CommandText = "CREATE TABLE IF NOT EXISTS Frosts (Year INTEGER UNIQUE)"
                Command.ExecuteNonQuery()

                'Determine Start and End Calculation Years in Database
                Dim ColumnNames As New List(Of String)
                Command.CommandText = "PRAGMA table_info(Frosts)"
                Using Reader = Command.ExecuteReader
                    Do While Reader.Read
                        ColumnNames.Add(Reader(1))
                    Loop
                End Using

                For Each Temperature In SpringFrostTemperatures.Except(ColumnNames)
                    Command.CommandText = String.Format("ALTER TABLE Frosts ADD COLUMN ""{0}"" BLOB", Temperature)
                Next

                Dim VariableName = IO.Path.GetFileNameWithoutExtension(DatabasePath)

                Dim StartYear As Integer = MinDate.Year
                If MinDate.DayOfYear > 180 Then StartYear += 1

                Command.CommandText = "SELECT MIN(Year) FROM Frosts"
                Dim Value = Command.ExecuteScalar
                If Value <> Nothing Then
                    If Value < StartYear Then StartYear = Value
                End If

                Dim EndYear As Integer = MaxDate.Year
                If MaxDate.DayOfYear < 181 Then EndYear -= 1

                For Year As Integer = StartYear To EndYear
                    'Insert New Record into Database for Calculation Year
                    Command.CommandText = String.Format("INSERT OR IGNORE INTO Frosts (Year) VALUES ('{0}')", Year)
                    Command.ExecuteNonQuery()

                    'Determine Null Temperature Entries
                    Dim CommandText As New System.Text.StringBuilder("SELECT ")
                    For Each Temperature In SpringFrostTemperatures
                        CommandText.Append(String.Format("""{0}"", ", Temperature))
                    Next
                    CommandText.Remove(CommandText.Length - 2, 2)
                    Command.CommandText = CommandText.Append(" FROM Frosts WHERE Year = " & Year).ToString

                    Dim UncalculatedTemperatures As New List(Of String)
                    Using Reader = Command.ExecuteReader
                        Reader.Read()

                        For I = 0 To SpringFrostTemperatures.Length - 1
                            If Reader.IsDBNull(I) Then UncalculatedTemperatures.Add(SpringFrostTemperatures(I))
                        Next
                    End Using

                    If UncalculatedTemperatures.Count > 0 Then
                        'Prepare Raster Dataset(s)
                        Dim FrostRasters(UncalculatedTemperatures.Count - 1) As Raster

                        Dim Path = IO.Path.Combine(IO.Path.GetTempPath, VariableName & "-" & Year & "_{0}.tif")
                        For I = 0 To UncalculatedTemperatures.Count - 1
                            Dim FrostRasterPath As String = String.Format(Path, UncalculatedTemperatures(I))
                            FrostRasters(I) = CreateNewRaster(FrostRasterPath, TemplateRaster.XCount, TemplateRaster.YCount, TemplateRaster.Projection, TemplateRaster.GeoTransform, {Single.MinValue})
                        Next

                        Using MinimumTemperatureConnection = CreateConnection(NLDAS_2AMinimumAirTemperaturePath, True)
                            MinimumTemperatureConnection.Open()

                            Using MinimumTemperatureCommand = MinimumTemperatureConnection.CreateCommand

                                'Extract Minimum Daily Temperature Rasters
                                Command.CommandText = "SELECT IMAGE FROM Rasters WHERE Date >= @Date1 and Date < @Date2 ORDER BY Date"
                                Command.Parameters.Add("@Date1", DbType.DateTime).Value = New DateTime(Year, 1, 1)
                                Command.Parameters.Add("@Date2", DbType.DateTime).Value = New DateTime(Year, 1, 1).AddDays(180)

                                Dim DailyRasterPath = String.Format(Path, "Daily")
                                Dim DayOfYear As Byte = 1

                                Using Reader = Command.ExecuteReader
                                    Do Until Not Reader.Read
                                        'Open Minimum Daily Temperature Raster
                                        IO.File.WriteAllBytes(DailyRasterPath, Reader(0))
                                        Dim DailyRaster As New Raster(DailyRasterPath)
                                        DailyRaster.Open(GDAL.Access.GA_ReadOnly)

                                        Dim NoDataValue = DailyRaster.BandNoDataValue(0)

                                        For Each FrostRaster In FrostRasters
                                            FrostRaster.Open(GDAL.Access.GA_Update)
                                        Next

                                        Do Until DailyRaster.BlocksProcessed
                                            Dim DailyPixels = DailyRaster.Read({1})

                                            'Check If Frost Occurs
                                            For F = 0 To FrostRasters.Length - 1
                                                Dim FrostPixels = FrostRasters(0).Read({1})

                                                For I = 0 To FrostPixels.Length - 1
                                                    If DailyPixels(I) = NoDataValue Then
                                                        FrostPixels(I) = Single.MinValue
                                                    Else
                                                        If DailyPixels(I) <= UncalculatedTemperatures(F) Then FrostPixels(I) = DayOfYear
                                                    End If
                                                Next

                                                FrostRasters(0).Write({1}, FrostPixels)

                                                FrostRasters(0).AdvanceBlock()
                                            Next

                                            DailyRaster.AdvanceBlock()
                                        Loop

                                        DailyRaster.Close()

                                        DayOfYear += 1
                                    Loop
                                End Using

                                IO.File.Delete(DailyRasterPath)
                            End Using
                        End Using

                        'Compress Output Raster(s) and Insert into Database
                        For F = 0 To FrostRasters.Length - 1
                            FrostRasters(F).Close()

                            Dim ScaledRasterPath As String = FrostRasters(F).Path & ".Scaled.tif"
                            ScaleRaster(FrostRasters(F).Path, ScaledRasterPath, , {"COMPRESS=DEFLATE"})

                            Command.CommandText = String.Format("UPDATE Frosts SET ""{0}"" = @Temperature WHERE Year = '{1}'", UncalculatedTemperatures(F), Year)
                            Command.Parameters.Add("@Temperature", DbType.Object).Value = IO.File.ReadAllBytes(ScaledRasterPath)
                            Command.ExecuteNonQuery()

                            IO.File.Delete(ScaledRasterPath)
                            IO.File.Delete(FrostRasters(F).Path)
                        Next
                    End If
                Next

                Dim YearCount As Integer = EndYear - StartYear + 1
                Dim VariableFileName = IO.Path.GetFileNameWithoutExtension(DatabasePath) & "_{0} (" & YearCount & " Year Average, " & StartYear & "-" & EndYear & ").tif"
                Dim RasterPath As String = IO.Path.Combine(IO.Path.GetTempPath, VariableFileName)
                For Each Temperature In SpringFrostTemperatures
                    'Prepare Output Datasets for Period Average Calculation
                    Dim YearlyRasterPath = String.Format(RasterPath, Temperature & ".Yearly")
                    Dim PeriodRasterPath = String.Format(RasterPath, Temperature)
                    Dim PeriodRaster = CreateNewRaster(PeriodRasterPath, TemplateRaster.XCount, TemplateRaster.YCount, TemplateRaster.Projection, TemplateRaster.GeoTransform, {Single.MinValue})

                    'Extract Yearly Rasters and Sum in Period Dataset
                    Command.CommandText = String.Format("SELECT ""{0}"" FROM  WHERE Year >= {1} and Year <= {2}", Temperature, StartYear, EndYear)
                    Using Reader = Command.ExecuteReader
                        Do Until Not Reader.Read
                            IO.File.WriteAllBytes(YearlyRasterPath, Reader(0))

                            Dim YearlyRaster As New Raster(YearlyRasterPath)
                            YearlyRaster.Open(GDAL.Access.GA_ReadOnly)
                            PeriodRaster.Open(GDAL.Access.GA_Update)

                            Do Until PeriodRaster.BlocksProcessed
                                Dim PeriodPixels = PeriodRaster.Read({1})
                                Dim YearlyPixels = YearlyRaster.Read({1})

                                Dim NoDataValue = YearlyRaster.BandNoDataValue(0)

                                For I = 0 To PeriodPixels.Length - 1
                                    If YearlyPixels(I) = NoDataValue Then
                                        PeriodPixels(I) = Single.MinValue
                                    Else
                                        PeriodPixels(I) += YearlyPixels(I)
                                    End If
                                Next

                                PeriodRaster.Write({1}, PeriodPixels)

                                PeriodRaster.AdvanceBlock()
                                YearlyRaster.AdvanceBlock()
                            Loop

                            YearlyRaster.Close()
                        Loop
                    End Using

                    'Divide by Number of Represented Years
                    PeriodRaster.Open(GDAL.Access.GA_Update)
                    Do Until PeriodRaster.BlocksProcessed
                        Dim PeriodPixels = PeriodRaster.Read({1})

                        For I = 0 To PeriodPixels.Length - 1
                            If PeriodPixels(I) <> Single.MinValue Then PeriodPixels(I) /= YearCount
                        Next

                        PeriodRaster.Write({1}, PeriodPixels)

                        PeriodRaster.AdvanceBlock()
                    Loop
                    PeriodRaster.Close()

                    'Move Calculation to Output Directory
                    Dim OutputRasterPath = IO.Path.Combine(OutputCalculationsDirectory, String.Format(VariableFileName, Temperature))
                    If IO.File.Exists(OutputRasterPath) Then IO.File.Delete(OutputRasterPath)
                    IO.File.Move(PeriodRasterPath, OutputRasterPath)

                    IO.File.Delete(YearlyRasterPath)
                Next
            End Using
        End Using
    End Sub

    Function SumProduct(Array1 As Double(), Array2() As Double) As Double
        Dim Sum As Double = 0

        For I = 0 To Array1.Length - 1
            Sum += Array1(I) * Array2(I)
        Next

        Return Sum
    End Function

    Function CurvePercentInterpolation(Value As Single, StartValue As Single, EndValue As Single, ByRef Curve(,) As Single, Optional CurveNumber As Integer = 0) As Single
        If Value > EndValue Then
            Return 0
        ElseIf Value = EndValue Then
            Return Curve(CurveNumber, Curve.GetLength(1) - 1)
        Else
            Dim Position As Double = (Value - StartValue) / (EndValue - StartValue) * (Curve.GetLength(1) - 1)
            Dim I As Integer = Int(Position)
            Dim Fraction As Double = Position - I
            Return Curve(CurveNumber, I) * (1 - Fraction) + Curve(CurveNumber, I + 1) * Fraction
        End If
    End Function

    Function CurveDaysInterpolation(Value As Single, StartValue As Single, ByRef Curve(,) As Single, Optional CurveNumber As Integer = 0) As Single
        Dim Position As Double = (Value - StartValue) / 10

        Dim Length = Curve.GetLength(1) - 1
        If Position >= Length Then
            Return 0
        ElseIf Position = Length Then
            Return Curve(CurveNumber, Length)
        End If

        Dim I As Integer = Int(Position)
        Dim Fraction As Double = Position - I

        Return Curve(CurveNumber, I) * (1 - Fraction) + Curve(CurveNumber, I + 1) * Fraction
    End Function

    Function GetCoverProperties() As CoverProperties()
        Using Connection = CreateConnection(ProjectDetailsPath)
            Connection.Open()

            Using Command = Connection.CreateCommand
                Dim CurveNames As New List(Of String)
                Dim CurveProperties As New List(Of String)
                Command.CommandText = "SELECT * FROM Curve"
                Using Reader = Command.ExecuteReader
                    Do Until Not Reader.Read
                        CurveNames.Add(Reader.GetString(0))
                        CurveProperties.Add(Reader.GetString(1))
                    Loop
                End Using

                Dim CoverProperties As New List(Of CoverProperties)
                Command.CommandText = "SELECT * FROM Cover"
                Using Reader = Command.ExecuteReader
                    Do Until Not Reader.Read
                        Dim Cover As New CoverProperties
                        Cover.Name = Reader.GetString(0)

                        Dim Properties = Reader.GetString(1).Split(";")

                        Cover.EffectivePrecipitationType = [Enum].Parse(GetType(EffectivePrecipitationType), Properties(0))

                        Cover.CurveName = Properties(1)

                        Cover.InitiationThresholdType = [Enum].Parse(GetType(ThresholdType), Properties(2))
                        Cover.InitiationThreshold = Properties(3)

                        Cover.IntermediateThresholdType = [Enum].Parse(GetType(ThresholdType), Properties(4))
                        Cover.IntermediateThreshold = Properties(5)

                        Cover.TerminationThresholdType = [Enum].Parse(GetType(ThresholdType), Properties(6))
                        Cover.TerminationThreshold = Properties(7)

                        Cover.CuttingIntermediateThresholdType = [Enum].Parse(GetType(ThresholdType), Properties(8))
                        Cover.CuttingIntermediateThreshold = Properties(10)

                        Cover.CuttingTerminationThresholdType = [Enum].Parse(GetType(ThresholdType), Properties(9))
                        Cover.CuttingTerminationThreshold = Properties(11)

                        Cover.SpringFrostTemperature = Properties(12)
                        Cover.KillingFrostTemperature = Properties(13)

                        Properties = CurveProperties(CurveNames.IndexOf(Cover.CurveName)).Split(";")

                        Cover.Variable = Properties(0)

                        Cover.SeasonalCurveType = [Enum].Parse(GetType(SeasonalCurveType), Properties(1))

                        Cover.InitiationToIntermediateCurveType = [Enum].Parse(GetType(CurveType), Properties(2))

                        Dim Values = Properties(3).Split(",")
                        Dim Length = CInt(Values.Length / 3 - 1)
                        ReDim Cover.InitialCurve(2, Length)
                        For Row = 0 To Length
                            For Col = 0 To 2
                                Dim Value = Values(Row * 3 + Col)
                                If IsNumeric(Value) Then Cover.InitialCurve(Col, Row) = Value
                            Next
                        Next

                        Cover.IntermediateToTerminationCurveType = [Enum].Parse(GetType(CurveType), Properties(4))

                        If Properties.Length > 5 Then
                            Values = Properties(5).Split(",")
                            Length = CInt(Values.Length / 3)
                            ReDim Cover.FinalCurve(2, Length)
                            For Col = 0 To 2
                                Cover.FinalCurve(Col, 0) = Cover.InitialCurve(Col, 10)
                            Next
                            For Row = 0 To Length - 1
                                For Col = 0 To 2
                                    Dim Value = Values(Row * 3 + Col)
                                    If IsNumeric(Value) Then Cover.FinalCurve(Col, Row + 1) = Value
                                Next
                            Next
                        End If

                        CoverProperties.Add(Cover)
                    Loop
                End Using

                Return CoverProperties.ToArray
            End Using
        End Using
    End Function

#End Region

#Region "ET Equations"

    ''' <summary>
    ''' Calculates hourly reference evapotranspiration from the ASCE standardized equation.
    ''' </summary>
    ''' <param name="Elevation">Height Above Mean Sea Level (Feet)</param>
    ''' <param name="Latitude">(Decimal Degrees)</param>
    ''' <param name="Longitude">(Decimal Degrees)</param>
    ''' <param name="RecordDate">Event Time at End of Hour (1-2 PM: Hour=14)</param>
    ''' <param name="AirTemperature">Average Hourly Temperature (Fahrenheit)</param>
    ''' <param name="RelativeHumidity">Average Hourly Relative Humidity (Percent)</param>
    ''' <param name="SolarRadiation">Total Hourly Incoming Solar Radiation (Langleys)</param>
    ''' <param name="ExtraterrestrialRadiation">Total Hourly Extraterrestrial Solar Radiation (Langleys)</param>
    ''' <param name="WindSpeed">Average Hourly Wind Speed (Miles/Hour)</param>
    ''' <param name="AnemometerHeight">Height of Anemometer from the Ground (Feet)</param>
    ''' <param name="ReferenceET">Short(Grass) or Long(Alfalfa) Reference Height</param>
    ''' <param name="AirPressure">Average Hourly Air Pressure (kiloPascals)</param>
    ''' <param name="DewpointTemperature">Output Calculated Dewpoint Temperature from Input Conditions (Fahrenheit)</param>
    ''' <returns>Estimated Reference Evapotranspiration (Inches/Hour)</returns>
    ''' <remarks>Source: ASCE Standardized Reference Evapotranspiration Equation (2005)</remarks>
    Function CalculateHourlyASCEReferenceET(Elevation As Double, Latitude As Double, Longitude As Double, RecordDate As DateTime, AirTemperature As Double, RelativeHumidity As Double, WindSpeed As Double, SolarRadiation As Double, ExtraterrestrialRadiation As Double, AnemometerHeight As Double, ReferenceET As ReferenceET, Optional AirPressure As Double = Double.MinValue, Optional ByRef DewpointTemperature As Single = Single.MinValue)
        'Mean Air Temperature (°C)
        Dim T As Double = ToCelsius(AirTemperature)

        'Elevation Above Mean Sea Level (m)
        Dim Z As Double = ToMeters(Elevation)

        'Atmospheric Pressure (kPa)
        Dim P As Double
        If AirPressure <> Double.MinValue Then
            P = AirPressure
        Else
            'P = 101.3 * ((293 - 0.0065 * Z) / 293) ^ 5.26
            P = (2.405994173 - 0.00005337529735 * Z) ^ 5.26
        End If

        'Psychrometric Constant (kPa/°C)
        Dim γ As Double = 0.000665 * P

        'Slope of the Saturation Vapor Pressure-Temperature Curve (kPa/°C)
        Dim Δ As Double = 2503 * Math.Exp(17.27 * T / (T + 237.3)) / (T + 237.3) ^ 2

        'Saturation Vapor Pressure (kPa)
        Dim Es As Double = 0.6108 * Math.Exp(17.27 * T / (T + 237.3))

        'Actual Vapor Pressure (kPa)
        Dim Ea As Double = 0.01 * RelativeHumidity * Es 'Conversion from Percent

        'Dewpoint Temperature (F)
        If DewpointTemperature = Single.MinValue Then
            Dim LogValue = Math.Log(1.637197119 * Ea)
            DewpointTemperature = ToFahrenheit(-23730 * LogValue / (-1727 + 100 * LogValue))
        End If

        'Extraterrestrial Radiation (MJ/(m^2*h))
        Dim Ra As Double = ToMJperM2(ExtraterrestrialRadiation)

        'Calculated Clear-sky Radiation (MJ/(m^2*h))
        Dim Rso As Double = (0.75 + 0.00002 * Z) * Ra

        'Measured Solar Radiation (MJ/(m^2*h))
        Dim Rs As Double = ToMJperM2(SolarRadiation)

        'Albedo (unitless)
        Dim α As Double = 0.23

        'Net Outgoing Long-Wave Radiation (MJ/(m^2*h))
        Dim Rns As Double = (1 - α) * Rs

        'Cloudiness Function (unitless)
        Dim Fcd As Double = 0.7
        If Rso > 0 Then Fcd = 1.35 * Limit(Rs / Rso, 0.3, 1) - 0.35

        'Stefan-Boltzmann Constant (MJ/(K^4*m^2*h))
        Dim σ As Double = 0.0000000002042

        'Net Shortwave Radiation (MJ/(m^2*h))
        Dim Rnl As Double = σ * Fcd * (0.34 - 0.14 * Ea ^ 0.5) * ToKelvin(T) ^ 4

        'Net Radiation (MJ/(m^2*h))
        Dim Rn As Double = Rns - Rnl

        'Soil Heat Flux Density (MJ/(m^2*h))
        Dim G As Double

        'Numerator Constant that Changes with Reference Type and Calculation Time Step (K*mm*s^3/(Mg*h))
        Dim Cn As Double

        'Denominator Constant that Changes with Reference Type and Calculation Time Step (s/m)
        Dim Cd As Double

        'Assignment of Reference Values
        If Rn >= 0 Then
            Select Case ReferenceET
                Case ReferenceET.ShortReference
                    G = 0.1 * Rn
                    Cn = 37
                    Cd = 0.24
                Case ReferenceET.LongReference
                    G = 0.04 * Rn
                    Cn = 66
                    Cd = 0.25
            End Select
        Else
            Select Case ReferenceET
                Case ReferenceET.ShortReference
                    G = 0.5 * Rn
                    Cn = 37
                    Cd = 0.96
                Case ReferenceET.LongReference
                    G = 0.2 * Rn
                    Cn = 66
                    Cd = 1.7
            End Select
        End If

        'Height of Wind Measurement Above Ground Surface (m)
        Dim Zw As Double = ToMeters(AnemometerHeight)

        'Measured Wind Speed at Zw (m/s)
        Dim Uz As Double = ToMetersPerSecond(WindSpeed)

        'Adjusted Wind Speed at 2 m Above Ground Surface (m/s)
        Dim U2 As Double = 4.87 * Uz / Math.Log(67.8 * Zw - 5.42)

        'Inverse Latent Heat of Vaporization (kg/MJ)
        Dim λ As Double = 0.408

        'ASCE Standardized Reference Evapotranspiration (mm/hr)
        Dim ET As Double = (λ * Δ * (Rn - G) + γ * Cn * U2 * (Es - Ea) / (T + 273)) / (Δ + γ * (1 + Cd * U2))

        Return ToInches(ET)
    End Function

    ''' <summary>
    ''' Calculates daily reference evapotranspiration from the Hargreaves-Samani equation.
    ''' </summary>
    ''' <param name="MinimumTemperature">Maximum Daily Temperature (Fahrenheit)</param>
    ''' <param name="AverageTemperature">Average Daily Temperature (Fahrenheit)</param>
    ''' <param name="MaximumTemperature">Minimum Daily Temperature (Fahrenheit)</param>
    ''' <param name="ExtraterrestrialRadiation">Total Daily Extraterrestrial Solar Radiation (Langleys)</param>
    ''' <returns>Estimated Reference Evapotranspiration (Inches/Day)</returns>
    ''' <remarks>Source: Hargreaves, G. H., Samani, Z. A. (1982). Estimating potential evapotranspiration. Journal of the Irrigation and Drainage Division, 108(3), 225-230.</remarks>
    Function CalculateDailyHargreavesReferenceET(MinimumTemperature As Double, AverageTemperature As Double, MaximumTemperature As Double, ExtraterrestrialRadiation As Double)
        Return AverageTemperature * (MaximumTemperature - MinimumTemperature) ^ 0.5 * ExtraterrestrialRadiation / 800000 ' 1340000
    End Function

    ''' <summary>
    ''' Calculates hourly open water surface evaporation for deep systems using the aerodynamic method.
    ''' </summary>
    ''' <param name="RecordDate">Event Time at End of Hour (1-2 PM: Hour=14)</param>
    ''' <param name="T">Average Hourly Temperature (Fahrenheit)</param>
    ''' <param name="P">Average Hourly Air Pressure (kiloPascals)</param>
    ''' <param name="RH">Average Hourly Relative Humidity (Percent)</param>
    ''' <param name="U">Average Hourly Wind Speed at 6.6 Feet Above the Surface (Miles/Hour)</param>
    ''' <returns>Deep Open Water Surface Evaporation (Inches/Hour)</returns>
    ''' <remarks>Source: Allen, R. G., Robison, C. W. (2009). Evapotranspiration and consumptive irrigation water requirements for Idaho: Supplement updating the time series through December 2008. Research Technical Completion Report: Kimberly Research and Extension Center, University of Idaho, Moscow, ID.</remarks>
    Function CalculateHourlyAerodynamicWaterSurfaceEvaporation(RecordDate As DateTime, T As Double, P As Double, RH As Double, U As Double)
        'Monthly Temperature Correction (F)
        Dim AerodynamicMonthlyTemperatureCorrection = {7.2, 5.4, 1.8, 0, 0, 0, 0, 1.8, 1.8, 5.4, 7.2, 7.2}

        'Air Temperature (C)
        Dim Ta As Double = ToCelsius(T)

        'Air Vapor Pressure (kPa)
        Dim Ea As Double = 0.01 * RH * (0.6108 * Math.Exp(17.27 * Ta / (Ta + 237.3)))

        'Saturated Air Specific Humidity (Unitless)
        Dim Qa As Double = CalculateSpecificHumidity(Ea, P)

        'Water Surface Temperature (C)
        Dim Tw As Double = ToCelsius(T + AerodynamicMonthlyTemperatureCorrection(RecordDate.Month - 1))

        'Water Surface Vapor Pressure
        Dim Ew As Double = 0.6108 * Math.Exp(17.27 * Tw / (Tw + 237.3))

        'Water Surface Specific Humidity (Unitless)
        Dim Qw As Double = CalculateSpecificHumidity(Ew, P)

        'Water Vapor Bulk Transfer Coefficient (Unitless)
        Dim Ce As Double = 0.0014

        'Moist Air Denisty (kg/m^3)
        Dim ρ As Double = 1.2929 * (273.13 / (Ta + 273.12)) * (7.5 * (P - 0.3783 * Ea) / 760)

        'Aerodynamic Water Surface Evaporation (mm)
        Dim E = ρ * Ce * (U * 0.5977) * (Qw - Qa) * 3600

        Return ToInches(E)
    End Function

    ''' <summary>
    ''' Calculates total incoming extraterrestrial radiation for a given location and time period on the earth for a sloped surface.
    ''' </summary>
    ''' <param name="RecordDate">Event Time at End of Hour (1-2 PM: Hour=14)</param>
    ''' <param name="Latitude">(Decimal Degrees)</param>
    ''' <param name="Longitude">(Decimal Degrees)</param>
    ''' <param name="UTCOffset">Offset from Universal Time (Hours)</param>
    ''' <param name="Slope">Positive Inclination from Horizontal (Decimal Degrees)</param>
    ''' <param name="Azimuth">Positive Clockwise Slope Orientation from North(0) [East(90), South(180), West(270)] (Decimal Degrees)</param>
    ''' <returns>Estimated Extraterrestrial Radiation for a Sloped Surface (MJ/(m^2*h)</returns>
    ''' <remarks>Adapted from: Allen, R. G., Trezza, R., Tasumi, M. (2006). Analytical integrated functions for daily solar radiation on slopes. Agricultural and Forest Meteorology, 139(1), 55-73.</remarks>
    Function CalculateHourlyRa(RecordDate As DateTime, Latitude As Double, Longitude As Double, UTCOffset As Double, Slope As Double, Azimuth As Double) As Double
        'Longitudinal Datum Reference Conversions
        If Longitude <= 0 Then
            Longitude = -Longitude
        Else
            Longitude = 360 - Longitude
        End If
        If UTCOffset >= 0 Then
            UTCOffset = UTCOffset * 15
        Else
            UTCOffset = 360 + UTCOffset * 15
        End If

        'Solar Constant (MJ/(m^2*h))
        Dim Gsc As Double = 4.92

        'Inverse Relative Distance Factor Squared for the Earth-Sun (unitless)
        Dim Dr As Double = 1 + 0.033 * Math.Cos((2 / 365) * π * RecordDate.DayOfYear)

        'Solar Declination (radians)
        Dim δ As Double = 0.409 * Math.Sin((2 / 365) * π * RecordDate.DayOfYear - 1.39)

        'Latitude (radians)
        Dim ϕ As Double = ToRadians(Latitude)

        'Slope (radians)
        Dim β As Double = ToRadians(Slope)

        'Aspect (radians)
        Dim γ As Double = ToRadians(Azimuth - 180)

        'Sine and Cosine of Angles
        Dim Sinδ As Double = Math.Sin(δ)
        Dim Sinϕ As Double = Math.Sin(ϕ)
        Dim Sinβ As Double = Math.Sin(β)
        Dim Sinγ As Double = Math.Sin(γ)
        Dim Cosδ As Double = Math.Cos(δ)
        Dim Cosϕ As Double = Math.Cos(ϕ)
        Dim Coss As Double = Math.Cos(β)
        Dim Cosγ As Double = Math.Cos(γ)

        'Flat Slope Sunset Time Angle (radians)
        Dim ωss As Double = Math.Acos(-Math.Tan(ϕ) * Math.Tan(δ))
        If Math.Abs(δ + ϕ) > π / 2 Then ωss = π
        If Math.Abs(δ - ϕ) > π / 2 Then ωss = 0

        'Flat Slope Sunrise Time Angle (radians)
        Dim ωsr As Double = -ωss

        'Solar-Slope Function Simplifications (radians)
        Dim A As Double = Sinδ * Cosϕ * Sinβ * Cosγ - Sinδ * Sinϕ * Coss
        Dim B As Double = Cosδ * Cosϕ * Coss + Cosδ * Sinϕ * Sinβ * Cosγ
        Dim C As Double = Cosδ * Sinβ * Sinγ

        'Numerical Stability Limit
        Dim LowerLimit As Double = 0.0000000000000001
        If Math.Abs(C) < LowerLimit Then C = LowerLimit

        'Flat Slope Time Angle Checks
        Dim Lωsr As Double = CalculateSolarSlopeIntegrationLimit(ωsr, A, B, C)
        Dim Lωss As Double = CalculateSolarSlopeIntegrationLimit(ωss, A, B, C)

        'Sloped Sunrise/Sunset Time Angle Equation, Full (radians)
        'ω = Math.Atan2((A * C ^ 2 ± B * Math.Sqrt(B ^ 2 * C ^ 2 + C ^ 4 - C ^ 2 * A ^ 2)) / ((B ^ 2 + C ^ 2) * C), (A * B ± Math.Sqrt(B ^ 2 * C ^ 2 + C ^ 4 - C ^ 2 * A ^ 2)) / (B ^ 2 + C ^ 2))

        'More Efficient Breakdown of Sloped Sunrise/Sunset Time Angle Equation
        Dim C2 As Double = C ^ 2
        Dim AB As Double = A * B
        Dim AC2 As Double = A * C2
        Dim B2pC2 As Double = B ^ 2 + C ^ 2
        Dim CBA As Double = C2 * (C2 + B ^ 2 - A ^ 2)
        If CBA < LowerLimit Then CBA = LowerLimit ^ 2
        CBA = (CBA) ^ 0.5

        'Sloped Sunrise/Sunset Time Angles (radians)
        Dim ωab(3) As Double
        ωab(0) = Math.Atan2((AC2 - B * CBA) / (B2pC2 * C), (AB + CBA) / B2pC2)
        ωab(1) = Math.Atan2((AC2 + B * CBA) / (B2pC2 * C), (AB - CBA) / B2pC2)
        ωab(2) = Math.Atan2((AC2 + B * CBA) / (B2pC2 * C), (AB + CBA) / B2pC2)
        ωab(3) = Math.Atan2((AC2 - B * CBA) / (B2pC2 * C), (AB - CBA) / B2pC2)
        Array.Sort(ωab)

        'Sloped Sunset Time Angle (radians)
        Dim ωb As Double = ωab(2)
        Dim Lωb As Double = CalculateSolarSlopeIntegrationLimit(ωb, A, B, C)
        If Lωss > Lωb Or Lωb > 0.001 Then
            Dim ωbx As Double = π - ωb
            Dim Lωbx As Double = CalculateSolarSlopeIntegrationLimit(ωbx, A, B, C)
            If Lωbx <= 0.001 And ωbx < ωss Then
                ωb = ωbx
            Else
                ωb = ωss
            End If
        End If
        If ωb > ωss Then ωb = ωss

        'Sloped Sunrise Time Angle (radians)
        Dim ωa As Double = ωab(1)
        Dim Lωa As Double = CalculateSolarSlopeIntegrationLimit(ωa, A, B, C)
        If Lωsr > Lωa Or Lωa > 0.001 Then
            Dim ωax As Double = -π - ωa
            Dim Lωax As Double = CalculateSolarSlopeIntegrationLimit(ωax, A, B, C)
            If Lωax <= 0.001 And ωax > ωsr Then
                ωa = ωax
            Else
                ωa = ωsr
            End If
        End If
        If ωa < ωsr Then ωa = ωsr
        If ωa > ωb Then ωa = ωb

        'Seasonal Correction Factor (radians)
        Dim Bsc As Double = 2 * π * (RecordDate.DayOfYear - 81) / 364

        'Seasonal Correction for Solar Time (hour)
        Dim Sc As Double = 0.1645 * Math.Sin(2 * Bsc) - 0.1255 * Math.Cos(Bsc) - 0.025 * Math.Sin(Bsc)

        'Solar Time Angle at Midpoint of Period (radians)
        Dim ω As Double = π / 12 * ((RecordDate.Hour + 0.5 + 0.06667 * (UTCOffset - Longitude) + Sc) - 12)

        'Solar Time Angle at End of Period (radians)
        Dim ω2 As Double = Limit(ω + π / 24, ωa, ωb)

        'Solar Time Angle at Beginning of Period (radians)
        Dim ω1 As Double = Limit(ω - π / 24, ωa, ωb)
        If ω1 > ω2 Then ω1 = ω2

        'Extraterrestrial Radiation (MJ/(m^2*h))
        Dim Ra As Double = 12 / π * Gsc * Dr * ((ω2 - ω1) * Sinϕ * Sinδ + Cosϕ * Cosδ * (Math.Sin(ω2) - Math.Sin(ω1)))

        'Solar Angle of Sun Above the Horizon (radians)
        'Dim βe As Double = Math.Asin(Math.Sin(ϕ) * Math.Sin(δ) + Math.Cos(ϕ) * Math.Cos(δ) * Math.Cos(ω))

        Return Ra
    End Function

    ''' <summary>
    ''' Calculates total incoming extraterrestrial radiation for a given location and time period on the earth for a horizontal surface.
    ''' </summary>
    ''' <param name="RecordDate">Event Time at End of Hour (1-2 PM: Hour=14)</param>
    ''' <param name="Latitude">(Decimal Degrees)</param>
    ''' <param name="Longitude">(Decimal Degrees)</param>
    ''' <param name="UTCOffset">Offset from Universal Time (Hours)</param>
    ''' <returns>Estimated Extraterrestrial Radiation for a Horizontal Surface (MJ/(m^2*h)</returns>
    ''' <remarks>Source: ASCE Standardized Reference Evapotranspiration Equation (2005)</remarks>
    Function CalculateHourlyRa(RecordDate As DateTime, Latitude As Double, Longitude As Double, UTCOffset As Double)
        'Longitudinal Datum Reference Conversions
        If Longitude <= 0 Then
            Longitude = -Longitude
        Else
            Longitude = 360 - Longitude
        End If
        If UTCOffset >= 0 Then
            UTCOffset = UTCOffset * 15
        Else
            UTCOffset = 360 + UTCOffset * 15
        End If

        'Solar Constant (MJ/(m^2*h))
        Dim Gsc As Double = 4.92

        'Inverse Relative Distance Factor Squared for the Earth-Sun (unitless)
        Dim Dr As Double = 1 + 0.033 * Math.Cos((2 / 365) * π * RecordDate.DayOfYear)

        'Latitude (radians)
        Dim ϕ As Double = ToRadians(Latitude)

        'Solar Declination (radians)
        Dim δs As Double = 0.409 * Math.Sin((2 / 365) * π * RecordDate.DayOfYear - 1.39)

        'Seasonal Correction Factor (radians)
        Dim B As Double = 2 * π * (RecordDate.DayOfYear - 81) / 364

        'Seasonal Correction for Solar Time (hour)
        Dim Sc As Double = 0.1645 * Math.Sin(2 * B) - 0.1255 * Math.Cos(B) - 0.025 * Math.Sin(B)

        'Sunset/-Sunrise Time Angle (radians)
        Dim ωs As Double = Math.Acos(-Math.Tan(ϕ) * Math.Tan(δs))

        'Solar Time Angle at Midpoint of Period (radians)
        Dim ω As Double = π / 12 * ((RecordDate.Hour + 0.5 + 0.06667 * (UTCOffset - Longitude) + Sc) - 12)

        'Solar Time Angle at End of Period (radians)
        Dim ω2 As Double = Limit(ω + π / 24, -ωs, ωs)

        'Solar Time Angle at Beginning of Period (radians)
        Dim ω1 As Double = Limit(ω - π / 24, -ωs, ωs)
        If ω1 > ω2 Then ω1 = ω2

        'Extraterrestrial Radiation (MJ/(m^2*h))
        Dim Ra As Double = 12 / π * Gsc * Dr * ((ω2 - ω1) * Math.Sin(ϕ) * Math.Sin(δs) + Math.Cos(ϕ) * Math.Cos(δs) * (Math.Sin(ω2) - Math.Sin(ω1)))

        Return Ra
    End Function

    ''' <summary>
    ''' Calculates instantaneous incoming extraterrestrial radiation for a given location and time period on the earth for a sloped surface.
    ''' </summary>
    ''' <param name="Longitude">(Decimal Degrees)</param>
    ''' <param name="Latitude">(Decimal Degrees)</param>
    ''' <param name="Elevation">Height Above Mean Sea Level (Feet)</param>
    ''' <param name="Slope">Positive Inclination from Horizontal (Decimal Degrees)</param>
    ''' <param name="Azimuth">Positive Clockwise Slope Orientation from North(0) [East(90), South(180), West(270)] (Decimal Degrees)</param>
    ''' <param name="Temperature">Air Temperature (Fahrenheit)</param>
    ''' <param name="Pressure">Air Pressure (kPa)</param>
    ''' <param name="SolarPosition">Earth-Sun Orientation and Distance</param>
    ''' <returns>Estimated Extraterrestrial Radiation for a Sloped Surface (Langleys/Hour)</returns>
    ''' <remarks>Adapted from: Reda, I., Andreas, A. (2004). Solar position algorithm for solar radiation applications. Solar energy, 76(5), 577-589.</remarks>
    Function CalculateInstantaneousRa(Longitude As Double, Latitude As Double, Elevation As Double, Slope As Double, Azimuth As Double, Temperature As Double, Pressure As Double, SolarPosition As SolarPosition) As Double
        'Variable Conversions
        Dim σ As Double = ToRadians(Longitude)
        Dim ϕ As Double = ToRadians(Latitude)
        Dim Z As Double = ToMeters(Elevation)
        Dim ω As Double = ToRadians(Slope)
        Dim γ As Double = ToRadians(Azimuth - 180)

        Dim Cosϕ As Double = Math.Cos(ϕ)
        Dim Sinϕ As Double = Math.Sin(ϕ)

        'Observer Local Hour Angle (radians)
        Dim H As Double = LimitAngle(SolarPosition.ν + σ - SolarPosition.α)
        Dim CosH As Double = Math.Cos(H)

        'Equatorial Horizontal Parallax of the Sun (radians)
        Dim ξ As Double = ToRadians(8.794 / (3600 * SolarPosition.R))
        Dim Sinξ As Double = Math.Sin(ξ)

        'Parallax in the Sun Right Ascension
        Dim U As Double = Math.Atan(0.99664719 * Math.Tan(ϕ))
        Dim X As Double = Math.Cos(U) + Z * Cosϕ / 6378140
        Dim Y As Double = 0.99664719 * Math.Sin(U) + Z * Sinϕ / 6378140
        Dim Δα As Double = Math.Atan2(-X * Sinξ * Math.Sin(H), Math.Cos(SolarPosition.δ) - X * Sinξ * CosH)

        'Topocentric Sun Right Ascension (radians)
        'Dim αp As Double = SolarPosition.α + Δα

        'Topocentric Sun Declination (radians)
        Dim δp As Double = Math.Atan2((Math.Sin(SolarPosition.δ) - Y * Sinξ) * Math.Cos(Δα), Math.Cos(SolarPosition.δ) - X * Sinξ * CosH)

        'Topocentric Local Hour Angle (radians)
        Dim Hp As Double = H - Δα
        Dim CosHp As Double = Math.Cos(Hp)

        'Topocentric Elevation Angle Without Atmospheric Refraction (radians)
        Dim e0 As Double = Math.Asin(Sinϕ * Math.Sin(δp) + Cosϕ * Math.Cos(δp) * CosHp)

        'Atmospheric Refraction Correction (radians)
        Dim Δe As Double = ToRadians((Pressure / 101) * (283 / (273 + ToCelsius(Temperature))) * 1.02 / (60 * Math.Tan(ToRadians(ToDegrees(e0) + 10.3 / (ToDegrees(e0) + 5.11)))))

        'Topocentric Elevation Angle (radians)
        Dim e As Double = e0 + Δe

        'Zenith Angle (radians)
        Dim θ As Double = π / 2 - e

        'Topocentric Astronomers Azimuth Angle (radians)
        Dim Γc As Double = Math.Atan2(Math.Sin(Hp), CosHp * Sinϕ - Math.Tan(δp) * Cosϕ)

        'Topocentric Azimuth Angle (radians)
        Dim Φ As Double = LimitAngle(Γc + π)

        'Incidence Angle for a Surface Oriented in Any Direction
        Dim I As Double = Math.Acos(Math.Cos(θ) * Math.Cos(ω) + Math.Sin(ω) * Math.Sin(θ) * Math.Cos(Γc - γ))

        'Solar Constant (W/m^2)
        Dim Gsc As Double = 1367

        'Extraterrestrial Radiation (W/m^2)
        Dim Ra As Double = 0
        If e > 0 And π / 2 - I > 0 Then Ra = Gsc * Math.Cos(I) / SolarPosition.R ^ 2

        Return ToLangleysPerHour(Ra)
    End Function

    ''' <summary>
    ''' Translates incoming solar radiation from one site to another taking into account slope differences (Source interpolation site assumed nearly horizontal).
    ''' </summary>
    ''' <param name="RsSource">Incoming Solar Radiation at Source Interpolation Site (Same Units as Other Input Radiations)</param>
    ''' <param name="RaSource">Extraterrestrial Solar Radiation at Source Interpolation Site (Same Units as Other Input Radiations)</param>
    ''' <param name="RaDestination">Extraterrestrial Solar Radiation at Source Interpolation Site (Same Units as Other Input Radiations)</param>
    ''' <param name="Slope">Positive Inclination from Horizontal (Decimal Degrees)</param>
    ''' <returns>Incoming Solar Radiation at Destination Translation Site (Same Units as Input Radiations)</returns>
    ''' <remarks>Adapted from: Allen, R. G., Trezza, R., Tasumi, M. (2006). Analytical integrated functions for daily solar radiation on slopes. Agricultural and Forest Meteorology, 139(1), 55-73.</remarks>
    Function TranslateRs(RsSource As Double, RaSource As Double, RaDestination As Double, Slope As Double) As Double
        If RsSource <= 0 Or RaSource <= 0 Then
            Return 0
        Else
            'Convert to Radians
            Slope = ToRadians(Slope)

            'Actual Atmospheric Transmissivity (Unitless)
            Dim τsw As Double = RsSource / RaSource

            'Source Actual Direct Radiation Index (Unitless)
            Dim Kb As Double = 0
            Select Case τsw
                Case Is >= 0.42
                    Kb = 1.56 * τsw - 0.55
                Case Is <= 0.175
                    Kb = 0.016 * τsw
                Case Else
                    Kb = ((0.765 * τsw + 0.828) * τsw + 0.28) * τsw + 0.022
            End Select

            'Source Actual Diffuse Radiation Index (Unitless)
            Dim Kd As Double = τsw - Kb

            'Direct Radiation Fraction (Unitless)
            Dim Fb As Double = RaDestination / RaSource

            'Reflectance Factor (Unitless)
            Dim Fi As Double = 0.75 + 0.25 * Math.Cos(Slope) - 0.5 * Slope / π

            'Sky-view Factor (Unitless)
            Dim Fia As Double = (1 - Kb) * (1 + (Kb / (Kb + Kd)) ^ 0.5 * Math.Sin(Slope / 2) ^ 3) * Fi + Fb * Kb

            'Incoming Solar Radiation at Interpolated Site (Source Site Units)
            Dim RsDestination As Double = RsSource * ((Fb * Kb + Fia * Kd) / τsw + 0.23 * (1 - Fi))

            Return RsDestination
        End If
    End Function

    ''' <summary>
    ''' Calculates the geocentric sun angles and distance at a given instant in time.
    ''' </summary>
    ''' <param name="RecordDate">Event Time</param>
    ''' <returns>Earth-Sun Orientation and Distance</returns>
    ''' <remarks>Adapted from: Reda, I., Andreas, A. (2004). Solar position algorithm for solar radiation applications. Solar energy, 76(5), 577-589.</remarks>
    Function CalculateSolarPosition(RecordDate As DateTime) As SolarPosition
        'Julian Day
        Dim Month = RecordDate.Month
        Dim Year = RecordDate.Year
        If Month < 3 Then
            Month += 12
            Year -= 1
        End If
        Dim JD As Double = Int(365.25 * (Year + 4716)) + Int(30.6001 * (Month + 1)) + RecordDate.Day + RecordDate.TimeOfDay.TotalHours / 24 - 1524.5
        If JD > 2299160 Then
            Dim A = Int(Year / 100)
            JD += (2 - A + Int(A / 4))
        End If

        'Approximate Difference between the Earth Rotation Time and Terrestrial Time (seconds)
        Dim ΔT As Double = LookUpΔT(RecordDate)

        'Julian Day, Ephemeris
        Dim JDE As Double = JD + ΔT / 86400 'seconds to day fraction

        'Julian Century, Ephemeris
        Dim JC As Double = (JD - 2451545) / 36525
        Dim JCE As Double = (JDE - 2451545) / 36525

        'Julian Millennium, Ephemeris
        Dim JME As Double = JCE / 10

        'Earth Heliocentric Longitude (radians)
        Dim L As Double = CalculateEarthHeliocentricProperty(JME, EarthHeliocentricType.Longitude)

        'Earth Heliocentric Latitude (radians)
        Dim B As Double = CalculateEarthHeliocentricProperty(JME, EarthHeliocentricType.Latitude)

        'Earth-Sun Distance (astronomical units, au)
        Dim R As Double = CalculateEarthHeliocentricProperty(JME, EarthHeliocentricType.Radius)

        'Geocentric Longitude (radians)
        Dim Θ As Double = LimitAngle(L + π)

        'Geocentric Latitude (radians)
        Dim β = -B

        'Mean Elongation of the Moon from the Sun (degrees)
        Dim X(4) As Double
        X(0) = CalculateSolarPolynomial(JCE, 1 / 189474, -0.0019142, 445267.11148, 297.85036)

        'Mean Anomaly of the Sun-Earth (degrees)
        X(1) = CalculateSolarPolynomial(JCE, 1 / -300000, -0.0001603, 35999.05034, 357.52772)

        'Mean Anomaly of the Moon (degrees)
        X(2) = CalculateSolarPolynomial(JCE, 1 / 56250, 0.0086972, 477198.867398, 134.96298)

        'Moon's Argument of Latitude  (degrees)
        X(3) = CalculateSolarPolynomial(JCE, 1 / 327270, -0.0036825, 483202.017538, 93.27191)

        'Longitude of the Ascending Node of the Moon’s Mean Orbit on the Ecliptic, Measured from the Mean Equinox of the Date (degrees)
        X(4) = CalculateSolarPolynomial(JCE, 1 / 450000, 0.0020708, -1934.136261, 125.04452)

        'Nutation in Longitude (radians)
        Dim Δψ As Double = CalculateNutation(JCE, X, NutationType.Longitude)

        'Nutation In Obliquity (radians)
        Dim Δε As Double = CalculateNutation(JCE, X, NutationType.Obliquity)

        'Mean Obliquity of the Ecliptic (arc seconds)
        Dim U As Double = JME / 10
        Dim ε0 As Double = 84381.448 + U * (-4680.96 + U * (-1.55 + U * (1999.25 + U * (-51.38 + U * (-249.67 + U * (-39.05 + U * (7.12 + U * (27.87 + U * (5.79 + U * 2.45)))))))))

        'True Obliquity of the Ecliptic (radians)
        Dim ε As Double = ToRadians(ε0 / 3600) + Δε

        'Aberration Correction (radians)
        Dim Δτ As Double = ToRadians(-20.4898 / (3600 * R))

        'Apparent Sun Longitude (radians)
        Dim λ As Double = Θ + Δψ + Δτ

        'Mean Sidereal Time at Greenwich (radians)
        Dim ν0 As Double = LimitAngle(ToRadians(280.46061837 + 360.98564736629 * (JD - 2451545) + JC * JC * (0.000387933 - JC / 38710000)))

        'Trigonometric Values
        Dim Cosε As Double = Math.Cos(ε)
        Dim Sinε As Double = Math.Sin(ε)
        Dim Sinλ As Double = Math.Sin(λ)

        'Apparent Sidereal Time at Greenwich (radians)
        Dim ν As Double = ν0 + Δψ * Cosε

        'Geocentric Sun Right Ascension (radians)
        Dim α As Double = LimitAngle(Math.Atan2(Sinλ * Cosε - Math.Tan(β) * Sinε, Math.Cos(λ)))

        'Geocentric Sun Declination (radians)
        Dim δ As Double = Math.Asin(Math.Sin(β) * Cosε + Math.Cos(β) * Sinε * Sinλ)

        Return New SolarPosition(R, δ, α, ν)
    End Function

    ''' <summary>
    ''' Returns a yearly average difference between universal time and actual terrestrial time (1620-2014).
    ''' </summary>
    ''' <param name="RecordDate">Event Time</param>
    ''' <returns>Time Difference (seconds)</returns>
    ''' <remarks>Source: http://asa.usno.navy.mil/SecK/DeltaT.html</remarks>
    Function LookUpΔT(RecordDate As DateTime) As Double
        Dim ΔT = {124, 119, 115, 110, 106, 102, 98, 95, 91, 88, 85, 82, 79, 77, 74, 72, 70, 67, 65, 63, 62, 60, 58, 57, 55, 54, 53, 51, 50, 49, 48, 47, 46, 45, 44, 43, 42, 41, 40, 38, 37, 36, 35, 34, 33, 32, 31, 30, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 14, 13, 12, 12, 11, 11, 10, 10, 10, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 10, 10, 10, 10, 10, 10, 10, 10, 10, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 13, 13, 13, 13, 13, 13, 13, 14, 14, 14, 14, 14, 14, 14, 15, 15, 15, 15, 15, 15, 15, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 17, 17, 17, 17, 17, 17, 17, 17, 17, 17, 17, 17, 17, 17, 17, 17, 17, 16, 16, 16, 16, 15, 15, 14, 14, 13.7, 13.4, 13.1, 12.9, 12.7, 12.6, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.4, 12.3, 12.2, 12, 11.7, 11.4, 11.1, 10.6, 10.2, 9.6, 9.1, 8.6, 8, 7.5, 7, 6.6, 6.3, 6, 5.8, 5.7, 5.6, 5.6, 5.6, 5.7, 5.8, 5.9, 6.1, 6.2, 6.3, 6.5, 6.6, 6.8, 6.9, 7.1, 7.2, 7.3, 7.4, 7.5, 7.6, 7.7, 7.7, 7.8, 7.8, 7.88, 7.82, 7.54, 6.97, 6.4, 6.02, 5.41, 4.1, 2.92, 1.82, 1.61, 0.1, -1.02, -1.28, -2.69, -3.24, -3.64, -4.54, -4.71, -5.11, -5.4, -5.42, -5.2, -5.46, -5.46, -5.79, -5.63, -5.64, -5.8, -5.66, -5.87, -6.01, -6.19, -6.64, -6.44, -6.47, -6.09, -5.76, -4.66, -3.74, -2.72, -1.54, -0.02, 1.24, 2.64, 3.86, 5.37, 6.14, 7.75, 9.13, 10.46, 11.53, 13.36, 14.65, 16.01, 17.2, 18.24, 19.06, 20.25, 20.95, 21.16, 22.25, 22.41, 23.03, 23.49, 23.62, 23.86, 24.49, 24.34, 24.08, 24.02, 24, 23.87, 23.95, 23.86, 23.93, 23.73, 23.92, 23.96, 24.02, 24.33, 24.83, 25.3, 25.7, 26.24, 26.77, 27.28, 27.78, 28.25, 28.71, 29.15, 29.57, 29.97, 30.36, 30.72, 31.07, 31.35, 31.68, 32.18, 32.68, 33.15, 33.59, 34, 34.47, 35.03, 35.73, 36.54, 37.43, 38.29, 39.2, 40.18, 41.17, 42.23, 43.37, 44.49, 45.48, 46.46, 47.52, 48.53, 49.59, 50.54, 51.38, 52.17, 52.96, 53.79, 54.34, 54.87, 55.32, 55.82, 56.3, 56.86, 57.57, 58.31, 59.12, 59.99, 60.78, 61.63, 62.3, 62.97, 63.47, 63.83, 64.09, 64.3, 64.47, 64.57, 64.69, 64.85, 65.15, 65.46, 65.78, 66.2, 66.45, 66.74, 67.09, 67.43}

        Dim Y = Limit(RecordDate.Year - 1620, 0, ΔT.Length - 1)

        Return ΔT(Y)
    End Function

    ''' <summary>
    ''' Calculates earth heliocentric longitude, latitude, or earth-sun distance.
    ''' </summary>
    ''' <param name="JM">Julian Millenium</param>
    ''' <param name="Type">Earth Heliocentric Property Type</param>
    ''' <returns>Earth Heliocentric Property [Longitude or Latitude (Decimal Degrees), or Earth-Sun Distance (astronomical units)]</returns>
    ''' <remarks>Adapted from: Reda, I., Andreas, A. (2004). Solar position algorithm for solar radiation applications. Solar energy, 76(5), 577-589.</remarks>
    Function CalculateEarthHeliocentricProperty(JM As Double, Type As EarthHeliocentricType) As Double
        Dim ABC()(,) As Double = Nothing
        Select Case Type
            Case EarthHeliocentricType.Longitude
                ABC = New Double()(,) {New Double(,) {{175347046, 0, 0}, {3341656, 4.6692568, 6283.07585}, {34894, 4.6261, 12566.1517}, {3497, 2.7441, 5753.3849}, {3418, 2.8289, 3.5231}, {3136, 3.6277, 77713.7715}, {2676, 4.4181, 7860.4194}, {2343, 6.1352, 3930.2097}, {1324, 0.7425, 11506.7698}, {1273, 2.0371, 529.691}, {1199, 1.1096, 1577.3435}, {990, 5.233, 5884.927}, {902, 2.045, 26.298}, {857, 3.508, 398.149}, {780, 1.179, 5223.694}, {753, 2.533, 5507.553}, {505, 4.583, 18849.228}, {492, 4.205, 775.523}, {357, 2.92, 0.067}, {317, 5.849, 11790.629}, {284, 1.899, 796.298}, {271, 0.315, 10977.079}, {243, 0.345, 5486.778}, {206, 4.806, 2544.314}, {205, 1.869, 5573.143}, {202, 2.458, 6069.777}, {156, 0.833, 213.299}, {132, 3.411, 2942.463}, {126, 1.083, 20.775}, {115, 0.645, 0.98}, {103, 0.636, 4694.003}, {102, 0.976, 15720.839}, {102, 4.267, 7.114}, {99, 6.21, 2146.17}, {98, 0.68, 155.42}, {86, 5.98, 161000.69}, {85, 1.3, 6275.96}, {85, 3.67, 71430.7}, {80, 1.81, 17260.15}, {79, 3.04, 12036.46}, {75, 1.76, 5088.63}, {74, 3.5, 3154.69}, {74, 4.68, 801.82}, {70, 0.83, 9437.76}, {62, 3.98, 8827.39}, {61, 1.82, 7084.9}, {57, 2.78, 6286.6}, {56, 4.39, 14143.5}, {56, 3.47, 6279.55}, {52, 0.19, 12139.55}, {52, 1.33, 1748.02}, {51, 0.28, 5856.48}, {49, 0.49, 1194.45}, {41, 5.37, 8429.24}, {41, 2.4, 19651.05}, {39, 6.17, 10447.39}, {37, 6.04, 10213.29}, {37, 2.57, 1059.38}, {36, 1.71, 2352.87}, {36, 1.78, 6812.77}, {33, 0.59, 17789.85}, {30, 0.44, 83996.85}, {30, 2.74, 1349.87}, {25, 3.16, 4690.48}}, New Double(,) {{628331966747, 0, 0}, {206059, 2.678235, 6283.07585}, {4303, 2.6351, 12566.1517}, {425, 1.59, 3.523}, {119, 5.796, 26.298}, {109, 2.966, 1577.344}, {93, 2.59, 18849.23}, {72, 1.14, 529.69}, {68, 1.87, 398.15}, {67, 4.41, 5507.55}, {59, 2.89, 5223.69}, {56, 2.17, 155.42}, {45, 0.4, 796.3}, {36, 0.47, 775.52}, {29, 2.65, 7.11}, {21, 5.34, 0.98}, {19, 1.85, 5486.78}, {19, 4.97, 213.3}, {17, 2.99, 6275.96}, {16, 0.03, 2544.31}, {16, 1.43, 2146.17}, {15, 1.21, 10977.08}, {12, 2.83, 1748.02}, {12, 3.26, 5088.63}, {12, 5.27, 1194.45}, {12, 2.08, 4694}, {11, 0.77, 553.57}, {10, 1.3, 6286.6}, {10, 4.24, 1349.87}, {9, 2.7, 242.73}, {9, 5.64, 951.72}, {8, 5.3, 2352.87}, {6, 2.65, 9437.76}, {6, 4.67, 4690.48}}, New Double(,) {{52919, 0, 0}, {8720, 1.0721, 6283.0758}, {309, 0.867, 12566.152}, {27, 0.05, 3.52}, {16, 5.19, 26.3}, {16, 3.68, 155.42}, {10, 0.76, 18849.23}, {9, 2.06, 77713.77}, {7, 0.83, 775.52}, {5, 4.66, 1577.34}, {4, 1.03, 7.11}, {4, 3.44, 5573.14}, {3, 5.14, 796.3}, {3, 6.05, 5507.55}, {3, 1.19, 242.73}, {3, 6.12, 529.69}, {3, 0.31, 398.15}, {3, 2.28, 553.57}, {2, 4.38, 5223.69}, {2, 3.75, 0.98}}, New Double(,) {{289, 5.844, 6283.076}, {35, 0, 0}, {17, 5.49, 12566.15}, {3, 5.2, 155.42}, {1, 4.72, 3.52}, {1, 5.3, 18849.23}, {1, 5.97, 242.73}}, New Double(,) {{114, 3.142, 0}, {8, 4.13, 6283.08}, {1, 3.84, 12566.15}}, New Double(,) {{1, 3.14, 0}}}
            Case EarthHeliocentricType.Latitude
                ABC = New Double()(,) {New Double(,) {{280, 3.199, 84334.662}, {102, 5.422, 5507.553}, {80, 3.88, 5223.69}, {44, 3.7, 2352.87}, {32, 4, 1577.34}}, New Double(,) {{9, 3.9, 5507.55}, {6, 1.73, 5223.69}}}
            Case EarthHeliocentricType.Radius
                ABC = New Double()(,) {New Double(,) {{100013989, 0, 0}, {1670700, 3.0984635, 6283.07585}, {13956, 3.05525, 12566.1517}, {3084, 5.1985, 77713.7715}, {1628, 1.1739, 5753.3849}, {1576, 2.8469, 7860.4194}, {925, 5.453, 11506.77}, {542, 4.564, 3930.21}, {472, 3.661, 5884.927}, {346, 0.964, 5507.553}, {329, 5.9, 5223.694}, {307, 0.299, 5573.143}, {243, 4.273, 11790.629}, {212, 5.847, 1577.344}, {186, 5.022, 10977.079}, {175, 3.012, 18849.228}, {110, 5.055, 5486.778}, {98, 0.89, 6069.78}, {86, 5.69, 15720.84}, {86, 1.27, 161000.69}, {65, 0.27, 17260.15}, {63, 0.92, 529.69}, {57, 2.01, 83996.85}, {56, 5.24, 71430.7}, {49, 3.25, 2544.31}, {47, 2.58, 775.52}, {45, 5.54, 9437.76}, {43, 6.01, 6275.96}, {39, 5.36, 4694}, {38, 2.39, 8827.39}, {37, 0.83, 19651.05}, {37, 4.9, 12139.55}, {36, 1.67, 12036.46}, {35, 1.84, 2942.46}, {33, 0.24, 7084.9}, {32, 0.18, 5088.63}, {32, 1.78, 398.15}, {28, 1.21, 6286.6}, {28, 1.9, 6279.55}, {26, 4.59, 10447.39}}, New Double(,) {{103019, 1.10749, 6283.07585}, {1721, 1.0644, 12566.1517}, {702, 3.142, 0}, {32, 1.02, 18849.23}, {31, 2.84, 5507.55}, {25, 1.32, 5223.69}, {18, 1.42, 1577.34}, {10, 5.91, 10977.08}, {9, 1.42, 6275.96}, {9, 0.27, 5486.78}}, New Double(,) {{4359, 5.7846, 6283.0758}, {124, 5.579, 12566.152}, {12, 3.14, 0}, {9, 3.63, 77713.77}, {6, 1.87, 5573.14}, {3, 5.47, 18849.23}}, New Double(,) {{145, 4.273, 6283.076}, {7, 3.92, 12566.15}}, New Double(,) {{4, 2.56, 6283.08}}}
        End Select

        Dim Value(ABC.GetLength(0) - 1) As Double
        For I = 0 To ABC.GetLength(0) - 1
            For J = 0 To ABC(I).GetLength(0) - 1
                Value(I) += ABC(I)(J, 0) * Math.Cos(ABC(I)(J, 1) + ABC(I)(J, 2) * JM)
            Next
        Next

        Dim EarthHeliocentricValue As Double = 0
        Dim JMpower As Double = 1
        For I = 0 To Value.Length - 1
            EarthHeliocentricValue += Value(I) * JMpower
            JMpower *= JM
        Next
        EarthHeliocentricValue = EarthHeliocentricValue / 10 ^ 8
        If Type = EarthHeliocentricType.Longitude Then EarthHeliocentricValue = LimitAngle(EarthHeliocentricValue)

        Return EarthHeliocentricValue
    End Function

    ''' <summary>
    ''' Calculates a third order polynomial for earth solar calculations (A*V^3+B*V^2+C*V+D).
    ''' </summary>
    ''' <param name="Variable">Input Variable Raised to a Power</param>
    ''' <param name="A">3rd Order Constant</param>
    ''' <param name="B">2nd Order Constant</param>
    ''' <param name="C">1st Order Constant</param>
    ''' <param name="D">Constant</param>
    ''' <returns>Calculated Value</returns>
    Function CalculateSolarPolynomial(Variable As Double, A As Double, B As Double, C As Double, D As Double) As Double
        Return LimitAngle(((A * Variable + B) * Variable + C) * Variable + D, 360)
    End Function

    ''' <summary>
    ''' Calculates nutation in longitude or obliquity for an input Julian Century.
    ''' </summary>
    ''' <param name="JC">Julian Century</param>
    ''' <param name="X">Earth-Moon-Sun Properties Array</param>
    ''' <param name="Type">Nutation Type</param>
    ''' <returns>Nutation</returns>
    ''' <remarks>Adapted from: Reda, I., Andreas, A. (2004). Solar position algorithm for solar radiation applications. Solar energy, 76(5), 577-589.</remarks>
    Function CalculateNutation(JC As Double, X() As Double, Type As NutationType) As Double
        Dim Y = {{0, 0, 0, 0, 1}, {-2, 0, 0, 2, 2}, {0, 0, 0, 2, 2}, {0, 0, 0, 0, 2}, {0, 1, 0, 0, 0}, {0, 0, 1, 0, 0}, {-2, 1, 0, 2, 2}, {0, 0, 0, 2, 1}, {0, 0, 1, 2, 2}, {-2, -1, 0, 2, 2}, {-2, 0, 1, 0, 0}, {-2, 0, 0, 2, 1}, {0, 0, -1, 2, 2}, {2, 0, 0, 0, 0}, {0, 0, 1, 0, 1}, {2, 0, -1, 2, 2}, {0, 0, -1, 0, 1}, {0, 0, 1, 2, 1}, {-2, 0, 2, 0, 0}, {0, 0, -2, 2, 1}, {2, 0, 0, 2, 2}, {0, 0, 2, 2, 2}, {0, 0, 2, 0, 0}, {-2, 0, 1, 2, 2}, {0, 0, 0, 2, 0}, {-2, 0, 0, 2, 0}, {0, 0, -1, 2, 1}, {0, 2, 0, 0, 0}, {2, 0, -1, 0, 1}, {-2, 2, 0, 2, 2}, {0, 1, 0, 0, 1}, {-2, 0, 1, 0, 1}, {0, -1, 0, 0, 1}, {0, 0, 2, -2, 0}, {2, 0, -1, 2, 1}, {2, 0, 1, 2, 2}, {0, 1, 0, 2, 2}, {-2, 1, 1, 0, 0}, {0, -1, 0, 2, 2}, {2, 0, 0, 2, 1}, {2, 0, 1, 0, 0}, {-2, 0, 2, 2, 2}, {-2, 0, 1, 2, 1}, {2, 0, -2, 0, 1}, {2, 0, 0, 0, 1}, {0, -1, 1, 0, 0}, {-2, -1, 0, 2, 1}, {-2, 0, 0, 0, 1}, {0, 0, 2, 2, 1}, {-2, 0, 2, 0, 1}, {-2, 1, 0, 2, 1}, {0, 0, 1, -2, 0}, {-1, 0, 1, 0, 0}, {-2, 1, 0, 0, 0}, {1, 0, 0, 0, 0}, {0, 0, 1, 2, 0}, {0, 0, -2, 2, 2}, {-1, -1, 1, 0, 0}, {0, 1, 1, 0, 0}, {0, -1, 1, 2, 2}, {2, -1, -1, 2, 2}, {0, 0, 3, 2, 2}, {2, -1, 0, 2, 2}}
        Dim ABCD = {{-171996, -174.2, 92025, 8.9}, {-13187, -1.6, 5736, -3.1}, {-2274, -0.2, 977, -0.5}, {2062, 0.2, -895, 0.5}, {1426, -3.4, 54, -0.1}, {712, 0.1, -7, 0}, {-517, 1.2, 224, -0.6}, {-386, -0.4, 200, 0}, {-301, 0, 129, -0.1}, {217, -0.5, -95, 0.3}, {-158, 0, 0, 0}, {129, 0.1, -70, 0}, {123, 0, -53, 0}, {63, 0, 0, 0}, {63, 0.1, -33, 0}, {-59, 0, 26, 0}, {-58, -0.1, 32, 0}, {-51, 0, 27, 0}, {48, 0, 0, 0}, {46, 0, -24, 0}, {-38, 0, 16, 0}, {-31, 0, 13, 0}, {29, 0, 0, 0}, {29, 0, -12, 0}, {26, 0, 0, 0}, {-22, 0, 0, 0}, {21, 0, -10, 0}, {17, -0.1, 0, 0}, {16, 0, -8, 0}, {-16, 0.1, 7, 0}, {-15, 0, 9, 0}, {-13, 0, 7, 0}, {-12, 0, 6, 0}, {11, 0, 0, 0}, {-10, 0, 5, 0}, {-8, 0, 3, 0}, {7, 0, -3, 0}, {-7, 0, 0, 0}, {-7, 0, 3, 0}, {-7, 0, 3, 0}, {6, 0, 0, 0}, {6, 0, -3, 0}, {6, 0, -3, 0}, {-6, 0, 3, 0}, {-6, 0, 3, 0}, {5, 0, 0, 0}, {-5, 0, 3, 0}, {-5, 0, 3, 0}, {-5, 0, 3, 0}, {4, 0, 0, 0}, {4, 0, 0, 0}, {4, 0, 0, 0}, {-4, 0, 0, 0}, {-4, 0, 0, 0}, {-4, 0, 0, 0}, {3, 0, 0, 0}, {-3, 0, 0, 0}, {-3, 0, 0, 0}, {-3, 0, 0, 0}, {-3, 0, 0, 0}, {-3, 0, 0, 0}, {-3, 0, 0, 0}, {-3, 0, 0, 0}}

        Select Case Type
            Case NutationType.Longitude
                Dim Δψ As Double = 0
                For I = 0 To Y.GetLength(0) - 1
                    Dim ΣXY As Double = 0
                    For J = 0 To Y.GetLength(1) - 1
                        ΣXY += X(J) * Y(I, J)
                    Next
                    Δψ += (ABCD(I, 0) + ABCD(I, 1) * JC) * Math.Sin(ToRadians(ΣXY))
                Next
                CalculateNutation = Δψ
            Case NutationType.Obliquity
                Dim Δε As Double = 0
                For I = 0 To Y.GetLength(0) - 1
                    Dim ΣXY As Double = 0
                    For J = 0 To Y.GetLength(1) - 1
                        ΣXY += X(J) * Y(I, J)
                    Next
                    Δε += (ABCD(I, 2) + ABCD(I, 3) * JC) * Math.Cos(ToRadians(ΣXY))
                Next
                CalculateNutation = Δε
            Case Else
                CalculateNutation = Double.MinValue
        End Select

        Return ToRadians(CalculateNutation / 36000000)
    End Function

    ''' <summary>
    ''' Redefines an angle between 0 to 2π or a specified upper boundary.
    ''' </summary>
    ''' <param name="Value">Input angle (Radians [or User-Defined])</param>
    ''' <param name="UpperBounds">Default = 2π [or User-Defined]</param>
    ''' <returns>Basic Positive Angle</returns>
    Function LimitAngle(Value As Double, Optional UpperBounds As Double = 2 * π) As Double
        Dim Output As Double = Value Mod UpperBounds

        If Output < 0 Then Output += UpperBounds

        Return Output
    End Function

    ''' <summary>
    ''' Calculates the least squares slope and offset of dataset X to dataset Y.
    ''' </summary>
    ''' <param name="X">1-Dimensional Independent Variable Array</param>
    ''' <param name="Y">1-Dimensional Dependent Variable Array</param>
    ''' <returns>Slope and Offset in Equation Form of 'Y = (Slope) * X + (Offset)'</returns>
    Function CalculateSimpleLinearRegression(X() As Single, Y() As Single) As Single()
        'Calculate X and Y Array Means
        Dim Xmean As Double = 0
        Dim Ymean As Double = 0
        For I = 0 To X.Length - 1
            Xmean += X(I)
            Ymean += Y(I)
        Next
        Xmean /= X.Length
        Ymean /= Y.Length

        'Calculate Covariance of X and Y and Variance of X
        Dim XYcovariance As Double = 0
        Dim Xvariance As Double = 0
        For I = 0 To X.Length - 1
            Dim Xdifference As Double = X(I) - Xmean
            Dim Ydifference As Double = Y(I) - Ymean

            XYcovariance += Xdifference * Ydifference
            Xvariance += Xdifference ^ 2
        Next

        'Find Slope and Offset
        Dim Slope As Single = XYcovariance / Xvariance
        Dim Offset As Single = Ymean - Slope * Xmean

        Return {Slope, Offset}
    End Function

    ''' <summary>
    ''' Calculates monthly effective precipitation according to USDA (1970) methodology.
    ''' </summary>
    ''' <param name="Precipitation">Monthly Precipitation (Inches)</param>
    ''' <param name="Evapotranspiration">Monthly Evapotranspiration (Inches)</param>
    ''' <returns>Estimated Effective Precipitation (Inches)</returns>
    ''' <remarks>Source: Bos et al. (2008). Water Requirements for Irrigation and the Environment. Springer Science.</remarks>
    Function CalculateUSDAEffectivePrecipitation(Precipitation As Single, Evapotranspiration As Single) As Single
        Dim Value = (18.010843538315541 * Precipitation ^ 0.824 - 2.935) * 0.03937007874015748 * 10 ^ (0.0254 * Evapotranspiration)
        If Value > Precipitation Then Value = Precipitation
        If Value > Evapotranspiration Then Value = Evapotranspiration
        Return Value
    End Function

    ''' <summary>
    ''' Container for geocentric sun angles and distance at a given instant in time.
    ''' </summary>
    Class SolarPosition
        ''' <summary>Geocentric Sun Right Ascension (radians)</summary>
        Public α As Double

        ''' <summary>Geocentric Sun Declination (radians)</summary>
        Public δ As Double

        ''' <summary>Earth-Sun Distance (astronomical units)</summary>
        Public R As Double

        ''' <summary>Apparent Sidereal Time at Greenwich (radians)</summary>
        Public ν As Double

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="R">Earth-Sun Distance (astronomical units)</param>
        ''' <param name="δ">Geocentric Sun Declination (radians)</param>
        ''' <param name="α">Geocentric Sun Right Ascension (radians)</param>
        ''' <param name="ν">Apparent Sidereal Time at Greenwich (radians)</param>
        ''' <remarks></remarks>
        Sub New(R, δ, α, ν)
            Me.R = R
            Me.δ = δ
            Me.α = α
            Me.ν = ν
        End Sub

    End Class

    Function CalculateSolarSlopeIntegrationLimit(SunAngle As Double, A As Double, B As Double, C As Double) As Double
        Return -A + B * Math.Cos(SunAngle) + C * Math.Sin(SunAngle)
    End Function

    Function Limit(ByRef Value As Double, ByRef Lower As Double, ByRef Upper As Double) As Double
        If Value < Lower Then
            Return Lower
        ElseIf Value > Upper Then
            Return Upper
        Else
            Return Value
        End If
    End Function

#End Region

#Region "General"

    Function RoundToSignificantDigits(Number As Double, SignificantDigits As Integer, Optional RoundUp As Boolean = True) As Double
        If Number = 0 Then
            Return 0
        Else
            Dim Scale As Double = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(Number))) + 1)
            Dim Power As Double = 10 ^ SignificantDigits
            Dim Value As Double = 0
            If RoundUp Then
                Value = Math.Ceiling(Number / Scale * Power)
            Else
                Value = Math.Floor(Number / Scale * Power)
            End If
            Return Scale * Value / Power
        End If
    End Function

    Public Class ProgressValues
        Public ProgressText As String
        Public ProgressBarMinValue As Integer
        Public ProgressBarMaxValue As Integer
        Public ProgressBarValue As Integer

        Sub New(ProgressText As String, ProgressBarMinValue As Integer, ProgressBarMaxValue As Integer, ProgressBarValue As Integer)
            Me.ProgressText = ProgressText
            Me.ProgressBarMinValue = ProgressBarMinValue
            Me.ProgressBarMaxValue = ProgressBarMaxValue
            Me.ProgressBarValue = ProgressBarValue
        End Sub

    End Class

#End Region

#Region "Conversions"

    ''' <summary>
    ''' Adjusts wind speed from one height to another using a logarithmic profile.
    ''' </summary>
    ''' <param name="Zw">Wind reference height (ft)</param>
    ''' <param name="H">Vegetation height (ft)</param>
    ''' <param name="Zu">Desired wind reference height (ft)</param>
    ''' <returns>Factor to adjust wind speed</returns>
    ''' <remarks></remarks>
    Function CalculateWindSpeedAdjustmentFactor(Zw As Double, Optional H As Double = 0.12 / 0.3048, Optional Zu As Double = 2 / 0.3048)
        Dim D = 0.67 * H 'zero plane displacement height
        Dim Zom = 0.123 * H 'aerodynamic roughness length
        Return Math.Log((Zu - D) / Zom) / Math.Log((Zw - D) / Zom) 'adjustment factor
    End Function

    ''' <summary>
    ''' Calculates relative humidity from specific humidity, pressure, and temperature.
    ''' </summary>
    ''' <param name="SpecificHumidity">unitless</param>
    ''' <param name="Pressure">Pa</param>
    ''' <param name="Temperature">C</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function CalculateRelativeHumidity(SpecificHumidity As Double, Pressure As Double, Temperature As Double)
        Return 81.80628272 * SpecificHumidity * Pressure / Math.Exp(35.34 * Temperature / (2 * Temperature + 487)) / (189 * SpecificHumidity + 311)
    End Function

    Function CalculateSpecificHumidity(E As Double, P As Double) As Double
        Return 0.622 * E / (P - 0.378 * E)
    End Function

    ''' <summary>
    ''' Adjusts pressure for change in elevation.
    ''' </summary>
    ''' <param name="AirPressure">Average Air Pressure at Referenced Site (Any Unit--Multiplier Relative)</param>
    ''' <param name="AirTemperature">Average Air Temperature at Interpolated Site (Fahrenheit)</param>
    ''' <param name="ΔZ">Difference in Elevation of Referenced Site from Interpolated Site (Feet)</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function AdjustAirPressure(AirPressure As Double, AirTemperature As Double, ΔZ As Double)
        'Acceleration of Gravity on Earth (ft/s^2) 
        Dim g As Double = 32.174

        'Gas Constant for Air (ft-lb/(slug-R)
        Dim R As Double = 1716

        Return AirPressure / Math.Exp(g * ΔZ / (R * ToRankine(AirTemperature)))
    End Function

    Function ToFahrenheit(Celsius As Double)
        Return Celsius * 1.8 + 32
    End Function

    Function ToCelsius(Fahrenheit As Double)
        Return (Fahrenheit - 32) / 1.8
    End Function

    Function ToRadians(Degrees As Double)
        Return Degrees * π / 180
    End Function

    Function ToDegrees(Radians As Double)
        Return Radians * 180 / π
    End Function

    Function ToMeters(Feet As Double)
        Return Feet * 0.304800609601219
    End Function

    Function ToFeet(Meters As Double)
        Return Meters / 0.304800609601219
    End Function

    Function ToInches(Millimeters As Double)
        Return Millimeters / 25.4
    End Function

    Function ToMJperM2(Langleys) As Double
        Return 0.04184 * Langleys
    End Function

    Function ToKelvin(Celsius) As Double
        Return 273.16 + Celsius
    End Function

    Function ToRankine(Fahrenheit) As Double
        Return Fahrenheit + 459.67
    End Function

    Function ToLangleysPerHour(WattsPerSquaredMeter As Double)
        Return WattsPerSquaredMeter / 11.63
    End Function

    Function ToMilesPerHour(MetersPerSecond As Double)
        Return MetersPerSecond / 0.44704
    End Function

    Function ToMetersPerSecond(MilesPerHour As Double)
        Return MilesPerHour * 0.44704
    End Function

#End Region

#End Region

#Region "Classes"

    Class CoverProperties

        Public Property Name As String

        Public Property InitiationThresholdType As ThresholdType
        Public Property InitiationThreshold As Single

        Public Property IntermediateThresholdType As ThresholdType
        Public Property IntermediateThreshold As Single

        Public Property TerminationThresholdType As ThresholdType
        Public Property TerminationThreshold As Single

        Public Property SeasonalCurveType As SeasonalCurveType
        Public Property IsCuttingType As Boolean = False

        Public Property CuttingIntermediateThresholdType As ThresholdType
        Public Property CuttingIntermediateThreshold As Single

        Public Property CuttingTerminationThresholdType As ThresholdType
        Public Property CuttingTerminationThreshold As Single

        Public Property SpringFrostTemperature As Single
        Public Property KillingFrostTemperature As Single

        Public Property CurveName As String

        Public InitialCurve(,) As Single
        Public FinalCurve(,) As Single

        Public Property InitiationToIntermediateCurveType As CurveType
        Public Property IntermediateToTerminationCurveType As CurveType

        Public Property EffectivePrecipitationType As EffectivePrecipitationType

        Public Property Variable As String = ""

        Function ToThresholdType(Name As String, CGDDType As String) As ThresholdType
            Select Case Name
                Case "ETr(Hargreaves)" : Return ThresholdType.Hargreaves_Evapotranspiration
                Case "Full Year", "Days", "Days EtoT", "Days CtoC" : Return ThresholdType.Number_of_Days
                Case "CGDD", "CGDD ItoT"
                    If CGDDType.Contains("32") Then : Return ThresholdType.Growing_Degree_Days_Base_32F
                    ElseIf CGDDType.Contains("41") Then : Return ThresholdType.Growing_Degree_Days_Base_41F
                    Else : Return ThresholdType.Growing_Degree_Days_Base_86F_and_50F
                    End If
                Case Else : Return Nothing
            End Select
        End Function

        Function ToCurveType(Name As String) As CurveType
            If Name.Contains("%CGDD") Then : Return Nothing
            ElseIf Name.Contains("%Days") Then : Return CurveType.Percent_Days
            Else : Return CurveType.Number_of_Days
            End If
        End Function

    End Class

#End Region

#Region "Enums"

    Enum ReferenceET
        LongReference
        ShortReference
    End Enum

    Enum NLDAS_2A
        Air_Temperature = 1
        Specific_Humidity = 2
        Air_Pressure = 3
        Windspeed_U = 4
        Windspeed_V = 5
        Precipitation = 10
        Solar_Radiation = 11
    End Enum

    Enum Hourly
        Air_Pressure
        Air_Temperature
        Precipitation
        Relative_Humidity
        Solar_Radiation
        Wind_Vector_U
        Wind_Vector_V
    End Enum

    Enum RasterDataType
        UInt8
        SInt8
        UInt16
        SInt16
        UInt32
        SInt32
        Float32
    End Enum

    Enum EarthHeliocentricType
        Longitude
        Latitude
        Radius
    End Enum

    Enum NutationType
        Longitude
        Obliquity
    End Enum

    Enum StatisticType
        Average
        Sum
    End Enum

    Enum Weather
        MeanAirTemperature
        MaximumAirTemperature
        MinimumAirTemperature
        DewpointTemperature
        SolarRadiation
        Windspeed
        Precipitation
    End Enum

    Public Enum ThresholdType
        Growing_Degree_Days_Base_32F
        Growing_Degree_Days_Base_41F
        Growing_Degree_Days_Base_86F_and_50F
        Number_of_Days
        Hargreaves_Evapotranspiration
    End Enum

    Enum CurveType
        Percent_Days
        Number_of_Days
    End Enum

    Enum SeasonalCurveType
        Full_Season
        Has_Cuttings
    End Enum

    Enum EffectivePrecipitationType
        None
        USDA_1970
        Eighty_Percent
        One_Hundred_Percent
    End Enum

    Enum DatabaseTableName
        Rasters
        Statistics
        Net
        Dates
    End Enum

#End Region

End Module
