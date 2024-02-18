using System.Net.Http.Headers;
using System.Text.Json.Serialization;

namespace MultiClientBlobStorage.Providers.Rbac;

public class AzureMgmtClientService
{
    private readonly AzureMgmtClientCredentialService _azureMgmtClientCredentialService;
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AzureMgmtClientService> _logger;

    public AzureMgmtClientService(AzureMgmtClientCredentialService azureMgmtClientCredentialService,
        IHttpClientFactory clientFactory,
        IConfiguration configuration,
        ILogger<AzureMgmtClientService> logger)
    {
        _azureMgmtClientCredentialService = azureMgmtClientCredentialService;
        _clientFactory = clientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Storage Blob Data Reader: ID: 2a2b9908-6ea1-4ae2-8e65-a410df84e7d1
    /// Role assignment required for application in Azure on resource group
    /// https://learn.microsoft.com/en-us/rest/api/authorization/role-assignments/create-by-id?view=rest-authorization-2022-04-01&tabs=HTTP
    /// https://learn.microsoft.com/en-us/azure/role-based-access-control/role-assignments-rest
    /// </summary>
    public async Task StorageBlobDataReaderRoleAssignment(string groupId, string storageAccountName, string blobContainerName)
    {
        // The role ID: Storage Blob Data Reader
        var roleId = "2a2b9908-6ea1-4ae2-8e65-a410df84e7d1";
        var roleNameUnique = $"{Guid.NewGuid()}"; // Must be a guid
        var subscriptionId = _configuration["AzureMgmt:SubscriptionId"];
        // the service principal ID
        var servicePrincipalId = groupId;
        // the resource group name
        var resourceGroupName = _configuration["AzureMgmt:ResourceGroupName"];

        var objectId = $"/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Storage/storageAccounts/{storageAccountName}/blobServices/default/containers/{blobContainerName}";
        var url = $"https://management.azure.com{objectId}/providers/Microsoft.Authorization/roleAssignments/{roleNameUnique}?api-version=2022-04-01";

        var client = _clientFactory.CreateClient();
        var accessToken = await _azureMgmtClientCredentialService.GetAccessToken();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var roleDefinitionId = $"{objectId}/providers/Microsoft.Authorization/roleDefinitions/{roleId}";

        var PayloadRoleAssignment = new PayloadRoleAssignment
        {
            Properties = new Properties
            {
                RoleDefinitionId = roleDefinitionId,
                PrincipalId = servicePrincipalId,
                PrincipalType = "Group"
            }
        };

        // view containers
        //var getRe = $"https://management.azure.com/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Storage/storageAccounts/{storageAccountName}/blobServices/default/containers?api-version=2023-01-01";
        //var response = await client.GetAsync(getRe);
        //var test = await response.Content.ReadAsStringAsync();

        var response = await client.PutAsJsonAsync(url, PayloadRoleAssignment);
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Created RBAC for read group {blobContainerName} {responseContent}", blobContainerName, responseContent);
            return;
        }

        var responseError = await response.Content.ReadAsStringAsync();
        _logger.LogCritical("Created RBAC for read group {blobContainerName} {responseError}", blobContainerName, responseError);
        throw new ApplicationException($"Status code: {response.StatusCode}, Error: {response.ReasonPhrase}, {responseError}");
    }

    private class PayloadRoleAssignment
    {
        [JsonPropertyName("properties")]
        public Properties Properties { get; set; } = new();
    }

    /// <summary>
    ///     "properties": {
    ///     "roleDefinitionId":
    ///     "subscriptions/SUBSCRIPTION_ID/resourcegroups/RESOURCE_GROUP_NAME/providers/Microsoft.Storage/storageAccounts/STORAGE_ACCOUNT_NAME/providers/Microsoft.Authorization/roleDefinitions/ROLE_ID",
    ///     "principalId": "SP_ID"
    ///     }
    /// </summary>
    private class Properties
    {
        [JsonPropertyName("roleDefinitionId")]
        public string RoleDefinitionId { get; set; } = string.Empty;
        [JsonPropertyName("principalId")]
        public string PrincipalId { get; set; } = string.Empty;
        [JsonPropertyName("principalType")]
        public string PrincipalType { get; set; } = "Group";
    }
}
