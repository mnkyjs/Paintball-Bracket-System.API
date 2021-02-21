using BracketSystem.Core.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace BracketSystem.Core.Models.Dtos
{
    public class UserForRegisterDto 
    {
        [Required]
        [StringLength(72, MinimumLength = 4, ErrorMessage = "Du musst ein Password mit 4 bis 12 Zeichen verwenden!")]
        public string Password { get; set; }

        public string TeamName { get; set; }

        [Required] public string UserName { get; set; }
        public void UpdateEntity(User entity)
        {
            TeamName = entity.TeamName;
            UserName = entity.UserName;
        }
    }
}