﻿using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Oqtane.Models;
using Oqtane.Services;
using Oqtane.Shared;

namespace Oqtane.Providers
{
    public class IdentityAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IUriHelper urihelper;
        private readonly SiteState sitestate;

        public IdentityAuthenticationStateProvider(IUriHelper urihelper, SiteState sitestate)
        {
            this.urihelper = urihelper;
            this.sitestate = sitestate;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // hack: create a new HttpClient rather than relying on the registered service as the AuthenticationStateProvider is initialized prior to IUriHelper ( https://github.com/aspnet/AspNetCore/issues/11867 )
            HttpClient http = new HttpClient();
            string apiurl = ServiceBase.CreateApiUrl(sitestate.Alias, urihelper.GetAbsoluteUri(), "User") + "/authenticate";
            User user = await http.GetJsonAsync<User>(apiurl);

            ClaimsIdentity identity = new ClaimsIdentity();
            if (user.IsAuthenticated)
            {
                identity = new ClaimsIdentity("Identity.Application");
                identity.AddClaim(new Claim(ClaimTypes.Name, user.Username));
                foreach(string role in user.Roles.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, role));
                }
            }
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public void NotifyAuthenticationChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
