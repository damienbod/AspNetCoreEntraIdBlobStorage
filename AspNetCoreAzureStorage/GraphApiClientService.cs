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
            return await _graphServiceClient
                .Me
                .Request()
                //.WithScopes("User.ReadBasic.All", "user.read")
                .GetAsync()
                .ConfigureAwait(false);
        }
    }
}
