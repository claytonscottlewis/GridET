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
        Me.Legend = New System.Windows.Forms.TreeView()
        Me.Map = New System.Windows.Forms.PictureBox()
        Me.ToolStrip = New System.Windows.Forms.ToolStrip()
        Me.Pan = New System.Windows.Forms.ToolStripButton()
        Me.ZoomFullExtent = New System.Windows.Forms.ToolStripButton()
        Me.ZoomInFixed = New System.Windows.Forms.ToolStripButton()
        Me.ZoomOutFixed = New System.Windows.Forms.ToolStripButton()
        Me.ZoomInBox = New System.Windows.Forms.ToolStripButton()
        Me.ZoomOutBox = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.StatusBar.SuspendLayout()
        CType(Me.SplitContainer, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer.Panel1.SuspendLayout()
        Me.SplitContainer.Panel2.SuspendLayout()
        Me.SplitContainer.SuspendLayout()
        CType(Me.Map, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ToolStrip.SuspendLayout()
        Me.SuspendLayout()
        '
        'StatusBar
        '
        Me.StatusBar.BackColor = System.Drawing.SystemColors.Control
        Me.StatusBar.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StatusText})
        Me.StatusBar.Location = New System.Drawing.Point(0, 282)
        Me.StatusBar.Name = "StatusBar"
        Me.StatusBar.Size = New System.Drawing.Size(404, 22)
        Me.StatusBar.SizingGrip = False
        Me.StatusBar.TabIndex = 1
        '
        'StatusText
        '
        Me.StatusText.Name = "StatusText"
        Me.StatusText.Size = New System.Drawing.Size(0, 17)
        '
        'SplitContainer
        '
        Me.SplitContainer.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SplitContainer.BackColor = System.Drawing.SystemColors.Control
        Me.SplitContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.SplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
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
        Me.SplitContainer.Size = New System.Drawing.Size(398, 255)
        Me.SplitContainer.SplitterDistance = 200
        Me.SplitContainer.SplitterWidth = 2
        Me.SplitContainer.TabIndex = 2
        '
        'Legend
        '
        Me.Legend.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.Legend.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Legend.Location = New System.Drawing.Point(0, 0)
        Me.Legend.Name = "Legend"
        Me.Legend.Size = New System.Drawing.Size(196, 251)
        Me.Legend.TabIndex = 0
        '
        'Map
        '
        Me.Map.BackColor = System.Drawing.SystemColors.Window
        Me.Map.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Map.Location = New System.Drawing.Point(0, 0)
        Me.Map.Name = "Map"
        Me.Map.Size = New System.Drawing.Size(192, 251)
        Me.Map.TabIndex = 1
        Me.Map.TabStop = False
        '
        'ToolStrip
        '
        Me.ToolStrip.BackColor = System.Drawing.SystemColors.Control
        Me.ToolStrip.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.ToolStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ZoomFullExtent, Me.ZoomInFixed, Me.ZoomOutFixed, Me.ZoomInBox, Me.ZoomOutBox, Me.ToolStripSeparator2, Me.Pan})
        Me.ToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow
        Me.ToolStrip.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip.Name = "ToolStrip"
        Me.ToolStrip.Size = New System.Drawing.Size(404, 27)
        Me.ToolStrip.TabIndex = 3
        Me.ToolStrip.Text = "ToolStrip"
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
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 27)
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
        Me.Size = New System.Drawing.Size(404, 304)
        Me.StatusBar.ResumeLayout(False)
        Me.StatusBar.PerformLayout()
        Me.SplitContainer.Panel1.ResumeLayout(False)
        Me.SplitContainer.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer.ResumeLayout(False)
        CType(Me.Map, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ToolStrip.ResumeLayout(False)
        Me.ToolStrip.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents StatusBar As System.Windows.Forms.StatusStrip
    Friend WithEvents SplitContainer As System.Windows.Forms.SplitContainer
    Friend WithEvents Map As System.Windows.Forms.PictureBox
    Friend WithEvents Legend As System.Windows.Forms.TreeView
    Friend WithEvents ToolStrip As System.Windows.Forms.ToolStrip
    Friend WithEvents Pan As System.Windows.Forms.ToolStripButton
    Friend WithEvents ZoomInFixed As System.Windows.Forms.ToolStripButton
    Friend WithEvents ZoomOutFixed As System.Windows.Forms.ToolStripButton
    Friend WithEvents ZoomInBox As System.Windows.Forms.ToolStripButton
    Friend WithEvents ZoomOutBox As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ZoomFullExtent As System.Windows.Forms.ToolStripButton
    Friend WithEvents StatusText As System.Windows.Forms.ToolStripStatusLabel

End Class
