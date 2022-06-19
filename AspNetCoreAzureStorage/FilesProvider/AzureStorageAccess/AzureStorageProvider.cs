using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Identity.Web;

namespace AspNetCoreAzureStorage.FilesProvider.AzureStorageAccess;

public class AzureStorageProvider
{
    private readonly TokenAcquisitionTokenCredential _tokenAcquisitionTokenCredential;
    private readonly IConfiguration _configuration;

    public AzureStorageProvider(TokenAcquisitionTokenCredential tokenAcquisitionTokenCredential,
        IConfiguration configuration)
    {
        _tokenAcquisitionTokenCredential = tokenAcquisitionTokenCredential;
        _configuration = configuration;
    }

    [AuthorizeForScopes(Scopes = new string[] { "https://storage.azure.com/user_impersonation" })]
    public async Task<string> AddNewFile(BlobFileUpload blobFileUpload, IFormFile file)
    {
        try
        {
            return await PersistFileToAzureStorage(blobFileUpload, file);
        }
        catch (Exception e)
        {
            throw new ApplicationException($"Exception {e}");
        }
    }

    [AuthorizeForScopes(Scopes = new string[] { "https://storage.azure.com/user_impersonation" })]
    public async Task<Azure.Response<BlobDownloadInfo>> DownloadFile(string fileName)
    {
        var storage = _configuration.GetValue<string>("AzureStorage:StorageAndContainerName");
        var fileFullName = $"{storage}{fileName}";
        var blobUri = new Uri(fileFullName);
        var blobClient = new BlobClient(blobUri, _tokenAcquisitionTokenCredential);
        return await blobClient.DownloadAsync();
    }

    private async Task<string> PersistFileToAzureStorage(
        BlobFileUpload blobFileUpload,
        IFormFile formFile,
        CancellationToken cancellationToken = default)
    {
        var storage = _configuration.GetValue<string>("AzureStorage:StorageAndContainerName");
        var fileFullName = $"{storage}{blobFileUpload.Name}";
        var blobUri = new Uri(fileFullName);

        var blobUploadOptions = new BlobUploadOptions
        {
            Metadata = new Dictionary<string, string?>
            {
                { "uploadedBy", blobFileUpload?.UploadedBy },
                { "description", blobFileUpload?.Description }
            }
        };

        var blobClient = new BlobClient(blobUri, _tokenAcquisitionTokenCredential);

        var inputStream = formFile.OpenReadStream();
        await blobClient.UploadAsync(inputStream, blobUploadOptions, cancellationToken);

        return $"{blobFileUpload?.Name} successfully saved to Azure Storage Container";
    }
}