using System;
using il_y.BracketSystem.Core.Models.Entities;

namespace il_y.BracketSystem.Core.Models.Dtos
{
    public class UserWithRolesDto : BaseDto<User>
    {
        public UserWithRolesDto()
        {
        }

        public UserWithRolesDto(User user) : this()
        {
            Id = user.Id;
            FromEntity(user);
        }

        public int Id { get; set; }
        public string UserName { get; set; }
        public DateTime Created { get; set; }
        public string[] RoleNames { get; set; }
        public string TeamName { get; set; }

        public static UserForListDto FromEntity(User entity)
        {
            UserForListDto vm = null;

            if (entity != null)
            {
                vm = new UserForListDto();
                CopyProperties(entity, vm);
            }

            return vm;
        }

        public override void UpdateEntity(User entity)
        {
            throw new NotImplementedException();
        }
    }
}