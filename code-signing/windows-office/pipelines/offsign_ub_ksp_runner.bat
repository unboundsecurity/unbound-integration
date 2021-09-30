@echo off
rem ==========================================================================
rem
rem
rem  This tool is used for signing VBA projects contained in Office files.
rem
rem  The tool depends on SignTool.exe (from the Windows SDK) being installed and SIPs being registered. Besides,
rem  the tool calls coffclearsig.exe to remove any existing signatures in the currently processed file before
rem  signing. Please ensure that offsign.bat and offclearsig.exe are in the same directory.
rem
rem  Paremeters:
rem     -c      subcommand to signer
rem	-p	partition     	(optional)
rem     -u      UKC username	(optional)
rem	-w      UKC password	(optional)
rem
rem  Example:
rem     offsign_ub_ksp_runner.bat -c "sign /v /t http://timestamp.digicert.com /fd sha256 /n cert book_with_macro_signed.xlsm" -p test -u username -w password
rem ==========================================================================

setlocal
set helpCmd=help
set ACTION=ERROR
set errNo=0

set E_COMMAND_PARAM="Signer command (-c) must be provided."

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
set ErrNo_COMMAND_PARAM=11

rem ==========================================================================

rem Set CMD parameters
set source_dir=%~dp0

:loop
IF NOT "%1"=="" (
	IF "%1"=="-c" (
        SET command=%~2
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


if "%command%"=="" (
	set ACTION=%E_COMMAND_PARAM%
	set errNo=%ErrNo_COMMAND_PARAM%
	goto LFail
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


rem set sign_cmd=%sign_cmd% %signtoolPath% sign /v %timestamp_cmd% /fd sha256 /n %cert% %input%
set sign_cmd=%sign_cmd% %signtoolPath% %command%


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
if %ACTION%==%E_COMMAND_PARAM% (
	goto LUsage
)
echo You should fix the problem and re-run OffSign.
echo.
goto EOF

:LUsage
echo.
echo offsign_ub_ksp_runner.bat -- signing VBA projects contained in Office files.
echo.
echo Usage:
echo		-c		subcommand to signer
echo		-p		partition	(optional)
echo		-u		UKC username	(optional)
echo		-w		UKC password	(optional)
goto EOF

:LDone
goto EOF

:EOF
EXIT /B %errno%
