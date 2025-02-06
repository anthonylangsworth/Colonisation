@echo off
pushd .

cd Colonisation.StarSystems\bin\debug\net8.0
echo Running "Colonisation.StarSystems" (make take several minutes)...
Colonisation.StarSystems
if errorlevel 1 goto exit
copy /y colonisationTargets.csv ..\..\..\..\Colonisation.Bodies >nul
copy /y colonisationTargets.csv ..\..\..\..\Colonisation.Bodies\bin\debug\net8.0 >nul
if errorlevel 1 goto exit
copy /y colonisationTargets.csv ..\..\..\..\Colonisation.Points >nul
copy /y colonisationTargets.csv ..\..\..\..\Colonisation.Bodies\bin\debug\net8.0 >nul
if errorlevel 1 goto exit

cd ..\..\..\..\Colonisation.Bodies\bin\debug\net8.0
echo Running "Colonisation.Bodies" (make take several minutes depending in EDSM)...
Colonisation.Bodies
if errorlevel 1 goto exit
copy /y systemBodies.json ..\..\..\..\Colonisation.Points >nul
copy /y systemBodies.json ..\..\..\..\Colonisation.Points\bin\debug\net8.0 >nul
if errorlevel 1 goto exit

cd ..\..\..\..\Colonisation.Points\bin\debug\net8.0
echo Running "Colonisation.points"...
Colonisation.Points
if errorlevel 1 goto exit
copy /y prioritisedColonisationTargets.csv ..\..\..\.. >nul
if errorlevel 1 goto exit

:exit
popd
