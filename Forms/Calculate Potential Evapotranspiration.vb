Public Class Calculate_Potential_Evapotranspiration

#Region "Cover Selection"

    Private Sub CheckAll_Click(sender As System.Object, e As System.EventArgs) Handles CheckAll.Click
        RemoveHandler CoverList.ItemChecked, AddressOf CoverList_ItemChecked

        For Item = 0 To CoverList.Items.Count - 1
            CoverList.Items(Item).Checked = True
        Next

        AddHandler CoverList.ItemChecked, AddressOf CoverList_ItemChecked

        CoverList_ItemChecked(Nothing, Nothing)
    End Sub

    Private Sub UncheckAll_Click(sender As System.Object, e As System.EventArgs) Handles UncheckAll.Click
        RemoveHandler CoverList.ItemChecked, AddressOf CoverList_ItemChecked

        For Item = 0 To CoverList.Items.Count - 1
            CoverList.Items(Item).Checked = False
        Next

        AddHandler CoverList.ItemChecked, AddressOf CoverList_ItemChecked

        CoverList_ItemChecked(Nothing, Nothing)
    End Sub

#End Region

#Region "Dates"

    Private Connection As System.Data.SQLite.SQLiteConnection
    Private Command As System.Data.SQLite.SQLiteCommand
    Private DateFormat As String = "MMMM dd, yyyy"
    Private CoverStartDate As New List(Of DateTime)
    Private CoverEndDate As New List(Of DateTime)

    Private Sub Calculate_Evapotranspiration_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        For Each Control In Me.Controls
            GetType(Control).InvokeMember("DoubleBuffered", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance Or Reflection.BindingFlags.SetProperty, Nothing, Control, New Object() {True})
        Next

        If IO.File.Exists(ProjectDetailsPath) Then
            CoverSelectionGroup.Enabled = True
            DatesGroup.Enabled = True

            Connection = CreateConnection(ProjectDetailsPath, False)
            Connection.Open()

            Command = Connection.CreateCommand
            Command.CommandText = "CREATE TABLE IF NOT EXISTS Cover (Name TEXT UNIQUE, Properties TEXT)"
            Command.ExecuteNonQuery()

            Command.CommandText = "SELECT Name FROM Cover ORDER BY ROWID"
            Using Reader = Command.ExecuteReader
                RemoveHandler CoverList.ItemChecked, AddressOf CoverList_ItemChecked

                Do While Reader.Read
                    Dim CoverName = Reader.GetString(0)

                    Dim MinDate = DateTime.MinValue
                    Dim MaxDate = DateTime.MaxValue

                    Dim Path As String = IO.Path.Combine(PotentialEvapotranspirationDirectory, String.Format(IO.Path.GetFileName(PotentialEvapotranspirationPath), CoverName))
                    If IO.File.Exists(Path) Then GetMaxAndMinDates({Path}, MaxDate, MinDate)

                    CoverStartDate.Add(MinDate)
                    CoverEndDate.Add(MaxDate)

                    CoverList.Items.Add(CoverName)
                Loop

                AddHandler CoverList.ItemChecked, AddressOf CoverList_ItemChecked
            End Using

            If CoverList.Items.Count > 0 Then
                Dim MinDate As DateTime = DateTime.MinValue
                Dim MaxDate As DateTime = DateTime.MaxValue
                GetMaxAndMinDates({IO.Directory.GetFiles(InputVariablesDirectory, "*.db")(0)}, MaxDate, MinDate)

                If Not MaxDate = DateTime.MaxValue Then
                    ReferenceDatasetEndDate.Text = MaxDate.ToString(DateFormat)
                    ReferenceDatasetStartDate.Text = MinDate.ToString(DateFormat)

                    CalculationStartDate.MaxDate = MaxDate
                    CalculationStartDate.MinDate = MinDate
                    CalculationEndDate.MaxDate = MaxDate
                    CalculationEndDate.MinDate = MinDate

                    CalculationEndDate.Value = MaxDate
                    CalculationStartDate.Value = MinDate

                    Dim ImageWidth As Integer = 200
                    Dim ImageHeight As Integer = CoverList.GetItemRect(0).Height
                    Dim ImageBuffer As Integer = 10
                    Dim ImageList As New ImageList
                    CoverList.SmallImageList = ImageList
                    CoverList.SmallImageList.ImageSize = New Size(ImageWidth, ImageHeight)

                    For I = 0 To CoverList.Items.Count - 1
                        Dim StartDate = CoverStartDate(I)
                        If StartDate = DateTime.MinValue Then StartDate = MinDate
                        Dim EndDate = CoverEndDate(I)
                        If EndDate = DateTime.MaxValue Then EndDate = MaxDate

                        Dim Image As New Bitmap(ImageWidth, ImageHeight)
                        Dim Graphic = Graphics.FromImage(Image)
                        Graphic.FillRectangle(Brushes.Transparent, 0, 0, Image.Width, Image.Height)

                        Dim Margin As Single = ImageHeight / 4
                        Graphic.FillRectangle(SystemBrushes.ControlDarkDark, ImageBuffer, Margin, Image.Width - ImageBuffer * 2, Image.Height - Margin * 2)

                        If EndDate.Subtract(StartDate).TotalDays > 0 And Not CoverEndDate(I) = DateTime.MaxValue Then
                            Dim Period As Double = (ImageWidth - ImageBuffer * 2) / EndDate.Subtract(StartDate).TotalDays
                            Dim StartDay As Single = StartDate.Subtract(MinDate).TotalDays * Period + ImageBuffer
                            Dim EndDay As Single = EndDate.Subtract(MinDate).TotalDays * Period + ImageBuffer
                            Dim Offset As Single = 1
                            Dim Mid As Single = ImageHeight / 2 - Offset

                            Graphic.DrawLine(New Pen(New SolidBrush(Color.FromArgb(96, 169, 115)), ImageHeight - Margin * 2 - Offset), StartDay, Mid, EndDay, Mid)
                        End If

                        ImageList.Images.Add(Image)
                        CoverList.Items(I).ImageIndex = I
                    Next
                Else
                    CalculateButton.Enabled = False
                    DatesGroup.Enabled = False
                    Exit Sub
                End If
            Else
                CalculateButton.Enabled = False
                DatesGroup.Enabled = False
            End If

            CheckAll_Click(Nothing, Nothing)
            Calculate_Evapotranspiration_Resize(Nothing, Nothing)

            'Calculate_Click(Nothing, Nothing)
        Else
            CoverSelectionGroup.Enabled = False
            DatesGroup.Enabled = False
            CalculateButton.Enabled = False
        End If
    End Sub

    Private Sub Cover_Properties_FormClosed(sender As Object, e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        If Not Command Is Nothing Then Command.Dispose()
        If Not Connection Is Nothing Then Connection.Dispose()
    End Sub

    Private Sub Calculate_Evapotranspiration_Resize(sender As Object, e As System.EventArgs) Handles Me.Resize
        CoverList.Columns(0).Width = CoverList.Width - SystemInformation.VerticalScrollBarWidth - 5
    End Sub

    Private Sub CoverList_ItemChecked(sender As Object, e As System.Windows.Forms.ItemCheckedEventArgs) Handles CoverList.ItemChecked
        Dim CalculationExists = CoverList.CheckedItems.Count > 0
        DatesGroup.Enabled = CalculationExists
        CalculateButton.Enabled = CalculationExists

        If DatesGroup.Enabled = True Then
            Dim MinDate = DateTime.MinValue
            Dim MaxDate = DateTime.MaxValue
            Dim NoCalculation As Boolean = False

            For Each Item As ListViewItem In CoverList.CheckedItems
                If CoverStartDate(Item.Index) > MinDate Then MinDate = CoverStartDate(Item.Index)
                If CoverEndDate(Item.Index) < MaxDate Then MaxDate = CoverEndDate(Item.Index)
                If CoverEndDate(Item.Index) = DateTime.MaxValue Then NoCalculation = True
            Next
            If NoCalculation Then MaxDate = DateTime.MaxValue

            If Not MaxDate = DateTime.MaxValue Then
                PreviousCalculationStartDate.Text = MinDate.ToString(DateFormat)
                PreviousCalculationEndDate.Text = MaxDate.ToString(DateFormat)

                CalculationStartDate.Value = MaxDate.AddDays(1)
                CalculationEndDate.Value = CalculationEndDate.MaxDate
            Else
                PreviousCalculationStartDate.Text = "-"
                PreviousCalculationEndDate.Text = "-"

                CalculationStartDate.Value = CalculationStartDate.MinDate
                CalculationEndDate.Value = CalculationEndDate.MaxDate
            End If
        End If
    End Sub

    Private Sub CalculateButton_Click(sender As System.Object, e As System.EventArgs) Handles CalculateButton.Click
        If Cancel_Button.Enabled = False Then
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()
            Exit Sub
        End If

        Dim CalculationWillOverwriteData As Boolean = False
        For I = 0 To CoverEndDate.Count - 1
            If CalculationStartDate.Value <= CoverEndDate(I) Then
                CalculationWillOverwriteData = True
                Exit For
            End If
        Next

        If CalculationWillOverwriteData Then
            If MsgBox("The selected time period will overwrite previous calculations.  Continue with action anyway?", MsgBoxStyle.YesNo) = MsgBoxResult.No Then
                Exit Sub
            End If
        End If

        Dim CurveNames As New List(Of String)
        Dim CurveProperties As New List(Of String)
        Command.CommandText = "SELECT * FROM Curve"
        Using Reader = Command.ExecuteReader
            Do Until Not Reader.Read
                CurveNames.Add(Reader.GetString(0))
                CurveProperties.Add(Reader.GetString(1))
            Loop
        End Using

        Dim CoverPropertiesList As New List(Of CoverProperties)
        Command.CommandText = "SELECT * FROM Cover"
        Using Reader = Command.ExecuteReader
            Do Until Not Reader.Read
                Dim Cover As New CoverProperties
                Cover.Name = Reader.GetString(0)

                If CoverList.FindItemWithText(Cover.Name).Checked Then
                    Dim Properties = Reader.GetString(1).Split(";")

                    Cover.InitiationThresholdType = [Enum].Parse(GetType(ThresholdType), Properties(1))
                    Cover.InitiationThreshold = Properties(2)

                    Cover.IntermediateThresholdType = [Enum].Parse(GetType(ThresholdType), Properties(3))
                    Cover.IntermediateThreshold = Properties(4)

                    Cover.TerminationThresholdType = [Enum].Parse(GetType(ThresholdType), Properties(5))
                    Cover.TerminationThreshold = Properties(6)

                    Cover.CuttingIntermediateThresholdType = [Enum].Parse(GetType(ThresholdType), Properties(7))
                    Cover.CuttingIntermediateThreshold = Properties(9)

                    Cover.CuttingTerminationThresholdType = [Enum].Parse(GetType(ThresholdType), Properties(8))
                    Cover.CuttingTerminationThreshold = Properties(10)

                    Cover.SpringFrostTemperature = Properties(11)
                    Cover.KillingFrostTemperature = Properties(12)

                    Cover.CurveName = Properties(0)
                    Properties = CurveProperties(CurveNames.IndexOf(Properties(0))).Split(";")

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

                    CoverPropertiesList.Add(Cover)
                End If
            Loop
        End Using

        CoverProperties = CoverPropertiesList.ToArray

        If CoverProperties.Length = 0 Then
            MsgBox("Use must select at least one cover for which to calculate potential evapotranspiration.")
            Exit Sub
        End If

        If Not BackgroundWorker.IsBusy Then
            BackgroundWorker.WorkerReportsProgress = True
            BackgroundWorker.WorkerSupportsCancellation = True

            CoverSelectionGroup.Enabled = False
            DatesGroup.Enabled = False
            CalculateButton.Enabled = False

            ProgressText.Text = ""
            ProgressBar.Minimum = 0
            ProgressBar.Maximum = CoverProperties.Length
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
            Me.Close()
        End If
    End Sub

#End Region

#Region "Background Execution"

    WithEvents BackgroundWorker As New System.ComponentModel.BackgroundWorker
    Private Timer As Stopwatch
    Private CoverProperties() As CoverProperties

    Private Sub BackgroundWorker_DoWork(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker.DoWork
        CalculatePotentialEvapotranspiration(CoverProperties, CalculationStartDate.Value, CalculationEndDate.Value, BackgroundWorker, e)
    End Sub

    Private Sub BackgroundWorker_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorker.ProgressChanged
        ProgressBar.Value += 1
        ProgressText.Text = e.UserState
    End Sub

    Private Sub BackgroundWorker_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker.RunWorkerCompleted
        If e.Cancelled Then
            ProgressText.Text = "Potential evapotranspiration calculations cancelled"
        ElseIf e.Error IsNot Nothing Then
            ProgressText.Text = "There was an error in the calculation process"
        Else
            Timer.Stop()
            ProgressText.Text = "Potential evapotranspiration calculations were successfully completed"
        End If
        ProgressText.Text &= String.Format(" ({0}).", Timer.Elapsed.ToString())

        CalculateButton.Enabled = True
        CalculateButton.Text = "OK"
        Cancel_Button.Enabled = False
    End Sub

#End Region

End Class