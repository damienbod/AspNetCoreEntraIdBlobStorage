using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Microsoft.Identity.Web;

namespace AspNetCoreAzureStorageUserAccess.FilesProvider.AzureStorageAccess;

public class BlobDownloadUserSasProvider
{
    private readonly LocalTokenAcquisitionTokenCredential _tokenAcquisitionTokenCredential;
    private readonly IConfiguration _configuration;

    public BlobDownloadUserSasProvider(LocalTokenAcquisitionTokenCredential tokenAcquisitionTokenCredential,
        IConfiguration configuration)
    {
        _tokenAcquisitionTokenCredential = tokenAcquisitionTokenCredential;
        _configuration = configuration;
    }

    public async Task<UserDelegationKey> RequestUserDelegationKey(BlobServiceClient blobServiceClient)
    {
        // Get a user delegation key for the Blob service that's valid for 1 day
        var userDelegationKey = await blobServiceClient.GetUserDelegationKeyAsync(
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow.AddHours(1));

        return userDelegationKey;
    }

    public Uri CreateUserDelegationSASBlob(BlobClient blobClient, UserDelegationKey userDelegationKey)
    {
        // Create a SAS token for the blob resource that's also valid for 1 day
        var sasBuilder = new BlobSasBuilder()
        {
            BlobContainerName = blobClient.BlobContainerName,
            BlobName = blobClient.Name,
            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow,
            ExpiresOn = DateTimeOffset.UtcNow.AddDays(1)
        };

        // Specify the necessary permissions
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        // Add the SAS token to the blob URI
        var uriBuilder = new BlobUriBuilder(blobClient.Uri)
        {
            // Specify the user delegation key
            Sas = sasBuilder.ToSasQueryParameters(
                userDelegationKey,
                blobClient
                .GetParentBlobContainerClient()
                .GetParentBlobServiceClient().AccountName)
        };

        return uriBuilder.ToUri();
    }

    [AuthorizeForScopes(Scopes = ["https://storage.azure.com/user_impersonation"])]
    public async Task<Azure.Response<BlobDownloadInfo>> DownloadFile(string fileName)
    {
        var storage = _configuration.GetValue<string>("AzureStorage:StorageAndContainerName");
        //var fileFullName = $"{storage}/{fileName}";
        //var blobUri = new Uri(fileFullName);
        //var blobClient = new BlobClient(blobUri, _tokenAcquisitionTokenCredential);

        var blobServiceClient = new BlobServiceClient(
            new Uri(storage!), 
            _tokenAcquisitionTokenCredential);

        var blobClient = blobServiceClient
                .GetBlobContainerClient(storage)
                .GetBlobClient(fileName);

        var userDelegationKey = await RequestUserDelegationKey(blobServiceClient);

        if (userDelegationKey == null)
        {
            throw new ArgumentNullException(nameof(userDelegationKey));
        }

        var blobSASURI = CreateUserDelegationSASBlob(blobClient, userDelegationKey);
        // Create a blob client object with SAS authorization
        var blobClientSAS = new BlobClient(blobSASURI);

        return await blobClientSAS.DownloadAsync();
    }
}