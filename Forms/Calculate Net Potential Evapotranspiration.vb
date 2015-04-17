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

    Private DateFormat As String = "MMMM, yyyy"
    Private CoverStatisticsStartDate As New List(Of DateTime)
    Private CoverStatisticsEndDate As New List(Of DateTime)
    Private CoverNetStartDate As New List(Of DateTime)
    Private CoverNetEndDate As New List(Of DateTime)
    Private PrecipitationStartDate As New List(Of DateTime)
    Private PrecipitationEndDate As New List(Of DateTime)

    Private Sub Calculate_Evapotranspiration_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        For Each Control In Me.Controls
            GetType(Control).InvokeMember("DoubleBuffered", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance Or Reflection.BindingFlags.SetProperty, Nothing, Control, New Object() {True})
        Next

        If IO.File.Exists(ProjectDetailsPath) Then
            CoverSelectionGroup.Enabled = True
            PrecipitationGroup.Enabled = True
            DatesGroup.Enabled = True

            RemoveHandler CoverList.ItemChecked, AddressOf CoverList_ItemChecked
            For Each Path In IO.Directory.GetFiles(PotentialEvapotranspirationDirectory)
                Dim CoverName = IO.Path.GetFileNameWithoutExtension(Path).Replace(" Potential Evapotranspiration", "")

                Dim MinDate = DateTime.MinValue
                Dim MaxDate = DateTime.MaxValue
                GetMaxAndMinDates({Path}, MaxDate, MinDate, PotentialEvapotranspirationTableName.Statistics)
                CoverStatisticsStartDate.Add(MinDate)
                CoverStatisticsEndDate.Add(MaxDate)

                MinDate = DateTime.MinValue
                MaxDate = DateTime.MaxValue
                GetMaxAndMinDates({Path}, MaxDate, MinDate, PotentialEvapotranspirationTableName.Net)
                CoverNetStartDate.Add(MinDate)
                CoverNetEndDate.Add(MaxDate)

                CoverList.Items.Add(CoverName)
            Next
            AddHandler CoverList.ItemChecked, AddressOf CoverList_ItemChecked

            For Each Path In IO.Directory.GetFiles(InputVariablesDirectory, "Precipitation *.db")
                Dim ClimateModelName = IO.Path.GetFileNameWithoutExtension(Path)
                ClimateModelName = ClimateModelName.Substring(14, ClimateModelName.Length - 14)

                Dim MinDate = DateTime.MinValue
                Dim MaxDate = DateTime.MaxValue
                GetMaxAndMinDates({Path}, MaxDate, MinDate, PotentialEvapotranspirationTableName.Statistics)
                PrecipitationStartDate.Add(MinDate)
                PrecipitationEndDate.Add(MaxDate)

                PrecipitationDataset.Items.Add(ClimateModelName)
            Next

            If CoverList.Items.Count > 0 And PrecipitationDataset.Items.Count > 0 Then
                Dim MinDate As DateTime = CoverStatisticsStartDate.Concat(PrecipitationStartDate).Min
                Dim MaxDate As DateTime = CoverStatisticsEndDate.Concat(PrecipitationEndDate).Max

                If Not MaxDate = DateTime.MaxValue Then
                    PotentialDatasetStartDate.Text = MinDate.ToString(DateFormat)
                    PotentialDatasetEndDate.Text = MaxDate.ToString(DateFormat)

                    CalculationStartDate.MinDate = MinDate
                    CalculationStartDate.MaxDate = MaxDate
                    CalculationEndDate.MinDate = MinDate
                    CalculationEndDate.MaxDate = MaxDate

                    CalculationStartDate.Value = MinDate
                    CalculationEndDate.Value = MaxDate
                Else
                    CalculateButton.Enabled = False
                    PrecipitationGroup.Enabled = False
                    DatesGroup.Enabled = False
                    Exit Sub
                End If
            Else
                CalculateButton.Enabled = False
                PrecipitationGroup.Enabled = False
                DatesGroup.Enabled = False
            End If

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

    Private Sub Calculate_Evapotranspiration_Resize(sender As Object, e As System.EventArgs) Handles Me.Resize
        CoverList.Columns(0).Width = CoverList.Width - SystemInformation.VerticalScrollBarWidth - 5
    End Sub

    Private Sub CoverList_ItemChecked(sender As Object, e As System.Windows.Forms.ItemCheckedEventArgs) Handles CoverList.ItemChecked
        Dim CalculationExists = CoverList.CheckedItems.Count > 0 And PrecipitationDataset.Text <> ""
        DatesGroup.Enabled = CalculationExists
        CalculateButton.Enabled = CalculationExists

        If CalculationExists Then
            Dim MinDate = DateTime.MinValue
            Dim MaxDate = DateTime.MaxValue

            For Each Item As ListViewItem In CoverList.CheckedItems
                If CoverNetStartDate(Item.Index) > MinDate Then MinDate = CoverNetStartDate(Item.Index)
                If CoverNetEndDate(Item.Index) < MaxDate Then MaxDate = CoverNetEndDate(Item.Index)
                If CoverNetEndDate(Item.Index) = DateTime.MaxValue Then CalculationExists = False
            Next

            If CalculationExists Then
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

            ProgressText.Text = ""
            ProgressBar.Minimum = 0
            ProgressBar.Maximum = CalculationEndDate.Value.Year - (CalculationStartDate.Value.Year - 1)
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

    Private Sub BackgroundWorker_DoWork(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker.DoWork
        'CalculatePotentialEvapotranspiration(CoverProperties, CalculationStartDate.Value, CalculationEndDate.Value, BackgroundWorker, e)
    End Sub

    Private Sub BackgroundWorker_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorker.ProgressChanged
        ProgressBar.Value += 1
        ProgressText.Text = e.UserState
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