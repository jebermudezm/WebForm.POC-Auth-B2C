using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.OWIN;
using Microsoft.Identity.Web.TokenCacheProviders.InMemory;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace WebApp.AzureAD.POC
{
    public partial class Startup
    {
        private static string postLogoutRedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];

        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = ConfigurationManager.AppSettings["ida:ClientId"],
                    Authority = ConfigurationManager.AppSettings["ida:AadInstance"],
                    RedirectUri = ConfigurationManager.AppSettings["ida:RedirectUri"],
                    PostLogoutRedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"],
                    TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        NameClaimType = "name"
                    },
                    Notifications = new OpenIdConnectAuthenticationNotifications
                    {
                        AuthenticationFailed = (context) =>
                        {
                            context.HandleResponse();
                            context.Response.Redirect("/Home/Error?message=" + context.Exception.Message);
                            return System.Threading.Tasks.Task.FromResult(0);
                        }
                    }
                }
            );
        }

        public void ConfigureAuthss(IAppBuilder app)
        {
            ////app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            ////app.UseCookieAuthentication(new CookieAuthenticationOptions());

            ////app.UseCookieAuthentication(new CookieAuthenticationOptions());
            ////OwinTokenAcquirerFactory factory = TokenAcquirerFactory.GetDefaultInstance<OwinTokenAcquirerFactory>();

            ////app.AddMicrosoftIdentityWebApp(factory);
            ////factory.Services
            ////    .Configure<ConfidentialClientApplicationOptions>(options => { options.RedirectUri = postLogoutRedirectUri; })
            ////    .AddMicrosoftGraph()
            ////    .AddInMemoryTokenCaches();
            ////factory.Build();

            //app.UseOpenIdConnectAuthentication(
            //    new OpenIdConnectAuthenticationOptions
            //    {
            //        ClientId = clientId,
            //        Authority = authority,
            //        PostLogoutRedirectUri = postLogoutRedirectUri,

            //        Notifications = new OpenIdConnectAuthenticationNotifications()
            //        {
            //            AuthenticationFailed = (context) =>
            //            {
            //                return System.Threading.Tasks.Task.FromResult(0);
            //            },

            //            SecurityTokenValidated = (context) =>
            //            {
            //                string name = context.AuthenticationTicket.Identity.FindFirst("preferred_username").Value;
            //                context.AuthenticationTicket.Identity.AddClaim(new Claim(ClaimTypes.Name, name, string.Empty));
            //                return System.Threading.Tasks.Task.FromResult(0);
            //            }
            //        }
            //    });

            // This makes any middleware defined above this line run before the Authorization rule is applied in web.config

            // Configure your Azure AD B2C details here
           

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
