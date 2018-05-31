<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Extract_by_Polygon
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
        Me.CalculateButton = New System.Windows.Forms.Button()
        Me.ProgressText = New System.Windows.Forms.Label()
        Me.ProgressBar = New System.Windows.Forms.ProgressBar()
        Me.Cancel_Button = New System.Windows.Forms.Button()
        Me.InputPolygonGroup = New System.Windows.Forms.GroupBox()
        Me.CoverRelateField = New System.Windows.Forms.ComboBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.InputPolygonAdd = New System.Windows.Forms.Button()
        Me.InputPolygonPath = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.PolygonRasterRelationGroup = New System.Windows.Forms.GroupBox()
        Me.RelationGrid = New System.Windows.Forms.DataGridView()
        Me.Column1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column2 = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.CalculationPeriod = New System.Windows.Forms.ComboBox()
        Me.OutputPolygonGroup = New System.Windows.Forms.GroupBox()
        Me.OutputPolygonSet = New System.Windows.Forms.Button()
        Me.OutputPolygonPath = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.InputPolygonGroup.SuspendLayout()
        Me.PolygonRasterRelationGroup.SuspendLayout()
        CType(Me.RelationGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.OutputPolygonGroup.SuspendLayout()
        Me.SuspendLayout()
        '
        'CalculateButton
        '
        Me.CalculateButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CalculateButton.Location = New System.Drawing.Point(398, 482)
        Me.CalculateButton.Margin = New System.Windows.Forms.Padding(3, 9, 3, 9)
        Me.CalculateButton.Name = "CalculateButton"
        Me.CalculateButton.Size = New System.Drawing.Size(75, 23)
        Me.CalculateButton.TabIndex = 7
        Me.CalculateButton.Text = "Calculate"
        Me.CalculateButton.UseVisualStyleBackColor = True
        '
        'ProgressText
        '
        Me.ProgressText.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ProgressText.Location = New System.Drawing.Point(30, 466)
        Me.ProgressText.Margin = New System.Windows.Forms.Padding(3, 21, 3, 0)
        Me.ProgressText.Name = "ProgressText"
        Me.ProgressText.Size = New System.Drawing.Size(524, 13)
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
        Me.ProgressBar.Size = New System.Drawing.Size(362, 23)
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
        Me.Cancel_Button.Size = New System.Drawing.Size(75, 23)
        Me.Cancel_Button.TabIndex = 34
        Me.Cancel_Button.Text = "Cancel"
        Me.Cancel_Button.UseVisualStyleBackColor = True
        '
        'InputPolygonGroup
        '
        Me.InputPolygonGroup.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.InputPolygonGroup.Controls.Add(Me.CoverRelateField)
        Me.InputPolygonGroup.Controls.Add(Me.Label5)
        Me.InputPolygonGroup.Controls.Add(Me.InputPolygonAdd)
        Me.InputPolygonGroup.Controls.Add(Me.InputPolygonPath)
        Me.InputPolygonGroup.Controls.Add(Me.Label8)
        Me.InputPolygonGroup.Location = New System.Drawing.Point(12, 12)
        Me.InputPolygonGroup.Name = "InputPolygonGroup"
        Me.InputPolygonGroup.Size = New System.Drawing.Size(560, 134)
        Me.InputPolygonGroup.TabIndex = 35
        Me.InputPolygonGroup.TabStop = False
        Me.InputPolygonGroup.Text = "Input Polygon Dataset"
        '
        'CoverRelateField
        '
        Me.CoverRelateField.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CoverRelateField.FormattingEnabled = True
        Me.CoverRelateField.Location = New System.Drawing.Point(18, 95)
        Me.CoverRelateField.Name = "CoverRelateField"
        Me.CoverRelateField.Size = New System.Drawing.Size(443, 21)
        Me.CoverRelateField.TabIndex = 27
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(18, 79)
        Me.Label5.Margin = New System.Windows.Forms.Padding(3, 12, 3, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(94, 13)
        Me.Label5.TabIndex = 24
        Me.Label5.Text = "Cover Relate Field"
        '
        'InputPolygonAdd
        '
        Me.InputPolygonAdd.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.InputPolygonAdd.Location = New System.Drawing.Point(467, 42)
        Me.InputPolygonAdd.Name = "InputPolygonAdd"
        Me.InputPolygonAdd.Size = New System.Drawing.Size(75, 23)
        Me.InputPolygonAdd.TabIndex = 23
        Me.InputPolygonAdd.Text = "Add"
        Me.InputPolygonAdd.UseVisualStyleBackColor = True
        '
        'InputPolygonPath
        '
        Me.InputPolygonPath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.InputPolygonPath.BackColor = System.Drawing.SystemColors.Window
        Me.InputPolygonPath.Location = New System.Drawing.Point(18, 44)
        Me.InputPolygonPath.Name = "InputPolygonPath"
        Me.InputPolygonPath.ReadOnly = True
        Me.InputPolygonPath.Size = New System.Drawing.Size(443, 20)
        Me.InputPolygonPath.TabIndex = 22
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(18, 28)
        Me.Label8.Margin = New System.Windows.Forms.Padding(3, 12, 3, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(183, 13)
        Me.Label8.TabIndex = 21
        Me.Label8.Text = "Path (OGR Readable Vector Format) "
        '
        'PolygonRasterRelationGroup
        '
        Me.PolygonRasterRelationGroup.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PolygonRasterRelationGroup.Controls.Add(Me.RelationGrid)
        Me.PolygonRasterRelationGroup.Controls.Add(Me.Label2)
        Me.PolygonRasterRelationGroup.Controls.Add(Me.CalculationPeriod)
        Me.PolygonRasterRelationGroup.Location = New System.Drawing.Point(12, 152)
        Me.PolygonRasterRelationGroup.Name = "PolygonRasterRelationGroup"
        Me.PolygonRasterRelationGroup.Size = New System.Drawing.Size(560, 211)
        Me.PolygonRasterRelationGroup.TabIndex = 36
        Me.PolygonRasterRelationGroup.TabStop = False
        Me.PolygonRasterRelationGroup.Text = "Polygon-Raster Relation"
        '
        'RelationGrid
        '
        Me.RelationGrid.AllowUserToAddRows = False
        Me.RelationGrid.AllowUserToDeleteRows = False
        Me.RelationGrid.AllowUserToResizeRows = False
        Me.RelationGrid.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.RelationGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.RelationGrid.BackgroundColor = System.Drawing.SystemColors.Window
        Me.RelationGrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.RelationGrid.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.RelationGrid.ColumnHeadersHeight = 28
        Me.RelationGrid.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Column1, Me.Column2})
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.RelationGrid.DefaultCellStyle = DataGridViewCellStyle2
        Me.RelationGrid.Location = New System.Drawing.Point(18, 72)
        Me.RelationGrid.Name = "RelationGrid"
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.RelationGrid.RowHeadersDefaultCellStyle = DataGridViewCellStyle3
        Me.RelationGrid.RowHeadersWidth = 65
        Me.RelationGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.RelationGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me.RelationGrid.ShowCellErrors = False
        Me.RelationGrid.ShowEditingIcon = False
        Me.RelationGrid.ShowRowErrors = False
        Me.RelationGrid.Size = New System.Drawing.Size(443, 121)
        Me.RelationGrid.TabIndex = 28
        '
        'Column1
        '
        Me.Column1.HeaderText = "Cover Relate Field Unique Values"
        Me.Column1.Name = "Column1"
        Me.Column1.ReadOnly = True
        Me.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.Column1.Width = 172
        '
        'Column2
        '
        Me.Column2.HeaderText = "Calculation Period Variables"
        Me.Column2.Name = "Column2"
        Me.Column2.Width = 144
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(18, 28)
        Me.Label2.Margin = New System.Windows.Forms.Padding(3, 12, 3, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(92, 13)
        Me.Label2.TabIndex = 21
        Me.Label2.Text = "Calculation Period"
        '
        'CalculationPeriod
        '
        Me.CalculationPeriod.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CalculationPeriod.FormattingEnabled = True
        Me.CalculationPeriod.Location = New System.Drawing.Point(18, 44)
        Me.CalculationPeriod.Name = "CalculationPeriod"
        Me.CalculationPeriod.Size = New System.Drawing.Size(443, 21)
        Me.CalculationPeriod.TabIndex = 27
        '
        'OutputPolygonGroup
        '
        Me.OutputPolygonGroup.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.OutputPolygonGroup.Controls.Add(Me.OutputPolygonSet)
        Me.OutputPolygonGroup.Controls.Add(Me.OutputPolygonPath)
        Me.OutputPolygonGroup.Controls.Add(Me.Label3)
        Me.OutputPolygonGroup.Location = New System.Drawing.Point(12, 369)
        Me.OutputPolygonGroup.Name = "OutputPolygonGroup"
        Me.OutputPolygonGroup.Size = New System.Drawing.Size(560, 83)
        Me.OutputPolygonGroup.TabIndex = 37
        Me.OutputPolygonGroup.TabStop = False
        Me.OutputPolygonGroup.Text = "Output Polygon Dataset"
        '
        'OutputPolygonSet
        '
        Me.OutputPolygonSet.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.OutputPolygonSet.Location = New System.Drawing.Point(467, 42)
        Me.OutputPolygonSet.Name = "OutputPolygonSet"
        Me.OutputPolygonSet.Size = New System.Drawing.Size(75, 23)
        Me.OutputPolygonSet.TabIndex = 23
        Me.OutputPolygonSet.Text = "Set"
        Me.OutputPolygonSet.UseVisualStyleBackColor = True
        '
        'OutputPolygonPath
        '
        Me.OutputPolygonPath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.OutputPolygonPath.BackColor = System.Drawing.SystemColors.Window
        Me.OutputPolygonPath.Location = New System.Drawing.Point(18, 44)
        Me.OutputPolygonPath.Name = "OutputPolygonPath"
        Me.OutputPolygonPath.ReadOnly = True
        Me.OutputPolygonPath.Size = New System.Drawing.Size(443, 20)
        Me.OutputPolygonPath.TabIndex = 22
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(18, 28)
        Me.Label3.Margin = New System.Windows.Forms.Padding(3, 12, 3, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(85, 13)
        Me.Label3.TabIndex = 21
        Me.Label3.Text = "Path (Shapefile) "
        '
        'Extract_by_Polygon
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(584, 523)
        Me.Controls.Add(Me.OutputPolygonGroup)
        Me.Controls.Add(Me.PolygonRasterRelationGroup)
        Me.Controls.Add(Me.InputPolygonGroup)
        Me.Controls.Add(Me.Cancel_Button)
        Me.Controls.Add(Me.ProgressText)
        Me.Controls.Add(Me.ProgressBar)
        Me.Controls.Add(Me.CalculateButton)
        Me.MinimizeBox = False
        Me.Name = "Extract_by_Polygon"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Polygon Raster Average Extraction Calculation"
        Me.InputPolygonGroup.ResumeLayout(False)
        Me.InputPolygonGroup.PerformLayout()
        Me.PolygonRasterRelationGroup.ResumeLayout(False)
        Me.PolygonRasterRelationGroup.PerformLayout()
        CType(Me.RelationGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.OutputPolygonGroup.ResumeLayout(False)
        Me.OutputPolygonGroup.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents CalculateButton As System.Windows.Forms.Button
    Friend WithEvents ProgressText As System.Windows.Forms.Label
    Friend WithEvents ProgressBar As System.Windows.Forms.ProgressBar
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents InputPolygonGroup As System.Windows.Forms.GroupBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents InputPolygonAdd As System.Windows.Forms.Button
    Friend WithEvents InputPolygonPath As System.Windows.Forms.TextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents CoverRelateField As System.Windows.Forms.ComboBox
    Friend WithEvents PolygonRasterRelationGroup As System.Windows.Forms.GroupBox
    Friend WithEvents CalculationPeriod As System.Windows.Forms.ComboBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents OutputPolygonGroup As System.Windows.Forms.GroupBox
    Friend WithEvents OutputPolygonSet As System.Windows.Forms.Button
    Friend WithEvents OutputPolygonPath As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents RelationGrid As System.Windows.Forms.DataGridView
    Friend WithEvents Column1 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Column2 As System.Windows.Forms.DataGridViewComboBoxColumn
End Class
