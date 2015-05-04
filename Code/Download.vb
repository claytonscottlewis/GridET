Module Download

#Region "DAYMET"

    Sub DownloadDAYMET(DatabasePath As String, DownloadStartDate As DateTime, DownloadEndDate As DateTime, Variable As String, BackgroundWorker As System.ComponentModel.BackgroundWorker, DoWorkEvent As System.ComponentModel.DoWorkEventArgs)
        Using Connection = CreateConnection(DatabasePath, False)
            Connection.Open()
            Using Command = Connection.CreateCommand

                'Download DAYMET Files
                Dim Year As Integer = 0
                For Year = DownloadStartDate.Year To DownloadEndDate.Year
                    'Start DAYMET Ftp File Download
                    Dim VariablePath As String = IO.Path.GetTempFileName
                    DownloadFileToDrive(BuildFtpStringDAYMET(Year, Variable), VariablePath)

                    Dim Raster As New Raster(String.Format("NETCDF:""{0}"":{1}", VariablePath, Variable))
                    Raster.Open(GDAL.Access.GA_ReadOnly)
                    Dim XCount = Raster.XCount
                    Dim YCount = Raster.YCount

                    'Determine Start and End Dates
                    Dim StartDoY As Integer = 1
                    If Year = DownloadStartDate.Year Then StartDoY = DownloadStartDate.DayOfYear
                    If StartDoY = 366 Then StartDoY = 365

                    Dim EndDoY As Integer = 365
                    If Year = DownloadEndDate.Year Then EndDoY = DownloadEndDate.DayOfYear
                    If EndDoY = 366 Then EndDoY = 365

                    For DoY = StartDoY To EndDoY
                        Dim Path = IO.Path.Combine(IO.Path.GetTempPath, String.Format("Daymet_{0}_{1}.tif", Year, DoY))

                        Using Driver = GDAL.Gdal.GetDriverByName(GDALProcess.RasterFormat.GTiff.ToString)
                            Using Dataset = Driver.Create(Path, XCount, YCount, 1, GDAL.DataType.GDT_Float32, {"COMPRESS=DEFLATE", "TILED=YES"})
                                Dataset.SetProjection(Raster.Projection)
                                Dataset.SetGeoTransform(Raster.GeoTransform)

                                Using Band = Dataset.GetRasterBand(1)
                                    Band.SetDescription(Variable)
                                    Band.SetNoDataValue(Single.MinValue)
                                End Using

                                Dim Data(XCount * YCount - 1) As Single
                                Raster.Dataset.ReadRaster(0, 0, XCount, YCount, Data, XCount, YCount, 1, {DoY}, 0, 0, 0)

                                For I = 0 To Data.Length - 1
                                    If Data(I) = Raster.BandNoDataValue(DoY - 1) Then Data(I) = Single.MinValue
                                Next

                                Dataset.WriteRaster(0, 0, XCount, YCount, Data, XCount, YCount, 1, {1}, 0, 0, 0)
                            End Using
                        End Using

                        'Store Raster in Database
                        Command.CommandText = "INSERT OR REPLACE INTO Rasters (Date, Image) VALUES (@Date, @Image)"
                        Command.Parameters.Add("@Date", DbType.DateTime).Value = New DateTime(Year - 1, 12, 31).AddDays(DoY).AddHours(13)
                        Command.Parameters.Add("@Image", DbType.Object).Value = IO.File.ReadAllBytes(Path)
                        Command.ExecuteNonQuery()

                        If DoY = 365 Then
                            Dim LastDay = New DateTime(Year, 12, 31)
                            If LastDay.DayOfYear = 366 Then
                                Command.CommandText = "INSERT OR REPLACE INTO Rasters (Date, Image) VALUES (@Date, @Image)"
                                Command.Parameters.Add("@Date", DbType.DateTime).Value = LastDay.AddHours(13)
                                Command.Parameters.Add("@Image", DbType.Object).Value = IO.File.ReadAllBytes(Path)
                                Command.ExecuteNonQuery()
                            End If

                            BackgroundWorker.ReportProgress(0)
                        End If

                        'Delete Intermediate File
                        IO.File.Delete(Path)

                        BackgroundWorker.ReportProgress(0)
                        If BackgroundWorker.CancellationPending Then : DoWorkEvent.Cancel = True : Exit Sub : End If

                        If Year = DownloadEndDate.Year Then
                            If DoY = DownloadEndDate.DayOfYear Then Exit For
                        End If
                    Next

                    Raster.Close()
                    IO.File.Delete(VariablePath)
                Next

            End Using
        End Using
    End Sub

    Function BuildFtpStringDAYMET(Optional Year As Integer = Nothing, Optional Variable As String = Nothing)
        Dim FtpDirectory As String = "ftp://daac.ornl.gov/data/daymet/Daymet_mosaics/data/"

        If Year = Nothing Or Variable = Nothing Then
            Return FtpDirectory
        Else
            Return FtpDirectory & Variable & "_" & Year & ".nc4"
        End If
    End Function

