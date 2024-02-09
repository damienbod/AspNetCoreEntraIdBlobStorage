using Azure.Core;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;

namespace AspNetCoreAzureStorageUserAccess.FilesProvider.AzureStorageAccess;

public class LocalTokenAcquisitionTokenCredential : TokenCredential
{
    private readonly ITokenAcquisition _tokenAcquisition;
    private readonly IConfiguration _configuration;

    public LocalTokenAcquisitionTokenCredential(ITokenAcquisition tokenAcquisition,
        IConfiguration configuration)
    {
        _tokenAcquisition = tokenAcquisition;
        _configuration = configuration;
    }

    public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override async ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
    {
        string[]? scopes = _configuration["AzureStorage:ScopeForAccessToken"]?.Split(' ');

        if (scopes == null)
        {
            throw new Exception("AzureStorage:ScopeForAccessToken configuration missing");
        }

        AuthenticationResult result = await _tokenAcquisition
            .GetAuthenticationResultForUserAsync(scopes);

        return new AccessToken(result.AccessToken, result.ExpiresOn);
    }
}