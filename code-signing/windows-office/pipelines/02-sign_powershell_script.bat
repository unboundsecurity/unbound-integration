@echo off
if not exist "..\signed_files" mkdir "..\signed_files"

rem Sign a PowerShell script
copy ..\files_to_sign\PSScript.ps1 ..\signed_files\PSScript_signed.ps1
echo Signing a PowerShell script ..\signed_files\PSScript_signed.ps1
ucl sign-code -n codesign -p test --hash sha256 --file ..\signed_files\PSScript_signed.ps1 -t http://timestamp.digicert.com