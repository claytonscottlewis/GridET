'            Copyright Clayton S. Lewis 2014-2018.
'   Distributed under the Boost Software License, Version 1.0.
'      (See accompanying file GridET License.rtf or copy at
'            http://www.boost.org/LICENSE_1_0.txt)

Module Global_Variables

#Region "Constants"

    Public Const π As Double = Math.PI

    'Derived from Strong, C., Khatri, K. B., Kochanski, A. K., Lewis, C. S., & Allen, L. N. (2017). Reference evapotranspiration from coarse-scale and dynamically downscaled data in complex terrain: Sensitivity to interpolation and resolution. Journal of Hydrology, 548, 406-418.
    Public MonthlyLapseRate() As Double = {-0.001742, -0.002699, -0.003064, -0.003432, -0.003262, -0.00319, -0.003046, -0.002941, -0.002659, -0.002622, -0.002247, -0.002132} 'Fahrenheit/Foot

    Public AirTemperatureCorrectionCoefficients As Double() = {1.58, 0.59, -1.53, -3.73, 1.4, 0.0551}
    Public HumidityCorrectionCoefficients As Double() = {-21.9, 0.78, 3.55, 11.6, -5.05, 0.274}

    Public DAYMETStartDate As DateTime = New DateTime(1980, 1, 1, 13, 0, 0, 0, DateTimeKind.Utc)
    Public NLDAS_2AStartDate As DateTime = New DateTime(1979, 1, 1, 13, 0, 0, 0, DateTimeKind.Utc)
    Public PRISMStartDate As DateTime = New DateTime(19, 1, 1)

    Public Const SingleMinValue = "-0.0000000000000000000000000000000000000340282347"

    Public MonthAndAnnualNames() As String = {"Month1", "Month2", "Month3", "Month4", "Month5", "Month6", "Month7", "Month8", "Month9", "Month10", "Month11", "Month12", "Annual"}

    Public NLDAS_2APaths() As String = {NLDAS_2AMeanAirTemperaturePath, NLDAS_2AMaximumAirTemperaturePath, NLDAS_2AMinimumAirTemperaturePath, NLDAS_2ADewpointTemperaturePath, NLDAS_2ASolarRadiationPath, NLDAS_2AWindSpeedPath, NLDAS_2APrecipitationPath, NLDAS_2AEvapotranspirationASCEPath, NLDAS_2AEvapotranspirationHargreavesPath, NLDAS_2AEvapotranspirationAerodynamicPath, NLDAS_2AGrowingDegreeDays32Path, NLDAS_2AGrowingDegreeDays41Path, NLDAS_2AGrowingDegreeDays8650Path}
    Public NLDAS_2AStatistics() As RasterType = {RasterType.Average, RasterType.Average, RasterType.Average, RasterType.Average, RasterType.Average, RasterType.Average, RasterType.Sum, RasterType.Sum, RasterType.Sum, RasterType.Sum, RasterType.Sum, RasterType.Sum, RasterType.Sum}

    Public Property ClimateModelDirectory As String = ""
    Public Property PixelCount As Int64 = -1
    Public Property ProjectMask As Byte()
    Public Property ProjectProjection As String
    Public Property ProjectGeoTransform As Double()
    Public Property ProjectExtent As Extent
    Public Property ProjectXCount As Integer
    Public Property ProjectYCount As Integer

#End Region

