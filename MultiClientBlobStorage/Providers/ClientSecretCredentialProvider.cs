using Azure.Identity;

namespace MultiClientBlobStorage.Providers;

public class ClientSecretCredentialProvider
{
    private readonly IConfiguration _configuration;

    public ClientSecretCredentialProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public ClientSecretCredential GetClientSecretCredential()
    {
        // Values from app registration
        var tenantId = _configuration.GetValue<string>("AzureAd:TenantId");
        var clientId = _configuration.GetValue<string>("AppAdminBlobStorageClient:ClientId");
        var clientSecret = _configuration.GetValue<string>("AppAdminBlobStorageClient:ClientSecret");

        var options = new ClientSecretCredentialOptions
        {
            AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
        };

        // https://docs.microsoft.com/dotnet/api/azure.identity.clientsecretcredential
        var clientSecretCredential = new ClientSecretCredential(
            tenantId, clientId, clientSecret, options);

        return clientSecretCredential;
    }
}