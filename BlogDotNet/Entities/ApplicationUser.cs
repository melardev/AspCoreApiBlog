using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BlogDotNet.Models;
using Microsoft.AspNetCore.Identity;

namespace BlogDotNet.Entities
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        // Not required if gives error, remove it

        [StringLength(160)] public string ProfileImage { get; set; }


        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string Bio { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
       

        public ICollection<Article> Articles { get; set; }


        /// <summary>
        /// Navigation property for the roles this user belongs to.
        /// </summary>
        public virtual ICollection<IdentityUserRole<string>> Roles { get; set; } = new List<IdentityUserRole<string>>();


        //public virtual ICollection<IdentityUserRole<int>> Roless { get; } = new List<IdentityUserRole<int>>();

        /// <summary>
        /// Navigation property for the claims this user possesses.
        /// </summary>
        public virtual ICollection<IdentityUserClaim<string>> Claims { get; } = new List<IdentityUserClaim<string>>();

        /// <summary>
        /// Navigation property for this users login accounts.
        /// </summary>
        public virtual ICollection<IdentityUserLogin<string>> Logins { get; } = new List<IdentityUserLogin<string>>();


        public ICollection<Like> Likes { get; set; }

        public virtual ICollection<UserRelation> Followers { get; set; }
        public virtual ICollection<UserRelation> Following { get; set; }

       
    }
}