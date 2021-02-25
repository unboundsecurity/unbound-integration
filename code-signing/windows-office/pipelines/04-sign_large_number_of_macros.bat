@echo off
rem If you are using your own certificate, edit this varialbe with the value of its unique "Issued To" field
set num_of_signatures=100

set cert=RaboBankDemo

set my_signtool="C:\Program Files (x86)\Windows Kits\10\bin\10.0.19041.0\x86\signtool.exe"

if not exist "..\signed_files" mkdir "..\signed_files"

rem Make large number of signatures on Word document for performance test
echo Signing macros of Word document in ..\signed_files\word_doc_with_macros_signed.xlsm
copy ..\files_to_sign\word_doc_with_macros.docm ..\signed_files\word_doc_with_macros_signed.docm
for /L %%i in (1, 1, %num_of_signatures%) do (
	%my_signtool% sign /v /t http://timestamp.digicert.com /fd sha256 /n %cert% "..\signed_files\word_doc_with_macros_signed.docm"
)

rem Run sync-cert for easier UKC log analysis
rem ucl sync-cert -n codesign
