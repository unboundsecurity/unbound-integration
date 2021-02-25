# Unbound Tech MS-Windows code signing demo
This repo contains examples showing how to use Unbound Tech UKC for the following MS-Windows code signing use cases:
* Signing binary code: EXE and DLL files 
* Signing PowerShell scripts 
* Signing embedded macros code in MS-Office Word and Excel documents

## Prerequisites
1. UKC client 2007 or above
You need to have the client configured on partion name "test" with USER without password assigned.

1. Signtool

	1. download and install the appropriate Windows SDK (for Windows 10, download https://developer.microsoft.com/en-us/windows/downloads/windows-10-sdk)
	
	1. It is enough to check only "signing tools..." during installation

3. Microsoft Visual C++ 2010 Redistributable Package (x86) - strictly this version: https://www.microsoft.com/en-us/download/details.aspx?id=5555
	
4. Microsoft Office Subject Interface Packages for Digitally Signing VBA Projects

	1. Download https://www.microsoft.com/en-us/download/confirmation.aspx?id=56617
	
	1. Install the files in "pipelines" directory of this project
	
	1. run the following commands to add required DLLs to X86 registry:
	
		C:\Windows\SysWOW64\regsvr32.exe msosip.dll
		
		C:\Windows\SysWOW64\regsvr32.exe msosipx.dll
		
		*Note: It is important to use regsvr32 from SysWOW64 directory and NOT system32
	
	Important - disregard the readme.txt in the package asking you to install VS redist 2015 - it will not work. Use 2010 as described in the previous steps

## Usage
In the pipelines directory you will find the scripts that run the demo. 

The script input files are located in `files_to_sign` directory and the signed output is saved in `signed_files` directory.

1. `init.bat` - initializes environment by importing pre-made key, cert and CA into UKC

2. `01-sign_exe.bat` - sign windows executables and DLLs. To verify signature 

3. `02-sign_powershell_script.bat` - sign Power Shell scripts

	*signatures generated in 1 and 2 can be verified by right clicking on the appropriate output file in signed_files dir -> properties -> digital signarues

4. `03-sign_ms_office_macros_docs.bat` - sign and verify Microsoft Office macros contained withing MS Word and Excel files

5. `04-sign_large_number_of_macros.bat` - sign MS Word document macros in loop for performance testing


## Sign a VBA macro in MS office document manually inside MS office programs
Follow the instructions in "Digitally sign a macro project in Excel, PowerPoint, Publisher, Visio, Outlook, or Word section" in this link
https://support.microsoft.com/en-us/office/digitally-sign-your-macro-project-956e9cc8-bbf6-4365-8bfa-98505ecd1c01


## Sign a MS office document manually inside MS office programs
Generate a signature -> File → Info → Protect → add a igital asignature → select certificate (RaboBankDemo is the certificate in UKC that is synced with Windows Certificate Manager)

Verificateion -> Right-click on the signed file -> properties -> digital signarues

