# Unbound Microsoft Windows Code Signing Demo
This repo contains examples of how to use Unbound UKC for the following Microsoft Windows code signing use cases:
* Signing binary code: EXE and DLL files 
* Signing PowerShell scripts 
* Signing embedded macros code in Microsoft Office Word and Excel documents

## Prerequisites
1. UKC client 2007 or later
You need to have the client configured on partion name "test" and a USER without password assigned.

1. Signtool

	1. Download and install the appropriate Windows SDK (for Windows 10, download https://developer.microsoft.com/en-us/windows/downloads/windows-10-sdk).
	
	1. It is enough to check only "signing tools..." during installation.

3. Microsoft Visual C++ 2010 Redistributable Package (x86) - you must use this version: https://www.microsoft.com/en-us/download/details.aspx?id=26999.
	
4. Microsoft Office Subject Interface Packages for Digitally Signing VBA Projects.

	1. Download https://www.microsoft.com/en-us/download/confirmation.aspx?id=56617.
	
	1. Install the files in the *pipelines* directory of this project.
	
	1. Run the following commands to add the required DLLs to X86 registry:
	
		C:\Windows\SysWOW64\regsvr32.exe msosip.dll
		
		C:\Windows\SysWOW64\regsvr32.exe msosipx.dll
		
		*Note: It is important to use regsvr32 from SysWOW64 directory and NOT system32.
	
	Important - disregard the *readme.txt* in the package asking you to install VS redist 2015 - it will not work. Use 2010 as described in the previous steps.

## Usage
In the *pipelines* directory, there are scripts that run the demo. 

The script input files are located in the `files_to_sign` directory and the signed output is saved in the `signed_files` directory.

1. `init.bat` - initializes the environment by importing a pre-made key, cert and CA into UKC.

2. `01-sign_exe.bat` - sign windows executables and DLLs.

3. `02-sign_powershell_script.bat` - sign PowerShell scripts.

	Note: Signatures generated in 1 and 2 can be verified by right-clicking on the appropriate output file in *signed_files* dir, then selecting *properties*, and then *digital signatures*.

4. `03-sign_ms_office_macros_docs.bat` - sign and verify Microsoft Office macros contained within Microsoft Word and Excel files.

5. `04-sign_large_number_of_macros.bat` - sign Microsoft Word document macros in a loop for performance testing.

6. `05-sign_ms_office_macros_docs_V3.bat` - sign and verify macros contained within Microsoft Word and Excel files (V3 Secure signature). The signer used in this example allows signing with a specific UKC user.


## Sign a VBA macro in a Microsoft Office document manually inside the Office programs
Follow the instructions in "Digitally sign a macro project in Excel, PowerPoint, Publisher, Visio, Outlook, or Word section" in this link:
https://support.microsoft.com/en-us/office/digitally-sign-your-macro-project-956e9cc8-bbf6-4365-8bfa-98505ecd1c01


## Sign a Microsoft Office document manually inside Office programs
Generate a signature by selecting *File* → *Info* → *Protect* → *add a digital signature* → *select certificate*.

For verification, right-click on the signed file, then select *properties*, and then *digital signatures*.
