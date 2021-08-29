@echo off
rem ==========================================================================
rem
rem
rem  This tool is used for signing of signatures for VBA projects contained in Office files.
rem
rem  The tool depends on SignTool.exe (from the Windows SDK) being installed and SIPs being registered. Besides,
rem  the tool will call coffclearsig.exe to remove any existing signatures in the currently processed file before
rem  signing, please ensure the offsign.bat and offclearsig.exe are in the same directory.
rem
rem  Paremeters:
rem     -i 			input file to sign
rem     -c          signing certificate name in store
rem		-p			partition     	(optional)
rem     -u          UKC username	(optional)
rem		-w          UKC password	(optional)
rem		-t          timestamp URL	(optional)
rem
rem  Example:
rem     offsign_ub_ksp_runner.bat -i mydoc.docx -c DEV -p test -u user -w 123456
rem ==========================================================================

setlocal
set helpCmd=help
set ACTION=ERROR
set errNo=0

set E_INPUT_PARAM="Input file (-i) must be provided."
set E_CERTIFICATE_PARAM="Signing certificate (-c) must be provided."

set E_SIGN_NOT_FOUND="Failed when searching signtool.exe."
set E_CLEAR_NOT_FOUND="Failed when searching offclearsig.exe."
set E_FAIL_CLEARSIG="Failed when executing the clear command."
set E_FAIL_1ST_SIGN="Failed when executing the first sign command."
set E_FAIL_2ND_SIGN="Failed when executing the second sign command."
set E_FAIL_3RD_SIGN="Failed when executing the third sign command."

set ErrNo_SIGN_NOT_FOUND=1
set ErrNo_CLEAR_NOT_FOUND=2
set ErrNo_FAIL_CLEARSIG=3
set ErrNo_FAIL_1ST_SIGN=4
set ErrNo_FAIL_2ND_SIGN=6
set ErrNo_FAIL_3RD_SIGN=8

set ErrNo_INPUT_PARAM=10
set ErrNo_CERTIFICATE_PARAM=11

rem ==========================================================================

rem Set CMD parameters
set source_dir=%~dp0

:loop
IF NOT "%1"=="" (
    IF "%1"=="-i" (
        SET input=%2
        SHIFT
    )
	IF "%1"=="-c" (
        SET cert=%2
        SHIFT
    )
	IF "%1"=="-p" (
        SET partition=%2
        SHIFT
    )
	IF "%1"=="-u" (
        SET user=%2
        SHIFT
    )
	IF "%1"=="-w" (
        SET password=%2
        SHIFT
    )
	IF "%1"=="-t" (
        SET timestamp=%2
        SHIFT
    )
	IF "%1"=="-h" (
        goto LUsage
    )
	IF "%1"=="--help" (
        goto LUsage
    )
	IF "%1"=="help" (
        goto LUsage
    )
    SHIFT
    GOTO :loop
)

if "%input%"=="" (
	set ACTION=%E_INPUT_PARAM%
	set errNo=%ErrNo_INPUT_PARAM%
	goto LUsage
)

if "%cert%"=="" (
	set ACTION=%E_CERTIFICATE_PARAM%
	set errNo=%ErrNo_CERTIFICATE_PARAM%
	goto LUsage
)

rem =================== CHANGE THIS ACCORDING TO YOUR ENVIRONMENT ======================================
rem set paths
set signtoolPath="C:\Program Files (x86)\Windows Kits\10\bin\10.0.20348.0\x86\signtool.exe"
set ub_ksp_runner="C:\Program Files\Dyadic\ekm-client\bin\x86\ub_ksp_runner.exe"


rem ==========================================================================
rem Find if signtool.exe is located in the specified file path

IF EXIST %signtoolPath% (
	goto LFindOffClearSig
)

set ACTION=%E_SIGN_NOT_FOUND%
set errNo=%ErrNo_SIGN_NOT_FOUND%
goto LFail


rem ==========================================================================
rem Find if OffClearSig.exe is located in the same path as OffSign.bat

:LFindOffClearSig
set offclearsig=offclearsig.exe
set offclearsigPath="%source_dir%%offclearsig%"
IF EXIST %offclearsigPath% (
	goto LRunCommand
)

set ACTION=%E_CLEAR_NOT_FOUND%
set errNo=%ErrNo_CLEAR_NOT_FOUND%
goto LFail


rem ==========================================================================
rem Run sign and verify commands

:LRunCommand
call %offclearsigPath% %input%
if %errorlevel%==0 (
	goto LRun1stSign
) else (
	set ACTION=%E_FAIL_CLEARSIG%
	set errNo=%ErrNo_FAIL_CLEARSIG%
	goto LFail
)

:LRun1stSign
rem prepare ub_ksp_runner input params
if not "%partition%"=="" (
	set sign_cmd=%sign_cmd% -p %partition%
)

if not "%user%"=="" (
	set sign_cmd=%sign_cmd% -u %user%
)

if not "%password%"=="" (
	set sign_cmd=%sign_cmd% -w %password%
)

if not "%timestamp%"=="" (
	set timestamp_cmd=/t %timestamp%
)

set sign_cmd=%sign_cmd% %signtoolPath% sign /v %timestamp_cmd% /fd sha256 /n %cert% %input%

rem echo sign_cmd=%sign_cmd%

%ub_ksp_runner% %sign_cmd% 
rem echo errorcode=%errorlevel%

if %errorlevel%==0 (
	goto LRun2ndSign
) else (
	set ACTION=%E_FAIL_1ST_SIGN%
	set errNo=%ErrNo_FAIL_1ST_SIGN%
	goto LFail
)

:LRun2ndSign
call %ub_ksp_runner% %sign_cmd% 
if %errorlevel%==0 (
	goto LRun3rdSign
) else (
	set ACTION=%E_FAIL_2ND_SIGN%
	set errNo=%ErrNo_FAIL_2ND_SIGN%
	goto LFail
)

:LRun3rdSign
call %ub_ksp_runner% %sign_cmd%
if %errorlevel%==0 (
	goto LDone
) else (
	set ACTION=%E_FAIL_3RD_SIGN%
	set errNo=%ErrNo_FAIL_3RD_SIGN%
	goto LFail
)

:LUsage
echo.
echo offsign_ub_ksp_runner.bat -- signing and verification of signatures for VBA projects contained in Office files.
echo.
echo Usage:
echo		-i		input file to sign
echo		-c		signing certificate name in store
echo		-p		partition	(optional)
echo		-u		UKC username	(optional)
echo		-w		UKC password	(optional)
echo		-t		Timestamp URL	(optional)
goto EOF

:LFail
echo Error!
echo %ACTION:"=%
if %ACTION%==%E_SIGN_NOT_FOUND% (
	echo The wrong path:
	echo 	%signtoolPath%
)
if %ACTION%==%E_CLEAR_NOT_FOUND% (
	echo The wrong path:
	echo 	%offclearsigPath%
)
echo You should fix the problem and re-run OffSign.
echo.
goto EOF

:LDone
goto EOF

:EOF
EXIT /B %errno%
