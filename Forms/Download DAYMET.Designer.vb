<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Download_DAYMET
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.DownloadButton = New System.Windows.Forms.Button()
        Me.DatesGroup = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.WebsiteEndDate = New System.Windows.Forms.Label()
        Me.WebsiteStartDate = New System.Windows.Forms.Label()
        Me.LocalEndDate = New System.Windows.Forms.Label()
        Me.LocalStartDate = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.DownloadEndDate = New System.Windows.Forms.DateTimePicker()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.DownloadStartDate = New System.Windows.Forms.DateTimePicker()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.ProgressText = New System.Windows.Forms.Label()
        Me.ProgressBar = New System.Windows.Forms.ProgressBar()
        Me.Cancel_Button = New System.Windows.Forms.Button()
        Me.DatesGroup.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'DownloadButton
        '
        Me.DownloadButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DownloadButton.Location = New System.Drawing.Point(397, 207)
        Me.DownloadButton.Margin = New System.Windows.Forms.Padding(3, 9, 3, 9)
        Me.DownloadButton.Name = "DownloadButton"
        Me.DownloadButton.Size = New System.Drawing.Size(76, 23)
        Me.DownloadButton.TabIndex = 7
        Me.DownloadButton.Text = "Download"
        Me.DownloadButton.UseVisualStyleBackColor = True
        '
        'DatesGroup
        '
        Me.DatesGroup.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DatesGroup.Controls.Add(Me.TableLayoutPanel1)
        Me.DatesGroup.Location = New System.Drawing.Point(12, 12)
        Me.DatesGroup.Name = "DatesGroup"
        Me.DatesGroup.Padding = New System.Windows.Forms.Padding(3, 6, 3, 3)
        Me.DatesGroup.Size = New System.Drawing.Size(560, 165)
        Me.DatesGroup.TabIndex = 9
        Me.DatesGroup.TabStop = False
        Me.DatesGroup.Text = "Dates"
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.AutoScroll = True
        Me.TableLayoutPanel1.BackColor = System.Drawing.SystemColors.Window
        Me.TableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.OutsetDouble
        Me.TableLayoutPanel1.ColumnCount = 3
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.WebsiteEndDate, 2, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.WebsiteStartDate, 1, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.LocalEndDate, 2, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.LocalStartDate, 1, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.Label6, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.DownloadEndDate, 2, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.Label3, 0, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.Label5, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.Label4, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.DownloadStartDate, 1, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.Label1, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Label2, 2, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(18, 36)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 4
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(525, 100)
        Me.TableLayoutPanel1.TabIndex = 4
        '
        'WebsiteEndDate
        '
        Me.WebsiteEndDate.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.WebsiteEndDate.Dock = System.Windows.Forms.DockStyle.Fill
        Me.WebsiteEndDate.Location = New System.Drawing.Point(340, 27)
        Me.WebsiteEndDate.Margin = New System.Windows.Forms.Padding(0)
        Me.WebsiteEndDate.Name = "WebsiteEndDate"
        Me.WebsiteEndDate.Size = New System.Drawing.Size(182, 21)
        Me.WebsiteEndDate.TabIndex = 12
        Me.WebsiteEndDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'WebsiteStartDate
        '
        Me.WebsiteStartDate.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.WebsiteStartDate.Dock = System.Windows.Forms.DockStyle.Fill
        Me.WebsiteStartDate.Location = New System.Drawing.Point(156, 27)
        Me.WebsiteStartDate.Margin = New System.Windows.Forms.Padding(0)
        Me.WebsiteStartDate.Name = "WebsiteStartDate"
        Me.WebsiteStartDate.Size = New System.Drawing.Size(181, 21)
        Me.WebsiteStartDate.TabIndex = 12
        Me.WebsiteStartDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LocalEndDate
        '
        Me.LocalEndDate.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LocalEndDate.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LocalEndDate.Location = New System.Drawing.Point(340, 51)
        Me.LocalEndDate.Margin = New System.Windows.Forms.Padding(0)
        Me.LocalEndDate.Name = "LocalEndDate"
        Me.LocalEndDate.Size = New System.Drawing.Size(182, 21)
        Me.LocalEndDate.TabIndex = 11
        Me.LocalEndDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LocalStartDate
        '
        Me.LocalStartDate.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LocalStartDate.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LocalStartDate.Location = New System.Drawing.Point(156, 51)
        Me.LocalStartDate.Margin = New System.Windows.Forms.Padding(0)
        Me.LocalStartDate.Name = "LocalStartDate"
        Me.LocalStartDate.Size = New System.Drawing.Size(181, 21)
        Me.LocalStartDate.TabIndex = 11
        Me.LocalStartDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label6
        '
        Me.Label6.BackColor = System.Drawing.SystemColors.Window
        Me.Label6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label6.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label6.Location = New System.Drawing.Point(3, 3)
        Me.Label6.Margin = New System.Windows.Forms.Padding(0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(150, 21)
        Me.Label6.TabIndex = 10
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'DownloadEndDate
        '
        Me.DownloadEndDate.CustomFormat = "MMMM dd, yyyy"
        Me.DownloadEndDate.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DownloadEndDate.DropDownAlign = System.Windows.Forms.LeftRightAlignment.Right
        Me.DownloadEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DownloadEndDate.Location = New System.Drawing.Point(340, 75)
        Me.DownloadEndDate.Margin = New System.Windows.Forms.Padding(0)
        Me.DownloadEndDate.Name = "DownloadEndDate"
        Me.DownloadEndDate.Size = New System.Drawing.Size(182, 20)
        Me.DownloadEndDate.TabIndex = 2
        '
        'Label3
        '
        Me.Label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label3.Location = New System.Drawing.Point(3, 75)
        Me.Label3.Margin = New System.Windows.Forms.Padding(0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(150, 22)
        Me.Label3.TabIndex = 11
        Me.Label3.Text = "Current Download"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label5
        '
        Me.Label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label5.Location = New System.Drawing.Point(3, 51)
        Me.Label5.Margin = New System.Windows.Forms.Padding(0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(150, 21)
        Me.Label5.TabIndex = 10
        Me.Label5.Text = "Local Directory"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label4
        '
        Me.Label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label4.Location = New System.Drawing.Point(3, 27)
        Me.Label4.Margin = New System.Windows.Forms.Padding(0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(150, 21)
        Me.Label4.TabIndex = 10
        Me.Label4.Text = "Website Directory"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'DownloadStartDate
        '
        Me.DownloadStartDate.CustomFormat = "MMMM dd, yyyy"
        Me.DownloadStartDate.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DownloadStartDate.DropDownAlign = System.Windows.Forms.LeftRightAlignment.Right
        Me.DownloadStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DownloadStartDate.Location = New System.Drawing.Point(156, 75)
        Me.DownloadStartDate.Margin = New System.Windows.Forms.Padding(0)
        Me.DownloadStartDate.Name = "DownloadStartDate"
        Me.DownloadStartDate.Size = New System.Drawing.Size(181, 20)
        Me.DownloadStartDate.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.BackColor = System.Drawing.SystemColors.Window
        Me.Label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label1.Location = New System.Drawing.Point(156, 3)
        Me.Label1.Margin = New System.Windows.Forms.Padding(0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(181, 21)
        Me.Label1.TabIndex = 9
        Me.Label1.Text = "Start Date"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label2
        '
        Me.Label2.BackColor = System.Drawing.SystemColors.Window
        Me.Label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label2.Location = New System.Drawing.Point(340, 3)
        Me.Label2.Margin = New System.Windows.Forms.Padding(0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(182, 21)
        Me.Label2.TabIndex = 10
        Me.Label2.Text = "End Date"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ProgressText
        '
        Me.ProgressText.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ProgressText.Location = New System.Drawing.Point(30, 191)
        Me.ProgressText.Margin = New System.Windows.Forms.Padding(3, 21, 3, 0)
        Me.ProgressText.Name = "ProgressText"
        Me.ProgressText.Size = New System.Drawing.Size(522, 13)
        Me.ProgressText.TabIndex = 33
        Me.ProgressText.Text = "Progress Update Text"
        Me.ProgressText.Visible = False
        '
        'ProgressBar
        '
        Me.ProgressBar.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ProgressBar.Location = New System.Drawing.Point(30, 207)
        Me.ProgressBar.Maximum = 5
        Me.ProgressBar.Name = "ProgressBar"
        Me.ProgressBar.Size = New System.Drawing.Size(361, 23)
        Me.ProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        Me.ProgressBar.TabIndex = 32
        Me.ProgressBar.Visible = False
        '
        'Cancel_Button
        '
        Me.Cancel_Button.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Cancel_Button.Location = New System.Drawing.Point(479, 207)
        Me.Cancel_Button.Margin = New System.Windows.Forms.Padding(3, 9, 3, 9)
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.Size = New System.Drawing.Size(76, 23)
        Me.Cancel_Button.TabIndex = 34
        Me.Cancel_Button.Text = "Cancel"
        Me.Cancel_Button.UseVisualStyleBackColor = True
        '
        'Download_DAYMET
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(584, 248)
        Me.Controls.Add(Me.Cancel_Button)
        Me.Controls.Add(Me.ProgressText)
        Me.Controls.Add(Me.ProgressBar)
        Me.Controls.Add(Me.DownloadButton)
        Me.Controls.Add(Me.DatesGroup)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Download_DAYMET"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Download DAYMET Imagery"
        Me.DatesGroup.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents DownloadButton As System.Windows.Forms.Button
    Friend WithEvents DatesGroup As System.Windows.Forms.GroupBox
    Friend WithEvents DownloadStartDate As System.Windows.Forms.DateTimePicker
    Friend WithEvents DownloadEndDate As System.Windows.Forms.DateTimePicker
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents LocalEndDate As System.Windows.Forms.Label
    Friend WithEvents LocalStartDate As System.Windows.Forms.Label
    Friend WithEvents WebsiteEndDate As System.Windows.Forms.Label
    Friend WithEvents WebsiteStartDate As System.Windows.Forms.Label
    Friend WithEvents ProgressText As System.Windows.Forms.Label
    Friend WithEvents ProgressBar As System.Windows.Forms.ProgressBar
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
End Class
