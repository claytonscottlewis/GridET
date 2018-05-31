<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class EarthDataLogin
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
        Me.OKButton = New System.Windows.Forms.Button()
        Me.ProgressText = New System.Windows.Forms.Label()
        Me.ProgressBar = New System.Windows.Forms.ProgressBar()
        Me.LoginGroup = New System.Windows.Forms.GroupBox()
        Me.PasswordBox = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.UsernameBox = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.CancelButton = New System.Windows.Forms.Button()
        Me.LoginGroup.SuspendLayout()
        Me.SuspendLayout()
        '
        'OKButton
        '
        Me.OKButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.OKButton.Location = New System.Drawing.Point(397, 207)
        Me.OKButton.Margin = New System.Windows.Forms.Padding(3, 9, 3, 9)
        Me.OKButton.Name = "OKButton"
        Me.OKButton.Size = New System.Drawing.Size(76, 23)
        Me.OKButton.TabIndex = 35
        Me.OKButton.Text = "OK"
        Me.OKButton.UseVisualStyleBackColor = True
        '
        'ProgressText
        '
        Me.ProgressText.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ProgressText.Location = New System.Drawing.Point(30, 191)
        Me.ProgressText.Margin = New System.Windows.Forms.Padding(3, 21, 3, 0)
        Me.ProgressText.Name = "ProgressText"
        Me.ProgressText.Size = New System.Drawing.Size(522, 13)
        Me.ProgressText.TabIndex = 38
        Me.ProgressText.Text = "Authenticating Earth Data client..."
        Me.ProgressText.Visible = False
        '
        'ProgressBar
        '
        Me.ProgressBar.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ProgressBar.Location = New System.Drawing.Point(30, 207)
        Me.ProgressBar.Maximum = 5
        Me.ProgressBar.Name = "ProgressBar"
        Me.ProgressBar.Size = New System.Drawing.Size(361, 23)
        Me.ProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        Me.ProgressBar.TabIndex = 37
        Me.ProgressBar.Visible = False
        '
        'LoginGroup
        '
        Me.LoginGroup.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LoginGroup.Controls.Add(Me.PasswordBox)
        Me.LoginGroup.Controls.Add(Me.Label1)
        Me.LoginGroup.Controls.Add(Me.UsernameBox)
        Me.LoginGroup.Controls.Add(Me.Label5)
        Me.LoginGroup.Location = New System.Drawing.Point(12, 12)
        Me.LoginGroup.Name = "LoginGroup"
        Me.LoginGroup.Padding = New System.Windows.Forms.Padding(3, 6, 3, 3)
        Me.LoginGroup.Size = New System.Drawing.Size(560, 165)
        Me.LoginGroup.TabIndex = 36
        Me.LoginGroup.TabStop = False
        Me.LoginGroup.Text = "EarthData Login"
        '
        'PasswordBox
        '
        Me.PasswordBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PasswordBox.BackColor = System.Drawing.SystemColors.Window
        Me.PasswordBox.Location = New System.Drawing.Point(17, 106)
        Me.PasswordBox.Name = "PasswordBox"
        Me.PasswordBox.PasswordChar = Global.Microsoft.VisualBasic.ChrW(9679)
        Me.PasswordBox.Size = New System.Drawing.Size(527, 20)
        Me.PasswordBox.TabIndex = 35
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(17, 90)
        Me.Label1.Margin = New System.Windows.Forms.Padding(3, 12, 3, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(53, 13)
        Me.Label1.TabIndex = 34
        Me.Label1.Text = "Password"
        '
        'UsernameBox
        '
        Me.UsernameBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.UsernameBox.BackColor = System.Drawing.SystemColors.Window
        Me.UsernameBox.Location = New System.Drawing.Point(17, 55)
        Me.UsernameBox.Name = "UsernameBox"
        Me.UsernameBox.Size = New System.Drawing.Size(527, 20)
        Me.UsernameBox.TabIndex = 33
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(17, 39)
        Me.Label5.Margin = New System.Windows.Forms.Padding(3, 12, 3, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(55, 13)
        Me.Label5.TabIndex = 32
        Me.Label5.Text = "Username"
        '
        'CancelButton
        '
        Me.CancelButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CancelButton.Location = New System.Drawing.Point(479, 207)
        Me.CancelButton.Margin = New System.Windows.Forms.Padding(3, 9, 3, 9)
        Me.CancelButton.Name = "CancelButton"
        Me.CancelButton.Size = New System.Drawing.Size(76, 23)
        Me.CancelButton.TabIndex = 39
        Me.CancelButton.Text = "Cancel"
        Me.CancelButton.UseVisualStyleBackColor = True
        '
        'EarthDataLogin
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.OKButton)
        Me.Controls.Add(Me.ProgressText)
        Me.Controls.Add(Me.ProgressBar)
        Me.Controls.Add(Me.LoginGroup)
        Me.Controls.Add(Me.CancelButton)
        Me.Name = "EarthDataLogin"
        Me.Size = New System.Drawing.Size(584, 242)
        Me.LoginGroup.ResumeLayout(False)
        Me.LoginGroup.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents OKButton As System.Windows.Forms.Button
    Friend WithEvents ProgressText As System.Windows.Forms.Label
    Friend WithEvents ProgressBar As System.Windows.Forms.ProgressBar
    Friend WithEvents LoginGroup As System.Windows.Forms.GroupBox
    Friend WithEvents CancelButton As System.Windows.Forms.Button
    Friend WithEvents PasswordBox As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents UsernameBox As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label

End Class
