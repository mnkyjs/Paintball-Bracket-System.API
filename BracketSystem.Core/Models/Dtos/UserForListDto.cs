using BracketSystem.Core.Models.Entities;
using System;

namespace BracketSystem.Core.Models.Dtos
{
    public class UserForListDto 
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public DateTime Created { get; set; }
        public Role Role { get; set; }
        public string TeamName { get; set; }

        public static UserForListDto FromEntity(User entity)
        {
            return new UserForListDto
            {
                Id = entity.Id,
                UserName = entity.UserName,
                Created = entity.Created,
                TeamName = entity.TeamName
            };
        }
    }
}