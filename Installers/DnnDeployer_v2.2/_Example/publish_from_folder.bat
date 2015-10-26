@ECHO OFF
SETLOCAL
REM GOTO EOF

PUSHD "%~dp0"
PUSHD ..
SET DNNCMD="%CD%\dnncmd\dnncmd.exe"
POPD

REM authenticate
SET USER=host
SET PASSWORD=abc123$
SET DNN_AUTH=-r http://dnn721 --user %USER% -p %PASSWORD%

SET SOURCE_FOLDER="\\FCBUILD\Deploy\2015.08\PB\QA\Modules-v1.2\v20150813.2"

REM wait for refresh and get version
%DNNCMD% module %DNN_AUTH% -v

REM install several modules
ECHO Publishing...
%DNNCMD% module %DNN_AUTH% -i -m %SOURCE_FOLDER%
ECHO Done

POPD

REM wait for refresh and get version
ECHO Checking site is back up...
%DNNCMD% module %DNN_AUTH% -v
ECHO Done


:EOF
PAUSE