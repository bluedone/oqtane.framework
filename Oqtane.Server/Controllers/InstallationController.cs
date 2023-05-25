using System;
using System.IO;
using System.Reflection;
using System.Linq;
using System.IO.Compression;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Shared;
using Oqtane.Themes;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using Oqtane.Repository;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Oqtane.Controllers
{
    [Route(ControllerRoutes.ApiRoute)]
    public class InstallationController : Controller
    {
        private readonly IConfigManager _configManager;
        private readonly IInstallationManager _installationManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly ILocalizationManager _localizationManager;
        private readonly IMemoryCache _cache;
        private readonly IHttpContextAccessor _accessor;
        private readonly IAliasRepository _aliases;
        private readonly ILogger<InstallationController> _filelogger;
        private readonly ITenantManager _tenantManager;
        private readonly ServerStateManager _serverState;

        public InstallationController(IConfigManager configManager, IInstallationManager installationManager, IDatabaseManager databaseManager, ILocalizationManager localizationManager, IMemoryCache cache, IHttpContextAccessor accessor, IAliasRepository aliases, ILogger<InstallationController> filelogger, ITenantManager tenantManager, ServerStateManager serverState)
        {
            _configManager = configManager;
            _installationManager = installationManager;
            _databaseManager = databaseManager;
            _localizationManager = localizationManager;
            _cache = cache;
            _accessor = accessor;
            _aliases = aliases;
            _filelogger = filelogger;
            _tenantManager = tenantManager;
            _serverState = serverState;
        }

        // POST api/<controller>
        [HttpPost]
        public async Task<Installation> Post([FromBody] InstallConfig config)
        {
            var installation = new Installation { Success = false, Message = "" };

            if (ModelState.IsValid && (User.IsInRole(RoleNames.Host) || string.IsNullOrEmpty(_configManager.GetSetting($"{SettingKeys.ConnectionStringsSection}:{SettingKeys.ConnectionStringKey}", ""))))
            {
                installation = _databaseManager.Install(config);

                if (installation.Success && config.Register)
                {
                    await RegisterContact(config.HostEmail);
                }
            }
            else
            {
                installation.Message = "Installation Not Authorized";
            }

            return installation;
        }

        // GET api/<controller>/installed/?path=xxx
        [HttpGet("installed")]
        public Installation IsInstalled(string path)
        {
            var installation = _databaseManager.IsInstalled();
            if (installation.Success)
            {
                path = _accessor.HttpContext.Request.Host.Value + "/" + WebUtility.UrlDecode(path);
                installation.Alias = _aliases.GetAlias(path);
            }
            return installation;
        }

        [HttpGet("upgrade")]
        [Authorize(Roles = RoleNames.Host)]
        public Installation Upgrade()
        {
            var installation = new Installation { Success = true, Message = "" };
            _installationManager.UpgradeFramework();
            return installation;
        }

        // GET api/<controller>/restart
        [HttpPost("restart")]
        [Authorize(Roles = RoleNames.Host)]
        public void Restart()
        {
            _installationManager.RestartApplication();
        }

        // GET api/<controller>/list
        [HttpGet("list")]
        public List<string> List()
        {
            return GetAssemblyList().Select(item => item.HashedName).ToList();
        }

        // GET api/<controller>/load?list=x,y
        [HttpGet("load")]
        public IActionResult Load(string list = "*")
        {
            return File(GetAssemblies(list), System.Net.Mime.MediaTypeNames.Application.Octet, "oqtane.dll");
        }

        private List<ClientAssembly> GetAssemblyList()
        {
            int siteId = _tenantManager.GetAlias().SiteId;

            return _cache.GetOrCreate($"assemblieslist:{siteId}", entry =>
            {
                var binFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                var assemblyList = new List<ClientAssembly>();

                // testmode setting is used for validating that the API is downloading the appropriate assemblies to the client
                bool hashfilename = true;
                if (_configManager.GetSetting($"{SettingKeys.TestModeKey}", "false") == "true")
                {
                    hashfilename = false;
                }

                // get site assemblies which should be downloaded to client
                var assemblies = _serverState.GetServerState(siteId).Assemblies;

                // populate assembly list
                foreach (var assembly in assemblies)
                {
                    if (assembly != Constants.ClientId)
                    {
                        var filepath = Path.Combine(binFolder, assembly) + ".dll";
                        if (System.IO.File.Exists(filepath))
                        {
                            assemblyList.Add(new ClientAssembly(Path.Combine(binFolder, assembly + ".dll"), hashfilename));
                        }
                    }
                }

                // insert satellite assemblies at beginning of list
                foreach (var culture in _localizationManager.GetInstalledCultures())
                {
                    if (culture != Constants.DefaultCulture)
                    {
                        var assembliesFolderPath = Path.Combine(binFolder, culture);
                        if (Directory.Exists(assembliesFolderPath))
                        {
                            foreach (var assembly in assemblies)
                            {
                                var filepath = Path.Combine(assembliesFolderPath, assembly) + ".resources.dll";
                                if (System.IO.File.Exists(filepath))
                                {
                                    assemblyList.Insert(0, new ClientAssembly(Path.Combine(assembliesFolderPath, assembly + ".resources.dll"), hashfilename));
                                }
                            }
                        }
                        else
                        {
                            _filelogger.LogError(Utilities.LogMessage(this, $"The Satellite Assembly Folder For {culture} Does Not Exist"));
                        }
                    }
                }

                return assemblyList;
            });
        }

        private byte[] GetAssemblies(string list)
        {
            if (list == "*")
            {
                return _cache.GetOrCreate("assemblies", entry =>
                {
                    return GetZIP(list);
                });
            }
            else
            {
                return GetZIP(list);
            }
        }

        private byte[] GetZIP(string list)
        {
            var binFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            // get list of assemblies which should be downloaded to client
            List<ClientAssembly> assemblies = GetAssemblyList();
            if (list != "*")
            {
                var filter = list.Split(',').ToList();
                assemblies.RemoveAll(item => !filter.Contains(item.HashedName));
            }

            // create zip file containing assemblies and debug symbols
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var assembly in assemblies)
                    {
                        if (Path.GetFileNameWithoutExtension(assembly.FilePath) != Constants.ClientId)
                        {
                            if (System.IO.File.Exists(assembly.FilePath))
                            {
                                using (var filestream = new FileStream(assembly.FilePath, FileMode.Open, FileAccess.Read))
                                using (var entrystream = archive.CreateEntry(assembly.HashedName).Open())
                                {
                                    filestream.CopyTo(entrystream);
                                }
                            }
                            var pdb = assembly.FilePath.Replace(".dll", ".pdb");
                            if (System.IO.File.Exists(pdb))
                            {
                                using (var filestream = new FileStream(pdb, FileMode.Open, FileAccess.Read))
                                using (var entrystream = archive.CreateEntry(assembly.HashedName.Replace(".dll", ".pdb")).Open())
                                {
                                    filestream.CopyTo(entrystream);
                                }
                            }
                        }
                    }
                }

                return memoryStream.ToArray();
            }
        }

        private async Task RegisterContact(string email)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Referer", HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value);
                    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(Constants.PackageId, Constants.Version));
                    Uri uri = new Uri(Constants.PackageRegistryUrl + $"/api/registry/contact/?id={_configManager.GetInstallationId()}&email={WebUtility.UrlEncode(email)}");
                    var response = await client.GetAsync(uri).ConfigureAwait(false);
                }
            }
            catch
            {
                // error calling registry service
            }
        }

        // GET api/<controller>/register?email=x
        [HttpPost("register")]
        [Authorize(Roles = RoleNames.Host)]
        public async Task Register(string email)
        {
            await RegisterContact(email);
        }

        public struct ClientAssembly
        {
            public ClientAssembly(string filepath, bool hashfilename)
            {
                FilePath = filepath;
                DateTime lastwritetime = System.IO.File.GetLastWriteTime(filepath);
                if (hashfilename)
                {
                    HashedName = GetDeterministicHashCode(filepath).ToString("X8") + "." + lastwritetime.ToString("yyyyMMddHHmmss") + Path.GetExtension(filepath);
                }
                else
                {
                    HashedName = Path.GetFileNameWithoutExtension(filepath) + "." + lastwritetime.ToString("yyyyMMddHHmmss") + Path.GetExtension(filepath);
                }
            }

            public string FilePath { get; private set; }
            public string HashedName { get; private set; }
        }

        private static int GetDeterministicHashCode(string value)
        {
            unchecked
            {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;

                for (int i = 0; i < value.Length; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ value[i];
                    if (i == value.Length - 1)
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ value[i + 1];
                }

                return hash1 + (hash2 * 1566083941);
            }
        }

    }
}
