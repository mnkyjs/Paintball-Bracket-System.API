using Microsoft.AspNetCore.Identity;

namespace il_y.BracketSystem.Core.Models.Entities
{
    public class UserRole : IdentityUserRole<int>
    {
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
}