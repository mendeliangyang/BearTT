@echo off
set i=0

:s
if %i% LSS 2 (
	set /a i+=1 
	echo %i%
	start "BearTT" "BearTT.exe" 
) else (
 	goto :end 
 )

goto :s
:end
pause