using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Identity.Web;

namespace DelegatedReadAppWriteBlobStorage.FilesProvider.AzureStorageAccess;

public class BlobDelegatedDownloadProvider
{
    private readonly DelegatedTokenAcquisitionTokenCredential _tokenAcquisitionTokenCredential;
    private readonly IConfiguration _configuration;

    public BlobDelegatedDownloadProvider(DelegatedTokenAcquisitionTokenCredential tokenAcquisitionTokenCredential,
        IConfiguration configuration)
    {
        _tokenAcquisitionTokenCredential = tokenAcquisitionTokenCredential;
        _configuration = configuration;
    }

    [AuthorizeForScopes(Scopes = ["https://storage.azure.com/user_impersonation"])]
    public async Task<Azure.Response<BlobDownloadInfo>> DownloadFile(string fileName)
    {
        var storage = _configuration.GetValue<string>("AzureStorage:StorageAndContainerName");
        var fileFullName = $"{storage}/{fileName}";
        var blobUri = new Uri(fileFullName);
        var blobClient = new BlobClient(blobUri, _tokenAcquisitionTokenCredential);
        return await blobClient.DownloadAsync();
    }
}