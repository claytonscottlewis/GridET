'            Copyright Clayton S. Lewis 2014-2018.
'   Distributed under the Boost Software License, Version 1.0.
'      (See accompanying file GridET License.rtf or copy at
'            http://www.boost.org/LICENSE_1_0.txt)

Module Download

#Region "DAYMET"

    Sub DownloadDAYMET(ByRef Client As EarthDataClient, ByVal DatabasePath As String, ByVal DownloadStartDate As DateTime, ByVal DownloadEndDate As DateTime, ByVal Variable As String, ByVal BackgroundWorker As System.ComponentModel.BackgroundWorker, ByVal DoWorkEvent As System.ComponentModel.DoWorkEventArgs)
        Using Connection = CreateConnection(DatabasePath, False), Command = Connection.CreateCommand : Connection.Open()

            'Download DAYMET Files
            Dim Year As Integer = 0
            For Year = DownloadStartDate.Year To DownloadEndDate.Year
                'Start DAYMET Ftp File Download
                Dim VariablePath As String = IO.Path.GetTempFileName
                Client.DownloadFile(BuildStringDAYMET(Year, Variable), VariablePath)

                Dim Projection As String, GeoTransform() As Double, BandNoDataValue() As Double
                Using Raster As New Raster(String.Format("NETCDF:""{0}"":{1}", VariablePath, Variable), GDAL.Access.GA_ReadOnly)

                    Projection = Raster.Projection
                    GeoTransform = Raster.GeoTransform
                    BandNoDataValue = Raster.BandNoDataValue

                End Using

                Using HRaster As New Raster(String.Format("HDF5:""{0}""://{1}", VariablePath, Variable), GDAL.Access.GA_ReadOnly)
                    Dim XCount = HRaster.XCount
                    Dim YCount = HRaster.YCount

                    'Determine Start And End Dates
                    Dim StartDoY As Integer = 1
                    If Year = DownloadStartDate.Year Then StartDoY = DownloadStartDate.DayOfYear
                    If StartDoY = 366 Then StartDoY = 365

                    Dim EndDoY As Integer = 365
                    If Year = DownloadEndDate.Year Then EndDoY = DownloadEndDate.DayOfYear
                    If EndDoY = 366 Then EndDoY = 365

                    Dim Data(XCount * YCount - 1) As Single
                    For DoY = StartDoY To EndDoY
                        Dim Path = IO.Path.Combine(IO.Path.GetTempPath, String.Format("Daymet_{0}_{1}.tif", Year, DoY))

                        Using Driver = GDAL.Gdal.GetDriverByName(GDALProcess.RasterFormat.GTiff.ToString)
                            Using Dataset = Driver.Create(Path, XCount, YCount, 1, GDAL.DataType.GDT_Float32, {"COMPRESS=DEFLATE", "TILED=YES"})
                                Dataset.SetProjection(Projection)
                                Dataset.SetGeoTransform(GeoTransform)

                                Using Band = Dataset.GetRasterBand(1)
                                    Band.SetDescription(Variable)
                                    Band.SetNoDataValue(Single.MinValue)
                                End Using

                                HRaster.Dataset.ReadRaster(0, 0, XCount, YCount, Data, XCount, YCount, 1, {DoY}, 0, 0, 0)

                                For I = 0 To Data.Length - 1
                                    If Data(I) = BandNoDataValue(DoY - 1) Then Data(I) = Single.MinValue
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

                End Using

                'HDF5 Driver Bug Holds Onto File
                'IO.File.Delete(VariablePath)
            Next

        End Using

        Client.Dispose()
    End Sub

    Function BuildStringDAYMET(Optional ByVal Year As Integer = Nothing, Optional ByVal Variable As String = Nothing)
        Dim Directory = "https://daac.ornl.gov/daacdata/daymet/Daymet_V3_CFMosaics/data/CFMosaic_NA"

        If Year = Nothing OrElse Variable = Nothing Then
            Return Directory
        Else
            Return String.Format(Directory & "/daymet_v3_{0}_{1}_na.nc4", Variable, Year)
        End If
    End Function

#End Region

