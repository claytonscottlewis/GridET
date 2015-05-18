Public Class Extract_by_Polygon

#Region "Input Polygon Dataset"

    Private FieldNames As New List(Of String)
    Private FieldUniqueValueCount As New List(Of Long)
    Private MaximumUniqueValueCount As Integer = 1000

    Private Sub InputPolygonAdd_Click(sender As System.Object, e As System.EventArgs) Handles InputPolygonAdd.Click
        Dim OpenFileDialog As New OpenFileDialog
        OpenFileDialog.Title = "Location of regional vector polygon file."
        OpenFileDialog.Multiselect = False
        OpenFileDialog.RestoreDirectory = True

        If Not OpenFileDialog.ShowDialog = Windows.Forms.DialogResult.OK Then Exit Sub

        Using Datasource = OGR.Ogr.OpenShared(OpenFileDialog.FileName, False)
            If Datasource Is Nothing Then
                MsgBox(String.Format("'{0}' not recognized as a supported file format.", OpenFileDialog.FileName))
                InputPolygonPath.Text = ""
            Else
                TableName = Datasource.GetLayerByIndex(0).GetName
                InputPolygonPath.Text = OpenFileDialog.FileName
            End If
        End Using
    End Sub

    Private Sub InputPolygonPath_TextChanged(sender As Object, e As System.EventArgs) Handles InputPolygonPath.TextChanged
        IntermediatePolygonPath = IO.Path.Combine(IO.Path.GetTempPath, String.Format("Extract by Polygon Intermediate Dataset - {0}.db", TableName))

        If Not InputPolygonPath.Text = "" Then
            If Not BackgroundWorker2.IsBusy Then
                BackgroundWorker2.WorkerReportsProgress = True

                ProgressText.Text = "Please wait while the input dataset is being evaluated..."
                ProgressBar.Minimum = 0
                ProgressBar.Maximum = 2
                ProgressBar.Value = 0
                ProgressText.Visible = True
                ProgressBar.Visible = True

                InputPolygonGroup.Enabled = False
                PolygonRasterRelationGroup.Enabled = False
                OutputPolygonGroup.Enabled = False
                CalculateButton.Enabled = False
                Cancel_Button.Enabled = False

                BackgroundWorker2.RunWorkerAsync()
            End If
        Else
            RelationGrid.Rows.Clear()
        End If
    End Sub

    Sub CreateIntermediateDataset(InputPath As String, IntermediatePath As String, DatasetName As String, ByRef Names As List(Of String), ByRef UniqueCount As List(Of Long), BackgroundWorker As System.ComponentModel.BackgroundWorker, DoWorkEvent As System.ComponentModel.DoWorkEventArgs)
        Try
            'Create Intermediate Dataset
            Dim SpatialReferenceSystem As OSR.SpatialReference
            Using Raster As New Raster(MaskRasterPath)
                SpatialReferenceSystem = New OSR.SpatialReference(Raster.Projection)
            End Using

            Dim Process As New GDALProcess
            Process.Ogr2Ogr(InputPolygonPath.Text, IntermediatePolygonPath, GDALProcess.VectorFormat.SQLite, , SpatialReferenceSystem, True)

            BackgroundWorker.ReportProgress(0)

            'Load Column Unique Value Counts
            Using Connection = CreateConnection(IntermediatePolygonPath)
                Connection.Open()

                Using Command = Connection.CreateCommand
                    Names = New List(Of String)
                    Command.CommandText = String.Format("PRAGMA table_info(""{0}"")", DatasetName)
                    Using Reader = Command.ExecuteReader
                        Do While Reader.Read
                            Names.Add(Reader(1))
                        Loop
                    End Using

                    UniqueCount = New List(Of Long)
                    UniqueCount.Add(-1)
                    UniqueCount.Add(-1)
                    Dim CommandText As New System.Text.StringBuilder("SELECT")
                    For I = 2 To Names.Count - 1
                        If I <> 2 Then CommandText.Append(",")
                        CommandText.Append(String.Format(" COUNT(DISTINCT ""{0}"")", Names(I)))
                    Next
                    CommandText.Append(String.Format(" FROM ""{0}""", DatasetName))

                    Command.CommandText = CommandText.ToString
                    Dim List As New List(Of Long)
                    Using Reader = Command.ExecuteReader
                        Reader.Read()

                        For I = 0 To Reader.FieldCount - 1
                            UniqueCount.Add(Reader.GetInt64(I))
                        Next
                    End Using
                End Using
            End Using
        Catch Exception As Exception
            MsgBox(Exception.ToString)
        End Try
    End Sub

