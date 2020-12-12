using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace BracketSystem.Core.Models.Entities
{
    public class User : IdentityUser<int>
    {
        public DateTime Created { get; set; }

        public virtual ICollection<Paintballfield> PaintballFields { get; set; } = new List<Paintballfield>();
        public virtual ICollection<Team> Teams { get; set; } = new List<Team>();
        public string TeamName { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}