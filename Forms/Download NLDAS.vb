Public Class Download_NLDAS

#Region "Dates"

    Private DateFormat As String = "MMMM dd, yyyy"
    Private ParameterStartDate As DateTime = Nothing
    Private ParameterEndDate As DateTime = Nothing

    Private Sub Form_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        For Each Control In Me.Controls
            GetType(Control).InvokeMember("DoubleBuffered", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance Or Reflection.BindingFlags.SetProperty, Nothing, Control, New Object() {True})
        Next

        If IO.Directory.Exists(ClimateModelDirectory) Then
            DatesGroup.Enabled = True

            Using Connection = CreateConnection(NLDAS_2ARastersPath, False)
                Connection.Open()

                Using Command = Connection.CreateCommand
                    Command.CommandText = "CREATE TABLE IF NOT EXISTS Rasters (Date NUMERIC UNIQUE, Image BLOB)"
                    Command.ExecuteNonQuery()

                    'Determine NLDAS Date Span for Download
                    Dim Years = DownloadFtpDirectory(BuildFtpStringNLDAS_2A(NLDAS_2AStartDate, 1))
                    Dim MaxYear As Integer = NLDAS_2AStartDate.Year
                    For Each Yr In Years
                        If IsNumeric(Yr) Then If Yr > MaxYear Then MaxYear = Yr
                    Next

                    Dim DaysOfYear = DownloadFtpDirectory(BuildFtpStringNLDAS_2A(New DateTime(MaxYear, 1, 1), 2))
                    Dim MaxDayOfYear As Integer = 0
                    For Each DoY In DaysOfYear
                        If IsNumeric(DoY) Then If DoY > MaxDayOfYear Then MaxDayOfYear = DoY
                    Next

                    Dim Hours = DownloadFtpDirectory(BuildFtpStringNLDAS_2A(New DateTime(MaxYear - 1, 12, 31).AddDays(MaxDayOfYear), 3))
                    Dim MaxHour As Integer = 0
                    For Each Hr In Hours
                        Dim FileName = Hr.Split(".")
                        If FileName.Length > 1 Then If FileName(2) > MaxHour Then MaxHour = FileName(2)
                    Next
                    MaxHour /= 100
                    Dim ModelEndDate = New DateTime(MaxYear - 1, 12, 31).AddDays(MaxDayOfYear).AddHours(-MaxHour)

                    WebsiteStartDate.Text = NLDAS_2AStartDate.ToString(DateFormat)
                    WebsiteEndDate.Text = ModelEndDate.ToString(DateFormat)

                    Command.CommandText = "SELECT MIN(Date) FROM Rasters"
                    Dim Value = Command.ExecuteScalar
                    ParameterStartDate = NLDAS_2AStartDate
                    If Not IsDBNull(Value) Then ParameterStartDate = CDate(Value)

                    Command.CommandText = "SELECT MAX(Date) FROM Rasters"
                    Value = Command.ExecuteScalar
                    ParameterEndDate = NLDAS_2AStartDate
                    If Not IsDBNull(Value) Then
                        ParameterEndDate = CDate(Value)
                        If ParameterEndDate.Hour <> 12 Then
                            ParameterEndDate = ParameterEndDate.AddDays(-2)
                            If ParameterEndDate < NLDAS_2AStartDate Then ParameterEndDate = NLDAS_2AStartDate.AddHours(-2)
                        Else
                            ParameterEndDate = ParameterEndDate.AddDays(-1)
                        End If
                    End If

                    If IsDBNull(Value) Or ParameterEndDate.Subtract(ParameterStartDate).TotalHours < -1 Then
                        LocalStartDate.Text = "-"
                        LocalEndDate.Text = "-"
                    Else
                        LocalStartDate.Text = ParameterStartDate.ToString(DateFormat)
                        LocalEndDate.Text = ParameterEndDate.ToString(DateFormat)
                    End If

                    DownloadStartDate.MinDate = NLDAS_2AStartDate
                    DownloadStartDate.MaxDate = ModelEndDate
                    DownloadEndDate.MinDate = NLDAS_2AStartDate
                    DownloadEndDate.MaxDate = ModelEndDate

                    If ParameterEndDate = ModelEndDate Then
                        DownloadStartDate.Value = ParameterEndDate
                        DownloadEndDate.Value = ParameterEndDate

                        ProgressText.Text = "Dataset is up-to-date."
                        ProgressText.Visible = True
                    ElseIf IsDBNull(Value) Or ParameterEndDate.Subtract(ParameterStartDate).TotalHours < -1 Then
                        DownloadStartDate.Value = ParameterEndDate.Date.AddHours(13)
                        DownloadEndDate.Value = ModelEndDate
                    Else
                        DownloadStartDate.Value = ParameterEndDate.AddHours(24)
                        DownloadEndDate.Value = ModelEndDate
                    End If

                End Using
            End Using
        Else
            DatesGroup.Enabled = False
            DownloadButton.Enabled = False
        End If
    End Sub

    Private Sub Form_Closing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = BackgroundWorker.IsBusy
        Cancel_Button_Click(Nothing, Nothing)
    End Sub

    Private Sub DownloadButton_Click(sender As System.Object, e As System.EventArgs) Handles DownloadButton.Click
        If Cancel_Button.Enabled = False Then
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()
            Exit Sub
        End If

        Dim CalculationWillOverwriteData As Boolean = False
        If DownloadStartDate.Value <= ParameterEndDate And ParameterEndDate <> NLDAS_2AStartDate Then CalculationWillOverwriteData = True

        If CalculationWillOverwriteData Then
            If MsgBox("The selected time period will overwrite previous data.  Continue with action anyway?", MsgBoxStyle.YesNo) = MsgBoxResult.No Then
                Exit Sub
            End If
        End If

        If Not BackgroundWorker.IsBusy Then
            BackgroundWorker.WorkerReportsProgress = True
            BackgroundWorker.WorkerSupportsCancellation = True

            DatesGroup.Enabled = False
            DownloadButton.Enabled = False

            ProgressText.Text = "Preparing to download..."
            ProgressBar.Minimum = 0
            ProgressBar.Maximum = Math.Ceiling(DownloadEndDate.Value.AddDays(1).Subtract(DownloadStartDate.Value).TotalHours / DownloadCount)
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
            RemoveHandler Me.FormClosing, AddressOf Form_Closing
            Me.Close()
        End If
    End Sub

