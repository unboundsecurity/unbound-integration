# ub-aws-cyok-java
This sample code demonstrates protecting data using client-side UKC encryption. Client-side encryption is the act of encrypting data before sending it to Amazon S3.

This code is based on the code found in the [AWS help](https://docs.aws.amazon.com/AmazonS3/latest/dev/UsingClientSideEncryption.html). All the necessary components to use UKC were added. 

## Prerequisites
1. UKC client is installed.
1. The Unbound Java Security Provider (ekm-java-provider-2.0.jar) file must be included in your project.
1. There must be a partition in UKC that uses the default user and password.

## Symmetric Master Key

```java

import com.amazonaws.AmazonServiceException;
import com.amazonaws.SdkClientException;
import com.amazonaws.auth.AWSStaticCredentialsProvider;
import com.amazonaws.auth.BasicAWSCredentials;
import com.amazonaws.regions.Regions;
import com.amazonaws.services.kms.AWSKMSClient;
import com.amazonaws.services.s3.AmazonS3;
import com.amazonaws.services.s3.AmazonS3EncryptionClientBuilder;
import com.amazonaws.services.s3.model.*;

import java.io.ByteArrayInputStream;

public class S3ClientSideEncryptionSymMasterKey {

    public static void main(String[] args) throws Exception {
        String clientRegion = "CLIENT_REGION";
        String bucketName = "BUCKET_NAME";
        String objectKeyName = "OBJECT_NAME";
        String accessKey = "ACCESS_KEY";
        String secretKey = "SECRET_KEY";
        String ekmPartitionName = "PARTITION";
        String masterKeyName = "MASTER_KEY";

        BasicAWSCredentials awsCredentials = new BasicAWSCredentials(accessKey,secretKey);

        try {
            // Create the Amazon S3 encryption client.
            EncryptionMaterials encryptionMaterials = new
                    KMSEncryptionMaterials(masterKeyName);
            AmazonS3 s3EncryptionClient = AmazonS3EncryptionClientBuilder.standard()
                    .withCredentials(new AWSStaticCredentialsProvider(awsCredentials))
                    .withKmsClient(new UbAWSKMSClient(ekmPartitionName,new UbAWSKMSGCMAlgorithm()))
                    .withEncryptionMaterials(new StaticEncryptionMaterialsProvider(encryptionMaterials))
                    .withRegion(clientRegion)
                    .build();

            // Upload a new object. The encryption client automatically encrypts it.
            byte[] plaintext = "S3 Object Encrypted Using Client-Side Symmetric Master Key.".getBytes();
            s3EncryptionClient.putObject(new PutObjectRequest(bucketName,
                    objectKeyName,
                    new ByteArrayInputStream(plaintext),
                    new ObjectMetadata()));

            // Download and decrypt the object.
            S3Object downloadedObject = s3EncryptionClient.getObject(bucketName, objectKeyName);
            byte[] decrypted = com.amazonaws.util.IOUtils.toByteArray(downloadedObject.getObjectContent());

            // Verify that the data that you downloaded is the same as the original data.
            System.out.println("Plaintext: " + new String(plaintext));
            System.out.println("Decrypted text: " + new String(decrypted));
        } catch (AmazonServiceException e) {
            // The call was transmitted successfully, but Amazon S3 couldn't process
            // it, so it returned an error response.
            e.printStackTrace();
        } catch (SdkClientException e) {
            // Amazon S3 couldn't be contacted for a response, or the client
            // couldn't parse the response from Amazon S3.
            e.printStackTrace();
        }
    }
}
```

## Asymmetric Master Key

```java

import com.amazonaws.AmazonServiceException;
import com.amazonaws.SdkClientException;
import com.amazonaws.auth.AWSStaticCredentialsProvider;
import com.amazonaws.auth.BasicAWSCredentials;
import com.amazonaws.regions.Regions;
import com.amazonaws.services.s3.AmazonS3;
import com.amazonaws.services.s3.AmazonS3EncryptionClientBuilder;
import com.amazonaws.services.s3.model.*;

import java.io.ByteArrayInputStream;

public class S3ClientSideEncryptionAsymmetricMasterKey {

    public static void main(String[] args) throws Exception {
        String clientRegion = "CLIENT_REGION";
        String bucketName = "BUCKET_NAME";
        String objectKeyName = "OBJECT_NAME";
        String accessKey = "ACCESS_KEY";
        String secretKey = "SECRET_KEY";
        String ekmPartitionName = "PARTITION";
        String masterKeyName = "MASTER_KEY";

        BasicAWSCredentials awsCredentials = new BasicAWSCredentials(accessKey,secretKey);

        try {
            // Create the Amazon S3 encryption client.
            EncryptionMaterials encryptionMaterials = new
                    KMSEncryptionMaterials(masterKeyName);
            AmazonS3 s3EncryptionClient = AmazonS3EncryptionClientBuilder.standard()
                    .withCredentials(new AWSStaticCredentialsProvider(awsCredentials))
                    .withKmsClient(new UbAWSKMSClient(ekmPartitionName,new UbAWSKMSRSAAlgorithm()))
                    .withEncryptionMaterials(new StaticEncryptionMaterialsProvider(encryptionMaterials))
                    .withRegion(clientRegion)
                    .build();

            // Upload a new object. The encryption client automatically encrypts it.
            byte[] plaintext = "S3 Object Encrypted Using Client-Side Symmetric Master Key.".getBytes();
            s3EncryptionClient.putObject(new PutObjectRequest(bucketName,
                    objectKeyName,
                    new ByteArrayInputStream(plaintext),
                    new ObjectMetadata()));

            // Download and decrypt the object.
            S3Object downloadedObject = s3EncryptionClient.getObject(bucketName, objectKeyName);
            byte[] decrypted = com.amazonaws.util.IOUtils.toByteArray(downloadedObject.getObjectContent());

            // Verify that the data that you downloaded is the same as the original data.
            System.out.println("Plaintext: " + new String(plaintext));
            System.out.println("Decrypted text: " + new String(decrypted));
        } catch (AmazonServiceException e) {
            // The call was transmitted successfully, but Amazon S3 couldn't process
            // it, so it returned an error response.
            e.printStackTrace();
        } catch (SdkClientException e) {
            // Amazon S3 couldn't be contacted for a response, or the client
            // couldn't parse the response from Amazon S3.
            e.printStackTrace();
        }
    }

}
```
