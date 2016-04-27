REM @ECHO OFF
SETLOCAL
ECHO Execute one by one. Exiting...
GOTO :EOF

REM SET URL=%1
REM SET USER=%2
SET PWD=abc123$
REM SET PORTALID=%4

PUSHD "%~dp0"
SET BASEFOLDER=%CD%
PUSHD ..
SET DNNCMD="%CD%\dnncmd\dnncmd.exe"
POPD

REM authenticate
SET PWD=abc123$
REM SET DNN_AUTH=-r http://zfranca7.dnndev.me --user host -p %PWD%
SET DNN_AUTH=-r http://721.dnndev.me --user host -p %PWD%

REM deployer version + DDN version
%DNNCMD% installfolder %DNN_AUTH% --version

REM upload all modules in folder to dnn install folder (with verbose)
%DNNCMD% installfolder %DNN_AUTH% --verbose --save --modules "%BASEFOLDER%\SampleModules"
REM upload a module to dnn install folder
%DNNCMD% installfolder %DNN_AUTH% --save --modules "%BASEFOLDER%\SampleModules\Blog_06.00.04_Install.zip"

REM delete a module (containing name) in dnn install folder
%DNNCMD% installfolder %DNN_AUTH% --delete --modules blog

REM get a list of modules in dnn install folder
%DNNCMD% installfolder %DNN_AUTH% --get
REM get a list of modules in dnn install folder and specific subfolder (case sensitive)
%DNNCMD% installfolder %DNN_AUTH% --get --packagetype Module

REM clear all modules in dnn install folder
%DNNCMD% installfolder %DNN_AUTH% --clear

REM upload all modules in folder to dnn install folder (no verbose)
%DNNCMD% installfolder %DNN_AUTH% --save --modules "%BASEFOLDER%\SampleModules"

REM install all modules in dnn install folder
%DNNCMD% installfolder %DNN_AUTH% --installresources

ENDLOCAL