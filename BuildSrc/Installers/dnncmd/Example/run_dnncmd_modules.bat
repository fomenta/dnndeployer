SETLOCAL
GOTO EOF

PUSHD "%~dp0"
PUSHD ..
SET DNNCMD="%CD%\v1.7\dnncmd.exe"
POPD

REM authenticate
SET PWD=abc123$
SET DNN_AUTH=-r http://dnn721 --user host -p %PWD%

REM install
%DNNCMD% module %DNN_AUTH% -i -m Blog_06.00.05_Install.zip
REM upgrade
%DNNCMD% module %DNN_AUTH% -i -m Blog_06.00.06_Install.zip
REM downgrade (force)
%DNNCMD% module %DNN_AUTH% -f -i -m Blog_06.00.04_Install.zip
REM downgrade (error)
%DNNCMD% module %DNN_AUTH% -i -m Blog_06.00.04_Install.zip
REM uninstall
%DNNCMD% module %DNN_AUTH% -u -m Blog
%DNNCMD% module %DNN_AUTH% -u -m DotNetNuke.Blog
REM list
%DNNCMD% module %DNN_AUTH% --list
%DNNCMD% module %DNN_AUTH% --list --pattern blog

REM multi-install (one by one)
%DNNCMD% module %DNN_AUTH% -i -m Blog_06.00.05_Install.zip
%DNNCMD% module %DNN_AUTH% -i -m DNNSimpleArticle_00.02.01_Install.zip
%DNNCMD% module %DNN_AUTH% -i -m UsersExportImport_v.01.01.01.zip


:EOF