#End Region

#Region "NLDAS_2A"

    ''' <summary>
    ''' Downloads and updates a database storing hourly NLDAS-2 Forcing File A GRIB rasters.
    ''' </summary>
    Sub DownloadNDLAS_2A(DatabasePath As String, DownloadStartDate As DateTime, DownloadEndDate As DateTime, Increment As Integer, BackgroundWorker As System.ComponentModel.BackgroundWorker, DoWorkEvent As System.ComponentModel.DoWorkEventArgs)
        Using Connection = CreateConnection(DatabasePath, False)
            Connection.Open()
            Using Command = Connection.CreateCommand

                'Download NDLAS Files
                Dim First As Integer = DownloadStartDate.Subtract(NLDAS_2AStartDate).TotalHours
                Dim Last As Integer = DownloadEndDate.Subtract(NLDAS_2AStartDate).TotalHours
                Dim Hour As Integer = First

                For Hour = First To Last Step Increment
                    Dim Count As Integer = Math.Min(Last - Hour + 1, Increment) - 1

                    'Start Asynchronous NLDAS Ftp File Downloads
                    Dim FileDownloads(Count) As Threading.Tasks.Task(Of Byte())  '() As Byte
                    Dim I As Integer = 0
                    For I = 0 To Count
                        Dim RecordDate As DateTime = NLDAS_2AStartDate.AddHours(Hour + I)
                        FileDownloads(I) = Threading.Tasks.Task.Factory.StartNew(Function() DownloadFtpFileToBytes(BuildFtpStringNLDAS_2A(RecordDate)))
                        'FileDownloads(I) = IO.File.ReadAllBytes("F:\NLDAS-2\" & RecordDate.Year & "\" & IO.Path.GetFileName(BuildFtpStringNLDAS_2A(RecordDate)))
                    Next
                    Threading.Tasks.Task.WaitAll(FileDownloads)

                    'Add Raster File and Associated Time Stamp into Database
                    If 1 = 2 Then
                        Using Transaction = Connection.BeginTransaction
                            For I = 0 To Count
                                Command.CommandText = "INSERT OR REPLACE INTO Rasters (Date, Image) VALUES (@Date, @Image)"
                                Command.Parameters.Add("@Date", DbType.DateTime).Value = NLDAS_2AStartDate.AddHours(Hour + I)
                                Command.Parameters.Add("@Image", DbType.Object).Value = FileDownloads(I).Result
                                Command.ExecuteNonQuery()
                            Next

                            Transaction.Commit()
                        End Using
                    End If

                    BackgroundWorker.ReportProgress(0, "Downloading...")
                    If BackgroundWorker.CancellationPending Then : DoWorkEvent.Cancel = True : Exit Sub : End If
                Next

            End Using
        End Using

        DownloadNLDAS_2ATopography()
    End Sub

    ''' <summary>
    ''' Creates the path to NLDAS_2A ftp directories and files. 
    ''' </summary>
    ''' <param name="RecordDate">Date of Directory or File of Interest</param>
    ''' <param name="Level">Path to Base Directory [Level=1], Year Directory [Level=2], Day of Year Directory [Level=3], or Hourly File [Level=4]</param>
    ''' <returns>Path to NLDAS_2A Directory or File</returns>
    Function BuildFtpStringNLDAS_2A(RecordDate As DateTime, Optional Level As Integer = 4) As String
        Dim Builder As New System.Text.StringBuilder("ftp://hydro1.sci.gsfc.nasa.gov/data/s4pa//NLDAS/NLDAS_FORA0125_H.002")
        If Level > 1 Then Builder.Append("/" & RecordDate.Year)
        If Level > 2 Then Builder.Append("/" & RecordDate.DayOfYear.ToString("000"))
        If Level > 3 Then Builder.Append("/NLDAS_FORA0125_H.A" & RecordDate.ToString("yyyyMMdd") & "." & RecordDate.ToString("HH") & "00.002.grb")
        Return Builder.ToString
    End Function

    ''' <summary>
    ''' Downloads NLDAS-2 elevation raster.
    ''' </summary>
    ''' <remarks>Converts units from meters to feet.</remarks>
    Sub DownloadNLDAS_2ATopography()
        'Download NLDAS-2A File for Geographic Referencing
        Dim NLDAS_2ARasterPath As String = "/vsimem/gtopomean15k.bin"
        GDAL.Gdal.FileFromMemBuffer(NLDAS_2ARasterPath, DownloadFileToBytes(BuildFtpStringNLDAS_2A(NLDAS_2AStartDate)))

        Dim NLDAS_2AFileNames = {"gtopomean15k.asc", "slope15k.asc", "aspect15k.asc"}
        Dim ProjectFileNames = {NLDAS_2AElevationRasterPath, NLDAS_2ASlopeRasterPath, NLDAS_2AAspectRasterPath}

        Try
            'Open NLDAS-2A File
            Using NLDASRaster = GDAL.Gdal.Open(NLDAS_2ARasterPath, GDAL.Access.GA_ReadOnly)

                For FileName = 0 To 2
                    If Not IO.File.Exists(ProjectFileNames(FileName)) Then

                        'Download NLDAS-2A File
                        Dim File = DownloadFileToString("http://ldas.gsfc.nasa.gov/nldas/asc/" & NLDAS_2AFileNames(FileName))

                        'Get Driver and Create Output Elevation Raster
                        Using Driver = GDAL.Gdal.GetDriverByName("GTiff")
                            Using ElevationRaster = Driver.Create(ProjectFileNames(FileName), NLDASRaster.RasterXSize, NLDASRaster.RasterYSize, 1, GDAL.DataType.GDT_Float32, {"COMPRESS=DEFLATE"})

                                'Convert NLDAS-2A Elevation to Feet and Position in Array 
                                Dim Values(NLDASRaster.RasterXSize * NLDASRaster.RasterYSize - 1) As Single
                                For Each Line In File
                                    Dim Data = Line.Split(New Char() {" "}, StringSplitOptions.RemoveEmptyEntries)
                                    If Data.Length = 5 Then
                                        Dim I = (NLDASRaster.RasterYSize - Data(1)) * NLDASRaster.RasterXSize + Data(0) - 1
                                        If Data(4) = "-9999.0000" Then
                                            Values(I) = -9999
                                        Else
                                            Select Case FileName
                                                Case 0 : Values(I) = ToFeet(Data(4))
                                                Case Else : Values(I) = Data(4)
                                            End Select
                                        End If
                                    End If
                                Next

                                'Write Converted Elevations to Output Raster
                                ElevationRaster.WriteRaster(0, 0, NLDASRaster.RasterXSize, NLDASRaster.RasterYSize, Values, NLDASRaster.RasterXSize, NLDASRaster.RasterYSize, 1, {1}, 0, 0, 0)

                                'Set Georeferencing to Output Elevation Raster
                                ElevationRaster.SetProjection(NLDASRaster.GetProjection)
                                Dim GeoTransform(5) As Double
                                NLDASRaster.GetGeoTransform(GeoTransform)
                                ElevationRaster.SetGeoTransform(GeoTransform)

                                'Set No Data Value
                                Using Band = ElevationRaster.GetRasterBand(1)
                                    Band.SetNoDataValue(-9999)
                                End Using
                            End Using
                        End Using

                    End If
                Next

            End Using
        Catch Exception As Exception
            MsgBox(Exception.Message)
        End Try

        'Release NLDAS-2A File
        GDAL.Gdal.Unlink(NLDAS_2ARasterPath)
    End Sub

    ''' <summary>
    ''' Downloads and updates a database storing hourly NLDAS-2 Forcing File A GRIB rasters.
    ''' </summary>
    ''' <param name="Path">Path to NLDAS_2A SQLite Raster Storage Database</param>
    ''' <remarks>Rasterlite ended up being too slow due excessive/exponential indexing per raster, and so this subroutine is not recommended for the sheer number NLDAS-2 datasets.</remarks>
    Sub UpdateNDLAS_2AtoRasterlite(Path As String)
        'Determine NLDAS-2A Ftp Directory Date Span
        Dim FtpStartDate = New DateTime(1979, 1, 1).AddHours(13)
        Dim Years = DownloadFtpDirectory(BuildFtpStringNLDAS_2A(FtpStartDate, 1))
        Dim MaxYear As Integer = FtpStartDate.Year
        For Each Yr In Years
            If IsNumeric(Yr) Then If Yr > MaxYear Then MaxYear = Yr
        Next

        Dim DaysOfYear = DownloadFtpDirectory(BuildFtpStringNLDAS_2A(New DateTime(MaxYear, 1, 1), 2))
        Dim MaxDayOfYear As Integer = 0
        For Each DoY In DaysOfYear
            If IsNumeric(DoY) Then If DoY > MaxDayOfYear Then MaxDayOfYear = DoY
        Next

        Dim Hours = DownloadFtpDirectory(BuildFtpStringNLDAS_2A(New DateTime(MaxYear - 1, 12, 31).AddDays(MaxDayOfYear), 3))
        Dim MaxHour As Integer = 0
        For Each Hr In Hours
            Dim FileName = Hr.Split(".")
            If FileName.Length > 1 Then If FileName(2) > MaxHour Then MaxHour = FileName(2)
        Next
        MaxHour /= 100
        Dim FtpEndDate = New DateTime(MaxYear - 1, 12, 31).AddDays(MaxDayOfYear).AddHours(MaxHour)

        'Determine Number of Rasters Already in File
        Dim FileEndDate As DateTime = FtpStartDate
        If IO.File.Exists(Path) Then
            Using Connection = CreateConnection(Path, False)
                Connection.Open()
                Using Command = Connection.CreateCommand
                    Command.CommandText = "SELECT Count(name) FROM sqlite_master WHERE TYPE='table' AND name LIKE 'R%_rasters'"
                    FileEndDate = FileEndDate.AddHours(Command.ExecuteScalar)
                End Using
            End Using
        End If

        'Ftp Download Details
        Dim First As Integer = FileEndDate.Subtract(FtpStartDate).TotalHours
        Dim Last As Integer = FtpEndDate.Subtract(FtpStartDate).TotalHours
        Dim Hour As Integer = First
        Dim Increment As Integer = 8

        'Download NDLAS Files
        For Hour = First To Last Step Increment
            Dim Count As Integer = Math.Min(Last - Hour + 1, Increment)
            Dim FileName(Count - 1) As String

            'Start Asynchronous NLDAS Ftp File Downloads
            Threading.Tasks.Parallel.For(0, Count, _
            Sub(I)
                Dim RecordDate As DateTime = FtpStartDate.AddHours(Hour + I)
                'FileName(I) = "F:\NLDAS-2\" & RecordDate.Year & "\" & IO.Path.GetFileName(BuildFtpStringNLDAS_2A(RecordDate))
                FileName(I) = IO.Path.Combine(IO.Path.GetTempPath, "R" & RecordDate.ToString("yyyyMMddHH"))
                DownloadFileToDrive(BuildFtpStringNLDAS_2A(RecordDate), FileName(I))
            End Sub)

            'Add Raster File to Database
            Dim Process As New GDALProcess
            For I = 0 To Count - 1
                Process.Translate({FileName(I)}, "RASTERLITE:" & Path & ",table=" & FileName(I), GDALProcess.RasterFormat.Rasterlite, , , , , , GDAL.DataType.GDT_Float32, """+proj=longlat +ellps=WGS84 +datum=WGS84 +no_defs""", "-co DRIVER=GTiff -co COMPRESS=DEFLATE --config OGR_SQLITE_PRAGMA ""JOURNAL_MODE=OFF,SYNCHRONOUS=OFF,PAGE_SIZE=4096""")
                IO.File.Delete(FileName(I))
            Next
        Next
    End Sub