#End Region

#Region "Background Execution"

    Public WithEvents BackgroundWorker As New System.ComponentModel.BackgroundWorker
    Private Timer As Stopwatch
    Private DownloadCount As Integer = 8

    Private Sub BackgroundWorker_DoWork(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker.DoWork
        Dim StartDate = New DateTime(DownloadStartDate.Value.Year, DownloadStartDate.Value.Month, DownloadStartDate.Value.Day).AddHours(13)
        Dim EndDate = New DateTime(DownloadEndDate.Value.Year, DownloadEndDate.Value.Month, DownloadEndDate.Value.Day).AddDays(1).AddHours(12)

        If StartDate < EndDate Then DownloadNDLAS_2A(NLDAS_2ARastersPath, StartDate, EndDate, DownloadCount, BackgroundWorker, e)
    End Sub

    Private Sub BackgroundWorker_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorker.ProgressChanged
        If ProgressBar.Maximum - ProgressBar.Value > 1 Then
            Dim Timespan As TimeSpan = New TimeSpan(Timer.Elapsed.Ticks / (ProgressBar.Value + 1) * (ProgressBar.Maximum - ProgressBar.Value - 1))
            ProgressText.Text = String.Format("Estimated time remaining...({0})", Timespan.ToString("d\.hh\:mm\:ss"))
        Else
            ProgressBar.Text = "Checking topography..."
        End If

        If ProgressBar.Value < ProgressBar.Maximum Then ProgressBar.Value += 1
    End Sub

    Private Sub BackgroundWorker_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker.RunWorkerCompleted
        If e.Cancelled Then
            ProgressText.Text = "Image downloads cancelled"
        ElseIf e.Error IsNot Nothing Then
            ProgressText.Text = "There was an error in downloading"
        Else
            Timer.Stop()
            ProgressText.Text = "The download operation was successfull"
        End If
        ProgressText.Text &= String.Format(" ({0}).", Timer.Elapsed.ToString())

        DownloadButton.Enabled = True
        DownloadButton.Text = "OK"
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
        Form_Load(Nothing, Nothing)
    End Sub

    Public Sub RunScheduledProcess()
        DownloadButton_Click(Nothing, Nothing)

        ProcessTimer = New Timer With {.Interval = 1000}
        ProcessTimer.Start()
    End Sub

    Public Sub CancelScheduledProcess()
        Cancel_Button_Click(Nothing, Nothing)
    End Sub

    Private Sub ProcessTimer_Tick(sender As Object, e As System.EventArgs) Handles ProcessTimer.Tick
        If BackgroundWorker.IsBusy Then ProcessTimerContinue()
    End Sub

    Private Sub ProcessTimerContinue()
        If BackgroundWorker.IsBusy Then ProcessTimer.Start()
    End Sub

#End Region

  
End Class