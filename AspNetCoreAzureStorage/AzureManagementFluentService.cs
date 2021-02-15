using Microsoft.Azure.Management.Graph.RBAC.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCoreAzureStorage
{
    public class AzureManagementFluentService
    {
        private readonly IConfiguration _configuration;

        public AzureManagementFluentService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<IRoleAssignment> GetAssignedRolesAsync(string scope)
        {
            var clientId = _configuration.GetValue<string>("AzureManagementFluent:ClientId");
            var clientSecret = _configuration.GetValue<string>("AzureManagementFluent:ClientSecret");
            var tenantId = _configuration.GetValue<string>("AzureManagementFluent:TenantId");

            AzureCredentialsFactory azureCredentialsFactory = new AzureCredentialsFactory();
            var credentials = azureCredentialsFactory
                .FromServicePrincipal(clientId, 
                    clientSecret, 
                    tenantId, 
                    AzureEnvironment.AzureGlobalCloud);

            // authenticate to Azure AD
            var authenticated = Microsoft.Azure.Management.Fluent.Azure
                        .Configure()
                        .Authenticate(credentials);

            var roleAssignments = authenticated
                .RoleAssignments
                .ListByScope(scope);

            // https://docs.microsoft.com/en-us/azure/role-based-access-control/built-in-roles
            // Storage Blob Data Contributor == "ba92f5b4-2d11-453d-a403-e96b0029c9fe"
            // Storage Blob Data Reader == "2a2b9908-6ea1-4ae2-8e65-a410df84e7d1"

            var storageBlobDataContributors = roleAssignments.Where(d => d.RoleDefinitionId.Contains("ba92f5b4-2d11-453d-a403-e96b0029c9fe")).ToList();
            var da = roleAssignments.ToList();

            return da;
        }
    }
}
