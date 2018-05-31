'            Copyright Clayton S. Lewis 2014-2018.
'   Distributed under the Boost Software License, Version 1.0.
'      (See accompanying file GridET License.rtf or copy at
'            http://www.boost.org/LICENSE_1_0.txt)

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
    Sub MergeRasters(ByVal InputRasterPaths() As String, ByVal ProjectionRasterPath As String, ByVal OutputRasterPath As String, Optional ByVal OutputRasterFormat As GDALProcess.RasterFormat = GDALProcess.RasterFormat.GTiff, Optional ByVal OutputCompression As GDALProcess.Compression = GDALProcess.Compression.DEFLATE, Optional ByVal ResamplingMethod As GDALProcess.ResamplingMethod = GDALProcess.ResamplingMethod.Cubic, Optional ByVal NoDataValue As String = SingleMinValue, Optional ByVal Scale As Double = 1, Optional ByVal Offset As Double = 0, Optional ByRef GDALProcess As GDALProcess = Nothing)
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
    Sub SnapToRaster(ByVal InputRasterPath As String, ByVal SnapRasterPath As String, ByVal OutputRasterPath As String, Optional ByVal UseSnapRasterExtent As Boolean = True, Optional ByVal MaskNoData As Boolean = True, Optional ByVal OutputRasterFormat As GDALProcess.RasterFormat = GDALProcess.RasterFormat.GTiff, Optional ByVal OutputCompression As GDALProcess.Compression = GDALProcess.Compression.DEFLATE, Optional ByVal ResamplingMethod As GDALProcess.ResamplingMethod = GDALProcess.ResamplingMethod.Average, Optional ByVal InNoDataValue As String = SingleMinValue, Optional ByVal OutNoDataValue As String = SingleMinValue, Optional ByVal OutputDataType As GDAL.DataType = GDAL.DataType.GDT_Unknown, Optional ByRef GDALProcess As GDALProcess = Nothing)
        Try
            'Get Raster Properties of Input and Snap Rasters
            Using InputRaster As New Raster(InputRasterPath, GDAL.Access.GA_ReadOnly),
                  SnapRaster As New Raster(SnapRasterPath, GDAL.Access.GA_ReadOnly)

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

                    'Get Intermediate Output Raster Properties
                    Using IntermediateRaster As New Raster(IntermediateRasterPath, GDAL.Access.GA_Update)

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

                    End Using
                End If

                'Compress Output
                GDALProcess.Translate({IntermediateRasterPath}, OutputRasterPath, OutputRasterFormat, OutputCompression, , OutNoDataValue)

                'Delete Temporary Files
                GDALProcess.DeleteRaster(IntermediateRasterPath)
                IO.File.Delete(ProjectionRasterPath)

            End Using
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
    Sub CreateCoordinateRasters(ByVal BaseRasterPath As String, ByVal LatitudeRasterPath As String, ByVal LongitudeRasterPath As String, Optional ByVal OutputRasterFormat As GDALProcess.RasterFormat = GDALProcess.RasterFormat.GTiff, Optional ByVal CreationOptions() As String = Nothing)
        Try
            'Get Base Raster Properties
            Using BaseRaster As New Raster(BaseRasterPath, GDAL.Access.GA_ReadOnly)
                Dim BaseNoDataValue As Single = BaseRaster.BandNoDataValue(0)

                'Setup Transformation from Projected to Geographic Coordinate System Units (Latitude and Longitude Degrees)
                Using SpatialReferenceSystem = New OSR.SpatialReference(BaseRaster.Projection),
                      GeographicCoordinateSystem = SpatialReferenceSystem.CloneGeogCS,
                      CoordinateTransformation = New OSR.CoordinateTransformation(SpatialReferenceSystem, GeographicCoordinateSystem)

                    'Create Output Datasets
                    Dim OutputNoDataValue As Single = Single.MinValue
                    Using LongitudeRaster = CreateNewRaster(LongitudeRasterPath, BaseRaster, {OutputNoDataValue}, , CreationOptions, , OutputRasterFormat),
                          LatitudeRaster = CreateNewRaster(LatitudeRasterPath, BaseRaster, {OutputNoDataValue}, , CreationOptions, , OutputRasterFormat)

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


                    End Using

                End Using

            End Using
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
    Sub RasterPixelsToPoints(ByVal InputRasterPath As String, ByVal OutputVectorPath As String, Optional ByVal VectorFormat As GDALProcess.VectorFormat = GDALProcess.VectorFormat.ESRI_Shapefile)
        Try
            ' Get Input Raster Properties
            Using InputRaster As New Raster(InputRasterPath, GDAL.Access.GA_ReadOnly)
                Dim InputNoDataValue = InputRaster.BandNoDataValue(0)

                'Create Output Vector Dataset (Delete If Already Exists - Coded Specifically for Shapefiles)
                Using Driver = OGR.Ogr.GetDriverByName(GetEnumName(VectorFormat))
                    If IO.File.Exists(OutputVectorPath) Then Driver.DeleteDataSource(OutputVectorPath)

                    Using DataSource = Driver.CreateDataSource(IO.Path.GetDirectoryName(OutputVectorPath), {}),
                          Layer = DataSource.CreateLayer(IO.Path.GetFileNameWithoutExtension(OutputVectorPath), New OSR.SpatialReference(InputRaster.Projection), OGR.wkbGeometryType.wkbPoint, {}),
                          Feature = New OGR.Feature(Layer.GetLayerDefn),
                          Geometery = New OGR.Geometry(OGR.wkbGeometryType.wkbPoint)

                        'Load Pixel Blocks
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
    Sub ScaleRaster(ByVal InputRasterPath As String, ByVal OutputRasterPath As String, Optional ByVal OutputRasterFormat As GDALProcess.RasterFormat = GDALProcess.RasterFormat.GTiff, Optional ByVal CreationOptions() As String = Nothing)
        Try
            'Get Input Raster Properties
            Using InputRaster As New Raster(InputRasterPath, GDAL.Access.GA_ReadOnly)

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

                Using OutputRaster As New Raster(OutputRasterPath, GDAL.Access.GA_Update)

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
                                    OutputValues(I) = Convert.ToUInt16((InputValues(I) - OffsetValue) / ScaleValue)
                                End If
                            Next

                            OutputRaster.Write({B}, OutputValues)
                        Next

                        InputRaster.AdvanceBlock()
                        OutputRaster.AdvanceBlock()
                    Loop

                End Using

            End Using
        Catch Exception As Exception
            MsgBox(Exception.Message)
        End Try
    End Sub

