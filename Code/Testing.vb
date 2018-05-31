Module Testing

    Public Sub Test(ByVal Path As String)
        'ProjectDirectory = "D:\GridET Project - Utah 2018\Utah - New Area"
        'ClimateModelDirectory = "D:\GridET Project - Utah 2018\Climate Models"

        Dim LapseRates(365) As Single
        For I = 1 To 366
            Dim RecordDate = (New DateTime(2011, 12, 31)).AddDays(I)
            'Lapse Rate (F/ft)
            Dim M1 = RecordDate.Month - 2
            Dim M2 = RecordDate.Month - 1
            If RecordDate.Day >= 15 Then
                M1 = RecordDate.Month - 1
                M2 = RecordDate.Month
            End If
            If M1 = -1 Then M1 = 11
            If M2 = 12 Then M2 = 0
            Dim Period = DateTime.DaysInMonth(RecordDate.Year, M1 + 1)
            Dim Fraction = If(RecordDate.Day >= 15, RecordDate.Day - 15, Period - (15 - RecordDate.Day)) / Period
            LapseRates(I - 1) = MonthlyLapseRate(M1) * (1 - Fraction) + MonthlyLapseRate(M2) * Fraction
        Next

        'Open NLDAS-2A Topographical Rasters and Get Masked Extent
        Dim NLDAS_2AElevationRaster As New Raster(NLDAS_2AElevationRasterPath, GDAL.Access.GA_ReadOnly)
        Dim NLDAS_2ASlopeRaster As New Raster(NLDAS_2ASlopeRasterPath, GDAL.Access.GA_ReadOnly)
        Dim NLDAS_2AAspectRaster As New Raster(NLDAS_2AAspectRasterPath, GDAL.Access.GA_ReadOnly)

        Dim NLDAS_2ANoDataValue As Double
        Using Band = NLDAS_2AElevationRaster.Dataset.GetRasterBand(1)
            Band.GetNoDataValue(NLDAS_2ANoDataValue, False)
        End Using

        Dim LapseRate(11)() As Double
        For I = 0 To LapseRate.Length - 1
            ReDim LapseRate(I)(NLDAS_2AElevationRaster.XCount * NLDAS_2AElevationRaster.YCount - 1)
        Next
        Dim Count(11) As Int32
        Dim Lock As New Object
        Dim N As Int32 = 0

        Dim Connection = CreateConnection(NLDAS_2ARastersPath)
        Connection.Open()
        Dim Command = Connection.CreateCommand

        'Load NLDAS-2A Grib Files for Each Day Period
        Command.CommandText = "SELECT Date, Image, CAST(strftime('%H', Date) AS INTEGER) Hour FROM Rasters WHERE Hour BETWEEN 16 AND 21"
        Using Reader = Command.ExecuteReader
            While Reader.Read
                Dim Time = Reader.GetDateTime(0)

                Dim RasterBytes As New RasterBytes
                RasterBytes.Time = Reader.GetString(0).Replace(" ", "").Replace(":", "").Replace("-", "")
                RasterBytes.Bytes = Reader(1)

                Threading.Interlocked.Increment(N)

                Task.Factory.StartNew(
                Sub()
                    ProcessRaster(RasterBytes, Lock, LapseRate, Count, N, Time)
                End Sub)

                While N > 8
                    Threading.Thread.Sleep(100)
                End While
            End While
        End Using

        Threading.Thread.Sleep(15000)

        For I = 0 To LapseRate.Length - 1
            For J = 0 To LapseRate(I).Length - 1
                If LapseRate(I)(J) <> Single.MinValue Then LapseRate(I)(J) *= 5280 / Count(I)
            Next
        Next

        Using OutputRaster = CreateNewRaster(Path, NLDAS_2AElevationRaster, {Single.MinValue, Single.MinValue, Single.MinValue, Single.MinValue, Single.MinValue, Single.MinValue, Single.MinValue, Single.MinValue, Single.MinValue, Single.MinValue, Single.MinValue, Single.MinValue}, GDAL.DataType.GDT_Float64, , 12)

            For I = 0 To LapseRate.Length - 1
                OutputRaster.Write({I + 1}, LapseRate(I))
            Next

        End Using
    End Sub

    Private Sub ProcessRaster(ByRef Item As RasterBytes, ByRef Lock As Object, ByRef LapseRate()() As Double, ByRef Count() As Int32, ByRef N As Int32, ByRef Time As DateTime)
        Dim M = CInt(Item.Time.Substring(4, 2)) - 1

        Dim InMemoryPath = "/vsimem/Raster" & Item.Time
        GDAL.Gdal.FileFromMemBuffer(InMemoryPath, Item.Bytes)

        Dim Values = ExtractNLDAS_2A(InMemoryPath)

        GDAL.Gdal.Unlink(InMemoryPath)

        Dim LR(Values(0).Length - 1) As Double
        For J = 0 To Values(0).Length - 1
            If Values(0)(J) = -9999 Then
                LR(J) = Single.MinValue
            Else
                Dim TimeVariables(5) As Double
                TimeVariables(0) = 1
                Dim FunctionDoY As Double = (Time.DayOfYear - 1) / 365 * 2 * π
                TimeVariables(1) = Math.Cos(FunctionDoY)
                TimeVariables(2) = Math.Sin(FunctionDoY)
                Dim FunctionHour As Double = (Time.Hour) / 23 * 2 * π
                TimeVariables(3) = Math.Cos(FunctionHour)
                TimeVariables(4) = Math.Sin(FunctionHour)

                TimeVariables(5) = Values(Hourly.Air_Temperature)(J)
                Dim AirTemperature = Values(Hourly.Air_Temperature)(J) - SumProduct(AirTemperatureCorrectionCoefficients, TimeVariables)

                TimeVariables(5) = Values(Hourly.Relative_Humidity)(J)
                Dim RelativeHumidity = Values(Hourly.Relative_Humidity)(J) - SumProduct(HumidityCorrectionCoefficients, TimeVariables)

                LR(J) = AirTemperature
            End If
        Next

        SyncLock Lock
            For J = 0 To Values(0).Length - 1
                If LR(J) = Single.MinValue Then
                    LapseRate(M)(J) = Single.MinValue
                Else
                    LapseRate(M)(J) += LR(J)
                End If
            Next
            Count(M) += 1
        End SyncLock

        Threading.Interlocked.Decrement(N)
    End Sub

    Private Class RasterBytes

        Public Time As String
        Public Bytes As Byte()

    End Class


End Module
