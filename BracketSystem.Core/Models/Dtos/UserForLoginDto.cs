using BracketSystem.Core.Models.Entities;

namespace BracketSystem.Core.Models.Dtos
{
    public class UserForLoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public static UserForLoginDto FromEntity(User entity)
        {
            return new UserForLoginDto
            {
                Username = entity.UserName
            };
        }
    }
}