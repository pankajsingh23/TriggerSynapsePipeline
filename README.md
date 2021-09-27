# TriggerSynapsePipeline

Triggers "Azure Syanpse Analytics Workspace Built-in Pipeline" using C#.

Prerequisite
1. Create an App Registration with "Azure Active Directory" and note down Directory (tenant) ID, Application (client) ID. Create a client secret and take note of the Secret Value.
2. Within the Azure Synapse Analytics Workspace's management pane, in "Access Control" settings assign the new app registration the `Synapse Contributor` and `Synapse Credential User` role. Learn more about roles and why this is required: https://docs.microsoft.com/en-us/azure/synapse-analytics/security/synapse-workspace-understand-what-role-you-need.
3. Add following values in appsettings.config file before you run the project.
```
"AppSettings": {
    "TenantId": "",
    "AppId": "",
    "SecretKey": "",
    "WorkspaceName": "",
    "Pipeline_Name": ""
  }
  ```
4. Remove folloiwng line from Program.cs and Add your Pipeline Parameters entries here.
   parameters.Add("File_Name", "EMP_STG.csv"); //Pipeline Parameters
   
