#requires -version 4
<#
.SYNOPSIS
  Compile and create Deploy packages for DnnCmd
#>
 
#---------------------------------------------------------[Initialisations]--------------------------------------------------------
 
$ErrorActionPreference = "Stop"  # Set Error Action to Stop
$Script:ScriptVersion = "1.0"    # Script Version

#region Functions
#-----------------------------------------------------------[Functions]------------------------------------------------------------

Function Init {
	Clear-Host
	Set-Alias "msbuild" "C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe" -Scope Script
}

Function Build-Project($ProjectRelPath) {
	"[nuget restore]"
	nuget.exe @("restore", $ProjectRelPath)
	""
	"[msbuild]"
	$argList = @($ProjectRelPath, 
			"/t:Rebuild", 
			"/p:Configuration=Release", 
			"/m", # multi-proccesor
			"/NoLogo", 
			"/ConsoleLoggerParameters:WarningsOnly;ErrorsOnly"
			"/FileLogger", 
			"/FileLoggerParameters:Verbosity=normal")
	
	"msbuild $argList"
	msbuild $argList
	if ($LASTEXITCODE) { throw "LASTEXITCODE: $LASTEXITCODE" }
}

Function Copy-RequestedFiles($sourceFolder, $targetSubfolder, [string[]]$files) {
	$targetFolder = Join-Path $PSScriptRoot $targetSubfolder
	
	if (-not (Test-Path $targetFolder -PathType Container)) { md $targetFolder | Out-Null }
	$files | % {
		"$targetSubfolder\$_"
		Copy-Item "$sourceFolder\$_" $targetFolder -Force
	}
}

Function Cleanup-TargetFolders {
	# target
	Remove-Item "$PSScriptRoot\dnncmd" -Force -Recurse -ErrorAction SilentlyContinue
	Remove-Item "$PSScriptRoot\DnnExtension" -Force -Recurse -ErrorAction SilentlyContinue
	
	# before compilation
	Remove-Item "$PSScriptRoot\..\..\BuildSrc\BuildToDnn\dev\dnncmd\bin" -Force -Recurse -ErrorAction SilentlyContinue
	Remove-Item "$PSScriptRoot\..\..\BuildSrc\Deployer\obj" -Force -Recurse -ErrorAction SilentlyContinue
	
}

Function BuildAll-Projects {
	Build-Project (Resolve-Path "$PSScriptRoot\..\..\BuildSrc\BuildToDnn\BuildToDnn.sln").Path
	Build-Project (Resolve-Path "$PSScriptRoot\..\..\BuildSrc\Deployer\Deployer.sln").Path
}

Function CopyOutputFrom-Projects {
	Copy-RequestedFiles -sourceFolder "$PSScriptRoot\..\..\BuildSrc\BuildToDnn\dev\dnncmd\bin\Release\Merged" `
						-targetSubfolder "dnncmd" -files "dnncmd.exe"

	$sourceFolder = (Resolve-Path "$PSScriptRoot\..\..\BuildSrc\Deployer\obj").Path
	$lastPackage = Get-ChildItem "$sourceFolder\Deployer_*_Install.zip" | select -Last 1 -ExpandProperty Name

	Copy-RequestedFiles -sourceFolder $sourceFolder -targetSubfolder "DnnExtension" -files $lastPackage
}
#endregion

#-----------------------------------------------------------[Initialize]-----------------------------------------------------------
Push-Location $PSScriptRoot
Init

#-----------------------------------------------------------[Execution]------------------------------------------------------------

Cleanup-TargetFolders
BuildAll-Projects
CopyOutputFrom-Projects

Pop-Location
