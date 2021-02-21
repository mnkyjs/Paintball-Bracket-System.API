using BracketSystem.Core.Models.Entities;

namespace BracketSystem.Core.Models.Dtos
{
    public class UserFlatDto
    {
        public string TeamName { get; set; }
        public string UserName { get; set; }

        public static UserFlatDto FromEntity(User entity)
        {
            return new UserFlatDto
            {
                TeamName = entity.TeamName,
                UserName = entity.UserName
            };
        }
    }
}