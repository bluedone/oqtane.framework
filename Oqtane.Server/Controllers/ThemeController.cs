using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Models;
using Microsoft.AspNetCore.Authorization;
using Oqtane.Shared;
using System.IO;
using System.Reflection;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Repository;
using System.Text.Json;

// ReSharper disable StringIndexOfIsCultureSpecific.1

namespace Oqtane.Controllers
{
    [Route(ControllerRoutes.Default)]
    public class ThemeController : Controller
    {
        private readonly IThemeRepository _themes;
        private readonly IInstallationManager _installationManager;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogManager _logger;

        public ThemeController(IThemeRepository themes, IInstallationManager installationManager, IWebHostEnvironment environment, ILogManager logger)
        {
            _themes = themes;
            _installationManager = installationManager;
            _environment = environment;
            _logger = logger;
        }

        // GET: api/<controller>
        [HttpGet]
        [Authorize(Roles = RoleNames.Registered)]
        public IEnumerable<Theme> Get()
        {
            return _themes.GetThemes();
        }

        [HttpGet("install")]
        [Authorize(Roles = RoleNames.Host)]
        public void InstallThemes()
        {
            _logger.Log(LogLevel.Information, this, LogFunction.Create, "Themes Installed");
            _installationManager.InstallPackages("Themes");
        }

        // DELETE api/<controller>/xxx
        [HttpDelete("{themename}")]
        [Authorize(Roles = RoleNames.Host)]
        public void Delete(string themename)
        {
            List<Theme> themes = _themes.GetThemes().ToList();
            Theme theme = themes.Where(item => item.ThemeName == themename).FirstOrDefault();
            if (theme != null && Utilities.GetAssemblyName(theme.ThemeName) != "Oqtane.Client")
            {
                // remove theme assets
                string assetpath = Path.Combine(_environment.WebRootPath, "Themes", Utilities.GetTypeName(theme.ThemeName));
                if (System.IO.File.Exists(Path.Combine(assetpath, "assets.json")))
                {
                    // use assets.json to clean up file resources
                    List<string> assets = JsonSerializer.Deserialize<List<string>>(System.IO.File.ReadAllText(Path.Combine(assetpath, "assets.json")));
                    assets.Reverse();
                    foreach (string asset in assets)
                    {
                        // legacy support for assets that were stored as absolute paths
                        string filepath = (asset.StartsWith("\\")) ? Path.Combine(_environment.ContentRootPath, asset.Substring(1)) : asset;
                        if (System.IO.File.Exists(filepath))
                        {
                            System.IO.File.Delete(filepath);
                            if (!Directory.EnumerateFiles(Path.GetDirectoryName(filepath)).Any())
                            {
                                Directory.Delete(Path.GetDirectoryName(filepath));
                            }
                        }
                    }
                    _logger.Log(LogLevel.Information, this, LogFunction.Delete, "Theme Assets Removed For {ThemeName}", theme.ThemeName);
                }
                else
                {
                    // attempt to delete assemblies based on naming convention
                    foreach (string asset in Directory.GetFiles(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), Utilities.GetTypeName(theme.ThemeName) + "*.*"))
                    {
                        System.IO.File.Delete(asset);
                    }
                    _logger.Log(LogLevel.Warning, this, LogFunction.Delete, "Theme Assets Removed For {ThemeName}. Please Note That Some Assets May Have Been Missed Due To A Missing Asset Manifest. An Asset Manifest Is Only Created If A Theme Is Installed From A Nuget Package.", theme.ThemeName);
                }

                // clean up theme static resource folder
                string folder = Path.Combine(_environment.WebRootPath, "Themes" , Utilities.GetTypeName(theme.ThemeName));
                if (Directory.Exists(folder))
                {
                    Directory.Delete(folder, true);
                    _logger.Log(LogLevel.Information, this, LogFunction.Delete, "Theme Static Resource Folder Removed For {ThemeName}", theme.ThemeName);
                }

                // remove theme
                _themes.DeleteTheme(theme.ThemeName);
                _logger.Log(LogLevel.Information, this, LogFunction.Delete, "Theme Removed For {ThemeName}", theme.ThemeName);
            }
        }

