Public Class Calculate_Reference_Evapotranspiration

#Region "Climate Dataset Selection"

    Private Sub CheckAll_Click(sender As System.Object, e As System.EventArgs) Handles CheckAll.Click
        RemoveHandler DatasetList.ItemChecked, AddressOf DatasetList_ItemChecked

        For Item = 0 To DatasetList.Items.Count - 1
            DatasetList.Items(Item).Checked = True
        Next

        AddHandler DatasetList.ItemChecked, AddressOf DatasetList_ItemChecked

        DatasetList_ItemChecked(Nothing, Nothing)
    End Sub

    Private Sub UncheckAll_Click(sender As System.Object, e As System.EventArgs) Handles UncheckAll.Click
        RemoveHandler DatasetList.ItemChecked, AddressOf DatasetList_ItemChecked

        For Item = 0 To DatasetList.Items.Count - 1
            DatasetList.Items(Item).Checked = False
        Next

        AddHandler DatasetList.ItemChecked, AddressOf DatasetList_ItemChecked

        DatasetList_ItemChecked(Nothing, Nothing)
    End Sub

#End Region

#Region "Dates"

    Private Connection As System.Data.SQLite.SQLiteConnection
    Private Command As System.Data.SQLite.SQLiteCommand
    Private DateFormat As String = "MMMM dd, yyyy"
    Private DatasetMinDate As New List(Of DateTime)
    Private DatasetMaxDate As New List(Of DateTime)
    Dim CalculationMinDate As New List(Of DateTime)
    Dim CalculationMaxDate As New List(Of DateTime)

    Private Sub Calculate_Evapotranspiration_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        For Each Control In Me.Controls
            GetType(Control).InvokeMember("DoubleBuffered", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance Or Reflection.BindingFlags.SetProperty, Nothing, Control, New Object() {True})
        Next

        If IO.Directory.Exists(ClimateModelDirectory) Then
            ClimateDatasetGroup.Enabled = True
            DatesGroup.Enabled = True

            For Each FileName In {NLDAS_2ARastersPath, DAYMETRastersPath, PRISMRastersPath}
                Dim Path = FileName

                If IO.File.Exists(Path) Then
                    Dim MinDate = DateTime.MinValue
                    Dim MaxDate = DateTime.MaxValue

                    If IO.File.Exists(Path) Then GetMaxAndMinDates({Path}, MaxDate, MinDate)
                    If MaxDate.Hour <> 13 And MaxDate <> DateTime.MaxValue Then MaxDate = MaxDate.AddDays(-1)

                    DatasetMinDate.Add(MinDate)
                    DatasetMaxDate.Add(MaxDate)

                    DatasetList.Items.Add(IO.Path.GetFileNameWithoutExtension(Path))

                    MinDate = DateTime.MinValue
                    MaxDate = DateTime.MaxValue

                    Dim Paths = IO.Directory.GetFiles(InputVariablesDirectory, "*" & IO.Path.GetFileName(FileName))
                    If Paths.Length > 0 Then GetMaxAndMinDates({Paths(0)}, MaxDate, MinDate)
                    If MaxDate.Hour <> 13 And MaxDate <> DateTime.MaxValue Then MaxDate = MaxDate.AddDays(-1)

                    CalculationMinDate.Add(MinDate)
                    CalculationMaxDate.Add(MaxDate)
                End If
            Next

            If DatasetList.Items.Count > 0 Then
                Dim ClimateDatasetMinDate = DatasetMinDate.Max
                Dim ClimateDatasetMaxDate = DatasetMaxDate.Min

                If Not ClimateDatasetMaxDate = DateTime.MaxValue Then
                    ClimateDatasetStartDate.Text = ClimateDatasetMinDate.ToString(DateFormat)
                    ClimateDatasetEndDate.Text = ClimateDatasetMaxDate.ToString(DateFormat)

                    CalculationStartDate.MinDate = ClimateDatasetMinDate
                    CalculationStartDate.MaxDate = ClimateDatasetMaxDate
                    CalculationEndDate.MinDate = ClimateDatasetMinDate
                    CalculationEndDate.MaxDate = ClimateDatasetMaxDate

                    CalculationStartDate.Value = ClimateDatasetMinDate
                    CalculationEndDate.Value = ClimateDatasetMaxDate
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
        Else
            ClimateDatasetGroup.Enabled = False
            DatesGroup.Enabled = False
            CalculateButton.Enabled = False
        End If
    End Sub

    Private Sub Calculate_Evapotranspiration_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = BackgroundWorker.IsBusy
        Cancel_Button_Click(Nothing, Nothing)
    End Sub

    Private Sub Calculate_Evapotranspiration_FormClosed(sender As Object, e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        If Not Command Is Nothing Then Command.Dispose()
        If Not Connection Is Nothing Then Connection.Dispose()
    End Sub

    Private Sub Calculate_Evapotranspiration_Resize(sender As Object, e As System.EventArgs) Handles Me.Resize
        DatasetList.Columns(0).Width = DatasetList.Width - SystemInformation.VerticalScrollBarWidth - 5
    End Sub

    Private Sub DatasetList_ItemChecked(sender As Object, e As System.Windows.Forms.ItemCheckedEventArgs) Handles DatasetList.ItemChecked
        Dim CalculationExists = DatasetList.CheckedItems.Count > 0
        DatesGroup.Enabled = CalculationExists
        CalculateButton.Enabled = CalculationExists

        If CalculationExists Then
            Dim MinDateCalculation = DateTime.MinValue
            Dim MaxDateCalculation = DateTime.MaxValue
            Dim MinDateDataset = DateTime.MinValue
            Dim MaxDateDataset = DateTime.MaxValue

            For Each Item As ListViewItem In DatasetList.CheckedItems
                If CalculationMinDate(Item.Index) > MinDateCalculation Then MinDateCalculation = CalculationMinDate(Item.Index)
                If CalculationMaxDate(Item.Index) < MaxDateCalculation Then MaxDateCalculation = CalculationMaxDate(Item.Index)
                If CalculationMaxDate(Item.Index) = DateTime.MaxValue Then CalculationExists = False
                If DatasetMinDate(Item.Index) > MinDateDataset Then MinDateDataset = DatasetMinDate(Item.Index)
                If DatasetMaxDate(Item.Index) < MaxDateDataset Then MaxDateDataset = DatasetMaxDate(Item.Index)
            Next

            ClimateDatasetStartDate.Text = MinDateDataset.ToString(DateFormat)
            ClimateDatasetEndDate.Text = MaxDateDataset.ToString(DateFormat)

            CalculationStartDate.MinDate = MinDateDataset
            CalculationStartDate.MaxDate = MaxDateDataset
            CalculationEndDate.MinDate = MinDateDataset
            CalculationEndDate.MaxDate = MaxDateDataset

            If CalculationExists Then
                PreviousCalculationStartDate.Text = MinDateCalculation.ToString(DateFormat)
                PreviousCalculationEndDate.Text = MaxDateCalculation.ToString(DateFormat)

                CalculationStartDate.Value = MaxDateCalculation
                If Not CalculationStartDate.Value = CalculationStartDate.MaxDate Then CalculationStartDate.Value = CalculationStartDate.Value.AddDays(1)
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

        For Each Item As ListViewItem In DatasetList.CheckedItems
            If CalculationStartDate.Value <= CalculationMaxDate(Item.Index) And CalculationMaxDate(Item.Index) <> DateTime.MaxValue Then
                If MsgBox("The selected time period will overwrite previous calculations.  Continue with action anyway?", MsgBoxStyle.YesNo) = MsgBoxResult.No Then
                    Exit Sub
                End If

                Exit For
            End If
        Next

        If Not BackgroundWorker.IsBusy Then
            BackgroundWorker.WorkerReportsProgress = True
            BackgroundWorker.WorkerSupportsCancellation = True

            ClimateDatasetGroup.Enabled = False
            DatesGroup.Enabled = False
            CalculateButton.Enabled = False

            ProgressText.Text = "Initializing calculation datasets..."
            ProgressBar.Minimum = 0
            ProgressBar.Maximum = (CalculationEndDate.Value.Subtract(CalculationStartDate.Value).TotalDays + 1) * DatasetList.CheckedItems.Count
            ProgressBar.Value = 0
            ProgressText.Visible = True
            ProgressBar.Visible = True

            Timer = New Stopwatch
            Timer.Start()

            For Each Item In DatasetList.CheckedItems
                SelectedClimateDatasets.Add(Item.Text)
            Next

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
            RemoveHandler Me.FormClosing, AddressOf Calculate_Evapotranspiration_FormClosing
            Me.Close()
        End If
    End Sub

#End Region

#Region "Background Execution"

    WithEvents BackgroundWorker As New System.ComponentModel.BackgroundWorker
    Private Timer As Stopwatch
    Private SelectedClimateDatasets As New List(Of String)

    Private Sub BackgroundWorker_DoWork(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker.DoWork
        Dim StartDate = New DateTime(CalculationStartDate.Value.Year, CalculationStartDate.Value.Month, CalculationStartDate.Value.Day).AddHours(13)
        Dim EndDate = New DateTime(CalculationEndDate.Value.Year, CalculationEndDate.Value.Month, CalculationEndDate.Value.Day).AddHours(12)

        If SelectedClimateDatasets.Contains("NLDAS_2A") Then CalculateReferenceEvapotranspirationNLDAS_2A(StartDate, EndDate.AddDays(1), BackgroundWorker, e)
        If SelectedClimateDatasets.Contains("DAYMET") Then CalculateReferenceEvapotranspirationDAYMET(StartDate, EndDate, BackgroundWorker, e)
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
            ProgressText.Text = "Reference evapotranspiration calculations cancelled"
        ElseIf e.Error IsNot Nothing Then
            ProgressText.Text = "There was an error in the calculation process"
        Else
            Timer.Stop()
            ProgressText.Text = "Reference evapotranspiration calculations were successfully completed"
        End If
        ProgressText.Text &= String.Format(" ({0}).", Timer.Elapsed.ToString())

        CalculateButton.Enabled = True
        CalculateButton.Text = "OK"
        Cancel_Button.Enabled = False
    End Sub

#End Region

End Class