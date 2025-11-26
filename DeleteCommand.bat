@ECHO OFF

ECHO %CD%

FOR /d /r . %%d IN (bin,obj) DO (

@IF EXIST "%%d" ( rd /s /q "%%d"

ECHO %%d Deleted
)

)

ECHO All bin and obj directories have been successfully deleted.

PAUSE 