SETLOCAL
GOTO EOF

PUSHD "%~dp0"
PUSHD ..
SET DNNCMD="%CD%\dnncmd\dnncmd.exe"
POPD

REM authenticate
REM SET PWD=abc123$
REM SET DNN_AUTH=-r http://dnn721 --user host -p %PWD%
SET DNN_AUTH=-r http://puertobahia-test-staging.azurewebsites.net --user pescobar -p abc1234$

REM install several
%DNNCMD% module %DNN_AUTH% -i -m SampleModules\Blog_06.00.04_Install.zip SampleModules\UsersExportImport_v.01.01.01.zip
%DNNCMD% module %DNN_AUTH% -i -f -m SampleModules\Blog_06.00.05_Install.zip SampleModules\UsersExportImport_v.01.01.01.zip

REM uninstall all
%DNNCMD% module %DNN_AUTH% -u -m DotNetNuke.Blog forDNN.UsersExportImport
REM wait for refresh and get version
%DNNCMD% module %DNN_AUTH% -v


REM install
%DNNCMD% module %DNN_AUTH% -i -m SampleModules\Blog_06.00.05_Install.zip
REM upgrade
%DNNCMD% module %DNN_AUTH% -i -m SampleModules\Blog_06.00.06_Install.zip
REM downgrade (force)
%DNNCMD% module %DNN_AUTH% -f -i -m SampleModules\Blog_06.00.04_Install.zip
REM downgrade (error)
%DNNCMD% module %DNN_AUTH% -i -m SampleModules\Blog_06.00.04_Install.zip
REM uninstall
%DNNCMD% module %DNN_AUTH% -u -m DotNetNuke.Blog
REM list
%DNNCMD% module %DNN_AUTH% --list
%DNNCMD% module %DNN_AUTH% --list --pattern blog

REM multi-install (one by one)
%DNNCMD% module %DNN_AUTH% -i -m SampleModules\Blog_06.00.05_Install.zip
%DNNCMD% module %DNN_AUTH% -i -m SampleModules\DNNSimpleArticle_00.02.01_Install.zip
%DNNCMD% module %DNN_AUTH% -i -m SampleModules\UsersExportImport_v.01.01.01.zip


:EOF
