using Microsoft.Identity.Client;

namespace MultiClientBlobStorage.Providers.Rbac;

public class AzureMgmtClientCredentialService
{
    private readonly IConfiguration _configuration;

    public AzureMgmtClientCredentialService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Using client secret from a singleton instance
    /// </summary>
    /// <returns></returns>
    public async Task<string> GetAccessToken()
    {
        string[] scopes = ["https://management.azure.com/.default"];
        var tenantId = _configuration["AzureMgmt:TenantId"];
        var clientId = _configuration.GetValue<string>("AzureMgmt:ClientId");
        var clientSecret = _configuration.GetValue<string>("AzureMgmt:ClientSecret");

        var app = ConfidentialClientApplicationBuilder.Create(clientId)
           .WithTenantId(tenantId)
           .WithClientSecret(clientSecret)
           .Build();

        var accessToken = await app.AcquireTokenForClient(scopes).ExecuteAsync();

        return accessToken.AccessToken;
    }
}
