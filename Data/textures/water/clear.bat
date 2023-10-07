@echo off
setlocal enabledelayedexpansion

for /f "tokens=1-3 delims=#" %%a in ('dir /b /a-d *_0*.png') do (
  set orig=%%a#%%b#%%c & set pre=%%a#%%b
  for /f "tokens=1-3 delims=_." %%d in ("%%c") do (
    set "str=%%d" & set "str2=%%e" & set "str2=!str2:_=!" 
    Call :StripLeading0 !str2! ret
    set "newfile=!pre!!ret!#!str!.%%f" 
  )
)
set "filename=!newfile!"
echo %filename%
exit /b

:StripLeading0 <input> <return>
setlocal
FOR /F "tokens=* delims=0*" %%A IN ("%1") DO SET Var=%%A
endlocal & set %2=%Var%