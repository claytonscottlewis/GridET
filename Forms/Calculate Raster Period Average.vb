'            Copyright Clayton S. Lewis 2014-2015.
'   Distributed under the Boost Software License, Version 1.0.
'      (See accompanying file GridET License.rtf or copy at
'            http://www.boost.org/LICENSE_1_0.txt)

Public Class Calculate_Raster_Period_Average

#Region "Cover Selection"

    Private Sub CheckAll_Click(sender As System.Object, e As System.EventArgs) Handles CheckAll.Click
        RemoveHandler ParameterList.ItemChecked, AddressOf ParameterList_ItemChecked

        For Item = 0 To ParameterList.Items.Count - 1
            ParameterList.Items(Item).Checked = True
        Next

        AddHandler ParameterList.ItemChecked, AddressOf ParameterList_ItemChecked

        ParameterList_ItemChecked(Nothing, Nothing)
    End Sub

    Private Sub UncheckAll_Click(sender As System.Object, e As System.EventArgs) Handles UncheckAll.Click
        RemoveHandler ParameterList.ItemChecked, AddressOf ParameterList_ItemChecked

        For Item = 0 To ParameterList.Items.Count - 1
            ParameterList.Items(Item).Checked = False
        Next

        AddHandler ParameterList.ItemChecked, AddressOf ParameterList_ItemChecked

        ParameterList_ItemChecked(Nothing, Nothing)
    End Sub

#End Region

