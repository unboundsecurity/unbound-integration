# ub-azure-storage-java
Azure Storage Java Client implementation for Unbound-tech EKM.

## Prerequisites
* EKM client is installed

* Ant (Version >= 1.93) is installed. 
To install on RH/Centos, use `yum install ant`.
Refer to https://ant.apache.org/bindownload.cgi for further details.

* The `lib:org.apache.ivy.ant` task is installed. 
To install on RH/Centos, use `yum install ivy`.
Refer to http://ant.apache.org/ivy/history/2.2.0/ant.html for further details.

## Build
Copy the Unbound Java Security Provider (`ekm-java-provider-2.0.jar`) file from the EKM Client distribution to the root directory of the project and run `ant`.

## Usage
Include The Unbound Java Security Provider (`ekm-java-provider-2.0.jar`) file and the `dist/ub-azure.jar` in your project.

### Assumptions
In the following code we assume that:
- EKM cluster contains partition named `"part1"`
- EKM partition "part1" contains RSA key named `"key-name"`

### Blob encryption
```java

import com.dyadicsec.provider.DYCryptoProvider;
import com.microsoft.azure.storage.CloudStorageAccount;
import com.microsoft.azure.storage.StorageException;
import com.microsoft.azure.storage.blob.*;
import com.microsoft.azure.storage.util.Utility;
import com.unboundtech.DyLocalResolver;
import com.unboundtech.DyRSAKey;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.net.URISyntaxException;
import java.security.GeneralSecurityException;
import java.security.Provider;
import java.security.Security;
import java.util.Random;
import java.util.UUID;

/**
 * Demonstrates how to use encryption with the Azure Blob service.
 */
public class BlobGettingStarted {

    public static void main(String[] args) throws GeneralSecurityException,
            URISyntaxException, StorageException,
            IOException {
        Utility.printSampleStartInfo("BlobBasicsEncryption");

        // Retrieve storage account information from connection string
        // How to create a storage connection string -
        // https://azure.microsoft.com/en-us/documentation/articles/storage-configure-connection-string/
        CloudStorageAccount account = CloudStorageAccount
                .parse(Utility.storageConnectionString);
        CloudBlobClient blobClient = account.createCloudBlobClient();

        // Get a reference to a container
        // The container name must be lower case
        // Append a random UUID to the end of the container name so that
        // this sample can be run more than once in quick succession.
        CloudBlobContainer container = blobClient
                .getContainerReference("blobencryptioncontainer"
                        + UUID.randomUUID().toString().replace("-", ""));
        //

        try {
            // Create the container if it does not exist
            container.createIfNotExists();

            int size = 5 * 1024;
            byte[] buffer = new byte[size];

            Random rand = new Random();
            rand.nextBytes(buffer);

            CloudBlockBlob blob = container.getBlockBlobReference("blockBlob");

            //init dyadic provider
            String partition = "part1";
            Provider dyadicProvider = new DYCryptoProvider(partition);
            Security.addProvider(dyadicProvider);

            // get key
            DyRSAKey key = new DyRSAKey("key-name");
            key.load();

            // Create the encryption policy to be used for upload.
            BlobEncryptionPolicy uploadPolicy = new BlobEncryptionPolicy(key, null);

            // Set the encryption policy on the request options.
            BlobRequestOptions uploadOptions = new BlobRequestOptions();
            uploadOptions.setEncryptionPolicy(uploadPolicy);

            System.out.println("Uploading the encrypted blob.");

            // Upload the encrypted contents to the blob.
            ByteArrayInputStream inputStream = new ByteArrayInputStream(buffer);
            blob.upload(inputStream, size, null, uploadOptions, null);

            DyLocalResolver resolver = new DyLocalResolver();

            BlobEncryptionPolicy downloadPolicy = new BlobEncryptionPolicy(
                    null, resolver);

            // Set the decryption policy on the request options.
            BlobRequestOptions downloadOptions = new BlobRequestOptions();
            downloadOptions.setEncryptionPolicy(downloadPolicy);

            System.out.println("Downloading the encrypted blob.");

            // Download and decrypt the encrypted contents from the blob.
            ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
            blob.download(outputStream, null, downloadOptions, null);
        } finally {
            // Delete the container
            container.deleteIfExists();
            Utility.printSampleCompleteInfo("BlobBasicsEncryption");
        }
    }
}
```


