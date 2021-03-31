@echo off
for /f "usebackq delims=" %%i in (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -prerelease -latest -property installationPath`) do (
  if exist "%%i\Common7\Tools\vsdevcmd.bat" (
    %comspec% /k " "%%i\Common7\Tools\vsdevcmd.bat" & cd "%~dp0" & echo on & dotnet build -c "Release Nuget" "Memenim.Core.sln" & exit "
    exit /b
  )
)
exit /b 2