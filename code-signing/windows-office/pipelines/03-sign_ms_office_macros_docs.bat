@echo off
rem If you are using your own certificate, edit this varialbe with the value of its unique "Issued To" field
set cert=RaboBankDemo

set my_signtool="C:\Program Files (x86)\Windows Kits\10\bin\10.0.19041.0\x86\signtool.exe"

if not exist "..\signed_files" mkdir "..\signed_files"

rem Sign macros of Excel document
copy ..\files_to_sign\book_with_macro.xlsm ..\signed_files\book_with_macro_signed.xlsm
echo Signing macros of Excel document in ..\signed_files\book_with_macro_signed.xlsm
%my_signtool% sign /v /t http://timestamp.digicert.com /fd sha256 /n %cert% "..\signed_files\book_with_macro_signed.xlsm"

rem Sign macros of Word document
echo Signing macros of Word document in ..\signed_files\word_doc_with_macros_signed.docm
copy ..\files_to_sign\word_doc_with_macros.docm ..\signed_files\word_doc_with_macros_signed.docm
%my_signtool% sign /v /t http://timestamp.digicert.com /fd sha256 /n %cert% "..\signed_files\word_doc_with_macros_signed.docm"

rem Verify signed documents
echo Verifying signature of the signed macros 
%my_signtool% verify /pa "..\signed_files\book_with_macro_signed.xlsm"
%my_signtool% verify /pa "..\signed_files\word_doc_with_macros_signed.docm"