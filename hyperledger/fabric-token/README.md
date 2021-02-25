# Using Unbound Key Control (UKC) with  Hyperledger Fabric Client
This demo shows how to use Unbound Key Control ("UKC") for secure distributed key storage as the crypto-provider for Hyperledger fabric client with Java.

## Solution Overview

### Unbound Java Security Provider
The Unbound solution uses [Unbound's Java Security Provider](https://www.unboundtech.com/docs/UKC/UKC_Developers_Guide/HTML/Content/Products/UKC-EKM/UKC_Developers_Guide/DevelopingInJava/Developing_in_Java.htm#_Toc509128625). 
The Fabric SDK is configured to use Unbound's Java Security Provider as the crypto provider. In this way, key generation and signing are transparently redirected to UKC.

This solution also has the advantage of requiring minimum changes to existing code.

The demo script (`./run_with_unbound.sh`) connects Unbound's security provider to Fabric SDK by setting a Java system attribute:

```
java -Dorg.hyperledger.fabric.sdk.security_provider_class_name=com.dyadicsec.provider.DYCryptoProvider 
```
### Admin Key Material Migration
The original POC files included the Fabric network *admin* certificate and private key material to be used for signing when enrolling a new user. The key material is stored in a local file.

Unbound's provider can sign only with keys stored in its own secure, distributed key storage. Therefore, Unbound created [custom code](https://github.com/unbound-tech/fabric-token-poc2-master/blob/master/src/main/java/org/example/unbound/UkcUserContextAdapter.java#L75) that reads each *admin* key from its file, imports it into UKC, and removes the private key material from the local file.

## POC Code

### Modified Repository
Unbound copied and modified the POC repository provided by Custodigit. The modified repository has been shared with Vadim at:
```
https://github.com/unbound-tech/fabric-token-poc2-master
```

### Files on the POC VM
All files required for this demo are found in the POC VM under the folder:

```
/root/unbound/fabric-token
```

## Steps
1. **Monitor the UKC log**  
Before we begin, lets open a separate terminal to monitor the UKC logs on the remote UKC EP (Entry point) server. 
    This allows us to learn about the cryptographic operations that are forwarded to the remote UKC server by Unbound's java security provider.
    
    To start monitoring the log, run this command in a new terminal window:
    ```
    ssh root@193.246.44.100 -p 22022 "/root/unbound/scripts/monitor_ukc_log.sh"
    ```
2. **Open the demo terminal**  
  For the rest of this demo, use a new shell session on the POC VM:
    ```
    ssh root@193.246.44.100 -p 22022
    cd unbound/fabric-token/
    ```
    The password is: !5eLfdwk
3. **Reset**   
Delete any previously generated keys from UKC and local files and copy the original admin key files to the `./users` folder.
    ```
    ./reset.sh
    ```
4. **Migrate admin keys to UKC**   
Import the admin keys into UKC so that Unbound's provider can use them.
The import process also deletes the private key material from the local files. This can later be undone with the `./reset.sh` script.
  
   To migrate admin keys run:
    ```
    ./migrate_admin_keys_to_ukc.sh
    ```
5. **UKC configuration**   
    UKC uses partitions as segregated containers for keys.
    You can see the available UKC partitions by running:
    
    ``` 
    ucl partition list 
    ```
    The output shows that there is 1 partition called *fabric*:
    ![](https://i.imgur.com/DkrqQqt.png)
    
    This partition is used by Unbound's security provider for key storage.
    
    Each partition can have many users. By default a partition has one **SO** with administration permissions and one **User** with key operation permissions. Our provider uses the default user for crypto operations.

6. **Review the imported keys**  
Review the keys imported to UKC by running:
   ```
   ucl list
   ```
   You see 4 objects: two ECC keys for the admins and their associated certificates. These are the keys that were imported in the step 4.
   
   ![](https://i.imgur.com/JLTYuOj.png)
7. **Run the demo**   
    Run the demo with: 
    ```
    ./run_with_unbound.sh
    ```
    While the program is running you can switch to the UKC log terminal to see the activity that is taking place.

6. **Review the created keys**  
After the demo has finished, you can review the user keys that were created by UKC by running:
    ```
    ucl list
    ```
    In the output, you can notice that there are two new objects created for the new user:
    - A private ECC key
    - The matching certificate
 
    ![](https://i.imgur.com/OjRTkgL.png)
    
   You can also switch to the UKC monitor terminal and search for the *CreateKeyPair* and *Sign* operations. 
   
   For example:
   
    ![](https://i.imgur.com/mtl3C0b.png)
    
    ![](https://i.imgur.com/I3hijzD.png)

   Notice that the key UID's in the log match the key UID's in the object list.
   
### Web UI
   Another way to inspect UKC keys is by using the web console, which can be accessed at:
    
[https://cdukc.unboundtech.com/keys](https://cdukc.unboundtech.com/keys)
    
To login use the following credentials:
- User: so
- Password: Password1!
- Partition: fabric

The screen *Keys and Certificates* shows all keys and certifiates that are stored in the UKC.
