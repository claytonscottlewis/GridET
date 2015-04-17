<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Main
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Main))
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.NewProjectToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.OpenProjectToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CloseProjectToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.SettingsToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SettingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CurveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CoverToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ProcessToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.UpdateDAYMETToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DAYMETToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.NLDASToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PRISMToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator()
        Me.LANDSATToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MODISToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator7 = New System.Windows.Forms.ToolStripSeparator()
        Me.EvapotranspirationToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ReferenceToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PotentialToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.NetPotentialToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ActualToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CalculateStatisticsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExtractByPolygonToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator6 = New System.Windows.Forms.ToolStripSeparator()
        Me.RunAllToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.HelpToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ManualToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.AboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MapViewer = New GridET.MapViewer()
        Me.ToolStripSeparator8 = New System.Windows.Forms.ToolStripSeparator()
        Me.ImportToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'MenuStrip1
        '
        Me.MenuStrip1.BackColor = System.Drawing.SystemColors.Control
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem, Me.SettingsToolStripMenuItem, Me.ProcessToolStripMenuItem, Me.HelpToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(584, 24)
        Me.MenuStrip1.TabIndex = 0
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NewProjectToolStripMenuItem, Me.ToolStripSeparator1, Me.OpenProjectToolStripMenuItem, Me.CloseProjectToolStripMenuItem, Me.ToolStripSeparator2, Me.SettingsToolStripMenuItem1, Me.ToolStripSeparator4, Me.ExitToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(56, 20)
        Me.FileToolStripMenuItem.Text = "Project"
        '
        'NewProjectToolStripMenuItem
        '
        Me.NewProjectToolStripMenuItem.Name = "NewProjectToolStripMenuItem"
        Me.NewProjectToolStripMenuItem.Size = New System.Drawing.Size(116, 22)
        Me.NewProjectToolStripMenuItem.Text = "New"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(113, 6)
        '
        'OpenProjectToolStripMenuItem
        '
        Me.OpenProjectToolStripMenuItem.Name = "OpenProjectToolStripMenuItem"
        Me.OpenProjectToolStripMenuItem.Size = New System.Drawing.Size(116, 22)
        Me.OpenProjectToolStripMenuItem.Text = "Open"
        '
        'CloseProjectToolStripMenuItem
        '
        Me.CloseProjectToolStripMenuItem.Name = "CloseProjectToolStripMenuItem"
        Me.CloseProjectToolStripMenuItem.Size = New System.Drawing.Size(116, 22)
        Me.CloseProjectToolStripMenuItem.Text = "Close"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(113, 6)
        '
        'SettingsToolStripMenuItem1
        '
        Me.SettingsToolStripMenuItem1.Name = "SettingsToolStripMenuItem1"
        Me.SettingsToolStripMenuItem1.Size = New System.Drawing.Size(116, 22)
        Me.SettingsToolStripMenuItem1.Text = "Settings"
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        Me.ToolStripSeparator4.Size = New System.Drawing.Size(113, 6)
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(116, 22)
        Me.ExitToolStripMenuItem.Text = "Exit"
        '
        'SettingsToolStripMenuItem
        '
        Me.SettingsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CurveToolStripMenuItem, Me.CoverToolStripMenuItem, Me.ToolStripSeparator8, Me.ImportToolStripMenuItem})
        Me.SettingsToolStripMenuItem.Name = "SettingsToolStripMenuItem"
        Me.SettingsToolStripMenuItem.Size = New System.Drawing.Size(72, 20)
        Me.SettingsToolStripMenuItem.Text = "Properties"
        '
        'CurveToolStripMenuItem
        '
        Me.CurveToolStripMenuItem.Name = "CurveToolStripMenuItem"
        Me.CurveToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.CurveToolStripMenuItem.Text = "Curve"
        '
        'CoverToolStripMenuItem
        '
        Me.CoverToolStripMenuItem.Name = "CoverToolStripMenuItem"
        Me.CoverToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.CoverToolStripMenuItem.Text = "Cover"
        '
        'ProcessToolStripMenuItem
        '
        Me.ProcessToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.UpdateDAYMETToolStripMenuItem, Me.ToolStripSeparator7, Me.EvapotranspirationToolStripMenuItem, Me.CalculateStatisticsToolStripMenuItem, Me.ToolStripSeparator6, Me.RunAllToolStripMenuItem})
        Me.ProcessToolStripMenuItem.Name = "ProcessToolStripMenuItem"
        Me.ProcessToolStripMenuItem.Size = New System.Drawing.Size(59, 20)
        Me.ProcessToolStripMenuItem.Text = "Process"
        '
        'UpdateDAYMETToolStripMenuItem
        '
        Me.UpdateDAYMETToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DAYMETToolStripMenuItem, Me.NLDASToolStripMenuItem, Me.PRISMToolStripMenuItem, Me.ToolStripSeparator5, Me.LANDSATToolStripMenuItem, Me.MODISToolStripMenuItem})
        Me.UpdateDAYMETToolStripMenuItem.Name = "UpdateDAYMETToolStripMenuItem"
        Me.UpdateDAYMETToolStripMenuItem.Size = New System.Drawing.Size(225, 22)
        Me.UpdateDAYMETToolStripMenuItem.Text = "Download Climate Model"
        '
        'DAYMETToolStripMenuItem
        '
        Me.DAYMETToolStripMenuItem.Name = "DAYMETToolStripMenuItem"
        Me.DAYMETToolStripMenuItem.Size = New System.Drawing.Size(126, 22)
        Me.DAYMETToolStripMenuItem.Text = "DAYMET"
        '
        'NLDASToolStripMenuItem
        '
        Me.NLDASToolStripMenuItem.Name = "NLDASToolStripMenuItem"
        Me.NLDASToolStripMenuItem.Size = New System.Drawing.Size(126, 22)
        Me.NLDASToolStripMenuItem.Text = "NLDAS"
        '
        'PRISMToolStripMenuItem
        '
        Me.PRISMToolStripMenuItem.Enabled = False
        Me.PRISMToolStripMenuItem.Name = "PRISMToolStripMenuItem"
        Me.PRISMToolStripMenuItem.Size = New System.Drawing.Size(126, 22)
        Me.PRISMToolStripMenuItem.Text = "PRISM"
        '
        'ToolStripSeparator5
        '
        Me.ToolStripSeparator5.Name = "ToolStripSeparator5"
        Me.ToolStripSeparator5.Size = New System.Drawing.Size(123, 6)
        '
        'LANDSATToolStripMenuItem
        '
        Me.LANDSATToolStripMenuItem.Enabled = False
        Me.LANDSATToolStripMenuItem.Name = "LANDSATToolStripMenuItem"
        Me.LANDSATToolStripMenuItem.Size = New System.Drawing.Size(126, 22)
        Me.LANDSATToolStripMenuItem.Text = "LANDSAT"
        '
        'MODISToolStripMenuItem
        '
        Me.MODISToolStripMenuItem.Enabled = False
        Me.MODISToolStripMenuItem.Name = "MODISToolStripMenuItem"
        Me.MODISToolStripMenuItem.Size = New System.Drawing.Size(126, 22)
        Me.MODISToolStripMenuItem.Text = "MODIS"
        '
        'ToolStripSeparator7
        '
        Me.ToolStripSeparator7.Name = "ToolStripSeparator7"
        Me.ToolStripSeparator7.Size = New System.Drawing.Size(222, 6)
        '
        'EvapotranspirationToolStripMenuItem
        '
        Me.EvapotranspirationToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ReferenceToolStripMenuItem, Me.PotentialToolStripMenuItem, Me.NetPotentialToolStripMenuItem, Me.ActualToolStripMenuItem})
        Me.EvapotranspirationToolStripMenuItem.Name = "EvapotranspirationToolStripMenuItem"
        Me.EvapotranspirationToolStripMenuItem.Size = New System.Drawing.Size(225, 22)
        Me.EvapotranspirationToolStripMenuItem.Text = "Calculate Evapotranspiration"
        '
        'ReferenceToolStripMenuItem
        '
        Me.ReferenceToolStripMenuItem.Name = "ReferenceToolStripMenuItem"
        Me.ReferenceToolStripMenuItem.Size = New System.Drawing.Size(143, 22)
        Me.ReferenceToolStripMenuItem.Text = "Reference"
        '
        'PotentialToolStripMenuItem
        '
        Me.PotentialToolStripMenuItem.Name = "PotentialToolStripMenuItem"
        Me.PotentialToolStripMenuItem.Size = New System.Drawing.Size(143, 22)
        Me.PotentialToolStripMenuItem.Text = "Potential"
        '
        'NetPotentialToolStripMenuItem
        '
        Me.NetPotentialToolStripMenuItem.Name = "NetPotentialToolStripMenuItem"
        Me.NetPotentialToolStripMenuItem.Size = New System.Drawing.Size(143, 22)
        Me.NetPotentialToolStripMenuItem.Text = "Net Potential"
        '
        'ActualToolStripMenuItem
        '
        Me.ActualToolStripMenuItem.Enabled = False
        Me.ActualToolStripMenuItem.Name = "ActualToolStripMenuItem"
        Me.ActualToolStripMenuItem.Size = New System.Drawing.Size(143, 22)
        Me.ActualToolStripMenuItem.Text = "Actual"
        '
        'CalculateStatisticsToolStripMenuItem
        '
        Me.CalculateStatisticsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ExtractByPolygonToolStripMenuItem})
        Me.CalculateStatisticsToolStripMenuItem.Name = "CalculateStatisticsToolStripMenuItem"
        Me.CalculateStatisticsToolStripMenuItem.Size = New System.Drawing.Size(225, 22)
        Me.CalculateStatisticsToolStripMenuItem.Text = "Calculate Statistics"
        '
        'ExtractByPolygonToolStripMenuItem
        '
        Me.ExtractByPolygonToolStripMenuItem.Name = "ExtractByPolygonToolStripMenuItem"
        Me.ExtractByPolygonToolStripMenuItem.Size = New System.Drawing.Size(172, 22)
        Me.ExtractByPolygonToolStripMenuItem.Text = "Extract by Polygon"
        '
        'ToolStripSeparator6
        '
        Me.ToolStripSeparator6.Name = "ToolStripSeparator6"
        Me.ToolStripSeparator6.Size = New System.Drawing.Size(222, 6)
        '
        'RunAllToolStripMenuItem
        '
        Me.RunAllToolStripMenuItem.Name = "RunAllToolStripMenuItem"
        Me.RunAllToolStripMenuItem.Size = New System.Drawing.Size(225, 22)
        Me.RunAllToolStripMenuItem.Text = "Run All"
        '
        'HelpToolStripMenuItem
        '
        Me.HelpToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ManualToolStripMenuItem, Me.ToolStripSeparator3, Me.AboutToolStripMenuItem})
        Me.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem"
        Me.HelpToolStripMenuItem.Size = New System.Drawing.Size(44, 20)
        Me.HelpToolStripMenuItem.Text = "Help"
        '
        'ManualToolStripMenuItem
        '
        Me.ManualToolStripMenuItem.Name = "ManualToolStripMenuItem"
        Me.ManualToolStripMenuItem.Size = New System.Drawing.Size(114, 22)
        Me.ManualToolStripMenuItem.Text = "Manual"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(111, 6)
        '
        'AboutToolStripMenuItem
        '
        Me.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        Me.AboutToolStripMenuItem.Size = New System.Drawing.Size(114, 22)
        Me.AboutToolStripMenuItem.Text = "About"
        '
        'MapViewer
        '
        Me.MapViewer.BackColor = System.Drawing.SystemColors.Control
        Me.MapViewer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MapViewer.Location = New System.Drawing.Point(0, 24)
        Me.MapViewer.Name = "MapViewer"
        Me.MapViewer.Size = New System.Drawing.Size(584, 388)
        Me.MapViewer.TabIndex = 1
        '
        'ToolStripSeparator8
        '
        Me.ToolStripSeparator8.Name = "ToolStripSeparator8"
        Me.ToolStripSeparator8.Size = New System.Drawing.Size(149, 6)
        '
        'ImportToolStripMenuItem
        '
        Me.ImportToolStripMenuItem.Name = "ImportToolStripMenuItem"
        Me.ImportToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.ImportToolStripMenuItem.Text = "Import"
        '
        'Main
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Window
        Me.ClientSize = New System.Drawing.Size(584, 412)
        Me.Controls.Add(Me.MapViewer)
        Me.Controls.Add(Me.MenuStrip1)
        Me.DoubleBuffered = True
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "Main"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "GridET"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents FileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents NewProjectToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OpenProjectToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CloseProjectToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ProcessToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents UpdateDAYMETToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DAYMETToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents NLDASToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PRISMToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents EvapotranspirationToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SettingsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CoverToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CurveToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SettingsToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator4 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator5 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents LANDSATToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MODISToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PotentialToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ReferenceToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ActualToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator6 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents RunAllToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents HelpToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ManualToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents AboutToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MapViewer As GridET.MapViewer
    Friend WithEvents CalculateStatisticsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator7 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents NetPotentialToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExtractByPolygonToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator8 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ImportToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
