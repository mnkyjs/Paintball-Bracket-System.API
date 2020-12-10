using il_y.BracketSystem.Core.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace BracketSystem.Core.Models.Dtos
{
    public class UserWithTokenDto
    {
        public string Token { get; set; }
        public UserFlatDto UserFlatDto { get; set; }
    }
}
