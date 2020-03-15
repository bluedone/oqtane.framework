﻿using Oqtane.Models;
using System.Threading.Tasks;
using System.Net.Http;
using System.Linq;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using Oqtane.Shared;
using System.Net;
using System;

namespace Oqtane.Services
{
    public class AliasService : ServiceBase, IAliasService
    {
        private readonly HttpClient _http;
        private readonly SiteState _siteState;
        private readonly NavigationManager _navigationManager;

        public AliasService(HttpClient http, SiteState siteState, NavigationManager navigationManager)
        {
            _http = http;
            _siteState = siteState;
            _navigationManager = navigationManager;
        }

        private string Apiurl
        {
            get { return CreateApiUrl(_siteState.Alias, _navigationManager.Uri, "Alias"); }
        }

        public async Task<List<Alias>> GetAliasesAsync()
        {
            List<Alias> aliases = await _http.GetJsonAsync<List<Alias>>(Apiurl);
            return aliases.OrderBy(item => item.Name).ToList();
        }

        public async Task<Alias> GetAliasAsync(int aliasId)
        {
            return await _http.GetJsonAsync<Alias>(Apiurl + "/" + aliasId.ToString());
        }

        public async Task<Alias> GetAliasAsync(string url, DateTime lastSyncDate)
        {
            Uri uri = new Uri(url);
            string name = uri.Authority;
            if (uri.Segments.Count() > 1)
            {
                name += "/" + uri.Segments[1];
            }
            if (name.EndsWith("/")) 
            { 
                name = name.Substring(0, name.Length - 1); 
            }
            return await _http.GetJsonAsync<Alias>(Apiurl + "/name/" + WebUtility.UrlEncode(name) + "?lastsyncdate=" + lastSyncDate.ToString("yyyyMMddHHmmssfff"));
        }

        public async Task<Alias> AddAliasAsync(Alias alias)
        {
            return await _http.PostJsonAsync<Alias>(Apiurl, alias);
        }

        public async Task<Alias> UpdateAliasAsync(Alias alias)
        {
            return await _http.PutJsonAsync<Alias>(Apiurl + "/" + alias.AliasId.ToString(), alias);
        }
        public async Task DeleteAliasAsync(int aliasId)
        {
            await _http.DeleteAsync(Apiurl + "/" + aliasId.ToString());
        }
    }
}
