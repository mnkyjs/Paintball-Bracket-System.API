using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using il_y.BracketSystem.Core.Data.Repositories;
using il_y.BracketSystem.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace il_y.BracketSystem.Core.Data
{
    public interface IUserRepo : IGenericRepository<User>
    {
        Task<IEnumerable<object>> UserListWithRoles();
    }

    public class UserRepo : GenericRepository<User>, IUserRepo
    {
        #region Constructors

        public UserRepo(DbContext dbContext) : base(dbContext)
        {
        }

        #endregion Constructors

        #region BracketContext

        public BracketContext BracketContext => DbContext as BracketContext;

        #endregion BracketContext

        public async Task<IEnumerable<object>> UserListWithRoles()
        {
            var users = await BracketContext.Users.OrderBy(x => x.UserName).Select(
                user => new
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Created = user.Created,
                    TeamName = user.TeamName,
                    Roles = (from userRole in user.UserRoles
                        join role in BracketContext.Roles
                            on userRole.RoleId
                            equals role.Id
                        select role.Name).ToList()
                }).ToListAsync();
            
            return users;
        }
    }
}