#End Region

#Region "Helper Functions"

    ''' <summary>
    ''' Downloads names of child directories and files of a specified ftp directory.
    ''' </summary>
    ''' <param name="URI">Ftp Directory Location</param>
    ''' <returns>Alphabetically Sorted Names of Child Directories and Files</returns>
    Function DownloadFtpDirectory(URI As String) As String()
        Dim DirectoryList() As String = {}

        'Create Ftp Request
        Dim Request As Net.FtpWebRequest = Net.FtpWebRequest.Create(URI)
        Request.Method = Net.WebRequestMethods.Ftp.ListDirectory

        Do
            Try
                Using Response = Request.GetResponse
                    Using Reader = New IO.StreamReader(Response.GetResponseStream())
                        DirectoryList = Reader.ReadToEnd.Replace(IO.Path.GetFileName(URI) & "/", "").Replace(vbLf, "").Split(vbNewLine)
                    End Using
                End Using
            Catch Exception As Net.WebException
                If CType(Exception.Response, Net.FtpWebResponse).StatusCode = Net.FtpStatusCode.ActionNotTakenFileUnavailable Then
                    Return Nothing
                End If
            End Try
        Loop Until DirectoryList.Length > 0

        Array.Sort(DirectoryList)
        Return DirectoryList
    End Function

    ''' <summary>
    ''' Downloads an ftp file to memory.
    ''' </summary>
    ''' <param name="URI">Ftp File Location</param>
    ''' <returns>Ftp File Byte Array</returns>
    Function DownloadFtpFileToBytes(URI As String) As Byte()
        Using MemoryStream As New IO.MemoryStream

            'Create Ftp Request
            Dim Request As Net.FtpWebRequest = Net.FtpWebRequest.Create(URI)
            Request.UseBinary = True

            'Request File until Download Succeeds
            Do
                Try
                    Using Response = Request.GetResponse
                        Using Reader = New IO.StreamReader(Response.GetResponseStream())
                            Reader.BaseStream.CopyTo(MemoryStream)
                        End Using
                    End Using
                Catch Exception As Net.WebException
                    If CType(Exception.Response, Net.FtpWebResponse).StatusCode = Net.FtpStatusCode.ActionNotTakenFileUnavailable Then
                        Return Nothing
                    End If
                End Try
            Loop Until MemoryStream.Length > 0

            DownloadFtpFileToBytes = MemoryStream.ToArray()
        End Using
    End Function

    ''' <summary>
    ''' Downloads a file to disk.
    ''' </summary>
    ''' <param name="URI">File Location</param>
    ''' <param name="Path">Disk File Location to Copy File</param>
    Sub DownloadFileToDrive(URI As String, Path As String)
        'Create Web Client
        Using WebClient As New Net.WebClient
            WebClient.Headers.Add(Net.HttpRequestHeader.UserAgent, "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)")
            WebClient.Headers("Accept") = "application/"

            Dim Downloaded As Boolean = False
            Do
                Try
                    WebClient.DownloadFile(URI, Path)
                    Downloaded = True
                Catch Exception As Net.WebException
                    Downloaded = False
                End Try
            Loop Until Downloaded = True
        End Using
    End Sub

    ''' <summary>
    ''' Downloads a file to memory.
    ''' </summary>
    ''' <param name="URI">File Location</param>
    ''' <returns>File Byte Array</returns>
    Function DownloadFileToBytes(URI As String) As Byte()
        DownloadFileToBytes = {}

        'Create Web Client
        Using WebClient As New Net.WebClient
            Dim Downloaded As Boolean = False
            Do
                Try
                    Using MemoryStream = New IO.MemoryStream(WebClient.DownloadData(URI))
                        DownloadFileToBytes = MemoryStream.ToArray
                    End Using
                    Downloaded = True
                Catch
                    Downloaded = False
                End Try
            Loop Until Downloaded = True
        End Using
    End Function

    ''' <summary>
    ''' Downloads a file to memory.
    ''' </summary>
    ''' <param name="URI">File Location</param>
    ''' <returns>File Line String Array</returns>
    Function DownloadFileToString(URI As String) As String()
        DownloadFileToString = {}

        'Create Web Client
        Using WebClient As New Net.WebClient
            Dim Downloaded As Boolean = False
            Do
                Try
                    Using Reader = New IO.StreamReader(WebClient.OpenRead(URI))
                        DownloadFileToString = Reader.ReadToEnd.Split(vbLf)
                    End Using
                    Downloaded = True
                Catch
                    Downloaded = False
                End Try
            Loop Until Downloaded = True
        End Using
    End Function

    Function DownloadWebpage(ByVal URI As String) As String
        Dim Response As String = ""

        Dim Completed = False
        Do
            Try
                'Create Http Request
                Dim Request As Net.HttpWebRequest = Net.WebRequest.Create(URI)
                Request.UserAgent = "Mozilla"

                Using HttpResponse As Net.HttpWebResponse = Request.GetResponse()
                    Using Buffered = New IO.BufferedStream(HttpResponse.GetResponseStream())
                        Using Reader As New IO.StreamReader(Buffered)
                            Response = Reader.ReadToEnd()
                        End Using
                    End Using
                End Using

                Completed = True
            Catch Exception As Net.WebException
                If Exception.Message.Contains("The remote server returned an error: (404) Not Found.") Then Exit Do
            End Try
        Loop Until Completed

        Return Response
    End Function

#End Region

End Module

'Sub UpdateNDLAS2A(Directory As String)
'    'Create Directory and Raster Files If They Do Not Already Exist
'    Dim Level = "NLDAS_2_A\"
'    If Not IO.Directory.Exists(Directory & Level) Then IO.Directory.CreateDirectory(Directory & Level)

'    For Each Parameter In [Enum].GetNames(GetType(Hourly))
'        Dim ParameterPath = Directory & Level & Parameter

'        If Not IO.File.Exists(ParameterPath & ".bsq") Then
'            IO.File.WriteAllText(ParameterPath & ".bsq", "")
'            IO.File.WriteAllText(ParameterPath & ".prj", RasterNLDAS2A.Projection)
'            IO.File.WriteAllText(ParameterPath & ".hdr", RasterNLDAS2A.GetHeader(0, -9999, RasterDataType.Float32))
'        End If
'    Next

'    'Determine NLDAS Date Span for Download
'    Dim NLDASStartDate = New DateTime(1979, 1, 1).AddHours(13)
'    Dim Years = DownloadFtpDirectory(BuildFtpStringNLDAS_2_A(NLDASStartDate, 1))
'    Dim MaxYear As Integer = NLDASStartDate.Year
'    For Each Yr In Years
'        If IsNumeric(Yr) Then If Yr > MaxYear Then MaxYear = Yr
'    Next

'    Dim DaysOfYear = DownloadFtpDirectory(BuildFtpStringNLDAS_2_A(New DateTime(MaxYear, 1, 1), 2))
'    Dim MaxDayOfYear As Integer = 0
'    For Each DoY In DaysOfYear
'        If IsNumeric(DoY) Then If DoY > MaxDayOfYear Then MaxDayOfYear = DoY
'    Next

'    Dim Hours = DownloadFtpDirectory(BuildFtpStringNLDAS_2_A(New DateTime(MaxYear - 1, 12, 31).AddDays(MaxDayOfYear), 3))
'    Dim MaxHour As Integer = 0
'    For Each Hr In Hours
'        Dim FileName = Hr.Split(".")
'        If FileName.Length > 1 Then If FileName(2) > MaxHour Then MaxHour = FileName(2)
'    Next
'    MaxHour /= 100
'    Dim NLDASEndDate = New DateTime(MaxYear - 1, 12, 31).AddDays(MaxDayOfYear).AddHours(MaxHour)

'    Dim FileInfo As New IO.FileInfo(Directory & Level & [Enum].GetNames(GetType(Hourly))(0) & ".bsq")
'    Dim ParameterEndDate = NLDASStartDate.AddHours(FileInfo.Length / (RasterNLDAS2A.XCount * RasterNLDAS2A.YCount) / 4)

'    'Download NDLAS Files and Update Parameter Raster Files
'    Dim First As Integer = ParameterEndDate.Subtract(NLDASStartDate).TotalHours
'    Dim Last As Integer = NLDASEndDate.Subtract(NLDASStartDate).TotalHours
'    Dim Hour As Integer = First
'    Dim Increment As Integer = 8

'    For Hour = First To Last Step Increment
'        Dim Count As Integer = Math.Min(Last - Hour, Increment)

'        'Start Asynchronous NLDAS Ftp File Downloads and Conversions
'        Dim Tasks(Count - 1) As Threading.Tasks.Task(Of Single()())
'        Dim I As Integer = 0
'        For I = 0 To Count - 1
'            Tasks(I) = Threading.Tasks.Task.Factory.StartNew(Function() DownloadNLDAS2A(NLDASStartDate.AddHours(Hour + I)), Threading.CancellationToken.None, Threading.Tasks.TaskCreationOptions.None, Threading.Tasks.TaskScheduler.Default)
'        Next
'        Threading.Tasks.Task.WaitAll(Tasks)

'        'Write Reduced Datasets to Raster File
'        For P = 0 To [Enum].GetNames(GetType(Hourly)).Length - 1
'            Dim ParameterPath = Directory & Level & [Enum].GetName(GetType(Hourly), P)

'            Using FileStream As New IO.FileStream(ParameterPath & ".bsq", IO.FileMode.Append)
'                For I = 0 To Count - 1
'                    Dim Bytes(Tasks(I).Result(P).Length * 4 - 1) As Byte
'                    Buffer.BlockCopy(Tasks(I).Result(P), 0, Bytes, 0, Bytes.Length)
'                    FileStream.Write(Bytes, 0, Bytes.Length)
'                Next
'            End Using

'            IO.File.WriteAllText(ParameterPath & ".hdr", RasterNLDAS2A.GetHeader(Hour + Count, -9999, RasterDataType.Float32))
'        Next
'    Next
'End Sub

'Function CopyNLDAS2AFileToRAM(RecordDate As DateTime) As Byte()
'    Dim Path = "F:\NLDAS-2\" & RecordDate.Year & "\NLDAS_FORA0125_H.A" & RecordDate.ToString("yyyyMMdd") & "." & RecordDate.ToString("HH") & "00.002.grb"
'    CopyNLDAS2AFileToRAM = IO.File.ReadAllBytes(Path)
'End Function
