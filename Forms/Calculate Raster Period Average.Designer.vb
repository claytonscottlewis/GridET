<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Calculate_Raster_Period_Average
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
        Me.ParameterSelectionGroup = New System.Windows.Forms.GroupBox()
        Me.ParameterList = New System.Windows.Forms.ListView()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.UncheckAll = New System.Windows.Forms.Button()
        Me.CheckAll = New System.Windows.Forms.Button()
        Me.CalculateButton = New System.Windows.Forms.Button()
        Me.DatesGroup = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.ParameterDatasetEndDate = New System.Windows.Forms.Label()
        Me.ParameterDatasetStartDate = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.CalculationStartDate = New System.Windows.Forms.DateTimePicker()
        Me.CalculationEndDate = New System.Windows.Forms.DateTimePicker()
        Me.ProgressText = New System.Windows.Forms.Label()
        Me.ProgressBar = New System.Windows.Forms.ProgressBar()
        Me.Cancel_Button = New System.Windows.Forms.Button()
        Me.ParameterSelectionGroup.SuspendLayout()
        Me.DatesGroup.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ParameterSelectionGroup
        '
        Me.ParameterSelectionGroup.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ParameterSelectionGroup.Controls.Add(Me.ParameterList)
        Me.ParameterSelectionGroup.Controls.Add(Me.UncheckAll)
        Me.ParameterSelectionGroup.Controls.Add(Me.CheckAll)
        Me.ParameterSelectionGroup.Location = New System.Drawing.Point(12, 12)
        Me.ParameterSelectionGroup.Name = "ParameterSelectionGroup"
        Me.ParameterSelectionGroup.Size = New System.Drawing.Size(560, 297)
        Me.ParameterSelectionGroup.TabIndex = 8
        Me.ParameterSelectionGroup.TabStop = False
        Me.ParameterSelectionGroup.Text = "Parameter Selection"
        '
        'ParameterList
        '
        Me.ParameterList.Alignment = System.Windows.Forms.ListViewAlignment.Left
        Me.ParameterList.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ParameterList.CheckBoxes = True
        Me.ParameterList.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1})
        Me.ParameterList.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ParameterList.FullRowSelect = True
        Me.ParameterList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.ParameterList.Location = New System.Drawing.Point(18, 32)
        Me.ParameterList.MultiSelect = False
        Me.ParameterList.Name = "ParameterList"
        Me.ParameterList.Size = New System.Drawing.Size(444, 238)
        Me.ParameterList.TabIndex = 8
        Me.ParameterList.UseCompatibleStateImageBehavior = False
        Me.ParameterList.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Width = 400
        '
        'UncheckAll
        '
        Me.UncheckAll.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.UncheckAll.Location = New System.Drawing.Point(468, 61)
        Me.UncheckAll.Name = "UncheckAll"
        Me.UncheckAll.Size = New System.Drawing.Size(75, 23)
        Me.UncheckAll.TabIndex = 3
        Me.UncheckAll.Text = "Uncheck All"
        Me.UncheckAll.UseVisualStyleBackColor = True
        '
        'CheckAll
        '
        Me.CheckAll.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CheckAll.Location = New System.Drawing.Point(468, 32)
        Me.CheckAll.Name = "CheckAll"
        Me.CheckAll.Size = New System.Drawing.Size(75, 23)
        Me.CheckAll.TabIndex = 2
        Me.CheckAll.Text = "Check All"
        Me.CheckAll.UseVisualStyleBackColor = True
        '
        'CalculateButton
        '
        Me.CalculateButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CalculateButton.Location = New System.Drawing.Point(399, 482)
        Me.CalculateButton.Margin = New System.Windows.Forms.Padding(3, 9, 3, 9)
        Me.CalculateButton.Name = "CalculateButton"
        Me.CalculateButton.Size = New System.Drawing.Size(75, 23)
        Me.CalculateButton.TabIndex = 7
        Me.CalculateButton.Text = "Calculate"
        Me.CalculateButton.UseVisualStyleBackColor = True
        '
        'DatesGroup
        '
        Me.DatesGroup.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DatesGroup.Controls.Add(Me.TableLayoutPanel1)
        Me.DatesGroup.Location = New System.Drawing.Point(12, 315)
        Me.DatesGroup.Name = "DatesGroup"
        Me.DatesGroup.Padding = New System.Windows.Forms.Padding(3, 6, 3, 3)
        Me.DatesGroup.Size = New System.Drawing.Size(560, 137)
        Me.DatesGroup.TabIndex = 9
        Me.DatesGroup.TabStop = False
        Me.DatesGroup.Text = "Dates"
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.AutoScroll = True
        Me.TableLayoutPanel1.BackColor = System.Drawing.SystemColors.Window
        Me.TableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.OutsetDouble
        Me.TableLayoutPanel1.ColumnCount = 3
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.ParameterDatasetEndDate, 2, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.ParameterDatasetStartDate, 1, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.Label6, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Label4, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.Label1, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Label2, 2, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Label3, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.CalculationStartDate, 1, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.CalculationEndDate, 2, 2)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(18, 32)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 3
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(525, 77)
        Me.TableLayoutPanel1.TabIndex = 4
        '
        'ParameterDatasetEndDate
        '
        Me.ParameterDatasetEndDate.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.ParameterDatasetEndDate.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ParameterDatasetEndDate.Location = New System.Drawing.Point(340, 27)
        Me.ParameterDatasetEndDate.Margin = New System.Windows.Forms.Padding(0)
        Me.ParameterDatasetEndDate.Name = "ParameterDatasetEndDate"
        Me.ParameterDatasetEndDate.Size = New System.Drawing.Size(182, 21)
        Me.ParameterDatasetEndDate.TabIndex = 12
        Me.ParameterDatasetEndDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ParameterDatasetStartDate
        '
        Me.ParameterDatasetStartDate.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.ParameterDatasetStartDate.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ParameterDatasetStartDate.Location = New System.Drawing.Point(156, 27)
        Me.ParameterDatasetStartDate.Margin = New System.Windows.Forms.Padding(0)
        Me.ParameterDatasetStartDate.Name = "ParameterDatasetStartDate"
        Me.ParameterDatasetStartDate.Size = New System.Drawing.Size(181, 21)
        Me.ParameterDatasetStartDate.TabIndex = 12
        Me.ParameterDatasetStartDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
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
        'Label4
        '
        Me.Label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label4.Location = New System.Drawing.Point(3, 27)
        Me.Label4.Margin = New System.Windows.Forms.Padding(0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(150, 21)
        Me.Label4.TabIndex = 10
        Me.Label4.Text = "Parameter Dataset"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
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
        'Label3
        '
        Me.Label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label3.Location = New System.Drawing.Point(3, 51)
        Me.Label3.Margin = New System.Windows.Forms.Padding(0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(150, 23)
        Me.Label3.TabIndex = 11
        Me.Label3.Text = "Current Calculation"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'CalculationStartDate
        '
        Me.CalculationStartDate.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CalculationStartDate.CustomFormat = "yyyy"
        Me.CalculationStartDate.DropDownAlign = System.Windows.Forms.LeftRightAlignment.Right
        Me.CalculationStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.CalculationStartDate.Location = New System.Drawing.Point(156, 51)
        Me.CalculationStartDate.Margin = New System.Windows.Forms.Padding(0)
        Me.CalculationStartDate.Name = "CalculationStartDate"
        Me.CalculationStartDate.Size = New System.Drawing.Size(181, 20)
        Me.CalculationStartDate.TabIndex = 0
        '
        'CalculationEndDate
        '
        Me.CalculationEndDate.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CalculationEndDate.CustomFormat = "yyyy"
        Me.CalculationEndDate.DropDownAlign = System.Windows.Forms.LeftRightAlignment.Right
        Me.CalculationEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.CalculationEndDate.Location = New System.Drawing.Point(340, 51)
        Me.CalculationEndDate.Margin = New System.Windows.Forms.Padding(0)
        Me.CalculationEndDate.Name = "CalculationEndDate"
        Me.CalculationEndDate.Size = New System.Drawing.Size(182, 20)
        Me.CalculationEndDate.TabIndex = 2
        '
        'ProgressText
        '
        Me.ProgressText.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ProgressText.Location = New System.Drawing.Point(30, 466)
        Me.ProgressText.Margin = New System.Windows.Forms.Padding(3, 21, 3, 0)
        Me.ProgressText.Name = "ProgressText"
        Me.ProgressText.Size = New System.Drawing.Size(525, 13)
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
        Me.ProgressBar.Size = New System.Drawing.Size(363, 23)
        Me.ProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        Me.ProgressBar.TabIndex = 32
        Me.ProgressBar.Visible = False
        '
        'Cancel_Button
        '
        Me.Cancel_Button.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Cancel_Button.Location = New System.Drawing.Point(480, 482)
        Me.Cancel_Button.Margin = New System.Windows.Forms.Padding(3, 9, 3, 9)
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.Size = New System.Drawing.Size(75, 23)
        Me.Cancel_Button.TabIndex = 34
        Me.Cancel_Button.Text = "Cancel"
        Me.Cancel_Button.UseVisualStyleBackColor = True
        '
        'Calculate_Raster_Period_Average
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(584, 523)
        Me.Controls.Add(Me.Cancel_Button)
        Me.Controls.Add(Me.ProgressText)
        Me.Controls.Add(Me.ProgressBar)
        Me.Controls.Add(Me.CalculateButton)
        Me.Controls.Add(Me.DatesGroup)
        Me.Controls.Add(Me.ParameterSelectionGroup)
        Me.MinimizeBox = False
        Me.Name = "Calculate_Raster_Period_Average"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Raster Period Average Calculation"
        Me.ParameterSelectionGroup.ResumeLayout(False)
        Me.DatesGroup.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ParameterSelectionGroup As System.Windows.Forms.GroupBox
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
    Friend WithEvents ParameterList As System.Windows.Forms.ListView
    Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ParameterDatasetEndDate As System.Windows.Forms.Label
    Friend WithEvents ParameterDatasetStartDate As System.Windows.Forms.Label
    Friend WithEvents ProgressText As System.Windows.Forms.Label
    Friend WithEvents ProgressBar As System.Windows.Forms.ProgressBar
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
End Class
