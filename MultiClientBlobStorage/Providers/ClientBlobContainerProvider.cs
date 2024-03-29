﻿using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MultiClientBlobStorage.Providers.Rbac;
using Polly;
using System.Text;

namespace MultiClientBlobStorage.Providers;

public class ClientBlobContainerProvider
{
    private readonly IConfiguration _configuration;
    private readonly ClientSecretCredentialProvider _clientSecretCredentialProvider;
    private readonly AzureMgmtClientService _azureMgmtClientService;

    public ClientBlobContainerProvider(ClientSecretCredentialProvider clientSecretCredentialProvider,
        AzureMgmtClientService azureMgmtClientService,
        IConfiguration configuration)
    {
        _configuration = configuration;
        _clientSecretCredentialProvider = clientSecretCredentialProvider;
        _azureMgmtClientService = azureMgmtClientService;
    }

    public async Task<BlobContainerClient?> CreateBlobContainerClient(string clientName)
    {
        try
        {
            // Create new Blob container
            var blobContainer = await CreateContainer(clientName);

            return blobContainer;
        }
        catch (Exception e)
        {
            throw new ApplicationException($"Exception {e}");
        }
    }

    public async Task ApplyReaderGroupToBlobContainer(BlobContainerClient blobContainer, string groupId)
    {
        var maxRetryAttempts = 20;
        var pauseBetweenFailures = TimeSpan.FromSeconds(3);

        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(maxRetryAttempts, i => pauseBetweenFailures);

        await retryPolicy.ExecuteAsync(async () =>
        {
            // RBAC security group Blob data read
            await _azureMgmtClientService
                .StorageBlobDataReaderRoleAssignment(groupId,
                    blobContainer.AccountName,
                    blobContainer.Name);

            // NOTE service principal blob write is configured on root 
        });
    }

    private async Task<BlobContainerClient> CreateContainer(string name)
    {
        try
        {
            var formatted = RemoveSpecialCharacters(name);
            string containerName = $"blob-{formatted.Trim()}-{Guid.NewGuid()}".ToLower();
            var storage = _configuration.GetValue<string>("AzureStorage:Storage");
            var credential = _clientSecretCredentialProvider.GetClientSecretCredential();

            if (storage != null && credential != null)
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
            if (c >= '0' && c <= '9' || c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z' || c == '.' || c == '_')
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }
}