'            Copyright Clayton S. Lewis 2014-2018.
'   Distributed under the Boost Software License, Version 1.0.
'      (See accompanying file GridET License.rtf or copy at
'            http://www.boost.org/LICENSE_1_0.txt)

Public Class Cover_Curves

#Region "Curve Name"

    Private Sub CurveListBox_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles CurveListBox.SelectedIndexChanged
        If Not CurveListBox.SelectedItem Is Nothing Then
            Command.CommandText = String.Format("SELECT Properties FROM Curve WHERE Name = '{0}'", CurveListBox.SelectedItem)
            ExtractPropertiesString(Command.ExecuteScalar.ToString)
            Save.Enabled = False
        End If
    End Sub

    Private Sub Add_Click(sender As System.Object, e As System.EventArgs) Handles Add.Click
        Do
            Dim Name = InputBox("Please enter a unique curve name:")

            If String.IsNullOrEmpty(Name) Then
                Exit Sub
            ElseIf CurveListBox.Items.Contains(Name) Then
                MsgBox(String.Format("'{0}' already exists as a curve name.", Name))
            Else
                CurveListBox.Items.Add(Name)
                VariableGroup.Enabled = True
                Command.CommandText = "INSERT OR IGNORE INTO Curve (Name) VALUES(@Name)"
                Command.Parameters.Add("@Name", DbType.String).Value = Name
                Command.ExecuteNonQuery()
                CurveListBox.SelectedIndex = CurveListBox.Items.Count - 1
                SeasonalCurveType.SelectedIndex = 0
                For Each Item In BaseVariable.Items
                    If Item.ToString.Contains("Evapotranspiration ASCE") Then
                        BaseVariable.Text = Item
                        Exit For
                    End If
                Next
                Exit Sub
            End If
        Loop
    End Sub

    Private Sub Remove_Click(sender As System.Object, e As System.EventArgs) Handles Remove.Click
        If CurveListBox.SelectedIndex >= 0 Then
            If MsgBox(String.Format("Are you sure you'd like to remove curve '{0}'?", CurveListBox.SelectedItem), MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                Command.CommandText = "DELETE FROM Curve WHERE Name = @Name"
                Command.Parameters.Add("@Name", DbType.String).Value = CurveListBox.SelectedItem
                Command.ExecuteNonQuery()

                Dim I As Integer = CurveListBox.SelectedIndex
                CurveListBox.Items.RemoveAt(I)
                If I > 0 Then
                    CurveListBox.SelectedIndex = I - 1
                ElseIf CurveListBox.Items.Count > 0 Then
                    CurveListBox.SelectedIndex = 0
                End If
            End If
        End If
        If CurveListBox.Items.Count = 0 Then
            ExtractPropertiesString("")
            VariableGroup.Enabled = False
        End If
    End Sub

    Private Sub Rename_Click(sender As System.Object, e As System.EventArgs) Handles Rename.Click
        If CurveListBox.SelectedIndex >= 0 Then
            Do
                Dim Name = InputBox("Please enter a unique curve name:", , CurveListBox.SelectedItem)

                If String.IsNullOrEmpty(Name) Then
                    Exit Sub
                ElseIf CurveListBox.Items.Contains(Name) And CurveListBox.SelectedItem <> Name Then
                    MsgBox(String.Format("'{0}' already exists as a curve name.", Name))
                Else
                    Dim UpdateText As New List(Of String())
                    Dim UpdateName As New List(Of String)

                    Command.CommandText = "SELECT Name, Properties FROM Cover"
                    Using Reader = Command.ExecuteReader
                        Do While Reader.Read
                            Dim Properties = Reader.GetString(1).Split(";")
                            If Properties(0) = CurveListBox.SelectedItem Then
                                Properties(0) = Name
                                UpdateText.Add(Properties)
                                UpdateName.Add(Reader.GetString(0))
                            End If
                        Loop
                    End Using

                    Using Transaction = Connection.BeginTransaction
                        For I = 0 To UpdateName.Count - 1
                            Command.CommandText = "UPDATE Cover SET Properties = @Properties WHERE Name = @Name"
                            Command.Parameters.Add("@Name", DbType.String).Value = UpdateName(I)
                            Command.Parameters.Add("@Properties", DbType.String).Value = String.Join(";", UpdateText(I))
                            Command.ExecuteNonQuery()
                        Next

                        Command.CommandText = "UPDATE Curve SET Name = @Name WHERE Name = @FormerName"
                        Command.Parameters.Add("@Name", DbType.String).Value = Name
                        Command.Parameters.Add("@FormerName", DbType.String).Value = CurveListBox.SelectedItem
                        Command.ExecuteNonQuery()

                        Transaction.Commit()
                    End Using

                    CurveListBox.Items(CurveListBox.SelectedIndex) = Name

                    Exit Sub
                End If
            Loop
        End If
    End Sub

    Private Sub Up_Click(sender As System.Object, e As System.EventArgs) Handles Up.Click
        If CurveListBox.SelectedIndex > 0 Then
            Dim RowName As String = CurveListBox.SelectedItem
            Command.CommandText = String.Format("SELECT Properties FROM Curve WHERE Name = '{0}'", RowName)
            Dim RowProperties As String = Command.ExecuteScalar.ToString

            Dim AdjacentRowName As String = CurveListBox.Items(CurveListBox.SelectedIndex - 1)
            Command.CommandText = String.Format("SELECT Properties FROM Curve WHERE Name = '{0}'", AdjacentRowName)
            Dim AdjacentRowProperties As String = Command.ExecuteScalar.ToString

            Using Transaction = Connection.BeginTransaction

                Command.CommandText = String.Format("UPDATE Curve SET Name = '' WHERE Name = '{0}'", RowName)
                Command.ExecuteNonQuery()

                Command.CommandText = String.Format("UPDATE Curve SET Name = '{0}', Properties = '{1}' WHERE Name = '{2}'", RowName, RowProperties, AdjacentRowName)
                Command.ExecuteNonQuery()

                Command.CommandText = String.Format("UPDATE Curve SET Name = '{0}', Properties = '{1}' WHERE Name = ''", AdjacentRowName, AdjacentRowProperties)
                Command.ExecuteNonQuery()

                Transaction.Commit()
            End Using

            CurveListBox.Items(CurveListBox.SelectedIndex - 1) = RowName
            CurveListBox.Items(CurveListBox.SelectedIndex) = AdjacentRowName
            CurveListBox.SelectedIndex -= 1
        End If
    End Sub

    Private Sub Down_Click(sender As System.Object, e As System.EventArgs) Handles Down.Click
        If CurveListBox.SelectedIndex >= 0 And CurveListBox.SelectedIndex < CurveListBox.Items.Count - 1 Then
            Dim RowName As String = CurveListBox.SelectedItem
            Command.CommandText = String.Format("SELECT Properties FROM Curve WHERE Name = '{0}'", RowName)
            Dim RowProperties As String = Command.ExecuteScalar.ToString

            Dim AdjacentRowName As String = CurveListBox.Items(CurveListBox.SelectedIndex + 1)
            Command.CommandText = String.Format("SELECT Properties FROM Curve WHERE Name = '{0}'", AdjacentRowName)
            Dim AdjacentRowProperties As String = Command.ExecuteScalar.ToString

            Using Transaction = Connection.BeginTransaction

                Command.CommandText = String.Format("UPDATE Curve SET Name = '' WHERE Name = '{0}'", RowName)
                Command.ExecuteNonQuery()

                Command.CommandText = String.Format("UPDATE Curve SET Name = '{0}', Properties = '{1}' WHERE Name = '{2}'", RowName, RowProperties, AdjacentRowName)
                Command.ExecuteNonQuery()

                Command.CommandText = String.Format("UPDATE Curve SET Name = '{0}', Properties = '{1}' WHERE Name = ''", AdjacentRowName, AdjacentRowProperties)
                Command.ExecuteNonQuery()

                Transaction.Commit()
            End Using

            CurveListBox.Items(CurveListBox.SelectedIndex + 1) = RowName
            CurveListBox.Items(CurveListBox.SelectedIndex) = AdjacentRowName
            CurveListBox.SelectedIndex += 1
        End If
    End Sub

#End Region

#Region "Properties and Database"

    Private DoSave As Boolean = True
    Private Connection As System.Data.SQLite.SQLiteConnection
    Private Command As System.Data.SQLite.SQLiteCommand

    Private Sub Save_Click(sender As System.Object, e As System.EventArgs) Handles Save.Click
        Command.CommandText = String.Format("UPDATE Curve SET Properties = '{0}' WHERE Name = '{1}'", CreatePropertiesString(), CurveListBox.SelectedItem)
        Command.ExecuteNonQuery()
        Save.Enabled = False
    End Sub

    Private Sub Curve_Properties_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        For Each Control In Me.Controls
            GetType(Control).InvokeMember("DoubleBuffered", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance Or Reflection.BindingFlags.SetProperty, Nothing, Control, New Object() {True})
        Next

        Save.Enabled = False

        InitialPeriodGrid.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        FinalPeriodGrid.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

        For I = 0 To 10
            InitialPeriodGrid.Rows.Add()
            InitialPeriodGrid.Rows(I).HeaderCell.Value = I * 10 & "%"
        Next

        If Not ProjectDirectory = "" Then
            For Each Directory In {DateVariablesDirectory, InputVariablesDirectory, ReferenceEvapotranspirationDirectory}
                If IO.Directory.Exists(Directory) Then
                    For Each File In IO.Directory.GetFiles(Directory, "*.db", IO.SearchOption.AllDirectories)
                        Dim Variable = IO.Path.GetFileNameWithoutExtension(File)
                        If Not BaseVariable.Items.Contains(Variable) Then BaseVariable.Items.Add(Variable)
                    Next
                End If
            Next
        End If

        SeasonalCurveType.Items.AddRange([Enum].GetNames(GetType(SeasonalCurveType)))

        Dim CurveTypes As String() = [Enum].GetNames(GetType(CurveType))
        CurveTypePeriod1.Items.AddRange(CurveTypes)
        CurveTypePeriod2.Items.AddRange(CurveTypes)

        For Each Grid In {InitialPeriodGrid, FinalPeriodGrid}
            For I = 1 To 2
                Grid.Columns(I).Visible = False
            Next
        Next

        'Open Database
        If IO.File.Exists(ProjectDetailsPath) Then
            NameGroup.Enabled = True
            VariableGroup.Enabled = True

            Connection = CreateConnection(ProjectDetailsPath, False)
            Connection.Open()

            Command = Connection.CreateCommand
         
            Command.CommandText = "SELECT Name FROM Curve ORDER BY ROWID"
            Using Reader = Command.ExecuteReader
                Do While Reader.Read
                    CurveListBox.Items.Add(Reader.GetString(0))
                Loop
            End Using

            If CurveListBox.Items.Count > 0 Then
                CurveListBox.SelectedIndex = 0
            Else
                VariableGroup.Enabled = False
            End If
        Else
            NameGroup.Enabled = False
            VariableGroup.Enabled = False
        End If
    End Sub

    Private Sub Curve_Properties_FormClosed(sender As Object, e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        If Not Command Is Nothing Then Command.Dispose()
        If Not Connection Is Nothing Then Connection.Dispose()
    End Sub

    Private Function CreatePropertiesString() As String
        Dim SB As New System.Text.StringBuilder

        SB.Append(BaseVariable.Text & ";")

        SB.Append(SeasonalCurveType.Text & ";")

        SB.Append(CurveTypePeriod1.Text & ";")
        For Row = 0 To InitialPeriodGrid.RowCount - 1
            For Col = 0 To 2
                SB.Append(InitialPeriodGrid(Col, Row).Value & ",")
            Next
        Next
        SB.Remove(SB.Length - 1, 1)

        SB.Append(";" & CurveTypePeriod2.Text & ";")
        For Row = 0 To FinalPeriodGrid.RowCount - 2
            For Col = 0 To 2
                SB.Append(FinalPeriodGrid(Col, Row).Value & ",")
            Next
        Next
        If Not SB(SB.Length - 1) = ";" Then SB.Remove(SB.Length - 1, 1)

        Return SB.ToString
    End Function

    Private Sub ExtractPropertiesString(PropertiesString As String)
        DoSave = False

        Try
            BaseVariable.Text = ""
            SeasonalCurveType.Text = ""
            CurveTypePeriod1.Text = ""
            CurveTypePeriod2.Text = ""
            For Col = 0 To 2
                For Row = 0 To 10
                    InitialPeriodGrid(Col, Row).Value = "0.000"
                Next
            Next
            FinalPeriodGrid.Rows.Clear()

            Dim Text = PropertiesString.Split(";")
            If Text.Length < 6 Then Exit Try

            Dim I As Integer = 0
            If BaseVariable.Items.Contains(Text(I)) Then BaseVariable.Text = Text(I)

            I += 1
            If SeasonalCurveType.Items.Contains(Text(I)) Then SeasonalCurveType.Text = Text(I)

            I += 1
            If CurveTypePeriod1.Items.Contains(Text(I)) Then CurveTypePeriod1.Text = Text(I)

            I += 1
            Dim Values = Text(I).Split(",")
            For Row = 0 To CInt(Values.Length / 3 - 1)
                For Col = 0 To 2
                    Dim Value = Values(Row * 3 + Col)
                    If IsNumeric(Value) Then InitialPeriodGrid(Col, Row).Value = FormatNumber(Value, 3, TriState.True, TriState.False, TriState.False)
                Next
            Next

            I += 1
            If CurveTypePeriod2.Items.Contains(Text(I)) Then CurveTypePeriod2.Text = Text(I)

            I += 1
            Values = Text(I).Split(",")
            For Row = 0 To CInt(Values.Length / 3 - 1)
                For Col = 0 To 2
                    If FinalPeriodGrid.RowCount <= Row + 1 Then FinalPeriodGrid.Rows.Add()

                    Dim Value = Values(Row * 3 + Col)

                    If IsNumeric(Value) Then
                        FinalPeriodGrid(Col, Row).Value = FormatNumber(Value, 3, TriState.True, TriState.False, TriState.False)
                    Else
                        FinalPeriodGrid(Col, Row).Value = ""
                    End If
                Next
            Next
        Catch : End Try

        DoSave = True
    End Sub

    Private Sub Grid_CellEndEdit(Grid As DataGridView, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles InitialPeriodGrid.CellEndEdit, FinalPeriodGrid.CellEndEdit
        Dim Cell = Grid(e.ColumnIndex, e.RowIndex)

        If IsNumeric(Cell.Value) Then
            Cell.Value = FormatNumber(Cell.Value, 3, TriState.True, TriState.False, TriState.False)
        Else
            MsgBox("Value entered was not a number! Please retry.")
            Cell.Value = "0.000"
        End If
    End Sub

    Private Sub Grid_CellValueChanged(sender As Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles InitialPeriodGrid.CellValueChanged, FinalPeriodGrid.CellValueChanged
        If DoSave Then Save.Enabled = True
    End Sub

    Private Sub Grid_RowsAdded(Grid As DataGridView, e As System.Windows.Forms.DataGridViewRowsAddedEventArgs) Handles InitialPeriodGrid.RowsAdded, FinalPeriodGrid.RowsAdded
        If Grid Is FinalPeriodGrid Then
            For I = 0 To Grid.Rows.Count - 2
                Grid.Rows(I).HeaderCell.Value = " " & CStr(I + 1)
            Next
        Else
            For Col = 0 To 2
                Grid(Col, Grid.RowCount - 1).Value = "0.000"
            Next
        End If
    End Sub

    Private Sub SeasonalCurveType_TextChanged(Box As ComboBox, e As System.EventArgs) Handles SeasonalCurveType.TextChanged
        Dim Visible As Boolean = False

        If Box.Text = Calculations.SeasonalCurveType.Has_Cuttings.ToString Then Visible = True

        For Each Grid In {InitialPeriodGrid, FinalPeriodGrid}
            For I = 1 To 2
                Grid.Columns(I).Visible = Visible
            Next
        Next

        If DoSave Then Save.Enabled = True
    End Sub

    Private Sub ComboBox_TextChanged(sender As Object, e As System.EventArgs) Handles CurveTypePeriod1.TextChanged, CurveTypePeriod2.TextChanged, BaseVariable.TextChanged
        If DoSave Then Save.Enabled = True
    End Sub

    Private Sub FinalPeriodGrid_RowsRemoved(sender As Object, e As System.Windows.Forms.DataGridViewRowsRemovedEventArgs) Handles FinalPeriodGrid.RowsRemoved
        If DoSave Then Save.Enabled = True
    End Sub

#End Region

End Class