Public Class Calculate_Net_Potential_Evapotranspiration

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

    Private DateFormat As String = "MMMM yyyy"
    Private CoverStatisticsStartDate As New List(Of DateTime)
    Private CoverStatisticsEndDate As New List(Of DateTime)
    Private CoverNetStartDate As New List(Of DateTime)
    Private CoverNetEndDate As New List(Of DateTime)
    Private PrecipitationStartDate As New List(Of DateTime)
    Private PrecipitationEndDate As New List(Of DateTime)
    Private EffectivePrecipitation As New List(Of EffectivePrecipitationType)

    Private Sub Calculate_Evapotranspiration_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        For Each Control In Me.Controls
            GetType(Control).InvokeMember("DoubleBuffered", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance Or Reflection.BindingFlags.SetProperty, Nothing, Control, New Object() {True})
        Next

        If IO.File.Exists(ProjectDetailsPath) Then
            CoverSelectionGroup.Enabled = True
            PrecipitationGroup.Enabled = True
            DatesGroup.Enabled = True

            Using Connection = CreateConnection(ProjectDetailsPath)
                Connection.Open()
                Using Command = Connection.CreateCommand

                    RemoveHandler CoverList.ItemChecked, AddressOf CoverList_ItemChecked
                    For Each Path In IO.Directory.GetFiles(PotentialEvapotranspirationDirectory, "*.db")
                        Dim CoverName = IO.Path.GetFileNameWithoutExtension(Path).Replace(" Potential Evapotranspiration", "")

                        Dim MinDate = DateTime.MinValue
                        Dim MaxDate = DateTime.MaxValue
                        GetMaxAndMinDates({Path}, MaxDate, MinDate, DatabaseTableName.Statistics)
                        CoverStatisticsStartDate.Add(MinDate)
                        CoverStatisticsEndDate.Add(MaxDate)

                        MinDate = DateTime.MinValue
                        MaxDate = DateTime.MaxValue
                        GetMaxAndMinDates({Path}, MaxDate, MinDate, DatabaseTableName.Net)
                        CoverNetStartDate.Add(MinDate)
                        CoverNetEndDate.Add(MaxDate)

                        Command.CommandText = "SELECT Properties FROM Cover WHERE Name = @Name"
                        Command.Parameters.Add("@Name", DbType.String).Value = CoverName
                        EffectivePrecipitation.Add([Enum].Parse(GetType(EffectivePrecipitationType), Command.ExecuteScalar.ToString.Split(";")(0)))

                        CoverList.Items.Add(CoverName)
                    Next
                    AddHandler CoverList.ItemChecked, AddressOf CoverList_ItemChecked

                End Using
            End Using

            For Each Path In IO.Directory.GetFiles(InputVariablesDirectory, "Precipitation *.db")
                Dim ClimateModelName = IO.Path.GetFileNameWithoutExtension(Path)
                ClimateModelName = ClimateModelName.Substring(14, ClimateModelName.Length - 14)

                Dim MinDate = DateTime.MinValue
                Dim MaxDate = DateTime.MaxValue
                GetMaxAndMinDates({Path}, MaxDate, MinDate, DatabaseTableName.Statistics)
                PrecipitationStartDate.Add(MinDate)
                PrecipitationEndDate.Add(MaxDate)

                PrecipitationDataset.Items.Add(ClimateModelName)
            Next

            If PrecipitationDataset.Items.Count > 0 Then PrecipitationDataset.Text = PrecipitationDataset.Items(0)
            CheckAll_Click(Nothing, Nothing)
            Calculate_Evapotranspiration_Resize(Nothing, Nothing)

            'Calculate_Click(Nothing, Nothing)
        Else
            CoverSelectionGroup.Enabled = False
            PrecipitationGroup.Enabled = False
            DatesGroup.Enabled = False
            CalculateButton.Enabled = False
        End If
    End Sub

    Private Sub Calculate_Evapotranspiration_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = BackgroundWorker.IsBusy
        Cancel_Button_Click(Nothing, Nothing)
    End Sub

    Private Sub Calculate_Evapotranspiration_Resize(sender As Object, e As System.EventArgs) Handles Me.Resize
        CoverList.Columns(0).Width = CoverList.Width - SystemInformation.VerticalScrollBarWidth - 5
    End Sub

    Private Sub DateTimePicker_PreviewKeyDown(sender As DateTimePicker, e As System.Windows.Forms.PreviewKeyDownEventArgs) Handles CalculationStartDate.PreviewKeyDown, CalculationEndDate.PreviewKeyDown
        sender.Value = New DateTime(sender.Value.Year, sender.Value.Month, 1)
    End Sub

    Private Sub DateTimePicker_ValueChanged(sender As Object, e As System.EventArgs) Handles CalculationStartDate.ValueChanged, CalculationEndDate.ValueChanged
        If sender Is CalculationStartDate Then
            sender.Value = New DateTime(sender.Value.Year, sender.Value.Month, 1)
        Else
            sender.Value = New DateTime(sender.Value.Year, sender.Value.Month, DateTime.DaysInMonth(sender.Value.Year, sender.Value.Month))
        End If
    End Sub

    Private Sub PrecipitationDataset_TextChanged(sender As Object, e As System.EventArgs) Handles PrecipitationDataset.TextChanged
        CoverList_ItemChecked(Nothing, Nothing)
    End Sub

    Private Sub CoverList_ItemChecked(sender As Object, e As System.Windows.Forms.ItemCheckedEventArgs) Handles CoverList.ItemChecked
        Dim CalculationExists = CoverList.CheckedItems.Count > 0 And PrecipitationDataset.Text <> ""
        DatesGroup.Enabled = CalculationExists
        CalculateButton.Enabled = CalculationExists

        If CalculationExists Then
            Dim MinDateNet = DateTime.MinValue
            Dim MaxDateNet = DateTime.MaxValue
            Dim MinDateStatistics = DateTime.MinValue
            Dim MaxDateStatistics = DateTime.MaxValue

            For Each Item As ListViewItem In CoverList.CheckedItems
                If CoverNetStartDate(Item.Index) > MinDateNet Then MinDateNet = CoverNetStartDate(Item.Index)
                If CoverNetEndDate(Item.Index) < MaxDateNet Then MaxDateNet = CoverNetEndDate(Item.Index)
                If CoverStatisticsStartDate(Item.Index) > MinDateStatistics Then MinDateStatistics = CoverStatisticsStartDate(Item.Index)
                If CoverStatisticsEndDate(Item.Index) < MaxDateStatistics Then MaxDateStatistics = CoverStatisticsEndDate(Item.Index)
                If CoverStatisticsEndDate(Item.Index) = DateTime.MaxValue Then CalculationExists = False
            Next
            If PrecipitationStartDate(PrecipitationDataset.SelectedIndex) > MinDateStatistics Then MinDateStatistics = PrecipitationStartDate(PrecipitationDataset.SelectedIndex)
            If PrecipitationEndDate(PrecipitationDataset.SelectedIndex) < MaxDateStatistics Then MaxDateStatistics = PrecipitationEndDate(PrecipitationDataset.SelectedIndex)
            If PrecipitationEndDate(PrecipitationDataset.SelectedIndex) = DateTime.MaxValue Then CalculationExists = False

            If CalculationExists Then
                PotentialDatasetStartDate.Text = MinDateStatistics.ToString(DateFormat)
                PotentialDatasetEndDate.Text = MaxDateStatistics.ToString(DateFormat)

                CalculationStartDate.MinDate = MinDateStatistics
                CalculationStartDate.MaxDate = MaxDateStatistics
                CalculationEndDate.MinDate = MinDateStatistics
                CalculationEndDate.MaxDate = MaxDateStatistics

                If MaxDateNet <> DateTime.MaxValue Then
                    PreviousCalculationStartDate.Text = MinDateNet.ToString(DateFormat)
                    PreviousCalculationEndDate.Text = MaxDateNet.ToString(DateFormat)

                    CalculationStartDate.Value = MaxDateNet
                    Dim Days = DateTime.DaysInMonth(MaxDateNet.Year, MaxDateNet.Month) - (MaxDateNet.Day) + 1
                    If Days <= MaxDateStatistics.Subtract(MaxDateNet).TotalDays Then
                        CalculationStartDate.Value = CalculationStartDate.Value.AddDays(Days)
                    End If
                    CalculationEndDate.Value = CalculationEndDate.MaxDate
                Else
                    PreviousCalculationStartDate.Text = "-"
                    PreviousCalculationEndDate.Text = "-"

                    CalculationStartDate.Value = CalculationStartDate.MinDate
                    CalculationEndDate.Value = CalculationEndDate.MaxDate
                End If
            Else
                PotentialDatasetStartDate.Text = "-"
                PotentialDatasetEndDate.Text = "-"

                PreviousCalculationStartDate.Text = "-"
                PreviousCalculationEndDate.Text = "-"

                CalculateButton.Enabled = False
                DatesGroup.Enabled = False
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
        For I = 0 To CoverStatisticsEndDate.Count - 1
            If CalculationStartDate.Value <= CoverNetEndDate(I) And CalculationEndDate.Value >= CoverNetStartDate(I) And CoverNetEndDate(I) <> DateTime.MaxValue Then
                CalculationWillOverwriteData = True
                Exit For
            End If
        Next

        If CalculationWillOverwriteData Then
            If MsgBox("The selected time period will overwrite previous calculations.  Continue with action anyway?", MsgBoxStyle.YesNo) = MsgBoxResult.No Then
                Exit Sub
            End If
        End If

        If CoverList.CheckedItems.Count = 0 Then
            MsgBox("Use must select at least one cover for which to calculate net potential evapotranspiration.")
            Exit Sub
        End If

        If Not BackgroundWorker.IsBusy Then
            BackgroundWorker.WorkerReportsProgress = True
            BackgroundWorker.WorkerSupportsCancellation = True

            CoverSelectionGroup.Enabled = False
            PrecipitationGroup.Enabled = False
            DatesGroup.Enabled = False
            CalculateButton.Enabled = False

            ProgressText.Text = "Initializing calculation datasets..."
            ProgressBar.Minimum = 0
            ProgressBar.Maximum = Math.Ceiling(CalculationEndDate.Value.Subtract(CalculationStartDate.Value).TotalDays / 365.25 * 15)
            ProgressBar.Value = 0
            ProgressText.Visible = True
            ProgressBar.Visible = True

            For Each CoverName As ListViewItem In CoverList.CheckedItems
                CoverPaths.Add(IO.Path.Combine(PotentialEvapotranspirationDirectory, String.Format(IO.Path.GetFileName(PotentialEvapotranspirationPath), CoverName.Text)))
                CoverEffectivePrecipitation.Add(EffectivePrecipitation(CoverName.Index))
            Next
            PrecipitationPath = IO.Path.Combine(InputVariablesDirectory, String.Format("Precipitation {0}.db", PrecipitationDataset.Text))

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
            RemoveHandler Me.FormClosing, AddressOf Calculate_Evapotranspiration_FormClosing
            Me.Close()
        End If
    End Sub

#End Region

#Region "Background Execution"

    WithEvents BackgroundWorker As New System.ComponentModel.BackgroundWorker
    Private Timer As Stopwatch
    Private CoverPaths As New List(Of String)
    Private PrecipitationPath As String
    Private CoverEffectivePrecipitation As New List(Of EffectivePrecipitationType)

    Private Sub BackgroundWorker_DoWork(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker.DoWork
        CalculateNetPotentialEvapotranspiration(CoverPaths.ToArray, CoverEffectivePrecipitation.ToArray, PrecipitationPath, CalculationStartDate.Value, CalculationEndDate.Value, BackgroundWorker, e)
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
            ProgressText.Text = "Net potential evapotranspiration calculations cancelled"
        ElseIf e.Error IsNot Nothing Then
            ProgressText.Text = "There was an error in the calculation process"
        Else
            Timer.Stop()
            ProgressText.Text = "Net potential evapotranspiration calculations were successfully completed"
        End If
        ProgressText.Text &= String.Format(" ({0}).", Timer.Elapsed.ToString())

        CalculateButton.Enabled = True
        CalculateButton.Text = "OK"
        Cancel_Button.Enabled = False
    End Sub

#End Region

 
End Class