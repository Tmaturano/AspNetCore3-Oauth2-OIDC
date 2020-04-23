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
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
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