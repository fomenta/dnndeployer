SETLOCAL
GOTO EOF

REM SET URL=%1
REM SET USER=%2
REM SET PWD=%3
REM SET PORTALID=%4

PUSHD "%~dp0"
PUSHD ..
SET DNNCMD="%CD%\dnncmd\dnncmd.exe"
POPD

REM authenticate
SET PWD=abc123$
SET DNN_AUTH=-r 721.dnndev.me --user host --password %PWD%

REM deployer version + DDN version
%DNNCMD% page %DNN_AUTH% --version

REM list portals
%DNNCMD% page %DNN_AUTH% --portals
REM get portal details
%DNNCMD% page %DNN_AUTH% --portal --portalid 0
REM create a page (add permissions to all users to view it)
%DNNCMD% page %DNN_AUTH% --add --name "Added Page" --after Home --permissions "Registered Users"
REM create a page (add permissions to all users to view and EDIT it)
%DNNCMD% page %DNN_AUTH% --add --name "Added Page" --after Home --permissions "Registered Users,VIEW|EDIT"

REM delete a page
%DNNCMD% page %DNN_AUTH% --delete --fullname "Page2"
%DNNCMD% page %DNN_AUTH% --delete --path //AddedPage
REM create a page with a module
%DNNCMD% page %DNN_AUTH% --add --name "Added Page" --after Home --permissions "Registered Users" --modules "forDNN.UsersExportImport"
REM create a page with a module on a specific pane called "leftPane"
%DNNCMD% page %DNN_AUTH% --add --name "Added Page" --after Home --permissions "Registered Users" --modules "forDNN.UsersExportImport,leftPane"
REM get page details
%DNNCMD% page %DNN_AUTH% --get --path //AddedPage

REM en otro portal
%DNNCMD% page %DNN_AUTH% --portalalias julietaq --add --name "Page1" --after Inicio --permissions "Registered Users"
%DNNCMD% page %DNN_AUTH% --portalalias alberta --add --name "Page1" --after Inicio --permissions "Registered Users"
%DNNCMD% page %DNN_AUTH% --portalid 7 --add --name "Page1" --after Inicio --permissions "Registered Users"

:EOF
