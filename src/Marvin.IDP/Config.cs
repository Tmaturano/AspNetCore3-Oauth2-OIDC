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
                new IdentityResources.Profile()
            };

        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[] 
            { };
        
        public static IEnumerable<Client> Clients =>
            new Client[] 
            { new Client {
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
                    IdentityServerConstants.StandardScopes.Profile
                },
                ClientSecrets =
                {
                    new Secret("secret".Sha256()) //To allow the client application to call the token endpoint
                }
            } };        
    }
}