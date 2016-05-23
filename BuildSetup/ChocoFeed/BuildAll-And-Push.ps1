#requires -Version 4
#requires -RunAsAdministrator
<#
.SYNOPSIS
  Build All Projects and Push Nuget to Chocolatey
.EXAMPLE
 Enter example here
#>
#---------------------------------------------------------[Initialisations]--------------------------------------------------------
 
$ErrorActionPreference = "Stop"  # Set Error Action to Stop
$Script:ScriptVersion = "1.0"    # Script Version

#-----------------------------------------------------------[Functions]------------------------------------------------------------

Function Init {
	Clear-Host
}

#-----------------------------------------------------------[Initialize]-----------------------------------------------------------
Init

#-----------------------------------------------------------[Execution]------------------------------------------------------------

"[CompileAndDeployVersion]"
& "$PSScriptRoot\..\DnnDeployer\CompileAndDeployVersion.ps1"
""
"[PackagingAndPublishingToFeed]"
& "$PSScriptRoot\PackagingAndPublishingToFeed.ps1"
