# Integrating UKC with MS SQL Server Always Encrypted

Unbound Key Control ("UKC") can be used to create keys that can be used with Microsoft SQL Server with the Always Encrypted ("AE") feature.

Refer to the following link for more information about AE:
[Encryption with AE](https://docs.microsoft.com/en-us/sql/relational-databases/security/encryption/always-encrypted-database-engine)

This repository contains 3 types of helper files:

1. DotNET - contains the source code for two helper programs:
   - [SetupAlwaysEncrypted](./SetupAlwaysEncrypted/DotNET) - Creates the column encryption key.
   - [UseAlwaysEncrypted](./UseAlwaysEncrypted) - Execute a test that creates data and then decrypts it.
1. [PowerShell](./SetupAlwaysEncrypted/PowerShell) - script that creates the CEK.
1. [SQL](./SetupAlwaysEncrypted/SQL) - sample SQL commands for creating the CMK, CEK, and a table with encrypted columns.


## 1. DotNET 

### Prerequisites
- Microsoft SQL Server 2008 or newer
- A Column Master Key ("CMK"), which is called *AlwaysEncryptedCMK* in the examples

### SetupAlwaysEncrypted Usage
[SetupAlwaysEncrypted](./SetupAlwaysEncrypted/DotNET) can be used to create the Column Encryption Key (CEK).

Compile the program and then create the CEK by running the command:

       SetupAlwaysEncrypted.exe AlwaysEncryptedCMK

This program creates a new Column Encryption Key (CEK) and encrypts it with the CMK.

### UseAlwaysEncrypted Usage
[UseAlwaysEncrypted](./UseAlwaysEncrypted) executes a test that creates data and then decrypts it using the sample program.

    UseAlwaysEncrypted.exe <IP address> encrypt_test

This program does the following:

- Creates 3 new records in the database.
- Prints the contents of the records containing a specific SSN (“123-45-6789”)
- Prints the contents of the records containing an additional specific SSN (“111-22-3333”)
- Prints the content of all the records.

## 2. PowerShell

Run the [PowerShell script (ukc_create_cek.ps1)](./SetupAlwaysEncrypted/PowerShell/ukc_create_cek.ps1).
	
Use the output of the script in the [SQL command](./SetupAlwaysEncrypted/SQL) to create the CEK.
	
## 3. SQL Samples

Additional SQL commands are provided in the [SQL directory](./SetupAlwaysEncrypted/SQL) to create the CMK and to create a sample table with encrypted columns.
