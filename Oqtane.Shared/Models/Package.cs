using System;

namespace Oqtane.Models
{
    /// <summary>
    /// A software Package for installation. 
    /// </summary>
    public class Package
    {
        /// <summary>
        /// ID of the Package. Usually constructed of the Namespace.
        /// </summary>
        public string PackageId { get; set; }

        /// <summary>
        /// Owner of the package
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// Url for the owner of the package
        /// </summary>
        public string OwnerUrl { get; set; }

        /// <summary>
        /// Friendly name of the package
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the Package. 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Friendly name of the package
        /// </summary>
        public string ProductUrl { get; set; }

        /// <summary>
        /// Version as defined in the NuGet package. 
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Download count to show how popular the package is. 
        /// </summary>
        public long Downloads { get; set; }

        /// <summary>
        /// date the package was released 
        /// </summary>
        public DateTime ReleaseDate { get; set; }

        /// <summary>
        /// The direct Url for downloading the package 
        /// </summary>
        public string PackageUrl { get; set; }

        /// <summary>
        /// Indicates if any known security vulnerabilities exist 
        /// </summary>
        public int Vulnerabilities { get; set; }
    }
}
