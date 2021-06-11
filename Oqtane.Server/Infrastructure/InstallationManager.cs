using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Oqtane.Shared;
// ReSharper disable AssignNullToNotNullAttribute

namespace Oqtane.Infrastructure
{
    public class InstallationManager : IInstallationManager
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IWebHostEnvironment _environment;

        public InstallationManager(IHostApplicationLifetime hostApplicationLifetime, IWebHostEnvironment environment)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _environment = environment;
        }

        public void InstallPackages()
        {
            if (!InstallPackages(_environment.WebRootPath, _environment.ContentRootPath))
            {
                // error installing packages
            }
        }

        public static bool InstallPackages(string webRootPath, string contentRootPath)
        {
            bool install = false;
            string binPath = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);

            string sourceFolder = Path.Combine(contentRootPath, "Packages");
            if (!Directory.Exists(sourceFolder))
            {
                Directory.CreateDirectory(sourceFolder);
            }

            // move packages to secure /Packages folder
            foreach (var folder in "Modules,Themes,Packages".Split(","))
            {
                foreach(var file in Directory.GetFiles(Path.Combine(webRootPath, folder), "*.nupkg*"))
                {
                    var destinationFile = Path.Combine(sourceFolder, Path.GetFileName(file));
                    if (File.Exists(destinationFile))
                    {
                        File.Delete(destinationFile);
                    }
                    if (destinationFile.ToLower().EndsWith(".nupkg.bak"))
                    {
                        // leave a copy in the current folder as it is distributed with the core framework
                        File.Copy(file, destinationFile);
                    }
                    else
                    {
                        // move to destination
                        File.Move(file, destinationFile);
                    }
                }
            }

            // iterate through Nuget packages in source folder
            foreach (string packagename in Directory.GetFiles(sourceFolder, "*.nupkg"))
            {
                // iterate through files
                using (ZipArchive archive = ZipFile.OpenRead(packagename))
                {
                    string frameworkversion = "";
                    // locate nuspec
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (entry.FullName.ToLower().EndsWith(".nuspec"))
                        {
                            // open nuspec
                            XmlTextReader reader = new XmlTextReader(entry.Open());
                            reader.Namespaces = false; // remove namespace
                            XmlDocument doc = new XmlDocument();
                            doc.Load(reader);
                            // get framework dependency
                            XmlNode node = doc.SelectSingleNode("/package/metadata/dependencies/dependency[@id='Oqtane.Framework']");
                            if (node != null)
                            {
                                frameworkversion = node.Attributes["version"].Value;
                            }
                            reader.Close();
                            break;
                        }
                    }

                    // if compatible with framework version
                    if (frameworkversion == "" || Version.Parse(Constants.Version).CompareTo(Version.Parse(frameworkversion)) >= 0)
                    {
                        List<string> assets = new List<string>();
                        bool manifest = false;
                        string name = Path.GetFileNameWithoutExtension(packagename);

                        // deploy to appropriate locations
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            string filename = "";

                            // evaluate entry root folder
                            switch (entry.FullName.Split('/')[0])
                            {
                                case "lib": // lib/net5.0/...
                                    filename = ExtractFile(entry, binPath, 2);
                                    break;
                                case "wwwroot": // wwwroot/...
                                    filename = ExtractFile(entry, webRootPath, 1);
                                    break;
                                case "runtimes": // runtimes/name/...
                                    filename = ExtractFile(entry, binPath, 0);
                                    break;
                                case "ref": // ref/net5.0/...
                                    filename = ExtractFile(entry, Path.Combine(binPath, "ref"), 2);
                                    break;
                            }

                            if (filename != "")
                            {
                                // ContentRootPath sometimes produces inconsistent path casing - so can't use string.Replace()
                                filename = Regex.Replace(filename, Regex.Escape(contentRootPath), "", RegexOptions.IgnoreCase);
                                assets.Add(filename);
                                if (!manifest && Path.GetExtension(filename) == ".log")
                                {
                                    manifest = true;
                                }
                            }
                        }

                        // save dynamic list of assets
                        if (!manifest && assets.Count != 0)
                        {
                            string manifestpath = Path.Combine(sourceFolder, name + ".log");
                            if (File.Exists(manifestpath))
                            {
                                File.Delete(manifestpath);
                            }
                            if (!Directory.Exists(Path.GetDirectoryName(manifestpath)))
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(manifestpath));
                            }
                            File.WriteAllText(manifestpath, JsonSerializer.Serialize(assets, new JsonSerializerOptions { WriteIndented = true }));
                        }
                    }
                }

                // remove package
                File.Delete(packagename);
                install = true;
            }

            return install;
        }

        private static string ExtractFile(ZipArchiveEntry entry, string folder, int ignoreLeadingSegments)
        {
            string[] segments = entry.FullName.Split('/'); // ZipArchiveEntries always use unix path separator
            string filename = Path.Combine(folder, string.Join(Path.DirectorySeparatorChar, segments, ignoreLeadingSegments, segments.Length - ignoreLeadingSegments));

            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(filename)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filename));
                }
                entry.ExtractToFile(filename, true);
            }
            catch
            {
                // an error occurred extracting the file
                filename = "";
            }
            return filename;
        }

        public bool UninstallPackage(string PackageName)
        {
            // get manifest with highest version
            string packagename = "";
            string[] packages = Directory.GetFiles(Path.Combine(_environment.ContentRootPath, "Packages"), PackageName + "*.log");
            if (packages.Length > 0)
            {
                packagename = packages[packages.Length - 1]; // use highest version 
            }

            if (!string.IsNullOrEmpty(packagename))
            {
                // use manifest to clean up file resources
                List<string> assets = JsonSerializer.Deserialize<List<string>>(File.ReadAllText(packagename));
                assets.Reverse();
                foreach (string asset in assets)
                {
                    // legacy support for assets that were stored as absolute paths
                    string filepath = asset.StartsWith("\\") ? Path.Combine(_environment.ContentRootPath, asset.Substring(1)) : asset;
                    if (File.Exists(filepath))
                    {
                        File.Delete(filepath);
                        if (!Directory.EnumerateFiles(Path.GetDirectoryName(filepath)).Any())
                        {
                            Directory.Delete(Path.GetDirectoryName(filepath));
                        }
                    }
                }

                // clean up package asset manifests
                foreach(string asset in packages)
                {
                    File.Delete(asset);
                }

                return true;
            }
            return false;
        }

        public void UpgradeFramework()
        {
            string folder = Path.Combine(_environment.ContentRootPath, "Packages");
            if (Directory.Exists(folder))
            {
                // get package with highest version
                string packagename = "";
                string[] packages = Directory.GetFiles(folder, Constants.PackageId + ".*.nupkg");
                if (packages.Length > 0)
                {
                    packagename = packages[packages.Length - 1]; // use highest version 
                }

                if (packagename != "")
                {
                    // verify package version
                    string packageversion = "";
                    string packageurl = "";
                    using (ZipArchive archive = ZipFile.OpenRead(packagename))
                    {
                        // locate nuspec
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            if (entry.FullName.ToLower().EndsWith(".nuspec"))
                            {
                                // open nuspec
                                XmlTextReader reader = new XmlTextReader(entry.Open());
                                reader.Namespaces = false; // remove namespace
                                XmlDocument doc = new XmlDocument();
                                doc.Load(reader);
                                // get framework version
                                XmlNode node = doc.SelectSingleNode("/package/metadata/version");
                                if (node != null)
                                {
                                    packageversion = node.InnerText;
                                }
                                node = doc.SelectSingleNode("/package/metadata/projectUrl");
                                if (node != null)
                                {
                                    packageurl = node.InnerText;
                                }
                                reader.Close();
                                break;
                            }
                        }
                    }

                    // ensure package version is greater than or equal to current framework version
                    if (packageversion != "" && Version.Parse(Constants.Version).CompareTo(Version.Parse(packageversion)) <= 0 && packageurl != "")
                    {
                        // install Oqtane.Framework and Oqtane.Updater nuget packages
                        InstallPackages();
                        // download upgrade zip package
                        var client = new WebClient();
                        Uri uri = new Uri(packageurl);
                        string upgradepackage = Path.Combine(folder, uri.Segments[uri.Segments.Length - 1]);
                        client.DownloadFile(packageurl, upgradepackage);
                        // install Oqtane.Upgrade zip package
                        if (File.Exists(upgradepackage))
                        {
                            FinishUpgrade();
                        }
                    }
                }
            }
        }

        private void FinishUpgrade()
        {
            // check if updater application exists
            string Updater = Constants.UpdaterPackageId + ".dll";
            string folder = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
            if (folder == null || !File.Exists(Path.Combine(folder, Updater))) return;

            // run updater application
            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = folder,
                    FileName = "dotnet",
                    Arguments = Path.Combine(folder, Updater) + " \"" + _environment.ContentRootPath + "\" \"" + _environment.WebRootPath + "\"",
                    UseShellExecute = false,
                    ErrorDialog = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false
                };
                process.Start();
            };
        }

        public void RestartApplication()
        {
            _hostApplicationLifetime.StopApplication();
        }
    }
}
