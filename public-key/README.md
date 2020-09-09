# Unbound Public Key

Unbound packages are provided with a signature that you can use to verify the integrity of the package. In addition, the certicate can be used to verify the public key.

## Verify a package using the public key

Use the following procedure to verify the package.

1. Download Unbound's public key from this repo. The file is called unbound.pgp.
1. To verify the RPM distro:

     a. Import the Unbound public key.
	 
     `rpm --import unbound.pgp`
	 
	 No response indicates success.

    b. Verify the RPM using the command:
	 
     `rpm -K <Unbound package>.rpm`
	 
	Results:
    - OK: `<filepath.rpm> :  md5 OK`
	
    - Unknown: `<filepath.rpm> :  md5 NOT OK (MISSING KEYS:  PGP#<pgp-keyid>)` This response indicates that the validation key could not be found in the RPM repository. 

    - Bad - any other output - file is corrupted.
	
1. To verify the DEB distro in Ubuntu:

    a. Make a keyring folder and import the Unbound public key into it:
    
	```
	mkdir /usr/share/debsig/keyrings/8C96D305FA28E1EF
	touch /usr/share/debsig/keyrings/8C96D305FA28E1EF/debsig.gpg
	gpg --no-default-keyring --keyring /usr/share/debsig/keyrings/8C96D305FA28E1EF/debsig.gpg --import unbound.pgp
	```
	
	**Note:** The 16 character directory name used in these commands is the last 16 characters of the key fingerprint. You can find the fingerprint using the command: `gpg  --with-fingerprint unbound.pgp`
	
	
    b. Make a folder for the file verification policy:
	
	`mkdir /etc/debsig/policies/8C96D305FA28E1EF`
	
    c. In the folder, create the policy file.
	
	- The policy file for Ubuntu 16.04:
	```
	cat << EOF > /etc/debsig/policies/8C96D305FA28E1EF/unbound.pol
	<?xml version="1.0"?>
	<!DOCTYPE Policy SYSTEM "https://www.debian.org/debsig/1.0/policy.dtd">
	<Policy xmlns="http://www.debian.org/debsig/1.0/"> 
    <Origin Name="test" id="8C96D305FA28E1EF" Description="UnboundTech"/>
    <Selection>
	    <Required Type="origin" File="debsig.gpg" id="8C96D305FA28E1EF"/>    
	</Selection>
    <Verification MinOptional="0"> 
        <Required Type="origin" File="debsig.gpg" id="8C96D305FA28E1EF"/>
	</Verification>
    </Policy>
	EOF
	```
	
	- The policy file for Ubuntu 18.04:
	```
        cat << EOF > /etc/debsig/policies/8C96D305FA28E1EF/unbound.pol
        <?xml version="1.0"?>
        <!DOCTYPE Policy SYSTEM "https://www.debian.org/debsig/1.0/policy.dtd">
        <Policy xmlns="https://www.debian.org/debsig/1.0/"> 
    <Origin Name="test" id="8C96D305FA28E1EF" Description="UnboundTech"/>
    <Selection>
            <Required Type="origin" File="debsig.gpg" id="8C96D305FA28E1EF"/>
        </Selection>
    <Verification MinOptional="0">
            <Required Type="origin" File="debsig.gpg" id="8C96D305FA28E1EF"/>
        </Verification>
    </Policy>
	EOF
	```
	
    d. Verify the DEB file.
	
	`debsig-verify -v <Unbound package>.deb`
	
	The expected output:

    ```
	debsig: Starting verification for: ekm-client_2.0.2007.45954.deb9_amd64.deb
	debsig: Using policy directory: /etc/debsig/policies/8C96D305FA28E1EF
	debsig:   Parsing policy file: /etc/debsig/policies/8C96D305FA28E1EF/unbound.pol
	debsig:     Checking Selection group(s).
	debsig:       Processing 'origin' key...
	debsig:     Selection group(s) passed, policy is usable.
	debsig: Using policy file: /etc/debsig/policies/8C96D305FA28E1EF/unbound.pol
	debsig:     Checking Verification group(s).
	debsig:       Processing 'origin' key...
	debsig:     Verification group(s) passed, deb is validated.
	debsig: Verified package from 'UnboundTech' 
    ```
	
