using BracketSystem.Core.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace BracketSystem.Core.Models.Dtos
{
    public class TeamDto
    {
        public int? Id { get; set; }
        public UserFlatDto User { get; set; } = new UserFlatDto();

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "You must specify password between 2 and 50 characters")]
        public string Name { get; set; }
        
        public static TeamDto FromEntity(Team entity)
        {
            return new TeamDto
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }
    }
}