#Region "NLDAS_2A"

    ''' <summary>
    ''' Downloads and updates a database storing hourly NLDAS-2 Forcing File A GRIB rasters.
    ''' </summary>
    Sub DownloadNDLAS_2A(ByRef Client As EarthDataClient, ByVal DatabasePath As String, ByVal DownloadStartDate As DateTime, ByVal DownloadEndDate As DateTime, ByVal Increment As Integer, ByVal BackgroundWorker As System.ComponentModel.BackgroundWorker, ByVal DoWorkEvent As System.ComponentModel.DoWorkEventArgs)
        Using Connection = CreateConnection(DatabasePath, False), Command = Connection.CreateCommand : Connection.Open()

            'Download NDLAS Files
            Dim First As Integer = DownloadStartDate.Subtract(NLDAS_2AStartDate).TotalHours
            Dim Last As Integer = DownloadEndDate.Subtract(NLDAS_2AStartDate).TotalHours
            Dim Hour As Integer = First

            For Hour = First To Last Step Increment
                Dim Count As Integer = Math.Min(Last - Hour + 1, Increment) - 1

                'Start Asynchronous NLDAS Ftp File Downloads
                Dim FileDownloads(Count)() As Byte
                Dim I As Integer = 0
                For I = 0 To Count
                    Dim RecordDate As DateTime = NLDAS_2AStartDate.AddHours(Hour + I)
                    FileDownloads(I) = Client.DownloadData(BuildStringNLDAS_2A(RecordDate))
                Next

                'Add Raster File and Associated Time Stamp into Database
                Using Transaction = Connection.BeginTransaction
                    For I = 0 To Count
                        Command.CommandText = "INSERT OR REPLACE INTO Rasters (Date, Image) VALUES (@Date, @Image)"
                        Command.Parameters.Add("@Date", DbType.DateTime).Value = NLDAS_2AStartDate.AddHours(Hour + I)
                        Command.Parameters.Add("@Image", DbType.Object).Value = FileDownloads(I)
                        Command.ExecuteNonQuery()
                    Next

                    Transaction.Commit()
                End Using

                BackgroundWorker.ReportProgress(0, "Downloading...")
                If BackgroundWorker.CancellationPending Then : DoWorkEvent.Cancel = True : Exit Sub : End If
            Next

        End Using

        DownloadNLDAS_2ATopography(Client)

        Client.Dispose()
    End Sub

    ''' <summary>
    ''' Creates the path to NLDAS_2A ftp directories and files. 
    ''' </summary>
    ''' <param name="RecordDate">Date of Directory or File of Interest</param>
    ''' <param name="Level">Path to Base Directory [Level=1], Year Directory [Level=2], Day of Year Directory [Level=3], or Hourly File [Level=4]</param>
    ''' <returns>Path to NLDAS_2A Directory or File</returns>
    Function BuildStringNLDAS_2A(ByVal RecordDate As DateTime, Optional ByVal Level As Integer = 4) As String
        Dim Builder As New System.Text.StringBuilder("https://hydro1.gesdisc.eosdis.nasa.gov/data/NLDAS/NLDAS_FORA0125_H.002")
        If Level > 1 Then Builder.Append("/" & RecordDate.Year)
        If Level > 2 Then Builder.Append("/" & RecordDate.DayOfYear.ToString("000"))
        If Level > 3 Then Builder.Append("/NLDAS_FORA0125_H.A" & RecordDate.ToString("yyyyMMdd") & "." & RecordDate.ToString("HH") & "00.002.grb")
        Return Builder.ToString
    End Function

    ''' <summary>
    ''' Downloads NLDAS-2 elevation raster.
    ''' </summary>
    ''' <remarks>Converts units from meters to feet.</remarks>
    Sub DownloadNLDAS_2ATopography(ByRef Client As EarthDataClient)
        'Download NLDAS-2A File for Geographic Referencing
        Dim NLDAS_2ARasterPath As String = "/vsimem/gtopomean15k.bin"
        GDAL.Gdal.FileFromMemBuffer(NLDAS_2ARasterPath, Client.DownloadData(BuildStringNLDAS_2A(NLDAS_2AStartDate)))

        Dim NLDAS_2AFileNames = {"gtopomean15k.asc", "slope15k.asc", "aspect15k.asc"}
        Dim ProjectFileNames = {NLDAS_2AElevationRasterPath, NLDAS_2ASlopeRasterPath, NLDAS_2AAspectRasterPath}

        Try
            'Open NLDAS-2A File
            Using NLDASRaster = GDAL.Gdal.Open(NLDAS_2ARasterPath, GDAL.Access.GA_ReadOnly)

                For FileName = 0 To 2
                    If Not IO.File.Exists(ProjectFileNames(FileName)) Then

                        'Download NLDAS-2A File
                        Dim File = Client.DownloadString("https://ldas.gsfc.nasa.gov/nldas/asc/" & NLDAS_2AFileNames(FileName))

                        'Get Driver and Create Output Elevation Raster
                        Using Driver = GDAL.Gdal.GetDriverByName("GTiff")
                            Using ElevationRaster = Driver.Create(ProjectFileNames(FileName), NLDASRaster.RasterXSize, NLDASRaster.RasterYSize, 1, GDAL.DataType.GDT_Float32, {"COMPRESS=DEFLATE"})

                                'Convert NLDAS-2A Elevation to Feet and Position in Array 
                                Dim Values(NLDASRaster.RasterXSize * NLDASRaster.RasterYSize - 1) As Single
                                For Each Line In File.Split(vbLf)
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

#End Region

#Region "Helper Functions"

    Class EarthDataClient : Implements IDisposable

        Private WithEvents Client As ExtendedWebClient

        Sub New(ByVal UserName As String, ByVal Password As String)
            Client = New ExtendedWebClient(UserName, Password)
        End Sub

        Private Class ExtendedWebClient : Inherits Net.WebClient

            Private CookieContainer As New Net.CookieContainer()
            Private CredentialCache As New Net.CredentialCache()

            Sub New(ByVal UserName As String, ByVal Password As String)
                CredentialCache.Add(New Uri("https://urs.earthdata.nasa.gov"), "Basic", New Net.NetworkCredential(UserName, Password))

                Me.DownloadData("https://hydro1.gesdisc.eosdis.nasa.gov/data/NLDAS/NLDAS_FORA0125_H.002/doc/gribtab_NLDAS_FORA_hourly.002.txt")
            End Sub

            Protected Overrides Function GetWebRequest(ByVal address As Uri) As Net.WebRequest
                Dim Request = DirectCast(MyBase.GetWebRequest(address), Net.HttpWebRequest)

                With Request
                    .Method = "GET"
                    .Credentials = CredentialCache
                    .CookieContainer = CookieContainer
                    .PreAuthenticate = False
                    .AllowAutoRedirect = True
                End With

                Return Request
            End Function

        End Class


        Dim Disposed As Boolean = False
        Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

        Protected Overridable Sub Dispose(ByVal Disposing As Boolean)
            If Disposed Then Return

            If Disposing Then
                Client.Dispose()
            End If

            Disposed = True
        End Sub

        ''' <summary>
        ''' Downloads and stores a text file by splitting each line into a string array.
        ''' </summary>
        ''' <param name="URL">File Location</param>
        ''' <returns>File Line String Array</returns>
        Function DownloadString(ByVal URL As String) As String
            DownloadString = ""

            Dim Downloaded As Boolean = False
            Do
                Try
                    DownloadString = Client.DownloadString(URL)
                    Downloaded = True
                Catch : End Try
            Loop Until Downloaded = True
        End Function

        ''' <summary>
        ''' Downloads and stores a file in a byte array.
        ''' </summary>
        ''' <param name="URL">File Location</param>
        ''' <returns>File Byte Array</returns>
        Function DownloadData(ByVal URL As String) As Byte()
            DownloadData = {}

            Dim Downloaded As Boolean = False
            Do
                Try
                    DownloadData = Client.DownloadData(URL)
                    Downloaded = True
                Catch : End Try
            Loop Until Downloaded = True
        End Function

        ''' <summary>
        ''' Downloads a file to disk.
        ''' </summary>
        ''' <param name="URL">File Location</param>
        ''' <param name="Path">Disk File Location to Copy File</param>
        Sub DownloadFile(ByVal URL As String, ByVal Path As String)
            Dim Downloaded As Boolean = False
            Do
                Try
                    Client.DownloadFile(URL, Path)
                    Downloaded = True
                Catch : End Try
            Loop Until Downloaded = True
        End Sub

        ''' <summary>
        ''' Downloads http directory structure.
        ''' </summary>
        ''' <param name="URL">File Location</param>
        ''' <returns>Alphabetically Sorted Names of Child Files</returns>
        Function DownloadFileNames(ByVal URL As String)
            Static IllegalCharacters As New HashSet(Of Char)(IO.Path.GetInvalidFileNameChars)

            Return DownloadString(URL).Split({"><a href="""}, StringSplitOptions.RemoveEmptyEntries) _
                                      .Select(Function(T) T.Split("""")(0)).Distinct _
                                      .Where(Function(T) Not T.Intersect(IllegalCharacters).Any) _
                                      .OrderBy(Function(T) T).ToArray()
        End Function

        ''' <summary>
        ''' Downloads http directory structure.
        ''' </summary>
        ''' <param name="URL">File Location</param>
        ''' <returns>Alphabetically Sorted Names of Child Directories</returns>
        Function DownloadDirectoryNames(ByVal URL As String)
            Static IllegalCharacters As New HashSet(Of Char)(IO.Path.GetInvalidFileNameChars.Where(Function(C) C <> "/"))

            Return DownloadString(URL).Split({"><a href="""}, StringSplitOptions.RemoveEmptyEntries) _
                                      .Select(Function(T) T.Split("""")(0)).Distinct _
                                      .Where(Function(T) Not T.Intersect(IllegalCharacters).Any AndAlso T.EndsWith("/")) _
                                      .Select(Function(T) T.Replace("/", "")) _
                                      .OrderBy(Function(T) T).ToArray()
        End Function

    End Class

#End Region

End Module