## Verify the certificate

Use the following procedure to verify the certificate.

1. Validate with the certificate authority directly using this command:

    `openssl x509 -inform pem -in unbound-cert.pem -noout -text`

    This proves to the verifier that it was signed by the signer. The results of the command are:
	
	````
    Certificate:
        Data:
            Version: 3 (0x2)
            Serial Number:
                0e:67:76:2c:f4:45:1e:19:f6:d3:fe:8b:43:6c:d6:46
            Signature Algorithm: sha256WithRSAEncryption
            Issuer: C = US, O = DigiCert Inc, OU = www.digicert.com, CN = DigiCert EV Code Signing CA (SHA2)
            Validity
                Not Before: May 29 00:00:00 2018 GMT
                Not After : Jun  2 12:00:00 2021 GMT
            Subject: jurisdictionC = IL, businessCategory = Private Organization, serialNumber = 514944917, C = IL, L = Petah Tiqwa, O = Unbound Tech LTD, CN = Unbound Tech LTD
            Subject Public Key Info:
                Public Key Algorithm: rsaEncryption
                    RSA Public-Key: (2048 bit)
                    Modulus:
                        00:e4:e8:32:24:59:95:ea:11:9a:bd:e6:68:ea:14:
                        8f:92:3c:ca:b9:ae:0c:3b:65:02:ba:89:0b:9d:27:
                        de:24:9a:8d:f9:f7:7a:3f:a9:6b:a7:c0:ee:b7:04:
                        df:8f:05:df:08:00:dc:28:70:56:60:3c:b1:2d:7f:
                        c2:ca:56:13:d9:9c:72:c8:2d:7b:78:0a:bf:65:10:
                        14:ed:4c:f6:f6:ec:dc:4f:fc:b3:ad:c4:2c:a4:df:
                        80:c4:10:04:83:3b:e7:a5:96:fc:39:88:7c:1c:08:
                        2f:c4:c3:61:5a:60:a9:6e:88:3a:f0:0c:89:00:24:
                        dd:4f:39:e9:02:04:ea:1b:f1:ac:e3:88:c6:c4:68:
                        ac:a8:9c:22:99:c1:02:91:ee:43:49:9d:ae:95:10:
                        55:4f:a0:db:86:b8:f9:68:b4:82:30:ef:91:1a:de:
                        3a:4f:ef:cb:54:9f:be:6b:11:ea:1f:e1:a3:dc:d6:
                        c8:9e:e0:d2:6c:a3:c3:73:0b:34:53:55:c3:10:2f:
                        6f:5a:f5:a1:1e:59:d0:f1:7e:22:40:de:c0:6b:f9:
                        98:50:a2:bc:e6:58:a6:d7:2c:dc:de:cf:62:af:c5:
                        1e:8e:02:50:16:5e:21:77:80:3e:fd:99:43:bb:11:
                        99:3f:a5:1c:3c:61:18:2b:56:16:2d:13:91:79:a0:
                        c0:71
                    Exponent: 65537 (0x10001)
            X509v3 extensions:
                X509v3 Authority Key Identifier:
                    keyid:8F:E8:7E:F0:6D:32:6A:00:05:23:C7:70:97:6A:3A:90:FF:6B:EA:D4
	    
                X509v3 Subject Key Identifier:
                    B7:B8:3E:BC:57:60:92:40:39:78:19:C0:19:4E:CD:CD:CF:90:AC:C4
                X509v3 Subject Alternative Name:
                    othername:<unsupported>
                X509v3 Key Usage: critical
                    Digital Signature
                X509v3 Extended Key Usage:
                    Code Signing
                X509v3 CRL Distribution Points:
	    
                    Full Name:
                      URI:http://crl3.digicert.com/EVCodeSigningSHA2-g1.crl
	    
                    Full Name:
                      URI:http://crl4.digicert.com/EVCodeSigningSHA2-g1.crl
	    
                X509v3 Certificate Policies:
                    Policy: 2.16.840.1.114412.3.2
                      CPS: https://www.digicert.com/CPS
                    Policy: 2.23.140.1.3
	    
                Authority Information Access:
                    OCSP - URI:http://ocsp.digicert.com
                    CA Issuers - URI:http://cacerts.digicert.com/DigiCertEVCodeSigningCA-SHA2.crt
	    
                X509v3 Basic Constraints: critical
                    CA:FALSE
        Signature Algorithm: sha256WithRSAEncryption
             a7:1d:ba:3a:cb:0d:81:2b:2f:ca:b6:85:8d:82:2b:e6:c6:8d:
             79:3a:f1:c9:9c:51:f5:de:47:1a:17:b2:d0:cf:cf:9e:49:ba:
             0c:b8:f0:a9:28:47:d2:05:b4:a3:53:a3:b7:a0:66:3d:e0:f6:
             32:63:0d:e1:01:f0:9b:c4:5f:34:2f:ff:fa:98:83:87:66:56:
             79:f9:5f:90:f0:4a:57:97:16:18:ea:7c:04:85:78:d3:e8:8b:
             13:6c:7c:00:af:ec:4a:25:c1:58:e4:0b:51:43:d9:5b:34:ea:
             25:51:68:30:fc:d2:d5:97:45:ee:c2:c3:36:5b:5d:d3:92:25:
             73:0a:0b:06:a2:63:c1:f6:06:19:7f:f0:29:27:de:80:0d:2a:
             42:a1:33:0d:12:1b:ad:91:5f:ee:64:92:7d:99:e6:f8:b8:e3:
             01:f3:8b:a1:2d:0a:84:22:fb:56:29:55:fc:33:f4:9f:64:c3:
             e7:f6:a3:a6:1d:56:59:41:83:3f:ee:4e:c5:2d:7c:ae:24:38:
             be:b5:83:d8:97:c4:61:f7:b0:28:75:be:27:2a:a9:32:91:3d:
             8d:66:fb:ff:5f:ca:61:cb:77:d9:f9:61:94:f0:44:3b:de:ef:
             a6:98:ae:39:c1:e6:9c:82:e5:c9:ec:70:3e:61:fd:ed:bb:b9:
             8f:f9:05:dc
	````

