﻿using Oqtane.Shared;
using Oqtane.Models;
using System;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;

namespace Oqtane.Services
{
    public class UserService : ServiceBase, IUserService
    {
        private readonly HttpClient http;
        private readonly SiteState sitestate;
        private readonly NavigationManager NavigationManager;

        public UserService(HttpClient http, SiteState sitestate, NavigationManager NavigationManager)
        {
            this.http = http;
            this.sitestate = sitestate;
            this.NavigationManager = NavigationManager;
        }

        private string apiurl
        {
            get { return CreateApiUrl(sitestate.Alias, NavigationManager.Uri, "User"); }
        }

        public async Task<User> GetUserAsync(int UserId, int SiteId)
        {
            return await http.GetJsonAsync<User>(apiurl + "/" + UserId.ToString() + "?siteid=" + SiteId.ToString());
        }

        public async Task<User> GetUserAsync(string Username, int SiteId)
        {
            return await http.GetJsonAsync<User>(apiurl + "/name/" + Username + "?siteid=" + SiteId.ToString());
        }

        public async Task<User> AddUserAsync(User User)
        {
            try
            {
                return await http.PostJsonAsync<User>(apiurl, User);
            }
            catch
            {
                return null;
            }
        }

        public async Task<User> AddUserAsync(User User, Alias Alias)
        {
            try
            {
                return await http.PostJsonAsync<User>(apiurl + "?alias=" + WebUtility.UrlEncode(Alias.Name), User);
            }
            catch
            {
                return null;
            }
        }

        public async Task<User> UpdateUserAsync(User User)
        {
            return await http.PutJsonAsync<User>(apiurl + "/" + User.UserId.ToString(), User);
        }
        public async Task DeleteUserAsync(int UserId)
        {
            await http.DeleteAsync(apiurl + "/" + UserId.ToString());
        }

        public async Task<User> LoginUserAsync(User User, bool SetCookie, bool IsPersistent)
        {
            return await http.PostJsonAsync<User>(apiurl + "/login?setcookie=" + SetCookie.ToString() + "&persistent=" + IsPersistent.ToString(), User);
        }

        public async Task LogoutUserAsync(User User)
        {
            // best practices recommend post is preferrable to get for logout
            await http.PostJsonAsync(apiurl + "/logout", User); 
        }

        public async Task<User> VerifyEmailAsync(User User, string Token)
        {
            return await http.PostJsonAsync<User>(apiurl + "/verify?token=" + Token, User);
        }

        public async Task ForgotPasswordAsync(User User)
        {
            await http.PostJsonAsync(apiurl + "/forgot", User);
        }

        public async Task<User> ResetPasswordAsync(User User, string Token)
        {
            return await http.PostJsonAsync<User>(apiurl + "/reset?token=" + Token, User);
        }

    }
}
