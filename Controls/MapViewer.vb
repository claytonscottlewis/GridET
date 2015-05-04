Public Class MapViewer

#Region "Variables"

    Private MapObject As MapServer.mapObj
    Private FullExtent As MapServer.rectObj
    Private DragOperation As Boolean = False
    Private DragRectangle As Rectangle = New Rectangle(New Point(0, 0), New Size(0, 0))
    Private DragStartPoint As Point
    Private DragEndPoint As Point
    Private MapPoint As Point
    Private StatusString As String = "[ X = {0} ] [ Y = {1} ] [ Scale = 1 : {2} ]"
    Private ColorPalette As Color()
    Private MapServerRaster As Raster

#End Region

#Region "Legend"

    Private Sub MapViewer_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        For Each Control In Me.Controls
            GetType(Control).InvokeMember("DoubleBuffered", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance Or Reflection.BindingFlags.SetProperty, Nothing, Control, New Object() {True})
        Next

        For I = 0 To LegendPalette.Name.Length - 1
            ColorRampBox.Items.Add(LegendPalette.Name(I))
        Next
        ColorRampBox.SelectedIndex = ColorRampBox.Items.Count - 1
    End Sub

    Public Sub LoadProjectDirectories()
        ProjectDirectoryBox.Items.Clear()

        If IO.Directory.Exists(ProjectDirectory) Then
            RemoveHandler ProjectDirectoryBox.SelectedValueChanged, AddressOf LoadSubDirectories

            For Each Directory In {AreaFeaturesDirectory, IntermediateCalculationsDirectory, OutputCalculationsDirectory}
                ProjectDirectoryBox.Items.Add(IO.Path.GetFileName(Directory))
            Next

            AddHandler ProjectDirectoryBox.SelectedValueChanged, AddressOf LoadSubDirectories

            If ProjectDirectoryBox.Items.Count > 0 Then
                ProjectDirectoryBox.Text = ProjectDirectoryBox.Items(ProjectDirectoryBox.Items.Count - 1)
            Else
                LoadSubDirectories(Nothing, Nothing)
            End If
        Else
            LoadSubDirectories(Nothing, Nothing)
        End If
    End Sub

    Private Sub LoadSubDirectories(sender As Object, e As System.EventArgs) Handles ProjectDirectoryBox.SelectedValueChanged
        SubDirectoryBox.Items.Clear()

        If ProjectDirectoryBox.Text <> "" Then
            RemoveHandler SubDirectoryBox.SelectedValueChanged, AddressOf LoadFiles

            For Each Directory In IO.Directory.GetDirectories(IO.Path.Combine(ProjectDirectory, ProjectDirectoryBox.Text), "*", IO.SearchOption.TopDirectoryOnly)
                SubDirectoryBox.Items.Add(IO.Path.GetFileName(Directory))
            Next

            AddHandler SubDirectoryBox.SelectedValueChanged, AddressOf LoadFiles

            If SubDirectoryBox.Items.Count > 0 Then
                SubDirectoryBox.Text = SubDirectoryBox.Items(0)
            Else
                LoadFiles(Nothing, Nothing)
            End If
        Else
            LoadFiles(Nothing, Nothing)
        End If
    End Sub

    Private Sub LoadFiles(sender As Object, e As System.EventArgs) Handles SubDirectoryBox.SelectedValueChanged
        FileBox.Items.Clear()

        If SubDirectoryBox.Text <> "" Then
            RemoveHandler FileBox.SelectedValueChanged, AddressOf LoadRasters

            For Each File In IO.Directory.GetFiles(IO.Path.Combine(ProjectDirectory, ProjectDirectoryBox.Text, SubDirectoryBox.Text), "*", IO.SearchOption.TopDirectoryOnly).Where(Function(F) F.EndsWith(".tif") Or F.EndsWith(".db")).ToArray
                FileBox.Items.Add(IO.Path.GetFileName(File))
            Next

            AddHandler FileBox.SelectedValueChanged, AddressOf LoadRasters

            If FileBox.Items.Count > 0 Then
                FileBox.Text = FileBox.Items(0)
            Else
                LoadRasters(Nothing, Nothing)
            End If
        Else
            LoadRasters(Nothing, Nothing)
        End If
    End Sub

    Private Sub LoadRasters(sender As Object, e As System.EventArgs) Handles FileBox.SelectedValueChanged
        RasterBox.Items.Clear()

        If FileBox.Text <> "" Then
            RemoveHandler RasterBox.SelectedValueChanged, AddressOf LoadRasterBand

            Dim FilePath = IO.Path.Combine(ProjectDirectory, ProjectDirectoryBox.Text, SubDirectoryBox.Text, FileBox.Text)
            Select Case IO.Path.GetExtension(FilePath)
                Case ".db"
                    Using Connection = CreateConnection(FilePath)
                        Connection.Open()

                        Using Command = Connection.CreateCommand
                            Command.CommandText = "SELECT Date FROM Rasters"

                            Using Reader = Command.ExecuteReader
                                While Reader.Read
                                    RasterBox.Items.Add(Reader.GetDateTime(0).ToString)
                                End While
                            End Using
                        End Using
                    End Using
                Case ".tif"
                    Using Raster As New Raster(FilePath)
                        Raster.Open(GDAL.Access.GA_ReadOnly)

                        For I = 1 To Raster.BandCount
                            Using Band = Raster.Dataset.GetRasterBand(I)
                                Dim BandDescription = Band.GetDescription
                                If BandDescription = "" Then BandDescription = "Band " & I
                                RasterBox.Items.Add(BandDescription)
                            End Using
                        Next
                    End Using
            End Select

            AddHandler RasterBox.SelectedValueChanged, AddressOf LoadRasterBand

            If RasterBox.Items.Count > 0 Then
                RasterBox.Text = RasterBox.Items(RasterBox.Items.Count - 1)
            Else
                LoadRasterBand(Nothing, Nothing)
            End If
        Else
            LoadRasterBand(Nothing, Nothing)
        End If
    End Sub

    Private Sub LoadRasterBand(sender As Object, e As System.EventArgs) Handles RasterBox.SelectedValueChanged
        If RasterBox.Text <> "" Then
            Dim FilePath = IO.Path.Combine(ProjectDirectory, ProjectDirectoryBox.Text, SubDirectoryBox.Text, FileBox.Text)

            If IO.File.Exists(MapServerRasterPath) Then
                MapServerRaster = New Raster(MapServerRasterPath)
            Else
                Using MaskRaster As New Raster(MaskRasterPath)
                    MapServerRaster = CreateNewRaster(MapServerRasterPath, MaskRaster.XCount, MaskRaster.YCount, MaskRaster.Projection, MaskRaster.GeoTransform, {Single.MinValue})
                End Using
            End If
            Dim RasterPath As String = ""
            Dim Band As Integer = 1
            Dim MinRasterValue As Double = Double.MaxValue
            Dim MaxRasterValue As Double = Double.MinValue

            Select Case IO.Path.GetExtension(FilePath)
                Case ".db"
                    Using Connection = CreateConnection(FilePath)
                        Connection.Open()

                        Using Command = Connection.CreateCommand
                            Command.CommandText = "SELECT Image FROM Rasters WHERE Date = @Date"
                            Command.Parameters.Add("@Date", DbType.DateTime).Value = DateTime.Parse(RasterBox.Text)

                            RasterPath = "/vsimem/MapServerTemporaryRaster" & RasterBox.Text
                            GDAL.Gdal.FileFromMemBuffer(RasterPath, Command.ExecuteScalar)
                        End Using
                    End Using
                Case ".tif"
                    RasterPath = FilePath
                    Band = RasterBox.SelectedIndex + 1
            End Select

            Using Raster As New Raster(RasterPath)
                Raster.Open(GDAL.Access.GA_ReadOnly)

                MapServerRaster = New Raster(MapServerRasterPath)
                MapServerRaster.Open(GDAL.Access.GA_Update)

                Do Until Raster.BlocksProcessed
                    Dim RasterPixels = Raster.Read({Band})

                    Dim NoDataValue = Raster.BandNoDataValue(Band - 1)

                    For I = 0 To RasterPixels.Length - 1
                        If RasterPixels(I) <> NoDataValue Then
                            If RasterPixels(I) < MinRasterValue Then MinRasterValue = RasterPixels(I)
                            If RasterPixels(I) > MaxRasterValue Then MaxRasterValue = RasterPixels(I)
                        Else
                            RasterPixels(I) = Single.MinValue
                        End If
                    Next

                    MapServerRaster.Write({1}, RasterPixels)

                    MapServerRaster.AdvanceBlock()
                    Raster.AdvanceBlock()
                Loop

            End Using
            If FilePath.EndsWith(".db") Then GDAL.Gdal.Unlink(RasterPath)

            MinRasterValue = RoundToSignificantDigits(MinRasterValue, 4, False)
            MaxRasterValue = RoundToSignificantDigits(MaxRasterValue, 4)
            If MaxRasterValue = MinRasterValue Then MaxRasterValue += 1

            CreateETMap(MinRasterValue, MaxRasterValue)
        Else
            MapObject = Nothing
            Map.Image = Nothing

            LegendImage.Image = Nothing
            LegendMaxValue.Text = ""
            LegendMinValue.Text = ""

            TopLeft.Text = ""
            TopRight.Text = ""
            BottomLeft.Text = ""
            BottomRight.Text = ""

            If MapServerRaster IsNot Nothing Then MapServerRaster.Dispose()
        End If
    End Sub

    Private Sub InvertColorRamp_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles InvertColorRamp.CheckedChanged
        Array.Reverse(ColorPalette)

        CreateETMap(Nothing, Nothing)
    End Sub

    Private Sub ColorRampBox_SelectedValueChanged(sender As Object, e As System.EventArgs) Handles ColorRampBox.SelectedValueChanged
        ColorPalette = LegendPalette.Value(ColorRampBox.SelectedIndex)
        If InvertColorRamp.CheckState = CheckState.Checked Then Array.Reverse(ColorPalette)

        If ProjectDirectoryBox.Items.Count > 0 Then CreateETMap(Nothing, Nothing)
    End Sub

