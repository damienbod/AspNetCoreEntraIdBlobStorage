using System.Net.Http.Headers;

namespace AzureMgmtClientCrendentials;

public class MicrosoftGraphApplicationClient
{
    private readonly AzureMgmtClientCredentialService _azureMgmtClientCredentialService;
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;

    public MicrosoftGraphApplicationClient(AzureMgmtClientCredentialService azureMgmtClientCredentialService, 
        IHttpClientFactory clientFactory,
        IConfiguration configuration)
    {
        _azureMgmtClientCredentialService = azureMgmtClientCredentialService;
        _clientFactory = clientFactory;
        _configuration = configuration;
    }

    /// <summary>
    /// Storage Blob Data Reader: ID: 2a2b9908-6ea1-4ae2-8e65-a410df84e7d1
    /// Application.ReadWrite.All AppRoleAssignment.ReadWrite.All
    /// https://cloud.google.com/bigquery/docs/omni-azure-create-connection#microsoft-rest-api
    /// </summary>
    public async Task StorageBlobDataReaderRoleAssignment(string groupId, string storageAccountName, string blobContainerName)
    {
        // The role ID: Storage Blob Data Reader
        var roleId = "2a2b9908-6ea1-4ae2-8e65-a410df84e7d1";
        var subscriptionId = "a2029853-0102-49be-931a-3edf53744c56";
        // the service principal ID
        var servicePrincipalId = groupId;
        // the resource group name
        var resourceGroupName = "blob-security-rg";

        var url = $"https://management.azure.com/subscriptions/{subscriptionId}/resourcegroups/{resourceGroupName}/providers/Microsoft.Storage/storageAccounts/{storageAccountName}/providers/Microsoft.Authorization/roleAssignments/{roleId}?api-version=2022-04-01";
        
        var client = _clientFactory.CreateClient();
        var accessToken = await _azureMgmtClientCredentialService.GetAccessToken();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
        var roleDefinitionId = $"/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Storage/storageAccounts/{storageAccountName}/blobServices/default/containers/{blobContainerName}/providers/Microsoft.Authorization/roleDefinitions/{roleId}";
        var PayloadRoleAssignment = new PayloadRoleAssignment
        {
            properties = new properties
            {
                roleDefinitionId = roleDefinitionId,
                principalId = servicePrincipalId
            }
        };

        // view containers
        //var getRe = $"https://management.azure.com/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Storage/storageAccounts/{storageAccountName}/blobServices/default/containers?api-version=2023-01-01";
        //var response = await client.GetAsync(getRe);

        var response = await client.PutAsJsonAsync<PayloadRoleAssignment>(url, PayloadRoleAssignment);
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var data = System.Text.Json.JsonSerializer.Deserialize<List<string>>(responseContent);
        }

        throw new ApplicationException($"Status code: {response.StatusCode}, Error: {response.ReasonPhrase}");
    }

    public class PayloadRoleAssignment
    {
        public properties properties { get; set; } = new();
    }

    /// <summary>
    /// "properties": {
    ///    "roleDefinitionId": "subscriptions/SUBSCRIPTION_ID/resourcegroups/RESOURCE_GROUP_NAME/providers/Microsoft.Storage/storageAccounts/STORAGE_ACCOUNT_NAME/providers/Microsoft.Authorization/roleDefinitions/ROLE_ID",
    ///    "principalId": "SP_ID"
    ///}
    /// </summary>
    public class properties
    {
        public string roleDefinitionId { get; set; } = string.Empty;
        public string principalId { get; set; } = string.Empty;
        
    }
}
