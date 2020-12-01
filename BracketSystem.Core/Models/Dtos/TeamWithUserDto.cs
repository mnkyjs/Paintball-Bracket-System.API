using System.ComponentModel.DataAnnotations;
using il_y.BracketSystem.Core.Models.Entities;

namespace il_y.BracketSystem.Core.Models.Dtos
{
    internal class TeamWithUserDto : BaseDto<Team>
    {
        public TeamWithUserDto()
        {
        }

        public TeamWithUserDto(Team team) : this()
        {
            Id = team.Id;
            FromEntity(team);
        }

        public int Id { get; set; }
        public User User { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "You must specify password between 2 and 50 characters")]
        public string Name { get; set; }

        public static TeamWithUserDto FromEntity(Team entity)
        {
            TeamWithUserDto vm = null;

            if (entity != null)
            {
                vm = new TeamWithUserDto();
                CopyProperties(entity, vm);
            }

            return vm;
        }

        public override void UpdateEntity(Team entity)
        {
            Name = entity.Name;
            User = entity.Creator;
        }
    }
}