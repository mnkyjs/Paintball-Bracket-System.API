using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using il_y.BracketSystem.Core.Models.Dtos;
using il_y.BracketSystem.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace il_y.BracketSystem.Core.Data.Repositories
{
    public class MatchScheduleRepo : GenericRepository<Match>, IMatchScheduleRepo
    {
        public MatchScheduleRepo(DbContext context)
            : base(context)
        {
        }

        #region BracketContext

        public BracketContext BracketContext => DbContext as BracketContext;

        #endregion BracketContext

        public async Task<List<TeamDto[]>> CreateSchedule(List<TeamDto> teams, User user, string url, DateTime date,
            int paintballFieldId, string clashName, bool addToExistingClashDay = false)
        {
            var matches = new List<TeamDto[]>();

            var tempTeam = BracketContext.Teams.FirstOrDefaultAsync(n => n.Name == "pause");

            if (teams.Count % 2 == 1)
                teams.Add(new TeamDto
                {
                    Id = tempTeam.Result.Id,
                    Name = tempTeam.Result.Name
                });

            for (var i = 0; i < teams.Count - 1; i++)
            {
                var mid = teams.Count / 2;
                var teamListOne = teams.GetRange(0, mid);
                var teamListTwo = teams.GetRange(mid, teams.Count - mid);

                teamListOne.Reverse();

                if (i % 2 == 0)
                    for (var f = 0; f <= teamListOne.Count - 1; f++)
                    {
                        var match = new TeamDto[2];
                        match[0] = teamListOne[f];
                        match[1] = teamListTwo[f];

                        matches.Add(match);
                    }
                else
                    for (var f = 0; f <= teamListOne.Count - 1; f++)
                    {
                        var match = new TeamDto[2];
                        match[1] = teamListOne[f];
                        match[0] = teamListTwo[f];

                        matches.Add(match);
                    }

                var tempCounterToRemove = teams.Count - 1;
                var teamToAddAtIndexOne = teams.ElementAt(tempCounterToRemove);
                teams.RemoveAt(tempCounterToRemove);
                teams.Insert(1, teamToAddAtIndexOne);
            }

            await SaveMatches(matches, user, addToExistingClashDay, date, paintballFieldId, clashName, url);
            return matches;
        }

        public async Task<List<Match>> DeleteMatches(User user)
        {
            var allMatches = await BracketContext.Matches.Where(u => u.User.Id == user.Id).ToListAsync();
            return allMatches;
        }

        public async Task<List<BlockDto>> GetMatches(User user)
        {
            var matches = await BracketContext.Matches.Where(u => u.User == user).Include(a => a.TeamA).Include(b => b.TeamB).ToListAsync();
            var listOfMatches = ShowMatches(matches);
            return await listOfMatches;
        }

        public async Task<List<BlockDto>> GetMatchesByDateAndUser(DateTime dateTime, User user, string name)
        {
            var matches = BracketContext.Matches.Where(x => x.Date == dateTime).Include(a => a.TeamA).Include(b => b.TeamB).Where(u => u.User == user)
                .Where(n => n.MatchName == name).ToList();
            var listOfMatchesWithDate = await ShowMatches(matches);
            return listOfMatchesWithDate;
        }

        public async Task<List<Match>> GetMatchesByDateAndUserToDelete(DateTime dateTime, User user, string name)
        {
            var matches = BracketContext.Matches.Where(x => x.Date == dateTime).Where(u => u.User == user)
                .Where(n => n.MatchName == name)
                .ToListAsync();
            return await matches;
        }

        public async Task<List<BlockDto>> GetMatchesByDate(DateTime dateTime, string name)
        {
            var matches = BracketContext.Matches.Include(u => u.User).Include(a => a.TeamA).Include(b => b.TeamB).Where(x => x.Date == dateTime).Where(n => n.MatchName == name)
                .ToList();
            var listOfMatchesWithDate = await ShowMatches(matches);
            return listOfMatchesWithDate;
        }

        public async Task<List<BlockDto>> GetMatchesByField(int paintballFieldId)
        {
            var matches = await BracketContext.Matches.Include(f => f.Paintballfield).Include(a => a.TeamA).Include(b => b.TeamB)
                .Where(li => li.PaintballfieldId == paintballFieldId)
                .OrderBy(f => f.Date).ToListAsync();

            var listOfMatches = ShowMatches(matches);
            return await listOfMatches;
        }

        public async Task<List<Match>> GetMatchesByUser(int userId)
        {
            var matches = await BracketContext.Matches.Where(u => u.User.Id == userId).OrderBy(dt => dt.Date)
                .ToListAsync();
            //var listOfMatchesWhichHaveCurrentUser = ShowMatches(matches);
            return matches;
        }

        private async Task ClearMatchDay(DateTime today, User currentUser)
        {
            await BracketContext.Matches.Where(t => t.Date == today).Where(u => u.User == currentUser)
                .ForEachAsync(t => BracketContext.Matches.Remove(t));
        }

        private async Task SaveMatches(IEnumerable<TeamDto[]> matches, User user, bool addToExistingClashDay,
            DateTime date,
            int paintballFieldId, string clashName, string url)
        {
            // if (!addToExistingClashDay) ClearMatchDay(date, user).Wait();

            var dbMatches = new List<Match>();

            foreach (var item in matches)
            {
                var dayMatch = new Match
                {
                    TeamA = new Team {Id = item[0].Id, Name = item[0].Name},
                    TeamB = new Team {Id = item[1].Id, Name = item[1].Name},
                    Date = date.Date.AddDays(1),
                    User = new User {Id = user.Id, UserName = user.UserName},
                    RandomUrl = url,
                    MatchName = clashName,
                    PaintballfieldId = paintballFieldId
                };
                BracketContext.Entry(dayMatch).State = EntityState.Added;
                dbMatches.Add(dayMatch);
            }

            await BracketContext.Matches.AddRangeAsync(dbMatches);
        }

        private async Task<List<BlockDto>> ShowMatches(List<Match> dbMatches)
        {
            List<BlockDto> matches = new List<BlockDto>();
            int blockNumber = 1;

            for (int i = 0; i < dbMatches.Count; i += 2)
            {
                var match = new BlockDto
                {
                    BlockNumber = blockNumber,
                    Games = new List<string>
                    {
                        $"{dbMatches[i].TeamA.Name} vs. {dbMatches[i].TeamB.Name}",
                        $"{dbMatches[i+1].TeamA.Name} vs. {dbMatches[i+1].TeamB.Name}",
                    },
                };

                blockNumber++;

                matches.Add(match);
            }

            return matches;
        }
    }
}