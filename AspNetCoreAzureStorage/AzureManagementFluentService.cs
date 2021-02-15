using Microsoft.Azure.Management.Graph.RBAC.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreAzureStorage
{
    public class AzureManagementFluentService
    {

        public AzureManagementFluentService()
        {
        }

        public List<IRoleAssignment> GetAssignedRolesAsync()
        {
            var clientId = "82d4bc96-976e-40d3-9f6a-9d2e3d8d19fc";
            var clientSecret = "AVR~IV_S42S961-Qf5I~3Et1X7vu252rn~";
            var tenantId = "7ff95b15-dc21-4ba6-bc92-824856578fc1";
            var scope = "subscriptions/1f943d6c-66d4-4c2f-a158-e6b99fcec7a2/resourceGroups/damienbod-rg/providers/Microsoft.Storage/storageAccounts/azureadfiles";
            //var scope = "subscriptions/1f943d6c-66d4-4c2f-a158-e6b99fcec7a2";


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

            var da = roleAssignments.ToList();

            return da;
        }
    }
}
