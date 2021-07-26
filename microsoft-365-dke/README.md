# Unbound key-store service for Microsoft 365 DKE
https://docs.microsoft.com/en-us/microsoft-365/compliance/double-key-encryption

# Prerequisites

1. Make sure you can access [Microsoft Azure portal](https://portal.azure.com/), and have the following permmisions:
    
    a. Create new app service.

    b. Create new app registration.

2. Make sure you can access access [Microsoft 365 compliance](https://compliance.microsoft.com/informationprotection?viewid=sensitivitylabels) and you have the following permmisions:

    a. Create new label.
    
    b. Publish a label.  

3. Make sure you have Microsoft 365 E5 license.

4. From UKC you need to have the following:

    a. partition with name <partition_name>
    
    b. RSA key, size 256 name <key_name>

    c. Ephemeral client template name <client_template_name> and the <client_template_activation_code>

   NOTE: we will use the <key_name> and the <partition_name> in the next stages.
   
# Create new app service in azure portal

1. Go to [Microsoft Azure portal](https://portal.azure.com/) -> App Services -> Create
2. Select your subscription and resource group and define the following instance details:

    Publish ->Docker container 

    Operation System -> Linux

3. At the bottom of the page, select Next: Docker 

4. Fill with the following details:

     Image Source -> Docker Hub

     Access type -> public

     Image and tag -> unboundukc/ms-dke-service:latest

 5. Click on Review + create button.  

 6. Wait for the deployment to finish and then click "Go to resource".  

 7. On the sidebar click on Configuration -> Application setttings -> "Advanced edit" button -> add the following application settings to the json :
 
        a. EP_HOST_NAME - EP server name.

        b. UKC_PARTITION - UKC partition name.

        c. UKC_PASSWORD - The password used to login with 'so' user for the selected partition.

        d. UKC_SERVER_IP - UKC server ip. 

        e. UKC_SO_PASSWORD - UKC so password.

        f. CLIENT_TEMPLATE_NAME - Ephemeral client template name.

        g. CLIENT_TEMPLATE_ACTIVATION_CODE - Ephemeral client template activation code.

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
            "name": "UKC_PASSWORD",
            "value": "Unbound1!",
            "slotSetting": false
        },
        {
            "name": "UKC_SERVER_IP",
            "value": "54.174.121.27",
            "slotSetting": false
        },
        {
            "name": "UKC_SO_PASSWORD",
            "value": "Unbound1!",
            "slotSetting": false
        },
        {
            "name": "CLIENT_TEMPLATE_NAME",
            "value": "template1",
            "slotSetting": false
        },
        {
            "name": "CLIENT_TEMPLATE_ACTIVATION_CODE",
            "value": "2595287639032430",
            "slotSetting": false
        }

        
     

 
 
    Alterntavly, you can add them manually by clicking the "New application settings" button.

    NOTE: Click save at top of the page when you done.

# Register your app service

1. In your browser, open the [Microsoft Azure portal](https://portal.azure.com/), and go to All Services > Identity > App Registrations.

2. Select New registration, and enter a meaningful name.

3. Select an account type from the options displayed.

If you're using Microsoft Azure with a non-custom domain, such as onmicrosoft.com, select Accounts in this organizational directory only (Microsoft only - Single tenant).

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

In the new client application:

 a. Define the Client ID as d3590ed6-52b3-4102-aeff-aad2292ab01c. This value is the Microsoft Office client ID, and enables Office to obtain an access token for your key store.

 b. Under Authorized scopes, select the user_impersonation scope.

 c. Select Add client application.

Repeat these steps, but this time, define the client ID as c00e9d32-3c8d-4a7d-832b-029040e7db99. This value is the Azure Information Protection unified labeling client ID.

# Create new label

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

# Publish the label

1. Open [Microsoft 365 compliance](https://compliance.microsoft.com/informationprotection?viewid=sensitivitylabelpolicies) and click on the Publish label button.

2. Click choose sesitivity labels to publish and choose the the created label.

3. Click the Next button 5 times.

4. Fill the relevant data and click Next.

5. Click the submit button.
# How to use the created label with office app?

1. Install Microsoft Azure Information Protection from [here](https://www.microsoft.com/en-us/download/details.aspx?id=53018) 

2. Open an office app like word/excel…

3. Choose sensitivity->choose your created label->edit the document->save.
