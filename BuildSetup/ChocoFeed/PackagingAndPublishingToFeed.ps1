#requires -Version 4
#requires -RunAsAdministrator
<#
.SYNOPSIS
  Build All Projects and Push Nuget to Chocolatey
.EXAMPLE
 test installation from local folder

cinst dnncmd -fdv -s $pwd

# uninstall after testing
choco uninstall dnncmd

# set apikey. take it from https://chocolatey.org/account
choco apikey -k ... -s https://chocolatey.org/
#>
#Push-Location C:\code\mine\chocolateyautomaticpackages\dnncmd
#---------------------------------------------------------[Initialisations]--------------------------------------------------------
 
$ErrorActionPreference = "Stop"  # Set Error Action to Stop
$Script:ScriptVersion = "1.0"    # Script Version

#-----------------------------------------------------------[Functions]------------------------------------------------------------

#region Functions
Function CleanupInput-DnnCmdPackage {
	Remove-Item "$PSScriptRoot\dnncmd.exe" -Force -ErrorAction SilentlyContinue
	Remove-Item "$PSScriptRoot\DnnExtension" -Force -Recurse -ErrorAction SilentlyContinue
}

Function GetLatestBuild-DnnCmdPackage {
	CleanupInput-DnnCmdPackage

	# get last folder ordered alphabetically (assuming it has the latest version)
	$source = Get-ChildItem "$PSScriptRoot\..\DnnDeployer*" -Directory | Select -ExpandProperty FullName -Last 1
	"Source folder: $source"

	# copy updated files
	Copy-Item "$source\dnncmd\*" . -Force
	if (!(Test-Path "$PSScriptRoot\DnnExtension" -PathType Container)) { mkdir "$PSScriptRoot\DnnExtension" -Force | Out-Null }
	Copy-Item "$source\DnnExtension\*Install.zip" "DnnExtension" -Force
}

Function UpdateNugetVersion-DnnCmdPackage {
	# get dnncmd file version
	$fileVersion = (Get-Item "$PSScriptRoot\dnncmd.exe").VersionInfo.FileVersion
	"File Version: $fileVersion"
	#$version = [Reflection.AssemblyName]::GetAssemblyName("$source\dnncmd\dnncmd.exe").Version.ToString()

	# update nuspec and copy to target folder on-the-fly
	(Get-Content "$PSScriptRoot\dnncmd.nuspec" -Encoding UTF8) `
			-replace "<version>(.+)</version>", "<version>$fileVersion</version>" |
		Set-Content "dnncmd.nuspec" -Encoding UTF8 -Force

	ShowPublish-DnnCmdPackage $fileVersion
}

Function ShowPublish-DnnCmdPackage([Parameter(Mandatory=$true)][string]$fileVersion) {
	$package = "dnncmd.$fileVersion.nupkg"

	"choco push $package -s https://chocolatey.org"
	choco push $package -s https://chocolatey.org

	"cup dnncmd -version $fileVersion -y"
	cup dnncmd -version $fileVersion -y
<#
Write-Host `
@"
	#set apikey. take it from https://chocolatey.org/account
	choco apikey -k ... -s https://chocolatey.org

	#Push to chocolatey feed:
	choco push $package -s https://chocolatey.org

	#Feed:
	https://chocolatey.org/packages/dnncmd/$fileVersion

	#Install:
	cinst dnncmd -version $fileVersion -y

	#Upgrade:
	cup dnncmd -version $fileVersion -y
"@
#>
}

#endregion

#-----------------------------------------------------------[Initialize]-----------------------------------------------------------
Clear-Host
Push-Location $PSScriptRoot

#-----------------------------------------------------------[Execution]------------------------------------------------------------

Remove-Item "$PSScriptRoot\*.nupkg" -Force -ErrorAction SilentlyContinue

GetLatestBuild-DnnCmdPackage

UpdateNugetVersion-DnnCmdPackage

"choco pack"
cpack

CleanupInput-DnnCmdPackage

Pop-Location