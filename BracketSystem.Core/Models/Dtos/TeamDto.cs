using BracketSystem.Core.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace BracketSystem.Core.Models.Dtos
{
    public class TeamDto : BaseDto<Team>
    {
        public TeamDto()
        {
        }

        public TeamDto(Team team) : this()
        {
            Id = team.Id;
            FromEntity(team);
        }

        public int Id { get; set; }
        public UserFlatDto User { get; set; } = new UserFlatDto();

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "You must specify password between 2 and 50 characters")]
        public string Name { get; set; }

        public override void UpdateEntity(Team entity)
        {
            entity.Name = Name;
        }

        public static TeamDto FromEntity(Team entity)
        {
            TeamDto vm = null;

            if (entity != null)
            {
                vm = new TeamDto();
                CopyProperties(entity, vm);
            }

            return vm;
        }
    }
}