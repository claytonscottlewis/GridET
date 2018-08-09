; Script generated by the Inno Script Studio Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "GridET"
#define MyAppVersion "1.1"
#define MyAppExeName "GridET.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{AA812993-0804-41C5-B215-70B62B954BAE}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
LicenseFile="Help File\GridET License.rtf"
OutputDir="Setup"
OutputBaseFilename="GridET Setup"
SetupIconFile=Images\GridET.ico
Compression=lzma
SolidCompression=yes
ArchitecturesInstallIn64BitMode=x64

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "bin\x64\Release\GridET.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\x64\Release\GridET.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\x64\Release\HtmlRenderer.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\x64\Release\HtmlRenderer.WinForms.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\x64\Release\GDAL\*"; DestDir: "{app}\GDAL\"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "bin\x64\Release\SQLite\*"; DestDir: "{app}\SQLite\"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "bin\x64\Release\Help File\*"; DestDir: "{app}\Help File\"; Flags: ignoreversion recursesubdirs createallsubdirs
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent
