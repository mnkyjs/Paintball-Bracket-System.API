using System.Collections.Generic;

namespace BracketSystem.Core.Models.Dtos
{
    public class BlockDto
    {
        public int BlockNumber { get; set; }

        public ICollection<string> Games { get; set; }
    }
}
