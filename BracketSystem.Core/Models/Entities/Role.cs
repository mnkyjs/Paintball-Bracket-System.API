using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace il_y.BracketSystem.Core.Models.Entities
{
    public class Role : IdentityRole<int>
    {
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}