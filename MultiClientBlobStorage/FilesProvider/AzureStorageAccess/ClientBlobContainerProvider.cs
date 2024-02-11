using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Text;

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
            var blobContainer = await CreateContainer(clientName);
            // 3. RBAC security group Blob data read
            // Storage Blob Data Reader
            // ID: 2a2b9908-6ea1-4ae2-8e65-a410df84e7d1
            // Application.ReadWrite.All AppRoleAssignment.ReadWrite.All
            // https://cloud.google.com/bigquery/docs/omni-azure-create-connection#microsoft-rest-api
            // NOTE blob write is configured on root 

            return blobContainer.Name;
        }
        catch (Exception e)
        {
            throw new ApplicationException($"Exception {e}");
        }
    }

    private async Task<BlobContainerClient> CreateContainer(string name)
    {
        try
        {
            var formatted = RemoveSpecialCharacters(name);
            string containerName = $"blob-{formatted.Trim()}-{Guid.NewGuid()}".ToLower();
            var storage = _configuration.GetValue<string>("AzureStorage:Storage");
            var credential = _clientSecretCredentialProvider.GetClientSecretCredential();

            if(storage != null && credential != null)
            {
                var blobServiceClient = new BlobServiceClient(new Uri(storage), credential);

                var metadata = new Dictionary<string, string?>
                {
                    { "name", name },
                };

                // Create the root container or handle the exception if it already exists
                var blobContainerClient = await blobServiceClient.CreateBlobContainerAsync(containerName, 
                    PublicAccessType.None,
                    metadata);

                if (blobContainerClient.Value.Exists())
                {
                    Console.WriteLine($"Created container: {name} {blobContainerClient.Value.Name}");
                }

                return blobContainerClient.Value;
            }

            throw new Exception($"Could not create container: {name}");
        }
        catch (RequestFailedException e)
        {
            Console.WriteLine("HTTP error code {0}: {1}", e.Status, e.ErrorCode);
            Console.WriteLine(e.Message);
            throw;
        }
    }

    private string RemoveSpecialCharacters(string str)
    {
        var sb = new StringBuilder();
        foreach (char c in str)
        {
            if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }
}