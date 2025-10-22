SETLOCAL ENABLEDELAYEDEXPANSION
set /p version=<VERSION.txt
mkdir tmp
cd tmp
mkdir MappingHelper
copy ..\Info.json MappingHelper
copy ..\MappingHelper\bin\Release\MappingHelper.dll MappingHelper

cd MappingHelper
for /f "delims=" %%a in (Info.json) do (
    SET s=%%a
    SET s=!s:$VERSION=%version%!
    echo !s!
)>>"InfoChanged.json"
del Info.json
ren InfoChanged.json Info.json
cd ..

tar -a -c -f MappingHelper-%version%.zip MappingHelper
move MappingHelper-%version%.zip ..
cd ..
rd /s /q tmp
pause