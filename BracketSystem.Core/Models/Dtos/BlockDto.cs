using System;
using System.Collections.Generic;
using System.Text;

namespace il_y.BracketSystem.Core.Models.Dtos
{
    public class BlockDto
    {
        public int BlockNumber { get; set; }

        public ICollection<string> Games { get; set; }
    }
}
