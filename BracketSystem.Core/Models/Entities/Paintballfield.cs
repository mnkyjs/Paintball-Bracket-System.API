using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace il_y.BracketSystem.Core.Models.Entities
{
    public class Paintballfield
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public int PostalCode { get; set; }
        public string Place { get; set; }
        public string PhoneNumber { get; set; }

        [ForeignKey("Location")] public int LocationId { get; set; }

        public virtual Location Location { get; set; }

        [ForeignKey("Creator")] public int CreatorId { get; set; }

        public virtual User Creator { get; set; }

        public virtual ICollection<Match> Matches { get; set; } = new List<Match>();
    }
}