1. Check if the certificate is active using this command.
    
	`openssl ocsp -no_nonce -issuer digicert-chain.pem -cert unbound-cert.pem -CAfile digicert-chain.pem  -url http://ocsp.digicert.com`

    This shows that the certificate is active.

    If the certificate is valid, the output is:
 
    ```
    Response verify OK
    unbound-cert.pem: good
        This Update: Sep  9 03:09:01 2020 GMT
        Next Update: Sep 16 02:24:01 2020 GMT
    ```

1. OpenSSL shows the content of X509 certificate using the command:

    `openssl x509 -text -in digicert-chain.pem`
	
	The results are:
	
	````
    Certificate:
        Data:
            Version: 3 (0x2)
            Serial Number:
                0e:67:76:2c:f4:45:1e:19:f6:d3:fe:8b:43:6c:d6:46
            Signature Algorithm: sha256WithRSAEncryption
            Issuer: C = US, O = DigiCert Inc, OU = www.digicert.com, CN = DigiCert EV Code Signing CA (SHA2)
            Validity
                Not Before: May 29 00:00:00 2018 GMT
                Not After : Jun  2 12:00:00 2021 GMT
            Subject: jurisdictionC = IL, businessCategory = Private Organization, serialNumber = 514944917, C = IL, L = Petah Tiqwa, O = Unbound Tech LTD, CN = Unbound Tech LTD
            Subject Public Key Info:
                Public Key Algorithm: rsaEncryption
                    RSA Public-Key: (2048 bit)
                    Modulus:
                        00:e4:e8:32:24:59:95:ea:11:9a:bd:e6:68:ea:14:
                        8f:92:3c:ca:b9:ae:0c:3b:65:02:ba:89:0b:9d:27:
                        de:24:9a:8d:f9:f7:7a:3f:a9:6b:a7:c0:ee:b7:04:
                        df:8f:05:df:08:00:dc:28:70:56:60:3c:b1:2d:7f:
                        c2:ca:56:13:d9:9c:72:c8:2d:7b:78:0a:bf:65:10:
                        14:ed:4c:f6:f6:ec:dc:4f:fc:b3:ad:c4:2c:a4:df:
                        80:c4:10:04:83:3b:e7:a5:96:fc:39:88:7c:1c:08:
                        2f:c4:c3:61:5a:60:a9:6e:88:3a:f0:0c:89:00:24:
                        dd:4f:39:e9:02:04:ea:1b:f1:ac:e3:88:c6:c4:68:
                        ac:a8:9c:22:99:c1:02:91:ee:43:49:9d:ae:95:10:
                        55:4f:a0:db:86:b8:f9:68:b4:82:30:ef:91:1a:de:
                        3a:4f:ef:cb:54:9f:be:6b:11:ea:1f:e1:a3:dc:d6:
                        c8:9e:e0:d2:6c:a3:c3:73:0b:34:53:55:c3:10:2f:
                        6f:5a:f5:a1:1e:59:d0:f1:7e:22:40:de:c0:6b:f9:
                        98:50:a2:bc:e6:58:a6:d7:2c:dc:de:cf:62:af:c5:
                        1e:8e:02:50:16:5e:21:77:80:3e:fd:99:43:bb:11:
                        99:3f:a5:1c:3c:61:18:2b:56:16:2d:13:91:79:a0:
                        c0:71
                    Exponent: 65537 (0x10001)
            X509v3 extensions:
                X509v3 Authority Key Identifier:
                    keyid:8F:E8:7E:F0:6D:32:6A:00:05:23:C7:70:97:6A:3A:90:FF:6B:EA:D4
	    
                X509v3 Subject Key Identifier:
                    B7:B8:3E:BC:57:60:92:40:39:78:19:C0:19:4E:CD:CD:CF:90:AC:C4
                X509v3 Subject Alternative Name:
                    othername:<unsupported>
                X509v3 Key Usage: critical
                    Digital Signature
                X509v3 Extended Key Usage:
                    Code Signing
                X509v3 CRL Distribution Points:
	    
                    Full Name:
                      URI:http://crl3.digicert.com/EVCodeSigningSHA2-g1.crl
	    
                    Full Name:
                      URI:http://crl4.digicert.com/EVCodeSigningSHA2-g1.crl
	    
                X509v3 Certificate Policies:
                    Policy: 2.16.840.1.114412.3.2
                      CPS: https://www.digicert.com/CPS
                    Policy: 2.23.140.1.3
	    
                Authority Information Access:
                    OCSP - URI:http://ocsp.digicert.com
                    CA Issuers - URI:http://cacerts.digicert.com/DigiCertEVCodeSigningCA-SHA2.crt
	    
                X509v3 Basic Constraints: critical
                    CA:FALSE
        Signature Algorithm: sha256WithRSAEncryption
             a7:1d:ba:3a:cb:0d:81:2b:2f:ca:b6:85:8d:82:2b:e6:c6:8d:
             79:3a:f1:c9:9c:51:f5:de:47:1a:17:b2:d0:cf:cf:9e:49:ba:
             0c:b8:f0:a9:28:47:d2:05:b4:a3:53:a3:b7:a0:66:3d:e0:f6:
             32:63:0d:e1:01:f0:9b:c4:5f:34:2f:ff:fa:98:83:87:66:56:
             79:f9:5f:90:f0:4a:57:97:16:18:ea:7c:04:85:78:d3:e8:8b:
             13:6c:7c:00:af:ec:4a:25:c1:58:e4:0b:51:43:d9:5b:34:ea:
             25:51:68:30:fc:d2:d5:97:45:ee:c2:c3:36:5b:5d:d3:92:25:
             73:0a:0b:06:a2:63:c1:f6:06:19:7f:f0:29:27:de:80:0d:2a:
             42:a1:33:0d:12:1b:ad:91:5f:ee:64:92:7d:99:e6:f8:b8:e3:
             01:f3:8b:a1:2d:0a:84:22:fb:56:29:55:fc:33:f4:9f:64:c3:
             e7:f6:a3:a6:1d:56:59:41:83:3f:ee:4e:c5:2d:7c:ae:24:38:
             be:b5:83:d8:97:c4:61:f7:b0:28:75:be:27:2a:a9:32:91:3d:
             8d:66:fb:ff:5f:ca:61:cb:77:d9:f9:61:94:f0:44:3b:de:ef:
             a6:98:ae:39:c1:e6:9c:82:e5:c9:ec:70:3e:61:fd:ed:bb:b9:
             8f:f9:05:dc
    -----BEGIN CERTIFICATE-----
    MIIFhDCCBGygAwIBAgIQDmd2LPRFHhn20/6LQ2zWRjANBgkqhkiG9w0BAQsFADBs
    MQswCQYDVQQGEwJVUzEVMBMGA1UEChMMRGlnaUNlcnQgSW5jMRkwFwYDVQQLExB3
    d3cuZGlnaWNlcnQuY29tMSswKQYDVQQDEyJEaWdpQ2VydCBFViBDb2RlIFNpZ25p
    bmcgQ0EgKFNIQTIpMB4XDTE4MDUyOTAwMDAwMFoXDTIxMDYwMjEyMDAwMFowgaEx
    EzARBgsrBgEEAYI3PAIBAxMCSUwxHTAbBgNVBA8MFFByaXZhdGUgT3JnYW5pemF0
    aW9uMRIwEAYDVQQFEwk1MTQ5NDQ5MTcxCzAJBgNVBAYTAklMMRQwEgYDVQQHEwtQ
    ZXRhaCBUaXF3YTEZMBcGA1UEChMQVW5ib3VuZCBUZWNoIExURDEZMBcGA1UEAxMQ
    VW5ib3VuZCBUZWNoIExURDCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEB
    AOToMiRZleoRmr3maOoUj5I8yrmuDDtlArqJC50n3iSajfn3ej+pa6fA7rcE348F
    3wgA3ChwVmA8sS1/wspWE9mccsgte3gKv2UQFO1M9vbs3E/8s63ELKTfgMQQBIM7
    56WW/DmIfBwIL8TDYVpgqW6IOvAMiQAk3U856QIE6hvxrOOIxsRorKicIpnBApHu
    Q0mdrpUQVU+g24a4+Wi0gjDvkRreOk/vy1SfvmsR6h/ho9zWyJ7g0myjw3MLNFNV
    wxAvb1r1oR5Z0PF+IkDewGv5mFCivOZYptcs3N7PYq/FHo4CUBZeIXeAPv2ZQ7sR
    mT+lHDxhGCtWFi0TkXmgwHECAwEAAaOCAeowggHmMB8GA1UdIwQYMBaAFI/ofvBt
    MmoABSPHcJdqOpD/a+rUMB0GA1UdDgQWBBS3uD68V2CSQDl4GcAZTs3Nz5CsxDAn
    BgNVHREEIDAeoBwGCCsGAQUFBwgDoBAwDgwMSUwtNTE0OTQ0OTE3MA4GA1UdDwEB
    /wQEAwIHgDATBgNVHSUEDDAKBggrBgEFBQcDAzB7BgNVHR8EdDByMDegNaAzhjFo
    dHRwOi8vY3JsMy5kaWdpY2VydC5jb20vRVZDb2RlU2lnbmluZ1NIQTItZzEuY3Js
    MDegNaAzhjFodHRwOi8vY3JsNC5kaWdpY2VydC5jb20vRVZDb2RlU2lnbmluZ1NI
    QTItZzEuY3JsMEsGA1UdIAREMEIwNwYJYIZIAYb9bAMCMCowKAYIKwYBBQUHAgEW
    HGh0dHBzOi8vd3d3LmRpZ2ljZXJ0LmNvbS9DUFMwBwYFZ4EMAQMwfgYIKwYBBQUH
    AQEEcjBwMCQGCCsGAQUFBzABhhhodHRwOi8vb2NzcC5kaWdpY2VydC5jb20wSAYI
    KwYBBQUHMAKGPGh0dHA6Ly9jYWNlcnRzLmRpZ2ljZXJ0LmNvbS9EaWdpQ2VydEVW
    Q29kZVNpZ25pbmdDQS1TSEEyLmNydDAMBgNVHRMBAf8EAjAAMA0GCSqGSIb3DQEB
    CwUAA4IBAQCnHbo6yw2BKy/KtoWNgivmxo15OvHJnFH13kcaF7LQz8+eSboMuPCp
    KEfSBbSjU6O3oGY94PYyYw3hAfCbxF80L//6mIOHZlZ5+V+Q8EpXlxYY6nwEhXjT
    6IsTbHwAr+xKJcFY5AtRQ9lbNOolUWgw/NLVl0XuwsM2W13TkiVzCgsGomPB9gYZ
    f/ApJ96ADSpCoTMNEhutkV/uZJJ9meb4uOMB84uhLQqEIvtWKVX8M/SfZMPn9qOm
    HVZZQYM/7k7FLXyuJDi+tYPYl8Rh97Aodb4nKqkykT2NZvv/X8phy3fZ+WGU8EQ7
    3u+mmK45weacguXJ7HA+Yf3tu7mP+QXc
    -----END CERTIFICATE-----
	
	````

