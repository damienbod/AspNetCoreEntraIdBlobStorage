using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCoreAzureStorage.FilesProvider.AzureStorageAccess
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
        public async Task<string> AddNewFile(BlobFileUpload blobFileUpload, IFormFile file)
        {
            try
            {
                // var scope = _configuration["AzureStorageApi:ScopeForAccessToken"];
                // var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { scope });

                return await PersistFileToAzureStorage(new TokenAcquisitionTokenCredential(_tokenAcquisition),
                    blobFileUpload, file);
            }
            catch (Exception e)
            {
                throw new ApplicationException($"Exception {e}");
            }
        }

        private async Task<string> PersistFileToAzureStorage(
            TokenAcquisitionTokenCredential tokenCredential,
            BlobFileUpload blobFileUpload,  
            IFormFile formFile,
            CancellationToken cancellationToken = default)
        {
            var storage = _configuration.GetValue<string>("AzureStorage:StorageAndContainerName");
            var fileFullName = $"{storage}{blobFileUpload.Name}";

            Uri blobUri = new Uri(fileFullName);

            BlobUploadOptions blobUploadOptions = new BlobUploadOptions
            {
                Metadata = new Dictionary<string, string>
                {
                    { "uploadedBy", blobFileUpload.UploadedBy },
                    { "description", blobFileUpload.Description }
                }
            };

            BlobClient blobClient = new BlobClient(blobUri, tokenCredential);

            var inputStream = formFile.OpenReadStream();
            await blobClient.UploadAsync(inputStream, blobUploadOptions, cancellationToken);

            return $"{blobFileUpload.Name} successfully saved to Azure Storage Container";
        }
    }
}
