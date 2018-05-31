'            Copyright Clayton S. Lewis 2014-2018.
'   Distributed under the Boost Software License, Version 1.0.
'      (See accompanying file GridET License.rtf or copy at
'            http://www.boost.org/LICENSE_1_0.txt)

Public Class Calculate_Net_Potential_Evapotranspiration

#Region "Cover Selection"

    Private Sub CheckAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckAll.Click
        RemoveHandler CoverList.ItemChecked, AddressOf CoverList_ItemChecked

        For Item = 0 To CoverList.Items.Count - 1
            CoverList.Items(Item).Checked = True
        Next

        AddHandler CoverList.ItemChecked, AddressOf CoverList_ItemChecked

        CoverList_ItemChecked(Nothing, Nothing)
    End Sub

    Private Sub UncheckAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UncheckAll.Click
        RemoveHandler CoverList.ItemChecked, AddressOf CoverList_ItemChecked

        For Item = 0 To CoverList.Items.Count - 1
            CoverList.Items(Item).Checked = False
        Next

        AddHandler CoverList.ItemChecked, AddressOf CoverList_ItemChecked

        CoverList_ItemChecked(Nothing, Nothing)
    End Sub

#End Region

#Region "Dates"

    Private CoverStatisticsStartDate() As DateTime
    Private CoverStatisticsEndDate() As DateTime
    Private CoverNetStartDate() As DateTime
    Private CoverNetEndDate() As DateTime
    Private PrecipitationStartDate() As DateTime
    Private PrecipitationEndDate() As DateTime
    Private EffectivePrecipitation As New List(Of EffectivePrecipitationType)
    Private DateFormat As String = "MMMM yyyy"

    Private Sub Calculate_Evapotranspiration_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        For Each Control In Me.Controls
            GetType(Control).InvokeMember("DoubleBuffered", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance Or Reflection.BindingFlags.SetProperty, Nothing, Control, New Object() {True})
        Next

        If IO.File.Exists(ProjectDetailsPath) Then
            CoverSelectionGroup.Enabled = True
            PrecipitationGroup.Enabled = True
            DatesGroup.Enabled = True

            Dim Tasks As New List(Of Task)

            Dim CoverNames As New List(Of String)
            Dim Paths = IO.Directory.GetFiles(PotentialEvapotranspirationDirectory, "*.db")
            ReDim CoverStatisticsStartDate(Paths.Length - 1)
            ReDim CoverStatisticsEndDate(Paths.Length - 1)
            ReDim CoverNetStartDate(Paths.Length - 1)
            ReDim CoverNetEndDate(Paths.Length - 1)
            For P = 0 To Paths.Length - 1 : Dim Path = Paths(P), I = P
                Tasks.Add(Task.Factory.StartNew(
                Sub()
                    Using RasterArray As New RasterArray(Path)

                        Dim MinDate = DateTime.MinValue
                        Dim MaxDate = DateTime.MaxValue
                        RasterArray.GetMinAndMaxDates(RasterType.Sum, MinDate, MaxDate)
                        CoverStatisticsStartDate(I) = MinDate
                        CoverStatisticsEndDate(I) = MaxDate

                        MinDate = DateTime.MinValue
                        MaxDate = DateTime.MaxValue
                        RasterArray.GetMinAndMaxDates(RasterType.Net, MinDate, MaxDate)
                        CoverNetStartDate(I) = MinDate
                        CoverNetEndDate(I) = MaxDate

                    End Using
                End Sub))

                CoverNames.Add(IO.Path.GetFileNameWithoutExtension(Path).Replace(" Potential Evapotranspiration", ""))
            Next

            Paths = IO.Directory.GetFiles(InputVariablesDirectory, "Precipitation *.db")
            ReDim PrecipitationStartDate(Paths.Length - 1)
            ReDim PrecipitationEndDate(Paths.Length - 1)
            For P = 0 To Paths.Length - 1 : Dim Path = Paths(P), I = P
                Tasks.Add(Task.Factory.StartNew(
                Sub()
                    Using RasterArray As New RasterArray(Path)

                        Dim MinDate = DateTime.MinValue
                        Dim MaxDate = DateTime.MaxValue
                        RasterArray.GetMinAndMaxDates(RasterType.Sum, MinDate, MaxDate)
                        PrecipitationStartDate(I) = MinDate
                        PrecipitationEndDate(I) = MaxDate

                    End Using
                End Sub))

                Dim ClimateModelName = IO.Path.GetFileNameWithoutExtension(Path)
                ClimateModelName = ClimateModelName.Substring(14, ClimateModelName.Length - 14)
                PrecipitationDataset.Items.Add(ClimateModelName)
            Next

            If CoverNames.Count > 0 Then
                Using Connection = CreateConnection(ProjectDetailsPath), Command = Connection.CreateCommand : Connection.Open()

                    Command.CommandText = "CREATE TEMP TABLE T (CoverName TEXT)"
                    Command.ExecuteNonQuery()

                    Command.CommandText = "INSERT INTO T (CoverName) VALUES (@CoverName)"
                    For Each CoverName In CoverNames
                        Command.Parameters.Add("@CoverName", DbType.String).Value = CoverName
                        Command.ExecuteNonQuery()
                    Next

                    Command.CommandText = String.Format("SELECT Properties FROM Cover WHERE Name IN (SELECT CoverName FROM T)", String.Join("@", Enumerable.Range(0, CoverNames.Count - 1)))
                    Using Reader = Command.ExecuteReader
                        While Reader.Read
                            EffectivePrecipitation.Add([Enum].Parse(GetType(EffectivePrecipitationType), Reader.GetString(0).Split(";")(0)))
                        End While
                    End Using

                End Using

                RemoveHandler CoverList.ItemChecked, AddressOf CoverList_ItemChecked
                For Each Cover In CoverNames
                    CoverList.Items.Add(Cover)
                Next
                AddHandler CoverList.ItemChecked, AddressOf CoverList_ItemChecked
            End If

            Task.WaitAll(Tasks.ToArray)

            If PrecipitationDataset.Items.Count > 0 Then PrecipitationDataset.Text = PrecipitationDataset.Items(0)
            CheckAll_Click(Nothing, Nothing)
            Calculate_Evapotranspiration_Resize(Nothing, Nothing)
        Else
            CoverSelectionGroup.Enabled = False
            PrecipitationGroup.Enabled = False
            DatesGroup.Enabled = False
            CalculateButton.Enabled = False
        End If
    End Sub

    Private Sub Calculate_Evapotranspiration_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = BackgroundWorker.IsBusy
        Cancel_Button_Click(Nothing, Nothing)
    End Sub

    Private Sub Calculate_Evapotranspiration_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        CoverList.Columns(0).Width = CoverList.Width - SystemInformation.VerticalScrollBarWidth - 5
    End Sub

    Private Sub DateTimePicker_PreviewKeyDown(ByVal sender As DateTimePicker, ByVal e As System.Windows.Forms.PreviewKeyDownEventArgs) Handles CalculationStartDate.PreviewKeyDown, CalculationEndDate.PreviewKeyDown
        sender.Value = New DateTime(sender.Value.Year, sender.Value.Month, 1)
    End Sub

    Private Sub DateTimePicker_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles CalculationStartDate.ValueChanged, CalculationEndDate.ValueChanged
        sender.Value = New DateTime(sender.Value.Year, sender.Value.Month, 1)

        If CalculationStartDate.Value > CalculationEndDate.Value Then CalculationStartDate.Value = CalculationEndDate.Value
    End Sub

    Private Sub PrecipitationDataset_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles PrecipitationDataset.TextChanged
        CoverList_ItemChecked(Nothing, Nothing)
    End Sub

    Private Sub CoverList_ItemChecked(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemCheckedEventArgs) Handles CoverList.ItemChecked
        Dim CalculationExists = CoverList.CheckedItems.Count > 0 And PrecipitationDataset.Text <> ""
        DatesGroup.Enabled = CalculationExists
        CalculateButton.Enabled = CalculationExists
        ProgressText.Visible = False

        If CalculationExists Then
            Dim IncompleteCalculation As Boolean = False
            Dim MinDateNet = DateTime.MinValue
            Dim MaxDateNet = DateTime.MaxValue
            Dim MinDateStatistics = DateTime.MinValue
            Dim MaxDateStatistics = DateTime.MaxValue

            For Each Item As ListViewItem In CoverList.CheckedItems
                If CoverNetStartDate(Item.Index) > MinDateNet Then MinDateNet = CoverNetStartDate(Item.Index)
                If CoverNetEndDate(Item.Index) < MaxDateNet Then MaxDateNet = CoverNetEndDate(Item.Index)
                If CoverNetEndDate(Item.Index) = DateTime.MaxValue Then IncompleteCalculation = True
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

                If MaxDateNet <> DateTime.MaxValue And MaxDateNet >= MinDateNet And Not IncompleteCalculation Then
                    PreviousCalculationStartDate.Text = MinDateNet.ToString(DateFormat)
                    PreviousCalculationEndDate.Text = MaxDateNet.ToString(DateFormat)

                    CalculationStartDate.Value = MaxDateNet
                    Dim Days = DateTime.DaysInMonth(MaxDateNet.Year, MaxDateNet.Month) - (MaxDateNet.Day) + 1
                    If Days <= MaxDateStatistics.Subtract(MaxDateNet).TotalDays Then
                        CalculationStartDate.Value = MaxDateNet.AddDays(Days)
                    Else
                        ProgressText.Text = "Dataset is up-to-date."
                        ProgressText.Visible = True
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

    Private Sub CalculateButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CalculateButton.Click
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
            ProgressBar.Maximum = Math.Ceiling(CalculationEndDate.Value.Subtract(CalculationStartDate.Value).TotalDays / 365.25 * 14)
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

    Private Sub BackgroundWorker_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker.DoWork
        CalculateNetPotentialEvapotranspiration(CoverPaths.ToArray, CoverEffectivePrecipitation.ToArray, PrecipitationPath, CalculationStartDate.Value, New DateTime(CalculationEndDate.Value.Year, CalculationEndDate.Value.Month, DateTime.DaysInMonth(CalculationEndDate.Value.Year, CalculationEndDate.Value.Month)), BackgroundWorker, e)
    End Sub

    Private Sub BackgroundWorker_ProgressChanged(ByVal sender As Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorker.ProgressChanged
        If ProgressBar.Value < ProgressBar.Maximum Then
            ProgressBar.Value += 1
            Dim Timespan As TimeSpan = New TimeSpan(Timer.Elapsed.Ticks * (ProgressBar.Maximum / ProgressBar.Value - 1))
            ProgressText.Text = String.Format("Estimated time remaining...({0})", Timespan.ToString("d\.hh\:mm\:ss"))
        End If
    End Sub

    Private Sub BackgroundWorker_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker.RunWorkerCompleted
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

#Region "Process Scheduling"

    Public ReadOnly Property Progress
        Get
            Me.Update()
            Return New ProgressValues(ProgressText.Text, ProgressBar.Minimum, ProgressBar.Maximum, ProgressBar.Value)
        End Get
    End Property

    WithEvents ProcessTimer As Timer

    Public Sub LoadScheduledProcess()
        Calculate_Evapotranspiration_Load(Nothing, Nothing)
    End Sub

    Public Sub RunScheduledProcess()
        CalculateButton_Click(Nothing, Nothing)

        ProcessTimer = New Timer With {.Interval = 1000}
        ProcessTimer.Start()
    End Sub

    Public Sub CancelScheduledProcess()
        Cancel_Button_Click(Nothing, Nothing)
    End Sub

    Private Sub ProcessTimer_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles ProcessTimer.Tick
        ProcessTimerContinue()
    End Sub

    Private Sub ProcessTimerContinue()
        If BackgroundWorker.IsBusy Then ProcessTimer.Start()
    End Sub

#End Region

End Class