#End Region

#Region "Polygon-Raster Relation"

    Private Sub Calculate_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        For Each Control In Me.Controls
            GetType(Control).InvokeMember("DoubleBuffered", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance Or Reflection.BindingFlags.SetProperty, Nothing, Control, New Object() {True})
        Next

        If IO.File.Exists(ProjectDetailsPath) Then
            CalculationPeriod.Items.Clear()
            For Each Directory In IO.Directory.GetDirectories(OutputCalculationsDirectory)
                CalculationPeriod.Items.Add(IO.Path.GetFileName(Directory))
            Next
        Else
            InputPolygonGroup.Enabled = False
            PolygonRasterRelationGroup.Enabled = False
            OutputPolygonGroup.Enabled = False
            CalculateButton.Enabled = False
            CalculateButton.Enabled = False
        End If
    End Sub

    Private Sub Calculate_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = BackgroundWorker.IsBusy Or BackgroundWorker2.IsBusy
        If Not e.Cancel Then If IO.File.Exists(IntermediatePolygonPath) Then IO.File.Delete(IntermediatePolygonPath)
    End Sub

    Private Sub Relate_TextChanged(sender As Object, e As System.EventArgs) Handles CoverRelateField.TextChanged, CalculationPeriod.TextChanged
        If CoverRelateField.Text <> "" And CalculationPeriod.Text <> "" Then
            RelationGrid.Rows.Clear()

            Dim CalculationVariables As New List(Of String)
            CalculationVariables.Add("")
            For Each Path In IO.Directory.GetFiles(IO.Path.Combine(OutputCalculationsDirectory, CalculationPeriod.Text), "*.tif")
                CalculationPaths.Add(Path)
                CalculationVariables.Add(IO.Path.GetFileNameWithoutExtension(Path))
            Next

            Dim CalculationVariablesColumn = CType(RelationGrid.Columns(1), DataGridViewComboBoxColumn)
            CalculationVariablesColumn.Items.Clear()
            CalculationVariablesColumn.Items.AddRange(CalculationVariables.ToArray)

            Dim CalculationVariablesArray(CalculationVariables.Count - 1) As String
            For I = 1 To CalculationVariables.Count - 1
                Dim Value = CalculationVariables(I).Substring(0, CalculationVariables(I).LastIndexOf("(") - 1).Replace(" Net Potential Evapotranspiration", "").Replace(" Potential Evapotranspiration", "")
                If Value.EndsWith("NLDAS_2A") Or Value.EndsWith("DAYMET") Then
                    Value = Value.Substring(0, Value.LastIndexOf(" "))
                End If
                CalculationVariablesArray(I) = Value
            Next
            Dim CalculationVariablesArrayList = CalculationVariablesArray.ToList
            CalculationVariablesArrayList(0) = ""

            Using Connection = CreateConnection(IntermediatePolygonPath)
                Connection.Open()

                Using Command = Connection.CreateCommand
                    Command.CommandText = String.Format("SELECT DISTINCT ""{0}"" FROM ""{1}"" ORDER BY ""{0}""", CoverRelateField.Text, TableName)

                    Using Reader = Command.ExecuteReader
                        While Reader.Read
                            Dim Value As String = ""
                            If Not Reader.IsDBNull(0) Then Value = Reader(0).ToString

                            Dim CalculationVariable = MatchClosestText(GetAlternateDescriptions(Value), CalculationVariablesArray)
                            If CalculationVariable <> "" Then
                                CalculationVariable = CalculationVariables.Where(Function(V As String) V.Contains(CalculationVariable.Replace("Dry ", "")))(0)
                            End If
                            Dim ContainsValue = CalculationVariablesArrayList.Where(Function(V) V.Contains(Value)).ToArray
                            If ContainsValue.Count > 0 Then CalculationVariable = CalculationVariables(CalculationVariablesArrayList.IndexOf(ContainsValue(0)))

                            RelationGrid.Rows.Add({Value, CalculationVariable})
                        End While
                    End Using

                End Using

            End Using

            For I = 0 To RelationGrid.RowCount - 1
                RelationGrid.Rows(I).HeaderCell.Value = CStr(I + 1)
            Next
        Else
            RelationGrid.Rows.Clear()
        End If
    End Sub

    Private Function GetAlternateDescriptions(Description As String) As String()
        Dim Result As New List(Of String)
        Result.Add(Description)

        If Description.StartsWith("Dry ") Or Description.Contains("Fallow") Or Description.Contains("Idle") Or Description.Contains("No Land Use") Then
            Result(0) = ""
        ElseIf Description.Contains("Bean") Or Description.Contains("Vegetable") Or Description.Contains("Tomato") Then
            Result.Add("Garden")
        ElseIf Description.Contains("GrassHay") Then
            Result.Add("Other Hay")
        ElseIf Description.Contains("Reservoir") Then
            Result.Add("Open Water Deep")
        ElseIf Description.Contains("Pond") Or Description.Contains("Lagoon") Or Description.Contains("Stream") Then
            Result.Add("Open Water Shallow")
        ElseIf Description.Contains("Berries") Or Description.Contains("Vineyard") Then
            Result.Add("Small Fruit")
        ElseIf Description.Contains("Oats") Then
            Result.Add("Spring Grain")
        ElseIf Description.Contains("Horticulture") Or Description.Contains("Urban") Then
            Result.Add("Turfgrass")
        ElseIf Description.Contains("Riparian") Then
            Result.Add("Wetlands-Narrow")
        End If

        Return Result.ToArray
    End Function

    Public Function MatchClosestText(Text() As String, Comparison() As String) As String
        If Text.Length = 0 Or Comparison.Length = 0 Then
            Return Nothing
        Else
            Dim ClosestComparison As String = ""
            Dim ShortestDistance As Long = Long.MaxValue

            For I = 0 To Text.Length - 1
                If Not Text(I) = "" Then
                    For J = 0 To Comparison.Length - 1
                        If Not Comparison(J) = "" Then
                            Dim Distance = CalculateLevenshteinDistance(Text(I), Comparison(J))
                            If Distance < ShortestDistance Then
                                ClosestComparison = Comparison(J)
                                ShortestDistance = Distance
                            End If
                        End If
                    Next
                End If
            Next

            Return ClosestComparison
        End If
    End Function

    Public Function CalculateLevenshteinDistance(ByRef String1 As String, ByVal String2 As String) As Long
        Dim Length1 As Long = String1.Length - 1
        Dim Length2 As Long = String2.Length - 1

        Dim Distance(Length1, Length2) As Long
        For I = 0 To Length1
            Distance(I, 0) = I
        Next
        For J = 0 To Length2
            Distance(0, J) = J
        Next

        For J = 1 To Length2
            For I = 1 To Length1
                Dim Cost As Long = Math.Abs(StrComp(Mid$(String1, I, 1), Mid$(String2, J, 1), CompareMethod.Text))
                Dim CostInsertion As Long = Distance(I - 1, J) + 1
                Dim CostDeletion As Long = Distance(I, J - 1) + 1
                Dim CostSubstitution As Long = Distance(I - 1, J - 1) + Cost

                Distance(I, J) = CostInsertion
                If CostDeletion < Distance(I, J) Then Distance(I, J) = CostDeletion
                If CostSubstitution < Distance(I, J) Then Distance(I, J) = CostSubstitution
            Next I
        Next J

        Return Distance(Length1, Length2)
    End Function

