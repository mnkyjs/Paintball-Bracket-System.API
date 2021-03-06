﻿using BracketSystem.Core.Data;
using BracketSystem.Core.Models.Dtos;
using BracketSystem.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BracketSystem.Core.Data.Repositories
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

        public async Task<List<TeamDto[]>> CreateSchedule(List<TeamDto> teams, User user, DateTime date,
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

            await SaveMatches(matches, user, date, paintballFieldId, clashName);
            return matches;
        }

        public async Task<List<Match>> DeleteMatches(User user)
        {
            var allMatches = await BracketContext.Matches.Where(u => u.User.Id == user.Id).ToListAsync();
            return allMatches;
        }

        public async Task<List<TeamDto[]>> GetMatches(User user)
        {
            var matches = await BracketContext.Matches.Where(u => u.User == user).ToListAsync();
            var listOfMatches = ShowMatches(matches);
            return await listOfMatches;
        }

        public async Task<IEnumerable<BlockDto>> GetMatchesByGuid(string guid)
        {
            var matches = BracketContext.Matches.Include(u => u.User).Include(a => a.TeamA).Include(b => b.TeamB)
                .Where(x => x.Guid == guid)
                .ToList();
            var listOfMatchesWithDate = await ShowMatchesForView(matches);
            return listOfMatchesWithDate;
        }

        public async Task<List<Match>> GetMatchesByDateAndUserToDelete(DateTime dateTime, User user, string name)
        {
            var matches = BracketContext.Matches.Where(x => x.Date == dateTime).Where(u => u.User == user)
                .Where(n => n.MatchName == name)
                .ToListAsync();
            return await matches;
        }

        public async Task<List<TeamDto[]>> GetMatchesByField(int paintballFieldId)
        {
            var matches = await BracketContext.Matches.Include(f => f.Paintballfield)
                .Where(li => li.PaintballfieldId == paintballFieldId)
                .OrderBy(f => f.Date).ToListAsync();

            var listOfMatches = ShowMatches(matches);
            return await listOfMatches;
        }

        private async Task<IEnumerable<BlockDto>> ShowMatchesForView(List<Match> dbTeams)
        {
            var result = new List<BlockDto>();
            var counter = 1;
            await Task.Run(() =>
            {
                for (var i = 0; i < dbTeams.Count; i += 2)
                {
                    if (dbTeams.Count % 2 == 1)
                    {
                        if (i == dbTeams.Count - 1)
                        {
                            break;
                        }
                    }

                    var block = new BlockDto
                    {
                        BlockNumber = counter,
                        Games = new List<string>
                        {
                            {$"{dbTeams[i].TeamA.Name} vs {dbTeams[i].TeamB.Name}"},
                            {$"{dbTeams[i + 1].TeamA.Name} vs {dbTeams[i + 1].TeamB.Name}"},
                        }
                    };
                    result.Add(block);
                    counter++;
                }
            });

            return result;
        }

        public async Task<List<Match>> GetMatchesByUser(int userId)
        {
            var matches = await BracketContext.Matches.Where(u => u.User.Id == userId).OrderBy(dt => dt.Date)
                .ToListAsync();
            //var listOfMatchesWhichHaveCurrentUser = ShowMatches(matches);
            return matches;
        }

        private async Task SaveMatches(IEnumerable<TeamDto[]> matches, User user,
            DateTime date,
            int paintballFieldId, string clashName)
        {
            var guid = Guid.NewGuid().ToString();
            var dbMatches = new List<Match>();

            foreach (var item in matches)
            {
                var dayMatch = new Match
                {
                    TeamA = new Team {Id = item[0].Id ?? default(int), Name = item[0].Name},
                    TeamB = new Team {Id = item[1].Id ?? default(int), Name = item[1].Name},
                    Date = date.Date.AddDays(1),
                    User = new User {Id = user.Id, UserName = user.UserName},
                    Guid = guid,
                    MatchName = clashName,
                    PaintballfieldId = paintballFieldId
                };
                BracketContext.Entry(dayMatch).State = EntityState.Added;
                dbMatches.Add(dayMatch);
            }

            await BracketContext.Matches.AddRangeAsync(dbMatches);
        }

        private async Task<List<TeamDto[]>> ShowMatches(IEnumerable<Match> dbMatches)
        {
            var showMatches = new List<TeamDto[]>();

            foreach (var match in dbMatches)
            {
                var userForList = new UserFlatDto
                {
                    UserName = match.User.UserName,
                    TeamName = match.User.TeamName
                };

                var tempMatch = new TeamDto[2];
                var teamA = await BracketContext.Teams.FirstOrDefaultAsync(i => i.Id == match.TeamA.Id);
                var teamADto = new TeamDto
                {
                    Id = teamA.Id,
                    Name = teamA.Name,
                    User = userForList
                };

                var teamB = await BracketContext.Teams.FirstOrDefaultAsync(i => i.Id == match.TeamB.Id);
                var teamBDto = new TeamDto
                {
                    Id = teamB.Id,
                    Name = teamB.Name,
                    User = userForList
                };

                tempMatch[0] = teamADto;
                tempMatch[1] = teamBDto;

                showMatches.Add(tempMatch);
            }

            return showMatches;
        }
    }
}