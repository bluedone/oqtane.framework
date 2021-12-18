using Microsoft.AspNetCore.Mvc.RazorPages;
using Oqtane.Infrastructure;
using Oqtane.Shared;
using Oqtane.Modules;
using Oqtane.Models;
using Oqtane.Themes;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Oqtane.Repository;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Oqtane.Pages
{
    public class HostModel : PageModel
    {
        private IConfiguration _configuration;
        private readonly ITenantManager _tenantManager;
        private readonly ILocalizationManager _localizationManager;
        private readonly ILanguageRepository _languages;
        private readonly IAntiforgery _antiforgery;
        private readonly ISiteRepository _sites;
        private readonly IPageRepository _pages;
        private readonly IUrlMappingRepository _urlMappings;
        private readonly IVisitorRepository _visitors;

        public HostModel(IConfiguration configuration, ITenantManager tenantManager, ILocalizationManager localizationManager, ILanguageRepository languages, IAntiforgery antiforgery, ISiteRepository sites, IPageRepository pages, IUrlMappingRepository urlMappings, IVisitorRepository visitors)
        {
            _configuration = configuration;
            _tenantManager = tenantManager;
            _localizationManager = localizationManager;
            _languages = languages;
            _antiforgery = antiforgery;
            _sites = sites;
            _pages = pages;
            _urlMappings = urlMappings;
            _visitors = visitors;
        }

        public string AntiForgeryToken = "";
        public string Runtime = "Server";
        public RenderMode RenderMode = RenderMode.Server;
        public int VisitorId = -1;
        public string HeadResources = "";
        public string BodyResources = "";
        public string Title = "";
        public string FavIcon = "favicon.ico";
        public string PWAScript = "";
        public string ThemeType = "";

        public IActionResult OnGet()
        {
            AntiForgeryToken = _antiforgery.GetAndStoreTokens(HttpContext).RequestToken;

            if (_configuration.GetSection("Runtime").Exists())
            {
                Runtime = _configuration.GetSection("Runtime").Value;
            }

            if (_configuration.GetSection("RenderMode").Exists())
            {
                RenderMode = (RenderMode)Enum.Parse(typeof(RenderMode), _configuration.GetSection("RenderMode").Value, true);
            }

            // if framework is installed 
            if (!string.IsNullOrEmpty(_configuration.GetConnectionString("DefaultConnection")))
            {
                var alias = _tenantManager.GetAlias();
                if (alias != null)
                {
                    Route route = new Route(HttpContext.Request.GetEncodedUrl(), alias.Path);

                    var site = _sites.GetSite(alias.SiteId);
                    if (site != null)
                    {
                        if (!string.IsNullOrEmpty(site.Runtime))
                        {
                            Runtime = site.Runtime;
                        }
                        if (!string.IsNullOrEmpty(site.RenderMode))
                        {
                            RenderMode = (RenderMode)Enum.Parse(typeof(RenderMode), site.RenderMode, true);
                        }
                        if (site.FaviconFileId != null)
                        {
                            FavIcon = Utilities.ContentUrl(alias, site.FaviconFileId.Value);
                        }
                        if (site.PwaIsEnabled && site.PwaAppIconFileId != null && site.PwaSplashIconFileId != null)
                        {
                            PWAScript = CreatePWAScript(alias, site, route);
                        }
                        Title = site.Name;
                        ThemeType = site.DefaultThemeType;

                        if (site.VisitorTracking)
                        {
                            TrackVisitor(site.SiteId);
                        }

                        var page = _pages.GetPage(route.PagePath, site.SiteId);
                        if (page != null)
                        {
                            // set page title
                            if (!string.IsNullOrEmpty(page.Title))
                            {
                                Title = page.Title;
                            }
                            else
                            {
                                Title = Title + " - " + page.Name;
                            }

                            // include theme resources
                            if (!string.IsNullOrEmpty(page.ThemeType))
                            {
                                ThemeType = page.ThemeType;
                            }
                        }
                        else
                        {
                            // page does not exist
                            var url = route.SiteUrl + "/" + route.PagePath;
                            var urlMapping = _urlMappings.GetUrlMapping(site.SiteId, url);
                            if (urlMapping != null && !string.IsNullOrEmpty(urlMapping.MappedUrl))
                            {
                                return RedirectPermanent(urlMapping.MappedUrl);
                            }
                        }
                    }

                    // include global resources
                    var assemblies = AppDomain.CurrentDomain.GetOqtaneAssemblies();
                    foreach (Assembly assembly in assemblies)
                    {
                        ProcessHostResources(assembly);
                        ProcessModuleControls(assembly);
                        ProcessThemeControls(assembly);
                    }

                    // set culture if not specified
                    if (HttpContext.Request.Cookies[CookieRequestCultureProvider.DefaultCookieName] == null)
                    {
                        // set default language for site if the culture is not supported
                        var languages = _languages.GetLanguages(alias.SiteId);
                        if (languages.Any() && languages.All(l => l.Code != CultureInfo.CurrentUICulture.Name))
                        {
                            var defaultLanguage = languages.Where(l => l.IsDefault).SingleOrDefault() ?? languages.First();
                            SetLocalizationCookie(defaultLanguage.Code);
                        }
                        else
                        {
                            SetLocalizationCookie(_localizationManager.GetDefaultCulture());
                        }
                    }
                }
            }
            return Page();
        }

        private void TrackVisitor(int SiteId)
        {
            // get request attributes
            string ip = HttpContext.Connection.RemoteIpAddress.ToString();
            string useragent = Request.Headers[HeaderNames.UserAgent];
            string language = Request.Headers[HeaderNames.AcceptLanguage];
            if (language.Contains(","))
            {
                language = language.Substring(0, language.IndexOf(","));
            }
            string url = Request.GetEncodedUrl();
            string referrer = Request.Headers[HeaderNames.Referer];
            int? userid = null;
            if (User.HasClaim(item => item.Type == ClaimTypes.PrimarySid))
            {
                userid = int.Parse(User.Claims.First(item => item.Type == ClaimTypes.PrimarySid).Value);
            }

            var VisitorCookie = "APP_VISITOR_" + SiteId.ToString();
            if (!int.TryParse(Request.Cookies[VisitorCookie], out VisitorId))
            {
                var visitor = new Visitor();
                visitor.SiteId = SiteId;
                visitor.IPAddress = ip;
                visitor.UserAgent = useragent;
                visitor.Language = language;
                visitor.Url = url;
                visitor.Referrer = referrer;
                visitor.UserId = userid;
                visitor.Visits = 1;
                visitor.CreatedOn = DateTime.UtcNow;
                visitor.VisitedOn = DateTime.UtcNow;
                visitor = _visitors.AddVisitor(visitor);

                Response.Cookies.Append(
                    VisitorCookie,
                    visitor.VisitorId.ToString(),
                    new CookieOptions()
                    {
                        Expires = DateTimeOffset.UtcNow.AddYears(1),
                        IsEssential = true
                    }
                );
            }
            else
            {
                var visitor = _visitors.GetVisitor(VisitorId);
                if (visitor != null)
                {
                    visitor.IPAddress = ip;
                    visitor.UserAgent = useragent;
                    visitor.Language = language;
                    visitor.Url = url;
                    if (!string.IsNullOrEmpty(referrer))
                    {
                        visitor.Referrer = referrer;
                    }
                    if (userid != null)
                    {
                        visitor.UserId = userid;
                    }
                    visitor.Visits += 1;
                    visitor.VisitedOn = DateTime.UtcNow;
                    _visitors.UpdateVisitor(visitor);
                }
                else
                {
                    Response.Cookies.Delete(VisitorCookie);
                }
            }
        }

        private string CreatePWAScript(Alias alias, Site site, Route route)
        {
            return
            "<script>" +
                "setTimeout(() => { " +
                    "var manifest = { " +
                        "\"name\": \"" + site.Name + "\", " +
                        "\"short_name\": \"" + site.Name + "\", " +
                        "\"start_url\": \"" + route.SiteUrl + "/\", " +
                        "\"display\": \"standalone\", " +
                        "\"background_color\": \"#fff\", " +
                        "\"description\": \"" + site.Name + "\", " +
                        "\"icons\": [{ " +
                            "\"src\": \"" + route.RootUrl + Utilities.ContentUrl(alias, site.PwaAppIconFileId.Value) + "\", " +
                            "\"sizes\": \"192x192\", " +
                            "\"type\": \"image/png\" " +
                            "}, { " +
                            "\"src\": \"" + route.RootUrl + Utilities.ContentUrl(alias, site.PwaSplashIconFileId.Value) + "\", " +
                            "\"sizes\": \"512x512\", " +
                            "\"type\": \"image/png\" " +
                        "}] " +
                    "}; " +
                    "const serialized = JSON.stringify(manifest); " +
                    "const blob = new Blob([serialized], {type: 'application/javascript'}); " +
                    "const url = URL.createObjectURL(blob); " +
                    "document.getElementById('app-manifest').setAttribute('href', url); " +
                "} " +
                ", 1000);" +
                "if ('serviceWorker' in navigator) { " +
                    "navigator.serviceWorker.register('/service-worker.js').then(function(registration) { " +
                        "console.log('ServiceWorker Registration Successful'); " +
                    "}).catch (function(err) { " +
                        "console.log('ServiceWorker Registration Failed ', err); " +
                    "}); " +
                "};" +
            "</script>";
        }

        private void ProcessHostResources(Assembly assembly)
        {
            var types = assembly.GetTypes().Where(item => item.GetInterfaces().Contains(typeof(IHostResources)));
            foreach (var type in types)
            {
                var obj = Activator.CreateInstance(type) as IHostResources;
                foreach (var resource in obj.Resources)
                {
                    ProcessResource(resource);
                }
            }
        }

        private void ProcessModuleControls(Assembly assembly)
        {
            var types = assembly.GetTypes().Where(item => item.GetInterfaces().Contains(typeof(IModuleControl)));
            foreach (var type in types)
            {
                // Check if type should be ignored
                if (type.IsOqtaneIgnore()) continue;

                var obj = Activator.CreateInstance(type) as IModuleControl;
                if (obj.Resources != null)
                {
                    foreach (var resource in obj.Resources)
                    {
                        if (resource.Declaration == ResourceDeclaration.Global)
                        {
                            ProcessResource(resource);
                        }
                    }
                }
            }
        }

        private void ProcessThemeControls(Assembly assembly)
        {
            var types = assembly.GetTypes().Where(item => item.GetInterfaces().Contains(typeof(IThemeControl)));
            foreach (var type in types)
            {
                // Check if type should be ignored
                if (type.IsOqtaneIgnore()) continue;

                var obj = Activator.CreateInstance(type) as IThemeControl;
                if (obj.Resources != null)
                {
                    foreach (var resource in obj.Resources)
                    {
                        if (resource.Declaration == ResourceDeclaration.Global || (Utilities.GetFullTypeName(type.AssemblyQualifiedName) == ThemeType && resource.ResourceType == ResourceType.Stylesheet))
                        {
                            ProcessResource(resource);
                        }
                    }
                }
            }
        }
        private void ProcessResource(Resource resource)
        {
            switch (resource.ResourceType)
            {
                case ResourceType.Stylesheet:
                    if (!HeadResources.Contains(resource.Url, StringComparison.OrdinalIgnoreCase))
                    {
                        var id = (resource.Declaration == ResourceDeclaration.Global) ? "" : "id=\"app-stylesheet-" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "-00\" ";
                        HeadResources += "<link " + id + "rel=\"stylesheet\" href=\"" + resource.Url + "\"" + CrossOrigin(resource.CrossOrigin) + Integrity(resource.Integrity) + " />" + Environment.NewLine;
                    }
                    break;
                case ResourceType.Script:
                    if (resource.Location == Shared.ResourceLocation.Body)
                    {
                        if (!BodyResources.Contains(resource.Url, StringComparison.OrdinalIgnoreCase))
                        {
                            BodyResources += "<script src=\"" + resource.Url + "\"" + CrossOrigin(resource.CrossOrigin) + Integrity(resource.Integrity) + "></script>" + Environment.NewLine;
                        }
                    }
                    else
                    {
                        if (!HeadResources.Contains(resource.Url, StringComparison.OrdinalIgnoreCase))
                        {
                            HeadResources += "<script src=\"" + resource.Url + "\"" + CrossOrigin(resource.CrossOrigin) + Integrity(resource.Integrity) + "></script>" + Environment.NewLine;
                        }
                    }
                    break;
            }
        }
        private string CrossOrigin(string crossorigin)
        {
            if (!string.IsNullOrEmpty(crossorigin))
            {
                return " crossorigin=\"" + crossorigin + "\"";
            }
            else
            {
                return "";
            }
        }
        private string Integrity(string integrity)
        {
            if (!string.IsNullOrEmpty(integrity))
            {
                return " integrity=\"" + integrity + "\"";
            }
            else
            {
                return "";
            }
        }

        private void SetLocalizationCookie(string culture)
        {
            HttpContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)));
        }
    }
}
