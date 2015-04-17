Class GDALProcess

#Region "Variables"

    Private ReadOnly Property GDALDirectory As String
        Get
            Return IO.Path.Combine(Application.StartupPath, "Dependencies\GDAL")
        End Get
    End Property

    Public Property Log As New System.Text.StringBuilder

    Public Property Process As Process

#End Region

#Region "Process Diagnostics"

    Sub New()

    End Sub

    Private Sub Start(Process As Process)
        Process.StartInfo.UseShellExecute = False
        Process.StartInfo.CreateNoWindow = True
        Process.StartInfo.RedirectStandardInput = True
        Process.StartInfo.RedirectStandardOutput = True
        Process.StartInfo.RedirectStandardError = True
        Process.EnableRaisingEvents = True
        AddHandler Process.OutputDataReceived, AddressOf Process_OutputDataReceived
        AddHandler Process.ErrorDataReceived, AddressOf Process_ErrorDataReceived
        Log.Clear()

        Process.Start()
        Process.BeginOutputReadLine()
        Process.BeginErrorReadLine()
        Process.WaitForExit()
    End Sub

    Private Sub Process_OutputDataReceived(Process As Process, Output As System.Diagnostics.DataReceivedEventArgs)
        If Not Output.Data Is Nothing Then Log.Append(Output.Data.ToString & Environment.NewLine)
    End Sub

    Private Sub Process_ErrorDataReceived(Process As Process, Err As System.Diagnostics.DataReceivedEventArgs)
        If Not Err.Data Is Nothing Then Log.Append(Err.Data.ToString & Environment.NewLine)
    End Sub

    Public Shared Sub SetEnvironmentVariable(VariableName As String, Value As String)
        Environment.SetEnvironmentVariable(VariableName, Value)
        GDAL.Gdal.SetConfigOption(VariableName, Value)
    End Sub

#End Region

