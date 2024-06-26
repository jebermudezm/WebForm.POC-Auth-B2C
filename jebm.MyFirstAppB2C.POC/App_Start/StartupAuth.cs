﻿using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Claims;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;

namespace jebm.MyFirstAppB2C.POC
{
    public partial class Startup
    {

        private string redirectUri = ConfigurationManager.AppSettings["ida:RedirectUri"];
        private string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private string instance = ConfigurationManager.AppSettings["ida:AADInstance"];
        private string policy = ConfigurationManager.AppSettings["ida:Policy"];
            

        public void ConfigureAuth(IAppBuilder app)
        {
            var authority = $"{instance}{policy}/v2.0/";
            IdentityModelEventSource.ShowPII = true;
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = clientId,
                    Authority = authority,
                    RedirectUri = redirectUri,

                    ResponseType = OpenIdConnectResponseType.IdToken,
                    Scope = OpenIdConnectScope.OpenIdProfile,
                    TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true
                    },

                    Notifications = new OpenIdConnectAuthenticationNotifications()
                    {
                        AuthenticationFailed = (context) =>
                        {
                            return System.Threading.Tasks.Task.FromResult(0);
                        },

                        SecurityTokenValidated = (context) =>
                        {
                            var claims = context.AuthenticationTicket.Identity.Claims;
                            var claimName = claims.FirstOrDefault(x => x.Type.Equals("name"));
                            //context.AuthenticationTicket.Identity.AddClaim(new System.Security.Claims.Claim(claimName.Type, claimName.Value, string.Empty));
                            //var claimEmailaddress = claims.FirstOrDefault(x => x.Type.Contains("emailaddress"));
                            //context.AuthenticationTicket.Identity.AddClaim(new System.Security.Claims.Claim("emailaddress", claimEmailaddress.Value, string.Empty));

                            string name = claimName.Value;
                            context.AuthenticationTicket.Identity.AddClaim(new Claim(ClaimTypes.Name, name, string.Empty));
                            return System.Threading.Tasks.Task.FromResult(0);
                        }
                    }
                });

            // This makes any middleware defined above this line run before the Authorization rule is applied in web.config
            app.UseStageMarker(PipelineStage.Authenticate);
        }

        private static string EnsureTrailingSlash(string value)
        {
            if (value == null)
            {
                value = string.Empty;
            }

            if (!value.EndsWith("/", StringComparison.Ordinal))
            {
                return value + "/";
            }

            return value;
        }
    }
}
