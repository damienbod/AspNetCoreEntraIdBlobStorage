using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace MultiClientBlobStorage.FilesProvider.AzureStorageAccess;

public class BlobApplicationUploadProvider
{
    private readonly IConfiguration _configuration;
    private readonly ClientSecretCredentialProvider _clientSecretCredentialProvider;

    public BlobApplicationUploadProvider(ClientSecretCredentialProvider clientSecretCredentialProvider, 
        IConfiguration configuration)
    {
        _configuration = configuration;
        _clientSecretCredentialProvider = clientSecretCredentialProvider;
    }

    public async Task<string> AddNewFile(BlobFileUploadModel blobFileUpload, IFormFile file)
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

    private async Task<string> PersistFileToAzureStorage(
        BlobFileUploadModel blobFileUpload,
        IFormFile formFile,
        CancellationToken cancellationToken = default)
    {
        var storage = _configuration.GetValue<string>("AzureStorage:StorageAndContainerName");
        var fileFullName = $"{storage}/{blobFileUpload.Name}";
        var blobUri = new Uri(fileFullName);

        var blobUploadOptions = new BlobUploadOptions
        {
            Metadata = new Dictionary<string, string?>
            {
                { "uploadedBy", blobFileUpload.UploadedBy },
                { "description", blobFileUpload.Description }
            }
        };

        var blobClient = new BlobClient(blobUri, _clientSecretCredentialProvider.GetClientSecretCredential());

        var inputStream = formFile.OpenReadStream();
        await blobClient.UploadAsync(inputStream, blobUploadOptions, cancellationToken);

        return $"{blobFileUpload.Name} successfully saved to Azure Blob Storage Container";
    }
}