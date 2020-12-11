
using BracketSystem.Core.Models.Dtos;
using BracketSystem.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BracketSystem.Core.Data
{
    public interface IMatchScheduleRepo : IGenericRepository<Match>
    {
        Task<List<TeamDto[]>> CreateSchedule(List<TeamDto> teams, User user, string url, DateTime date,
            int paintballFieldId, string clashName, bool addToExistingClashDay);

        Task<List<Match>> DeleteMatches(User user);

        Task<List<TeamDto[]>> GetMatches(User user);

        Task<List<TeamDto[]>> GetMatchesByDate(DateTime dateTime, string name);

        Task<List<Match>> GetMatchesByDateAndUserToDelete(DateTime dateTime, User user, string name);

        Task<List<TeamDto[]>> GetMatchesByField(int paintballFieldId);
        Task<List<Match>> GetMatchesByUser(int userId);
    }
}