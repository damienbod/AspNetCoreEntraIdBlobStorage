using Microsoft.Identity.Client;

namespace GraphClientCrendentials;

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
        string[] scopes = ["https://graph.microsoft.com/.default"];
        var tenantId = _configuration["Graph:TenantId"];
        var clientId = _configuration.GetValue<string>("Graph:ClientId");
        var clientSecret = _configuration.GetValue<string>("Graph:ClientSecret");

        var app = ConfidentialClientApplicationBuilder.Create(clientId)
           .WithTenantId(tenantId)
           .WithClientSecret(clientSecret)
           .Build();

        var accessToken = await app.AcquireTokenForClient(scopes).ExecuteAsync();

        return accessToken.AccessToken;
    }
}
