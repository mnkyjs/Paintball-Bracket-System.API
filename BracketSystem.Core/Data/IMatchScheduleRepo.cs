using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using il_y.BracketSystem.Core.Models.Dtos;
using il_y.BracketSystem.Core.Models.Entities;

namespace il_y.BracketSystem.Core.Data
{
    public interface IMatchScheduleRepo : IGenericRepository<Match>
    {
        Task<List<Match>> DeleteMatches(User user);

        Task<List<BlockDto>> GetMatchesByField(int paintballFieldId);

        Task<List<BlockDto>> GetMatches(User user);

        Task<List<Match>> GetMatchesByUser(int userId);

        Task<List<BlockDto>> GetMatchesByDateAndUser(DateTime dateTime, User user, string name);

        Task<List<Match>> GetMatchesByDateAndUserToDelete(DateTime dateTime, User user, string name);

        Task<List<BlockDto>> GetMatchesByDate(DateTime dateTime, string name);

        Task<List<TeamDto[]>> CreateSchedule(List<TeamDto> teams, User user, string url, DateTime date,
            int paintballFieldId, string clashName, bool addToExistingClashDay);
    }
}