#End Region

#Region "Map and Toolbar"

    Private Sub Zoom(MapX As Double, MapY As Double, ZoomFactor As Integer)
        If MapObject IsNot Nothing Then
            MapObject.zoomPoint(ZoomFactor, New MapServer.pointObj(MapX, MapY, 0, 0), Map.Width, Map.Height, MapObject.extent, Nothing)
            UpdateMap()
        End If
    End Sub

    Private Sub ZoomScale(MapX As Double, MapY As Double, Scale As Double)
        If MapObject IsNot Nothing Then
            MapObject.zoomScale(Scale, New MapServer.pointObj(MapX, MapY, 0, 0), Map.Width, Map.Height, MapObject.extent, Nothing)
        End If
    End Sub

    Private Sub ZoomFullExtent_Click(sender As Object, e As EventArgs) Handles ZoomFullExtent.Click
        If MapObject IsNot Nothing Then
            MapObject.extent = FullExtent
            UpdateMap()
        End If
    End Sub

    Private Sub ZoomInFixed_Click(sender As Object, e As EventArgs) Handles ZoomInFixed.Click
        If MapObject IsNot Nothing Then Zoom(Map.Width / 2, Map.Height / 2, 2)
    End Sub

    Private Sub ZoomOutFixed_Click(sender As Object, e As EventArgs) Handles ZoomOutFixed.Click
        If MapObject IsNot Nothing Then Zoom(Map.Width / 2, Map.Height / 2, -2)
    End Sub

    Private Sub Pan_Click(sender As Object, e As System.EventArgs) Handles Pan.Click
        If Pan.Checked Then
            Cursor = Cursors.SizeAll
        Else
            Cursor = Cursors.Arrow
        End If
    End Sub

    Private Sub Zoom_CheckedChanged(sender As Object, e As System.EventArgs) Handles Pan.CheckedChanged, ZoomInBox.CheckedChanged, ZoomOutBox.CheckedChanged
        RemoveHandler Pan.CheckedChanged, AddressOf Zoom_CheckedChanged
        RemoveHandler ZoomInBox.CheckedChanged, AddressOf Zoom_CheckedChanged
        RemoveHandler ZoomOutBox.CheckedChanged, AddressOf Zoom_CheckedChanged

        For Each Button In {Pan, ZoomInBox, ZoomOutBox}
            If Button IsNot sender Then
                Button.Checked = False
            End If
        Next

        AddHandler Pan.CheckedChanged, AddressOf Zoom_CheckedChanged
        AddHandler ZoomInBox.CheckedChanged, AddressOf Zoom_CheckedChanged
        AddHandler ZoomOutBox.CheckedChanged, AddressOf Zoom_CheckedChanged
    End Sub

    Private Sub Map_MouseWheel(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles Map.MouseWheel
        If MapObject IsNot Nothing Then
            'Dim ScaleFactor = 0.9
            'If Math.Sign(e.Delta) < 0 Then ScaleFactor = 1 / ScaleFactor
            'ZoomScale(e.X, e.Y, MapObject.scaledenom * ScaleFactor)
            Zoom(e.X, e.Y, 2 * Math.Sign(e.Delta))
        End If
    End Sub

    Private Sub ZoomInBox_Click(sender As System.Object, e As System.EventArgs) Handles ZoomInBox.Click
        If ZoomInBox.Checked Then
            Cursor = Cursors.Cross
        Else
            Cursor = Cursors.Arrow
        End If
    End Sub

    Private Sub ZoomOutBox_Click(sender As System.Object, e As System.EventArgs) Handles ZoomOutBox.Click
        If ZoomOutBox.Checked Then
            Cursor = Cursors.Cross
        Else
            Cursor = Cursors.Arrow
        End If
    End Sub

    Private Sub Map_MouseDown(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles Map.MouseDown
        If MapObject IsNot Nothing Then
            If e.Button = Windows.Forms.MouseButtons.Left And (ZoomInBox.Checked Or ZoomOutBox.Checked) Then
                DragOperation = True
                DragStartPoint = Map.PointToScreen(New Point(e.X, e.Y))
                DragRectangle = New Rectangle
            End If
        End If
    End Sub

    Private Sub Map_MouseMove(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles Map.MouseMove
        If DragOperation Then
            Dim Point = Map.PointToScreen(New Point(Limit(e.X, 0, Map.Width), Limit(e.Y, 0, Map.Height)))
            If DragEndPoint = Point Then Exit Sub

            ControlPaint.DrawReversibleFrame(DragRectangle, Map.BackColor, FrameStyle.Dashed)

            DragEndPoint = Point
            DragRectangle = New Rectangle(DragStartPoint.X, DragStartPoint.Y, DragEndPoint.X - DragStartPoint.X, DragEndPoint.Y - DragStartPoint.Y)

            ControlPaint.DrawReversibleFrame(DragRectangle, Map.BackColor, FrameStyle.Dashed)
        End If

        MapPoint = e.Location
        UpdateMapPoint()

        Map.Focus()
    End Sub

    Private Sub Map_MouseUp(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles Map.MouseUp
        If DragRectangle.Width > 0 And DragRectangle.Height > 0 Then
            If DragOperation Then
                Dim Scale As Double = MapObject.scaledenom

                Dim Point = Map.PointToScreen(New Point(0, 0))
                MapObject.zoomRectangle(New MapServer.rectObj(DragRectangle.Left - Point.X, DragRectangle.Bottom - Point.Y, DragRectangle.Right - Point.X, DragRectangle.Top - Point.Y, True), Map.Width, Map.Height, MapObject.extent, Nothing)
                If ZoomOutBox.Checked = True Then
                    MapObject.zoomScale(Scale * 2, New MapServer.pointObj(Map.Width / 2, Map.Height / 2, 0, 0), Map.Width, Map.Height, MapObject.extent, Nothing)
                End If

                UpdateMap()

                Me.Refresh()
                'Cursor = Cursors.Arrow
                DragOperation = False
                'ZoomInBox.Checked = False
                'ZoomOutBox.Checked = False
            ElseIf Pan.Checked Then
                Zoom(e.X, e.Y, 1)
            End If
        End If
    End Sub

    Private Sub Map_MouseLeave(sender As Object, e As System.EventArgs) Handles Map.MouseLeave
        TopLeft.Text = ""
        TopRight.Text = ""
        BottomLeft.Text = ""
        BottomRight.Text = ""
    End Sub

    Private Sub Map_SizeChanged(sender As Object, e As System.EventArgs) Handles Map.SizeChanged
        If MapObject IsNot Nothing Then
            MapObject.width = Map.Width
            MapObject.height = Map.Height

            UpdateMap()
        End If
    End Sub

    Private Sub SplitContainer_SplitterMoved(sender As Object, e As System.Windows.Forms.SplitterEventArgs) Handles SplitContainer.SplitterMoved
        Map_SizeChanged(Nothing, Nothing)
    End Sub

#End Region

#Region "Functions"

    Private Sub CreateETMap(MinRasterValue As Double, MaxRasterValue As Double)
        If MinRasterValue <> Nothing Or MaxRasterValue <> Nothing Then
            LegendMinValue.Text = "Min:  " & MinRasterValue
            LegendMaxValue.Text = "Max:  " & MaxRasterValue
        Else
            MinRasterValue = Trim(LegendMinValue.Text.Split(":")(1))
            MaxRasterValue = Trim(LegendMaxValue.Text.Split(":")(1))
        End If

        Using Writer As New IO.StreamWriter(MapServerMapFilePath)
            Writer.WriteLine("MAP")
            Writer.WriteLine("  STATUS ON")
            Writer.WriteLine("  IMAGETYPE ""png24""")
            Writer.WriteLine("  RESOLUTION 96")
            Writer.WriteLine("  DEFRESOLUTION 96")
            Writer.WriteLine(String.Format("  SIZE {0} {1}", Map.Width, Map.Height))

            Using Raster As New Raster(MaskRasterPath)
                Writer.WriteLine(String.Format("  EXTENT {0} {1} {2} {3}", Raster.Extent.Xmin, Raster.Extent.Ymin, Raster.Extent.Xmax, Raster.Extent.Ymax))
                Writer.Write("  UNITS " & GetUnits(Raster.Projection))

                Writer.WriteLine(Environment.NewLine & "  PROJECTION")
                For Each ProjectionPart In GetProj4Array(Raster.Projection)
                    Writer.WriteLine(String.Format("    ""{0}""", ProjectionPart))
                Next
                Writer.WriteLine("  END #PROJECTION")

                For Each OutputFormatName In {"png24", "print"}
                    Writer.WriteLine(Environment.NewLine & "  OUTPUTFORMAT")
                    Writer.WriteLine(String.Format("    NAME ""{0}""", OutputFormatName))
                    Writer.WriteLine("    MIMETYPE ""image/png""")
                    Writer.WriteLine("    DRIVER ""AGG/PNG""")
                    Writer.WriteLine("    EXTENSION ""png""")
                    Writer.WriteLine("    IMAGEMODE RGB")
                    Writer.WriteLine("    TRANSPARENT FALSE")
                    Writer.WriteLine("  END #OUTPUTFORMAT")
                Next
            End Using

            Writer.WriteLine(Environment.NewLine & "  LAYER")
            Writer.WriteLine(String.Format("    DATA ""{0}""", MapServerRasterPath.Replace("\", "\\")))
            Writer.WriteLine(String.Format("    NAME ""{0}""", IO.Path.GetFileNameWithoutExtension(MapServerRasterPath)))
            Writer.WriteLine("    STATUS ON")
            Writer.WriteLine("    TYPE RASTER")
            Writer.WriteLine("    TILEITEM ""location""")

            Writer.WriteLine(String.Format("    EXTENT {0} {1} {2} {3}", MapServerRaster.Extent.Xmin, MapServerRaster.Extent.Ymin, MapServerRaster.Extent.Xmax, MapServerRaster.Extent.Ymax))
            Writer.WriteLine("    UNITS " & GetUnits(MapServerRaster.Projection))

            Writer.WriteLine(Environment.NewLine & "    PROJECTION")
            For Each ProjectionPart In GetProj4Array(MapServerRaster.Projection)
                Writer.WriteLine(String.Format("      ""{0}""", ProjectionPart))
            Next
            Writer.WriteLine("    END #PROJECTION" & Environment.NewLine)

            For Each Line In GetColorGradient(MinRasterValue, MaxRasterValue, ColorPalette)
                Writer.WriteLine("    " & Line)
            Next

            Writer.WriteLine("  END #LAYER")

            Writer.WriteLine(Environment.NewLine & "END #MAP")
        End Using

        LoadMapFile()
    End Sub

    Private Sub LoadMapFile()
        MapObject = New MapServer.mapObj(MapServerMapFilePath)
        MapObject.width = Map.Width
        MapObject.height = Map.Height

        UpdateMap()

        FullExtent = New MapServer.rectObj(MapObject.extent.minx, MapObject.extent.miny, MapObject.extent.maxx, MapObject.extent.maxy, 0)
    End Sub

    Public Sub UpdateMap()
        Using Stream As New IO.MemoryStream(MapObject.draw().getBytes())
            Map.Image = System.Drawing.Image.FromStream(Stream)
        End Using

        UpdateLegend()
        UpdateMapPoint()
    End Sub

    Private Sub UpdateLegend()
        Dim ColorLength As Integer = ColorPalette.Length - 1
        Dim LegendHeight As Integer = Math.Ceiling(1260 / ColorLength) * ColorLength
        Dim LegendWidth As Integer = LegendHeight / 3
        Dim BarHeight As Single = LegendHeight / ColorLength

        Dim Bitmap As New Bitmap(LegendWidth, LegendHeight)
        Using Graphic = Graphics.FromImage(Bitmap)
            For I = ColorLength To 1 Step -1
                Using GradientBrush As Brush = New Drawing2D.LinearGradientBrush(New Point(0, 0), New Point(0, BarHeight), ColorPalette(I), ColorPalette(I - 1))
                    Graphic.FillRectangle(GradientBrush, 0, LegendHeight - BarHeight * I, LegendWidth, BarHeight + 1)
                End Using
            Next
        End Using

        Dim LegendBitmap As New Bitmap(LegendImage.Width, LegendImage.Height)
        Using Graphic = Graphics.FromImage(LegendBitmap)
            Graphic.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBilinear
            Graphic.DrawImage(Bitmap, New Rectangle(Point.Empty, LegendBitmap.Size))
        End Using

        LegendImage.Image = LegendBitmap
    End Sub

    Private Sub UpdateMapPoint()
        If MapObject IsNot Nothing Then
            Dim XCoordinate = MapObject.extent.minx + (MapObject.extent.maxx - MapObject.extent.minx) * ((MapPoint.X) / Map.Width)
            Dim YCoordinate = MapObject.extent.maxy + (MapObject.extent.miny - MapObject.extent.maxy) * ((MapPoint.Y) / Map.Height)
          
            StatusText.Text = String.Format(StatusString, FormatNumber(XCoordinate, , , , TriState.True), FormatNumber(YCoordinate, , , , TriState.True), FormatNumber(MapObject.scaledenom, , , , TriState.True))

            Dim Values() As Single = {Single.NaN, Single.NaN, Single.NaN, Single.NaN}
            If XCoordinate > MapServerRaster.Extent.Xmin + MapServerRaster.XResolution And XCoordinate < MapServerRaster.Extent.Xmax - MapServerRaster.XResolution And YCoordinate > MapServerRaster.Extent.Ymin + MapServerRaster.YResolution And YCoordinate < MapServerRaster.Extent.Ymax - MapServerRaster.YResolution Then
                Dim Point = MapServerRaster.CoordinateToPixelLocation(New Point64(XCoordinate, YCoordinate))

                MapServerRaster.Dataset.ReadRaster(Math.Floor(Point.X), Math.Floor(Point.Y), 2, 2, Values, 2, 2, 1, {1}, Nothing, Nothing, Nothing)

                Dim ValuesText(3) As String
                For I = 0 To 3
                    If Values(I) = MapServerRaster.BandNoDataValue(0) Then
                        ValuesText(I) = ""
                    Else
                        ValuesText(I) = Values(I)
                    End If
                Next

                TopLeft.Text = ValuesText(0)
                TopRight.Text = ValuesText(1)
                BottomLeft.Text = ValuesText(2)
                BottomRight.Text = ValuesText(3)
            Else
                TopLeft.Text = ""
                TopRight.Text = ""
                BottomLeft.Text = ""
                BottomRight.Text = ""
            End If
        End If
    End Sub

    Private Function GetUnits(Projection As String) As String
        Using SpatialReferenceSystem = New OSR.SpatialReference(Projection)
            Dim Unit = SpatialReferenceSystem.GetLinearUnitsName.ToUpper
            Select Case True
                Case Unit.Contains("DEGREE") : Return "DD"
                Case Unit.Contains("FEET"), Unit.Contains("INCH"), Unit.Contains("FOOT") : Return "FEET"
                Case Unit.Contains("MILE") : Return "MILES"
                Case Unit.Contains("KM"), Unit.Contains("KILOMETER") : Return "KILOMETERS"
                Case Else : Return "METERS"
            End Select
        End Using
    End Function

    Private Function GetProj4Array(Projection) As String()
        Using SpatialReferenceSystem = New OSR.SpatialReference(Projection)
            Dim Proj4String As String = ""
            SpatialReferenceSystem.ExportToProj4(Proj4String)
            Return Proj4String.Replace(" ", "").Substring(1).Split("+")
        End Using
    End Function

    Private Function GetColorGradient(MinValue As Double, MaxValue As Double, Colors() As Color) As String()
        Dim Output As New List(Of String)

        Dim Levels = Colors.Length - 2
        Dim Interval = (MaxValue - MinValue) / (Levels + 1)
        For I = 0 To Levels
            Dim Min = MinValue + I * Interval
            Dim Max = Min + Interval
            Dim Color1 = Colors(I)
            Dim Color2 = Colors(I + 1)

            Output.Add("CLASS")
            Output.Add(String.Format("  EXPRESSION ([pixel] >= {0} AND [pixel] < {1})", Min, Max))
            Output.Add("  STYLE")
            Output.Add(String.Format("    COLORRANGE {0} {1} {2} {3} {4} {5}", Color1.R, Color1.G, Color1.B, Color2.R, Color2.G, Color2.B))
            Output.Add(String.Format("    DATARANGE {0} {1}", Min, Max))
            Output.Add("    RANGEITEM ""pixel""")
            Output.Add("  END #STYLE")
            Output.Add("END #Class")
        Next

        Return Output.ToArray()
    End Function

    Private Class LegendPalette
        Public Shared Name As String() = {"Diametric", _
                                          "Dualistic", _
                                          "Horizon", _
                                          "Monochrome", _
                                          "Oasis", _
                                          "Polar", _
                                          "Temperature" _
                                         }

        Public Shared Value()() As Color = {New Color() {Color.FromArgb(245, 0, 0), Color.FromArgb(245, 245, 0), Color.FromArgb(0, 245, 0)}, _
                                            New Color() {Color.FromArgb(255, 0, 0), Color.FromArgb(255, 255, 0), Color.FromArgb(0, 255, 255), Color.FromArgb(0, 0, 255)}, _
                                            New Color() {Color.FromArgb(255, 255, 128), Color.FromArgb(242, 167, 46), Color.FromArgb(107, 0, 0)}, _
                                            New Color() {Color.FromArgb(0, 0, 0), Color.FromArgb(255, 255, 255)}, _
                                            New Color() {Color.FromArgb(194, 82, 60), Color.FromArgb(237, 161, 19), Color.FromArgb(255, 255, 0), Color.FromArgb(0, 219, 0), Color.FromArgb(32, 153, 143), Color.FromArgb(11, 44, 122)}, _
                                            New Color() {Color.FromArgb(69, 117, 181), Color.FromArgb(255, 255, 191), Color.FromArgb(214, 47, 39)}, _
                                            New Color() {Color.FromArgb(255, 0, 255), Color.FromArgb(0, 0, 255), Color.FromArgb(0, 255, 255), Color.FromArgb(0, 255, 0), Color.FromArgb(255, 255, 0), Color.FromArgb(255, 128, 0), Color.FromArgb(128, 0, 0)} _
                                           }
    End Class

#End Region

End Class
