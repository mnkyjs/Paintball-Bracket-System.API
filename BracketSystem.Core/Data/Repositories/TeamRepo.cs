using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using il_y.BracketSystem.Core.Models;
using il_y.BracketSystem.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace il_y.BracketSystem.Core.Data.Repositories
{
    public class TeamRepo : GenericRepository<Team>, ITeamRepo
    {
        #region Constructors

        public TeamRepo(BracketContext context)
            : base(context)
        {
        }

        #endregion Constructors

        #region BracketContext

        public BracketContext BracketContext => DbContext as BracketContext;

        #endregion BracketContext

        #region Methods

        public async Task<List<Team>> GetAllRecordsFromDatabase()
        {
            var tempTeams = await BracketContext.Teams.Include(u => u.Creator).ToListAsync();

            // Log.Information("Just a small logging Test {Data}", teams);

            return tempTeams.Where(team => team.Name != "pause").ToList();
        }

        public async Task<PagedResult<Team>> FindTeams(int page = 1, int pageSize = 10, string filter = null,
            string sortColumn = "Name",
            string sortOrder = "asc")
        {
            sortColumn = string.IsNullOrEmpty(sortColumn) ? "Name" : sortColumn;
            bool sortAscending = string.IsNullOrEmpty(sortOrder) || sortOrder == "asc";
            IQueryable<Team> query;
            Task<int> count;
            
            

            if (string.IsNullOrEmpty(filter))
            {
                count = BracketContext.Teams.CountAsync();
                query = BracketContext.Teams.Where(team => team.Name != "pause");
            }
            else
            {
                filter = filter.ToLower();
                count = BracketContext.Teams
                    .Where(p => p.Name.Contains(filter))
                    .CountAsync();
                query = BracketContext.Teams
                    .Where(p => p.Name.Contains(filter) && p.Name != "pause");
            }

            // Order result set
            query = sortAscending ? query.OrderBy(sortColumn) : query.OrderByDescending(sortColumn);

            // Apply paging
            var items = query.Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Team>(await items, await count, page, pageSize);
        }

        #endregion Methods
    }
}