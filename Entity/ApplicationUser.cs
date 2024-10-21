using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace IdentityApp.Entity
{
    public class ApplicationUser: IdentityUser
    {
        public string? FullName { get; set; }
        public ICollection<Post> Posts{ get; set; }
    }
}