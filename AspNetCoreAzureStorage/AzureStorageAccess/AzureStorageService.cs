using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreAzureStorage
{
    public class AzureStorageService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly IConfiguration _configuration;

        public AzureStorageService(IHttpClientFactory clientFactory, 
            ITokenAcquisition tokenAcquisition, 
            IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _tokenAcquisition = tokenAcquisition;
            _configuration = configuration;
        }

        [AuthorizeForScopes(Scopes = new string[] { "https://storage.azure.com/user_impersonation" })]
        public async Task<string> AddNewFile()
        {
            try
            {
                
                var scope = _configuration["AzureStorageApi:ScopeForAccessToken"];
                // var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { scope });

                return await CreateBlob(new TokenAcquisitionTokenCredential(_tokenAcquisition));
            }
            catch (Exception e)
            {
                throw new ApplicationException($"Exception {e}");
            }
        }

 
        private static async Task<string> CreateBlob(TokenAcquisitionTokenCredential tokenCredential)
        {
            Uri blobUri = new Uri("https://azureadfiles.blob.core.windows.net/sample-container/Blob1.txt");
            BlobClient blobClient = new BlobClient(blobUri, tokenCredential);

            string blobContents = "Blob created by Azure AD authenticated user.";
            byte[] byteArray = Encoding.ASCII.GetBytes(blobContents);

            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                await blobClient.UploadAsync(stream);
            }
            return "Blob successfully created";
        }
    }
}
