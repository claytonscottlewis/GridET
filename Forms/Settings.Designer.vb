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
        Me.OK_Button = New System.Windows.Forms.Button()
        Me.Cancel_Button = New System.Windows.Forms.Button()
        Me.FileLocationsGroup = New System.Windows.Forms.GroupBox()
        Me.ClimateModelSet = New System.Windows.Forms.Button()
        Me.ClimateModelBox = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.FileLocationsGroup.SuspendLayout()
        Me.SuspendLayout()
        '
        'OK_Button
        '
        Me.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.OK_Button.Location = New System.Drawing.Point(408, 165)
        Me.OK_Button.Name = "OK_Button"
        Me.OK_Button.Size = New System.Drawing.Size(75, 23)
        Me.OK_Button.TabIndex = 0
        Me.OK_Button.Text = "OK"
        '
        'Cancel_Button
        '
        Me.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Cancel_Button.Location = New System.Drawing.Point(489, 165)
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.Size = New System.Drawing.Size(75, 23)
        Me.Cancel_Button.TabIndex = 1
        Me.Cancel_Button.Text = "Cancel"
        '
        'FileLocationsGroup
        '
        Me.FileLocationsGroup.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FileLocationsGroup.Controls.Add(Me.ClimateModelSet)
        Me.FileLocationsGroup.Controls.Add(Me.ClimateModelBox)
        Me.FileLocationsGroup.Controls.Add(Me.Label5)
        Me.FileLocationsGroup.Location = New System.Drawing.Point(12, 12)
        Me.FileLocationsGroup.Name = "FileLocationsGroup"
        Me.FileLocationsGroup.Size = New System.Drawing.Size(570, 134)
        Me.FileLocationsGroup.TabIndex = 12
        Me.FileLocationsGroup.TabStop = False
        Me.FileLocationsGroup.Text = "File Location"
        '
        'ClimateModelSet
        '
        Me.ClimateModelSet.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ClimateModelSet.Location = New System.Drawing.Point(477, 60)
        Me.ClimateModelSet.Name = "ClimateModelSet"
        Me.ClimateModelSet.Size = New System.Drawing.Size(75, 23)
        Me.ClimateModelSet.TabIndex = 26
        Me.ClimateModelSet.Text = "Set"
        Me.ClimateModelSet.UseVisualStyleBackColor = True
        '
        'ClimateModelDirectory
        '
        Me.ClimateModelBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ClimateModelBox.BackColor = System.Drawing.SystemColors.Window
        Me.ClimateModelBox.Location = New System.Drawing.Point(18, 62)
        Me.ClimateModelBox.Name = "ClimateModelDirectory"
        Me.ClimateModelBox.ReadOnly = True
        Me.ClimateModelBox.Size = New System.Drawing.Size(453, 20)
        Me.ClimateModelBox.TabIndex = 25
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(18, 46)
        Me.Label5.Margin = New System.Windows.Forms.Padding(3, 12, 3, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(143, 13)
        Me.Label5.TabIndex = 24
        Me.Label5.Text = "Climate Model Directory Path"
        '
        'Settings
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(594, 205)
        Me.Controls.Add(Me.OK_Button)
        Me.Controls.Add(Me.FileLocationsGroup)
        Me.Controls.Add(Me.Cancel_Button)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Settings"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Project Settings"
        Me.FileLocationsGroup.ResumeLayout(False)
        Me.FileLocationsGroup.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents FileLocationsGroup As System.Windows.Forms.GroupBox
    Friend WithEvents ClimateModelSet As System.Windows.Forms.Button
    Friend WithEvents ClimateModelBox As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents OK_Button As System.Windows.Forms.Button
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button

End Class
