# Unbound Performance Testing Using Jmeter

This repository uses [Apache Jmeter](https://jmeter.apache.org/) to test the performance of UKC and CoT.

See [Developing in Java](https://www.unboundtech.com/docs/UKC/UKC_Developers_Guide/HTML/Content/Products/UKC-EKM/UKC_Developers_Guide/DevelopingInJava/Developing_in_Java.htm) for more information about configuring the Java environment.

## Prerequisites for UKC Performance tests

You need the following before running the performance tests:
1. UKC cluster - installed and running
1. UKC client
1. Jmeter - install version 5.4 or newer
1. Maven CLI - installed

## Test the UKC Performance

### Set up the environment

1. Download this repository.
1. From the root of the downloaded repo, run the Maven command to compile the JAR for testing.

    `mvn package`
	
    This command creates the *ub-maven-test-1.0.0.jar* file in the *target/* directory.
1. Copy *ub-maven-test-1.0.0.jar* to the *lib* directory in your Jmeter installation.
1. Copy *ekm-java-provider-2.0.jar* from the UKC client to the *lib* directory in your Jmeter installation.

### Testing

The following tests are provided:
1. AES-GCM - encrypt and decrypt.
2. RSA-OAEP - decrypt.
3. ECDSA-P256 - sign.
4. ECDH-P256 - derive.

For the tests, first confirm that the default user does not have a password. (Note: A blank password is the default)

#### AES-GCM - encrypt and decrypt
1. Create a key for testing. For example:

    `ucl generate -t AES -p <PART_NAME> --name aes-256-key --purpose E`
1. Open the *UBUKCTest.jmx* file in Jmeter.
1. Run the relevant tests: Encrypt GCM or Decrypt GCM.

#### RSA-OAEP - decrypt
1. Create a key for testing. For example:

    `ucl generate -t RSA -p <PART_NAME> --name rsa-2048-key`
1. Open the *UBUKCTest.jmx* file in Jmeter.
1. Run the relevant test: Decrypt RSA.

#### ECDSA-P256 - sign
1. Create a key for testing. For example:

    `ucl generate -t ECC -p <PART_NAME> --name ecdsa-p256-key`
1. Open the *UBUKCTest.jmx* file in Jmeter.
1. Run the relevant test: Sign ECDSA.

#### ECDH-P256 - derive and encrypt
1. Create a key for testing. For example:

    `ucl generate -t ECC -p <PART_NAME> --purpose D --name ecdh-p256-key`
1. Open the *UBUKCTest.jmx* file in Jmeter.
1. Run the relevant tests: Derive ECDH.

## Prerequisites for CoT Performance tests

You need the following before running the performance tests:
1. EKP Client - installed an running
1. CoT server - installed and running
1. Jmeter - install version 5.4 or newer
1. Maven CLI - installed

## Test the CoT Performance

### Set up the Environment

1. Download this repository.
1. From the root of the downloaded repo, run the Maven command to compile the JAR for testing.

   `mvn package`

   This command creates the *ub-maven-test-1.0.0.jar* file in the *target/* directory.
1. Copy *ub-maven-test-1.0.0.jar* to the *lib* directory in your Jmeter installation.
1. Copy *ekm-java-provider-2.0.jar* from the CoT-client *.tar* file to the *lib* directory in your Jmeter installation.

### Testing

The following tests are provided:
1. RSA - sign

For the tests, first confirm that the default user has the password: "Password1!".
You can change the password in *RSASignTest.java* if necessary.

#### RSA - Sign
1. Open the *UBCoTtest.jmx* file in Jmeter.
1. Run the relevant test: Sign RSA.
1. You can use the 'User Defined Variables' in the following manner:
   - **createKeys** - When set to *true* new keys are created, and the number of **preactivated keys** must be equal or more than the number of threads running.  When set to *false* old keys are used, and the number of **keys** must be equal or more than the number of threads running.
   - **deleteKeys** - When set to *true* the keys used are deleted at the end of the test. When set to *false* the keys used are not deleted at the end of the test.

## Test the Legacy CoT Performance

### Set up the Environment

1. Download this repository.
1. From the root of the downloaded repo, run the Maven command to compile the JAR for testing.

    `mvn package`
	
    This command creates the *ub-maven-test-1.0.0.jar* file in the *target/* directory.
1. Copy *ub-maven-test-1.0.0.jar* to the *lib* directory in your Jmeter installation.
1. Copy *ekm-java-provider-2.0.jar* from the EKP client to the *lib* directory in your Jmeter installation.

### Testing

The following tests are provided:
1. RSA - sign

For the tests, first confirm that the default user has the password: "Password1!".
You can change the password in *RSASignTest.java* if necessary.

#### RSA - Sign
1. Open the *UBCoTtest.jmx* file in Jmeter.
1. Run the relevant test: Sign RSA. 
1. Use the 'User Defined Variables' in the following manner:
   - **createKeys** - When set to *true* new keys are created. When set to *false* old keys are used. 
   - **deleteKeys** - When set to *true* the keys used are deleted at the end of the test. When set to *false* the keys used are not deleted at the end of the test.
1. We recommend using:
    - 'createKeys' = 'true'
	- 'deleteKeys' = 'true'

