<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Cover_Curves
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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle7 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle8 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.NameGroup = New System.Windows.Forms.GroupBox()
        Me.Rename = New System.Windows.Forms.Button()
        Me.Down = New System.Windows.Forms.Button()
        Me.Up = New System.Windows.Forms.Button()
        Me.Remove = New System.Windows.Forms.Button()
        Me.Add = New System.Windows.Forms.Button()
        Me.CurveListBox = New System.Windows.Forms.ListBox()
        Me.VariableGroup = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.InitialPeriodGrid = New System.Windows.Forms.DataGridView()
        Me.DataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn2 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn3 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.FinalPeriodGrid = New System.Windows.Forms.DataGridView()
        Me.Column1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column2 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column3 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.CurveTypePeriod2 = New System.Windows.Forms.ComboBox()
        Me.CurveTypePeriod1 = New System.Windows.Forms.ComboBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.BaseVariable = New System.Windows.Forms.ComboBox()
        Me.SeasonalCurveType = New System.Windows.Forms.ComboBox()
        Me.Save = New System.Windows.Forms.Button()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.NameGroup.SuspendLayout()
        Me.VariableGroup.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        CType(Me.InitialPeriodGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.FinalPeriodGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.IsSplitterFixed = True
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Margin = New System.Windows.Forms.Padding(0)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.NameGroup)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.VariableGroup)
        Me.SplitContainer1.Size = New System.Drawing.Size(584, 562)
        Me.SplitContainer1.SplitterDistance = 225
        Me.SplitContainer1.SplitterWidth = 1
        Me.SplitContainer1.TabIndex = 10
        '
        'NameGroup
        '
        Me.NameGroup.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.NameGroup.Controls.Add(Me.Rename)
        Me.NameGroup.Controls.Add(Me.Down)
        Me.NameGroup.Controls.Add(Me.Up)
        Me.NameGroup.Controls.Add(Me.Remove)
        Me.NameGroup.Controls.Add(Me.Add)
        Me.NameGroup.Controls.Add(Me.CurveListBox)
        Me.NameGroup.Location = New System.Drawing.Point(12, 12)
        Me.NameGroup.Name = "NameGroup"
        Me.NameGroup.Size = New System.Drawing.Size(560, 208)
        Me.NameGroup.TabIndex = 9
        Me.NameGroup.TabStop = False
        Me.NameGroup.Text = "Curve Name"
        '
        'Rename
        '
        Me.Rename.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Rename.Location = New System.Drawing.Point(476, 90)
        Me.Rename.Name = "Rename"
        Me.Rename.Size = New System.Drawing.Size(67, 23)
        Me.Rename.TabIndex = 26
        Me.Rename.Text = "Rename"
        Me.Rename.UseVisualStyleBackColor = True
        '
        'Down
        '
        Me.Down.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Down.Location = New System.Drawing.Point(476, 148)
        Me.Down.Name = "Down"
        Me.Down.Size = New System.Drawing.Size(67, 23)
        Me.Down.TabIndex = 25
        Me.Down.Text = "Down"
        Me.Down.UseVisualStyleBackColor = True
        '
        'Up
        '
        Me.Up.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Up.Location = New System.Drawing.Point(476, 119)
        Me.Up.Name = "Up"
        Me.Up.Size = New System.Drawing.Size(67, 23)
        Me.Up.TabIndex = 24
        Me.Up.Text = "Up"
        Me.Up.UseVisualStyleBackColor = True
        '
        'Remove
        '
        Me.Remove.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Remove.Location = New System.Drawing.Point(476, 61)
        Me.Remove.Name = "Remove"
        Me.Remove.Size = New System.Drawing.Size(67, 23)
        Me.Remove.TabIndex = 23
        Me.Remove.Text = "Remove"
        Me.Remove.UseVisualStyleBackColor = True
        '
        'Add
        '
        Me.Add.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Add.Location = New System.Drawing.Point(476, 32)
        Me.Add.Name = "Add"
        Me.Add.Size = New System.Drawing.Size(67, 23)
        Me.Add.TabIndex = 22
        Me.Add.Text = "Add"
        Me.Add.UseVisualStyleBackColor = True
        '
        'CurveListBox
        '
        Me.CurveListBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CurveListBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CurveListBox.FormattingEnabled = True
        Me.CurveListBox.ItemHeight = 16
        Me.CurveListBox.Location = New System.Drawing.Point(18, 32)
        Me.CurveListBox.Name = "CurveListBox"
        Me.CurveListBox.ScrollAlwaysVisible = True
        Me.CurveListBox.Size = New System.Drawing.Size(452, 148)
        Me.CurveListBox.TabIndex = 21
        '
        'VariableGroup
        '
        Me.VariableGroup.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.VariableGroup.Controls.Add(Me.TableLayoutPanel1)
        Me.VariableGroup.Controls.Add(Me.Save)
        Me.VariableGroup.Location = New System.Drawing.Point(12, 0)
        Me.VariableGroup.Name = "VariableGroup"
        Me.VariableGroup.Padding = New System.Windows.Forms.Padding(3, 6, 3, 3)
        Me.VariableGroup.Size = New System.Drawing.Size(559, 308)
        Me.VariableGroup.TabIndex = 10
        Me.VariableGroup.TabStop = False
        Me.VariableGroup.Text = "Curve Variables"
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.AutoScroll = True
        Me.TableLayoutPanel1.BackColor = System.Drawing.SystemColors.Window
        Me.TableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.OutsetDouble
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.Label3, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.InitialPeriodGrid, 0, 5)
        Me.TableLayoutPanel1.Controls.Add(Me.FinalPeriodGrid, 1, 5)
        Me.TableLayoutPanel1.Controls.Add(Me.CurveTypePeriod2, 1, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.CurveTypePeriod1, 0, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.Label9, 0, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.Label4, 1, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.Label2, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Label1, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.BaseVariable, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.SeasonalCurveType, 1, 1)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(18, 62)
        Me.TableLayoutPanel1.Margin = New System.Windows.Forms.Padding(0, 9, 0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 6
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(524, 218)
        Me.TableLayoutPanel1.TabIndex = 30
        '
        'Label3
        '
        Me.Label3.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label3.BackColor = System.Drawing.SystemColors.Control
        Me.Label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.TableLayoutPanel1.SetColumnSpan(Me.Label3, 2)
        Me.Label3.Location = New System.Drawing.Point(3, 51)
        Me.Label3.Margin = New System.Windows.Forms.Padding(0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(518, 21)
        Me.Label3.TabIndex = 33
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'InitialPeriodGrid
        '
        Me.InitialPeriodGrid.AllowUserToAddRows = False
        Me.InitialPeriodGrid.AllowUserToDeleteRows = False
        Me.InitialPeriodGrid.AllowUserToResizeColumns = False
        Me.InitialPeriodGrid.AllowUserToResizeRows = False
        Me.InitialPeriodGrid.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.InitialPeriodGrid.BackgroundColor = System.Drawing.SystemColors.Window
        Me.InitialPeriodGrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.InitialPeriodGrid.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.InitialPeriodGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        Me.InitialPeriodGrid.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.DataGridViewTextBoxColumn1, Me.DataGridViewTextBoxColumn2, Me.DataGridViewTextBoxColumn3})
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.InitialPeriodGrid.DefaultCellStyle = DataGridViewCellStyle2
        Me.InitialPeriodGrid.Location = New System.Drawing.Point(6, 126)
        Me.InitialPeriodGrid.MultiSelect = False
        Me.InitialPeriodGrid.Name = "InitialPeriodGrid"
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.InitialPeriodGrid.RowHeadersDefaultCellStyle = DataGridViewCellStyle3
        Me.InitialPeriodGrid.RowHeadersWidth = 65
        Me.InitialPeriodGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle4.Format = "N3"
        DataGridViewCellStyle4.NullValue = Nothing
        Me.InitialPeriodGrid.RowsDefaultCellStyle = DataGridViewCellStyle4
        Me.InitialPeriodGrid.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.InitialPeriodGrid.RowTemplate.DefaultCellStyle.Format = "N3"
        Me.InitialPeriodGrid.RowTemplate.DefaultCellStyle.NullValue = Nothing
        Me.InitialPeriodGrid.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.InitialPeriodGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me.InitialPeriodGrid.Size = New System.Drawing.Size(251, 86)
        Me.InitialPeriodGrid.TabIndex = 30
        '
        'DataGridViewTextBoxColumn1
        '
        Me.DataGridViewTextBoxColumn1.HeaderText = "Curve 1"
        Me.DataGridViewTextBoxColumn1.Name = "DataGridViewTextBoxColumn1"
        Me.DataGridViewTextBoxColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.DataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'DataGridViewTextBoxColumn2
        '
        Me.DataGridViewTextBoxColumn2.HeaderText = "Curve 2"
        Me.DataGridViewTextBoxColumn2.Name = "DataGridViewTextBoxColumn2"
        Me.DataGridViewTextBoxColumn2.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.DataGridViewTextBoxColumn2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'DataGridViewTextBoxColumn3
        '
        Me.DataGridViewTextBoxColumn3.HeaderText = "Curve 3"
        Me.DataGridViewTextBoxColumn3.Name = "DataGridViewTextBoxColumn3"
        Me.DataGridViewTextBoxColumn3.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.DataGridViewTextBoxColumn3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'FinalPeriodGrid
        '
        Me.FinalPeriodGrid.AllowUserToResizeColumns = False
        Me.FinalPeriodGrid.AllowUserToResizeRows = False
        Me.FinalPeriodGrid.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FinalPeriodGrid.BackgroundColor = System.Drawing.SystemColors.Window
        Me.FinalPeriodGrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.FinalPeriodGrid.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle5
        Me.FinalPeriodGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        Me.FinalPeriodGrid.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Column1, Me.Column2, Me.Column3})
        DataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle6.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.FinalPeriodGrid.DefaultCellStyle = DataGridViewCellStyle6
        Me.FinalPeriodGrid.Location = New System.Drawing.Point(266, 126)
        Me.FinalPeriodGrid.Name = "FinalPeriodGrid"
        DataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle7.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.FinalPeriodGrid.RowHeadersDefaultCellStyle = DataGridViewCellStyle7
        Me.FinalPeriodGrid.RowHeadersWidth = 65
        Me.FinalPeriodGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle8.Format = "N3"
        DataGridViewCellStyle8.NullValue = Nothing
        Me.FinalPeriodGrid.RowsDefaultCellStyle = DataGridViewCellStyle8
        Me.FinalPeriodGrid.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.FinalPeriodGrid.RowTemplate.DefaultCellStyle.Format = "N3"
        Me.FinalPeriodGrid.RowTemplate.DefaultCellStyle.NullValue = Nothing
        Me.FinalPeriodGrid.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.FinalPeriodGrid.Size = New System.Drawing.Size(252, 86)
        Me.FinalPeriodGrid.TabIndex = 31
        '
        'Column1
        '
        Me.Column1.HeaderText = "Curve 1"
        Me.Column1.Name = "Column1"
        Me.Column1.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'Column2
        '
        Me.Column2.HeaderText = "Curve 2"
        Me.Column2.Name = "Column2"
        Me.Column2.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.Column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'Column3
        '
        Me.Column3.HeaderText = "Curve 3"
        Me.Column3.Name = "Column3"
        Me.Column3.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.Column3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'CurveTypePeriod2
        '
        Me.CurveTypePeriod2.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CurveTypePeriod2.BackColor = System.Drawing.SystemColors.Window
        Me.CurveTypePeriod2.FormattingEnabled = True
        Me.CurveTypePeriod2.Location = New System.Drawing.Point(263, 99)
        Me.CurveTypePeriod2.Margin = New System.Windows.Forms.Padding(0)
        Me.CurveTypePeriod2.Name = "CurveTypePeriod2"
        Me.CurveTypePeriod2.Size = New System.Drawing.Size(258, 21)
        Me.CurveTypePeriod2.TabIndex = 30
        '
        'CurveTypePeriod1
        '
        Me.CurveTypePeriod1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CurveTypePeriod1.BackColor = System.Drawing.SystemColors.Window
        Me.CurveTypePeriod1.FormattingEnabled = True
        Me.CurveTypePeriod1.Location = New System.Drawing.Point(3, 99)
        Me.CurveTypePeriod1.Margin = New System.Windows.Forms.Padding(0)
        Me.CurveTypePeriod1.Name = "CurveTypePeriod1"
        Me.CurveTypePeriod1.Size = New System.Drawing.Size(257, 21)
        Me.CurveTypePeriod1.TabIndex = 29
        '
        'Label9
        '
        Me.Label9.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label9.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label9.Location = New System.Drawing.Point(3, 75)
        Me.Label9.Margin = New System.Windows.Forms.Padding(0)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(257, 21)
        Me.Label9.TabIndex = 28
        Me.Label9.Text = "Initial Period Curve Type"
        Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label4
        '
        Me.Label4.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label4.Location = New System.Drawing.Point(263, 75)
        Me.Label4.Margin = New System.Windows.Forms.Padding(0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(258, 21)
        Me.Label4.TabIndex = 29
        Me.Label4.Text = "Final Period Curve Type"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label2
        '
        Me.Label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label2.Location = New System.Drawing.Point(3, 3)
        Me.Label2.Margin = New System.Windows.Forms.Padding(0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(257, 21)
        Me.Label2.TabIndex = 32
        Me.Label2.Text = "Base Atmospheric Variable"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label1
        '
        Me.Label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label1.Location = New System.Drawing.Point(263, 3)
        Me.Label1.Margin = New System.Windows.Forms.Padding(0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(258, 21)
        Me.Label1.TabIndex = 31
        Me.Label1.Text = "Seasonal Curve Type"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'BaseVariable
        '
        Me.BaseVariable.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BaseVariable.BackColor = System.Drawing.SystemColors.Window
        Me.BaseVariable.FormattingEnabled = True
        Me.BaseVariable.Location = New System.Drawing.Point(3, 27)
        Me.BaseVariable.Margin = New System.Windows.Forms.Padding(0)
        Me.BaseVariable.Name = "BaseVariable"
        Me.BaseVariable.Size = New System.Drawing.Size(257, 21)
        Me.BaseVariable.TabIndex = 32
        '
        'SeasonalCurveType
        '
        Me.SeasonalCurveType.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SeasonalCurveType.BackColor = System.Drawing.SystemColors.Window
        Me.SeasonalCurveType.FormattingEnabled = True
        Me.SeasonalCurveType.Location = New System.Drawing.Point(263, 27)
        Me.SeasonalCurveType.Margin = New System.Windows.Forms.Padding(0)
        Me.SeasonalCurveType.Name = "SeasonalCurveType"
        Me.SeasonalCurveType.Size = New System.Drawing.Size(258, 21)
        Me.SeasonalCurveType.TabIndex = 31
        '
        'Save
        '
        Me.Save.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Save.Location = New System.Drawing.Point(475, 27)
        Me.Save.Margin = New System.Windows.Forms.Padding(3, 9, 3, 9)
        Me.Save.Name = "Save"
        Me.Save.Size = New System.Drawing.Size(67, 23)
        Me.Save.TabIndex = 26
        Me.Save.Text = "Save"
        Me.Save.UseVisualStyleBackColor = True
        '
        'Cover_Curves
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(584, 562)
        Me.Controls.Add(Me.SplitContainer1)
        Me.MinimizeBox = False
        Me.Name = "Cover_Curves"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Curve Properties"
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.NameGroup.ResumeLayout(False)
        Me.VariableGroup.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        CType(Me.InitialPeriodGrid, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.FinalPeriodGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents NameGroup As System.Windows.Forms.GroupBox
    Friend WithEvents Rename As System.Windows.Forms.Button
    Friend WithEvents Down As System.Windows.Forms.Button
    Friend WithEvents Up As System.Windows.Forms.Button
    Friend WithEvents Remove As System.Windows.Forms.Button
    Friend WithEvents Add As System.Windows.Forms.Button
    Friend WithEvents CurveListBox As System.Windows.Forms.ListBox
    Friend WithEvents VariableGroup As System.Windows.Forms.GroupBox
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents SeasonalCurveType As System.Windows.Forms.ComboBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents InitialPeriodGrid As System.Windows.Forms.DataGridView
    Friend WithEvents FinalPeriodGrid As System.Windows.Forms.DataGridView
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents CurveTypePeriod2 As System.Windows.Forms.ComboBox
    Friend WithEvents CurveTypePeriod1 As System.Windows.Forms.ComboBox
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Save As System.Windows.Forms.Button
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents BaseVariable As System.Windows.Forms.ComboBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Column1 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Column2 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Column3 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn1 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn2 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn3 As System.Windows.Forms.DataGridViewTextBoxColumn
End Class
