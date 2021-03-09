# Code-Signing with .NET

This sample code demostrates code-signing using .NET.

## Prerequisites

- UKC client on Windows
- UKC certificate synchronized to the keystore. See [ucl sync-cert](https://www.unboundsecurity.com/docs/UKC/UKC_User_Guide/HTML/Content/Products/UKC-EKM/UKC_User_Guide/CLI/cliCertManagement/WinKeyStore.html) for more details.

## Files

This repo contains the following files:

- CNGSigner.cs - sign using the Unbound CNG provider.
- CodeSigner.cs - sign using Microsoft crypto libraries.
- Program.cs - sample of signing using the previous two files.
- SignApp.csproj - sample project definition.

## Usage

Build the .NET executable using the files in this repo. 

As an example, assume the following:
1. We built an executable named *SignApp.exe*.
2. We have a file the we want to sign called *..\Notepad-unsigned.orig.msi*.
3. The SHA1 of the certificate is *3a448fa86d5cd3aa2787d420a94212ea8da4b791*.
4. The timestamp provider is *http://timestamp.digicert.com*.

Using the assumptions, run:
    `SignApp.exe "..\Notepad-unsigned.orig.msi" "3a448fa86d5cd3aa2787d420a94212ea8da4b791" "http://timestamp.digicert.com"`


