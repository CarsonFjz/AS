using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using HR.Security.Core.Services.Clients;
using HR.Security.Core.Services.Users;
using Autofac;
using Autofac.Integration.Owin;

namespace HR.OAuth2.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        //public override Task TokenEndpointResponse(OAuthTokenEndpointResponseContext context)
        //{
        //    context.Identity.AddClaim(new Claim("access_token", context.AccessToken));

        //    return Task.FromResult<object>(null);
        //}

        public override async Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (string.IsNullOrEmpty(context.ClientId))
            {
                return;
            }

            var clientService = context.OwinContext.GetAutofacLifetimeScope().Resolve<IClientService>();

            var clientObj = await clientService.GetByIdAsync(context.ClientId);

            if (clientObj != null)
            {
                if (clientObj.RedirectUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }
        }

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId;
            string clientSecret;

            if(context.TryGetBasicCredentials(out clientId, out clientSecret) || context.TryGetFormCredentials(out clientId, out clientSecret))
            {
                var clientService = context.OwinContext.GetAutofacLifetimeScope().Resolve<IClientService>();

                var clientObj = await clientService.GetByIdAsync(clientId);

                if(clientObj != null)
                {
                    if(clientObj.ClientSecret == clientSecret)
                    {
                        context.Validated();
                    }
                }
            }
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var userAccountService = context.OwinContext.GetAutofacLifetimeScope().Resolve<IUserAccountService>();

            var authenticateResult = await userAccountService.AuthenticateAsync(context.UserName, context.Password);

            var signInStatus = authenticateResult.Item1;
            var userAccount = authenticateResult.Item2;

            if(signInStatus != Security.Core.Results.SignInStatus.Succeeded)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");

                return;
            }

            Claim nameClaim = new Claim(ClaimTypes.Name, userAccount.UserName);
            Claim nameIdentifierClaim = new Claim(ClaimTypes.NameIdentifier, userAccount.ID.ToString());

            ClaimsIdentity oAuthIdentity = new ClaimsIdentity
            (
                new[] { nameClaim, nameIdentifierClaim },

                OAuthDefaults.AuthenticationType
            );

            AuthenticationProperties properties = CreateProperties(userAccount.UserName);

            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);

            context.Validated(ticket);
        }

        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }
    }
}