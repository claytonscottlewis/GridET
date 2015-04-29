Module GIS

#Region "Operations"

    ''' <summary>
    ''' Collates separate rasters into one (re)projected image and may handle multiple input coordinate reference systems.
    ''' </summary>
    ''' <param name="InputRasterPaths">Locations of Raster Files to Merge</param>
    ''' <param name="ProjectionRasterPath">Location of Projection File Containg Well-Known-Text for Intended Output Coordinate Reference System</param>
    ''' <param name="OutputRasterPath">Output File Location of Merged Raster</param>
    ''' <param name="OutputRasterFormat">Output Raster Format Type</param>
    ''' <param name="OutputCompression">Output Compression Type</param>
    ''' <param name="ResamplingMethod">Algebraic Function to Apply for Resampling</param>
    ''' <param name="NoDataValue">Output Mask or No Data Value for All Bands</param>
    ''' <param name="Scale">Unit Multiplier (Scale*Data+Offset)</param>
    ''' <param name="Offset">Unit Offset (Scale*Data+Offset)</param>
    Sub MergeRasters(InputRasterPaths() As String, ProjectionRasterPath As String, OutputRasterPath As String, Optional OutputRasterFormat As GDALProcess.RasterFormat = GDALProcess.RasterFormat.GTiff, Optional OutputCompression As GDALProcess.Compression = GDALProcess.Compression.DEFLATE, Optional ResamplingMethod As GDALProcess.ResamplingMethod = GDALProcess.ResamplingMethod.Cubic, Optional NoDataValue As String = "-9999", Optional Scale As Double = 1, Optional Offset As Double = 0, Optional ByRef GDALProcess As GDALProcess = Nothing)
        Try
            'Open Each Input Raster and Categorize by Projection
            Dim Projections As New List(Of String)
            Dim FileGroups As New List(Of List(Of String))
            For Each Path In InputRasterPaths
                Using DataSet = GDAL.Gdal.OpenShared(Path, GDAL.Access.GA_ReadOnly)
                    Dim Projection = DataSet.GetProjection
                    If Not Projections.Contains(Projection) Then
                        Projections.Add(Projection)
                        FileGroups.Add(New List(Of String))
                    End If
                    FileGroups(Projections.IndexOf(Projection)).Add(Path)
                End Using
            Next

            'Group Input Rasters into Virtual Dataset(s) by Projection
            GDALProcess = New GDALProcess
            Dim Level1VRTPaths(FileGroups.Count - 1) As String
            For I = 0 To FileGroups.Count - 1
                Level1VRTPaths(I) = OutputRasterPath & ".1." & I & ".vrt"
                GDALProcess.BuildVRT(FileGroups(I).ToArray, Level1VRTPaths(I), {NoDataValue}, {NoDataValue})
            Next

            'Warp Virtual Dataset(s) into Final Coordinate Reference System (Store in Secondary Virtual Dataset)
            Dim Level2VRTPaths(FileGroups.Count - 1) As String
            For I = 0 To FileGroups.Count - 1S
                Level2VRTPaths(I) = OutputRasterPath & ".2." & I & ".vrt"
                GDALProcess.Warp(Level1VRTPaths(I), Level2VRTPaths(I), ProjectionRasterPath, , , , , ResamplingMethod, GDALProcess.RasterFormat.VRT, OutputCompression, {NoDataValue}, {NoDataValue}, True)
            Next

            'Combine Warped Dataset(s) into Tertiary Virtual Dataset and Set Unit Scale if Necessary
            Dim Level3VRTPath As String = OutputRasterPath & ".3.0.vrt"
            GDALProcess.BuildVRT(Level2VRTPaths, Level3VRTPath, {NoDataValue}, {(NoDataValue - Offset) / Scale})
            If Scale <> 1 Or Offset <> 0 Then
                Using DataSet = GDAL.Gdal.Open(Level3VRTPath, GDAL.Access.GA_Update)
                    Using Band = DataSet.GetRasterBand(1)
                        If Scale <> 1 Then Band.SetScale(Scale)
                        If Offset <> 0 Then Band.SetOffset(Offset)
                    End Using
                End Using
            End If

            'Compute Transformations, Resampling, and Translate into Final Dataset
            GDALProcess.Translate({Level3VRTPath}, OutputRasterPath, OutputRasterFormat, OutputCompression, , NoDataValue, True, True)

            'Remove Intermediate Virtual Datasets
            If Not OutputRasterFormat = GDALProcess.RasterFormat.VRT Then
                For I = 0 To FileGroups.Count - 1
                    IO.File.Delete(Level1VRTPaths(I))
                    IO.File.Delete(Level2VRTPaths(I))
                Next
                IO.File.Delete(Level3VRTPath)
            End If
        Catch Exception As Exception
            MsgBox(Exception.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Resamples and reprojects, if necessary, an image to match the extent, mask, and pixel size of another image. 
    ''' </summary>
    ''' <param name="InputRasterPath">Location of Raster File to Snap</param>
    ''' <param name="SnapRasterPath">Location of Snap Raster File</param>
    ''' <param name="OutputRasterPath">Output File Location for Snapped Raster</param>
    ''' <param name="MaskNoData">Set as No Data in Output Raster Regions of No Data in Snap Raster</param>
    ''' <param name="OutputRasterFormat">Output Raster Format Type</param>
    ''' <param name="OutputCompression">Output Compression Type</param>
    ''' <param name="ResamplingMethod">Resampling Method Type</param>
    ''' <param name="InNoDataValue">Input Mask or No Data Value for All Bands</param>
    ''' <param name="OutNoDataValue">Output Mask or No Data Value for All Bands</param>
    Sub SnapToRaster(InputRasterPath As String, SnapRasterPath As String, OutputRasterPath As String, Optional UseSnapRasterExtent As Boolean = True, Optional MaskNoData As Boolean = True, Optional OutputRasterFormat As GDALProcess.RasterFormat = GDALProcess.RasterFormat.GTiff, Optional OutputCompression As GDALProcess.Compression = GDALProcess.Compression.DEFLATE, Optional ResamplingMethod As GDALProcess.ResamplingMethod = GDALProcess.ResamplingMethod.Average, Optional InNoDataValue As String = "-9999", Optional OutNoDataValue As String = "-9999", Optional OutputDataType As GDAL.DataType = GDAL.DataType.GDT_Unknown, Optional ByRef GDALProcess As GDALProcess = Nothing)
        Try
            'Get Raster Properties of Input and Snap Rasters
            Dim InputRaster As New Raster(InputRasterPath)
            Dim SnapRaster As New Raster(SnapRasterPath)

            'Prepare Intermediate Calculation Files
            Dim ProjectionRasterPath As String = IO.Path.GetTempFileName
            IO.File.WriteAllText(ProjectionRasterPath, SnapRaster.Projection)
            Dim IntermediateRasterPath As String = IO.Path.Combine(IO.Path.GetDirectoryName(OutputRasterPath), IO.Path.GetFileNameWithoutExtension(OutputRasterPath) & "-Intermediate" & IO.Path.GetExtension(OutputRasterPath))

            'Warp Input Image to Match Extent and Resolution of Snap Raster
            GDALProcess = New GDALProcess
            Dim Extent = SnapRaster.Extent
            If Not UseSnapRasterExtent Then Extent = GetMaskedExtent(InputRaster.Extent, SnapRaster)

            GDALProcess.Warp(InputRasterPath, IntermediateRasterPath, ProjectionRasterPath, , Extent, SnapRaster.XResolution, SnapRaster.YResolution, ResamplingMethod, OutputRasterFormat, OutputCompression, {InNoDataValue}, {OutNoDataValue}, True, OutputDataType)

            'Mask Snap Raster No Data Regions in Output if Option Selected
            If MaskNoData And UseSnapRasterExtent Then
                Dim SnapNoDataValue As Byte = SnapRaster.BandNoDataValue(0)
                SnapRaster.Open(GDAL.Access.GA_ReadOnly)

                'Get Intermediate Output Raster Properties
                Dim IntermediateRaster As New Raster(IntermediateRasterPath)
                IntermediateRaster.Open(GDAL.Access.GA_Update)

                'Load Pixel Blocks
                Do Until SnapRaster.BlocksProcessed
                    Dim SnapValues = SnapRaster.Read({1})

                    For B = 1 To IntermediateRaster.BandCount
                        Dim IntermediateValues = IntermediateRaster.Read({B})
                        Dim IntermediateNoDataValue = IntermediateRaster.BandNoDataValue(B - 1)

                        'Mask Output Values where Snap Raster has No Data
                        For I = 0 To SnapValues.Length - 1
                            If SnapValues(I) = SnapNoDataValue Then IntermediateValues(I) = IntermediateNoDataValue
                        Next

                        IntermediateRaster.Write({B}, IntermediateValues)
                    Next

                    SnapRaster.AdvanceBlock()
                    IntermediateRaster.AdvanceBlock()
                Loop
                SnapRaster.Close()
                IntermediateRaster.Close()
            End If

            'Compress Output
            GDALProcess.Translate({IntermediateRasterPath}, OutputRasterPath, OutputRasterFormat, OutputCompression, , OutNoDataValue)

            'Delete Temporary Files
            GDALProcess.DeleteRaster(IntermediateRasterPath)
            IO.File.Delete(ProjectionRasterPath)
        Catch Exception As Exception
            MsgBox(Exception.Message)
        End Try
    End Sub

    ''' <summary>
    ''' For a given input raster calculates latitude and longitude degree coordinates and stores nonmasked values in output rasters.
    ''' </summary>
    ''' <param name="BaseRasterPath">Raster File Location from which to Calculate Latitude and Longitude, Must Be Georeferenced</param>
    ''' <param name="LatitudeRasterPath">Output File Location for Calculated Latitude Raster</param>
    ''' <param name="LongitudeRasterPath">Output File Location for Calculated Longitude Raster</param>
    ''' <param name="OutputRasterFormat">Output Raster Format Type</param>
    ''' <param name="CreationOptions">Output Raster Structure Settings</param>
    Sub CreateCoordinateRasters(BaseRasterPath As String, LatitudeRasterPath As String, LongitudeRasterPath As String, Optional OutputRasterFormat As GDALProcess.RasterFormat = GDALProcess.RasterFormat.GTiff, Optional CreationOptions() As String = Nothing)
        Try
            'Get Base Raster Properties
            Dim BaseRaster As New Raster(BaseRasterPath)
            Dim BaseNoDataValue As Single = BaseRaster.BandNoDataValue(0)

            'Setup Transformation from Projected to Geographic Coordinate System Units (Latitude and Longitude Degrees)
            Dim SpatialReferenceSystem = New OSR.SpatialReference(BaseRaster.Projection)
            Dim GeographicCoordinateSystem = SpatialReferenceSystem.CloneGeogCS
            Dim CoordinateTransformation = New OSR.CoordinateTransformation(SpatialReferenceSystem, GeographicCoordinateSystem)

            'Create Output Datasets
            Dim OutputNoDataValue As Single = -9999
            Dim LongitudeRaster = CreateNewRaster(LongitudeRasterPath, BaseRaster.XCount, BaseRaster.YCount, BaseRaster.Projection, BaseRaster.GeoTransform, {OutputNoDataValue}, , CreationOptions, , OutputRasterFormat)
            Dim LatitudeRaster = CreateNewRaster(LatitudeRasterPath, BaseRaster.XCount, BaseRaster.YCount, BaseRaster.Projection, BaseRaster.GeoTransform, {OutputNoDataValue}, , CreationOptions, , OutputRasterFormat)

            'Open Rasters and Prepare Pixel Block Arrays
            BaseRaster.Open(GDAL.Access.GA_ReadOnly)
            LongitudeRaster.Open(GDAL.Access.GA_Update)
            LatitudeRaster.Open(GDAL.Access.GA_Update)

            Do Until BaseRaster.BlocksProcessed
                Dim BaseValues = BaseRaster.Read({1})
                Dim LongitudeValues = LongitudeRaster.GetValuesArray({1})
                Dim LatitudeValues = LatitudeRaster.GetValuesArray({1})

                'Calculate Affine and Coordinate Transformations to Ouput Latitude and Longitude Degrees
                Dim I As Integer = 0
                For Y = 0 To BaseRaster.BlockYSize - 1
                    For X = 0 To BaseRaster.XCount - 1
                        If BaseValues(I) = BaseNoDataValue Then
                            LongitudeValues(I) = OutputNoDataValue
                            LatitudeValues(I) = OutputNoDataValue
                        Else
                            Dim ProjectedCoordinate = BaseRaster.PixelLocationToCoordinate(New Point64(X, Y + BaseRaster.BlockYOffset))

                            Dim GeographicCoordinate(2) As Double
                            CoordinateTransformation.TransformPoint(GeographicCoordinate, ProjectedCoordinate.X, ProjectedCoordinate.Y, 0)

                            LongitudeValues(I) = GeographicCoordinate(0)
                            LatitudeValues(I) = GeographicCoordinate(1)
                        End If
                        I += 1
                    Next
                Next

                'Write Calculated Values to Output
                LongitudeRaster.Write({1}, LongitudeValues)
                LatitudeRaster.Write({1}, LatitudeValues)

                BaseRaster.AdvanceBlock()
                LongitudeRaster.AdvanceBlock()
                LatitudeRaster.AdvanceBlock()
            Loop

            'Release Memory
            BaseRaster.Close()
            LongitudeRaster.Close()
            LatitudeRaster.Close()
            SpatialReferenceSystem.Dispose()
            GeographicCoordinateSystem.Dispose()
            CoordinateTransformation.Dispose()
        Catch Exception As Exception
            MsgBox(Exception.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Exports the cell center for each pixel not defined as no data in a specified input raster into a point vector file.
    ''' </summary>
    ''' <param name="InputRasterPath">Raster File Location to Convert Pixels to Points</param>
    ''' <param name="OutputVectorPath">Output Point File Location to Store Pixel Center Coordinates</param>
    ''' <param name="VectorFormat">Output Vector Format Type</param>
    Sub RasterPixelsToPoints(InputRasterPath As String, OutputVectorPath As String, Optional VectorFormat As GDALProcess.VectorFormat = GDALProcess.VectorFormat.ESRI_Shapefile)
        Try
            ' Get Input Raster Properties
            Dim InputRaster As New Raster(InputRasterPath)
            Dim InputNoDataValue = InputRaster.BandNoDataValue(0)

            'Create Output Vector Dataset (Delete If Already Exists - Coded Specifically for Shapefiles)
            Using Driver = OGR.Ogr.GetDriverByName(GetEnumName(VectorFormat))
                If IO.File.Exists(OutputVectorPath) Then Driver.DeleteDataSource(OutputVectorPath)
                Using DataSource = Driver.CreateDataSource(IO.Path.GetDirectoryName(OutputVectorPath), {})
                    Using Layer = DataSource.CreateLayer(IO.Path.GetFileNameWithoutExtension(OutputVectorPath), New OSR.SpatialReference(InputRaster.Projection), OGR.wkbGeometryType.wkbPoint, {})

                        'Create Default Geometries
                        Dim Feature As New OGR.Feature(Layer.GetLayerDefn)
                        Dim Geometery As New OGR.Geometry(OGR.wkbGeometryType.wkbPoint)

                        'Open Input Raster and Load Pixel Blocks
                        InputRaster.Open(GDAL.Access.GA_ReadOnly)
                        Do Until InputRaster.BlocksProcessed
                            Dim InputValues = InputRaster.Read({1})

                            'For Each Unmasked Pixel in Input Raster, Calculate the Cell Center Coordinates and Add Point to Vector File
                            Dim I As Integer = 0
                            For Y = 0 To InputRaster.BlockYSize - 1
                                For X = 0 To InputRaster.XCount - 1
                                    If Not InputValues(I) = InputNoDataValue Then
                                        Dim Coordinate = InputRaster.PixelLocationToCoordinate(New Point64(X, Y + InputRaster.BlockYOffset))
                                        Geometery.SetPoint(0, Coordinate.X, Coordinate.Y, 0)
                                        Feature.SetGeometry(Geometery)
                                        Feature.SetFID(I)
                                        Layer.CreateFeature(Feature)
                                    End If
                                    I += 1
                                Next
                            Next

                            InputRaster.AdvanceBlock()
                        Loop

                        'Release Memory and Close Vector Dataset
                        InputRaster.Close()
                        Feature.Dispose()
                        Geometery.Dispose()
                    End Using
                End Using
            End Using
        Catch Exception As Exception
            MsgBox(Exception.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Scales individual bands of an input raster dataset between the minimum and maximum values to a 16 bit resolution (No data value = 0).
    ''' </summary>
    ''' <param name="InputRasterPath">File Location of Raster to Scale</param>
    ''' <param name="OutputRasterPath">Output File Location for Scaled Raster</param>
    ''' <param name="OutputRasterFormat">Output Raster Format Type</param>
    ''' <param name="CreationOptions">Output Raster Structure Settings</param>
    Sub ScaleRaster(InputRasterPath As String, OutputRasterPath As String, Optional OutputRasterFormat As GDALProcess.RasterFormat = GDALProcess.RasterFormat.GTiff, Optional CreationOptions() As String = Nothing)
        Try
            'Get Input Raster Properties
            Dim InputRaster As New Raster(InputRasterPath)
            InputRaster.Open(GDAL.Access.GA_ReadOnly)

            'Retrieve Minimum and Maximum Values for Each Band and Calculate the Multiplier and Offset Values
            Dim Min(InputRaster.BandCount - 1) As Double
            Dim Max(InputRaster.BandCount - 1) As Double
            Dim Scale(InputRaster.BandCount - 1) As Double
            Dim Offset(InputRaster.BandCount - 1) As Double
            For B = 0 To InputRaster.BandCount - 1
                Dim MinMax(1) As Double
                Using Band = InputRaster.Dataset.GetRasterBand(B + 1)
                    Band.ComputeRasterMinMax(MinMax, 0)
                End Using
                Min(B) = MinMax(0)
                Max(B) = MinMax(1)
                If Max(B) = Min(B) Then Max(B) = Min(B) + 1

                Scale(B) = (Max(B) - Min(B)) / (UInt16.MaxValue - 1)
                Offset(B) = Min(B) - Scale(B)
            Next

            'Create Output Dataset
            Using Driver = GDAL.Gdal.GetDriverByName(OutputRasterFormat.ToString)
                Using Output = Driver.Create(OutputRasterPath, InputRaster.XCount, InputRaster.YCount, InputRaster.BandCount, GDAL.DataType.GDT_UInt16, CreationOptions)
                    Output.SetProjection(InputRaster.Projection)
                    Output.SetGeoTransform(InputRaster.GeoTransform)
                    For B = 0 To Output.RasterCount - 1
                        Using Band = Output.GetRasterBand(B + 1)
                            Band.SetNoDataValue(0)
                            Band.SetScale(Scale(B))
                            Band.SetOffset(Offset(B))
                        End Using
                    Next
                End Using
            End Using
            Dim OutputRaster As New Raster(OutputRasterPath)
            OutputRaster.Open(GDAL.Access.GA_Update)

            'Load Pixel Blocks from Input Raster for Each Band
            Do Until InputRaster.BlocksProcessed
                For B = 1 To InputRaster.BandCount
                    Dim InputValues = InputRaster.Read({B})
                    Dim OutputValues = OutputRaster.GetValuesArray({B})
                    Dim NoDataValue = InputRaster.BandNoDataValue(B - 1)
                    Dim ScaleValue = Scale(B - 1)
                    Dim OffsetValue = Offset(B - 1)

                    'Set No Data to 0 or Calculate Scaled Value
                    For I = 0 To InputValues.Length - 1
                        If InputValues(I) = NoDataValue Then
                            OutputValues(I) = 0
                        Else
                            Dim ff = (InputValues(I) - OffsetValue)
                            OutputValues(I) = Convert.ToUInt16((InputValues(I) - OffsetValue) / ScaleValue)
                        End If
                    Next

                    OutputRaster.Write({B}, OutputValues)
                Next

                InputRaster.AdvanceBlock()
                OutputRaster.AdvanceBlock()
            Loop

            'Release Memory
            InputRaster.Close()
            OutputRaster.Close()
        Catch Exception As Exception
            MsgBox(Exception.Message)
        End Try
    End Sub

#End Region

#Region "Helper Functions"

    Function BilinearInterpolation(QuadValues As QuadValues, FractionX As Single, FractionY As Single) As Single
        Dim Value As Single

        Dim Scenario As String = "1" & CByte(Single.IsNaN(QuadValues.TopLeft)) & CByte(Single.IsNaN(QuadValues.TopRight)) & CByte(Single.IsNaN(QuadValues.BottomLeft)) & CByte(Single.IsNaN(QuadValues.BottomRight))
        Select Case CInt(Scenario)
            Case 10000
                Value = ((QuadValues.TopLeft - QuadValues.TopRight - QuadValues.BottomLeft + QuadValues.BottomRight) * FractionY - QuadValues.TopLeft + QuadValues.TopRight) * FractionX + (QuadValues.BottomLeft - QuadValues.TopLeft) * FractionY + QuadValues.TopLeft
            Case 11000
                Value = (((QuadValues.TopRight + QuadValues.BottomLeft - QuadValues.BottomRight) * FractionY - QuadValues.TopRight) * FractionX - QuadValues.BottomLeft * FractionY) / ((FractionY - 1) * FractionX - FractionY)
            Case 10100
                Value = (((QuadValues.TopLeft - QuadValues.BottomLeft + QuadValues.BottomRight) * FractionY - QuadValues.TopLeft) * FractionX + (QuadValues.BottomLeft - QuadValues.TopLeft) * FractionY + QuadValues.TopLeft) / (1 + (FractionY - 1) * FractionX)
            Case 10010
                Value = (((QuadValues.TopLeft - QuadValues.TopRight + QuadValues.BottomRight) * FractionY - QuadValues.TopLeft + QuadValues.TopRight) * FractionX - QuadValues.TopLeft * (FractionY - 1)) / (FractionX * FractionY + 1 - FractionY)
            Case 10001
                Value = (((-QuadValues.TopLeft + QuadValues.TopRight + QuadValues.BottomLeft) * FractionY + QuadValues.TopLeft - QuadValues.TopRight) * FractionX + (QuadValues.TopLeft - QuadValues.BottomLeft) * FractionY - QuadValues.TopLeft) / (FractionX * FractionY - 1)
            Case 11010
                Value = QuadValues.TopRight + (QuadValues.BottomRight - QuadValues.TopRight) * FractionY
            Case 10101
                Value = QuadValues.TopLeft + (QuadValues.BottomLeft - QuadValues.TopLeft) * FractionY
            Case 10011
                Value = QuadValues.TopLeft + (QuadValues.TopRight - QuadValues.TopLeft) * FractionX
            Case 11100
                Value = QuadValues.BottomLeft + (QuadValues.BottomRight - QuadValues.BottomLeft) * FractionX
            Case 10111
                Value = QuadValues.TopLeft
            Case 11011
                Value = QuadValues.TopRight
            Case 11101
                Value = QuadValues.BottomLeft
            Case 11110
                Value = QuadValues.BottomRight
            Case Else
                Value = Single.NaN
        End Select

        Return Value
    End Function

    Function GetEnumName(Enumeration As Object) As String
        Return [Enum].GetName(Enumeration.GetType, Enumeration).Replace("_", " ")
    End Function

    Function GetMaskedExtent(MaskExtent As Extent, InputRaster As Raster) As Extent
        Dim Output As New Extent

        Output.Xmin = InputRaster.Extent.Xmin + InputRaster.XResolution * Math.Floor((MaskExtent.Xmin - InputRaster.Extent.Xmin) / InputRaster.XResolution)
        Output.Xmax = InputRaster.Extent.Xmax + InputRaster.XResolution * Math.Ceiling((MaskExtent.Xmax - InputRaster.Extent.Xmax) / InputRaster.XResolution)
        Output.Ymin = InputRaster.Extent.Ymin + InputRaster.YResolution * Math.Floor((MaskExtent.Ymin - InputRaster.Extent.Ymin) / InputRaster.YResolution)
        Output.Ymax = InputRaster.Extent.Ymax + InputRaster.YResolution * Math.Ceiling((MaskExtent.Ymax - InputRaster.Extent.Ymax) / InputRaster.YResolution)

        If Output.Xmin < InputRaster.Extent.Xmin Then Output.Xmin = InputRaster.Extent.Xmin
        If Output.Xmax > InputRaster.Extent.Xmax Then Output.Xmax = InputRaster.Extent.Xmax
        If Output.Ymin < InputRaster.Extent.Ymin Then Output.Ymin = InputRaster.Extent.Ymin
        If Output.Ymax > InputRaster.Extent.Ymax Then Output.Ymax = InputRaster.Extent.Ymax

        Return Output
    End Function

    Function CreateNewRaster(Path As String, XCount As Integer, YCount As Integer, Projection As String, GeoTransform() As Double, NoDataValue As Object(), Optional DataType As GDAL.DataType = GDAL.DataType.GDT_Float32, Optional Options() As String = Nothing, Optional BandCount As Integer = 1, Optional RasterFormat As GDALProcess.RasterFormat = GDALProcess.RasterFormat.GTiff) As Raster
         Using Driver = GDAL.Gdal.GetDriverByName(RasterFormat.ToString)
            Using Dataset = Driver.Create(Path, XCount, YCount, BandCount, DataType, Options)
                Dataset.SetProjection(Projection)
                Dataset.SetGeoTransform(GeoTransform)

                For B = 1 To BandCount
                    Using Band = Dataset.GetRasterBand(B)
                        Dim f = NoDataValue(Limit(B - 1, 0, NoDataValue.Length - 1))
                        Band.SetNoDataValue(NoDataValue(Limit(B - 1, 0, NoDataValue.Length - 1)))
                    End Using
                Next
            End Using
        End Using

        Return New Raster(Path)
    End Function

#End Region

#Region "Helper Classes"

    Class Raster

#Region "Dispose"

        Implements IDisposable

        Dim Disposed As Boolean = False
        Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

        Protected Overridable Sub Dispose(Disposing As Boolean)
            If Disposed Then Return

            If Disposing Then
                Close()
                GeoTransform = Nothing
                InverseGeoTransform = Nothing
                BandNoDataValue = Nothing
                BandValueMultiplier = Nothing
                BandValueOffset = Nothing
                FetchNoDataValue = Nothing
            End If

            Disposed = True
        End Sub

#End Region

#Region "Variables"

        Public Property Path As String
        Public Property GeoTransform As Double()
        Public Property InverseGeoTransform As Double()
        Public Property Projection As String
        Public Property Extent As Extent
        Public Property XResolution As Double
        Public Property YResolution As Double
        Public Property XResolutionHalf As Double
        Public Property YResolutionHalf As Double
        Public Property XCount As Integer
        Public Property YCount As Integer
        Public Property BandCount As Integer
        Public Property BandNoDataValue As Double()
        Public ReadOnly Property BlocksProcessed As Boolean
            Get
                Return Processed
            End Get
        End Property
        Public ReadOnly Property BlockYOffset As Integer
            Get
                Return FetchYOffset
            End Get
        End Property
        Public ReadOnly Property BlockYSize As Integer
            Get
                Return FetchYSize
            End Get
        End Property

        Public Property Dataset As GDAL.Dataset
        Private Property BandType As GDAL.DataType()
        Private Property BandValueMultiplier As Double()
        Private Property BandValueOffset As Double()
        Private Property FetchNoDataValue As Double()
        Private Property BlockLength As Integer = 2 ^ 20 '1 MB
        Private Property BlockYCount As Integer
        Private Property BlockCount As Integer
        Private Property FetchBlock As Integer
        Private Property FetchYOffset As Integer
        Private Property FetchYSize As Integer
        Private Property FetchLength As Integer
        Private Property Processed As Boolean
        Private Property AccessType As GDAL.Access = GDAL.Access.GA_ReadOnly

#End Region

#Region "Functions"

        Sub New()
            Extent = New Extent
        End Sub

        Sub New(RasterPath As String)
            Try
                Using Dataset = GDAL.Gdal.OpenShared(RasterPath, GDAL.Access.GA_ReadOnly)
                    Projection = Dataset.GetProjection()

                    ReDim GeoTransform(5)
                    Dataset.GetGeoTransform(GeoTransform)
                    ReDim InverseGeoTransform(5)
                    GDAL.Gdal.InvGeoTransform(GeoTransform, InverseGeoTransform)

                    XCount = Dataset.RasterXSize
                    YCount = Dataset.RasterYSize

                    Dim Xmin = GeoTransform(0) + GeoTransform(2) * YCount
                    Dim Ymin = GeoTransform(3) + GeoTransform(5) * YCount
                    Dim Xmax = GeoTransform(0) + GeoTransform(1) * XCount
                    Dim Ymax = GeoTransform(3) + GeoTransform(4) * XCount
                    Extent = New Extent(Xmin, Xmax, Ymin, Ymax)

                    XResolution = (Xmax - Xmin) / XCount
                    YResolution = (Ymax - Ymin) / YCount
                    XResolutionHalf = XResolution / 2
                    YResolutionHalf = YResolution / 2

                    BandCount = Dataset.RasterCount
                    ReDim BandType(BandCount - 1)
                    ReDim BandNoDataValue(BandCount - 1)
                    ReDim FetchNoDataValue(BandCount - 1)
                    ReDim BandValueMultiplier(BandCount - 1)
                    ReDim BandValueOffset(BandCount - 1)
                    For I = 1 To BandCount
                        Using Band = Dataset.GetRasterBand(I)
                            BandType(I - 1) = Band.DataType
                            Band.GetNoDataValue(FetchNoDataValue(I - 1), True)
                            BandNoDataValue(I - 1) = FetchNoDataValue(I - 1)
                            Band.GetScale(BandValueMultiplier(I - 1), True)
                            Band.GetOffset(BandValueOffset(I - 1), True)
                        End Using
                    Next

                    Path = RasterPath
                End Using
            Catch Exception As Exception
                MsgBox(Exception)
            End Try
        End Sub

        Function CoordinateToPixelLocation(ByRef Point As Point64) As Point64
            Dim Out As New Point64
            Out.X = InverseGeoTransform(0) + InverseGeoTransform(1) * Point.X + InverseGeoTransform(2) * Point.Y - 0.5
            Out.Y = InverseGeoTransform(3) + InverseGeoTransform(4) * Point.X + InverseGeoTransform(5) * Point.Y - 0.5
            Return Out
        End Function

        Function PixelLocationToCoordinate(ByVal Point As Point64) As Point64
            Point.X += 0.5
            Point.Y += 0.5

            Dim Out As New Point64
            Out.X = GeoTransform(0) + GeoTransform(1) * Point.X + GeoTransform(2) * Point.Y
            Out.Y = GeoTransform(3) + GeoTransform(4) * Point.X + GeoTransform(5) * Point.Y
            Return Out
        End Function

        Function GetPixelQuadFromCoordinate(Values()() As Single, Coordinate As Point64) As PixelQuad
            Dim PixelQuad As New PixelQuad(Values.Length)

            Dim PixelLocation = CoordinateToPixelLocation(Coordinate)

            Dim XLeft As Integer = Int(PixelLocation.X)
            PixelQuad.FractionX = PixelLocation.X - XLeft
            Dim YTop As Integer = Int(PixelLocation.Y)
            PixelQuad.FractionY = PixelLocation.Y - YTop

            Dim ITopLeft As Integer = XCount * YTop + XLeft
            Dim ITopRight As Integer = ITopLeft + 1
            Dim IBottomLeft As Integer = ITopLeft + XCount
            Dim IBottomRight As Integer = IBottomLeft + 1

            Dim Scenario As Integer = 0
            If Coordinate.X - Extent.Xmin < XResolutionHalf Or Extent.Xmax - Coordinate.X <= XResolutionHalf Then
                If Coordinate.Y - Extent.Ymin < YResolutionHalf Then
                    Scenario = 1
                ElseIf Extent.Ymax - Coordinate.Y <= YResolutionHalf Then
                    Scenario = 2
                Else
                    Scenario = 3
                End If
            ElseIf Coordinate.Y - Extent.Ymin < YResolutionHalf Or Extent.Ymax - Coordinate.Y <= YResolutionHalf Then
                Scenario = 4
            End If

            For V = 0 To Values.Length - 1
                Select Case Scenario
                    Case 0
                        PixelQuad.Band(V).TopLeft = Values(V)(ITopLeft)
                        PixelQuad.Band(V).BottomLeft = Values(V)(IBottomLeft)
                        PixelQuad.Band(V).TopRight = Values(V)(ITopRight)
                        PixelQuad.Band(V).BottomRight = Values(V)(IBottomRight)
                    Case 1
                        PixelQuad.Band(V).BottomLeft = Values(V)(IBottomLeft)
                    Case 2
                        PixelQuad.Band(V).TopLeft = Values(V)(ITopLeft)
                    Case 3
                        PixelQuad.Band(V).TopLeft = Values(V)(ITopLeft)
                        PixelQuad.Band(V).BottomLeft = Values(V)(IBottomLeft)
                    Case 4
                        PixelQuad.Band(V).TopLeft = Values(V)(ITopLeft)
                        PixelQuad.Band(V).TopRight = Values(V)(ITopRight)
                End Select
            Next

            Return PixelQuad
        End Function

        Sub Open(Access As GDAL.Access)
            BlockLength = Math.Max(BlockLength, XCount)
            BlockYCount = Math.Min(Int(BlockLength / XCount), YCount)
            BlockLength = XCount * BlockYCount
            BlockCount = Math.Ceiling(YCount / BlockYCount) - 1
            FetchBlock = 0
            Processed = False
            If Dataset Is Nothing Then Dataset = GDAL.Gdal.OpenShared(Path, Access)
            AccessType = Access
        End Sub

        Sub Close()
            If Not Dataset Is Nothing Then Dataset.Dispose()
        End Sub

        Function Read(Band() As Integer) As Array
            StartFetch(Band)

            Dim Values = GetValues(Band)
            Dataset.ReadRaster(0, FetchYOffset, XCount, FetchYSize, Values, XCount, FetchYSize, Band.Length, Band, 0, 0, 0)

            For B = 0 To Band.Length - 1
                Dim I = Band(B) - 1
                If BandValueMultiplier(I) <> 1 Or BandValueOffset(I) <> 0 Then
                    Dim BandLength = XCount * FetchYSize
                    Dim FetchOffset = B * BandLength

                    If TypeOf Values(0) Is Single Then
                        BandNoDataValue(I) = Single.MinValue
                    Else
                        BandNoDataValue(I) = Double.MinValue
                    End If

                    For J = FetchOffset To FetchOffset + BandLength - 1
                        If Values(J) = FetchNoDataValue(I) Then
                            Values(J) = BandNoDataValue(I)
                        Else
                            Values(J) = Values(J) * BandValueMultiplier(I) + BandValueOffset(I)
                        End If
                    Next
                End If
            Next

            Read = Values

            EndFetch()
        End Function

        Sub Write(Band() As Integer, Values As Object)
            StartFetch(Band)

            Dataset.WriteRaster(0, FetchYOffset, XCount, FetchYSize, Values, XCount, FetchYSize, Band.Length, Band, 0, 0, 0)

            EndFetch()
        End Sub

        Function GetValuesArray(Band() As Integer) As Array
            StartFetch(Band)
            Return GetValues(Band)
        End Function

        Private Function GetValues(Band() As Integer) As Object
            Dim Values
            Select Case BandType(Band(0) - 1)
                Case GDAL.DataType.GDT_Byte : Values = Array.CreateInstance(GetType(Byte), FetchLength)
                Case GDAL.DataType.GDT_Int16 : Values = Array.CreateInstance(GetType(Short), FetchLength)
                Case GDAL.DataType.GDT_UInt16 : Values = Array.CreateInstance(GetType(Single), FetchLength)
                Case GDAL.DataType.GDT_Int32 : Values = Array.CreateInstance(GetType(Integer), FetchLength)
                Case GDAL.DataType.GDT_UInt32 : Values = Array.CreateInstance(GetType(Double), FetchLength)
                Case GDAL.DataType.GDT_Float32 : Values = Array.CreateInstance(GetType(Single), FetchLength)
                Case GDAL.DataType.GDT_Float64 : Values = Array.CreateInstance(GetType(Double), FetchLength)
                Case Else : Values = Array.CreateInstance(GetType(Double), FetchLength)
            End Select

            Return Values
        End Function

        Private Sub StartFetch(Band() As Integer)
            FetchYOffset = FetchBlock * BlockYCount
            FetchYSize = Math.Min(BlockYCount, YCount - FetchYOffset)
            FetchLength = Band.Length * XCount * FetchYSize

            If Dataset.getCPtr(Dataset).Wrapper Is Nothing Then Dataset = GDAL.Gdal.OpenShared(Path, AccessType)
        End Sub

        Private Sub EndFetch()
            If FetchBlock = BlockCount Then Processed = True
        End Sub

        Sub AdvanceBlock()
            If Processed = False Then FetchBlock += 1
        End Sub

        Sub SetBlockLength(BlockLength As Integer)
            Me.BlockLength = BlockLength
        End Sub

        Sub AddStatistics()
            If Dataset Is Nothing Then Dataset = GDAL.Gdal.OpenShared(Path, GDAL.Access.GA_Update)

            For B = 1 To BandCount
                Using Band = Dataset.GetRasterBand(B)
                    Dim Min As Double = 0, Max As Double = 0, Mean As Double = 0, StDev As Double = 0, Buckets As Integer = 256, Histogram(Buckets - 1) As Integer

                    Band.ComputeStatistics(False, Min, Max, Mean, StDev, Nothing, Nothing)
                    Band.SetStatistics(Min, Max, Mean, StDev)

                    Band.GetHistogram(Min, Max, Buckets, Histogram, True, False, Nothing, Nothing)
                    Band.SetDefaultHistogram(Min, Max, Buckets, Histogram)
                End Using
            Next
        End Sub

#End Region

    End Class

    Class Point64
        Public X As Double
        Public Y As Double

        Sub New()
        End Sub

        Sub New(X As Double, Y As Double)
            Me.X = X
            Me.Y = Y
        End Sub

        Public Overrides Function ToString() As String
            Return X & "," & Y
        End Function

    End Class

    Class Extent
        Public Xmin As Double
        Public Xmax As Double
        Public Ymin As Double
        Public Ymax As Double

        Sub New()

        End Sub

        Sub New(Xmin As Double, Xmax As Double, Ymin As Double, Ymax As Double)
            Me.Xmin = Xmin
            Me.Xmax = Xmax
            Me.Ymin = Ymin
            Me.Ymax = Ymax
        End Sub

    End Class

    Class PixelQuad
        Public Band() As QuadValues
        Public FractionX As Double
        Public FractionY As Double

        Sub New(Count As Integer)
            ReDim Band(Count - 1)
            For I = 0 To Count - 1
                Band(I) = New QuadValues
            Next
        End Sub

    End Class

    Class QuadValues
        Public TopLeft As Single = Single.NaN
        Public BottomLeft As Single = Single.NaN
        Public TopRight As Single = Single.NaN
        Public BottomRight As Single = Single.NaN

        Sub New()

        End Sub

        Sub New(TopLeft As Single, TopRight As Single, BottomLeft As Single, BottomRight As Single)
            Me.TopLeft = TopLeft
            Me.TopRight = TopRight
            Me.BottomLeft = BottomLeft
            Me.BottomRight = BottomRight
        End Sub

    End Class

#End Region

End Module