#Region "Utilities"

    Function RunCommand(Filename As FileName, Arguments As String) As String
        Process = New Process
        Process.StartInfo.FileName = IO.Path.Combine(GDALDirectory, Filename.ToString)
        Process.StartInfo.Arguments = Arguments

        Start(Process)
        Return Log.ToString
        Process.Dispose()
    End Function

    Function BuildVRT(InPath() As String, OutPath As String, Optional NoData() As String = Nothing, Optional VirtualNoData() As String = Nothing, Optional Band() As String = Nothing) As String
        Process = New Process
        Process.StartInfo.FileName = IO.Path.Combine(GDALDirectory, "gdalbuildvrt.exe")

        If IO.File.Exists(OutPath) Then IO.File.Delete(OutPath)

        Dim TempPath = IO.Path.GetTempFileName
        Using Writer As New IO.StreamWriter(TempPath)
            For Each Path In InPath
                Writer.WriteLine(Path)
            Next
        End Using

        Dim Command As New System.Text.StringBuilder()
        If NoData IsNot Nothing Then
            Command.Append(" -srcnodata """)
            For Each Value In NoData
                Command.Append(Value & " ")
            Next
            Command(Command.Length - 1) = """"""
        End If
        If VirtualNoData IsNot Nothing Then
            Command.Append(" -vrtnodata """)
            For Each Value In VirtualNoData
                Command.Append(Value & " ")
            Next
            Command(Command.Length - 1) = """"""
        End If
        If Band IsNot Nothing Then
            Command.Append(" -b """ & String.Join(",", Band) & """")
        End If
        Command.Append(" -input_file_list """ & TempPath & """")
        Command.Append(" """ & OutPath & """")
        Process.StartInfo.Arguments = Command.ToString

        Start(Process)

        IO.File.Delete(TempPath)
        Return Log.ToString
        Process.Dispose()
    End Function

    Function Translate(InPath() As String, OutPath As String, Optional RasterFormat As RasterFormat = RasterFormat.GTiff, Optional Compression As Compression = GDALProcess.Compression.NONE, Optional Extent As Extent = Nothing, Optional NoData As String = Nothing, Optional BigTiff As Boolean = False, Optional Unscale As Boolean = False, Optional DataType As GDAL.DataType = GDAL.DataType.GDT_Unknown, Optional Projection As String = Nothing, Optional DatabaseOptions As String = Nothing) As String
        Process = New Process
        Process.StartInfo.FileName = IO.Path.Combine(GDALDirectory, "gdal_translate.exe")

        Dim Command As New System.Text.StringBuilder()
        If Not DataType = GDAL.DataType.GDT_Unknown Then Command.Append(" -ot " & GetDataType(DataType))
        Command.Append(GetRasterFormatString(RasterFormat))
        If Unscale Then Command.Append(" -unscale")
        If Not Extent Is Nothing Then Command.Append(GetExtentString(Extent))
        If Not Projection Is Nothing Then Command.Append(" -a_srs " & Projection)
        If Not NoData Is Nothing Then Command.Append(" -a_nodata " & NoData)
        Command.Append(GetCompressionString(Compression, RasterFormat))
        If RasterFormat = RasterFormat.GTiff Then If BigTiff Then Command.Append(" -co ""BIGTIFF=YES""")
        For Each Path In InPath
            Command.Append(" """ & Path & """")
        Next
        Command.Append(" """ & OutPath & """")
        If Not DatabaseOptions Is Nothing Then Command.Append(" " & DatabaseOptions)
        Process.StartInfo.Arguments = Command.ToString

        Start(Process)
        Return Log.ToString
        Process.Dispose()
    End Function

    Function Warp(InPath As String, OutPath As String, TargetSpatialReference As String, Optional CutlinePath As String = Nothing, Optional TargetExtent As Extent = Nothing, Optional TargetXResolution As Double = Nothing, Optional TargetYResolution As Double = Nothing, Optional ResamplingMethod As ResamplingMethod = ResamplingMethod.Average, Optional RasterFormat As RasterFormat = RasterFormat.GTiff, Optional Compression As Compression = GDALProcess.Compression.NONE, Optional InNoData() As String = Nothing, Optional OutNoData() As String = Nothing, Optional OverWrite As Boolean = False) As String
        Process = New Process
        Process.StartInfo.FileName = IO.Path.Combine(GDALDirectory, "gdalwarp.exe")

        Dim Command As New System.Text.StringBuilder()
        Command.Append(" -t_srs """ & TargetSpatialReference & """")
        If Not TargetExtent Is Nothing Then Command.Append(" -te " & TargetExtent.Xmin & " " & TargetExtent.Ymin & " " & TargetExtent.Xmax & " " & TargetExtent.Ymax)
        If Not TargetXResolution = Nothing Then Command.Append(" -tr " & TargetXResolution & " " & TargetYResolution)
        Command.Append(GetResamplingMethodString(ResamplingMethod))
        Command.Append(" -multi")
        Command.Append(GetRasterFormatString(RasterFormat))
        Command.Append(GetCompressionString(Compression, RasterFormat))
        If CutlinePath IsNot Nothing Then Command.Append(" -cutline """ & CutlinePath & """ -crop_to_cutline")
        If InNoData IsNot Nothing Then
            Command.Append(" -srcnodata """)
            For Each Value In InNoData
                Command.Append(Value & " ")
            Next
            Command(Command.Length - 1) = """"
        End If
        If OutNoData IsNot Nothing Then
            Command.Append(" -dstnodata """)
            For Each Value In OutNoData
                Command.Append(Value & " ")
            Next
            Command(Command.Length - 1) = """"
        End If
        If OverWrite Then Command.Append(" -overwrite")
        Command.Append(" """ & InPath & """")
        Command.Append(" """ & OutPath & """")
        Process.StartInfo.Arguments = Command.ToString

        Start(Process)
        Return Log.ToString
        Process.Dispose()
    End Function

    Function DeleteRaster(InPath As String) As String
        Process = New Process
        Process.StartInfo.FileName = IO.Path.Combine(GDALDirectory, "gdalmanage.exe")

        Dim Command As New System.Text.StringBuilder()
        Command.Append("delete """ & InPath & """")
        Process.StartInfo.Arguments = Command.ToString

        Start(Process)
        Return Log.ToString
        Process.Dispose()
    End Function

    Function TileIndex(InPath() As String, OutPath As String, Optional VectorFormat As VectorFormat = VectorFormat.ESRI_Shapefile) As String
        Process = New Process
        Process.StartInfo.FileName = """" & IO.Path.Combine(GDALDirectory, "gdaltindex.exe")

        Dim Command As New System.Text.StringBuilder()
        Command.Append(GetVectorFormatString(VectorFormat))
        Command.Append(" -tileindex PATH")
        Command.Append(" -write_absolute_path")
        Command.Append(" -src_srs_name SRS")
        Command.Append(" -src_srs_format PROJ")
        Command.Append(" """ & OutPath & """")

        Dim OptRasterPath = IO.Path.GetTempFileName
        Using Writer As New IO.StreamWriter(OptRasterPath)
            For Each Path In InPath
                Writer.WriteLine("""" & Path & """")
            Next
        End Using
        Command.Append(" --optfile """ & OptRasterPath & """")

        Process.StartInfo.Arguments = Command.ToString

        Start(Process)

        Process.WaitForExit()

        IO.File.Delete(OptRasterPath)
        Return Log.ToString
        Process.Dispose()
    End Function

    Function DEM(InPath As String, OutPath As String, DEMOutput As DEMDerivative, Optional RasterFormat As RasterFormat = RasterFormat.GTiff, Optional Compression As Compression = GDALProcess.Compression.NONE) As String
        Process = New Process
        Process.StartInfo.FileName = IO.Path.Combine(GDALDirectory, "gdaldem.exe")

        Dim Command As New System.Text.StringBuilder()
        Command.Append(" " & GetEnumName(DEMOutput).ToLower)
        Command.Append(" """ & InPath & """")
        Command.Append(" """ & OutPath & """")
        If DEMOutput = GDALProcess.DEMDerivative.Aspect Then Command.Append(" -zero_for_flat")
        Command.Append(GetRasterFormatString(RasterFormat))
        Command.Append(GetCompressionString(Compression, RasterFormat))
        Process.StartInfo.Arguments = Command.ToString

        Start(Process)
        Return Log.ToString
        Process.Dispose()
    End Function

    Function Rasterize(InPath As String, OutPath As String, Resolution As String, Optional RasterFormat As RasterFormat = RasterFormat.GTiff, Optional Compression As Compression = GDALProcess.Compression.DEFLATE, Optional BurnValue As String = Nothing, Optional NoData As String = Nothing, Optional DataType As GDAL.DataType = GDAL.DataType.GDT_Byte) As String
        Process = New Process
        Process.StartInfo.FileName = IO.Path.Combine(GDALDirectory, "gdal_rasterize.exe")

        Dim Command As New System.Text.StringBuilder()
        If Not BurnValue Is Nothing Then Command.Append(" -burn " & BurnValue)
        Command.Append(GetRasterFormatString(RasterFormat))
        If Not NoData Is Nothing Then Command.Append(" -a_nodata " & NoData & " -init " & NoData)
        Command.Append(GetCompressionString(Compression, RasterFormat))
        Command.Append(" -tr " & Resolution & " " & Resolution & " -tap")
        If Not DataType = GDAL.DataType.GDT_Unknown Then Command.Append(" -ot " & GetDataType(DataType))
        Command.Append(" """ & InPath & """")
        Command.Append(" """ & OutPath & """")
        Process.StartInfo.Arguments = Command.ToString

        Start(Process)
        Return Log.ToString
        Process.Dispose()
    End Function

    Function Info(InPath As String, Optional SubDatasetName As String = Nothing, Optional Brief As Boolean = False, Optional GetMinMax As Boolean = False, Optional GetStatistics As Boolean = False) As String
        Process = New Process
        Process.StartInfo.FileName = IO.Path.Combine(GDALDirectory, "gdalinfo.exe")

        Dim Command As New System.Text.StringBuilder()
        If GetMinMax Then Command.Append(" -mm")
        If GetStatistics Then Command.Append(" -stats")
        If Brief Then Command.Append(" -nogcp -nomd -norat -noct")
        If Not SubDatasetName = Nothing Then Command.Append(" -sd """ & SubDatasetName & """")
        Command.Append(" """ & InPath & """")
        Process.StartInfo.Arguments = Command.ToString

        Start(Process)
        Return Log.ToString
        Process.Dispose()
    End Function

    Function Ogr2Ogr(InPath As String, OutPath As String, Optional VectorFormat As VectorFormat = VectorFormat.SQLite, Optional Reprojection As OSR.SpatialReference = Nothing, Optional Overwrite As Boolean = True)
        Process = New Process
        Process.StartInfo.FileName = IO.Path.Combine(GDALDirectory, "ogr2ogr.exe")

        Dim TemporaryPaths As New List(Of String)

        Dim Command As New System.Text.StringBuilder()
        If Reprojection IsNot Nothing Then
            Dim ReprojectionPath As String = IO.Path.GetTempFileName
            Dim WKT As String = ""
            Reprojection.ExportToWkt(WKT)
            IO.File.WriteAllText(ReprojectionPath, WKT)
            TemporaryPaths.Add(ReprojectionPath)

            Command.Append(String.Format(" -t_srs ""{0}""", ReprojectionPath))
        End If
        Command.Append(GetVectorFormatString(VectorFormat))
        If Overwrite Then Command.Append(" -overwrite")
        If VectorFormat = GDALProcess.VectorFormat.SQLite Then
            Command.Append(" -gt ""65536""")
            If IO.File.Exists(OutPath) Then IO.File.Delete(OutPath)
        End If
        Command.Append(String.Format(" ""{0}""", OutPath))
        Command.Append(String.Format(" ""{0}""", InPath))
        Process.StartInfo.Arguments = Command.ToString

        Start(Process)
        For Each Path In TemporaryPaths
            IO.File.Delete(Path)
        Next
        Return Log.ToString
        Process.Dispose()
    End Function

    'Sub Calc(InPath() As String, OutPath As String, Equation As String, Optional RasterFormat As RasterFormat = RasterFormat.GTiff, Optional Compression As Compression = GDALProcess.Compression.DEFLATE, Optional NoData As String = Nothing)
    '   process =  New Process
    '        Process.StartInfo.FileName = "C:\Users\A00578752\Documents\x32-gdal-1-11-0-mapserver-6-4-1\bin\gdal\python\scripts\gdal_calc.py" 'GDALDirectory & "gdal_calc.py"

    '        Dim Command As New System.Text.StringBuilder()
    '        Command.Append("--calc=""" & Equation & """")
    '        Dim Alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
    '        For P = 0 To InPath.Length - 1
    '            Command.Append(" -" & Alpha(P) & " """ & InPath(P) & """")
    '        Next
    '        Command.Append(" --outfile=""" & OutPath & """")
    '        Command.Append(GetRasterFormatString(RasterFormat).Replace(" -of ", " --format="))
    '        Command.Append(GetCompressionString(Compression, RasterFormat).Replace(" -co ", " --co="))
    '        If Not NoData Is Nothing Then Command.Append(" --NoDataValue=" & NoData)
    '        Command.Append(" --overwrite")
    '        Process.StartInfo.Arguments = Command.ToString

    '        Start(Process)
    '    process.dispose
    'End Sub

