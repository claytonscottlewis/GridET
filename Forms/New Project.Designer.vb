<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class New_Project
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
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.CreateButton = New System.Windows.Forms.Button()
        Me.Cancel_Button = New System.Windows.Forms.Button()
        Me.AreaOfInterestGroup = New System.Windows.Forms.GroupBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Scaling = New System.Windows.Forms.NumericUpDown()
        Me.ResolutionLabel = New System.Windows.Forms.Label()
        Me.Resolution = New System.Windows.Forms.NumericUpDown()
        Me.ElevationAdd = New System.Windows.Forms.Button()
        Me.ElevationDatasetPaths = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.MaskAdd = New System.Windows.Forms.Button()
        Me.MaskDatasetPath = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.FileLocationsGroup = New System.Windows.Forms.GroupBox()
        Me.ClimateModelSet = New System.Windows.Forms.Button()
        Me.ClimateModelDirectory = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.ProjectSet = New System.Windows.Forms.Button()
        Me.ProjectDirectory = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.ProgressBar = New System.Windows.Forms.ProgressBar()
        Me.ProgressText = New System.Windows.Forms.Label()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.AreaOfInterestGroup.SuspendLayout()
        CType(Me.Scaling, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Resolution, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.FileLocationsGroup.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.CreateButton, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Cancel_Button, 1, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(386, 416)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(146, 29)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'CreateButton
        '
        Me.CreateButton.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.CreateButton.Location = New System.Drawing.Point(3, 3)
        Me.CreateButton.Name = "CreateButton"
        Me.CreateButton.Size = New System.Drawing.Size(67, 23)
        Me.CreateButton.TabIndex = 0
        Me.CreateButton.Text = "Create"
        '
        'CancelButton
        '
        Me.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Cancel_Button.Location = New System.Drawing.Point(76, 3)
        Me.Cancel_Button.Name = "CancelButton"
        Me.Cancel_Button.Size = New System.Drawing.Size(67, 23)
        Me.Cancel_Button.TabIndex = 1
        Me.Cancel_Button.Text = "Cancel"
        '
        'AreaOfInterestGroup
        '
        Me.AreaOfInterestGroup.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.AreaOfInterestGroup.Controls.Add(Me.Label4)
        Me.AreaOfInterestGroup.Controls.Add(Me.Scaling)
        Me.AreaOfInterestGroup.Controls.Add(Me.ResolutionLabel)
        Me.AreaOfInterestGroup.Controls.Add(Me.Resolution)
        Me.AreaOfInterestGroup.Controls.Add(Me.ElevationAdd)
        Me.AreaOfInterestGroup.Controls.Add(Me.ElevationDatasetPaths)
        Me.AreaOfInterestGroup.Controls.Add(Me.Label2)
        Me.AreaOfInterestGroup.Controls.Add(Me.MaskAdd)
        Me.AreaOfInterestGroup.Controls.Add(Me.MaskDatasetPath)
        Me.AreaOfInterestGroup.Controls.Add(Me.Label1)
        Me.AreaOfInterestGroup.Location = New System.Drawing.Point(12, 156)
        Me.AreaOfInterestGroup.Name = "AreaOfInterestGroup"
        Me.AreaOfInterestGroup.Size = New System.Drawing.Size(520, 242)
        Me.AreaOfInterestGroup.TabIndex = 11
        Me.AreaOfInterestGroup.TabStop = False
        Me.AreaOfInterestGroup.Text = "Area of Interest"
        '
        'Label4
        '
        Me.Label4.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label4.Location = New System.Drawing.Point(21, 185)
        Me.Label4.Margin = New System.Windows.Forms.Padding(3, 9, 3, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(410, 13)
        Me.Label4.TabIndex = 30
        Me.Label4.Text = "Vertical Scaling Factor"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Scaling
        '
        Me.Scaling.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Scaling.DecimalPlaces = 4
        Me.Scaling.Location = New System.Drawing.Point(331, 201)
        Me.Scaling.Maximum = New Decimal(New Integer() {-1, -1, -1, 0})
        Me.Scaling.Minimum = New Decimal(New Integer() {-1, -1, -1, -2147483648})
        Me.Scaling.Name = "Scaling"
        Me.Scaling.Size = New System.Drawing.Size(100, 20)
        Me.Scaling.TabIndex = 29
        Me.Scaling.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'ResolutionLabel
        '
        Me.ResolutionLabel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ResolutionLabel.Location = New System.Drawing.Point(20, 76)
        Me.ResolutionLabel.Margin = New System.Windows.Forms.Padding(3, 9, 3, 0)
        Me.ResolutionLabel.Name = "ResolutionLabel"
        Me.ResolutionLabel.Size = New System.Drawing.Size(411, 13)
        Me.ResolutionLabel.TabIndex = 28
        Me.ResolutionLabel.Text = "Project Raster Resolution (Map Units)"
        Me.ResolutionLabel.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Resolution
        '
        Me.Resolution.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Resolution.DecimalPlaces = 4
        Me.Resolution.Location = New System.Drawing.Point(331, 92)
        Me.Resolution.Maximum = New Decimal(New Integer() {-1, -1, -1, 0})
        Me.Resolution.Minimum = New Decimal(New Integer() {-1, -1, -1, -2147483648})
        Me.Resolution.Name = "Resolution"
        Me.Resolution.Size = New System.Drawing.Size(100, 20)
        Me.Resolution.TabIndex = 27
        Me.Resolution.Value = New Decimal(New Integer() {1760, 0, 0, 0})
        '
        'ElevationAdd
        '
        Me.ElevationAdd.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ElevationAdd.Location = New System.Drawing.Point(435, 151)
        Me.ElevationAdd.Name = "ElevationAdd"
        Me.ElevationAdd.Size = New System.Drawing.Size(67, 23)
        Me.ElevationAdd.TabIndex = 26
        Me.ElevationAdd.Text = "Add"
        Me.ElevationAdd.UseVisualStyleBackColor = True
        '
        'ElevationDatasetPaths
        '
        Me.ElevationDatasetPaths.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ElevationDatasetPaths.BackColor = System.Drawing.SystemColors.Window
        Me.ElevationDatasetPaths.Location = New System.Drawing.Point(18, 153)
        Me.ElevationDatasetPaths.Name = "ElevationDatasetPaths"
        Me.ElevationDatasetPaths.ReadOnly = True
        Me.ElevationDatasetPaths.Size = New System.Drawing.Size(411, 20)
        Me.ElevationDatasetPaths.TabIndex = 25
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(18, 136)
        Me.Label2.Margin = New System.Windows.Forms.Padding(3, 21, 3, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(286, 13)
        Me.Label2.TabIndex = 24
        Me.Label2.Text = "Elevation Dataset Path(s) (GDAL Readable Raster Format) "
        '
        'MaskAdd
        '
        Me.MaskAdd.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.MaskAdd.Location = New System.Drawing.Point(435, 42)
        Me.MaskAdd.Name = "MaskAdd"
        Me.MaskAdd.Size = New System.Drawing.Size(67, 23)
        Me.MaskAdd.TabIndex = 23
        Me.MaskAdd.Text = "Add"
        Me.MaskAdd.UseVisualStyleBackColor = True
        '
        'MaskDatasetPath
        '
        Me.MaskDatasetPath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.MaskDatasetPath.BackColor = System.Drawing.SystemColors.Window
        Me.MaskDatasetPath.Location = New System.Drawing.Point(18, 44)
        Me.MaskDatasetPath.Name = "MaskDatasetPath"
        Me.MaskDatasetPath.ReadOnly = True
        Me.MaskDatasetPath.Size = New System.Drawing.Size(411, 20)
        Me.MaskDatasetPath.TabIndex = 22
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(18, 28)
        Me.Label1.Margin = New System.Windows.Forms.Padding(3, 12, 3, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(257, 13)
        Me.Label1.TabIndex = 21
        Me.Label1.Text = "Mask Dataset Path (GDAL Readable Vector Format) "
        '
        'FileLocationsGroup
        '
        Me.FileLocationsGroup.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FileLocationsGroup.Controls.Add(Me.ClimateModelSet)
        Me.FileLocationsGroup.Controls.Add(Me.ClimateModelDirectory)
        Me.FileLocationsGroup.Controls.Add(Me.Label5)
        Me.FileLocationsGroup.Controls.Add(Me.ProjectSet)
        Me.FileLocationsGroup.Controls.Add(Me.ProjectDirectory)
        Me.FileLocationsGroup.Controls.Add(Me.Label8)
        Me.FileLocationsGroup.Location = New System.Drawing.Point(12, 12)
        Me.FileLocationsGroup.Name = "FileLocationsGroup"
        Me.FileLocationsGroup.Size = New System.Drawing.Size(520, 134)
        Me.FileLocationsGroup.TabIndex = 12
        Me.FileLocationsGroup.TabStop = False
        Me.FileLocationsGroup.Text = "File Locations"
        '
        'ClimateModelSet
        '
        Me.ClimateModelSet.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ClimateModelSet.Location = New System.Drawing.Point(435, 93)
        Me.ClimateModelSet.Name = "ClimateModelSet"
        Me.ClimateModelSet.Size = New System.Drawing.Size(67, 23)
        Me.ClimateModelSet.TabIndex = 26
        Me.ClimateModelSet.Text = "Set"
        Me.ClimateModelSet.UseVisualStyleBackColor = True
        '
        'ClimateModelDirectory
        '
        Me.ClimateModelDirectory.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ClimateModelDirectory.BackColor = System.Drawing.SystemColors.Window
        Me.ClimateModelDirectory.Location = New System.Drawing.Point(18, 95)
        Me.ClimateModelDirectory.Name = "ClimateModelDirectory"
        Me.ClimateModelDirectory.ReadOnly = True
        Me.ClimateModelDirectory.Size = New System.Drawing.Size(411, 20)
        Me.ClimateModelDirectory.TabIndex = 25
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(18, 79)
        Me.Label5.Margin = New System.Windows.Forms.Padding(3, 12, 3, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(143, 13)
        Me.Label5.TabIndex = 24
        Me.Label5.Text = "Climate Model Directory Path"
        '
        'ProjectSet
        '
        Me.ProjectSet.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ProjectSet.Location = New System.Drawing.Point(435, 42)
        Me.ProjectSet.Name = "ProjectSet"
        Me.ProjectSet.Size = New System.Drawing.Size(67, 23)
        Me.ProjectSet.TabIndex = 23
        Me.ProjectSet.Text = "Set"
        Me.ProjectSet.UseVisualStyleBackColor = True
        '
        'ProjectDirectory
        '
        Me.ProjectDirectory.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ProjectDirectory.BackColor = System.Drawing.SystemColors.Window
        Me.ProjectDirectory.Location = New System.Drawing.Point(18, 44)
        Me.ProjectDirectory.Name = "ProjectDirectory"
        Me.ProjectDirectory.ReadOnly = True
        Me.ProjectDirectory.Size = New System.Drawing.Size(411, 20)
        Me.ProjectDirectory.TabIndex = 22
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(18, 28)
        Me.Label8.Margin = New System.Windows.Forms.Padding(3, 12, 3, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(110, 13)
        Me.Label8.TabIndex = 21
        Me.Label8.Text = "Project Directory Path"
        '
        'ProgressBar
        '
        Me.ProgressBar.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ProgressBar.Location = New System.Drawing.Point(12, 419)
        Me.ProgressBar.Maximum = 5
        Me.ProgressBar.Name = "ProgressBar"
        Me.ProgressBar.Size = New System.Drawing.Size(371, 23)
        Me.ProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        Me.ProgressBar.TabIndex = 13
        Me.ProgressBar.Visible = False
        '
        'ProgressText
        '
        Me.ProgressText.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ProgressText.Location = New System.Drawing.Point(12, 403)
        Me.ProgressText.Margin = New System.Windows.Forms.Padding(3, 21, 3, 0)
        Me.ProgressText.Name = "ProgressText"
        Me.ProgressText.Size = New System.Drawing.Size(520, 13)
        Me.ProgressText.TabIndex = 31
        Me.ProgressText.Text = "Progress Update Text"
        Me.ProgressText.Visible = False
        '
        'New_Project
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(544, 459)
        Me.Controls.Add(Me.ProgressText)
        Me.Controls.Add(Me.ProgressBar)
        Me.Controls.Add(Me.FileLocationsGroup)
        Me.Controls.Add(Me.AreaOfInterestGroup)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "New_Project"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Create New Project"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.AreaOfInterestGroup.ResumeLayout(False)
        Me.AreaOfInterestGroup.PerformLayout()
        CType(Me.Scaling, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Resolution, System.ComponentModel.ISupportInitialize).EndInit()
        Me.FileLocationsGroup.ResumeLayout(False)
        Me.FileLocationsGroup.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents CreateButton As System.Windows.Forms.Button
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents AreaOfInterestGroup As System.Windows.Forms.GroupBox
    Friend WithEvents FileLocationsGroup As System.Windows.Forms.GroupBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Scaling As System.Windows.Forms.NumericUpDown
    Friend WithEvents ResolutionLabel As System.Windows.Forms.Label
    Friend WithEvents Resolution As System.Windows.Forms.NumericUpDown
    Friend WithEvents ElevationAdd As System.Windows.Forms.Button
    Friend WithEvents ElevationDatasetPaths As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents MaskAdd As System.Windows.Forms.Button
    Friend WithEvents MaskDatasetPath As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents ClimateModelSet As System.Windows.Forms.Button
    Friend WithEvents ClimateModelDirectory As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents ProjectSet As System.Windows.Forms.Button
    Friend WithEvents ProjectDirectory As System.Windows.Forms.TextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents ProgressBar As System.Windows.Forms.ProgressBar
    Friend WithEvents ProgressText As System.Windows.Forms.Label

End Class
