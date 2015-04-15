SETLOCAL
GOTO EOF

REM SET URL=%1
REM SET USER=%2
REM SET PWD=%3
REM SET PORTALID=%4

PUSHD "%~dp0"
PUSHD ..
SET DNNCMD="%CD%\v1.7\dnncmd.exe"
POPD

REM authenticate
SET PWD=abc123$
REM SET DNN_AUTH=-r http://zfranca7.dnndev.me --user host -p %PWD%
SET DNN_AUTH=-r http://dnn721 --user host -p %PWD%

REM deployer version + DDN version
%DNNCMD% page %DNN_AUTH% --version

REM list modules on page
%DNNCMD% pagemodule %DNN_AUTH% --path //AddedPage --get
REM remove all modules from page
%DNNCMD% pagemodule %DNN_AUTH% --path //AddedPage --clear

REM By default, before adding a new mode, all previously added modules are removed 
REM     (unless you use --keepexisting)
REM add module to page (on default pane, called ContentPane)
%DNNCMD% pagemodule %DNN_AUTH% --path //AddedPage --add --module ZFranca.Consultas.Inventarios
%DNNCMD% pagemodule %DNN_AUTH% --path //Page2 --add --module "DotNetNuke.Blog"
%DNNCMD% pagemodule %DNN_AUTH% --path //AddedPage --add --module ZFranca.Consultas.Inventarios --keepexisting
REM add module to page on a specific pane called leftPane
%DNNCMD% pagemodule %DNN_AUTH% --path //AddedPage --add --module forDNN.UsersExportImport --pane leftPanel
REM add an extra module to page but keep existing modules previously added to page
%DNNCMD% pagemodule %DNN_AUTH% --path //AddedPage --add --module forDNN.UsersExportImport --keepexisting

REM en otro portal
%DNNCMD% pagemodule %DNN_AUTH% --portalid 7 --path //Page1 --add --module ZFranca.Consultas.Inventarios
%DNNCMD% pagemodule %DNN_AUTH% --portalid 7 --path //Page1 --add --module ZFranca.VisorDocumento

REM con titulo
%DNNCMD% pagemodule %DNN_AUTH% --path //TestingUserControls --add --module forDNN.UsersExportImport --title "User Export/Import 2"

%DNNCMD% pagemodule %DNN_AUTH% --path //TestingUserControls --add --module forDNN.UsersExportImport --title "User Export/Import 2" --settingname DefaultUserControl --settingvalue ControlKeyA

:EOF
