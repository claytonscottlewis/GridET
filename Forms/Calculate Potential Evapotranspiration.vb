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
    Private CoverProperties() As CoverProperties
    Private CoverStartDate() As DateTime
    Private CoverEndDate() As DateTime
    Private ReferenceVariable As New List(Of String)
    Private ReferenceStartDate As New List(Of DateTime)
    Private ReferenceEndDate As New List(Of DateTime)

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

            CoverProperties = GetCoverProperties()
            ReDim CoverStartDate(CoverProperties.Length - 1)
            ReDim CoverEndDate(CoverProperties.Length - 1)

            RemoveHandler CoverList.ItemChecked, AddressOf CoverList_ItemChecked
            For I = 0 To CoverProperties.Length - 1
                Dim MinDate = DateTime.MinValue
                Dim MaxDate = DateTime.MaxValue

                Dim Path As String = IO.Path.Combine(PotentialEvapotranspirationDirectory, String.Format(IO.Path.GetFileName(PotentialEvapotranspirationPath), CoverProperties(I).Name))
                If IO.File.Exists(Path) Then GetMaxAndMinDates({Path}, MaxDate, MinDate)

                CoverStartDate(I) = MinDate
                CoverEndDate(I) = MaxDate

                CoverList.Items.Add(CoverProperties(I).Name)

                ReferenceVariable.Add(CoverProperties(I).Variable)
            Next
            AddHandler CoverList.ItemChecked, AddressOf CoverList_ItemChecked

            ReferenceVariable = ReferenceVariable.Distinct.ToList()
            For I = 0 To ReferenceVariable.Count - 1
                Dim MinDate = DateTime.MinValue
                Dim MaxDate = DateTime.MaxValue

                Dim Path As String = IO.Directory.GetFiles(IntermediateCalculationsDirectory, "*" & ReferenceVariable(I) & ".db", IO.SearchOption.AllDirectories)(0)
                If IO.File.Exists(Path) Then GetMaxAndMinDates({Path}, MaxDate, MinDate)

                ReferenceStartDate.Add(MinDate)
                ReferenceEndDate.Add(MaxDate)
            Next

            If CoverList.Items.Count > 0 Then
                Dim MinDate As DateTime = ReferenceStartDate.Min
                Dim MaxDate As DateTime = ReferenceEndDate.Max

                If MaxDate = DateTime.MaxValue Then
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
            CoverSelectionGroup.Enabled = False
            DatesGroup.Enabled = False
            CalculateButton.Enabled = False
        End If
    End Sub

    Private Sub Calculate_Evapotranspiration_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = BackgroundWorker.IsBusy
        Cancel_Button_Click(Nothing, Nothing)
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
        ProgressText.Visible = False

        If CalculationExists Then
            Dim MinDateReference = DateTime.MinValue
            Dim MaxDateReference = DateTime.MaxValue
            Dim MinDateCover = DateTime.MinValue
            Dim MaxDateCover = DateTime.MaxValue
            Dim NoCalculation As Boolean = False

            For Each Item As ListViewItem In CoverList.CheckedItems
                Dim ReferenceIndex = ReferenceVariable.IndexOf(CoverProperties(Item.Index).Variable)
                If ReferenceStartDate(ReferenceIndex) > MinDateReference Then MinDateReference = ReferenceStartDate(ReferenceIndex)
                If ReferenceEndDate(ReferenceIndex) < MaxDateReference Then MaxDateReference = ReferenceEndDate(ReferenceIndex)
                If CoverStartDate(Item.Index) > MinDateCover Then MinDateCover = CoverStartDate(Item.Index)
                If CoverEndDate(Item.Index) < MaxDateCover Then MaxDateCover = CoverEndDate(Item.Index)
                If CoverEndDate(Item.Index) = DateTime.MaxValue Then NoCalculation = True
            Next
            If NoCalculation Then MaxDateCover = DateTime.MaxValue

            ReferenceDatasetStartDate.Text = MinDateReference.ToString(DateFormat)
            ReferenceDatasetEndDate.Text = MaxDateReference.ToString(DateFormat)

            CalculationStartDate.MinDate = MinDateReference
            CalculationStartDate.MaxDate = MaxDateReference
            CalculationEndDate.MinDate = MinDateReference
            CalculationEndDate.MaxDate = MaxDateReference

            If NoCalculation Then
                PreviousCalculationStartDate.Text = "-"
                PreviousCalculationEndDate.Text = "-"

                CalculationStartDate.Value = CalculationStartDate.MinDate
                CalculationEndDate.Value = CalculationEndDate.MaxDate
            Else
                PreviousCalculationStartDate.Text = MinDateCover.ToString(DateFormat)
                PreviousCalculationEndDate.Text = MaxDateCover.ToString(DateFormat)

                CalculationStartDate.Value = MaxDateCover
                If Not CalculationStartDate.Value = CalculationStartDate.MaxDate Then
                    CalculationStartDate.Value = CalculationStartDate.Value.AddDays(1)
                Else
                    ProgressText.Text = "Dataset is up-to-date."
                    ProgressText.Visible = True
                End If
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

        For I = 0 To CoverProperties.Length - 1
            If CoverList.Items(I).Checked Then CoverPropertiesList.Add(CoverProperties(I))
        Next

        If CoverPropertiesList.Count = 0 Then
            MsgBox("Use must select at least one cover for which to calculate potential evapotranspiration.")
            Exit Sub
        End If

        If Not BackgroundWorker.IsBusy Then
            BackgroundWorker.WorkerReportsProgress = True
            BackgroundWorker.WorkerSupportsCancellation = True

            CoverSelectionGroup.Enabled = False
            DatesGroup.Enabled = False
            CalculateButton.Enabled = False

            ProgressText.Text = "Initializing calculation datasets..."
            ProgressBar.Minimum = 0
            ProgressBar.Maximum = CoverPropertiesList.Count
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
            RemoveHandler Me.FormClosing, AddressOf Calculate_Evapotranspiration_FormClosing
            Me.Close()
        End If
    End Sub

#End Region

#Region "Background Execution"

    WithEvents BackgroundWorker As New System.ComponentModel.BackgroundWorker
    Private Timer As Stopwatch
    Private CoverPropertiesList As New List(Of CoverProperties)

    Private Sub BackgroundWorker_DoWork(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker.DoWork
        CalculatePotentialEvapotranspiration(CoverPropertiesList.ToArray, CalculationStartDate.Value, CalculationEndDate.Value, BackgroundWorker, e)
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