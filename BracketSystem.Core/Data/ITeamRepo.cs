using System.Collections.Generic;
using System.Threading.Tasks;
using il_y.BracketSystem.Core.Models;
using il_y.BracketSystem.Core.Models.Entities;

namespace il_y.BracketSystem.Core.Data
{
    #region Interfaces

    public interface ITeamRepo : IGenericRepository<Team>
    {
        #region Methods

        Task<List<Team>> GetAllRecordsFromDatabase();

        Task<PagedResult<Team>> FindTeams(int page = 1, int pageSize = 10,
            string filter = null, string sortColumn = "Name", string sortOrder = "asc");

        #endregion
    }
}

#endregion