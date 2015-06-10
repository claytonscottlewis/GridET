'            Copyright Clayton S. Lewis 2014-2015.
'   Distributed under the Boost Software License, Version 1.0.
'      (See accompanying file GridET License.rtf or copy at
'            http://www.boost.org/LICENSE_1_0.txt)

Public Class Process_Scheduler

#Region "Process"

    Private Sub CheckAll_Click(sender As System.Object, e As System.EventArgs) Handles CheckAll.Click
        For Item = 0 To ProcessList.Items.Count - 1
            ProcessList.Items(Item).Checked = True
        Next
    End Sub

    Private Sub UncheckAll_Click(sender As System.Object, e As System.EventArgs) Handles UncheckAll.Click
        For Item = 0 To ProcessList.Items.Count - 1
            ProcessList.Items(Item).Checked = False
        Next
    End Sub

    Enum AutomateProcess
        None
        Download_DAYMET
        Download_NLDAS
        Calculate_Reference_Evapotranspiration
        Calculate_Potential_Evapotranspiration
        Calculate_Net_Potential_Evapotranspiration
    End Enum

#End Region

#Region "Time"

    WithEvents StartProcesses As New Timer
    WithEvents WaitTimer As New Timer
    Private DateFormat As String = "hh mm tt"

    Private Sub Process_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Me.Owner = Main
        Me.StartPosition = FormStartPosition.Manual
        Me.Left = Main.Bounds.X + (Main.Bounds.Width - Me.Width) / 2
        Me.Top = Main.Bounds.Y + (Main.Bounds.Height - Me.Height) / 2

        For Each Control In Me.Controls
            GetType(Control).InvokeMember("DoubleBuffered", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance Or Reflection.BindingFlags.SetProperty, Nothing, Control, New Object() {True})
        Next

        For Each Process In [Enum].GetNames(GetType(AutomateProcess))
            If Process <> "None" Then ProcessList.Items.Add(Process.Replace("_", " "))
        Next

        CheckAll_Click(Nothing, Nothing)

        If Not IO.File.Exists(ProjectDetailsPath) Then
            ProcessGroup.Enabled = False
            TimeGroup.Enabled = False
            StartButton.Enabled = False
        End If
    End Sub

    Private Sub Process_Scheduler_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If StartButton.Enabled And StartButton.Text = "Stop" Then
            e.Cancel = True
            Me.Hide()
        Else
            Me.DialogResult = Windows.Forms.DialogResult.Cancel
        End If
    End Sub

    Private Sub Process_Resize(sender As Object, e As System.EventArgs) Handles Me.Resize
        ProcessList.Columns(0).Width = ProcessList.Width - SystemInformation.VerticalScrollBarWidth - 5
    End Sub

    Private Sub StartButton_Click(sender As System.Object, e As System.EventArgs) Handles StartButton.Click
        If ProcessList.CheckedItems.Count = 0 Then
            MsgBox("Use must select at least one process to schedule.")
            Exit Sub
        End If

        If StartButton.Text = "Start" Then
            BackgroundWorker = New System.ComponentModel.BackgroundWorker
            BackgroundWorker.WorkerReportsProgress = True
            BackgroundWorker.WorkerSupportsCancellation = True

            ProcessGroup.Enabled = False
            TimeGroup.Enabled = False

            StartButton.Text = "Stop"
            Cancel_Button.Text = "Hide"

            ProgressText.Visible = True
            ProgressBar.Visible = True

            ProcessCancelled = False

            Dim CurrentTime = Now

            Dim AddDays As Integer = 0
            If RunTime.Value.Hour + RunTime.Value.Minute / 60 <= CurrentTime.Hour + CurrentTime.Minute / 60 Then AddDays = 1

            StartProcesses.Interval = New DateTime(CurrentTime.Year, CurrentTime.Month, CurrentTime.Day).AddDays(AddDays).AddHours(RunTime.Value.Hour).AddMinutes(RunTime.Value.Minute).Subtract(CurrentTime).TotalMilliseconds
            If 86400000 - StartProcesses.Interval < 60000 Then StartProcesses.Interval = 1
            StartProcesses.Start()

            ProgressText.Text = "Waiting..."
        Else
            ProcessCancelled = True

            If BackgroundWorker.IsBusy = True Then
                ProgressText.Text = "Attempting to cancel..."
                BackgroundWorker.CancelAsync()
            Else
                StartProcesses.Enabled = False
                BackgroundWorker_RunWorkerCompleted(Nothing, New System.ComponentModel.RunWorkerCompletedEventArgs(Nothing, Nothing, True))
            End If
        End If
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        If Cancel_Button.Text = "Hide" Then
            Me.DialogResult = Windows.Forms.DialogResult.None
            Me.Hide()
        Else
            Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Close()
        End If
    End Sub

    Private Sub StartProcesses_Tick(sender As Object, e As System.EventArgs) Handles StartProcesses.Tick
        If BackgroundWorker.IsBusy Then
            WaitTimer.Interval = 10000
            WaitTimer.Start()
        Else
            If WaitTimer.Enabled Then WaitTimer.Stop()
            If StartProcesses.Enabled Then StartProcesses.Stop()

            RunProcessList = New List(Of AutomateProcess)
            For Each Item As ListViewItem In ProcessList.Items
                If Item.Checked Then
                    RunProcessList.Add([Enum].Parse(GetType(AutomateProcess), Item.Text.Replace(" ", "_")))
                Else
                    RunProcessList.Add(AutomateProcess.None)
                End If
            Next

            Timer = New Stopwatch
            Timer.Start()

            BackgroundWorker.RunWorkerAsync()
        End If
    End Sub

    Private Sub WaitTimer_Tick(sender As Object, e As System.EventArgs) Handles WaitTimer.Tick
        StartProcesses_Tick(Nothing, Nothing)
    End Sub

