@echo off
pushd .

cd Colonisation.StarSystems
copy /y systemsPopulated.json bin\debug\net8.0 >nul
if errorlevel 1 goto exit
copy /y systemsWithCoordinates.json bin\debug\net8.0 >nul
if errorlevel 1 goto exit
copy /y stations.json bin\debug\net8.0 >nul
if errorlevel 1 goto exit

cd bin\debug\net8.0
echo Running "Colonisation.StarSystems" (may take several minutes)...
Colonisation.StarSystems
if errorlevel 1 goto exit
copy /y colonisationTargets.csv ..\..\..\..\Colonisation.Bodies >nul
if errorlevel 1 goto exit
copy /y colonisationTargets.csv ..\..\..\..\Colonisation.Bodies\bin\debug\net8.0 >nul
if errorlevel 1 goto exit
copy /y colonisationTargets.csv ..\..\..\..\Colonisation.Points >nul
if errorlevel 1 goto exit
copy /y colonisationTargets.csv ..\..\..\..\Colonisation.Points\bin\debug\net8.0 >nul
if errorlevel 1 goto exit

cd ..\..\..\..\Colonisation.Bodies\bin\debug\net8.0
echo Running "Colonisation.Bodies" (may take several minutes depending in EDSM)...
Colonisation.Bodies
if errorlevel 1 goto exit
copy /y systemBodies.json ..\..\..\..\Colonisation.Points >nul
if errorlevel 1 goto exit
copy /y systemBodies.json ..\..\..\..\Colonisation.Points\bin\debug\net8.0 >nul
if errorlevel 1 goto exit

cd ..\..\..\..\Colonisation.Points\bin\debug\net8.0
echo Running "Colonisation.Points"...
Colonisation.Points
if errorlevel 1 goto exit
copy /y prioritisedColonisationTargets.csv ..\..\.. >nul
if errorlevel 1 goto exit

:exit
popd
