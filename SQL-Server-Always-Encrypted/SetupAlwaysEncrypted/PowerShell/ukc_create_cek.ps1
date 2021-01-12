##############################################################################################
#
# This script generates an encoded encryption key for generation of a column encryption key (CEK)
# for MS-SQL Always Encrypted feature with the Unbound cryptographic key storage provider.
#
# The generated value should be used as the ENC_VALUE when creating CEK.
# 
# For more information, see "Unbound Key Control Integration Guide".
#
#
# Sample output:
# Use the following in your CEK SQL for ENC_BYTES: 
# 0x015400000164007900610064006900630020007300650063007500720069007400790020006B0065007900200
# 0730074006F0072006100670065002000700072006F00760069006400650072002F007....
#
##############################################################################################

$dyadicProviderName = "Dyadic Security Key Storage Provider"

# UKC server (EP) url.
$UkcServerUrl = "https://your.ukc.server.and.port:8443"

# Your UKC username/partition and password
$username = "youruser@yourpartition"
$password = "yourpassword"

# The name of your RSA key in UKC
# Make sure this matches the master encryption key in your database
$keyName = "test7"
$keyPath = "$($dyadicProviderName)/$($keyName)"

# Use KMIP to get random bytes with entropy
$KmipRequest = @"
{
   "tag":"RequestMessage",
   "value":[
      {
         "tag":"RequestHeader",
         "value":[
            {
               "tag":"ProtocolVersion",
               "value":[
                  {
                     "tag":"ProtocolVersionMajor",
                     "type":"Integer",
                     "value":1
                  },
                  {
                     "tag":"ProtocolVersionMinor",
                     "type":"Integer",
                     "value":0
                  }
               ]
            },
            {
               "tag":"Authentication",
               "value":[
                  {
                     "tag":"Credential",
                     "value":[
                        {
                           "tag":"CredentialType",
                           "type":"Enumeration",
                           "value":"UsernameAndPassword"
                        },
                        {
                           "tag":"CredentialValue",
                           "value":[
                              {
                                 "tag":"Username",
                                 "type":"TextString",
                                 "value":"$($username)"
                              },
                              {
                                 "tag":"Password",
                                 "type":"TextString",
                                 "value":"$($password)"
                              }
                           ]
                        }
                     ]
                  }
               ]
            },
            {
               "tag":"BatchCount",
               "type":"Integer",
               "value":1
            }
         ]
      },
      {
         "tag":"BatchItem",
         "value":[
            {
               "tag":"Operation",
               "type":"Enumeration",
               "value":"RNGRetrieve"
            },
            {
               "tag":"RequestPayload",
               "value":[
                  {
                     "tag":"DataLength",
                     "type":"Integer",
                     "value":32
                  }
               ]
            }
         ]
      }
   ]
}
"@

# Use this code if you need to authenticate with a client certificate (server settings no-cert=0)
#$client_certificate = Get-PfxCertificate -FilePath "path\to\your\client_certificate.pfx"
#$response = Invoke-RestMethod -Certificate $client_certificate -Uri "$($UkcServerUrl)/kmip" -Method Post -Body $KmipRequest -ContentType 'application/json'
# Otherwise, to authenticate when the server is set to no-cert mode (no-cert=1) you can use this:
$response = Invoke-RestMethod -Uri "$($UkcServerUrl)/kmip" -Method Post -Body $KmipRequest -ContentType 'application/json'

# Uncomment this if you want to debug the response from UKC
# echo $response | ConvertTo-Json -Depth 100

$byteString =  $response.value[1].value[2].value.value
$bytes = $byteString -split '(..)' | ? { $_ } |  % {[Convert]::ToInt16($_, 16) }
$randomBuffer = $bytes
$rngProvider = New-Object System.Security.Cryptography.RNGCryptoServiceProvider
$rngProvider.GetBytes($randomBuffer)
$cspProvider = New-Object System.Data.SqlClient.SqlColumnEncryptionCngProvider
$encrypted = $cspProvider.EncryptColumnEncryptionKey($keyPath, "RSA_OAEP", $randomBuffer)

$encodedBytes = [system.String]::Join("", ($encrypted | % { [String]::Format("{0:x2}", $_).ToUpper()}))

echo "Use the following in your CEK SQL for ENC_BYTES: "
echo "0x$($encodedBytes)"
