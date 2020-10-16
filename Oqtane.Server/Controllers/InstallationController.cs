﻿using System;
using System.IO;
using System.Reflection;
using System.Linq;
using System.IO.Compression;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Shared;
using Oqtane.Themes;

namespace Oqtane.Controllers
{
    [Route(ControllerRoutes.Default)]
    public class InstallationController : Controller
    {
        private readonly IConfigurationRoot _config;
        private readonly IInstallationManager _installationManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly ILocalizationManager _localizationManager;

        public InstallationController(IConfigurationRoot config, IInstallationManager installationManager, IDatabaseManager databaseManager, ILocalizationManager localizationManager)
        {
            _config = config;
            _installationManager = installationManager;
            _databaseManager = databaseManager;
            _localizationManager = localizationManager;
        }

        // POST api/<controller>
        [HttpPost]
        public Installation Post([FromBody] InstallConfig config)
        {
            var installation = new Installation {Success = false, Message = ""};

            if (ModelState.IsValid && (User.IsInRole(RoleNames.Host) || string.IsNullOrEmpty(_config.GetConnectionString(SettingKeys.ConnectionStringKey))))
            {
                installation = _databaseManager.Install(config);
            }
            else
            {
                installation.Message = "Installation Not Authorized";
            }

            return installation;
        }

        // GET api/<controller>/installed
        [HttpGet("installed")]
        public Installation IsInstalled()
        {
            bool isInstalled = _databaseManager.IsInstalled();
            return new Installation {Success = isInstalled, Message = string.Empty};
        }

        [HttpGet("upgrade")]
        [Authorize(Roles = RoleNames.Host)]
        public Installation Upgrade()
        {
            var installation = new Installation {Success = true, Message = ""};
            _installationManager.UpgradeFramework();
            return installation;
        }

        // GET api/<controller>/load
        [HttpGet("load")]
        public IActionResult Load()
        {
            if (_config.GetSection("Runtime").Value == "WebAssembly")
            {
                // get list of assemblies which should be downloaded to browser
                var assemblies = AppDomain.CurrentDomain.GetOqtaneClientAssemblies();
                var list = assemblies.Select(a => a.GetName().Name).ToList();
                var binFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

                // Get the satellite assemblies
                foreach (var culture in _localizationManager.GetSupportedCultures())
                {
                    var assembliesFolderPath = Path.Combine(binFolder, culture);
                    if (culture == Constants.DefaultCulture)
                    {
                        continue;
                    }

                    if(Directory.Exists(assembliesFolderPath))
                    {
                        foreach (var resourceFile in Directory.EnumerateFiles(assembliesFolderPath))
                        {
                            list.Add(Path.Combine(culture, Path.GetFileNameWithoutExtension(resourceFile)));
                        }
                    }
                    else
                    {
                        Console.WriteLine($"The satellite assemblies folder named '{culture}' is not found.");
                    }
                }

                // get module and theme dependencies
                foreach (var assembly in assemblies)
                {
                    foreach (var type in assembly.GetTypes().Where(item => item.GetInterfaces().Contains(typeof(IModule))))
                    {
                        var instance = Activator.CreateInstance(type) as IModule;
                        foreach (string name in instance.ModuleDefinition.Dependencies.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (!list.Contains(name)) list.Add(name);
                        }
                    }
                    foreach (var type in assembly.GetTypes().Where(item => item.GetInterfaces().Contains(typeof(ITheme))))
                    {
                        var instance = Activator.CreateInstance(type) as ITheme;
                        foreach (string name in instance.Theme.Dependencies.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (!list.Contains(name)) list.Add(name);
                        }
                    }
                }

                // create zip file containing assemblies and debug symbols
                byte[] zipfile;
                using (var memoryStream = new MemoryStream())
                {
                    using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        ZipArchiveEntry entry;
                        foreach (string file in list)
                        {
                            entry = archive.CreateEntry(file + ".dll");
                            using (var filestream = new FileStream(Path.Combine(binFolder, file + ".dll"), FileMode.Open, FileAccess.Read))
                            using (var entrystream = entry.Open())
                            {
                                filestream.CopyTo(entrystream);
                            }

                            // include debug symbols ( we may want to consider restricting this to only host users or when running in debug mode for performance )
                            if (System.IO.File.Exists(Path.Combine(binFolder, file + ".pdb")))
                            {
                                entry = archive.CreateEntry(file + ".pdb");
                                using (var filestream = new FileStream(Path.Combine(binFolder, file + ".pdb"), FileMode.Open, FileAccess.Read))
                                using (var entrystream = entry.Open())
                                {
                                    filestream.CopyTo(entrystream);
                                }
                            }
                        }
                    }
                    zipfile = memoryStream.ToArray();
                }
                return File(zipfile, System.Net.Mime.MediaTypeNames.Application.Octet, "oqtane.zip");
            }
            else
            {
                HttpContext.Response.StatusCode = 401;
                return null;
            }
        }
    }
}
