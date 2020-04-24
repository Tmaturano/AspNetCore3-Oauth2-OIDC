// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace Marvin.IDP
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Address(),
                new IdentityResource(
                    "roles", //scope name
                    "Your roles(s)", //display name
                    new List<string>() { "role" }), //list of claims that must be returned when an application asks for this roles scope.
                new IdentityResource(
                    "country", //scope name
                    "The country you're living in", //display name
                    new List<string>() { "country" }),
                new IdentityResource(
                    "subscriptionlevel", //scope name
                    "Your subscription level", //display name
                    new List<string>() { "subscriptionlevel" }),
            };

        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[]
            {
                new ApiResource(
                    "imagegalleryapi",
                    "Image Gallery API",
                    new List<string>() { "role" }) //list of claims that have to be returned when requesting this scope
                {
                    ApiSecrets = { new Secret("apisecret".Sha256()) } // need to define a secret when using Reference Token
                } 
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    AccessTokenType = AccessTokenType.Reference, //Reference instead of a self contained (jwt)
                    AccessTokenLifetime = 120,  //2 minutes. There's a 5 minutes off set time because of out of sync
                    AllowOfflineAccess = true, // the scope offline_access will be supported by our client, for refreshing token
                    
                    //For this demo, we let the Absolute Expiration, that is 30 days
                    //RefreshTokenExpiration = TokenExpiration.Sliding, //this way, the lifetime will be renewed when a new refresh token is requested
                    //SlidingRefreshTokenLifetime = 10, // we can set the sliding time in seconds, where the default is 15 days

                    UpdateAccessTokenClaimsOnRefresh = true, //if something in the user consent is updated, then it will refresh in the token as well                    
                    ClientName = "Image Gallery",  //This will appear in the consent screen and log in
                    ClientId = "imagegalleryclient",
                    AllowedGrantTypes = GrantTypes.Code,  //Working with the Code Flow
                    RequirePkce = true, //To avoid code substitution attacks
                    RedirectUris = new List<string>()
                    {
                        "https://localhost:44389/signin-oidc"  //The code is delivered to a browser by URI redirection
                    },
                    PostLogoutRedirectUris = new List<string>()
                    {
                        "https://localhost:44389/signout-callback-oidc"
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Address,
                        "roles",
                        "imagegalleryapi", // clients can request the API
                        "country",
                        "subscriptionlevel"
                    },
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256()) //To allow the client application to call the token endpoint
                    }
                }
            };
    }
}