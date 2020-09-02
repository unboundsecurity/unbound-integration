# Unbound Public Key

Unbound packages are provided with a signature that you can use to verify the integrity of the package. In addition, the certicate can be used to verify the public key.

## Verify a package using the public key

Use the following procedure to verify the package.

1. Download Unbound's public key from this repo. The file is called ub.pgp.
1. To verify the RPM distro:

     a. Import the Unbound public key.
	 
     `rpm --import ub.pgp`

    b. Verify the RPM using the command:
	 
     `rpm -K <Unbound package>.rpm`
1. To verify the DEB distro in Ubuntu:

    a. Make a keyring folder and import the Unbound public key into it:
    
	```
	mkdir /usr/share/debsig/keyrings/8C96D305FA28E1EF
	touch /usr/share/debsig/keyrings/8C96D305FA28E1EF/debsig.gpg
	gpg --no-default-keyring --keyring /usr/share/debsig/keyrings/8C96D305FA28E1EF/debsig.gpg --import ub.pgp
	```
	
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

## Verify the certificate

Use the following procedure to verify the certificate.

1. Validate with the certificate authority directly using this command:

    `openssl x509 -inform pem -in certificate.pem -noout -text`

    This proves to the verifier that it was signed by the signer.

1. Check if the certificate is active using this command. Note that DigiCert is used as an example.
    
	`openssl ocsp -no_nonce -issuer <OSCPCHAINPUBLICKEYNAME> -cert certificate.pem -VAfile <OSCPCHAINPUBLICKEYNAME> -text -url http://ocsp.digicert.com -respout ocsptest`

    This shows that the certificate is active.

    If the certificate is valid, the output is:
 
    `Response verify OK`

1. OpenSSL shows the content of X509 certificate using the command:

    `openssl x509 -text -in <X509-certificate-file>`

1. GPG shows the content of the public key file using the command:

    `gpg2 -v --list-packets ub.gpg`

1. Check the details of the public key and see that the same modulus and public exponent exists in both files, i.e. this is the same key/certificate.

    Note:
	
    It is recommended to verify that the certificate is valid before sharing it with anyone who will be verifying packages.
