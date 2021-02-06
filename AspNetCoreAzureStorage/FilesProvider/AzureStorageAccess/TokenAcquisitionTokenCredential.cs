﻿using Azure.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCoreAzureStorage.FilesProvider.AzureStorageAccess
{
    public class TokenAcquisitionTokenCredential : TokenCredential
    {
        private ITokenAcquisition _tokenAcquisition;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor from an ITokenAcquisition service.
        /// </summary>
        /// <param name="tokenAcquisition">Token acquisition.</param>
        public TokenAcquisitionTokenCredential(ITokenAcquisition tokenAcquisition,
            IConfiguration configuration)
        {
            _tokenAcquisition = tokenAcquisition;
            _configuration = configuration;
            _tokenAcquisition = tokenAcquisition;
        }

        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override async ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            // requestContext.Scopes "https://storage.azure.com/.default"
            string[] scopes = _configuration["AzureStorage:ScopeForAccessToken"]?.Split(' ');

            AuthenticationResult result = await _tokenAcquisition
                .GetAuthenticationResultForUserAsync(scopes)
                .ConfigureAwait(false);

            return new AccessToken(result.AccessToken, result.ExpiresOn);
        }
    }
}