# Unbound integration with Microsoft DKE

** Important: This is an alpha version not yet fully tested for production **

[Microsoft Double Key Encryption (DKE)](https://docs.microsoft.com/en-us/microsoft-365/compliance/double-key-encryption) provides strong protection for sensitive data in Office 365 applications by using encryption keys which are stored in an external keystore.  

This repo provides an implementation which uses Unbound Core KMS as the keystore.  
The keystore is available as a docker image that implements DKE API and connects it to Unbound CORE KMS server (UKC).

The following sections specify the steps required for configuring and using Unbound DKE keystore service. 
These instructions use Azure as the service deployment platform. It is possible if needed to deploy the service on other platforms too, or on-prem. 

# Prerequisites

1. A Running Unbound UKC (Unbound CORE KMS) server with a user partition that has:
    1. An RSA key for encryption. [See here how to create it](https://www.unboundsecurity.com/docs/UKC/UKC_Interfaces/Content/Products/UKC-EKM/UKC_User_Guide/UG-If/uiSO/KeyTab.html#h2_1)  
       The size of the key must be 2048 bits, and it must has *Decrypt* in its *Permitted operations*
       You'll need to use the **name of the key** for the DKE service configuration below. 
    3. An Ephemeral Client Template. [See here how to create one](https://www.unboundsecurity.com/docs/UKC/UKC_Interfaces/Content/Products/UKC-EKM/UKC_User_Guide/UG-If/uiSO/ClientsTab.html#Multi-us).  
       You'll need the **name of the client and its *activation code*** for the DKE service configuration below.
    4. A user assigned to role **User** (or equivalent custom role that has permissions for Decrypt with the encryption Key)


2. Access to [Microsoft Azure portal](https://portal.azure.com/), with the following permissions:
    * Create new app service.
    * Create new app registration.

2. Access to [Microsoft 365 compliance](https://compliance.microsoft.com/informationprotection?viewid=sensitivitylabels) with the following permissions:
    * Create new label.
    * Publish a label.  

3. Microsoft 365 with "Microsoft 365 E5" license.

# Azure app service configuration
The following sections guide you through the process of configuring an publishing Unbound DKE service as an Azure web app service

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
8. On the sidebar click on *Configuration* -> *Application setttings* -> *Advanced edit* button -> add the following application settings to the json and then click *Ok* and click *Save* button at top of the page:
 
        a. EP_HOST_NAME - EP server name.

        b. UKC_PARTITION - UKC partition name.

        c. UKC_SERVER_IP - UKC server ip. 

        d. UKC_USER_NAME - the user name that will be sent to the UKC requests with a Basic auth header.
        
        e. UKC_PASSWORD - the password that will be sent to the UKC requests with a Basic auth header.

   For example:

        {
            "name": "EP_HOST_NAME",
            "value": "ep1",
            "slotSetting": false
        },
        {
            "name": "UKC_PARTITION",
            "value": "test",
            "slotSetting": false
        },
        {
            "name": "UKC_SERVER_IP",
            "value": "54.174.121.27",
            "slotSetting": false
        },
        {
            "name": "UKC_USER_NAME",
            "value": "dke_user",
            "slotSetting": false
        },
        {
            "name": "UKC_PASSWORD",
            "value": "********",
            "slotSetting": false
        },

    ```
 
    Alternatively, you can add them manually by clicking the "New application settings" button.


## Integration with UKC

As mentioned above, configuring the service with UKC credentials by setting the
environment variables "UKC_USER_NAME" and "UKC_PASSWORD" will let the service send UKC requests with a Basic auth header.

If the service receives an “Authorization” header (supports both Bearer or Basic auth header) in the request, it will pass it as part of the UKC requests header.

Apart from that, the protection of the service should be provided in the network level, by using the service in a single shared network location only available when in the organization (e.g. only after VPN).
 

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

10. Click the Next button 3 more times and then click Create label.

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
