Module SQLite

    Function CreateConnection(Path As String, Optional ReadOnlyMode As Boolean = True) As System.Data.SQLite.SQLiteConnection
        Dim Directory = IO.Path.GetDirectoryName(Path)
        If Not IO.Directory.Exists(Directory) Then IO.Directory.CreateDirectory(Directory)
        If Not IO.File.Exists(Path) Then System.Data.SQLite.SQLiteConnection.CreateFile(Path)

        Dim ConnectionBuilder As New System.Data.SQLite.SQLiteConnectionStringBuilder
        ConnectionBuilder.DataSource = Path
        ConnectionBuilder.ReadOnly = ReadOnlyMode
        If Not ReadOnlyMode Then
            ConnectionBuilder.DefaultTimeout = 1000
            ConnectionBuilder.PageSize = 4096
            ConnectionBuilder.MaxPageCount = 1073741823
            ConnectionBuilder.JournalMode = Data.SQLite.SQLiteJournalModeEnum.Wal
            ConnectionBuilder.SyncMode = Data.SQLite.SynchronizationModes.Full
        End If

        Return New System.Data.SQLite.SQLiteConnection(ConnectionBuilder.ToString)
    End Function

    ''' <summary>
    ''' Checks to see if scalar command will return a DBNull value.
    ''' </summary>
    <System.Runtime.CompilerServices.Extension()> Public Function IsExecuteScalarDBNull(SQLiteCommand As Data.SQLite.SQLiteCommand) As Boolean
        Using Reader = SQLiteCommand.ExecuteReader
            Reader.Read()

            If Reader.IsDBNull(0) Then
                Return True
            Else
                Return False
            End If
        End Using
    End Function

End Module
