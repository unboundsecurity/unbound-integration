# Unbound key-store service for Microsoft 365 DKE
https://docs.microsoft.com/en-us/microsoft-365/compliance/double-key-encryption

# Prerequisites

1. Make sure you have permissions to access Microsoft azure portal, and also can click the create new app service and create new app registration buttons. link: https://portal.azure.com/ 

2. Make sure you have permission to access Microsoft 365 compliance and you can click the create new label button : https://compliance.microsoft.com/informationprotection?viewid=sensitivitylabels 

3. make sure you have Microsoft 365 E5 license.

4. From UKC server:
 a. create a partition "test"
 b. create an RSA key in partition "test" with the following command : ucl generate -t RSA -n <key_name> -p test.

# Create new app service in azure portal

1. Goto azure portal -> App Services -> Create
2. Select your subscription and resource group and define the following instance details:
Runtime stack -> Net Core 3.1
Operation System -> Linux
3. At the bottom of the page, select Review + create, and then select Add.
4. goto Configuration -> general setttings -> under Startup Command enter "/home/site/wwwroot/data/start.sh"

# Register your app service

1. In your browser, open the Microsoft Azure portal, and go to All Services > Identity > App Registrations.

2. Select New registration, and enter a meaningful name.

3. Select an account type from the options displayed.

If you're using Microsoft Azure with a non-custom domain, such as onmicrosoft.com, select Accounts in this organizational directory only (Microsoft only - Single tenant).

4. At the bottom of the page, select Register to create the new App Registration.

5. In your new App Registration, in the left pane, under Manage, select Authentication.

6. Select Add a platform.

7. On the Configure platforms popup, select Web.

8. Under Redirect URIs, enter the URI of your double key encryption service(you can view it on the app service page). Enter the App Service URL, including both the hostname and domain.

For example: https://unbound-dke.azurewebsites.net

The URL you enter must match the hostname where your DKE service is deployed.
 the scheme must be https.
Ensure the hostname exactly matches your App Service hostname. 

9. Under Implicit grant, select the ID tokens checkbox.

10. Select Save to save your changes.

11. On the left pane, select Expose an API, then next to Application ID URI, select Set.

12. Still on the Expose an API page, in the Scopes defined by this API area, select Add a scope. In the new scope:

 a. Define the scope name as user_impersonation.

 b. Select the administrators and users who can consent.

 c. Define any remaining values required.

 d. Select Add scope.

 e. Select Save at the top to save your changes.

13. Still on the Expose an API page, in the Authorized client applications area, select Add a client application.

In the new client application:

 a. Define the Client ID as d3590ed6-52b3-4102-aeff-aad2292ab01c. This value is the Microsoft Office client ID, and enables Office to obtain an access token for your key store.

 b. Under Authorized scopes, select the user_impersonation scope.

 c. Select Add application.

 d. Select Save at the top to save your changes.

Repeat these steps, but this time, define the client ID as c00e9d32-3c8d-4a7d-832b-029040e7db99. This value is the Azure Information Protection unified labeling client ID.

# Build the project

1. goto : /src/customer-key-store/Models/TestStore.cs Line 17,18
replace ukcKeyName="<key_name>";
        ukcKeyUid="<key_uid>";
3. open appsettings.json file
 a. Locate the ValidIssuers setting and replace <tenant_ID> with your tenant ID. You can locate your tenant ID by going to the Azure portal and viewing the tenant properties. for example  "https://sts.windows.net/<tenant_ID>/"
 b. Locate the JwtAudience setting and replace  <yourhostname> with the hostname of the machine where the DKE service will run
 (you can view it on youre created app service page). for example https://unbound-dke-container.azurewebsites.net/
 c. locate the AuthorizedEmailAddress setting. 
 Add the email address or addresses that you want to authorize. Separate multiple email addresses with double quotes and commas.

# publish the program to the app service 
1. go to the program src folder and run: Dotnet publish

2. zip the created “publish” folder. For example: “publish.zip”

3. To create connection to your app service run : az webapp create-remote-connection --subscription '<subscription_id>' --resource-group <resource_group_name> -n <app_service_name>
For example : az webapp create-remote-connection --subscription 'c2727d11-526c-4244-8434-797dc6046f5e' --resource-group W_R -n "unbound-dkeV2"
then,  ssh root@127.0.0.1 -p <port>

4. to publish the zip folder: az webapp deployment source config-zip --resource-group <resource_group_name> --name "<app_service_name>" --src <zip_file>
For example: az webapp deployment source config-zip --resource-group W_R -n "unbound-dkeV2" --name "unbound-dkeV2" --src publish.zip

5. goto <yourhostname>/<key_name> and check if you see the key details.
for example: https://unbound-dke.azurewebsites.net/test1

# create new label

1. open Microsoft 365 compliance and create new label.

2. mark the checkbox of “use double key encryption” and enter the app service url with the key you use fro encryption . For example : https://unbound-dkev2.azurewebsites.net/my_key

3. choose the permmssion you like

# publish the label

1. open Microsoft 365 compliance and publish the labels created.

# How to use the created label with office app?

1. install Microsoft Azure Information Protection from here: Azure Information Protection 

2. open an office app like word/excel…

3. choose sensitivity->choose your created label->edit the document->save.

# Instructions to run in dev container 

1. open in devcontainer to build the container(or run the dockerFile)
2. build the project(run the build task)
3. start the program
4. navigate to https://localhost:5001/test1 
