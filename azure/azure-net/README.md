# Azure Storage Client .NET 
Unbound Azure Storage .Net Client implementation


## Prerequisites
* UKC client is installed

## Usage

### Blob encryption

```c#

namespace BlobGettingStarted
{
    using System;
    using System.IO;
    using Microsoft.Azure.KeyVault;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    /// <summary>
    /// Demonstrates how to use encryption with the Azure Blob service.
    /// </summary>
    public class Program
    {
        const string DemoContainer = "democontainer";

        static void Main(string[] args)
        {
            Console.WriteLine("Blob encryption sample");

            // Retrieve storage account information from connection string
            // How to create a storage connection string - https://azure.microsoft.com/en-us/documentation/articles/storage-configure-connection-string/
            CloudStorageAccount storageAccount = EncryptionShared.Utility.CreateStorageAccountFromConnectionString();

            CloudBlobClient client = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = client.GetContainerReference(DemoContainer + Guid.NewGuid().ToString("N"));

            try
            {
                container.Create();
                int size = 5 * 1024;
                byte[] buffer = new byte[size];

                Random rand = new Random();
                rand.NextBytes(buffer);

                CloudBlockBlob blob = container.GetBlockBlobReference("blockblob");               

                // Create the IKey used for encryption.
                EKMIkey key = new EKMIkey("key-name");

                // Create the encryption policy to be used for upload.
                BlobEncryptionPolicy uploadPolicy = new BlobEncryptionPolicy(key, null);

                // Set the encryption policy on the request options.
                BlobRequestOptions uploadOptions = new BlobRequestOptions() { EncryptionPolicy = uploadPolicy };

                Console.WriteLine("Uploading the encrypted blob.");

                // Upload the encrypted contents to the blob.
                using (MemoryStream stream = new MemoryStream(buffer))
                {
                    blob.UploadFromStream(stream, size, null, uploadOptions, null);
                }

                // Download the encrypted blob.
                // For downloads, a resolver can be set up that will help pick the key based on the key id.
                EKMIKeyResolver resolver = new EKMIKeyResolver();            

                BlobEncryptionPolicy downloadPolicy = new BlobEncryptionPolicy(null, resolver);

                // Set the decryption policy on the request options.
                BlobRequestOptions downloadOptions = new BlobRequestOptions() { EncryptionPolicy = downloadPolicy };

                Console.WriteLine("Downloading the encrypted blob.");

                // Download and decrypt the encrypted contents from the blob.
                using (MemoryStream outputStream = new MemoryStream())
                {
                    blob.DownloadToStream(outputStream, null, downloadOptions, null);
                }

                Console.WriteLine("Press enter key to exit"); 
                Console.ReadLine(); 
            }
            finally
            {
                container.DeleteIfExists();
            }
        }
    }
}

```

### Queue encryption 

```c#

namespace QueueGettingStarted
{
    using System;
    using Microsoft.Azure.KeyVault;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Queue;

    /// <summary>
    /// Demonstrates how to use encryption with the Azure Queue service.
    /// </summary>
    public class Program
    {
        const string DemoQueue = "demoqueue";

        static void Main(string[] args)
        {
            Console.WriteLine("Queue encryption sample");

            // Retrieve storage account information from connection string
            // How to create a storage connection string - https://azure.microsoft.com/en-us/documentation/articles/storage-configure-connection-string/
            CloudStorageAccount storageAccount = EncryptionShared.Utility.CreateStorageAccountFromConnectionString();
            CloudQueueClient client = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = client.GetQueueReference(DemoQueue + Guid.NewGuid().ToString("N"));

            try
            {
                queue.Create();

                // Create the IKey used for encryption.
                EKMIkey key = new EKMIkey("key-name");     

                // Create the encryption policy to be used for insert and update.
                QueueEncryptionPolicy insertPolicy = new QueueEncryptionPolicy(key, null);

                // Set the encryption policy on the request options.
                QueueRequestOptions insertOptions = new QueueRequestOptions() { EncryptionPolicy = insertPolicy };

                string messageStr = Guid.NewGuid().ToString();
                CloudQueueMessage message = new CloudQueueMessage(messageStr);

                // Add message
                Console.WriteLine("Inserting the encrypted message.");
                queue.AddMessage(message, null, null, insertOptions, null);

                // For retrieves, a resolver can be set up that will help pick the key based on the key id.
                EKMIKeyResolver resolver = new EKMIKeyResolver();     
              

                QueueEncryptionPolicy retrPolicy = new QueueEncryptionPolicy(null, resolver);
                QueueRequestOptions retrieveOptions = new QueueRequestOptions() { EncryptionPolicy = retrPolicy };

                // Retrieve message
                Console.WriteLine("Retrieving the encrypted message.");
                CloudQueueMessage retrMessage = queue.GetMessage(null, retrieveOptions, null);

                // Update message
                Console.WriteLine("Updating the encrypted message.");
                string updatedMessage = Guid.NewGuid().ToString("N");
                retrMessage.SetMessageContent(updatedMessage);
                queue.UpdateMessage(retrMessage, TimeSpan.FromSeconds(0), MessageUpdateFields.Content | MessageUpdateFields.Visibility, insertOptions, null);

                // Retrieve updated message
                Console.WriteLine("Retrieving the updated encrypted message.");
                retrMessage = queue.GetMessage(null, retrieveOptions, null);

                Console.WriteLine("Press enter key to exit");
                Console.ReadLine();
            }
            finally
            {
                queue.DeleteIfExists();
            }
        }
    }
}


```
