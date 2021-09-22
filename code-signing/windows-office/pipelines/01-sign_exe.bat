@echo off
if not exist "..\signed_files" mkdir "..\signed_files"

rem Sign a windows executable
copy ..\files_to_sign\procexp64.exe ..\signed_files\procexp64_signed.exe
echo Signing a Windows exe file ..\signed_files\procexp64_signed.exe
ucl sign-code -n codesign -p test --hash sha256 --file "..\signed_files\procexp64_signed.exe" -t http://timestamp.digicert.com

rem Sign a Dynamic-link Library
copy ..\files_to_sign\dylog.dll ..\signed_files\dylog_signed.dll
echo Signing a Windows DLL file ..\signed_files\dylog_signed.dll
ucl sign-code -n codesign -p test --hash sha256 --file "..\signed_files\dylog_signed.dll" -t http://timestamp.digicert.com
