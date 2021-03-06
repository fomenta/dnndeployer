Clear-Host
# add sym-link to module
$symName, $targetDir = "C:\inetpub\dnn721\DesktopModules\Deployer", "$PSScriptRoot\Deployer"
If (-not (Test-Path $symName)) {
	New-Item -ItemType SymbolicLink -Path $symName -Value $targetDir
}

#add vdir to module
C:\Windows\System32\inetsrv\appcmd.exe add vdir /app.name:"dnn721/" /path:/DesktopModules/Deployer /physicalPath:"$deployerDir"

# add reverse link to bin
$symName, $targetDir = "C:\Users\PEscobar\Documents\GitHub\dnndeployer\bin", "C:\inetpub\dnn721\bin"
if (Test-Path $symName) { Remove-Item $symName -Recurse -Force }
New-Item -ItemType SymbolicLink -Path $symName -Value $targetDir
