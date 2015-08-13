@ECHO OFF
SETLOCAL
REM GOTO EOF

PUSHD "%~dp0"
PUSHD ..
SET DNNCMD="%CD%\dnncmd\dnncmd.exe"
POPD

REM authenticate
REM SET USER=host
REM SET PWD=abc123$
REM SET DNN_AUTH=-r http://dnn721 --user host -p %PWD%

SET USER=host
SET PWD=puertobahia123$#
SET DNN_AUTH=-r http://puertobahia-test.azurewebsites.net --user %USER% -p %PWD%

REM SET USER=pescobar
REM SET PWD=abc1234$
REM SET DNN_AUTH=-r http://puertobahia-test-stage.azurewebsites.net --user pescobar -p abc1234$

PUSHD "\\FCBUILD\Deploy\2015.08\PB\QA\Modules-v1.2\v20150813.2"

REM wait for refresh and get version
%DNNCMD% module %DNN_AUTH% -v

REM install several modules
ECHO Publishing...
%DNNCMD% module %DNN_AUTH% -i -m "AbrirRecalada_1.1.0_Install.zip" "PuertoBahia.Core.Database_1.1.0_Install.zip" "PuertoBahia.Core.Entities_1.1.0_Install.zip" "PuertoBahia.Core.Utils_1.1.0_Install.zip"
ECHO Done

POPD

REM wait for refresh and get version
ECHO Checking site is back up...
%DNNCMD% module %DNN_AUTH% -v
ECHO Done


:EOF
PAUSE