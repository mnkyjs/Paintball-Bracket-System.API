using BracketSystem.Core.Models.Entities;
using System;

namespace BracketSystem.Core.Models.Dtos
{
    public class MatchDto : BaseDto<Match>
    {
        public MatchDto()
        {
        }

        public MatchDto(Match match) : this()
        {
            Id = match.Id;
            FromEntity(match);
        }

        public int Id { get; set; }
        public Team TeamA { get; set; }
        public Team TeamB { get; set; }
        public User User { get; set; }
        public string MatchName { get; set; }
        public bool IsShared { get; set; }
        public DateTime Date { get; set; }

        public override void UpdateEntity(Match entity)
        {
            entity.Date = Date;
            entity.TeamA = TeamA;
            entity.TeamB = TeamB;
            entity.MatchName = MatchName;
        }

        public static MatchDto FromEntity(Match entity)
        {
            MatchDto vm = null;

            if (entity != null)
            {
                vm = new MatchDto();
                CopyProperties(entity, vm);
            }

            return vm;
        }
    }
}