#End Region

#Region "Background Execution"

    WithEvents BackgroundWorker As New System.ComponentModel.BackgroundWorker
    Private Timer As Stopwatch
    Private ProcessForm As Object
    Private RunProcessList As List(Of AutomateProcess)
    Private ProcessOrdinal As Integer
    Private LastProcessDate As DateTime = DateTime.MinValue
    Private ProcessCancelled As Boolean = False

    Private Sub BackgroundWorker_DoWork(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker.DoWork
        LastProcessDate = Now

        For I = 0 To RunProcessList.Count - 1
            If RunProcessList(I) <> AutomateProcess.None Then
                ProcessOrdinal = I
                ProcessForm = Nothing

                Select Case RunProcessList(I)
                    Case AutomateProcess.Download_DAYMET
                        ProcessForm = New Download_DAYMET
                    Case AutomateProcess.Download_NLDAS
                        ProcessForm = New Download_NLDAS
                    Case AutomateProcess.Calculate_Reference_Evapotranspiration
                        ProcessForm = New Calculate_Reference_Evapotranspiration
                    Case AutomateProcess.Calculate_Potential_Evapotranspiration
                        ProcessForm = New Calculate_Potential_Evapotranspiration
                    Case AutomateProcess.Calculate_Net_Potential_Evapotranspiration
                        ProcessForm = Calculate_Net_Potential_Evapotranspiration
                End Select

                If ProcessForm IsNot Nothing Then
                    BackgroundWorker.ReportProgress(0)

                    ProcessForm.LoadScheduledProcess()

                    If CType(ProcessForm.Progress, ProgressValues).ProgressText <> "Dataset is up-to-date." Then
                        ProcessForm.RunScheduledProcess()

                        Do Until Not ProcessForm.BackgroundWorker.IsBusy
                            If ProcessCancelled Then
                                If Not ProcessForm.BackgroundWorker.CancellationPending Then ProcessForm.BackgroundWorker.CancelAsync()
                            End If
                            If Not BackgroundWorker.CancellationPending Then BackgroundWorker.ReportProgress(0)
                            Application.DoEvents()

                            Threading.Thread.Sleep(1000)
                        Loop
                    End If
                End If

                If ProcessCancelled Then Exit Sub
            End If
        Next
    End Sub

    Private Sub BackgroundWorker_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorker.ProgressChanged
        ProgressText.Text = RunProcessList(ProcessOrdinal).ToString.Replace("_", " ") & ":  "

        If ProcessForm IsNot Nothing Then
            Dim ProgressValues = CType(ProcessForm.Progress, ProgressValues)
            ProgressBar.Minimum = ProgressValues.ProgressBarMinValue
            ProgressBar.Maximum = ProgressValues.ProgressBarMaxValue
            ProgressBar.Value = ProgressValues.ProgressBarValue
            If ProgressValues.ProgressText.Contains("Progress Update Text") Then
                ProgressText.Text &= "Loading..."
            Else
                ProgressText.Text &= ProgressValues.ProgressText
            End If
        End If
    End Sub

    Private Sub BackgroundWorker_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker.RunWorkerCompleted
        If ProcessCancelled Then
            ProgressBar.Visible = False

            ProcessGroup.Enabled = True
            TimeGroup.Enabled = True

            StartButton.Text = "Start"
            Cancel_Button.Text = "Cancel"
        End If

        If e.Cancelled Or ProcessCancelled Then
            ProgressText.Text = "Run cancelled"
        ElseIf e.Error IsNot Nothing Then
            ProgressText.Text = "There was an error in the process"
        Else
            Timer.Stop()
            ProgressText.Text = "Last run successfully completed"
        End If
        ProgressText.Text &= String.Format(" ({0}).", Timer.Elapsed.ToString())

        If e.Cancelled Or ProcessCancelled Or e.Error IsNot Nothing Then
            Me.Show()
        Else
            Dim CurrentTime = Now
            If CurrentTime.Subtract(LastProcessDate).TotalDays >= 1 Then
                StartProcesses_Tick(Nothing, Nothing)
            Else
                StartProcesses.Interval = New DateTime(CurrentTime.Year, CurrentTime.Month, CurrentTime.Day).AddDays(1).AddHours(RunTime.Value.Hour).AddMinutes(RunTime.Value.Minute).Subtract(CurrentTime).TotalMilliseconds
                StartProcesses.Start()
            End If
        End If
    End Sub

#End Region

End Class