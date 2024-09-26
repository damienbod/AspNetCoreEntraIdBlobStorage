using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Graph;
using System.Security.Cryptography.X509Certificates;

namespace MultiClientBlobStorage.Providers.GroupUserServices;

public class GraphApplicationClientService
{
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly IConfiguration _configuration;
    private readonly string _tenantId;
    private GraphServiceClient? _graphServiceClient;

    public GraphApplicationClientService(IConfiguration configuration)
    {
        _configuration = configuration;
        _clientId = configuration.GetValue<string>("GraphApi:ClientId")!;
        _clientSecret = configuration.GetValue<string>("GraphApi:ClientSecret")!;
        _tenantId = configuration.GetValue<string>("GraphApi:TenantId")!;
    }

    public GraphServiceClient GetGraphClientWithClientSecretCredential()
    {
        if (_graphServiceClient != null)
        {
            return _graphServiceClient;
        }

        TokenCredentialOptions options = new() { AuthorityHost = AzureAuthorityHosts.AzurePublicCloud };

        // https://docs.microsoft.com/dotnet/api/azure.identity.clientsecretcredential
        ClientSecretCredential clientSecretCredential = new(
            _tenantId, _clientId, _clientSecret, options);

        var scopes = "https://graph.microsoft.com/.default";
        _graphServiceClient = new GraphServiceClient(clientSecretCredential, scopes.Split(' '));
        return _graphServiceClient;
    }


    /// <summary>
    ///     Using Graph SDK client with a certificate
    ///     https://learn.microsoft.com/en-us/azure/active-directory/develop/msal-net-client-assertions
    /// </summary>
    public async Task<GraphServiceClient> GetGraphClientWithClientCertificateCredentialAsync()
    {
        if (_graphServiceClient != null)
        {
            return _graphServiceClient;
        }

        TokenCredentialOptions options = new() { AuthorityHost = AzureAuthorityHosts.AzurePublicCloud };

        var certificate = await GetCertificateAsync();
        ClientCertificateCredential clientCertificateCredential = new(
            _tenantId, _clientId, certificate, options);

        var scopes = "https://graph.microsoft.com/.default";

        _graphServiceClient = new GraphServiceClient(clientCertificateCredential, scopes.Split(' '));
        return _graphServiceClient;
    }

    public async Task<X509Certificate2> GetCertificateAsync()
    {
        var identifier = _configuration["GraphApi:ClientCertificates:0:KeyVaultCertificateName"];

        if (identifier == null)
        {
            throw new ArgumentNullException(nameof(identifier));
        }

        var vaultBaseUrl = _configuration["GraphApi:ClientCertificates:0:KeyVaultUrl"];
        if (vaultBaseUrl == null)
        {
            throw new ArgumentNullException(nameof(vaultBaseUrl));
        }

        SecretClient secretClient = new(new Uri(vaultBaseUrl), new DefaultAzureCredential());
        var secretName = identifier;
        KeyVaultSecret secret = await secretClient.GetSecretAsync(secretName);

        var privateKeyBytes = Convert.FromBase64String(secret.Value);

        X509Certificate2 certificateWithPrivateKey = new(privateKeyBytes,
            string.Empty, X509KeyStorageFlags.MachineKeySet);

        return certificateWithPrivateKey;
    }
}
