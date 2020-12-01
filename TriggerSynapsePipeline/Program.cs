using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest.Azure.Authentication;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TriggerSynapsePipeline
{
    class Program
    {
        static IConfiguration Configuration;
        static async Task Main(string[] args)
        {
            Configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables()
               .AddCommandLine(args)
               .Build();

            var runPipelineId = await TriggerSynapsePipeline();
            Console.WriteLine(runPipelineId);
            Console.ReadLine();
        }

        private static async Task<string> TriggerSynapsePipeline()
        {
            string tenantID = Configuration.GetSection("AppSettings:TenantId").Value;
            string applicationId = Configuration.GetSection("AppSettings:AppId").Value;
            string appSecretKey = Configuration.GetSection("AppSettings:SecretKey").Value;
            string workspaceName = Configuration.GetSection("AppSettings:WorkspaceName").Value;
            string pipelineName = Configuration.GetSection("AppSettings:Pipeline_Name").Value;

            var apiURL = $"https://{workspaceName}.dev.azuresynapse.net/pipelines/{pipelineName}/createRun?api-version=2018-06-01";

            var adSettings = new ActiveDirectoryServiceSettings
            {
                AuthenticationEndpoint = new Uri(AzureEnvironment.AzureGlobalCloud.AuthenticationEndpoint),
                TokenAudience = new Uri("https://dev.azuresynapse.net"),
                ValidateAuthority = true
            };

            await ApplicationTokenProvider.LoginSilentAsync(
                            tenantID,
                            applicationId,
                            appSecretKey,
                                adSettings,
                                TokenCache.DefaultShared);

            var token = TokenCache.DefaultShared.ReadItems()
                .Where(t => t.ClientId == applicationId)
                .OrderByDescending(t => t.ExpiresOn)
                .First();


            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("File_Name", "EMP_STG.csv"); //Pipeline Parameters

            return await MakeAPICall(apiURL, token.AccessToken, parameters);
        }

        private static async Task<string> MakeAPICall(string apiUrl, string authorizationKey, Dictionary<string, string> parameters)
        {
            var httpClientHandler = new HttpClientHandler();
            using (HttpClient httpClient = new HttpClient(httpClientHandler))
            {

                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authorizationKey);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage result;
                using (HttpContent content = new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json"))
                {
                    result = await httpClient.PostAsync(apiUrl, content);
                }

                var resultContent = result.Content.ReadAsStringAsync().Result;
                if (result.StatusCode == HttpStatusCode.OK || result.StatusCode == HttpStatusCode.Accepted)
                {
                    return resultContent;
                }
            }
            return null;
        }
    }
}

