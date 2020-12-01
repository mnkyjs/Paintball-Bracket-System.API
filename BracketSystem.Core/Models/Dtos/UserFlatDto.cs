using il_y.BracketSystem.Core.Models.Entities;

namespace il_y.BracketSystem.Core.Models.Dtos
{
    public class UserFlatDto : BaseDto<User>
    {
        public UserFlatDto()
        {
        }

        public UserFlatDto(User user) : this()
        {
            FromEntity(user);
        }

        public string TeamName { get; set; }
        public string UserName { get; set; }

        public static UserFlatDto FromEntity(User entity)
        {
            UserFlatDto vm = null;

            if (entity != null)
            {
                vm = new UserFlatDto();
                CopyProperties(entity, vm);
            }

            return vm;
        }

        public override void UpdateEntity(User entity)
        {
            entity.UserName = UserName;
            entity.TeamName = TeamName;
        }
    }
}