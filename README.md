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


## Preparing for Production
We have few options for the signing material:

* Raw RSA (SHA256) keys, which can store somewhere safe or even hard code.
* Signing certificate, wich has the added value of having a set expiration date. You're forced to replace it from time to time. That certificate can be deployed together with the identity server, or it can be stored in a safer location and read from it, like the Window Certificate Store or Key Vault on Azure.
* For this demo, it's stored in the certificate store.

### Preparing for Production (creating a signing certificate)

* Created a self-signed certificate, or you can buy one from a certificate authority.
* PowerShell command to create the self-signed certificate: Run as administrator and type: New-SelfSignedCertificate -Subject "CN=MarvinIdSrvSigningCert" -CertStoreLocation "cert:\\LocalMachine\My"
* After that, go to the "Manage computer certificates", then select Personal and you'll see the MarvinIdSrvSigningCert. Open it, go to the details tab and search for Thumbprint. Copy that value and change in the Startup.cs in the Identity Server project.
* Since this is a self-signed certificate, we need to add it to the Trusted Root Certification Authority Certificates folder (right bellow the personal folder) in order to be seen as valid. In Real Life, we typically have an in-company trusted CA, or certificates like this are bought from trusted root CAs. 