#End Region

#Region "Output Polygon Dataset"

    Private Sub OutputPolygonSet_Click(sender As System.Object, e As System.EventArgs) Handles OutputPolygonSet.Click
        Dim SaveFileDialog As New SaveFileDialog
        SaveFileDialog.Title = "Location of output vector polygon file."

        If Not SaveFileDialog.ShowDialog = Windows.Forms.DialogResult.OK Then Exit Sub

        OutputPolygonPath.Text = SaveFileDialog.FileName
        OutputPath = SaveFileDialog.FileName
    End Sub

    Private Sub CalculateButton_Click(sender As System.Object, e As System.EventArgs) Handles CalculateButton.Click
        If Cancel_Button.Enabled = False Then
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()
            Exit Sub
        End If

        If OutputPolygonPath.Text = "" Then
            MsgBox("Please select an output dataset path.")
            Exit Sub
        End If

        If Not BackgroundWorker.IsBusy And RelationGrid.RowCount > 0 Then
            RelateField = CoverRelateField.Text

            ReDim CalculationVariableRelates(CalculationPaths.Count - 1)
            For I = 0 To CalculationVariableRelates.Length - 1
                CalculationVariableRelates(I) = New List(Of String)
            Next

            Dim CalculationVariablesColumn = CType(RelationGrid.Columns(1), DataGridViewComboBoxColumn)
            For I = 0 To RelationGrid.RowCount - 1
                If RelationGrid(1, I).Value <> "" Then
                    CalculationVariableRelates(CalculationVariablesColumn.Items.IndexOf(RelationGrid(1, I).Value) - 1).Add(RelationGrid(0, I).Value)
                End If
            Next

            Dim CalculationCount As Integer = 0
            For I = 0 To CalculationVariableRelates.Length - 1
                If CalculationVariableRelates(I).Count > 0 Then CalculationCount += 1
            Next

            BackgroundWorker.WorkerReportsProgress = True
            BackgroundWorker.WorkerSupportsCancellation = True

            InputPolygonGroup.Enabled = False
            PolygonRasterRelationGroup.Enabled = False
            OutputPolygonGroup.Enabled = False
            CalculateButton.Enabled = False

            ProgressText.Text = "Initializing calculation datasets..."
            ProgressBar.Minimum = 0
            ProgressBar.Maximum = CalculationCount + 1
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
                Cancel_Button.Enabled = False
            End If
        Else
            Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
            RemoveHandler Me.FormClosing, AddressOf Calculate_FormClosing
            Me.Close()
        End If
    End Sub

