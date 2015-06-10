<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Cover_Properties
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
        Me.VariableGroup = New System.Windows.Forms.GroupBox()
        Me.Save = New System.Windows.Forms.Button()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.EffectivePrecipitation = New System.Windows.Forms.ComboBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.CurveName = New System.Windows.Forms.ComboBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.InitiationThresholdType = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.InitiationThreshold = New System.Windows.Forms.NumericUpDown()
        Me.IntermediateThresholdType = New System.Windows.Forms.ComboBox()
        Me.IntermediateThreshold = New System.Windows.Forms.NumericUpDown()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.TerminationThresholdType = New System.Windows.Forms.ComboBox()
        Me.TerminationThreshold = New System.Windows.Forms.NumericUpDown()
        Me.CuttingIntermediateThresholdType = New System.Windows.Forms.ComboBox()
        Me.CuttingTerminationThresholdType = New System.Windows.Forms.ComboBox()
        Me.CuttingIntermediateThreshold = New System.Windows.Forms.NumericUpDown()
        Me.CuttingTerminationThreshold = New System.Windows.Forms.NumericUpDown()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.SpringFrost = New System.Windows.Forms.NumericUpDown()
        Me.KillingFrost = New System.Windows.Forms.NumericUpDown()
        Me.NameGroup = New System.Windows.Forms.GroupBox()
        Me.Rename = New System.Windows.Forms.Button()
        Me.Down = New System.Windows.Forms.Button()
        Me.Up = New System.Windows.Forms.Button()
        Me.Remove = New System.Windows.Forms.Button()
        Me.Add = New System.Windows.Forms.Button()
        Me.CoverBox = New System.Windows.Forms.ListBox()
        Me.VariableGroup.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        CType(Me.InitiationThreshold, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.IntermediateThreshold, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TerminationThreshold, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CuttingIntermediateThreshold, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CuttingTerminationThreshold, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SpringFrost, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.KillingFrost, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.NameGroup.SuspendLayout()
        Me.SuspendLayout()
        '
        'VariableGroup
        '
        Me.VariableGroup.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.VariableGroup.Controls.Add(Me.Save)
        Me.VariableGroup.Controls.Add(Me.TableLayoutPanel1)
        Me.VariableGroup.Location = New System.Drawing.Point(12, 226)
        Me.VariableGroup.Name = "VariableGroup"
        Me.VariableGroup.Padding = New System.Windows.Forms.Padding(3, 6, 3, 3)
        Me.VariableGroup.Size = New System.Drawing.Size(559, 308)
        Me.VariableGroup.TabIndex = 9
        Me.VariableGroup.TabStop = False
        Me.VariableGroup.Text = "Cover Variables"
        '
        'Save
        '
        Me.Save.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Save.Location = New System.Drawing.Point(476, 27)
        Me.Save.Margin = New System.Windows.Forms.Padding(3, 9, 3, 9)
        Me.Save.Name = "Save"
        Me.Save.Size = New System.Drawing.Size(67, 23)
        Me.Save.TabIndex = 7
        Me.Save.Text = "Save"
        Me.Save.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.AutoScroll = True
        Me.TableLayoutPanel1.BackColor = System.Drawing.SystemColors.Window
        Me.TableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.OutsetDouble
        Me.TableLayoutPanel1.ColumnCount = 3
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.EffectivePrecipitation, 1, 8)
        Me.TableLayoutPanel1.Controls.Add(Me.Label3, 0, 8)
        Me.TableLayoutPanel1.Controls.Add(Me.Label2, 0, 7)
        Me.TableLayoutPanel1.Controls.Add(Me.Label9, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.CurveName, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Label4, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.InitiationThresholdType, 1, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.Label1, 0, 6)
        Me.TableLayoutPanel1.Controls.Add(Me.InitiationThreshold, 2, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.IntermediateThresholdType, 1, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.IntermediateThreshold, 2, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.Label6, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.Label8, 0, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.TerminationThresholdType, 1, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.TerminationThreshold, 2, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.CuttingIntermediateThresholdType, 1, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.CuttingTerminationThresholdType, 1, 5)
        Me.TableLayoutPanel1.Controls.Add(Me.CuttingIntermediateThreshold, 2, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.CuttingTerminationThreshold, 2, 5)
        Me.TableLayoutPanel1.Controls.Add(Me.Label11, 0, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.Label13, 0, 5)
        Me.TableLayoutPanel1.Controls.Add(Me.SpringFrost, 2, 6)
        Me.TableLayoutPanel1.Controls.Add(Me.KillingFrost, 2, 7)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(18, 62)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 9
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(525, 218)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'EffectivePrecipitation
        '
        Me.EffectivePrecipitation.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.EffectivePrecipitation.BackColor = System.Drawing.SystemColors.Window
        Me.TableLayoutPanel1.SetColumnSpan(Me.EffectivePrecipitation, 2)
        Me.EffectivePrecipitation.FormattingEnabled = True
        Me.EffectivePrecipitation.Location = New System.Drawing.Point(206, 195)
        Me.EffectivePrecipitation.Margin = New System.Windows.Forms.Padding(0)
        Me.EffectivePrecipitation.Name = "EffectivePrecipitation"
        Me.EffectivePrecipitation.Size = New System.Drawing.Size(316, 21)
        Me.EffectivePrecipitation.TabIndex = 9
        '
        'Label3
        '
        Me.Label3.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label3.Location = New System.Drawing.Point(3, 195)
        Me.Label3.Margin = New System.Windows.Forms.Padding(0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(200, 20)
        Me.Label3.TabIndex = 21
        Me.Label3.Text = "Effective Precipitation"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label2
        '
        Me.Label2.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label2.Location = New System.Drawing.Point(3, 171)
        Me.Label2.Margin = New System.Windows.Forms.Padding(0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(200, 21)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Killing Frost Temperature"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label9
        '
        Me.Label9.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label9.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label9.Location = New System.Drawing.Point(3, 3)
        Me.Label9.Margin = New System.Windows.Forms.Padding(0)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(200, 21)
        Me.Label9.TabIndex = 8
        Me.Label9.Text = "Curve Name"
        Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'CurveName
        '
        Me.CurveName.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CurveName.BackColor = System.Drawing.SystemColors.Window
        Me.TableLayoutPanel1.SetColumnSpan(Me.CurveName, 2)
        Me.CurveName.FormattingEnabled = True
        Me.CurveName.Location = New System.Drawing.Point(206, 3)
        Me.CurveName.Margin = New System.Windows.Forms.Padding(0)
        Me.CurveName.Name = "CurveName"
        Me.CurveName.Size = New System.Drawing.Size(316, 21)
        Me.CurveName.TabIndex = 8
        '
        'Label4
        '
        Me.Label4.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label4.Location = New System.Drawing.Point(3, 27)
        Me.Label4.Margin = New System.Windows.Forms.Padding(0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(200, 21)
        Me.Label4.TabIndex = 3
        Me.Label4.Text = "Initiation Threshold"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'InitiationThresholdType
        '
        Me.InitiationThresholdType.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.InitiationThresholdType.BackColor = System.Drawing.SystemColors.Window
        Me.InitiationThresholdType.FormattingEnabled = True
        Me.InitiationThresholdType.Location = New System.Drawing.Point(206, 27)
        Me.InitiationThresholdType.Margin = New System.Windows.Forms.Padding(0)
        Me.InitiationThresholdType.Name = "InitiationThresholdType"
        Me.InitiationThresholdType.Size = New System.Drawing.Size(156, 21)
        Me.InitiationThresholdType.TabIndex = 9
        '
        'Label1
        '
        Me.Label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label1.Location = New System.Drawing.Point(3, 147)
        Me.Label1.Margin = New System.Windows.Forms.Padding(0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(200, 21)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Spring Frost Temperature"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'InitiationThreshold
        '
        Me.InitiationThreshold.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.InitiationThreshold.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.InitiationThreshold.DecimalPlaces = 3
        Me.InitiationThreshold.Location = New System.Drawing.Point(365, 31)
        Me.InitiationThreshold.Margin = New System.Windows.Forms.Padding(0, 3, 0, 0)
        Me.InitiationThreshold.Maximum = New Decimal(New Integer() {1316134911, 2328, 0, 0})
        Me.InitiationThreshold.Minimum = New Decimal(New Integer() {1316134911, 2328, 0, -2147483648})
        Me.InitiationThreshold.Name = "InitiationThreshold"
        Me.InitiationThreshold.Size = New System.Drawing.Size(157, 16)
        Me.InitiationThreshold.TabIndex = 14
        Me.InitiationThreshold.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'IntermediateThresholdType
        '
        Me.IntermediateThresholdType.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.IntermediateThresholdType.BackColor = System.Drawing.SystemColors.Window
        Me.IntermediateThresholdType.FormattingEnabled = True
        Me.IntermediateThresholdType.Location = New System.Drawing.Point(206, 51)
        Me.IntermediateThresholdType.Margin = New System.Windows.Forms.Padding(0)
        Me.IntermediateThresholdType.Name = "IntermediateThresholdType"
        Me.IntermediateThresholdType.Size = New System.Drawing.Size(156, 21)
        Me.IntermediateThresholdType.TabIndex = 10
        '
        'IntermediateThreshold
        '
        Me.IntermediateThreshold.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.IntermediateThreshold.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.IntermediateThreshold.DecimalPlaces = 3
        Me.IntermediateThreshold.Location = New System.Drawing.Point(365, 55)
        Me.IntermediateThreshold.Margin = New System.Windows.Forms.Padding(0, 3, 0, 0)
        Me.IntermediateThreshold.Maximum = New Decimal(New Integer() {1316134911, 2328, 0, 0})
        Me.IntermediateThreshold.Minimum = New Decimal(New Integer() {1316134911, 2328, 0, -2147483648})
        Me.IntermediateThreshold.Name = "IntermediateThreshold"
        Me.IntermediateThreshold.Size = New System.Drawing.Size(157, 16)
        Me.IntermediateThreshold.TabIndex = 15
        Me.IntermediateThreshold.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label6
        '
        Me.Label6.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label6.Location = New System.Drawing.Point(3, 51)
        Me.Label6.Margin = New System.Windows.Forms.Padding(0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(200, 21)
        Me.Label6.TabIndex = 5
        Me.Label6.Text = "Intermediate Threshold"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label8
        '
        Me.Label8.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label8.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label8.Location = New System.Drawing.Point(3, 75)
        Me.Label8.Margin = New System.Windows.Forms.Padding(0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(200, 21)
        Me.Label8.TabIndex = 7
        Me.Label8.Text = "Termination Threshold"
        Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TerminationThresholdType
        '
        Me.TerminationThresholdType.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TerminationThresholdType.BackColor = System.Drawing.SystemColors.Window
        Me.TerminationThresholdType.FormattingEnabled = True
        Me.TerminationThresholdType.Location = New System.Drawing.Point(206, 75)
        Me.TerminationThresholdType.Margin = New System.Windows.Forms.Padding(0)
        Me.TerminationThresholdType.Name = "TerminationThresholdType"
        Me.TerminationThresholdType.Size = New System.Drawing.Size(156, 21)
        Me.TerminationThresholdType.TabIndex = 11
        '
        'TerminationThreshold
        '
        Me.TerminationThreshold.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TerminationThreshold.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TerminationThreshold.DecimalPlaces = 3
        Me.TerminationThreshold.Location = New System.Drawing.Point(365, 79)
        Me.TerminationThreshold.Margin = New System.Windows.Forms.Padding(0, 3, 0, 0)
        Me.TerminationThreshold.Maximum = New Decimal(New Integer() {1316134911, 2328, 0, 0})
        Me.TerminationThreshold.Minimum = New Decimal(New Integer() {1316134911, 2328, 0, -2147483648})
        Me.TerminationThreshold.Name = "TerminationThreshold"
        Me.TerminationThreshold.Size = New System.Drawing.Size(157, 16)
        Me.TerminationThreshold.TabIndex = 16
        Me.TerminationThreshold.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'CuttingIntermediateThresholdType
        '
        Me.CuttingIntermediateThresholdType.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CuttingIntermediateThresholdType.BackColor = System.Drawing.SystemColors.Window
        Me.CuttingIntermediateThresholdType.FormattingEnabled = True
        Me.CuttingIntermediateThresholdType.Location = New System.Drawing.Point(206, 99)
        Me.CuttingIntermediateThresholdType.Margin = New System.Windows.Forms.Padding(0)
        Me.CuttingIntermediateThresholdType.Name = "CuttingIntermediateThresholdType"
        Me.CuttingIntermediateThresholdType.Size = New System.Drawing.Size(156, 21)
        Me.CuttingIntermediateThresholdType.TabIndex = 12
        '
        'CuttingTerminationThresholdType
        '
        Me.CuttingTerminationThresholdType.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CuttingTerminationThresholdType.BackColor = System.Drawing.SystemColors.Window
        Me.CuttingTerminationThresholdType.FormattingEnabled = True
        Me.CuttingTerminationThresholdType.Location = New System.Drawing.Point(206, 123)
        Me.CuttingTerminationThresholdType.Margin = New System.Windows.Forms.Padding(0)
        Me.CuttingTerminationThresholdType.Name = "CuttingTerminationThresholdType"
        Me.CuttingTerminationThresholdType.Size = New System.Drawing.Size(156, 21)
        Me.CuttingTerminationThresholdType.TabIndex = 13
        '
        'CuttingIntermediateThreshold
        '
        Me.CuttingIntermediateThreshold.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CuttingIntermediateThreshold.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.CuttingIntermediateThreshold.DecimalPlaces = 3
        Me.CuttingIntermediateThreshold.Location = New System.Drawing.Point(365, 103)
        Me.CuttingIntermediateThreshold.Margin = New System.Windows.Forms.Padding(0, 3, 0, 0)
        Me.CuttingIntermediateThreshold.Maximum = New Decimal(New Integer() {1316134911, 2328, 0, 0})
        Me.CuttingIntermediateThreshold.Minimum = New Decimal(New Integer() {1316134911, 2328, 0, -2147483648})
        Me.CuttingIntermediateThreshold.Name = "CuttingIntermediateThreshold"
        Me.CuttingIntermediateThreshold.Size = New System.Drawing.Size(157, 16)
        Me.CuttingIntermediateThreshold.TabIndex = 17
        Me.CuttingIntermediateThreshold.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'CuttingTerminationThreshold
        '
        Me.CuttingTerminationThreshold.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CuttingTerminationThreshold.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.CuttingTerminationThreshold.DecimalPlaces = 3
        Me.CuttingTerminationThreshold.Location = New System.Drawing.Point(365, 127)
        Me.CuttingTerminationThreshold.Margin = New System.Windows.Forms.Padding(0, 3, 0, 0)
        Me.CuttingTerminationThreshold.Maximum = New Decimal(New Integer() {1316134911, 2328, 0, 0})
        Me.CuttingTerminationThreshold.Minimum = New Decimal(New Integer() {1316134911, 2328, 0, -2147483648})
        Me.CuttingTerminationThreshold.Name = "CuttingTerminationThreshold"
        Me.CuttingTerminationThreshold.Size = New System.Drawing.Size(157, 16)
        Me.CuttingTerminationThreshold.TabIndex = 18
        Me.CuttingTerminationThreshold.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label11
        '
        Me.Label11.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label11.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label11.Location = New System.Drawing.Point(3, 99)
        Me.Label11.Margin = New System.Windows.Forms.Padding(0)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(200, 21)
        Me.Label11.TabIndex = 10
        Me.Label11.Text = "Cutting Intermediate Threshold"
        Me.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label13
        '
        Me.Label13.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label13.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label13.Location = New System.Drawing.Point(3, 123)
        Me.Label13.Margin = New System.Windows.Forms.Padding(0)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(200, 21)
        Me.Label13.TabIndex = 12
        Me.Label13.Text = "Cutting Termination Threshold"
        Me.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'SpringFrost
        '
        Me.SpringFrost.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SpringFrost.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.SpringFrost.DecimalPlaces = 3
        Me.SpringFrost.Location = New System.Drawing.Point(365, 151)
        Me.SpringFrost.Margin = New System.Windows.Forms.Padding(0, 3, 0, 0)
        Me.SpringFrost.Maximum = New Decimal(New Integer() {1316134911, 2328, 0, 0})
        Me.SpringFrost.Minimum = New Decimal(New Integer() {1316134911, 2328, 0, -2147483648})
        Me.SpringFrost.Name = "SpringFrost"
        Me.SpringFrost.Size = New System.Drawing.Size(157, 16)
        Me.SpringFrost.TabIndex = 19
        Me.SpringFrost.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'KillingFrost
        '
        Me.KillingFrost.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.KillingFrost.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.KillingFrost.DecimalPlaces = 3
        Me.KillingFrost.Location = New System.Drawing.Point(365, 175)
        Me.KillingFrost.Margin = New System.Windows.Forms.Padding(0, 3, 0, 0)
        Me.KillingFrost.Maximum = New Decimal(New Integer() {1316134911, 2328, 0, 0})
        Me.KillingFrost.Minimum = New Decimal(New Integer() {1316134911, 2328, 0, -2147483648})
        Me.KillingFrost.Name = "KillingFrost"
        Me.KillingFrost.Size = New System.Drawing.Size(157, 16)
        Me.KillingFrost.TabIndex = 20
        Me.KillingFrost.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
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
        Me.NameGroup.Controls.Add(Me.CoverBox)
        Me.NameGroup.Location = New System.Drawing.Point(12, 12)
        Me.NameGroup.Name = "NameGroup"
        Me.NameGroup.Size = New System.Drawing.Size(560, 208)
        Me.NameGroup.TabIndex = 8
        Me.NameGroup.TabStop = False
        Me.NameGroup.Text = "Cover Name"
        '
        'Rename
        '
        Me.Rename.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Rename.Location = New System.Drawing.Point(476, 90)
        Me.Rename.Name = "Rename"
        Me.Rename.Size = New System.Drawing.Size(67, 23)
        Me.Rename.TabIndex = 4
        Me.Rename.Text = "Rename"
        Me.Rename.UseVisualStyleBackColor = True
        '
        'Down
        '
        Me.Down.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Down.Location = New System.Drawing.Point(476, 148)
        Me.Down.Name = "Down"
        Me.Down.Size = New System.Drawing.Size(67, 23)
        Me.Down.TabIndex = 6
        Me.Down.Text = "Down"
        Me.Down.UseVisualStyleBackColor = True
        '
        'Up
        '
        Me.Up.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Up.Location = New System.Drawing.Point(476, 119)
        Me.Up.Name = "Up"
        Me.Up.Size = New System.Drawing.Size(67, 23)
        Me.Up.TabIndex = 5
        Me.Up.Text = "Up"
        Me.Up.UseVisualStyleBackColor = True
        '
        'Remove
        '
        Me.Remove.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Remove.Location = New System.Drawing.Point(476, 61)
        Me.Remove.Name = "Remove"
        Me.Remove.Size = New System.Drawing.Size(67, 23)
        Me.Remove.TabIndex = 3
        Me.Remove.Text = "Remove"
        Me.Remove.UseVisualStyleBackColor = True
        '
        'Add
        '
        Me.Add.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Add.Location = New System.Drawing.Point(476, 32)
        Me.Add.Name = "Add"
        Me.Add.Size = New System.Drawing.Size(67, 23)
        Me.Add.TabIndex = 2
        Me.Add.Text = "Add"
        Me.Add.UseVisualStyleBackColor = True
        '
        'CoverBox
        '
        Me.CoverBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CoverBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CoverBox.FormattingEnabled = True
        Me.CoverBox.ItemHeight = 16
        Me.CoverBox.Location = New System.Drawing.Point(18, 32)
        Me.CoverBox.Name = "CoverBox"
        Me.CoverBox.ScrollAlwaysVisible = True
        Me.CoverBox.Size = New System.Drawing.Size(452, 148)
        Me.CoverBox.TabIndex = 1
        '
        'Cover_Properties
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(584, 546)
        Me.Controls.Add(Me.VariableGroup)
        Me.Controls.Add(Me.NameGroup)
        Me.MinimizeBox = False
        Me.Name = "Cover_Properties"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Cover Properties"
        Me.VariableGroup.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        CType(Me.InitiationThreshold, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.IntermediateThreshold, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TerminationThreshold, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CuttingIntermediateThreshold, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CuttingTerminationThreshold, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SpringFrost, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.KillingFrost, System.ComponentModel.ISupportInitialize).EndInit()
        Me.NameGroup.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents VariableGroup As System.Windows.Forms.GroupBox
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents NameGroup As System.Windows.Forms.GroupBox
    Friend WithEvents Down As System.Windows.Forms.Button
    Friend WithEvents Up As System.Windows.Forms.Button
    Friend WithEvents Remove As System.Windows.Forms.Button
    Friend WithEvents Add As System.Windows.Forms.Button
    Friend WithEvents CoverBox As System.Windows.Forms.ListBox
    Friend WithEvents InitiationThresholdType As System.Windows.Forms.ComboBox
    Friend WithEvents CuttingIntermediateThresholdType As System.Windows.Forms.ComboBox
    Friend WithEvents CuttingTerminationThresholdType As System.Windows.Forms.ComboBox
    Friend WithEvents IntermediateThresholdType As System.Windows.Forms.ComboBox
    Friend WithEvents TerminationThresholdType As System.Windows.Forms.ComboBox
    Friend WithEvents TerminationThreshold As System.Windows.Forms.NumericUpDown
    Friend WithEvents IntermediateThreshold As System.Windows.Forms.NumericUpDown
    Friend WithEvents CurveName As System.Windows.Forms.ComboBox
    Friend WithEvents CuttingTerminationThreshold As System.Windows.Forms.NumericUpDown
    Friend WithEvents CuttingIntermediateThreshold As System.Windows.Forms.NumericUpDown
    Friend WithEvents Save As System.Windows.Forms.Button
    Friend WithEvents InitiationThreshold As System.Windows.Forms.NumericUpDown
    Friend WithEvents SpringFrost As System.Windows.Forms.NumericUpDown
    Friend WithEvents KillingFrost As System.Windows.Forms.NumericUpDown
    Friend WithEvents Rename As System.Windows.Forms.Button
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents EffectivePrecipitation As System.Windows.Forms.ComboBox
End Class
