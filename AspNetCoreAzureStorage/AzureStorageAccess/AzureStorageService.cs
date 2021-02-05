using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCoreAzureStorage
{
    public class AzureStorageService
    {
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly IConfiguration _configuration;

        public AzureStorageService(ITokenAcquisition tokenAcquisition, 
            IConfiguration configuration)
        {
            _tokenAcquisition = tokenAcquisition;
            _configuration = configuration;
        }

        [AuthorizeForScopes(Scopes = new string[] { "https://storage.azure.com/user_impersonation" })]
        public async Task<string> AddNewFile(string fileName, IFormFile file)
        {
            try
            {
                // var scope = _configuration["AzureStorageApi:ScopeForAccessToken"];
                // var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { scope });

                return await PersistFileToAzureStorage(new TokenAcquisitionTokenCredential(_tokenAcquisition),
                    fileName, file);
            }
            catch (Exception e)
            {
                throw new ApplicationException($"Exception {e}");
            }
        }

        private async Task<string> PersistFileToAzureStorage(
            TokenAcquisitionTokenCredential tokenCredential, 
            string fileName,  IFormFile formFile,
            CancellationToken cancellationToken = default)
        {
            var storage = _configuration.GetValue<string>("AzureStorage:StorageAndContainerName");
            var fileFullName = $"{storage}{fileName}";

            Uri blobUri = new Uri(fileFullName);
            BlobClient blobClient = new BlobClient(blobUri, tokenCredential);

            var inputStream = formFile.OpenReadStream();
            await blobClient.UploadAsync(inputStream, cancellationToken);

            return "{fileName} successfully saved to Azure Storage Container";
        }
    }
}
