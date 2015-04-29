<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MapViewer
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MapViewer))
        Me.StatusBar = New System.Windows.Forms.StatusStrip()
        Me.StatusText = New System.Windows.Forms.ToolStripStatusLabel()
        Me.SplitContainer = New System.Windows.Forms.SplitContainer()
        Me.Legend = New System.Windows.Forms.Panel()
        Me.ColorRampBox = New System.Windows.Forms.ComboBox()
        Me.InvertColorRamp = New System.Windows.Forms.CheckBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.BottomRight = New System.Windows.Forms.Label()
        Me.BottomLeft = New System.Windows.Forms.Label()
        Me.TopRight = New System.Windows.Forms.Label()
        Me.TopLeft = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.LegendMinValue = New System.Windows.Forms.Label()
        Me.LegendMaxValue = New System.Windows.Forms.Label()
        Me.LegendImage = New System.Windows.Forms.PictureBox()
        Me.RasterBox = New System.Windows.Forms.ComboBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.FileBox = New System.Windows.Forms.ComboBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.SubDirectoryBox = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.ProjectDirectoryBox = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Map = New System.Windows.Forms.PictureBox()
        Me.ToolStrip = New System.Windows.Forms.ToolStrip()
        Me.ZoomFullExtent = New System.Windows.Forms.ToolStripButton()
        Me.ZoomInFixed = New System.Windows.Forms.ToolStripButton()
        Me.ZoomOutFixed = New System.Windows.Forms.ToolStripButton()
        Me.ZoomInBox = New System.Windows.Forms.ToolStripButton()
        Me.ZoomOutBox = New System.Windows.Forms.ToolStripButton()
        Me.Pan = New System.Windows.Forms.ToolStripButton()
        Me.StatusBar.SuspendLayout()
        CType(Me.SplitContainer, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer.Panel1.SuspendLayout()
        Me.SplitContainer.Panel2.SuspendLayout()
        Me.SplitContainer.SuspendLayout()
        Me.Legend.SuspendLayout()
        CType(Me.LegendImage, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Map, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ToolStrip.SuspendLayout()
        Me.SuspendLayout()
        '
        'StatusBar
        '
        Me.StatusBar.BackColor = System.Drawing.SystemColors.Control
        Me.StatusBar.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StatusText})
        Me.StatusBar.Location = New System.Drawing.Point(0, 483)
        Me.StatusBar.Name = "StatusBar"
        Me.StatusBar.Size = New System.Drawing.Size(600, 22)
        Me.StatusBar.SizingGrip = False
        Me.StatusBar.TabIndex = 1
        '
        'StatusText
        '
        Me.StatusText.Name = "StatusText"
        Me.StatusText.Size = New System.Drawing.Size(585, 17)
        Me.StatusText.Spring = True
        Me.StatusText.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'SplitContainer
        '
        Me.SplitContainer.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SplitContainer.BackColor = System.Drawing.SystemColors.Control
        Me.SplitContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.SplitContainer.Location = New System.Drawing.Point(3, 27)
        Me.SplitContainer.Margin = New System.Windows.Forms.Padding(3, 0, 3, 0)
        Me.SplitContainer.Name = "SplitContainer"
        '
        'SplitContainer.Panel1
        '
        Me.SplitContainer.Panel1.Controls.Add(Me.Legend)
        '
        'SplitContainer.Panel2
        '
        Me.SplitContainer.Panel2.BackColor = System.Drawing.SystemColors.Window
        Me.SplitContainer.Panel2.Controls.Add(Me.Map)
        Me.SplitContainer.Size = New System.Drawing.Size(594, 456)
        Me.SplitContainer.SplitterDistance = 194
        Me.SplitContainer.SplitterWidth = 2
        Me.SplitContainer.TabIndex = 2
        '
        'Legend
        '
        Me.Legend.AutoScroll = True
        Me.Legend.BackColor = System.Drawing.SystemColors.Control
        Me.Legend.Controls.Add(Me.ColorRampBox)
        Me.Legend.Controls.Add(Me.InvertColorRamp)
        Me.Legend.Controls.Add(Me.Label7)
        Me.Legend.Controls.Add(Me.BottomRight)
        Me.Legend.Controls.Add(Me.BottomLeft)
        Me.Legend.Controls.Add(Me.TopRight)
        Me.Legend.Controls.Add(Me.TopLeft)
        Me.Legend.Controls.Add(Me.Label6)
        Me.Legend.Controls.Add(Me.Label5)
        Me.Legend.Controls.Add(Me.LegendMinValue)
        Me.Legend.Controls.Add(Me.LegendMaxValue)
        Me.Legend.Controls.Add(Me.LegendImage)
        Me.Legend.Controls.Add(Me.RasterBox)
        Me.Legend.Controls.Add(Me.Label4)
        Me.Legend.Controls.Add(Me.FileBox)
        Me.Legend.Controls.Add(Me.Label3)
        Me.Legend.Controls.Add(Me.SubDirectoryBox)
        Me.Legend.Controls.Add(Me.Label2)
        Me.Legend.Controls.Add(Me.ProjectDirectoryBox)
        Me.Legend.Controls.Add(Me.Label1)
        Me.Legend.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Legend.Location = New System.Drawing.Point(0, 0)
        Me.Legend.Name = "Legend"
        Me.Legend.Size = New System.Drawing.Size(190, 452)
        Me.Legend.TabIndex = 0
        '
        'ColorRampBox
        '
        Me.ColorRampBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ColorRampBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ColorRampBox.FormattingEnabled = True
        Me.ColorRampBox.Location = New System.Drawing.Point(9, 190)
        Me.ColorRampBox.Margin = New System.Windows.Forms.Padding(9, 1, 9, 3)
        Me.ColorRampBox.MaxDropDownItems = 25
        Me.ColorRampBox.Name = "ColorRampBox"
        Me.ColorRampBox.Size = New System.Drawing.Size(174, 21)
        Me.ColorRampBox.TabIndex = 23
        '
        'InvertColorRamp
        '
        Me.InvertColorRamp.AutoSize = True
        Me.InvertColorRamp.Location = New System.Drawing.Point(12, 358)
        Me.InvertColorRamp.Margin = New System.Windows.Forms.Padding(3, 1, 3, 3)
        Me.InvertColorRamp.Name = "InvertColorRamp"
        Me.InvertColorRamp.Size = New System.Drawing.Size(111, 17)
        Me.InvertColorRamp.TabIndex = 21
        Me.InvertColorRamp.Text = "Invert Color Ramp"
        Me.InvertColorRamp.UseVisualStyleBackColor = True
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(9, 175)
        Me.Label7.Margin = New System.Windows.Forms.Padding(9, 3, 9, 1)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(62, 13)
        Me.Label7.TabIndex = 22
        Me.Label7.Text = "Color Ramp"
        '
        'BottomRight
        '
        Me.BottomRight.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.BottomRight.Location = New System.Drawing.Point(98, 423)
        Me.BottomRight.Margin = New System.Windows.Forms.Padding(9, 3, 9, 1)
        Me.BottomRight.Name = "BottomRight"
        Me.BottomRight.Size = New System.Drawing.Size(83, 21)
        Me.BottomRight.TabIndex = 20
        Me.BottomRight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'BottomLeft
        '
        Me.BottomLeft.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.BottomLeft.Location = New System.Drawing.Point(12, 423)
        Me.BottomLeft.Margin = New System.Windows.Forms.Padding(9, 3, 9, 1)
        Me.BottomLeft.Name = "BottomLeft"
        Me.BottomLeft.Size = New System.Drawing.Size(83, 21)
        Me.BottomLeft.TabIndex = 19
        Me.BottomLeft.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TopRight
        '
        Me.TopRight.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.TopRight.Location = New System.Drawing.Point(98, 398)
        Me.TopRight.Margin = New System.Windows.Forms.Padding(9, 3, 9, 1)
        Me.TopRight.Name = "TopRight"
        Me.TopRight.Size = New System.Drawing.Size(83, 21)
        Me.TopRight.TabIndex = 18
        Me.TopRight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TopLeft
        '
        Me.TopLeft.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.TopLeft.Location = New System.Drawing.Point(12, 398)
        Me.TopLeft.Margin = New System.Windows.Forms.Padding(9, 3, 9, 1)
        Me.TopLeft.Name = "TopLeft"
        Me.TopLeft.Size = New System.Drawing.Size(83, 21)
        Me.TopLeft.TabIndex = 17
        Me.TopLeft.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(9, 381)
        Me.Label6.Margin = New System.Windows.Forms.Padding(9, 3, 9, 1)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(72, 13)
        Me.Label6.TabIndex = 16
        Me.Label6.Text = "Cursor Values"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(9, 217)
        Me.Label5.Margin = New System.Windows.Forms.Padding(9, 3, 9, 1)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(43, 13)
        Me.Label5.TabIndex = 11
        Me.Label5.Text = "Legend"
        '
        'LegendMinValue
        '
        Me.LegendMinValue.AutoSize = True
        Me.LegendMinValue.Location = New System.Drawing.Point(58, 340)
        Me.LegendMinValue.Name = "LegendMinValue"
        Me.LegendMinValue.Size = New System.Drawing.Size(0, 13)
        Me.LegendMinValue.TabIndex = 10
        '
        'LegendMaxValue
        '
        Me.LegendMaxValue.AutoSize = True
        Me.LegendMaxValue.Location = New System.Drawing.Point(58, 233)
        Me.LegendMaxValue.Name = "LegendMaxValue"
        Me.LegendMaxValue.Size = New System.Drawing.Size(0, 13)
        Me.LegendMaxValue.TabIndex = 9
        '
        'LegendImage
        '
        Me.LegendImage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LegendImage.Location = New System.Drawing.Point(12, 234)
        Me.LegendImage.Name = "LegendImage"
        Me.LegendImage.Size = New System.Drawing.Size(40, 120)
        Me.LegendImage.TabIndex = 8
        Me.LegendImage.TabStop = False
        '
        'RasterBox
        '
        Me.RasterBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.RasterBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.RasterBox.FormattingEnabled = True
        Me.RasterBox.Location = New System.Drawing.Point(9, 148)
        Me.RasterBox.Margin = New System.Windows.Forms.Padding(9, 1, 9, 3)
        Me.RasterBox.MaxDropDownItems = 25
        Me.RasterBox.Name = "RasterBox"
        Me.RasterBox.Size = New System.Drawing.Size(172, 21)
        Me.RasterBox.TabIndex = 7
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(9, 133)
        Me.Label4.Margin = New System.Windows.Forms.Padding(9, 3, 9, 1)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(38, 13)
        Me.Label4.TabIndex = 6
        Me.Label4.Text = "Raster"
        '
        'FileBox
        '
        Me.FileBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FileBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.FileBox.FormattingEnabled = True
        Me.FileBox.Location = New System.Drawing.Point(9, 106)
        Me.FileBox.Margin = New System.Windows.Forms.Padding(9, 1, 9, 3)
        Me.FileBox.MaxDropDownItems = 25
        Me.FileBox.Name = "FileBox"
        Me.FileBox.Size = New System.Drawing.Size(172, 21)
        Me.FileBox.TabIndex = 5
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(9, 91)
        Me.Label3.Margin = New System.Windows.Forms.Padding(9, 3, 9, 1)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(23, 13)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = "File"
        '
        'SubDirectoryBox
        '
        Me.SubDirectoryBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SubDirectoryBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.SubDirectoryBox.FormattingEnabled = True
        Me.SubDirectoryBox.Location = New System.Drawing.Point(9, 64)
        Me.SubDirectoryBox.Margin = New System.Windows.Forms.Padding(9, 1, 9, 3)
        Me.SubDirectoryBox.MaxDropDownItems = 25
        Me.SubDirectoryBox.Name = "SubDirectoryBox"
        Me.SubDirectoryBox.Size = New System.Drawing.Size(172, 21)
        Me.SubDirectoryBox.TabIndex = 3
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(9, 49)
        Me.Label2.Margin = New System.Windows.Forms.Padding(9, 3, 9, 1)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(71, 13)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Sub Directory"
        '
        'ProjectDirectoryBox
        '
        Me.ProjectDirectoryBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ProjectDirectoryBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ProjectDirectoryBox.FormattingEnabled = True
        Me.ProjectDirectoryBox.Location = New System.Drawing.Point(9, 22)
        Me.ProjectDirectoryBox.Margin = New System.Windows.Forms.Padding(9, 1, 9, 3)
        Me.ProjectDirectoryBox.MaxDropDownItems = 25
        Me.ProjectDirectoryBox.Name = "ProjectDirectoryBox"
        Me.ProjectDirectoryBox.Size = New System.Drawing.Size(172, 21)
        Me.ProjectDirectoryBox.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(9, 7)
        Me.Label1.Margin = New System.Windows.Forms.Padding(9, 3, 9, 1)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(85, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Project Directory"
        '
        'Map
        '
        Me.Map.BackColor = System.Drawing.SystemColors.Window
        Me.Map.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Map.Location = New System.Drawing.Point(0, 0)
        Me.Map.Name = "Map"
        Me.Map.Size = New System.Drawing.Size(394, 452)
        Me.Map.TabIndex = 1
        Me.Map.TabStop = False
        '
        'ToolStrip
        '
        Me.ToolStrip.BackColor = System.Drawing.SystemColors.Control
        Me.ToolStrip.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.ToolStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ZoomFullExtent, Me.ZoomInFixed, Me.ZoomOutFixed, Me.ZoomInBox, Me.ZoomOutBox, Me.Pan})
        Me.ToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow
        Me.ToolStrip.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip.Name = "ToolStrip"
        Me.ToolStrip.Size = New System.Drawing.Size(600, 27)
        Me.ToolStrip.TabIndex = 3
        Me.ToolStrip.Text = "ToolStrip"
        '
        'ZoomFullExtent
        '
        Me.ZoomFullExtent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ZoomFullExtent.Image = CType(resources.GetObject("ZoomFullExtent.Image"), System.Drawing.Image)
        Me.ZoomFullExtent.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ZoomFullExtent.Name = "ZoomFullExtent"
        Me.ZoomFullExtent.Size = New System.Drawing.Size(24, 24)
        Me.ZoomFullExtent.Text = "Zoom To Full Extent"
        '
        'ZoomInFixed
        '
        Me.ZoomInFixed.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ZoomInFixed.Image = CType(resources.GetObject("ZoomInFixed.Image"), System.Drawing.Image)
        Me.ZoomInFixed.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ZoomInFixed.Name = "ZoomInFixed"
        Me.ZoomInFixed.Size = New System.Drawing.Size(24, 24)
        Me.ZoomInFixed.Text = "Fixed Zoom In"
        '
        'ZoomOutFixed
        '
        Me.ZoomOutFixed.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ZoomOutFixed.Image = CType(resources.GetObject("ZoomOutFixed.Image"), System.Drawing.Image)
        Me.ZoomOutFixed.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ZoomOutFixed.Name = "ZoomOutFixed"
        Me.ZoomOutFixed.Size = New System.Drawing.Size(24, 24)
        Me.ZoomOutFixed.Text = "Fixed Zoom Out"
        '
        'ZoomInBox
        '
        Me.ZoomInBox.CheckOnClick = True
        Me.ZoomInBox.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ZoomInBox.Image = CType(resources.GetObject("ZoomInBox.Image"), System.Drawing.Image)
        Me.ZoomInBox.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ZoomInBox.Name = "ZoomInBox"
        Me.ZoomInBox.Size = New System.Drawing.Size(24, 24)
        Me.ZoomInBox.Text = "Zoom In From Rectangle"
        '
        'ZoomOutBox
        '
        Me.ZoomOutBox.CheckOnClick = True
        Me.ZoomOutBox.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ZoomOutBox.Image = CType(resources.GetObject("ZoomOutBox.Image"), System.Drawing.Image)
        Me.ZoomOutBox.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ZoomOutBox.Name = "ZoomOutBox"
        Me.ZoomOutBox.Size = New System.Drawing.Size(24, 24)
        Me.ZoomOutBox.Text = "Zoom Out From Rectangle"
        '
        'Pan
        '
        Me.Pan.CheckOnClick = True
        Me.Pan.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.Pan.Image = CType(resources.GetObject("Pan.Image"), System.Drawing.Image)
        Me.Pan.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.Pan.Name = "Pan"
        Me.Pan.Size = New System.Drawing.Size(24, 24)
        Me.Pan.Text = "Pan"
        '
        'MapViewer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Window
        Me.Controls.Add(Me.ToolStrip)
        Me.Controls.Add(Me.SplitContainer)
        Me.Controls.Add(Me.StatusBar)
        Me.DoubleBuffered = True
        Me.Name = "MapViewer"
        Me.Size = New System.Drawing.Size(600, 505)
        Me.StatusBar.ResumeLayout(False)
        Me.StatusBar.PerformLayout()
        Me.SplitContainer.Panel1.ResumeLayout(False)
        Me.SplitContainer.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer.ResumeLayout(False)
        Me.Legend.ResumeLayout(False)
        Me.Legend.PerformLayout()
        CType(Me.LegendImage, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Map, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ToolStrip.ResumeLayout(False)
        Me.ToolStrip.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents StatusBar As System.Windows.Forms.StatusStrip
    Friend WithEvents SplitContainer As System.Windows.Forms.SplitContainer
    Friend WithEvents ToolStrip As System.Windows.Forms.ToolStrip
    Friend WithEvents Pan As System.Windows.Forms.ToolStripButton
    Friend WithEvents ZoomInFixed As System.Windows.Forms.ToolStripButton
    Friend WithEvents ZoomOutFixed As System.Windows.Forms.ToolStripButton
    Friend WithEvents ZoomInBox As System.Windows.Forms.ToolStripButton
    Friend WithEvents ZoomOutBox As System.Windows.Forms.ToolStripButton
    Friend WithEvents ZoomFullExtent As System.Windows.Forms.ToolStripButton
    Friend WithEvents StatusText As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents Legend As System.Windows.Forms.Panel
    Friend WithEvents ProjectDirectoryBox As System.Windows.Forms.ComboBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents SubDirectoryBox As System.Windows.Forms.ComboBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents FileBox As System.Windows.Forms.ComboBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents RasterBox As System.Windows.Forms.ComboBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents LegendMinValue As System.Windows.Forms.Label
    Friend WithEvents LegendMaxValue As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents LegendImage As System.Windows.Forms.PictureBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents TopLeft As System.Windows.Forms.Label
    Friend WithEvents BottomRight As System.Windows.Forms.Label
    Friend WithEvents BottomLeft As System.Windows.Forms.Label
    Friend WithEvents TopRight As System.Windows.Forms.Label
    Friend WithEvents InvertColorRamp As System.Windows.Forms.CheckBox
    Friend WithEvents ColorRampBox As System.Windows.Forms.ComboBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Map As System.Windows.Forms.PictureBox

End Class
