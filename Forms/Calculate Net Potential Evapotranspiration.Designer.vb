<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Calculate_Net_Potential_Evapotranspiration
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
        Me.CoverSelectionGroup = New System.Windows.Forms.GroupBox()
        Me.CoverList = New System.Windows.Forms.ListView()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.UncheckAll = New System.Windows.Forms.Button()
        Me.CheckAll = New System.Windows.Forms.Button()
        Me.CalculateButton = New System.Windows.Forms.Button()
        Me.DatesGroup = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.PotentialDatasetEndDate = New System.Windows.Forms.Label()
        Me.PotentialDatasetStartDate = New System.Windows.Forms.Label()
        Me.PreviousCalculationEndDate = New System.Windows.Forms.Label()
        Me.PreviousCalculationStartDate = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.CalculationEndDate = New System.Windows.Forms.DateTimePicker()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.CalculationStartDate = New System.Windows.Forms.DateTimePicker()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.ProgressText = New System.Windows.Forms.Label()
        Me.ProgressBar = New System.Windows.Forms.ProgressBar()
        Me.Cancel_Button = New System.Windows.Forms.Button()
        Me.PrecipitationGroup = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.PrecipitationDataset = New System.Windows.Forms.ComboBox()
        Me.CoverSelectionGroup.SuspendLayout()
        Me.DatesGroup.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.PrecipitationGroup.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'CoverSelectionGroup
        '
        Me.CoverSelectionGroup.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CoverSelectionGroup.Controls.Add(Me.CoverList)
        Me.CoverSelectionGroup.Controls.Add(Me.UncheckAll)
        Me.CoverSelectionGroup.Controls.Add(Me.CheckAll)
        Me.CoverSelectionGroup.Location = New System.Drawing.Point(12, 12)
        Me.CoverSelectionGroup.Name = "CoverSelectionGroup"
        Me.CoverSelectionGroup.Size = New System.Drawing.Size(560, 180)
        Me.CoverSelectionGroup.TabIndex = 8
        Me.CoverSelectionGroup.TabStop = False
        Me.CoverSelectionGroup.Text = "Cover Selection"
        '
        'CoverList
        '
        Me.CoverList.Alignment = System.Windows.Forms.ListViewAlignment.Left
        Me.CoverList.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CoverList.CheckBoxes = True
        Me.CoverList.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1})
        Me.CoverList.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CoverList.FullRowSelect = True
        Me.CoverList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.CoverList.Location = New System.Drawing.Point(18, 32)
        Me.CoverList.MultiSelect = False
        Me.CoverList.Name = "CoverList"
        Me.CoverList.Size = New System.Drawing.Size(443, 121)
        Me.CoverList.TabIndex = 8
        Me.CoverList.UseCompatibleStateImageBehavior = False
        Me.CoverList.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Width = 400
        '
        'UncheckAll
        '
        Me.UncheckAll.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.UncheckAll.Location = New System.Drawing.Point(467, 61)
        Me.UncheckAll.Name = "UncheckAll"
        Me.UncheckAll.Size = New System.Drawing.Size(76, 23)
        Me.UncheckAll.TabIndex = 3
        Me.UncheckAll.Text = "Uncheck All"
        Me.UncheckAll.UseVisualStyleBackColor = True
        '
        'CheckAll
        '
        Me.CheckAll.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CheckAll.Location = New System.Drawing.Point(467, 32)
        Me.CheckAll.Name = "CheckAll"
        Me.CheckAll.Size = New System.Drawing.Size(76, 23)
        Me.CheckAll.TabIndex = 2
        Me.CheckAll.Text = "Check All"
        Me.CheckAll.UseVisualStyleBackColor = True
        '
        'CalculateButton
        '
        Me.CalculateButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CalculateButton.Location = New System.Drawing.Point(397, 482)
        Me.CalculateButton.Margin = New System.Windows.Forms.Padding(3, 9, 3, 9)
        Me.CalculateButton.Name = "CalculateButton"
        Me.CalculateButton.Size = New System.Drawing.Size(76, 23)
        Me.CalculateButton.TabIndex = 7
        Me.CalculateButton.Text = "Calculate"
        Me.CalculateButton.UseVisualStyleBackColor = True
        '
        'DatesGroup
        '
        Me.DatesGroup.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DatesGroup.Controls.Add(Me.TableLayoutPanel1)
        Me.DatesGroup.Location = New System.Drawing.Point(12, 292)
        Me.DatesGroup.Name = "DatesGroup"
        Me.DatesGroup.Padding = New System.Windows.Forms.Padding(3, 6, 3, 3)
        Me.DatesGroup.Size = New System.Drawing.Size(560, 160)
        Me.DatesGroup.TabIndex = 9
        Me.DatesGroup.TabStop = False
        Me.DatesGroup.Text = "Dates"
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.AutoScroll = True
        Me.TableLayoutPanel1.BackColor = System.Drawing.SystemColors.Window
        Me.TableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.OutsetDouble
        Me.TableLayoutPanel1.ColumnCount = 3
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.PotentialDatasetEndDate, 2, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.PotentialDatasetStartDate, 1, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.PreviousCalculationEndDate, 2, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.PreviousCalculationStartDate, 1, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.Label6, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.CalculationEndDate, 2, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.Label3, 0, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.Label5, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.Label4, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.CalculationStartDate, 1, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.Label1, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Label2, 2, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(18, 32)
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
        'PotentialDatasetEndDate
        '
        Me.PotentialDatasetEndDate.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.PotentialDatasetEndDate.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PotentialDatasetEndDate.Location = New System.Drawing.Point(340, 27)
        Me.PotentialDatasetEndDate.Margin = New System.Windows.Forms.Padding(0)
        Me.PotentialDatasetEndDate.Name = "PotentialDatasetEndDate"
        Me.PotentialDatasetEndDate.Size = New System.Drawing.Size(182, 21)
        Me.PotentialDatasetEndDate.TabIndex = 12
        Me.PotentialDatasetEndDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'PotentialDatasetStartDate
        '
        Me.PotentialDatasetStartDate.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.PotentialDatasetStartDate.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PotentialDatasetStartDate.Location = New System.Drawing.Point(156, 27)
        Me.PotentialDatasetStartDate.Margin = New System.Windows.Forms.Padding(0)
        Me.PotentialDatasetStartDate.Name = "PotentialDatasetStartDate"
        Me.PotentialDatasetStartDate.Size = New System.Drawing.Size(181, 21)
        Me.PotentialDatasetStartDate.TabIndex = 12
        Me.PotentialDatasetStartDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'PreviousCalculationEndDate
        '
        Me.PreviousCalculationEndDate.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.PreviousCalculationEndDate.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PreviousCalculationEndDate.Location = New System.Drawing.Point(340, 51)
        Me.PreviousCalculationEndDate.Margin = New System.Windows.Forms.Padding(0)
        Me.PreviousCalculationEndDate.Name = "PreviousCalculationEndDate"
        Me.PreviousCalculationEndDate.Size = New System.Drawing.Size(182, 21)
        Me.PreviousCalculationEndDate.TabIndex = 11
        Me.PreviousCalculationEndDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'PreviousCalculationStartDate
        '
        Me.PreviousCalculationStartDate.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.PreviousCalculationStartDate.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PreviousCalculationStartDate.Location = New System.Drawing.Point(156, 51)
        Me.PreviousCalculationStartDate.Margin = New System.Windows.Forms.Padding(0)
        Me.PreviousCalculationStartDate.Name = "PreviousCalculationStartDate"
        Me.PreviousCalculationStartDate.Size = New System.Drawing.Size(181, 21)
        Me.PreviousCalculationStartDate.TabIndex = 11
        Me.PreviousCalculationStartDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
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
        'CalculationEndDate
        '
        Me.CalculationEndDate.CustomFormat = "MMMM, yyyy"
        Me.CalculationEndDate.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CalculationEndDate.DropDownAlign = System.Windows.Forms.LeftRightAlignment.Right
        Me.CalculationEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.CalculationEndDate.Location = New System.Drawing.Point(340, 75)
        Me.CalculationEndDate.Margin = New System.Windows.Forms.Padding(0)
        Me.CalculationEndDate.Name = "CalculationEndDate"
        Me.CalculationEndDate.Size = New System.Drawing.Size(182, 20)
        Me.CalculationEndDate.TabIndex = 2
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
        Me.Label3.Text = "Current Calculation"
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
        Me.Label5.Text = "Previous Calculation"
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
        Me.Label4.Text = "Potential Dataset"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'CalculationStartDate
        '
        Me.CalculationStartDate.CustomFormat = "MMMM, yyyy"
        Me.CalculationStartDate.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CalculationStartDate.DropDownAlign = System.Windows.Forms.LeftRightAlignment.Right
        Me.CalculationStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.CalculationStartDate.Location = New System.Drawing.Point(156, 75)
        Me.CalculationStartDate.Margin = New System.Windows.Forms.Padding(0)
        Me.CalculationStartDate.Name = "CalculationStartDate"
        Me.CalculationStartDate.Size = New System.Drawing.Size(181, 20)
        Me.CalculationStartDate.TabIndex = 0
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
        Me.ProgressText.Location = New System.Drawing.Point(30, 466)
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
        Me.ProgressBar.Location = New System.Drawing.Point(30, 482)
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
        Me.Cancel_Button.Location = New System.Drawing.Point(479, 482)
        Me.Cancel_Button.Margin = New System.Windows.Forms.Padding(3, 9, 3, 9)
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.Size = New System.Drawing.Size(76, 23)
        Me.Cancel_Button.TabIndex = 34
        Me.Cancel_Button.Text = "Cancel"
        Me.Cancel_Button.UseVisualStyleBackColor = True
        '
        'PrecipitationGroup
        '
        Me.PrecipitationGroup.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PrecipitationGroup.Controls.Add(Me.TableLayoutPanel2)
        Me.PrecipitationGroup.Location = New System.Drawing.Point(12, 198)
        Me.PrecipitationGroup.Name = "PrecipitationGroup"
        Me.PrecipitationGroup.Padding = New System.Windows.Forms.Padding(3, 6, 3, 3)
        Me.PrecipitationGroup.Size = New System.Drawing.Size(560, 88)
        Me.PrecipitationGroup.TabIndex = 35
        Me.PrecipitationGroup.TabStop = False
        Me.PrecipitationGroup.Text = "Precipitation"
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel2.AutoScroll = True
        Me.TableLayoutPanel2.BackColor = System.Drawing.SystemColors.Window
        Me.TableLayoutPanel2.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.OutsetDouble
        Me.TableLayoutPanel2.ColumnCount = 2
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150.0!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.Controls.Add(Me.Label15, 0, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.PrecipitationDataset, 1, 0)
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(18, 32)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 1
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(525, 28)
        Me.TableLayoutPanel2.TabIndex = 4
        '
        'Label15
        '
        Me.Label15.BackColor = System.Drawing.SystemColors.Window
        Me.Label15.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label15.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label15.Location = New System.Drawing.Point(3, 3)
        Me.Label15.Margin = New System.Windows.Forms.Padding(0)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(150, 22)
        Me.Label15.TabIndex = 9
        Me.Label15.Text = "Precipitation Dataset"
        Me.Label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'PrecipitationDataset
        '
        Me.PrecipitationDataset.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PrecipitationDataset.FormattingEnabled = True
        Me.PrecipitationDataset.Location = New System.Drawing.Point(156, 3)
        Me.PrecipitationDataset.Margin = New System.Windows.Forms.Padding(0)
        Me.PrecipitationDataset.Name = "PrecipitationDataset"
        Me.PrecipitationDataset.Size = New System.Drawing.Size(366, 21)
        Me.PrecipitationDataset.TabIndex = 10
        '
        'Calculate_Net_Potential_Evapotranspiration
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(584, 523)
        Me.Controls.Add(Me.PrecipitationGroup)
        Me.Controls.Add(Me.Cancel_Button)
        Me.Controls.Add(Me.ProgressText)
        Me.Controls.Add(Me.ProgressBar)
        Me.Controls.Add(Me.CalculateButton)
        Me.Controls.Add(Me.DatesGroup)
        Me.Controls.Add(Me.CoverSelectionGroup)
        Me.MinimizeBox = False
        Me.Name = "Calculate_Net_Potential_Evapotranspiration"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Net Potential Evapotranspiration Calculation"
        Me.CoverSelectionGroup.ResumeLayout(False)
        Me.DatesGroup.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.PrecipitationGroup.ResumeLayout(False)
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents CoverSelectionGroup As System.Windows.Forms.GroupBox
    Friend WithEvents UncheckAll As System.Windows.Forms.Button
    Friend WithEvents CheckAll As System.Windows.Forms.Button
    Friend WithEvents CalculateButton As System.Windows.Forms.Button
    Friend WithEvents DatesGroup As System.Windows.Forms.GroupBox
    Friend WithEvents CalculationStartDate As System.Windows.Forms.DateTimePicker
    Friend WithEvents CalculationEndDate As System.Windows.Forms.DateTimePicker
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents CoverList As System.Windows.Forms.ListView
    Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents PotentialDatasetEndDate As System.Windows.Forms.Label
    Friend WithEvents PotentialDatasetStartDate As System.Windows.Forms.Label
    Friend WithEvents ProgressText As System.Windows.Forms.Label
    Friend WithEvents ProgressBar As System.Windows.Forms.ProgressBar
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents PreviousCalculationEndDate As System.Windows.Forms.Label
    Friend WithEvents PreviousCalculationStartDate As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents PrecipitationGroup As System.Windows.Forms.GroupBox
    Friend WithEvents TableLayoutPanel2 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents PrecipitationDataset As System.Windows.Forms.ComboBox
End Class
