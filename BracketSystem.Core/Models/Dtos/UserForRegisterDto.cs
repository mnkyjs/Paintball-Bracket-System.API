using BracketSystem.Core.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace BracketSystem.Core.Models.Dtos
{
    public class UserForRegisterDto : BaseDto<User>
    {
        [Required]
        [StringLength(72, MinimumLength = 4, ErrorMessage = "Du musst ein Password mit 4 bis 12 Zeichen verwenden!")]
        public string Password { get; set; }

        public string TeamName { get; set; }

        [Required] public string UserName { get; set; }
        public override void UpdateEntity(User entity)
        {
            entity.UserName = UserName;
            entity.TeamName = TeamName;
        }
    }
}