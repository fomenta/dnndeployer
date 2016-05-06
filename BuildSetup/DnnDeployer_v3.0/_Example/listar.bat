@ECHO OFF
REM FOR /F "eol=; tokens=2,3* delims=, " %i in (portales.txt) do @echo %i %j %k
FOR /F "eol=; skip=1 tokens=1-4 delims=, " %%i in (portales.txt) do (
	ECHO URL: %%i
	ECHO PortalID: %%l
	ECHO User: %%j
	ECHO Password: %%k
	ECHO ------------------------
)
