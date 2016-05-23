@ECHO OFF
SETLOCAL
REM GOTO EOF

PUSHD "%~dp0"
PUSHD ..
SET DNNCMD="%CD%\dnncmd\dnncmd.exe"
POPD

REM authenticate
SET PWD=abc123$
SET DNN_AUTH=-r 721.dnndev.me --user host --password %PWD%

SET SOURCE_FOLDER="\\BUILDSERVER\Deploy\v20150813.2"

REM wait for refresh and get version
%DNNCMD% module %DNN_AUTH% -v

REM install several modules
ECHO Publishing...
%DNNCMD% module %DNN_AUTH% -i -m %SOURCE_FOLDER%
ECHO Done

POPD

REM wait for refresh and get version
ECHO Checking site is back up...
%DNNCMD% module %DNN_AUTH% --version
ECHO Done


:EOF
PAUSE