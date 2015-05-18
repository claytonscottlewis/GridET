using System;
using System.Windows.Forms;

namespace  {
public static class GridET {

	public static readonly String CHMFile = "GridET.chm";
	public static readonly String Climate_Model_Directory = "Climate-Model-Directory.html";
	public static readonly String Climate_Model_Download = "Climate-Model-Download.html";
	public static readonly String Cover_Properties = "Cover-Properties.html";
	public static readonly String Curve_Properties = "Curve-Properties.html";
	public static readonly String DAYMET = "DAYMET.html";
	public static readonly String Directory_Structure = "Directory-Structure.html";
	public static readonly String Evapotranspiration_Calculation = "Evapotranspiration-Calculation.html";
	public static readonly String Extract_by_Polygon = "Extract-by-Polygon.html";
	public static readonly String GDAL = "GDAL.html";
	public static readonly String Geostatistical_Calculation = "Geostatistical-Calculation.html";
	public static readonly String GridET_Project = "GridET-Project.html";
	public static readonly String Help_File = "Help-File.html";
	public static readonly String Import = "Import.html";
	public static readonly String Installation = "Installation.html";
	public static readonly String Introduction = "Introduction.html";
	public static readonly String License = "License.html";
	public static readonly String Map_Viewer = "Map-Viewer.html";
	public static readonly String MapServer = "MapServer.html";
	public static readonly String Net_Potential_Evapotranspiration = "Net-Potential-Evapotranspiration.html";
	public static readonly String New_Project = "New-Project.html";
	public static readonly String NLDAS = "NLDAS.html";
	public static readonly String Open_Project = "Open-Project.html";
	public static readonly String Others = "Others.html";
	public static readonly String Potential_Evapotranspiration = "Potential-Evapotranspiration.html";
	public static readonly String Process_Scheduler = "Process-Scheduler.html";
	public static readonly String Raster_Period_Average = "Raster-Period-Average.html";
	public static readonly String Raster_Selection = "Raster-Selection.html";
	public static readonly String Reference_Evapotranspiration = "Reference-Evapotranspiration.html";
	public static readonly String References = "References.html";
	public static readonly String Settings = "Settings.html";
	public static readonly String Source_Code_Commentary = "Source-Code-Commentary.html";
	public static readonly String SQLite_Database = "SQLite-Database.html";
	public static readonly String Units = "Units.html";
	public static readonly String Zoom_and_Pan = "Zoom-and-Pan.html";

	public static void ShowHelp(Form f, String constant) {
		Help.ShowHelp(f, CHMFile, HelpNavigator.Topic, constant);
	}
}
}
