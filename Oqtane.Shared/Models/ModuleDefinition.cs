using System;
using System.ComponentModel.DataAnnotations.Schema;
using Oqtane.Documentation;

namespace Oqtane.Models
{
    /// <summary>
    /// Describes a Module type (Definition) in Oqtane.
    /// The available Modules are determined at StartUp.
    /// </summary>
    public class ModuleDefinition : IAuditable
    {
        [PrivateApi("The constructor is probably just for internal use and shouldn't appear in the docs")]
        public ModuleDefinition()
        {
            Name = "";
            Description = "";
            Categories = "";
            Version = "";
            Owner = "";
            Url = "";
            Contact = "";
            License = "";
            Dependencies = "";
            PermissionNames = "";
            ServerManagerType = "";
            ControlTypeRoutes = "";
            ReleaseVersions = "";
            DefaultAction = "";
            SettingsType = "";
            PackageName = "";
            Runtimes = "";
            Template = "";
        }

        /// <summary>
        /// Reference to the <see cref="ModuleDefinition"/>.
        /// </summary>
        public int ModuleDefinitionId { get; set; }

        /// <summary>
        /// Name of the <see cref="ModuleDefinition"/>
        /// </summary>
        public string ModuleDefinitionName { get; set; }

        /// <summary>
        /// Nice name to show in admin / edit dialogs.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Module description for admin dialogs.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Categories this Module will be shown in (in the admin-dialogs)
        /// </summary>
        public string Categories { get; set; }

        /// <summary>
        /// Version information of this Module based on the DLL / NuGet package.
        /// </summary>
        public string Version { get; set; }

        #region IAuditable Properties

        /// <inheritdoc/>
        public string CreatedBy { get; set; }
        /// <inheritdoc/>
        public DateTime CreatedOn { get; set; }
        /// <inheritdoc/>
        public string ModifiedBy { get; set; }
        /// <inheritdoc/>
        public DateTime ModifiedOn { get; set; }

        #endregion

        // additional IModule properties 
        [NotMapped]
        public string Owner { get; set; }
        [NotMapped]
        public string Url { get; set; }
        [NotMapped]
        public string Contact { get; set; }
        [NotMapped]
        public string License { get; set; }
        [NotMapped]
        public string Runtimes { get; set; }
        [NotMapped]
        public string Dependencies { get; set; }
        [NotMapped]
        public string PermissionNames { get; set; }
        [NotMapped]
        public string ServerManagerType { get; set; }
        [NotMapped]
        public string ControlTypeRoutes { get; set; }
        [NotMapped]
        public string ReleaseVersions { get; set; }
        [NotMapped]
        public string DefaultAction { get; set; }
        [NotMapped]
        public string SettingsType { get; set; } // added in 2.0.2
        [NotMapped]
        public string PackageName { get; set; } // added in 2.1.0

        // internal properties
        [NotMapped]
        public int SiteId { get; set; }
        [NotMapped]
        public string ControlTypeTemplate { get; set; }
        [NotMapped]
        public string AssemblyName { get; set; }
        [NotMapped]
        public string Permissions { get; set; }
        [NotMapped]
        public string Template { get; set; }
    }
}
