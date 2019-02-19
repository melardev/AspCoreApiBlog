using System;
using Microsoft.AspNetCore.Identity;

namespace BlogDotNet.Entities
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole(string roleName) : base(roleName)
        {
        }

        public ApplicationRole(string roleName, string description,
            DateTime createdAt)
            : base(roleName)
        {
            base.Name = roleName;

            this.Description = description;
            this.CreatedAt = createdAt;
        }

        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual IdentityRole<string> Role { get; set; }
    }
}