#End Region

#Region "Background Execution"

    WithEvents BackgroundWorker As New System.ComponentModel.BackgroundWorker
    WithEvents BackgroundWorker2 As New System.ComponentModel.BackgroundWorker
    Private Timer As Stopwatch
    Private IntermediatePolygonPath As String = ""
    Private TableName As String = ""
    Private RelateField As String = ""
    Private CalculationPaths As New List(Of String)
    Private CalculationVariableRelates() As List(Of String)
    Private OutputPath As String = ""

    Private Sub BackgroundWorker_DoWork(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker.DoWork
        CalculateAverageRasterValueInPolygon(CalculationPaths.ToArray, CalculationVariableRelates, IntermediatePolygonPath, TableName, RelateField, OutputPath, GDALProcess.VectorFormat.ESRI_Shapefile, BackgroundWorker, e)

        ''Open Intermediate Vector Database and Add Monthly Calculation Columns
        'Dim CalculationColumns(MonthAndAnnualNames.Length) As String
        'For I = 0 To CalculationColumns.Length - 2
        '    CalculationColumns(I) = "___" & MonthAndAnnualNames(I) & "___"
        'Next
        'CalculationColumns(MonthAndAnnualNames.Length) = "___Count___"

        'Dim MaskRaster As New Raster(MaskRasterPath)
        'Dim SpatialReferenceSystem = New OSR.SpatialReference(MaskRaster.Projection)

        'Using ConnectionWrite = CreateConnection(IntermediatePolygonPath, False)
        '    ConnectionWrite.Open()

        '    Using CommandWrite = ConnectionWrite.CreateCommand
        '        Using Transaction = ConnectionWrite.BeginTransaction
        '            For I = 0 To CalculationColumns.Length - 1
        '                CommandWrite.CommandText = String.Format("ALTER TABLE ""{0}"" ADD COLUMN ""{1}"" REAL DEFAULT '0'", TableName, CalculationColumns(I))
        '                CommandWrite.ExecuteNonQuery()
        '            Next

        '            Transaction.Commit()
        '        End Using

        '        Dim CalculationColumnString = "(""" & String.Join(""",""", CalculationColumns) & ")"""
        '        Dim CalculationBands() = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13}

        '        'For Each Calculation
        '        For I = 0 To CalculationPaths.Count - 1
        '            If CalculationVariableRelates(I).Count > 0 Then
        '                'Rasterize Polygon FID's for Related Variables to Mask Raster Template
        '                Dim RasterizedFIDPath = IO.Path.Combine(IO.Path.GetTempPath, String.Format("Extract by Polygon {0} Rasterized FID {1}.tif", TableName, I))

        '                Dim WhereExpression As New System.Text.StringBuilder()
        '                For J = 0 To CalculationVariableRelates(I).Count - 1
        '                    If J > 0 Then WhereExpression.Append(" OR")
        '                    WhereExpression.Append(String.Format(" ""{0}"" = '{1}'", RelateField, CalculationVariableRelates(I)(J)))
        '                Next

        '                Dim Process As New GDALProcess
        '                Process.Rasterize(IntermediatePolygonPath, RasterizedFIDPath, TableName, "FID", MaskRaster.Extent, MaskRaster.XCount, MaskRaster.YCount, WhereExpression.ToString, , , Integer.MinValue)

        '                'Open Rasterized FID and Calculation Rasters
        '                Dim RasterizedFIDRaster As New Raster(RasterizedFIDPath)
        '                RasterizedFIDRaster.Open(GDAL.Access.GA_ReadOnly)
        '                Dim CalculationRaster As New Raster(CalculationPaths(I))
        '                CalculationRaster.Open(GDAL.Access.GA_ReadOnly)

        '                'Add Rasterized Pixel Values to Polygon Database
        '                Using Transaction = ConnectionWrite.BeginTransaction
        '                    Do Until RasterizedFIDRaster.BlocksProcessed
        '                        Dim RasterizedFIDPixels = RasterizedFIDRaster.Read({1})
        '                        Dim CalculationPixels = CalculationRaster.Read(CalculationBands)

        '                        Dim NoDataValue = RasterizedFIDRaster.BandNoDataValue(0)
        '                        Dim BandOffset = CalculationRaster.BlockYSize * CalculationRaster.XCount

        '                        For J = 0 To RasterizedFIDPixels.Length - 1
        '                            If RasterizedFIDPixels(J) <> NoDataValue Then
        '                                Dim CommandText As New System.Text.StringBuilder(String.Format("UPDATE ""{0}"" SET", TableName))
        '                                For K = 0 To CalculationBands.Length - 1
        '                                    CommandText.Append(String.Format(" ""{0}"" = ""{0}"" + '{1}',", CalculationColumns(K), CalculationPixels(BandOffset * K + J)))
        '                                Next
        '                                CommandText.Append(String.Format(" ""{0}"" = ""{0}"" + '1' WHERE ""ogc_fid"" = '{1}'", CalculationColumns(MonthAndAnnualNames.Length), RasterizedFIDPixels(J)))

        '                                CommandWrite.CommandText = CommandText.ToString
        '                                CommandWrite.ExecuteNonQuery()
        '                            End If
        '                        Next

        '                        If BackgroundWorker.CancellationPending Then : e.Cancel = True : Exit Sub : End If
        '                    Loop

        '                    Transaction.Commit()
        '                End Using

        '                'Add Interpolated Pixel Values for Polygons not Containing a Raster Pixel
        '                Using Transaction = ConnectionWrite.BeginTransaction
        '                    Using ConnectionRead = CreateConnection(IntermediatePolygonPath)
        '                        ConnectionRead.Open()

        '                        Using CommandRead = ConnectionRead.CreateCommand
        '                            CommandRead.CommandText = String.Format("SELECT ""ogc_fid"", ""GEOMETRY"" FROM ""{0}"" WHERE {1} AND ""{2}"" = '0'", TableName, WhereExpression.ToString, CalculationColumns(MonthAndAnnualNames.Length))

        '                            Using Reader = CommandRead.ExecuteReader
        '                                While Reader.Read
        '                                    Dim FID = Reader.GetInt64(0)

        '                                    Using Geometry = OGR.Geometry.CreateFromWkb(Reader.GetValue(1)).Centroid
        '                                        Dim CentroidLocation = MaskRaster.CoordinateToPixelLocation(New Point64(Geometry.GetX(0), Geometry.GetY(0)))
        '                                        Dim X = Math.Floor(CentroidLocation.X)
        '                                        Dim Y = Math.Floor(CentroidLocation.Y)

        '                                        Dim Values(4 * CalculationRaster.BandCount - 1) As Single
        '                                        CalculationRaster.Dataset.ReadRaster(X, Y, 2, 2, Values, 2, 2, CalculationBands.Length, CalculationBands, Nothing, Nothing, Nothing)

        '                                        Dim CommandText As New System.Text.StringBuilder(String.Format("UPDATE ""{0}"" SET", TableName))
        '                                        For J = 0 To CalculationBands.Length - 1
        '                                            Dim Offset As Integer = 4 * J
        '                                            Dim Value = BilinearInterpolation(New QuadValues(Values(Offset), Values(Offset + 1), Values(Offset + 2), Values(Offset + 3)), CentroidLocation.X - X, CentroidLocation.Y - Y)
        '                                            CommandText.Append(String.Format(" ""{0}"" = ""{0}"" + '{1}',", CalculationColumns(J), Value))
        '                                        Next
        '                                        CommandText.Append(String.Format(" ""{0}"" = ""{0}"" + '1' WHERE ""ogc_fid"" = '{1}'", CalculationColumns(MonthAndAnnualNames.Length), FID))

        '                                        CommandWrite.CommandText = CommandText.ToString
        '                                        CommandWrite.ExecuteNonQuery()
        '                                    End Using

        '                                End While
        '                            End Using
        '                        End Using
        '                    End Using

        '                    Transaction.Commit()
        '                End Using

        '                'Release Memory
        '                CalculationRaster.Close()
        '                RasterizedFIDRaster.Close()
        '                MaskRaster.Close()

        '                IO.File.Delete(RasterizedFIDPath)

        '                If BackgroundWorker.CancellationPending Then : e.Cancel = True : Exit Sub : End If
        '                BackgroundWorker.ReportProgress(0)
        '            End If
        '        Next

        '        'Compute Polygon Average
        '        Dim ComputeCommandText As New System.Text.StringBuilder(String.Format("UPDATE ""{0}"" SET", TableName))
        '        For J = 0 To CalculationColumns.Length - 2
        '            If J > 0 Then ComputeCommandText.Append(",")
        '            ComputeCommandText.Append(String.Format(" ""{0}"" = ""{0}"" / ""{1}""", CalculationColumns(J), CalculationColumns(MonthAndAnnualNames.Length)))
        '        Next
        '        ComputeCommandText.Append(String.Format(" WHERE ""{0}"" > '0'", CalculationColumns(MonthAndAnnualNames.Length)))

        '        CommandWrite.CommandText = ComputeCommandText.ToString
        '        CommandWrite.ExecuteNonQuery()

        '        If BackgroundWorker.CancellationPending Then : e.Cancel = True : Exit Sub : End If
        '    End Using
        'End Using

        ''Extract Calculation Columns to Output Vector Dataset
        'Dim ExtractCommandText As New System.Text.StringBuilder("SELECT ""ogc_fid"" AS ""FID"", ""GEOMETRY"",")
        'For I = 0 To CalculationColumns.Length - 1
        '    If I > 0 Then ExtractCommandText.Append(",")
        '    ExtractCommandText.Append(String.Format(" ""{0}"" AS ""{1}""", CalculationColumns(I), CalculationColumns(I).Replace("_", "")))
        'Next
        'ExtractCommandText.Append(String.Format(" FROM ""{0}""", TableName))

        'Dim ExtractProcess As New GDALProcess
        'ExtractProcess.Ogr2Ogr(IntermediatePolygonPath, OutputPath, GDALProcess.VectorFormat.ESRI_Shapefile, SpatialReferenceSystem, , True, ExtractCommandText.ToString)
    End Sub

    Private Sub BackgroundWorker_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorker.ProgressChanged
        If ProgressBar.Maximum - ProgressBar.Value > 1 Then
            Dim Timespan As TimeSpan = New TimeSpan(Timer.Elapsed.Ticks / (ProgressBar.Value + 1) * (ProgressBar.Maximum - ProgressBar.Value - 1))
            ProgressText.Text = String.Format("Estimated time remaining...({0})", Timespan.ToString("d\.hh\:mm\:ss"))
        End If

        If ProgressBar.Value < ProgressBar.Maximum Then ProgressBar.Value += 1
    End Sub

    Private Sub BackgroundWorker_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker.RunWorkerCompleted
        If e.Cancelled Then
            ProgressText.Text = "Polygon raster average extraction calculations cancelled"
        ElseIf e.Error IsNot Nothing Then
            ProgressText.Text = "There was an error in the calculation process"
        Else
            Timer.Stop()
            ProgressText.Text = "Polygon raster average extraction calculations were successfully completed"
        End If
        ProgressText.Text &= String.Format(" ({0}).", Timer.Elapsed.ToString())

        CalculateButton.Enabled = True
        CalculateButton.Text = "OK"
        Cancel_Button.Enabled = False
    End Sub

    Private Sub BackgroundWorker2_DoWork(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker2.DoWork
        CreateIntermediateDataset(InputPolygonPath.Text, IntermediatePolygonPath, TableName, FieldNames, FieldUniqueValueCount, BackgroundWorker2, e)
    End Sub

    Private Sub BackgroundWorker2_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorker2.ProgressChanged
        ProgressBar.Value += 1

        If ProgressBar.Value = 1 Then
            ProgressText.Text = "...determining unique values per field..."
        Else
            ProgressText.Text = "Completed."
        End If
    End Sub

    Private Sub BackgroundWorker2_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker2.RunWorkerCompleted
        If e.Cancelled Then
        ElseIf e.Error IsNot Nothing Then
            ProgressText.Text = "There was an error in the calculation process"
            InputPolygonPath.Text = ""
        Else
            CoverRelateField.Items.Clear()

            For I = 2 To FieldNames.Count - 1
                If FieldUniqueValueCount(I) <= MaximumUniqueValueCount Then
                    CoverRelateField.Items.Add(FieldNames(I))
                End If
            Next

            CoverRelateField.SelectedText = MatchClosestText({"description"}, CoverRelateField.Items.OfType(Of String)().ToArray)
        End If

        ProgressText.Visible = False
        ProgressBar.Visible = False

        InputPolygonGroup.Enabled = True
        PolygonRasterRelationGroup.Enabled = True
        OutputPolygonGroup.Enabled = True
        CalculateButton.Enabled = True
        Cancel_Button.Enabled = True
    End Sub

#End Region

End Class