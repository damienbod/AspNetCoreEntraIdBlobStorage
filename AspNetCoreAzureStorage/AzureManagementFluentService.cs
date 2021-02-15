using Microsoft.Azure.Management.Graph.RBAC.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System.Threading.Tasks;

namespace AspNetCoreAzureStorage
{
    public class AzureManagementFluentService
    {

        public AzureManagementFluentService()
        {
        }

        public async Task<IPagedCollection<IRoleAssignment>> GetAssignedRolesAsync()
        {
            var clientId = "82d4bc96-976e-40d3-9f6a-9d2e3d8d19fc";
            var clientSecret = "AVR~IV_S42S961-Qf5I~3Et1X7vu252rn~";
            var tenantId = "7ff95b15-dc21-4ba6-bc92-824856578fc1";

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

            var roleAssignments = await authenticated
                .RoleAssignments
                .ListByScopeAsync("scope");

            return roleAssignments;
        }
    }
}
