using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ImageGallery.Client.HttpHandlers
{
    /// <summary>
    /// Class to add something to the existing HttpClient
    /// </summary>
    public class BearerTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;

        public BearerTokenHandler(IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory)
        {
            _httpContextAccessor = httpContextAccessor ?? 
                throw new ArgumentNullException(nameof(httpContextAccessor));

            _httpClientFactory = httpClientFactory ??
                throw new ArgumentException(nameof(httpClientFactory));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var accessToken = await GetAccessTokenAsync();

            if (!string.IsNullOrWhiteSpace(accessToken))
                request.SetBearerToken(accessToken);

            return await base.SendAsync(request, cancellationToken);
        }

        private async Task<string> GetAccessTokenAsync()
        {
            // get the expires_at value & parse it
            var expiresAt = await _httpContextAccessor.HttpContext.GetTokenAsync("expires_at");

            var expiresAtAsDateTimeOffset = DateTimeOffset.Parse(expiresAt, CultureInfo.InvariantCulture);

            // if the access token expires in 60 econds or less, we're going to renew it
            if (expiresAtAsDateTimeOffset.AddSeconds(-60).ToUniversalTime() > DateTime.UtcNow)
            {
                // no need to refresh, return the access token
                return await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            }

            var idpClient = _httpClientFactory.CreateClient("IDPClient");

            // get the discovery document
            var discoveryDocument = await idpClient.GetDiscoveryDocumentAsync();

            // refresh the tokens
            var refreshToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            var refreshResponse = await idpClient.RequestRefreshTokenAsync(
                new RefreshTokenRequest
                {
                    Address = discoveryDocument.TokenEndpoint,
                    ClientId = "imagegalleryclient",
                    ClientSecret = "secret",
                    RefreshToken = refreshToken
                });

            // store the tokens
            var updatedTokens = new List<AuthenticationToken>();
            updatedTokens.Add(new AuthenticationToken
            {
                Name = OpenIdConnectParameterNames.IdToken,
                Value = refreshResponse.IdentityToken
            });

            updatedTokens.Add(new AuthenticationToken
            {
                Name = OpenIdConnectParameterNames.AccessToken,
                Value = refreshResponse.AccessToken
            });

            updatedTokens.Add(new AuthenticationToken
            {
                Name = OpenIdConnectParameterNames.RefreshToken,
                Value = refreshResponse.RefreshToken
            });

            updatedTokens.Add(new AuthenticationToken
            {
                Name = "expires_at",
                Value = (DateTime.UtcNow + TimeSpan.FromSeconds(refreshResponse.ExpiresIn)).ToString("o", CultureInfo.InvariantCulture)
            });

            // store in the same place as the OpenID Connect middleware stores them (cookie headers)
            // get authenticate result, containning the current principal & properties
            var currentAuthenticateResult = await _httpContextAccessor.HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // store the updated tokens
            currentAuthenticateResult.Properties.StoreTokens(updatedTokens);

            // sign in (effectively write the new cookie containing our updated tokens)
            await _httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                currentAuthenticateResult.Principal,
                currentAuthenticateResult.Properties);

            return refreshResponse.AccessToken;
        }
    }
}
