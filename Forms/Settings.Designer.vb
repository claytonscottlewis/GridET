<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Settings
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
        Me.OK_Button = New System.Windows.Forms.Button()
        Me.Cancel_Button = New System.Windows.Forms.Button()
        Me.FileLocationsGroup = New System.Windows.Forms.GroupBox()
        Me.ClimateModelSet = New System.Windows.Forms.Button()
        Me.ClimateModelDirectory = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.TableLayoutPanel1.SuspendLayout
        Me.FileLocationsGroup.SuspendLayout
        Me.SuspendLayout
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50!))
        Me.TableLayoutPanel1.Controls.Add(Me.OK_Button, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Cancel_Button, 1, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(386, 162)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(146, 29)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'OK_Button
        '
        Me.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.OK_Button.Location = New System.Drawing.Point(3, 3)
        Me.OK_Button.Name = "OK_Button"
        Me.OK_Button.Size = New System.Drawing.Size(67, 23)
        Me.OK_Button.TabIndex = 0
        Me.OK_Button.Text = "OK"
        '
        'Cancel_Button
        '
        Me.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Cancel_Button.Location = New System.Drawing.Point(76, 3)
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.Size = New System.Drawing.Size(67, 23)
        Me.Cancel_Button.TabIndex = 1
        Me.Cancel_Button.Text = "Cancel"
        '
        'FileLocationsGroup
        '
        Me.FileLocationsGroup.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.FileLocationsGroup.Controls.Add(Me.ClimateModelSet)
        Me.FileLocationsGroup.Controls.Add(Me.ClimateModelDirectory)
        Me.FileLocationsGroup.Controls.Add(Me.Label5)
        Me.FileLocationsGroup.Location = New System.Drawing.Point(12, 12)
        Me.FileLocationsGroup.Name = "FileLocationsGroup"
        Me.FileLocationsGroup.Size = New System.Drawing.Size(520, 134)
        Me.FileLocationsGroup.TabIndex = 12
        Me.FileLocationsGroup.TabStop = false
        Me.FileLocationsGroup.Text = "File Location"
        '
        'ClimateModelSet
        '
        Me.ClimateModelSet.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.ClimateModelSet.Location = New System.Drawing.Point(435, 60)
        Me.ClimateModelSet.Name = "ClimateModelSet"
        Me.ClimateModelSet.Size = New System.Drawing.Size(67, 23)
        Me.ClimateModelSet.TabIndex = 26
        Me.ClimateModelSet.Text = "Set"
        Me.ClimateModelSet.UseVisualStyleBackColor = true
        '
        'ClimateModelDirectory
        '
        Me.ClimateModelDirectory.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.ClimateModelDirectory.BackColor = System.Drawing.SystemColors.Window
        Me.ClimateModelDirectory.Location = New System.Drawing.Point(18, 62)
        Me.ClimateModelDirectory.Name = "ClimateModelDirectory"
        Me.ClimateModelDirectory.ReadOnly = true
        Me.ClimateModelDirectory.Size = New System.Drawing.Size(411, 20)
        Me.ClimateModelDirectory.TabIndex = 25
        '
        'Label5
        '
        Me.Label5.AutoSize = true
        Me.Label5.Location = New System.Drawing.Point(18, 46)
        Me.Label5.Margin = New System.Windows.Forms.Padding(3, 12, 3, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(143, 13)
        Me.Label5.TabIndex = 24
        Me.Label5.Text = "Climate Model Directory Path"
        '
        'Settings
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(544, 205)
        Me.Controls.Add(Me.FileLocationsGroup)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = false
        Me.MinimizeBox = false
        Me.Name = "Settings"
        Me.ShowInTaskbar = false
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Project Settings"
        Me.TableLayoutPanel1.ResumeLayout(false)
        Me.FileLocationsGroup.ResumeLayout(false)
        Me.FileLocationsGroup.PerformLayout
        Me.ResumeLayout(false)

End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents FileLocationsGroup As System.Windows.Forms.GroupBox
    Friend WithEvents ClimateModelSet As System.Windows.Forms.Button
    Friend WithEvents ClimateModelDirectory As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents OK_Button As System.Windows.Forms.Button
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button

End Class