### Queue encryption 

```java

package com.microsoft.azure.storage.encryption.queue.gettingstarted;

import java.net.URISyntaxException;
import java.security.*;
import java.util.EnumSet;
import java.util.UUID;

import com.dyadicsec.provider.DYCryptoProvider;

import com.microsoft.azure.storage.CloudStorageAccount;
import com.microsoft.azure.storage.StorageException;

import com.microsoft.azure.storage.queue.CloudQueue;
import com.microsoft.azure.storage.queue.CloudQueueClient;
import com.microsoft.azure.storage.queue.CloudQueueMessage;
import com.microsoft.azure.storage.queue.MessageUpdateFields;
import com.microsoft.azure.storage.queue.QueueEncryptionPolicy;
import com.microsoft.azure.storage.queue.QueueRequestOptions;

import com.microsoft.azure.storage.util.Utility;
import com.unboundtech.DyLocalResolver;
import com.unboundtech.DyRSAKey;

public class QueueGettingStarted {

    public static void main(String[] args) throws InvalidKeyException,
            URISyntaxException, StorageException {

        Utility.printSampleStartInfo("QueueBasicsEncryption");

        // Retrieve storage account information from connection string
        // How to create a storage connection string -
        // https://azure.microsoft.com/en-us/documentation/articles/storage-configure-connection-string/
        CloudStorageAccount account = CloudStorageAccount
                .parse(Utility.storageConnectionString);
        CloudQueueClient client = account.createCloudQueueClient();
        CloudQueue queue = client.getQueueReference("demoqueue"
                + UUID.randomUUID().toString().replace("-", ""));

        try {
            queue.createIfNotExists();

            //init dyadic provider
            String partition = "part1";
            Provider dyadicProvider = new DYCryptoProvider(partition);
            Security.addProvider(dyadicProvider);

            // get key
            DyRSAKey key = new DyRSAKey("key-name");
            key.load();

            // Create the encryption policy to be used for insert and update.
            QueueEncryptionPolicy insertPolicy = new QueueEncryptionPolicy(key,
                    null);

            // Set the encryption policy on the request options.
            QueueRequestOptions insertOptions = new QueueRequestOptions();
            insertOptions.setEncryptionPolicy(insertPolicy);

            String messageStr = UUID.randomUUID().toString();
            CloudQueueMessage message = new CloudQueueMessage(messageStr);

            // Add message
            System.out.println("Inserting the encrypted message.");
            queue.addMessage(message, 0, 0, insertOptions, null);

            // For retrieves, a resolver can be set up that will help pick the
            // key based on the key id.
            DyLocalResolver resolver = new DyLocalResolver();

            QueueEncryptionPolicy retrPolicy = new QueueEncryptionPolicy(null,
                    resolver);
            QueueRequestOptions retrieveOptions = new QueueRequestOptions();
            retrieveOptions.setEncryptionPolicy(retrPolicy);

            // Retrieve message
            System.out.println("Retrieving the encrypted message.");
            CloudQueueMessage retrMessage = queue.retrieveMessage(1,
                    retrieveOptions, null);

            // Update message
            System.out.println("Updating the encrypted message.");
            String updatedMessage = UUID.randomUUID().toString();
            retrMessage.setMessageContent(updatedMessage);
            queue.updateMessage(retrMessage, 0, EnumSet
                    .of(MessageUpdateFields.CONTENT,
                            MessageUpdateFields.VISIBILITY), insertOptions,
                    null);

            // Retrieve updated message
            System.out.println("Retrieving the updated encrypted message.");
            retrMessage = queue.retrieveMessage(1, retrieveOptions, null);
        } catch (Throwable t) {
            Utility.printException(t);
        } finally {
            queue.deleteIfExists();
            Utility.printSampleCompleteInfo("QueueBasicsEncryption");
        }
    }

}

```
