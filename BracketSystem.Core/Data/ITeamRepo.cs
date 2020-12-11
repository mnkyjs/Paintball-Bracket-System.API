
using BracketSystem.Core.Models;
using BracketSystem.Core.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BracketSystem.Core.Data
{
    #region Interfaces

    public interface ITeamRepo : IGenericRepository<Team>
    {
        #region Methods

        Task<PagedResult<Team>> FindTeams(int page = 1, int pageSize = 10,
            string filter = null, string sortColumn = "Name", string sortOrder = "asc");

        Task<List<Team>> GetAllRecordsFromDatabase();
        #endregion
    }
}

#endregion