@echo off
echo You are about to run the initialization script for Unbound Tech code-signing Demo.
echo The following steps will be executed: & echo.
echo 1. Delete previoulsy created files and keys in UKC
echo 2. Import a pre-created key and certificates to UKC
echo 3. Sync the certificate with Windows Certificate Manager for the current user.
echo After these steps are complete, you can run the different demo scripts in "pipelines" directory.

set /p answer="Would you like to continue (Y/N)\? "

if %answer%==No (exit /b 0)
if %answer%==N (exit /b 0)
if %answer%==no (exit /b 0)
if %answer%==n (exit /b 0)

rem delete any existing objects from previous run
echo delete any existing objects from previous run
ucl delete -p test -n codesign -y >NUL 2>NUL
ucl delete -p test -n codesignCA -y >NUL 2>NUL
del "..\signed_files\*.*" /F /Q >NUL 2>NUL

rem import key and certificates to UKC
echo import key and certificate to UKC
ucl import -i "..\init_files\signkey.pem" -n codesign -p test
ucl import -i "..\init_files\signcert.crt" -n codesign -p test
ucl import -i "..\init_files\server.crt" -n codesignCA -p test

rem Sync the certificate with Windows Certificate Manager for the current user
echo Sync the certificate with Windows Certificate Manager for the current user
ucl sync-cert -n codesign
ucl sync-cert -n codesignCA
