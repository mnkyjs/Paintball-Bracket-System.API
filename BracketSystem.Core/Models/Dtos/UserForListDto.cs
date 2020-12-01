using System;
using il_y.BracketSystem.Core.Models.Entities;

namespace il_y.BracketSystem.Core.Models.Dtos
{
    public class UserForListDto : BaseDto<User>
    {
        public UserForListDto()
        {
        }

        public UserForListDto(User user) : this()
        {
            Id = user.Id;
            FromEntity(user);
        }

        public int Id { get; set; }
        public string UserName { get; set; }
        public DateTime Created { get; set; }
        public Role Role { get; set; }
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
            entity.UserName = UserName;
            entity.Created = DateTime.Now;
            entity.TeamName = TeamName ?? string.Empty;
        }
    }
}