        // GET: api/<controller>/templates
        [HttpGet("templates")]
        [Authorize(Roles = RoleNames.Host)]
        public List<string> GetTemplates()
        {
            var templates = new List<string>();
            string templatePath = Utilities.PathCombine(_environment.WebRootPath, "Themes", "Templates", Path.DirectorySeparatorChar.ToString());
            foreach (string directory in Directory.GetDirectories(templatePath))
            {
                templates.Add(directory.Replace(templatePath, ""));
            }
            return templates;
        }

        // POST api/<controller>
        [HttpPost]
        [Authorize(Roles = RoleNames.Host)]
        public Theme Post([FromBody] Theme theme)
        {
            if (ModelState.IsValid)
            {
                string rootPath;
                DirectoryInfo rootFolder = Directory.GetParent(_environment.ContentRootPath);
                string templatePath = Utilities.PathCombine(_environment.WebRootPath, "Themes", "Templates", theme.Template, Path.DirectorySeparatorChar.ToString());

                rootPath = Utilities.PathCombine(rootFolder.Parent.FullName, theme.Owner + "." + theme.Name, Path.DirectorySeparatorChar.ToString());
                theme.ThemeName = theme.Owner + "." + theme.Name + ", " + theme.Owner + "." + theme.Name + ".Client.Oqtane";

                ProcessTemplatesRecursively(new DirectoryInfo(templatePath), rootPath, rootFolder.Name, templatePath, theme);
                _logger.Log(LogLevel.Information, this, LogFunction.Create, "Theme Created {Theme}", theme);
            }

            return theme;
        }

        private void ProcessTemplatesRecursively(DirectoryInfo current, string rootPath, string rootFolder, string templatePath, Theme theme)
        {
            // process folder
            string folderPath = Utilities.PathCombine(rootPath, current.FullName.Replace(templatePath, ""));
            folderPath = folderPath.Replace("[Owner]", theme.Owner);
            folderPath = folderPath.Replace("[Theme]", theme.Name);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            FileInfo[] files = current.GetFiles("*.*");
            if (files != null)
            {
                foreach (FileInfo file in files)
                {
                    // process file
                    string filePath = Path.Combine(folderPath, file.Name);
                    filePath = filePath.Replace("[Owner]", theme.Owner);
                    filePath = filePath.Replace("[Theme]", theme.Name);

                    string text = System.IO.File.ReadAllText(file.FullName);
                    text = text.Replace("[Owner]", theme.Owner);
                    text = text.Replace("[Theme]", theme.Name);
                    text = text.Replace("[RootPath]", rootPath);
                    text = text.Replace("[RootFolder]", rootFolder);
                    text = text.Replace("[Folder]", folderPath);
                    text = text.Replace("[File]", Path.GetFileName(filePath));
                    if (theme.Version == "local")
                    {
                        text = text.Replace("[FrameworkVersion]", Constants.Version);
                        text = text.Replace("[ClientReference]", "<Reference Include=\"Oqtane.Client\"><HintPath>..\\..\\oqtane.framework\\Oqtane.Server\\bin\\Debug\\net5.0\\Oqtane.Client.dll</HintPath></Reference>");
                        text = text.Replace("[SharedReference]", "<Reference Include=\"Oqtane.Shared\"><HintPath>..\\..\\oqtane.framework\\Oqtane.Server\\bin\\Debug\\net5.0\\Oqtane.Shared.dll</HintPath></Reference>");
                    }
                    else
                    {
                        text = text.Replace("[FrameworkVersion]", theme.Version);
                        text = text.Replace("[ClientReference]", "<PackageReference Include=\"Oqtane.Client\" Version=\"" + theme.Version + "\" />");
                        text = text.Replace("[SharedReference]", "<PackageReference Include=\"Oqtane.Shared\" Version=\"" + theme.Version + "\" />");
                    }
                    System.IO.File.WriteAllText(filePath, text);
                }

                DirectoryInfo[] folders = current.GetDirectories();

                foreach (DirectoryInfo folder in folders.Reverse())
                {
                    ProcessTemplatesRecursively(folder, rootPath, rootFolder, templatePath, theme);
                }
            }
        }
    }
}
