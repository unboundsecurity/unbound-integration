# Unbound Key Control Performance Testing Using Jmeter

This repository uses [Apache Jmeter](https://jmeter.apache.org/) to test the performance of UKC.

Refer to [Developing in Java](https://www.unboundtech.com/docs/UKC/UKC_Developers_Guide/HTML/Content/Products/UKC-EKM/UKC_Developers_Guide/DevelopingInJava/Developing_in_Java.htm) for more information about configuring the Java environment.

## Prerequisites

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

### Configuration
1. Create a key for testing. For example:

    `ucl generate -t AES -p <PART_NAME> --name aes-256-key --purpose E`
    
    Note: The test uses the key named *aes-256-key*.
1. Confirm that the default user does not have a password. (Note: A blank password is the default)
1. Open the *Ubtest.jmx* file in Jmeter.

### Testing
The **Encrypt** and **Decrypt** tests are now visible in the test plan and can be run.

The tests use an AES-GCM key with 32-bit data.