#Region "Dates"

    Private DateFormat As String = "yyyy"
    Private ParameterStartDate As New List(Of DateTime)
    Private ParameterEndDate As New List(Of DateTime)
    Private ParameterPath As New List(Of String)
    Private ParameterTableType As New List(Of DatabaseTableName)

    Private Sub Calculate_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        For Each Control In Me.Controls
            GetType(Control).InvokeMember("DoubleBuffered", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance Or Reflection.BindingFlags.SetProperty, Nothing, Control, New Object() {True})
        Next

        If IO.Directory.Exists(IntermediateCalculationsDirectory) Then
            ParameterSelectionGroup.Enabled = True
            DatesGroup.Enabled = True

            RemoveHandler ParameterList.ItemChecked, AddressOf ParameterList_ItemChecked
            For Each Path In IO.Directory.GetFiles(InputVariablesDirectory, "*.db").Concat(IO.Directory.GetFiles(DateVariablesDirectory, "*.db")).Concat(IO.Directory.GetFiles(ReferenceEvapotranspirationDirectory, "*.db")).Concat(IO.Directory.GetFiles(PotentialEvapotranspirationDirectory, "*.db"))
                Dim ParameterName = IO.Path.GetFileNameWithoutExtension(Path)

                Dim MinDate = DateTime.MinValue
                Dim MaxDate = DateTime.MaxValue
                GetMaxAndMinDates({Path}, MaxDate, MinDate, DatabaseTableName.Statistics)

                If Not MaxDate = DateTime.MaxValue Then
                    ParameterStartDate.Add(MinDate)
                    ParameterEndDate.Add(MaxDate)

                    ParameterList.Items.Add(ParameterName)
                    ParameterPath.Add(Path)
                    ParameterTableType.Add(DatabaseTableName.Statistics)

                    If ParameterName.EndsWith(" Potential Evapotranspiration") Then
                        MinDate = DateTime.MinValue
                        MaxDate = DateTime.MaxValue
                        GetMaxAndMinDates({Path}, MaxDate, MinDate, DatabaseTableName.Net)

                        If Not MaxDate = DateTime.MaxValue Then
                            ParameterStartDate.Add(MinDate)
                            ParameterEndDate.Add(MaxDate)

                            ParameterList.Items.Add(ParameterName.Insert(ParameterName.Length - 29, " Net"))
                            ParameterPath.Add(Path)
                            ParameterTableType.Add(DatabaseTableName.Net)
                        End If
                    End If
                End If
            Next
            AddHandler ParameterList.ItemChecked, AddressOf ParameterList_ItemChecked

            If ParameterList.Items.Count > 0 Then
                Dim MinDate As DateTime = ParameterStartDate.Min
                Dim MaxDate As DateTime = ParameterEndDate.Max

                If Not MaxDate = DateTime.MaxValue Then
                    If Not MinDate.Month = 1 And MinDate.Day = 1 Then MinDate = New DateTime(MinDate.Year + 1, 1, 1)
                    If Not MaxDate.Month = 12 And MaxDate.Day = 31 Then MaxDate = New DateTime(MaxDate.Year - 1, 12, 31)

                    If MaxDate < MinDate Then
                        CalculateButton.Enabled = False
                        DatesGroup.Enabled = False
                        Exit Sub
                    End If
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
            Calculate_Resize(Nothing, Nothing)
        Else
            ParameterSelectionGroup.Enabled = False
            DatesGroup.Enabled = False
            CalculateButton.Enabled = False
        End If
    End Sub

    Private Sub Calculate_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = BackgroundWorker.IsBusy
        Cancel_Button_Click(Nothing, Nothing)
    End Sub

    Private Sub Calculate_Resize(sender As Object, e As System.EventArgs) Handles Me.Resize
        ParameterList.Columns(0).Width = ParameterList.Width - SystemInformation.VerticalScrollBarWidth - 5
    End Sub

    Private Sub ParameterList_ItemChecked(sender As Object, e As System.Windows.Forms.ItemCheckedEventArgs) Handles ParameterList.ItemChecked
        Dim CalculationExists = ParameterList.CheckedItems.Count > 0
        DatesGroup.Enabled = CalculationExists
        CalculateButton.Enabled = CalculationExists

        ParameterDatasetStartDate.Text = ""
        ParameterDatasetEndDate.Text = ""

        If CalculationExists Then
            Dim MinDate = DateTime.MinValue
            Dim MaxDate = DateTime.MaxValue
            Dim NoCalculation As Boolean = False

            For Each Item As ListViewItem In ParameterList.CheckedItems
                If ParameterStartDate(Item.Index) > MinDate Then MinDate = ParameterStartDate(Item.Index)
                If ParameterEndDate(Item.Index) < MaxDate Then MaxDate = ParameterEndDate(Item.Index)
                If ParameterEndDate(Item.Index) = DateTime.MaxValue Then NoCalculation = True
            Next
            If NoCalculation Then MaxDate = DateTime.MaxValue

            If Not MinDate.Month = 1 And MinDate.Day = 1 Then MinDate = New DateTime(MinDate.Year + 1, 1, 1)
            If Not MaxDate.Month = 12 And MaxDate.Day = 31 Then MaxDate = New DateTime(MaxDate.Year - 1, 12, 31)
            If MaxDate < MinDate Then NoCalculation = True

            CalculationStartDate.MinDate = MinDate
            CalculationStartDate.MaxDate = MaxDate
            CalculationEndDate.MinDate = MinDate
            CalculationEndDate.MaxDate = MaxDate

            If NoCalculation Then
                DatesGroup.Enabled = False
                CalculateButton.Enabled = False
            Else
                ParameterDatasetStartDate.Text = CalculationStartDate.MinDate.ToString(DateFormat)
                ParameterDatasetEndDate.Text = CalculationEndDate.MaxDate.ToString(DateFormat)

                CalculationStartDate.Value = CalculationStartDate.MinDate
                CalculationEndDate.Value = CalculationEndDate.MaxDate
            End If
        End If
    End Sub

    Private Sub DateTimePicker_ValueChanged(sender As Object, e As System.EventArgs) Handles CalculationStartDate.ValueChanged, CalculationEndDate.ValueChanged
        If CalculationStartDate.Value > CalculationEndDate.Value Then CalculationStartDate.Value = CalculationEndDate.Value
    End Sub

    Private Sub CalculateButton_Click(sender As System.Object, e As System.EventArgs) Handles CalculateButton.Click
        If Cancel_Button.Enabled = False Then
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()
            Exit Sub
        End If

        OutputDirectory = IO.Path.Combine(OutputCalculationsDirectory, String.Format("{0}-{1}", CalculationStartDate.Value.Year, CalculationEndDate.Value.Year))
        If Not IO.Directory.Exists(OutputDirectory) Then
            IO.Directory.CreateDirectory(OutputDirectory)
        Else
            Dim ExistingParameters = IO.Directory.GetFiles(OutputDirectory, "*.tif")
            For I = 0 To ExistingParameters.Length - 1
                Dim Parameter = IO.Path.GetFileNameWithoutExtension(ExistingParameters(I))
                ExistingParameters(I) = Parameter.Substring(0, Parameter.LastIndexOf("(") - 1)
            Next

            For Each Parameter As ListViewItem In ParameterList.CheckedItems
                If ExistingParameters.Contains(Parameter.Text) Then
                    If MsgBox("The selected time period will overwrite previous calculations.  Continue with action anyway?", MsgBoxStyle.YesNo) = MsgBoxResult.No Then
                        Exit Sub
                    End If
                    Exit For
                End If
            Next
        End If

        ReDim DatabasePaths(ParameterList.CheckedItems.Count - 1)
        ReDim TableNames(ParameterList.CheckedItems.Count - 1)

        Dim J As Integer = 0
        For I = 0 To ParameterList.Items.Count - 1
            If ParameterList.Items(I).Checked Then
                DatabasePaths(J) = ParameterPath(I)
                TableNames(J) = ParameterTableType(I)
                J += 1
            End If
        Next

        If Not BackgroundWorker.IsBusy Then
            BackgroundWorker.WorkerReportsProgress = True
            BackgroundWorker.WorkerSupportsCancellation = True

            ParameterSelectionGroup.Enabled = False
            DatesGroup.Enabled = False
            CalculateButton.Enabled = False

            ProgressText.Text = "Initializing calculation datasets..."
            ProgressBar.Minimum = 0
            ProgressBar.Maximum = DatabasePaths.Length * 14
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
    Private Timer As Stopwatch
    Private DatabasePaths() As String
    Private TableNames() As DatabaseTableName
    Private OutputDirectory As String = ""

    Private Sub BackgroundWorker_DoWork(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker.DoWork
        CalculateRasterPeriodAverages(DatabasePaths, TableNames, OutputDirectory, CalculationStartDate.Value.Year, CalculationEndDate.Value.Year, BackgroundWorker, e)
    End Sub

    Private Sub BackgroundWorker_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorker.ProgressChanged
        If ProgressBar.Value < ProgressBar.Maximum Then
            ProgressBar.Value += 1
            Dim Timespan As TimeSpan = New TimeSpan(Timer.Elapsed.Ticks * (ProgressBar.Maximum / ProgressBar.Value - 1))
            ProgressText.Text = String.Format("Estimated time remaining...({0})", Timespan.ToString("d\.hh\:mm\:ss"))
        End If
    End Sub

    Private Sub BackgroundWorker_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker.RunWorkerCompleted
        If e.Cancelled Then
            ProgressText.Text = "Raster period average calculations cancelled"
        ElseIf e.Error IsNot Nothing Then
            ProgressText.Text = "There was an error in the calculation process"
        Else
            Timer.Stop()
            ProgressText.Text = "Raster period average calculations were successfully completed"
        End If
        ProgressText.Text &= String.Format(" ({0}).", Timer.Elapsed.ToString())

        CalculateButton.Enabled = True
        CalculateButton.Text = "OK"
        Cancel_Button.Enabled = False
    End Sub

#End Region

End Class