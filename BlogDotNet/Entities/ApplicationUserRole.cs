using System;
using Microsoft.AspNetCore.Identity;

namespace BlogDotNet.Entities
{
    public class ApplicationUserRole : IdentityUserRole<Guid>
    {
        public ApplicationRole Role { get; set; }
        public ApplicationUser User { get; set; }
    }
}