#End Region

#Region "Helper Functions"

    Function GetEnumName(ByVal Enumeration As Object) As String
        Return [Enum].GetName(Enumeration.GetType, Enumeration).Replace("_", " ")
    End Function

    Function GetMaskedExtent(ByVal MaskExtent As Extent, ByVal InputRaster As Raster) As Extent
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

    Function CreateNewRaster(ByVal Path As String, ByVal XCount As Integer, ByVal YCount As Integer, ByVal Projection As String, ByVal GeoTransform() As Double, ByVal NoDataValue As Object(), Optional ByVal DataType As GDAL.DataType = GDAL.DataType.GDT_Float32, Optional ByVal Options() As String = Nothing, Optional ByVal BandCount As Integer = 1, Optional ByVal RasterFormat As GDALProcess.RasterFormat = GDALProcess.RasterFormat.GTiff) As Raster
        Using Driver = GDAL.Gdal.GetDriverByName(RasterFormat.ToString)
            Using Dataset = Driver.Create(Path, XCount, YCount, BandCount, DataType, Options)
                Dataset.SetProjection(Projection)
                Dataset.SetGeoTransform(GeoTransform)

                For B = 1 To BandCount
                    Using Band = Dataset.GetRasterBand(B)
                        Band.SetNoDataValue(NoDataValue(Limit(B - 1, 0, NoDataValue.Length - 1)))
                    End Using
                Next
            End Using
        End Using

        Return New Raster(Path, GDAL.Access.GA_Update)
    End Function

    Function CreateNewRaster(ByVal Path As String, ByVal TemplateRaster As Raster, ByVal NoDataValue As Object(), Optional ByVal DataType As GDAL.DataType = GDAL.DataType.GDT_Float32, Optional ByVal Options() As String = Nothing, Optional ByVal BandCount As Integer = 1, Optional ByVal RasterFormat As GDALProcess.RasterFormat = GDALProcess.RasterFormat.ENVI) As Raster
        Return CreateNewRaster(Path, TemplateRaster.XCount, TemplateRaster.YCount, TemplateRaster.Projection, TemplateRaster.GeoTransform, NoDataValue, DataType, Options, BandCount, RasterFormat)
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

        Protected Overridable Sub Dispose(ByVal Disposing As Boolean)
            If Disposed Then Return

            If Disposing Then
                Dataset.Dispose()
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
        Private Property BlockLength As Integer
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

        Sub New(ByVal RasterPath As String, ByVal Access As GDAL.Access)
            Try
                Dataset = GDAL.Gdal.OpenShared(RasterPath, Access)
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

                Reset()
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

        Function GetPixelQuadFromCoordinate(ByRef Values() As Single, ByVal NoDataValue As Single, ByVal X As Double, ByVal Y As Double) As PixelQuad
            Dim PixelX = InverseGeoTransform(0) + InverseGeoTransform(1) * X + InverseGeoTransform(2) * Y - 0.5
            Dim PixelY = InverseGeoTransform(3) + InverseGeoTransform(4) * X + InverseGeoTransform(5) * Y - 0.5

            Dim XLeft As Integer = Math.Floor(PixelX)
            Dim FractionX = PixelX - XLeft
            Dim YTop As Integer = Math.Floor(PixelY)
            Dim FractionY = PixelY - YTop

            Dim TopLeft As Integer = XCount * YTop + XLeft
            Dim TopRight As Integer = TopLeft + 1
            Dim BottomLeft As Integer = TopLeft + XCount
            Dim BottomRight As Integer = BottomLeft + 1

            With Extent
                If X < .Xmin OrElse X > .Xmax OrElse Y < .Ymin OrElse Y > .Ymax Then
                    TopLeft = -1
                    TopRight = -1
                    BottomLeft = -1
                    BottomRight = -1
                ElseIf X - .Xmin < XResolutionHalf Then
                    If Y - .Ymin < YResolutionHalf Then
                        TopLeft = -1
                        BottomLeft = -1
                        BottomRight = -1
                    ElseIf .Ymax - Y <= YResolutionHalf Then
                        TopLeft = -1
                        TopRight = -1
                        BottomLeft = -1
                    Else
                        TopLeft = -1
                        BottomLeft = -1
                    End If
                ElseIf .Xmax - X <= XResolutionHalf Then
                    If Y - .Ymin < YResolutionHalf Then
                        TopRight = -1
                        BottomLeft = -1
                        BottomRight = -1
                    ElseIf .Ymax - Y <= YResolutionHalf Then
                        TopLeft = -1
                        TopRight = -1
                        BottomRight = -1
                    Else
                        TopRight = -1
                        BottomRight = -1
                    End If
                ElseIf Y - .Ymin < YResolutionHalf Then
                    BottomLeft = -1
                    BottomRight = -1
                ElseIf .Ymax - Y <= YResolutionHalf Then
                    TopLeft = -1
                    TopRight = -1
                End If
            End With

            If Values(TopLeft) = NoDataValue Then Values(TopLeft) = -1
            If Values(TopRight) = NoDataValue Then Values(TopRight) = -1
            If Values(BottomLeft) = NoDataValue Then Values(BottomLeft) = -1
            If Values(BottomRight) = NoDataValue Then Values(BottomRight) = -1

            Return New PixelQuad(TopLeft, TopRight, BottomLeft, BottomRight, FractionX, FractionY)
        End Function

        Sub Reset()
            BlockLength = Math.Max(2 ^ 28, XCount)
            BlockYCount = Math.Min(Int(BlockLength / XCount), YCount)
            BlockLength = XCount * BlockYCount
            BlockCount = Math.Ceiling(YCount / BlockYCount) - 1
            FetchBlock = 0
            Processed = False
        End Sub

        Function Read(ByVal Band() As Integer) As Array
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

        Sub Write(ByVal Band() As Integer, ByVal Values As Object)
            StartFetch(Band)

            Dataset.WriteRaster(0, FetchYOffset, XCount, FetchYSize, Values, XCount, FetchYSize, Band.Length, Band, 0, 0, 0)

            EndFetch()
        End Sub

        Function GetValuesArray(ByVal Band() As Integer) As Array
            StartFetch(Band)
            Return GetValues(Band)
        End Function

        Private Function GetValues(ByVal Band() As Integer) As Object
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

        Private Sub StartFetch(ByVal Band() As Integer)
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

        Sub SetBlockLength(ByVal BlockLength As Integer)
            Me.BlockLength = BlockLength
        End Sub

        Sub AddStatistics()
            Dataset = GDAL.Gdal.Open(Path, GDAL.Access.GA_Update)

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

        Sub New(ByVal X As Double, ByVal Y As Double)
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

        Sub New(ByVal Xmin As Double, ByVal Xmax As Double, ByVal Ymin As Double, ByVal Ymax As Double)
            Me.Xmin = Xmin
            Me.Xmax = Xmax
            Me.Ymin = Ymin
            Me.Ymax = Ymax
        End Sub

    End Class

    Structure PixelQuad
        Private TL As Int32
        Private TR As Int32
        Private BL As Int32
        Private BR As Int32
        Private X As Single
        Private Y As Single

        Private Scenario As Byte
        Private Sub ChooseScenario()
            Scenario = If(TL >= 0, 8, 0) + If(TR >= 0, 4, 0) + If(BL >= 0, 2, 0) + If(BR >= 0, 1, 0)
        End Sub

        Sub New(ByVal TopLeft As Int32, ByVal TopRight As Int32, ByVal BottomLeft As Int32, ByVal BottomRight As Int32, ByVal FractionX As Single, ByVal FractionY As Single)
            Me.TL = TopLeft
            Me.TR = TopRight
            Me.BL = BottomLeft
            Me.BR = BottomRight
            Me.X = FractionX
            Me.Y = FractionY

            ChooseScenario()
        End Sub

        Public Function GetValues(ByRef Array() As Single) As QuadValues
            Return New QuadValues(Array(TL), Array(TR), Array(BL), Array(BR))
        End Function

        Public Function BilinearInterpolation(ByRef Array() As Single) As Single
            Return BilinearInterpolation(Array(TL), Array(TR), Array(BL), Array(BR))
        End Function

        Public Function BilinearInterpolation(ByRef QuadValues As QuadValues) As Single
            With QuadValues
                BilinearInterpolation = BilinearInterpolation(.TopLeft, .TopRight, .BottomLeft, .BottomRight)
            End With
        End Function

        Public Function BilinearInterpolation(ByVal TopLeft As Single, ByVal TopRight As Single, ByVal BottomLeft As Single, ByVal BottomRight As Single, ByVal FractionX As Single, ByVal FractionY As Single) As Single
            X = FractionX
            Y = FractionY

            ChooseScenario()

            Return BilinearInterpolation(TopLeft, TopRight, BottomLeft, BottomRight)
        End Function

        Public Function BilinearInterpolation(ByVal TopLeft As Single, ByVal TopRight As Single, ByVal BottomLeft As Single, ByVal BottomRight As Single) As Single
            Dim Value As Single

            'Exists (TL)(TR)(BL)(BR)
            Select Case Scenario
                Case 15 '1111
                    Value = ((TopLeft - TopRight - BottomLeft + BottomRight) * Y - TopLeft + TopRight) * X + (BottomLeft - TopLeft) * Y + TopLeft
                Case 14 '1110
                    Value = (((-TopLeft + TopRight + BottomLeft) * Y + TopLeft - TopRight) * X + (TopLeft - BottomLeft) * Y - TopLeft) / (X * Y - 1)
                Case 13 '1101
                    Value = (((TopLeft - TopRight + BottomRight) * Y - TopLeft + TopRight) * X - TopLeft * (Y - 1)) / (X * Y + 1 - Y)
                Case 11 '1011
                    Value = (((TopLeft - BottomLeft + BottomRight) * Y - TopLeft) * X + (BottomLeft - TopLeft) * Y + TopLeft) / (1 + (Y - 1) * X)
                Case 7 '0111
                    Value = (((TopRight + BottomLeft - BottomRight) * Y - TopRight) * X - BottomLeft * Y) / ((Y - 1) * X - Y)
                Case 12 '1100
                    Value = TopLeft + (TopRight - TopLeft) * X
                Case 10 '1010
                    Value = TopLeft + (BottomLeft - TopLeft) * Y
                Case 9 '1001
                    Value = TopLeft + (BottomRight - TopLeft) * (X + Y) / 2
                Case 6 '0110
                    Value = TopRight + (BottomLeft - TopRight) * ((1 - X) + Y) / 2
                Case 5 '0101
                    Value = TopRight + (BottomRight - TopRight) * Y
                Case 3 '0011
                    Value = BottomLeft + (BottomRight - BottomLeft) * X
                Case 8 '1000
                    Value = TopLeft
                Case 4 '0100
                    Value = TopRight
                Case 2 '0010
                    Value = BottomLeft
                Case 1 '0001
                    Value = BottomRight
                Case 0 '0000
                    Value = Single.NaN
            End Select

            Return Value
        End Function

    End Structure

    Structure QuadValues
        Public TopLeft As Single
        Public TopRight As Single
        Public BottomRight As Single
        Public BottomLeft As Single

        Public Sub New(ByVal TopLeft As Single, ByVal TopRight As Single, ByVal BottomLeft As Single, ByVal BottomRight As Single)
            Me.TopLeft = TopLeft
            Me.TopRight = TopRight
            Me.BottomLeft = BottomLeft
            Me.BottomRight = BottomRight
        End Sub

    End Structure

    Class RasterArray : Implements IDisposable

        Private Disposed As Boolean = False
        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

        Protected Overridable Sub Dispose(ByVal Disposing As Boolean)
            If Disposed Then Return

            If Disposing Then
                Connection.Dispose()
                Command.Dispose()
            End If

            Disposed = True
        End Sub

        Private SQLitePath As String
        Private ArrayPath As String
        Private Connection As Data.SQLite.SQLiteConnection
        Private Command As Data.SQLite.SQLiteCommand
        Private Lock As New Object

        Private Length As Int64
        Private RasterCount As Int64
        Private RasterLength As Int64
        Private BufferSize As Int32

        Public Sub New(ByVal Path As String)
            Dim Directory = IO.Path.GetDirectoryName(Path)
            Dim ExtensionlessPath = IO.Path.Combine(Directory, IO.Path.GetFileNameWithoutExtension(Path))
            SQLitePath = ExtensionlessPath & ".db"
            ArrayPath = ExtensionlessPath & ".ra"
            If Not IO.Directory.Exists(Directory) Then IO.Directory.CreateDirectory(Directory)
            If Not IO.File.Exists(SQLitePath) Then Data.SQLite.SQLiteConnection.CreateFile(SQLitePath)
            If Not IO.File.Exists(ArrayPath) Then Using Stream = IO.File.Create(ArrayPath) : End Using

            Dim ConnectionBuilder As New Data.SQLite.SQLiteConnectionStringBuilder
            ConnectionBuilder.DataSource = SQLitePath
            ConnectionBuilder.ReadOnly = False
            ConnectionBuilder.DefaultTimeout = 1000
            ConnectionBuilder.PageSize = UInt16.MaxValue
            ConnectionBuilder.MaxPageCount = Int32.MaxValue
            ConnectionBuilder.JournalMode = Data.SQLite.SQLiteJournalModeEnum.Delete
            ConnectionBuilder.SyncMode = Data.SQLite.SynchronizationModes.Full
            ConnectionBuilder.DateTimeKind = DateTimeKind.Utc
            ConnectionBuilder.DateTimeFormatString = "yyyy-MM-dd HH:mm:ss.fffffffK"
            Connection = New System.Data.SQLite.SQLiteConnection(ConnectionBuilder.ToString)
            Connection.Open()

            Command = Connection.CreateCommand
            Using Transaction = Connection.BeginTransaction

                Command.CommandText = "CREATE TABLE IF NOT EXISTS Properties (Key TEXT UNIQUE, Value INTEGER)"
                Command.ExecuteNonQuery()

                Command.CommandText = String.Format("INSERT OR IGNORE INTO Properties (Key, Value) VALUES ('PixelCount', {0}), ('RasterCount', 0)", PixelCount)
                Command.ExecuteNonQuery()

                For Each RasterType As RasterType In [Enum].GetValues(GetType(RasterType))
                    Command.CommandText = String.Format("CREATE TABLE IF NOT EXISTS {0} ({1} UNIQUE, Complete BOOLEAN, Position INTEGER, Offset DOUBLE, Scale DOUBLE)", GetTableName(RasterType), If(RasterType = Calculations.RasterType.Attribute OrElse RasterType = RasterType.Date, "Name TEXT", "Date DATETIME"))
                    Command.ExecuteNonQuery()
                Next

                Command.CommandText = "SELECT Key, Value FROM Properties"
                Using Reader = Command.ExecuteReader
                    While Reader.Read
                        Select Case Reader.GetString(0)
                            Case "PixelCount" : Length = Reader.GetInt64(1)
                            Case "RasterCount" : RasterCount = Reader.GetInt64(1)
                        End Select
                    End While
                End Using

                Transaction.Commit()
            End Using

            RasterLength = 2 * PixelCount
            BufferSize = 2 ^ 17
        End Sub

        Public Function ReadAttribute(ByVal Attribute As AttributeType) As Single()
            Return Read(Attribute.ToString, Nothing, RasterType.Attribute)
        End Function

        Public Function ReadDate(ByVal Year As Int16, ByVal Calculation As CalculationDateType) As UInt16()
            Return Read(Calculation.ToString & vbTab & Year, Nothing, RasterType.Date)
        End Function

        Public Function ReadRaster(ByVal Year As Int16, ByVal Month As Byte, ByVal Day As Byte) As Single()
            Return Read("", New DateTime(Year, Month, Day, 13, 0, 0, 0, DateTimeKind.Utc), RasterType.Raster)
        End Function

        Public Function ReadStatistic(ByVal Year As Int16, ByVal Month As Byte, ByVal RasterType As RasterType) As Single()
            Return Read("", New DateTime(Year, If(Month < 13, Month, 12), If(Month < 13, 1, 31), 0, 0, 0, 0, DateTimeKind.Utc), RasterType)
        End Function

        Private Function Read(ByVal Name As String, ByVal Time As DateTime, ByVal RasterType As RasterType) As Object
            'Load Raster Postion, Offset, And Scale
            Dim RasterAdded As Boolean = False
            Dim Position As Int64 = -1
            Dim Offset As Double = -1
            Dim Scale As Double = -1

            SyncLock Lock
                Command.CommandText = String.Format("SELECT Position, Offset, Scale FROM {0} WHERE {1} = @{1} AND Complete", GetTableName(RasterType), If(RasterType = RasterType.Attribute OrElse RasterType = RasterType.Date, "Name", "Date"))
                Command.Parameters.Add("@Name", DbType.String).Value = Name
                Command.Parameters.Add("@Date", DbType.DateTime).Value = Time.ToUniversalTime
                Using Reader = Command.ExecuteReader
                    If Reader.HasRows Then
                        Reader.Read()

                        Position = Reader.GetInt64(0)
                        Offset = Reader.GetDouble(1)
                        Scale = Reader.GetDouble(2)

                        RasterAdded = True
                    End If
                End Using
            End SyncLock

            'Load Raster 16 Bit Integers And Convert To 32 Bit Floats
            If RasterAdded Then
                Using Stream As New IO.FileStream(ArrayPath, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.ReadWrite, BufferSize)
                    Stream.Seek(Position, IO.SeekOrigin.Begin)

                    If RasterType = RasterType.Date Then
                        Dim Array(PixelCount - 1) As UInt16

                        'Dim Bytes(PixelCount * 2 - 1) As Byte
                        'Stream.Read(Bytes, 0, Bytes.Length)
                        'Buffer.BlockCopy(Bytes, 0, Array, 0, Bytes.Length)

                        Dim Bytes(1) As Byte
                        For I = 0 To PixelCount - 1
                            Stream.Read(Bytes, 0, 2)
                            Array(I) = BitConverter.ToUInt16(Bytes, 0)
                        Next

                        Read = Array
                    Else
                        Dim Array(PixelCount - 1) As Single

                        Dim Bytes(1) As Byte
                        For I = 0 To PixelCount - 1
                            Stream.Read(Bytes, 0, 2)
                            Array(I) = BitConverter.ToUInt16(Bytes, 0) * Scale + Offset
                        Next

                        Read = Array
                    End If
                End Using
            Else
                Read = Nothing
            End If
        End Function

        Public Sub WriteAttribute(ByRef Array() As Single, ByVal Attribute As AttributeType)
            Write(Array, Nothing, Attribute.ToString, Nothing, RasterType.Attribute)
        End Sub

        Public Sub WriteDate(ByRef Array() As UInt16, ByVal Year As Int16, ByVal Calculation As CalculationDateType)
            Write(Nothing, Array, Calculation.ToString & vbTab & Year, Nothing, RasterType.Date)
        End Sub

        Public Sub WriteRaster(ByRef Array() As Single, ByVal Year As Int16, ByVal Month As Byte, ByVal Day As Byte)
            Write(Array, Nothing, "", New DateTime(Year, Month, Day, 13, 0, 0, 0, DateTimeKind.Utc), RasterType.Raster)
        End Sub

        Public Sub WriteStatistic(ByRef Array() As Single, ByVal Year As Int16, ByVal Month As Byte, ByVal RasterType As RasterType)
            Write(Array, Nothing, "", New DateTime(Year, If(Month < 13, Month, 12), If(Month < 13, 1, 31), 0, 0, 0, 0, DateTimeKind.Utc), RasterType)
        End Sub

        Private Sub Write(ByRef SingleArray() As Single, ByRef UInt16Array() As UInt16, ByVal Name As String, ByVal Time As DateTime, ByVal RasterType As RasterType)
            'Determine Offset And Scale
            Dim Offset As Double = 0
            Dim Scale As Double = 1

            If RasterType <> RasterType.Date Then
                Dim Min = Double.MaxValue
                Dim Max = Double.MinValue
                For Each Value In SingleArray
                    If Value < Min Then Min = Value
                    If Value > Max Then Max = Value
                Next
                If Max = Min Then Max += 1

                Offset = Min
                Scale = (Max - Min) / UInt16.MaxValue
            End If

            'Determine Raster Starting Position In File
            Dim Position As Int64 = -1
            Dim TableName = GetTableName(RasterType)
            Dim ColumnName = If(RasterType = Calculations.RasterType.Attribute OrElse RasterType = RasterType.Date, "Name", "Date")
            SyncLock Lock
                Using Transaction = Connection.BeginTransaction

                    Dim RasterAdded As Boolean = False
                    Command.CommandText = String.Format("SELECT Position FROM {0} WHERE {1} = @{1}", TableName, ColumnName)
                    Command.Parameters.Add("@" & ColumnName, DbType.String).Value = Name
                    Command.Parameters.Add("@Date", DbType.DateTime).Value = Time.ToUniversalTime
                    Using Reader = Command.ExecuteReader
                        If Reader.HasRows Then
                            Reader.Read()
                            Position = Reader.GetInt64(0)
                            RasterAdded = True
                        End If
                    End Using

                    Command.Parameters.Add("@Offset", DbType.Double).Value = Offset
                    Command.Parameters.Add("@Scale", DbType.Double).Value = Scale

                    If RasterAdded Then
                        Command.CommandText = String.Format("UPDATE {0} SET Complete = 0, Offset = @Offset, Scale = @Scale WHERE {1} = @{1}", TableName, ColumnName)
                    Else
                        Position = RasterCount * RasterLength
                        RasterCount += 1

                        Command.CommandText = "UPDATE Properties SET Value = Value + 1 WHERE Key = 'RasterCount'"
                        Command.ExecuteNonQuery()

                        Command.CommandText = String.Format("INSERT INTO {0} ({1}, Complete, Position, Offset, Scale) VALUES (@{1}, 0, @Position, @Offset, @Scale)", TableName, ColumnName)
                        Command.Parameters.Add("@Position", DbType.Int64).Value = Position
                    End If
                    Command.ExecuteNonQuery()

                    Transaction.Commit()
                End Using
            End SyncLock

            'Convert 32 Bit Float Array To 16 Bit Integers And Output Bytes To File
            Using Stream As New IO.FileStream(ArrayPath, IO.FileMode.Open, IO.FileAccess.Write, IO.FileShare.ReadWrite, BufferSize)
                Stream.Seek(Position, IO.SeekOrigin.Begin)

                If RasterType = RasterType.Date Then
                    Dim Bytes(PixelCount * 2 - 1) As Byte
                    Buffer.BlockCopy(UInt16Array, 0, Bytes, 0, Bytes.Length)
                    Stream.Write(Bytes, 0, Bytes.Length)
                Else
                    For Each Value In SingleArray
                        Dim ScaledBytes = BitConverter.GetBytes(Convert.ToUInt16((Value - Offset) / Scale))
                        Stream.Write(ScaledBytes, 0, 2)
                    Next
                End If
            End Using

            SyncLock Lock
                Command.CommandText = String.Format("UPDATE {0} SET Complete = 1 WHERE {1} = @{1}", TableName, ColumnName)
                Command.ExecuteNonQuery()
            End SyncLock
        End Sub

        Private Function GetTableName(ByVal RasterType As RasterType) As String
            Return RasterType.ToString & "s"
        End Function

        Public Sub CalculatePeriodStatistics(ByVal RasterType As RasterType, ByVal MinDate As DateTime, ByVal MaxDate As DateTime)
            If MaxDate >= MinDate Then
                'Get Fully Processed Months
                Dim Months As New List(Of YearMonth)
                SyncLock Lock
                    Command.CommandText = "SELECT SUBSTR(Date,1,7) YearMonth, COUNT(*) FROM Rasters WHERE Date BETWEEN @Date1 AND @Date2 AND Complete GROUP BY YearMonth ORDER BY YearMonth"
                    Command.Parameters.Add("@Date1", DbType.DateTime).Value = New DateTime(MinDate.Year, MinDate.Month, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                    Command.Parameters.Add("@Date2", DbType.DateTime).Value = (New DateTime(MaxDate.Year, MaxDate.Month, 1, 0, 0, 0, 0, DateTimeKind.Utc)).AddMonths(1).AddTicks(-1)
                    Using Reader = Command.ExecuteReader
                        While Reader.Read
                            Dim Time = Reader.GetString(0).Split("-")
                            Dim YearMonth As New YearMonth(Time(0), Time(1), Reader.GetInt32(1))

                            If YearMonth.Days = DateTime.DaysInMonth(Time(0), Time(1)) Then Months.Add(YearMonth)
                        End While
                    End Using
                End SyncLock

                'Get Fully Processed Years
                Dim Years As New List(Of YearMonth)
                Dim YearLookup As New Dictionary(Of Int16, Int32)
                SyncLock Lock
                    Command.CommandText = "SELECT SUBSTR(Date,1,4) Year, COUNT(*) FROM Rasters WHERE Date BETWEEN @Date1 AND @Date2 AND Complete GROUP BY Year ORDER BY Year"
                    Command.Parameters.Add("@Date1", DbType.DateTime).Value = New DateTime(MinDate.Year, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                    Command.Parameters.Add("@Date2", DbType.DateTime).Value = (New DateTime(MaxDate.Year + 1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).AddTicks(-1)
                    Using Reader = Command.ExecuteReader
                        While Reader.Read
                            Dim X = Reader.GetString(0)
                            Dim YearMonth As New YearMonth(Reader.GetString(0), 13, Reader.GetInt32(1))

                            If YearMonth.Days = (New DateTime(YearMonth.Year, 12, 31)).DayOfYear Then
                                YearLookup.Add(YearMonth.Year, Years.Count)
                                Years.Add(YearMonth)
                            End If
                        End While
                    End Using
                End SyncLock

                Dim YearlyStatistics(PixelCount - 1) As Single
                Dim MonthLookup As New HashSet(Of Int32)

                For M = 0 To Months.Count - 1
                    Dim MonthDate = Months(M)
                    Dim MonthlyStatistics(PixelCount - 1) As Single

                    'Load Daily Rasters And Sum In Monthly Array
                    For Day = 1 To MonthDate.Days
                        Dim DailyValues = ReadRaster(MonthDate.Year, MonthDate.Month, Day)
                        For I = 0 To PixelCount - 1
                            MonthlyStatistics(I) += DailyValues(I)
                        Next
                    Next

                    'Annual Calculations
                    If YearLookup.ContainsKey(MonthDate.Year) Then
                        For I = 0 To PixelCount - 1
                            YearlyStatistics(I) += MonthlyStatistics(I)
                        Next
                        MonthLookup.Add(MonthDate.Year * 12 + MonthDate.Month)

                        If MonthDate.Month = 12 OrElse M = Months.Count - 1 Then
                            For J = 1 To 12
                                If Not MonthLookup.Contains(MonthDate.Year * 12 + J) Then
                                    If RasterType = Calculations.RasterType.Sum Then
                                        'Load Monthly Sums And Add To Annual Array
                                        Dim MonthlyValues = ReadStatistic(MonthDate.Year, J, RasterType)
                                        If MonthlyValues IsNot Nothing Then
                                            For I = 0 To PixelCount - 1
                                                YearlyStatistics(I) += MonthlyValues(I)
                                            Next
                                        Else
                                            'Load Daily Rasters And Sum In Annual Array
                                            For Day = 1 To MonthDate.Days
                                                Dim DailyValues = ReadRaster(MonthDate.Year, MonthDate.Month, Day)
                                                For I = 0 To PixelCount - 1
                                                    YearlyStatistics(I) += DailyValues(I)
                                                Next
                                            Next
                                        End If
                                    ElseIf RasterType = Calculations.RasterType.Average Then
                                        'Load Daily Rasters And Sum In Annual Array
                                        For Day = 1 To MonthDate.Days
                                            Dim DailyValues = ReadRaster(MonthDate.Year, MonthDate.Month, Day)
                                            For I = 0 To PixelCount - 1
                                                YearlyStatistics(I) += DailyValues(I)
                                            Next
                                        Next
                                    End If
                                End If
                            Next

                            'Daily Average If Specified
                            If RasterType = RasterType.Average Then
                                Dim TotalDays = Years(YearLookup(MonthDate.Year)).Days
                                For I = 0 To PixelCount - 1
                                    YearlyStatistics(I) /= TotalDays
                                Next
                            End If

                            WriteStatistic(YearlyStatistics, MonthDate.Year, 13, RasterType)

                            If M <> Months.Count - 1 Then
                                ReDim YearlyStatistics(PixelCount - 1)
                                MonthLookup.Clear()
                            End If
                        End If
                    End If

                    'Daily Average If Specified
                    If RasterType = RasterType.Average Then
                        For I = 0 To PixelCount - 1
                            MonthlyStatistics(I) /= MonthDate.Days
                        Next
                    End If

                    'Write Monthly Statistics
                    WriteStatistic(MonthlyStatistics, MonthDate.Year, MonthDate.Month, RasterType)
                Next
            End If
        End Sub

        Public Sub GetMinAndMaxDates(ByVal RasterType As RasterType, ByRef MinDate As DateTime, ByRef MaxDate As DateTime)
            If RasterType <> Calculations.RasterType.Attribute Then
                SyncLock Lock
                    Command.CommandText = String.Format("SELECT MIN(Date) FROM {0} WHERE Complete UNION ALL SELECT MAX(Date) FROM {0} WHERE Complete", GetTableName(RasterType))
                    Using Reader = Command.ExecuteReader
                        Reader.Read()
                        If Not Reader.IsDBNull(0) Then MinDate = Reader.GetDateTime(0)

                        Reader.Read()
                        If Not Reader.IsDBNull(0) Then MaxDate = Reader.GetDateTime(0)
                    End Using
                End SyncLock
            End If
        End Sub

        Public Sub ExportAttribute(ByVal Path As String, ByVal Attribute As AttributeType, Optional ByVal UseCompression As Boolean = False, Optional ByVal RasterFormat As GDALProcess.RasterFormat = GDALProcess.RasterFormat.GTiff)
            Export(Path, Attribute.ToString, Nothing, RasterType.Attribute, UseCompression, RasterFormat)
        End Sub

        Public Sub ExportDate(ByVal Path As String, ByVal Year As Int16, ByVal Calculation As CalculationDateType, Optional ByVal UseCompression As Boolean = False, Optional ByVal RasterFormat As GDALProcess.RasterFormat = GDALProcess.RasterFormat.GTiff)
            Export(Path, Calculation.ToString & vbTab & Year, Nothing, RasterType.Date, UseCompression, RasterFormat)
        End Sub

        Public Sub ExportRaster(ByVal Path As String, ByVal Year As Int16, ByVal Month As Byte, ByVal Day As Byte, Optional ByVal UseCompression As Boolean = False, Optional ByVal RasterFormat As GDALProcess.RasterFormat = GDALProcess.RasterFormat.GTiff)
            Export(Path, "", New DateTime(Year, Month, Day, 13, 0, 0, 0, DateTimeKind.Utc), RasterType.Raster, UseCompression, RasterFormat)
        End Sub

        Public Sub ExportStatistic(ByVal Path As String, ByVal Year As Int16, ByVal Month As Byte, ByVal RasterType As RasterType, Optional ByVal UseCompression As Boolean = False, Optional ByVal RasterFormat As GDALProcess.RasterFormat = GDALProcess.RasterFormat.GTiff)
            Export(Path, "", New DateTime(Year, If(Month < 13, Month, 12), If(Month < 13, 1, 31), 0, 0, 0, 0, DateTimeKind.Utc), RasterType, UseCompression, RasterFormat)
        End Sub

        Private Sub Export(ByVal Path As String, ByVal Name As String, ByVal Time As DateTime, ByVal RasterType As RasterType, ByVal UseCompression As Boolean, ByVal RasterFormat As GDALProcess.RasterFormat)
            Dim Values() As Single = Read(Name, Time, RasterType)
            If Values IsNot Nothing Then
                Dim Array(ProjectMask.Length - 1) As Single

                Dim NoDataValue = Single.MinValue
                Dim J As Int32 = 0
                For I = 0 To ProjectMask.Length - 1
                    If ProjectMask(I) > 0 Then
                        Array(I) = Values(J)
                        J += 1
                    Else
                        Array(I) = NoDataValue
                    End If
                Next

                Using Driver = GDAL.Gdal.GetDriverByName(RasterFormat.ToString)
                    Using Dataset = Driver.Create(Path, ProjectXCount, ProjectYCount, 1, GDAL.DataType.GDT_Float32, If(UseCompression, {"COMPRESS=DEFLATE"}, {}))
                        Dataset.SetProjection(ProjectProjection)
                        Dataset.SetGeoTransform(ProjectGeoTransform)

                        Dataset.WriteRaster(0, 0, ProjectXCount, ProjectYCount, Array, ProjectXCount, ProjectYCount, 1, {1}, 0, 0, 0)

                        Using Band = Dataset.GetRasterBand(1)
                            Band.SetNoDataValue(NoDataValue)

                            Dim Min As Double = 0, Max As Double = 0, Mean As Double = 0, StDev As Double = 0, Buckets As Integer = 256, Histogram(Buckets - 1) As Integer

                            Band.ComputeStatistics(False, Min, Max, Mean, StDev, Nothing, Nothing)
                            Band.SetStatistics(Min, Max, Mean, StDev)

                            Band.GetHistogram(Min, Max, Buckets, Histogram, True, False, Nothing, Nothing)
                            Band.SetDefaultHistogram(Min, Max, Buckets, Histogram)
                        End Using
                    End Using
                End Using
            End If
        End Sub

        Private Class YearMonth
            Public Year As Int16
            Public Month As Byte
            Public Days As Int16

            Public Sub New()

            End Sub

            Public Sub New(ByVal Year As Int16, ByVal Month As Byte, ByVal Days As Int16)
                Me.Year = Year
                Me.Month = Month
                Me.Days = Days
            End Sub

        End Class

    End Class

#End Region

End Module