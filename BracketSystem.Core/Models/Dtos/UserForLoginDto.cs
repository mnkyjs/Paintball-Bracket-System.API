using il_y.BracketSystem.Core.Models.Entities;

namespace il_y.BracketSystem.Core.Models.Dtos
{
    public class UserForLoginDto : BaseDto<User>
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public static UserForLoginDto FromEntity(User entity)
        {
            UserForLoginDto vm = null;

            if (entity != null)
            {
                vm = new UserForLoginDto();
                CopyProperties(entity, vm);
            }

            return vm;
        }

        public override void UpdateEntity(User entity)
        {
            entity.UserName = Username;
        }
    }
}