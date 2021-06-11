using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oqtane.Models
{
    /// <summary>
    /// Describes a User in Oqtane.
    /// </summary>
    public class User : IAuditable, IDeletable
    {
        /// <summary>
        /// ID of this User.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Username used for login.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Name shown in menus / dialogs etc.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// User E-Mail address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Reference to a <see cref="File"/> containing the users photo.
        /// </summary>
        public int? PhotoFileId { get; set; }

        /// <summary>
        /// Timestamp of last login.
        /// </summary>
        public DateTime? LastLoginOn { get; set; }

        /// <summary>
        /// Tracking information of IP used when the user last worked on this site. 
        /// </summary>
        public string LastIPAddress { get; set; }

        /// <summary>
        /// Reference to the <see cref="Site"/> this user belongs to.
        /// </summary>
        [NotMapped]
        public int SiteId { get; set; }

        /// <summary>
        /// Role names this user has.
        /// TODO: todoc - is this comma separated?
        /// </summary>
        [NotMapped]
        public string Roles { get; set; }

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
        
        #region Extended IAuditable Properties, may be moved to an Interface some day so not documented yet

        public string DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool IsDeleted { get; set; }

        #endregion

        /// <summary>
        /// The users password. Note that this is not plaintext, so you can probably never really work with this. 
        /// </summary>
        [NotMapped]
        public string Password { get; set; }

        /// <summary>
        /// Information if this user is authenticated. Anonymous users are not authenticated.
        /// </summary>
        [NotMapped]
        public bool IsAuthenticated { get; set; }
    }
}
