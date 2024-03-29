﻿using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Identity.Web;

namespace DelegatedEntraIDBlobStorage.FilesProvider.AzureStorageAccess;

public class BlobDelegatedUploadProvider
{
    private readonly DelegatedTokenAcquisitionTokenCredential _tokenAcquisitionTokenCredential;
    private readonly IConfiguration _configuration;

    public BlobDelegatedUploadProvider(DelegatedTokenAcquisitionTokenCredential tokenAcquisitionTokenCredential,
        IConfiguration configuration)
    {
        _tokenAcquisitionTokenCredential = tokenAcquisitionTokenCredential;
        _configuration = configuration;
    }

    [AuthorizeForScopes(Scopes = ["https://storage.azure.com/user_impersonation"])]
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

        var blobClient = new BlobClient(blobUri, _tokenAcquisitionTokenCredential);

        var inputStream = formFile.OpenReadStream();
        await blobClient.UploadAsync(inputStream, blobUploadOptions, cancellationToken);

        return $"{blobFileUpload.Name} successfully saved to Azure Blob Storage Container";
    }
}