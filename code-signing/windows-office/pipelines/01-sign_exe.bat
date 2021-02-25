@echo off
if not exist "..\signed_files" mkdir "..\signed_files"

rem Sign a windows executable
copy ..\files_to_sign\sha256sum.exe ..\signed_files\sha256sum_signed.exe
echo Signing a Windows exe file ..\signed_files\sha256sum_signed.exe
ucl sign-code -n codesign -p test --hash sha256 --file "..\signed_files\sha256sum_signed.exe" -t http://timestamp.digicert.com

rem Sign a Dynamic-link Library
copy ..\files_to_sign\libintl-8.dll ..\signed_files\libintl-8_signed.dll
echo Signing a Windows DLL file ..\signed_files\ibintl-8_signed.dll
ucl sign-code -n codesign -p test --hash sha256 --file "..\signed_files\libintl-8_signed.dll" -t http://timestamp.digicert.com
