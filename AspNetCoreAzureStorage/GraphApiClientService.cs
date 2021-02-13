using Microsoft.Graph;
using Microsoft.Identity.Web;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AspNetCoreAzureStorage
{
    public class GraphApiClientService
    {
        private readonly GraphServiceClient _graphServiceClient;

        public GraphApiClientService(GraphServiceClient graphServiceClient)
        {
            _graphServiceClient = graphServiceClient;
        }

        public async Task<User> GetGraphApiUser()
        {
            var resId = "/subscriptions/1f943d6c-66d4-4c2f-a158-e6b99fcec7a2/resourceGroups/damienbod-rg/providers/Microsoft.Storage/storageAccounts/azureadfiles";
            var user = await _graphServiceClient
                .Me
                .Request()
                .GetAsync()
                .ConfigureAwait(false);

            var servicePrincipalSearch = await _graphServiceClient
                .ServicePrincipals
                .Request()
                .WithScopes("user.read", "Directory.Read.All", "User.ReadBasic.All")
                // .Select(x => new { x.Id, x.AppRoles })
                .Filter($"resourceId eq {resId}")
                // .Filter($"appId eq '{req.ApplicationId}'")
                .GetAsync();

            var appRoles = await _graphServiceClient.Users[user.Id]
                .AppRoleAssignments
                .Request()
                .GetAsync()
                .ConfigureAwait(false);

            return user;

            return await _graphServiceClient
                .Me
                .Request()
                //.WithScopes("User.ReadBasic.All", "user.read")
                .GetAsync()
                .ConfigureAwait(false);
        }
    }
}
