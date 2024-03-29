# Unbound integration with Microsoft DKE

** Important: This is a beta version not yet fully tested for production **

[Microsoft Double Key Encryption (DKE)](https://docs.microsoft.com/en-us/microsoft-365/compliance/double-key-encryption) provides strong protection for sensitive data in Office 365 applications by using encryption keys which are stored in an external keystore.  

This repo provides an implementation which uses Unbound Core KMS as the keystore.  
The keystore is a dotnet service application that implements DKE API and connects it to Unbound CORE KMS server (UKC).
The service is also provided installed in a Docker image and ready to be deployed as a container.

The following sections specify the steps required for configuring and using Unbound DKE keystore service. 
The instructions focus on deployment as a Docker container and use Azure as the service deployment platform. 
It is possible if needed to deploy the service on other platforms too, or on-prem. 

# Prerequisites

1. A Running Unbound UKC (Unbound CORE KMS) server with a user partition that has:
    1. An RSA key for encryption. [See here how to create it](https://www.unboundsecurity.com/docs/UKC/UKC_Interfaces/Content/Products/UKC-EKM/UKC_User_Guide/UG-If/uiSO/KeyTab.html#h2_1)  
       * The size of the key must be 2048 bits, and it must has *Decrypt* in its *Permitted operations*.
       * You'll need to use the **name of the key** for the DKE service configuration below. 
    2. A user assigned to role **User** (or equivalent custom role that has permissions for Decrypt with the encryption Key)  
       * The user must have a password (the default CORE 'user' does not have a password by default).
       * You'll need to use the **user name** and its **password** for the DKE service configuration below.

2. Access to [Microsoft Azure portal](https://portal.azure.com/), with the following permissions:
    * Create new app service.
    * Create new app registration.

2. Access to [Microsoft 365 compliance](https://compliance.microsoft.com/informationprotection?viewid=sensitivitylabels) with the following permissions:
    * Create new label.
    * Publish a label.  

3. Microsoft 365 with "Microsoft 365 E5" license.

# Azure app service configuration
The following sections guide you through the process of configuring and deploying Unbound DKE service as an Azure web app conteinerized service.

## Create a new app service in Azure portal
1. In a web browser, open the [Microsoft Azure portal](https://portal.azure.com/) -> App Services -> Create
2. Select your subscription and resource group and select the following options for the instance details:
    * Publish: Docker container 
    * Operation System: Linux
3. At the bottom of the page, select *Next: Docker* 
4. Fill with the following details:
    * Image Source: Docker Hub
    * Access type: Public
    * Image and tag: `unboundukc/ms-dke-service:latest`
5. Click on the *Review + create* button
6. Click on *Finish/confirm* button
7. Wait for the deployment to finish and then click *Go to resource*
8. On the sidebar click on *Configuration* and select the *Application setttings* tab at the top.  
   Then, for each of the settings below, click the *+ New application setting* button enter the setting's *Name* and the *Value* relevant for your configuration and click *Ok*. 
   When you're finished adding all the settings make sure you click the **Save** button at the top of the page.  
   The required settings are:
   * `UB_CORE_URL`  
     The URL for Unbound CORE KMS service, for example: `https://ukc-ep`.  
     This is used for sending requests to Unbound CORE service. The URL must be accessible from within the container.
   * `UB_PARTITION`  
     The name of the partition which stores the encryption key.
   * `UB_USER`
     The user name that will be used to authorize UKC requests with a Basic auth header.  
     As mentioned above, this must be a user with permissions for Decrypt with the encryption key.
   * `UB_USER_PASSWORD`  
     The user password which will be used for authentication.
   * `UB_CA_CERT_B64`  
     This setting is Optional. It allows to pass a Base64 representation of a CA certificate in pem format.    
     This may be used for TLS validation of a self-signed CA certificate.  
     In case this is not specified, the service container will run a script to try and fetch  
     the certificate from the Unbound CORE server at UB_CORE_URL and install it in the local OS certificate store.
     
## Integration with UKC
As mentioned above, configuring the service with UKC credentials by setting the
environment variables "UB_USER" and "UB_USER_PASSWORD" will let the service send requests to Unbound CORE server with a Basic authentication header.

If the service receives an “Authorization” header (supports both Bearer or Basic auth header) in the request, it will pass it as part of the UKC requests header.

Apart from that, the protection of the service should be provided in the network level, by using the service in a single shared network location only available when in the organization (e.g. only after VPN).

### Establishing TLS 
The DKE service communicates with Unbound CORE server using REST API over secure http (HTTPS) using TLS.  
TLS automatically validates the certificate of the server, in our case of the Unbound CORE server.  
The validation specifically uses two important stages:  
1. Validating that the host name which was specified in `UB_URL` matches one of the subject identifiers in the server certificate.  
   In case your server's public host name is different than its private one, you'll need to add the public address to the server certificate using 
   the [ekm_renew_server_certificate](https://www.unboundsecurity.com/docs/UKC/UKC_Interfaces/Content/Products/UKC-EKM/UKC_User_Guide/UG-If/AdminScripts/CertScripts.html#ekm_rene) utility. 
2. Validating the certificate issuing CA certificate.  
   In case the Unbound CORE server certificate was issued using the default self-signed CA, the CA certificate needs to be added to the OS trusted-ca's list.  
   With the default DKE Container, this will be automatically done using the `start` script. It can also be done by setting an environment variable `UB_CA_CERT_B64` as described above.
   
### Testing the deployment
At this stage, after you deployed the service, it will be a good idea to check that its functioning or troubleshoot if its not.  
1. You can click the *Log stream* entry under *Monitoring* on the left and look for a message indicating the service is running / or failure message.
2. You can open a browser and try browsing to get public key data from the service by using the service URL with the key name as its path, for example: `https://my-dke-service.azurewebsites.net\my-key`. The service URL is available under the `Overview` tab on the left.

## Register your app service
1. In a web browser, open the [Microsoft Azure portal](https://portal.azure.com/), and click on the top left menu button and then select *All Services* and search for 'App registrations' and select it.
2. Select New registration, and enter a meaningful name.
3. Select an account type from the options displayed.

    If you're using Microsoft Azure with a non-custom domain, such as *onmicrosoft.com*, select Accounts in this organizational directory only (Microsoft only - Single tenant).
4. At the bottom of the page, select Register to create the new App Registration.
5. In your new App Registration, in the left pane, under Manage, select Authentication.
6. Select Add a platform.
7. On the Configure platforms popup, select Web.
8. Under Redirect URIs, enter the URI of your double key encryption service(you can view it on the app service page). Enter the App Service URL, including both the hostname and domain.

    For example: https://unbound-dke.azurewebsites.net
	
    The URL you enter must match the hostname where your DKE service is deployed and the scheme must be https.
	
    Ensure the hostname exactly matches your App Service hostname. 
9. Under Implicit grant, select the ID tokens checkbox.
10. Select Configure button when done.
11. On the left pane, select Expose an API, then next to Application ID URI, select Set and Save.
12. Still on the Expose an API page, in the Scopes defined by this API area, select Add a scope. In the new scope:

     a. Define the scope name as user_impersonation.
     
     b. Select the administrators and users who can consent.
     
     c. Define any remaining values required.
     
     d. Select Add scope.
     
13. Still on the Expose an API page, in the Authorized client applications area, select Add a client application.
14. 
    In the new client application:
    
     a. Define the Client ID as d3590ed6-52b3-4102-aeff-aad2292ab01c. This value is the Microsoft Office client ID, and enables Office to obtain an access token for your key store.
     
     b. Under Authorized scopes, select the user_impersonation scope.
     
     c. Select Add client application.
	 
    Repeat these steps, but this time, define the client ID as c00e9d32-3c8d-4a7d-832b-029040e7db99. This value is the Azure Information Protection unified labeling client ID.

# Sensitivity labels configuration (Microsoft 365)
## Create a new label

1. Open [Microsoft 365 compliance](https://compliance.microsoft.com/informationprotection?viewid=sensitivitylabels) and click on the create new label button.

2. Fill the relevant details and click Next.

3. Mark the "Files & emails" checkbox and click Next.

4. Mark the "Encrypt files and emails" checkbox and click Next.

5. Chose the "Configure encryption settings radio button.

6. In the "Assign permissions now or let users decide?" dropdown Choose "assign permmision now".

7. In the "Allow offline access" choose never.

8. Under "Assign permissions to specific users and groups", click assign permmisions-> choose and fill the relevant data.

9. Mark the checkbox of “use double key encryption” and enter the app service url with the key you will use to encrypt/decrypt the files with .
 For example : https://unbound-dke.azurewebsites.net/<key_name> and click Next.

10. Click the Next until finished, then click Create label.

## Publish the label

1. Open [Microsoft 365 compliance](https://compliance.microsoft.com/informationprotection?viewid=sensitivitylabelpolicies) and click on the Publish label button.

2. Click choose sesitivity labels to publish and choose the the created label.

3. Click the Next button 5 times.

4. Fill the relevant data and click Next.

5. Click the submit button.

After creating the label it can be used in Office applications.  
Note that the label cannot be used for encryption for up to 24 hours after creating it (until Microsoft approves it). This means that you cannot save files using this label for up to 24 hours.

## Using sensitivity labels with Office applications

1. Install Microsoft Azure Information Protection from [here](https://www.microsoft.com/en-us/download/details.aspx?id=53018) 

2. Enable DKE for your client by downloading and installing the following registry key from [here](https://github.com/unboundsecurity/unbound-integration/blob/master/microsoft-365-dke/enable_dke_windows.reg)

3. Open an office app like word/excel…

4. Choose sensitivity->choose your created label->edit the document->save.

## For Developers > How to run the project with visual code dev container ?

1. Open 'devcontainer.json' file and edit the "containerEnv" section with the relevant data.

2. run "open in container" from visual studio.

3. open in browser http://localhost:8080/<key_name>
