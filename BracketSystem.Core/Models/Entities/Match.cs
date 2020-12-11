using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BracketSystem.Core.Models.Entities
{
    public class Match
    {
        public int Id { get; set; }
        public string RandomUrl { get; set; }
        public string MatchName { get; set; }
        public DateTime? Date { get; set; }

        [ForeignKey("TeamA")] public int TeamAId { get; set; }

        public virtual Team TeamA { get; set; }

        [ForeignKey("TeamB")] public int TeamBId { get; set; }

        public virtual Team TeamB { get; set; }
        public virtual User User { get; set; }
        public int PaintballfieldId { get; set; }
        public virtual Paintballfield Paintballfield { get; set; }
    }
}