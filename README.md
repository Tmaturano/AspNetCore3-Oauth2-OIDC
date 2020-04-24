# AspNetCore3-Oauth2-OIDC
This project demonstrate an Authentication/Authorization flow using Identity Server 4 and AspNet Core 3.1 and it's a result of the Pluralsight course (Securing ASP.NET Core 3 with OAuth2 and OpenID Connect, by Kevin Dockx)

The client is a simple MVC project that communicates with a .Net Core API project where both are protected by the Identity Server project, using authorization code flow with PKCE protection, handling Access and Refresh Tokens and Token Revocation.

* dotnet new -i IdentityServer4.Templates  //To install IS4 the templates for the project
* dotnet new is4empty -n CompanyName.IDP   //To add an empty IS4 project
* dotnet new is4ui //This will add all the controllers and support classes and all the views related to those controllers. Run this command in the Identity Server project folder

* https://localhost:44318/.well-known/openid-configuration //It's an URL that display all information regarding authorization, authentication, token renew, etc, provided by IS4

* The IDP level uses Reference Token instead of self-contained(JWT) to get great control over token lifetime.
* The validation of the identity token at level of the client.
* The validation of the access token at level of the API.
* The tokens have a limited lifetime and it uses refresh tokens to get a new access token when expired.
