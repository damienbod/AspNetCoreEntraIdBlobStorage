using Microsoft.Azure.Management.Graph.RBAC.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.Azure.Management.Fluent.Azure;

namespace AspNetCoreAzureStorage
{
    public class AzureManagementFluentService
    {
        private readonly IConfiguration _configuration;
        private IAuthenticated _authenticatedClient;

        public AzureManagementFluentService(IConfiguration configuration)
        {
            _configuration = configuration;
            AuthenticateClient();
        }

        public bool HasRoleStorageBlobDataContributorForScope(string userPrincipalId, string scope)
        {
            var roleAssignments = GetStorageBlobDataContributors(scope);
            return roleAssignments.Count(t => t.PrincipalId == userPrincipalId) > 0;
        }

        public bool HasRoleStorageBlobDataReaderForScope(string userPrincipalId, string scope)
        {
            var roleAssignments = GetStorageBlobDataReaders(scope);
            return roleAssignments.Count(t => t.PrincipalId == userPrincipalId) > 0;
        }

        /// <summary>
        /// returns IRoleAssignment for Storage Blob Data Contributor 
        /// </summary>
        /// <param name="scope">Scope of the Azure storage</param>
        /// <returns>IEnumerable of the IRoleAssignment</returns>
        private IEnumerable<IRoleAssignment> GetStorageBlobDataContributors(string scope)
        {
            var roleAssignments = _authenticatedClient
                .RoleAssignments
                .ListByScope(scope);

            // https://docs.microsoft.com/en-us/azure/role-based-access-control/built-in-roles
            // Storage Blob Data Contributor == "ba92f5b4-2d11-453d-a403-e96b0029c9fe"
            // Storage Blob Data Reader == "2a2b9908-6ea1-4ae2-8e65-a410df84e7d1"

            var storageBlobDataContributors = roleAssignments
                .Where(d => d.RoleDefinitionId.Contains("ba92f5b4-2d11-453d-a403-e96b0029c9fe"));

            return storageBlobDataContributors;
        }

        /// <summary>
        /// returns IRoleAssignment for Storage Blob Data Contributor or Storage Blob Data Reader
        /// </summary>
        /// <param name="scope">Scope of the Azure storage</param>
        /// <returns>IEnumerable of the IRoleAssignment</returns>
        private IEnumerable<IRoleAssignment> GetStorageBlobDataReaders(string scope)
        {
            var roleAssignments = _authenticatedClient
                .RoleAssignments
                .ListByScope(scope);

            // https://docs.microsoft.com/en-us/azure/role-based-access-control/built-in-roles
            // Storage Blob Data Contributor == "ba92f5b4-2d11-453d-a403-e96b0029c9fe"
            // Storage Blob Data Reader == "2a2b9908-6ea1-4ae2-8e65-a410df84e7d1"

            var storageBlobDataReaders = roleAssignments
                .Where(d => d.RoleDefinitionId.Contains("ba92f5b4-2d11-453d-a403-e96b0029c9fe") ||
                d.RoleDefinitionId.Contains("2a2b9908-6ea1-4ae2-8e65-a410df84e7d1"));

            return storageBlobDataReaders;
        }

        private void AuthenticateClient()
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
            _authenticatedClient = Microsoft.Azure.Management.Fluent.Azure.Configure()
                        .Authenticate(credentials);
        }
    }
}
