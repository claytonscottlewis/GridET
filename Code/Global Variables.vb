Module Global_Variables

#Region "Constants"

    Public Const π As Double = Math.PI
    Public Const LapseRate As Double = -0.003566167 'Fahrenheit/Foot (From -6.5 Celsius/Kilometer)

    Public AirTemperatureCorrectionCoefficients As Double() = {1.58, 0.59, -1.53, -3.73, 1.4, 0.0551}
    Public HumidityCorrectionCoefficients As Double() = {-21.9, 0.78, 3.55, 11.6, -5.05, 0.274}

    Public DAYMETStartDate As DateTime = New DateTime(1980, 1, 1).AddHours(13)
    Public NLDAS_2AStartDate As DateTime = New DateTime(1979, 1, 1).AddHours(13)
    Public PRISMStartDate As DateTime = New DateTime(19, 1, 1)

    Public MonthAndAnnualNames() As String = {"Month1", "Month2", "Month3", "Month4", "Month5", "Month6", "Month7", "Month8", "Month9", "Month10", "Month11", "Month12", "Annual"}

#End Region

#Region "Project Paths"

    'Level 1
    Public Property ProjectDirectory As String = ""

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
    Public Property ClimateModelDirectory As String = ""

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
