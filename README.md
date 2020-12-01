# TriggerSynapsePipeline

Triggers "Azure Syanpse Analytics Workspace Built-in Pipeline" using C#.

Prerequisite
1. Register an APP with "Azure Active Directory" and note down TenantId, Application Id, and Secrety Key.
2. Add this APP under "Access Control(IAM)" section of "Azure Synapse Analytics Workspace" that you have provisioned. Give "Contributor" role to this APP.
3. Add following values in appsettings.config file before you run the project.

"AppSettings": {
    "TenantId": "",
    "AppId": "",
    "SecretKey": "",
    "WorkspaceName": "",
    "Pipeline_Name": ""
  }
  
4. Remove folloiwng line from Program.cs and Add your Pipeline Parameters entries here.
   parameters.Add("File_Name", "EMP_STG.csv"); //Pipeline Parameters
   
