using System;
using System.Linq;
using System.Threading;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace MVP_API
{
    public class TokenSecureAuthentication
    {
        private readonly string _issuer;

        public TokenSecureAuthentication(string issuer)
        {
            _issuer = issuer;
        }
        public SecurityKey GetOpenIdConnectConfigSigningKey()
        {
            IConfigurationManager<OpenIdConnectConfiguration> configurationManager =
                new ConfigurationManager<OpenIdConnectConfiguration>(
                    $"{_issuer}.well-known/openid-configuration",
                    new OpenIdConnectConfigurationRetriever());

            var openIdConnectConfiguration = configurationManager.GetConfigurationAsync(CancellationToken.None)
                .GetAwaiter().GetResult();

            return openIdConnectConfiguration.SigningKeys.ToList()[0];
        }

        /// <summary>
        /// This exists to fix weird issue when validating issuer.
        /// </summary>
        /// <param name="issuer"></param>
        /// <param name="securitytoken"></param>
        /// <param name="validationparameters"></param>
        /// <returns></returns>
        public string CustomIssuerValidator(string issuer, SecurityToken securitytoken, TokenValidationParameters validationparameters)
        {
            try
            {
                var issuerTrimmed = _issuer.Split(new[] { @"https://" }, StringSplitOptions.None)[1].TrimEnd('/');
                if (issuer.ToLower().Contains(issuerTrimmed.ToLower())) return issuer;
                throw new SecurityTokenInvalidIssuerException($"IDX10205: Issuer validation failed. Issuer. Did not match: validationParameters.ValidIssuer: '{issuerTrimmed}'.");
            }
            catch (IndexOutOfRangeException)
            {
                throw new IndexOutOfRangeException("The .config valid issuer should start with https:// and end with a trailing slash /.");
            }
        }
    }
}