#End Region

#Region "Helper Functions"

    Private Function GetRasterFormatString(RasterFormat As RasterFormat) As String
        Return " -of " & GetEnumName(RasterFormat)
    End Function

    Private Function GetResamplingMethodString(ResamplingMethod As ResamplingMethod) As String
        Return " -r " & GetEnumName(ResamplingMethod).ToLower
    End Function

    Private Function GetVectorFormatString(VectorFormat As VectorFormat) As String
        Return String.Format(" -f ""{0}""", GetEnumName(VectorFormat))
    End Function

    Private Function GetExtentString(Extent As Extent) As String
        Return " -projwin " & Extent.Xmin & " " & Extent.Ymax & " " & Extent.Xmax & " " & Extent.Ymin
    End Function

    Private Function GetCompressionString(Compression As Compression, RasterFormat As RasterFormat) As String
        Dim Format As String = ""

        Select Case RasterFormat
            Case RasterFormat.HFA
                Select Case Compression
                    Case Is <> GDALProcess.Compression.NONE
                        Format = "YES"
                End Select
            Case RasterFormat.GTiff
                Format = GetEnumName(Compression)
        End Select

        If Format.Length > 1 Then Format = " -co ""COMPRESS=" & Format & """"
        Return Format
    End Function

    Private Function GetDataType(DataType As GDAL.DataType)
        Return GetEnumName(DataType).Replace("GDT ", "")
    End Function

#End Region

#Region "Enums"

    Enum Compression
        JPEG
        LZW
        PACKBITS
        DEFLATE
        CCITTRLE
        CCITTFAX3
        CCITTFAX4
        NONE
    End Enum

    Enum ResamplingMethod
        Near
        Bilinear
        Cubic
        CubicSpline
        Lanczos
        Average
        Mode
    End Enum

    Enum DEMDerivative
        Slope
        Aspect
    End Enum

    Enum FileName
        cs2cs
        gdal_contour
        gdal_grid
        gdal_rasterize
        gdal_translate
        gdal_addo
        gdal_buildvrt
        gdaldem
        gdalenhance
        gdalinfo
        gdallocationinfo
        gdalmanage
        gdalserver
        gdalsrsinfo
        gdaltindex
        gdaltransform
        gdalwarp
        ogr2ogr
        ogrinfo
        ogrlineref
        ogrtindex
        proj
        testepsg
    End Enum

    Enum RasterFormat
        AAIGrid
        ACE2
        ADRG
        AIG
        AIRSAR
        ARG
        BLX
        BAG
        BMP
        BSB
        BT
        CEOS
        COASP
        COSAR
        CPG
        CTG
        DDS
        DIMAP
        DIPEx
        DODS
        DOQ1
        DOQ2
        DTED
        E00GRID
        ECRGTOC
        ECW
        EHdr
        EIR
        ELAS
        ENVI
        EPSILON
        ERS
        ESAT
        FAST
        FIT
        FITS
        FujiBAS
        GENBIN
        GEORASTER
        GFF
        GIF
        GRIB
        GMT
        GRASS
        GRASSASCIIGrid
        GSAG
        GSBG
        GS7BG
        GSC
        GTA
        GTiff
        GTX
        GXF
        HDF4
        HDF5
        HF2
        HFA
        IDA
        ILWIS
        INGR
        IRIS
        ISIS2
        ISIS3
        JAXAPALSAR
        JDEM
        JPEG
        JPEGLS
        JPEG2000
        JP2ECW
        JP2KAK
        JP2MrSID
        JP2OpenJPEG
        JPIPKAK
        KMLSUPEROVERLAY
        KRO
        L1B
        LAN
        LCP
        Leveller
        LOSLAS
        MBTiles
        MAP
        MEM
        MFF
        MFF2
        MG4Lidar
        MrSID
        MSG
        MSGN
        NDF
        NGSGEOID
        NITF
        netCDF
        NTv2
        NWT_GRC
        NWT_GRD
        OGDI
        OZI
        PAux
        PCIDSK
        PCRaster
        PDF
        PDS
        PNG
        PostGISRaster
        PNM
        R
        RASDAMAN
        Rasterlite
        RIK
        RMF
        RPFTOC
        RS2
        RST
        SAGA
        SAR_CEOS
        SDE
        SDTS
        SGI
        SNODAS
        SRP
        SRTMHGT
        TERRAGEN
        TIL
        TSX
        USGSDEM
        VRT
        WCS
        WEBP
        WMS
        XPM
        XYZ
        ZMap
    End Enum

    Enum VectorFormat
        AeronavFAA
        ArcObjects
        AVCBin
        AVCE00
        ARCGEN
        BNA
        DWG
        DXF
        CartoDB
        CSV
        CouchDB
        VFK
        DODS
        EDIGEO
        ElasticSearch
        FileGDB
        PGeo
        SDE
        ESRI_Shapefile
        FMEObjects_Gateway
        GeoJSON
        Geoconcept
        Geomedia
        GPKG
        GeoRSS
        GFT
        GME
        GML
        GMT
        GPSBabel
        GPX
        GRASS
        GPSTrackMaker
        HTF
        Idrisi
        IDB
        Interlis
        INGRES
        KML
        LIBKML
        MapInfo_File
        DGN
        MDB
        Memory
        MySQL
        NAS
        OCI
        ODBC
        MSSQLSpatial
        ODS
        OGDI
        OpenAir
        OpenFileGDB
        OSM
        PCIDSK
        PDF
        PDS
        PGDump
        PostgreSQL
        REC
        S57
        SDTS
        SEGUKOOA
        SEGY
        Selafin
        SOSI
        SQLite
        SUA
        SVG
        SXF
        UK_NTF
        TIGER
        VRT
        WFS
        XLS
        XLSX
        XPLANE
        Walk
        WAsP
    End Enum

#End Region

End Class