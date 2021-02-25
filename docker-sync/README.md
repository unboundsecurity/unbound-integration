This Python script syncronizes the target keys between Docker and the UKC.

It has one parameter:

`--creds`


This parameter contains the credentials for the UKC. It has the format:

`'{"username":"<username>","password":"<password>"}'`

## Prerequisites
This script requires the pypkcs11 library. You can obtain the library [here](https://github.com/unbound-tech/unbound-pypkcs11).