1. GPG shows the content of the public key file using the command:

    `gpg2 -v --list-packets unbound.pgp`

    The expected response is:
	
	```
    gpg: armor header: Version: EKM
    # off=0 ctb=99 tag=6 hlen=3 plen=269
    :public key packet:
            version 4, algo 1, created 1527724800, expires 0
            pkey[0]: E4E832245995EA119ABDE668EA148F923CCAB9AE0C3B6502BA890B9D27DE249A8DF9F77A3FA96BA7C0EEB704DF8F05DF0800DC287056603CB12D7FC2CA5613D99C72C82D7B780ABF651014ED4CF6F6ECDC4FFCB3ADC42CA4DF80C41004833BE7A596FC39887C1C082FC4C3615A60A96E883AF00C890024DD4F39E90204EA1BF1ACE388C6C468ACA89C2299C10291EE43499DAE9510554FA0DB86B8F968B48230EF911ADE3A4FEFCB549FBE6B11EA1FE1A3DCD6C89EE0D26CA3C3730B345355C3102F6F5AF5A11E59D0F17E2240DEC06BF99850A2BCE658A6D72CDCDECF62AFC51E8E0250165E2177803EFD9943BB11993FA51C3C61182B56162D139179A0C071
            pkey[1]: 010001
            keyid: 8C96D305FA28E1EF
    # off=272 ctb=b4 tag=13 hlen=2 plen=7
    :user ID packet: "unbound"
    # off=281 ctb=89 tag=2 hlen=3 plen=313
    :signature packet: algo 1, keyid 8C96D305FA28E1EF
            version 4, created 1527724800, md5len 0, sigclass 0x13
            digest algo 8, begin of digest 1d 9c
            hashed subpkt 2 len 4 (sig created 2018-05-31)
            hashed subpkt 27 len 1 (key flags: 2F)
            hashed subpkt 11 len 6 (pref-sym-algos: 9 8 7 3 2 1)
            hashed subpkt 21 len 5 (pref-hash-algos: 8 2 9 10 11)
            hashed subpkt 22 len 3 (pref-zip-algos: 2 3 1)
            hashed subpkt 30 len 1 (features: 01)
            hashed subpkt 23 len 1 (keyserver preferences: 80)
            subpkt 16 len 8 (issuer key ID 8C96D305FA28E1EF)
            data: 403D97670F754106E6B8D40A27AB95826805198C641450459D15C53234677951270E120F00319C6D0384AEF4F425E93AF9A4B905909E17631D4B8C38A89C20863D45EAA03903EE3B62FF50872ED118FDCE1DBB4FB8026CC31970D2CF22D32614AA36522AC5D14F528131C67ED9BC467247711FF402FC2110F1E8CFC3F5EE845493CE8F353901D79B704EBD952FA8196DA89036745E644E7967179E79C620099939A5488D81DC6DDA4A1AF191B6DC560E19C32ED73B14D411EAE97BFF3AA826A663E536C8CAB05E72B6274DDEED3893809745304077EE245F18E0DD85EB49954A94D8BE460D4E5AAEC2D369FCD0658E10781D93C423496B311BB9562940F30F6D
    
	```
1. Check the details of the public key and see that the same modulus and public exponent exists in both files, i.e. this is the same key/certificate.
