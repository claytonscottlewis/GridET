Public Class Cover_Properties

#Region "Cover Name"

    Private Sub CoverListBox_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles CoverBox.SelectedIndexChanged
        If Not CoverBox.SelectedItem Is Nothing Then
            Command.CommandText = String.Format("SELECT Properties FROM Cover WHERE Name = '{0}'", CoverBox.SelectedItem)
            ExtractPropertiesString(Command.ExecuteScalar.ToString)
            Save.Enabled = False
        End If
    End Sub

    Private Sub Add_Click(sender As System.Object, e As System.EventArgs) Handles Add.Click
        Do
            Dim Name = InputBox("Please enter a unique cover name:")

            If String.IsNullOrEmpty(Name) Then
                Exit Sub
            ElseIf CoverBox.Items.Contains(Name) Then
                MsgBox(String.Format("'{0}' already exists as a cover name.", Name))
            Else
                CoverBox.Items.Add(Name)
                VariableGroup.Enabled = True
                Command.CommandText = String.Format("INSERT OR IGNORE INTO Cover (Name) VALUES('{0}')", Name)
                Command.ExecuteNonQuery()
                CoverBox.SelectedIndex = CoverBox.Items.Count - 1
                Exit Sub
            End If
        Loop
    End Sub

    Private Sub Remove_Click(sender As System.Object, e As System.EventArgs) Handles Remove.Click
        If CoverBox.SelectedIndex >= 0 Then
            If MsgBox(String.Format("Are you sure you'd like to remove cover '{0}'?", CoverBox.SelectedItem), MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                Command.CommandText = String.Format("DELETE FROM Cover WHERE Name = '{0}'", CoverBox.SelectedItem)
                Command.ExecuteNonQuery()

                Dim I As Integer = CoverBox.SelectedIndex
                CoverBox.Items.RemoveAt(I)
                If I > 0 Then
                    CoverBox.SelectedIndex = I - 1
                ElseIf CoverBox.Items.Count > 0 Then
                    CoverBox.SelectedIndex = 0
                End If
            End If
        End If
        If CoverBox.Items.Count = 0 Then
            ExtractPropertiesString("")
            VariableGroup.Enabled = False
        End If
    End Sub

    Private Sub Rename_Click(sender As System.Object, e As System.EventArgs) Handles Rename.Click
        If CoverBox.SelectedIndex >= 0 Then
            Do
                Dim Name = InputBox("Please enter a unique cover name:", , CoverBox.SelectedItem)

                If String.IsNullOrEmpty(Name) Then
                    Exit Sub
                ElseIf CoverBox.Items.Contains(Name) And CoverBox.SelectedItem <> Name Then
                    MsgBox(String.Format("'{0}' already exists as a cover name.", Name))
                ElseIf Name.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0 Then
                    MsgBox(String.Format("The cover name '{0}' cannot contain invalid filename characters.", Name))
                Else
                    Command.CommandText = String.Format("UPDATE Cover SET Name = '{0}' WHERE Name ='{1}'", Name, CoverBox.SelectedItem)
                    Command.ExecuteNonQuery()
                    CoverBox.Items(CoverBox.SelectedIndex) = Name
                    Exit Sub
                End If
            Loop
        End If
    End Sub

    Private Sub Up_Click(sender As System.Object, e As System.EventArgs) Handles Up.Click
        If CoverBox.SelectedIndex > 0 Then
            Dim RowName As String = CoverBox.SelectedItem
            Command.CommandText = String.Format("SELECT Properties FROM Cover WHERE Name = '{0}'", RowName)
            Dim RowProperties As String = Command.ExecuteScalar.ToString

            Dim AdjacentRowName As String = CoverBox.Items(CoverBox.SelectedIndex - 1)
            Command.CommandText = String.Format("SELECT Properties FROM Cover WHERE Name = '{0}'", AdjacentRowName)
            Dim AdjacentRowProperties As String = Command.ExecuteScalar.ToString

            Using Transaction = Connection.BeginTransaction

                Command.CommandText = String.Format("UPDATE Cover SET Name = '' WHERE Name = '{0}'", RowName)
                Command.ExecuteNonQuery()

                Command.CommandText = String.Format("UPDATE Cover SET Name = '{0}', Properties = '{1}' WHERE Name = '{2}'", RowName, RowProperties, AdjacentRowName)
                Command.ExecuteNonQuery()

                Command.CommandText = String.Format("UPDATE Cover SET Name = '{0}', Properties = '{1}' WHERE Name = ''", AdjacentRowName, AdjacentRowProperties)
                Command.ExecuteNonQuery()

                Transaction.Commit()
            End Using

            CoverBox.Items(CoverBox.SelectedIndex - 1) = RowName
            CoverBox.Items(CoverBox.SelectedIndex) = AdjacentRowName
            CoverBox.SelectedIndex -= 1
        End If
    End Sub

    Private Sub Down_Click(sender As System.Object, e As System.EventArgs) Handles Down.Click
        If CoverBox.SelectedIndex >= 0 And CoverBox.SelectedIndex < CoverBox.Items.Count - 1 Then
            Dim RowName As String = CoverBox.SelectedItem
            Command.CommandText = String.Format("SELECT Properties FROM Cover WHERE Name = '{0}'", RowName)
            Dim RowProperties As String = Command.ExecuteScalar.ToString

            Dim AdjacentRowName As String = CoverBox.Items(CoverBox.SelectedIndex + 1)
            Command.CommandText = String.Format("SELECT Properties FROM Cover WHERE Name = '{0}'", AdjacentRowName)
            Dim AdjacentRowProperties As String = Command.ExecuteScalar.ToString

            Using Transaction = Connection.BeginTransaction

                Command.CommandText = String.Format("UPDATE Cover SET Name = '' WHERE Name = '{0}'", RowName)
                Command.ExecuteNonQuery()

                Command.CommandText = String.Format("UPDATE Cover SET Name = '{0}', Properties = '{1}' WHERE Name = '{2}'", RowName, RowProperties, AdjacentRowName)
                Command.ExecuteNonQuery()

                Command.CommandText = String.Format("UPDATE Cover SET Name = '{0}', Properties = '{1}' WHERE Name = ''", AdjacentRowName, AdjacentRowProperties)
                Command.ExecuteNonQuery()

                Transaction.Commit()
            End Using

            CoverBox.Items(CoverBox.SelectedIndex + 1) = RowName
            CoverBox.Items(CoverBox.SelectedIndex) = AdjacentRowName
            CoverBox.SelectedIndex += 1
        End If
    End Sub

#End Region

#Region "Properties and Database"

    Private Properties As New List(Of Control)
    Private ComboBoxes As New List(Of ComboBox)
    Private NumericBoxes As New List(Of NumericUpDown)
    Private DoSave As Boolean = True
    Private Connection As System.Data.SQLite.SQLiteConnection
    Private Command As System.Data.SQLite.SQLiteCommand

    Private Sub Save_Click(sender As System.Object, e As System.EventArgs) Handles Save.Click
        Command.CommandText = String.Format("UPDATE Cover SET Properties = '{0}' WHERE Name = '{1}'", CreatePropertiesString(), CoverBox.SelectedItem)
        Command.ExecuteNonQuery()
        Save.Enabled = False
    End Sub

    Private Sub Cover_Properties_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        For Each Control In Me.Controls
            GetType(Control).InvokeMember("DoubleBuffered", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance Or Reflection.BindingFlags.SetProperty, Nothing, Control, New Object() {True})
        Next

        For Each Control As Control In TableLayoutPanel1.Controls
            Select Case Control.GetType
                Case GetType(ComboBox) : ComboBoxes.Add(Control) : Properties.Add(Control)
                Case GetType(NumericUpDown) : NumericBoxes.Add(Control) : Properties.Add(Control)
            End Select
        Next

        Dim ThresholdTypes As String() = [Enum].GetNames(GetType(ThresholdType))
        For I = 1 To ComboBoxes.Count - 1
            ComboBoxes(I).Items.AddRange(ThresholdTypes)
        Next

        EffectivePrecipitation.Items.AddRange([Enum].GetNames(GetType(EffectivePrecipitationType)))

        For Each ComboBox In ComboBoxes
            AddHandler ComboBox.TextChanged, AddressOf ComboBox_TextChanged
        Next

        For Each NumericBox In NumericBoxes
            AddHandler NumericBox.ValueChanged, AddressOf NumericUpDown_ValueChanged
        Next

        Save.Enabled = False

        'Open Database
        If IO.File.Exists(ProjectDetailsPath) Then
            NameGroup.Enabled = True
            VariableGroup.Enabled = True

            Connection = CreateConnection(ProjectDetailsPath, False)
            Connection.Open()

            Command = Connection.CreateCommand
            Command.CommandText = "CREATE TABLE IF NOT EXISTS Cover (Name TEXT UNIQUE, Properties TEXT)"
            Command.ExecuteNonQuery()

            Command.CommandText = "SELECT Name FROM Cover ORDER BY ROWID"
            Using Reader = Command.ExecuteReader
                Do While Reader.Read
                    CoverBox.Items.Add(Reader.GetString(0))
                Loop
            End Using

            Command.CommandText = "SELECT Name FROM Curve ORDER BY ROWID"
            Using Reader = Command.ExecuteReader
                Do While Reader.Read
                    CurveName.Items.Add(Reader.GetString(0))
                Loop
            End Using

            If CoverBox.Items.Count > 0 Then
                CoverBox.SelectedIndex = 0
            Else
                VariableGroup.Enabled = False
            End If
        Else
            NameGroup.Enabled = False
            VariableGroup.Enabled = False
        End If
    End Sub

    Private Sub Cover_Properties_FormClosed(sender As Object, e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        If Not Command Is Nothing Then Command.Dispose()
        If Not Connection Is Nothing Then Connection.Dispose()
    End Sub

    Private Function CreatePropertiesString() As String
        Dim SB As New System.Text.StringBuilder

        For I = 0 To Properties.Count - 1
            SB.Append(Properties(I).Text & ";")
        Next
        SB.Remove(SB.Length - 1, 1)

        Return SB.ToString
    End Function

    Private Sub ExtractPropertiesString(PropertiesString As String)
        DoSave = False

        Dim Text = PropertiesString.Split(";")

        For I = 0 To Properties.Count - 1
            If I >= Text.Length Then
                If Properties(I).GetType Is GetType(ComboBox) Then
                    CType(Properties(I), ComboBox).SelectedIndex = -1
                Else
                    CType(Properties(I), NumericUpDown).Value = 0
                End If
            Else
                If Properties(I).GetType Is GetType(ComboBox) Then
                    Dim Box = CType(Properties(I), ComboBox)
                    Box.SelectedIndex = -1
                    If Box.Items.Contains(Text(I)) Then Box.Text = Text(I)
                Else
                    CType(Properties(I), NumericUpDown).Value = Text(I)
                End If
            End If
        Next

        CurveName_TextChanged(Nothing, Nothing)

        DoSave = True
    End Sub

    Private Sub CurveName_TextChanged(sender As Object, e As System.EventArgs) Handles CurveName.TextChanged
        Dim HasCuttings As Boolean = False
        If Not CurveName.Text = "" Then
            Command.CommandText = String.Format("SELECT Properties FROM Curve WHERE Name = '{0}'", CurveName.Text)
            Dim Properties = Command.ExecuteScalar.ToString.Split(";")
            If Properties.Length > 1 Then
                If [Enum].Parse(GetType(SeasonalCurveType), Properties(1)) = SeasonalCurveType.Has_Cuttings Then HasCuttings = True
            End If
        End If

        For Row = 4 To 5
            For Col = 0 To 2
                TableLayoutPanel1.GetControlFromPosition(Col, Row).Enabled = HasCuttings
            Next
        Next
    End Sub

    Private Sub ComboBox_TextChanged(sender As Object, e As System.EventArgs)
        If DoSave Then Save.Enabled = True
    End Sub

    Private Sub NumericUpDown_ValueChanged(sender As Object, e As System.EventArgs)
        If DoSave Then Save.Enabled = True
    End Sub

#End Region

End Class