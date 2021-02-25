CREATE COLUMN MASTER KEY [CMK1]
 WITH
 (
 KEY_STORE_PROVIDER_NAME = N'MSSQL_CNG_STORE',
 KEY_PATH = N'Dyadic Security Key Storage Provider/AlwaysEncryptedCMK'
 
 )