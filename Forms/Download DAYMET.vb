'            Copyright Clayton S. Lewis 2014-2015.
'   Distributed under the Boost Software License, Version 1.0.
'      (See accompanying file GridET License.rtf or copy at
'            http://www.boost.org/LICENSE_1_0.txt)

Public Class Download_DAYMET

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

            Using Connection = CreateConnection(DAYMETRastersPath, False)
                Connection.Open()

                Using Command = Connection.CreateCommand
                    Command.CommandText = "CREATE TABLE IF NOT EXISTS Rasters (Date NUMERIC UNIQUE, Image BLOB)"
                    Command.ExecuteNonQuery()

                    'Determine DAYMET Date Span for Download
                    Dim Files = DownloadFtpDirectory(BuildFtpStringDAYMET())
                    Dim Variables As New List(Of String)
                    Dim MaxYear As Integer = DAYMETStartDate.Year

                    For Each File In Files
                        If Not File = "" Then
                            Dim Parts = File.Replace(".nc4", "").Split("_")

                            If Not Variables.Contains(Parts(0)) Then Variables.Add(Parts(0))
                            If Parts(1) > MaxYear Then MaxYear = Parts(1)
                        End If
                    Next

                    Me.Variables = Variables.ToArray
                    Dim ModelEndDate = New DateTime(MaxYear, 12, 31).AddHours(13)

                    WebsiteStartDate.Text = DAYMETStartDate.ToString(DateFormat)
                    WebsiteEndDate.Text = ModelEndDate.ToString(DateFormat)

                    Command.CommandText = "SELECT MIN(Date) FROM Rasters"
                    Dim Value = Command.ExecuteScalar
                    ParameterStartDate = DAYMETStartDate
                    If Not IsDBNull(Value) Then ParameterStartDate = CDate(Value)

                    Command.CommandText = "SELECT MAX(Date) FROM Rasters"
                    Value = Command.ExecuteScalar
                    ParameterEndDate = DAYMETStartDate
                    If Not IsDBNull(Value) Then ParameterEndDate = CDate(Value)

                    If IsDBNull(Value) Or ParameterEndDate < ParameterStartDate Then
                        LocalStartDate.Text = "-"
                        LocalEndDate.Text = "-"
                    Else
                        LocalStartDate.Text = ParameterStartDate.ToString(DateFormat)
                        LocalEndDate.Text = ParameterEndDate.ToString(DateFormat)
                    End If

                    DownloadStartDate.MinDate = DAYMETStartDate
                    DownloadStartDate.MaxDate = ModelEndDate
                    DownloadEndDate.MinDate = DAYMETStartDate
                    DownloadEndDate.MaxDate = ModelEndDate

                    If ParameterEndDate = ModelEndDate Then
                        DownloadStartDate.Value = ParameterEndDate
                        DownloadEndDate.Value = ParameterEndDate

                        ProgressText.Text = "Dataset is up-to-date."
                        ProgressText.Visible = True
                    ElseIf IsDBNull(Value) Then
                        DownloadStartDate.Value = ParameterEndDate
                        DownloadEndDate.Value = ModelEndDate
                    Else
                        DownloadStartDate.Value = ParameterEndDate.AddDays(1)
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

    Private Sub DateTimePicker_ValueChanged(sender As Object, e As System.EventArgs) Handles DownloadStartDate.ValueChanged, DownloadEndDate.ValueChanged
        If DownloadStartDate.Value > DownloadEndDate.Value Then DownloadStartDate.Value = DownloadEndDate.Value
    End Sub

    Private Sub DownloadButton_Click(sender As System.Object, e As System.EventArgs) Handles DownloadButton.Click
        If Cancel_Button.Enabled = False Then
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()
            Exit Sub
        End If

        Dim CalculationWillOverwriteData As Boolean = False
        If DownloadStartDate.Value <= ParameterEndDate And ParameterEndDate <> DAYMETStartDate Then CalculationWillOverwriteData = True

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
            ProgressBar.Maximum = Math.Ceiling(DownloadEndDate.Value.AddDays(1).Subtract(DownloadStartDate.Value).TotalDays)
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

    WithEvents BackgroundWorker As New System.ComponentModel.BackgroundWorker
    Private Timer As Stopwatch
    Private Variables() As String

    Private Sub BackgroundWorker_DoWork(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker.DoWork
        If DownloadStartDate.Value < DownloadEndDate.Value Then DownloadDAYMET(DAYMETRastersPath, DownloadStartDate.Value, DownloadEndDate.Value, Variables(1), BackgroundWorker, e)
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
        ProcessTimerContinue()
    End Sub

    Private Sub ProcessTimerContinue()
        If BackgroundWorker.IsBusy Then ProcessTimer.Start()
    End Sub

#End Region

End Class