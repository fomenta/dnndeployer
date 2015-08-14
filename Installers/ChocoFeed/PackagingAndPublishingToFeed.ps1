#requires -Version 4.0

$ErrorActionPreference = "Stop"
Clear-Host
Push-Location C:\code\mine\chocolateyautomaticpackages\dnncmd

# -------------------------------- MAIN ----------------------------------------

# get last folder ordered alphabetically (assuming it has the latest version)
$source = Get-ChildItem "$PSScriptRoot\..\DnnDeployer_v*" -Directory | Select -ExpandProperty FullName -Last 1
"Source folder: $source"


# copy updated files
Copy-Item "$source\dnncmd\*" . -Force
if (!(Test-Path "$source\DnnExtension" -PathType Container)) { mkdir "$source\DnnExtension" -Force }
Copy-Item "$source\DnnExtension\*Install.zip" "DnnExtension" -Force


# get dnncmd file version
$fileVersion = (Get-Item "$source\dnncmd\dnncmd.exe").VersionInfo.FileVersion
"File Version: $fileVersion"
#$version = [Reflection.AssemblyName]::GetAssemblyName("$source\dnncmd\dnncmd.exe").Version.ToString()


# update nuspec and copy to target folder on-the-fly
(Get-Content "$PSScriptRoot\dnncmd.nuspec" -Encoding UTF8) `
	-replace "<version>(.+)</version>", "<version>$fileVersion</version>" |
	Set-Content "dnncmd.nuspec" -Encoding UTF8 -Force


# test installation from local folder
#cinst dnncmd -fdv -s $pwd

# uninstall after testing
#choco uninstall dnncmd


# set apikey. take it from https://chocolatey.org/account
#choco apikey -k ... -s https://chocolatey.org/

# pack nuget
cpack

$package = "dnncmd.$fileVersion.nupkg"

Write-Host `
"Push to chocolatey feed:
choco push $package -s https://chocolatey.org/

Feed:
https://chocolatey.org/packages/dnncmd/$fileVersion

Install:
cinst dnncmd -version $fileVersion

Upgrade:
cup dnncmd -version $fileVersion";
