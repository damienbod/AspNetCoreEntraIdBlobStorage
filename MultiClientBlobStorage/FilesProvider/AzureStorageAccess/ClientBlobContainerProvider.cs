using Azure.Storage.Blobs;

namespace MultiClientBlobStorage.FilesProvider.AzureStorageAccess;

public class ClientBlobContainerProvider
{
    private readonly IConfiguration _configuration;
    private readonly ClientSecretCredentialProvider _clientSecretCredentialProvider;

    public ClientBlobContainerProvider(ClientSecretCredentialProvider clientSecretCredentialProvider, 
        IConfiguration configuration)
    {
        _configuration = configuration;
        _clientSecretCredentialProvider = clientSecretCredentialProvider;
    }

    public async Task<string> CreateClient(string clientName)
    {
        try
        {
            // 1. Create new security group for client users
            // 2. Create new Blob container
            // 3. RBAC security group Blob data read
            // NOTE blob write is configured on root 

            return await PersistFileToAzureStorage(clientName);
        }
        catch (Exception e)
        {
            throw new ApplicationException($"Exception {e}");
        }
    }

    private async Task<string> PersistFileToAzureStorage(string clientName)
    {
        var storage = _configuration.GetValue<string>("AzureStorage:StorageAndContainerName");
        var fileFullName = $"{storage}/{clientName}";
        var blobUri = new Uri(fileFullName);

        var blobClient = new BlobClient(blobUri, _clientSecretCredentialProvider.GetClientSecretCredential());

        return $"{clientName}: Azure Blob Storage Container created";
    }
}