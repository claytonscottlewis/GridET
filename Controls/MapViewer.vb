Public Class MapViewer

#Region "Variables"

    Private MapRasterPath As String
    Private MapObject As MapServer.mapObj
    Private FullExtent As MapServer.rectObj
    Private DragOperation As Boolean = False
    Private DragRectangle As Rectangle = New Rectangle(New Point(0, 0), New Size(0, 0))
    Private DragStartPoint As Point
    Private DragEndPoint As Point
    Private MapPoint As Point
    Private StatusString As String = "[ Scale = 1 : {0} ] [ X = {1} ] [ Y = {2} ]"

#End Region

#Region "Subroutines"

    Private Sub MapViewer_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        For Each Control In Me.Controls
            GetType(Control).InvokeMember("DoubleBuffered", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance Or Reflection.BindingFlags.SetProperty, Nothing, Control, New Object() {True})
        Next
    End Sub

    Public Sub CreateETMap(VectorPaths() As String, RasterPaths() As String)
        Using Raster As New Raster(RasterPaths(0))
            Raster.Open(GDAL.Access.GA_Update)

            Dim Data(Raster.XCount * Raster.YCount - 1) As Single
            Raster.Dataset.ReadRaster(0, 0, Raster.XCount, Raster.YCount, Data, Raster.XCount, Raster.YCount, 1, {1}, 0, 0, 0)

            For I = 0 To Data.Length - 1
                If Data(I) = 0 Then Data(I) = Single.MinValue
            Next

            Raster.Dataset.WriteRaster(0, 0, Raster.XCount, Raster.YCount, Data, Raster.XCount, Raster.YCount, 1, {1}, 0, 0, 0)
        End Using

        Dim MapRasterPath = IO.Path.GetTempFileName
        Using Writer As New IO.StreamWriter(MapRasterPath)
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

            For Each Path In RasterPaths
                Writer.WriteLine(Environment.NewLine & "  LAYER")
                Writer.WriteLine(String.Format("    DATA ""{0}""", Path.Replace("\", "\\")))
                Writer.WriteLine(String.Format("    NAME ""{0}""", IO.Path.GetFileNameWithoutExtension(Path)))
                Writer.WriteLine("    STATUS OFF")
                Writer.WriteLine("    TYPE RASTER")
                Writer.WriteLine("    TILEITEM ""location""")

                Using Raster As New Raster(Path)
                    Writer.WriteLine(String.Format("    EXTENT {0} {1} {2} {3}", Raster.Extent.Xmin, Raster.Extent.Ymin, Raster.Extent.Xmax, Raster.Extent.Ymax))
                    Writer.WriteLine("    UNITS " & GetUnits(Raster.Projection))

                    Writer.WriteLine(Environment.NewLine & "    PROJECTION")
                    For Each ProjectionPart In GetProj4Array(Raster.Projection)
                        Writer.WriteLine(String.Format("      ""{0}""", ProjectionPart))
                    Next
                    Writer.WriteLine("    END #PROJECTION" & Environment.NewLine)

                    Raster.Open(GDAL.Access.GA_ReadOnly)
                    Dim Min As Double = 0
                    Raster.Dataset.GetRasterBand(1).GetMinimum(Min, 0)
                    Dim Max As Double = 0
                    Raster.Dataset.GetRasterBand(1).GetMaximum(Max, 0)

                    For Each Line In GetColorGradient(Min, Max, {Color.FromArgb(255, 0, 255), Color.FromArgb(0, 0, 255), Color.FromArgb(0, 255, 255), Color.FromArgb(0, 255, 0), Color.FromArgb(255, 255, 0), Color.FromArgb(255, 128, 0), Color.FromArgb(128, 0, 0)})
                        Writer.WriteLine("    " & Line)
                    Next
                End Using

                Writer.WriteLine("  END #LAYER")
            Next

            Writer.WriteLine(Environment.NewLine & "END #MAP")
        End Using

        LoadMapFile(MapRasterPath)
    End Sub

    Public Sub LoadMapFile(Path As String)
        MapRasterPath = Path

        MapObject = New MapServer.mapObj(Path)
        MapObject.width = Map.Width
        MapObject.height = Map.Height

        RefreshMap()

        FullExtent = New MapServer.rectObj(MapObject.extent.minx, MapObject.extent.miny, MapObject.extent.maxx, MapObject.extent.maxy, 0)
    End Sub

    Private Sub Map_SizeChanged(sender As Object, e As System.EventArgs) Handles Map.SizeChanged
        If Not MapRasterPath = "" Then
            MapObject.width = Map.Width
            MapObject.height = Map.Height

            RefreshMap()
        End If
    End Sub

    Public Sub RefreshMap()
        Using Stream As New IO.MemoryStream(MapObject.draw().getBytes())
            Map.Image = System.Drawing.Image.FromStream(Stream)
        End Using

        RefreshMapLayers()
        RefreshLegend()
        UpdateStatusLabel()
    End Sub

    Private Sub RefreshMapLayers()
        ''--> initialize map layers with check boxes for on/off
        ''    note: list is created in reverse so top layer in map appears at top of list
        'trvLegend.Nodes.Clear()
        'For i As Integer = MapObject.numlayers - 1 To 0 Step -1
        '    Dim layer As MapServer.layerObj = MapObject.getLayer(i)
        '    Dim node As New TreeNode(MapObject.getLayer(i).name)

        '    If layer.status = CInt(MapServer.mapscript.MS_ON) Then
        '        node.Checked = True
        '    Else
        '        node.Checked = False
        '    End If

        '    trvLegend.Nodes.Add(node)
        'Next
    End Sub

    Private Sub RefreshLegend()
        Using Stream As New IO.MemoryStream(MapObject.drawLegend().getBytes())
            SplitContainer.Panel1.BackgroundImageLayout = ImageLayout.Center
            SplitContainer.Panel1.BackgroundImage = System.Drawing.Image.FromStream(Stream)
        End Using
    End Sub

    Private Sub UpdateStatusLabel()
        If Not MapObject Is Nothing Then StatusText.Text = String.Format(StatusString, FormatNumber(MapObject.scaledenom, , , , TriState.True), FormatNumber(MapObject.extent.minx + (MapObject.extent.maxx - MapObject.extent.minx) * (MapPoint.X / Map.Width), , , , TriState.True), FormatNumber(MapObject.extent.maxy + (MapObject.extent.miny - MapObject.extent.maxy) * (MapPoint.Y / Map.Height), , , , TriState.True))
    End Sub

    Private Sub Zoom(MapX As Double, MapY As Double, ZoomFactor As Integer)
        If Not MapObject Is Nothing Then
            MapObject.zoomPoint(ZoomFactor, New MapServer.pointObj(MapX, MapY, 0, 0), Map.Width, Map.Height, MapObject.extent, Nothing)
            RefreshMap()
        End If
    End Sub

    Private Sub ZoomFullExtent_Click(sender As Object, e As EventArgs) Handles ZoomFullExtent.Click
        If Not MapObject Is Nothing Then
            MapObject.extent = FullExtent
            RefreshMap()
        End If
    End Sub

    Private Sub ZoomInFixed_Click(sender As Object, e As EventArgs) Handles ZoomInFixed.Click
        If Not MapObject Is Nothing Then Zoom(Map.Width / 2, Map.Height / 2, 2)
    End Sub

    Private Sub ZoomOutFixed_Click(sender As Object, e As EventArgs) Handles ZoomOutFixed.Click
        If Not MapObject Is Nothing Then Zoom(Map.Width / 2, Map.Height / 2, -2)
    End Sub

    Private Sub Map_MouseWheel(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles Map.MouseWheel
        If Not MapObject Is Nothing Then Zoom(e.X, e.Y, e.Delta * 2)
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
        If Not MapObject Is Nothing Then
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
        UpdateStatusLabel()
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

                RefreshMap()

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

    Private Sub CheckedChanged(sender As Object, e As System.EventArgs) Handles Pan.CheckedChanged, ZoomInBox.CheckedChanged, ZoomOutBox.CheckedChanged
        RemoveHandler Pan.CheckedChanged, AddressOf CheckedChanged
        RemoveHandler ZoomInBox.CheckedChanged, AddressOf CheckedChanged
        RemoveHandler ZoomOutBox.CheckedChanged, AddressOf CheckedChanged

        For Each Button In {Pan, ZoomInBox, ZoomOutBox}
            If Button IsNot sender Then
                Button.Checked = False
            End If
        Next

        AddHandler Pan.CheckedChanged, AddressOf CheckedChanged
        AddHandler ZoomInBox.CheckedChanged, AddressOf CheckedChanged
        AddHandler ZoomOutBox.CheckedChanged, AddressOf CheckedChanged
    End Sub

    Private Sub Pan_Click(sender As Object, e As System.EventArgs) Handles Pan.Click
        If Pan.Checked Then
            Cursor = Cursors.SizeAll
        Else
            Cursor = Cursors.Arrow
        End If
    End Sub

    Private Function GetUnits(Projection As String) As String
        Using SpatialReferenceSystem = New OSR.SpatialReference(Projection)
            Dim Unit = SpatialReferenceSystem.GetLinearUnitsName.ToUpper
            Select Case True
                Case Unit.Contains("DEGREE") : Return "DD"
                Case Unit.Contains("FEET"), Unit.Contains("INCH") : Return "FEET"
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

    'Private Sub trvLegend_NodeMouseClick(sender As Object, e As TreeNodeMouseClickEventArgs) Handles trvLegend.NodeMouseClick
    '    '--> adjust layer visibility based on check box status
    '    For i As Integer = 0 To trvLegend.Nodes.Count - 1
    '        Dim layer As layerObj = m_map.getLayerByName(trvLegend.Nodes(i).Text)
    '        If trvLegend.Nodes(i).Checked Then
    '            layer.status = CInt(mapscript.MS_ON)
    '        Else
    '            layer.status = CInt(mapscript.MS_OFF)
    '        End If
    '    Next
    '    RefreshMap()
    'End Sub

#End Region

End Class