#Region "Project Paths"

    'Level 1
    Public ReadOnly Property ProjectDirectory As String
        Get
            Return My.Settings.LastProjectDirectory
        End Get
    End Property

    Public ReadOnly Property ProjectDetailsPath As String
        Get
            Return IO.Path.Combine(ProjectDirectory, "Project Details.db")
        End Get
    End Property

    Public ReadOnly Property MapServerMapFilePath As String
        Get
            Return IO.Path.Combine(ProjectDirectory, "MapServer Map File.map")
        End Get
    End Property

    Public ReadOnly Property MapServerRasterPath As String
        Get
            Return IO.Path.Combine(ProjectDirectory, "MapServer Raster.tif")
        End Get
    End Property

    Public ReadOnly Property MapServerTemporaryRasterPath As String
        Get
            Return IO.Path.Combine(ProjectDirectory, "MapServer Temporary Raster Path.tif")
        End Get
    End Property

    'Level 2
    Public ReadOnly Property AreaFeaturesDirectory As String
        Get
            Return IO.Path.Combine(ProjectDirectory, "Area Features")
        End Get
    End Property

    'Level 3
    Public ReadOnly Property AttributesDirectory As String
        Get
            Return IO.Path.Combine(AreaFeaturesDirectory, "Attributes")
        End Get
    End Property

    Public ReadOnly Property MaskAttributesPath As String
        Get
            Return IO.Path.Combine(AttributesDirectory, "Attributes.db")
        End Get
    End Property

    'Level 3
    Public ReadOnly Property ElevationDirectory As String
        Get
            Return IO.Path.Combine(AreaFeaturesDirectory, "Elevation")
        End Get
    End Property

    Public ReadOnly Property MaskElevationRasterPath As String
        Get
            Return IO.Path.Combine(ElevationDirectory, "Elevation.tif")
        End Get
    End Property

    Public ReadOnly Property MaskAspectRasterPath As String
        Get
            Return IO.Path.Combine(ElevationDirectory, "Aspect.tif")
        End Get
    End Property

    Public ReadOnly Property MaskSlopeRasterPath As String
        Get
            Return IO.Path.Combine(ElevationDirectory, "Slope.tif")
        End Get
    End Property

    Public ReadOnly Property MaskSlopeAreaRasterPath As String
        Get
            Return IO.Path.Combine(ElevationDirectory, "Slope Area.tif")
        End Get
    End Property

    'Level 3
    Public ReadOnly Property CoordinateDirectory As String
        Get
            Return IO.Path.Combine(AreaFeaturesDirectory, "Coordinate")
        End Get
    End Property

    Public ReadOnly Property MaskLatitudeRasterPath As String
        Get
            Return IO.Path.Combine(CoordinateDirectory, "Latitude.tif")
        End Get
    End Property

    Public ReadOnly Property MaskLongitudeRasterPath As String
        Get
            Return IO.Path.Combine(CoordinateDirectory, "Longitude.tif")
        End Get
    End Property

    'Level 3
    Public ReadOnly Property MaskDirectory As String
        Get
            Return IO.Path.Combine(AreaFeaturesDirectory, "Mask")
        End Get
    End Property

    Public ReadOnly Property MaskRasterPath As String
        Get
            Return IO.Path.Combine(MaskDirectory, "Mask.tif")
        End Get
    End Property

    Public ReadOnly Property MaskPointsPath As String
        Get
            Return IO.Path.Combine(MaskDirectory, "Mask Pixel Centers.shp")
        End Get
    End Property

    Public ReadOnly Property MaskProjectionPath As String
        Get
            Return IO.Path.Combine(MaskDirectory, "Mask.prj")
        End Get
    End Property

    'Level 2
    Public ReadOnly Property IntermediateCalculationsDirectory As String
        Get
            Return IO.Path.Combine(ProjectDirectory, "Intermediate Calculations")
        End Get
    End Property

    'Level 3
    Public ReadOnly Property InputVariablesDirectory As String
        Get
            Return IO.Path.Combine(IntermediateCalculationsDirectory, "Input Variables")
        End Get
    End Property

    Public ReadOnly Property NLDAS_2AMeanAirTemperaturePath As String
        Get
            Return IO.Path.Combine(InputVariablesDirectory, "Mean Air Temperature NLDAS_2A.db")
        End Get
    End Property

    Public ReadOnly Property NLDAS_2AMaximumAirTemperaturePath As String
        Get
            Return IO.Path.Combine(InputVariablesDirectory, "Maximum Air Temperature NLDAS_2A.db")
        End Get
    End Property

    Public ReadOnly Property NLDAS_2AMinimumAirTemperaturePath As String
        Get
            Return IO.Path.Combine(InputVariablesDirectory, "Minimum Air Temperature NLDAS_2A.db")
        End Get
    End Property

    Public ReadOnly Property NLDAS_2ASolarRadiationPath As String
        Get
            Return IO.Path.Combine(InputVariablesDirectory, "Solar Radiation NLDAS_2A.db")
        End Get
    End Property

    Public ReadOnly Property NLDAS_2AWindSpeedPath As String
        Get
            Return IO.Path.Combine(InputVariablesDirectory, "Wind Speed NLDAS_2A.db")
        End Get
    End Property

    Public ReadOnly Property NLDAS_2ADewpointTemperaturePath As String
        Get
            Return IO.Path.Combine(InputVariablesDirectory, "Dewpoint Temperature NLDAS_2A.db")
        End Get
    End Property

    Public ReadOnly Property NLDAS_2APrecipitationPath As String
        Get
            Return IO.Path.Combine(InputVariablesDirectory, "Precipitation NLDAS_2A.db")
        End Get
    End Property

    Public ReadOnly Property DAYMETPrecipitationPath As String
        Get
            Return IO.Path.Combine(InputVariablesDirectory, "Precipitation DAYMET.db")
        End Get
    End Property

    'Level 3
    Public ReadOnly Property ReferenceEvapotranspirationDirectory As String
        Get
            Return IO.Path.Combine(IntermediateCalculationsDirectory, "Reference Evapotranspiration")
        End Get
    End Property

    Public ReadOnly Property NLDAS_2AEvapotranspirationASCEPath As String
        Get
            Return IO.Path.Combine(ReferenceEvapotranspirationDirectory, "Reference Evapotranspiration ASCE NLDAS_2A.db")
        End Get
    End Property

    Public ReadOnly Property NLDAS_2AEvapotranspirationHargreavesPath As String
        Get
            Return IO.Path.Combine(ReferenceEvapotranspirationDirectory, "Reference Evapotranspiration Hargreaves NLDAS_2A.db")
        End Get
    End Property

    Public ReadOnly Property NLDAS_2AEvapotranspirationAerodynamicPath As String
        Get
            Return IO.Path.Combine(ReferenceEvapotranspirationDirectory, "Reference Evapotranspiration Aerodynamic NLDAS_2A.db")
        End Get
    End Property

    'Level 3
    Public ReadOnly Property DateVariablesDirectory As String
        Get
            Return IO.Path.Combine(IntermediateCalculationsDirectory, "Date Variables")
        End Get
    End Property

    Public ReadOnly Property NLDAS_2AGrowingDegreeDays32Path As String
        Get
            Return IO.Path.Combine(DateVariablesDirectory, "Growing Degree Days 32F NLDAS_2A.db")
        End Get
    End Property

    Public ReadOnly Property NLDAS_2AGrowingDegreeDays41Path As String
        Get
            Return IO.Path.Combine(DateVariablesDirectory, "Growing Degree Days 41F NLDAS_2A.db")
        End Get
    End Property

    Public ReadOnly Property NLDAS_2AGrowingDegreeDays8650Path As String
        Get
            Return IO.Path.Combine(DateVariablesDirectory, "Growing Degree Days 86-50F NLDAS_2A.db")
        End Get
    End Property

    Public ReadOnly Property NLDAS_2ALastSpringFrostPath As String
        Get
            Return IO.Path.Combine(DateVariablesDirectory, "Last Spring Frost.db")
        End Get
    End Property

    'Level 3
    Public ReadOnly Property PotentialEvapotranspirationDirectory As String
        Get
            Return IO.Path.Combine(IntermediateCalculationsDirectory, "Potential Evapotranspiration")
        End Get
    End Property

    Public ReadOnly Property PotentialEvapotranspirationPath As String
        Get
            Return IO.Path.Combine(PotentialEvapotranspirationDirectory, "{0} Potential Evapotranspiration.db")
        End Get
    End Property

    'Level 2
    Public ReadOnly Property OutputCalculationsDirectory As String
        Get
            Return IO.Path.Combine(ProjectDirectory, "Output Calculations")
        End Get
    End Property

    'Level 1
    Public ReadOnly Property NLDAS_2ARastersPath As String
        Get
            Return IO.Path.Combine(ClimateModelDirectory, "NLDAS_2A.db")
        End Get
    End Property

    Public ReadOnly Property NLDAS_2AElevationRasterPath As String
        Get
            Return IO.Path.Combine(ClimateModelDirectory, "NLDAS_2A Elevation.tif")
        End Get
    End Property

    Public ReadOnly Property NLDAS_2ASlopeRasterPath As String
        Get
            Return IO.Path.Combine(ClimateModelDirectory, "NLDAS_2A Slope.tif")
        End Get
    End Property

    Public ReadOnly Property NLDAS_2AAspectRasterPath As String
        Get
            Return IO.Path.Combine(ClimateModelDirectory, "NLDAS_2A Aspect.tif")
        End Get
    End Property

    Public ReadOnly Property DAYMETRastersPath As String
        Get
            Return IO.Path.Combine(ClimateModelDirectory, "DAYMET.db")
        End Get
    End Property

    Public ReadOnly Property PRISMRastersPath As String
        Get
            Return IO.Path.Combine(ClimateModelDirectory, "PRISM.db")
        End Get
    End Property

#End Region

End Module
