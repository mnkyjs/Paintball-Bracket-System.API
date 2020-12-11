using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BracketSystem.Core.Models.Entities
{
    [Serializable]
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [ForeignKey("Creator")] public int CreatorId { get; set; }

        public virtual User Creator { get; set; }

        [InverseProperty("TeamA")] public virtual ICollection<Match> MatchesTeamA { get; set; } = new List<Match>();

        [InverseProperty("TeamB")] public virtual ICollection<Match> MatchesTeamB { get; set; } = new List<Match>();
    }
}