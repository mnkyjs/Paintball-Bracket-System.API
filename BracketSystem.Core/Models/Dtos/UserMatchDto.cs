using BracketSystem.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BracketSystem.Core.Models.Dtos
{
    public class UserMatchDto
    {
        public UserMatchDto()
        {
        }

        public UserMatchDto(List<Match> match) : this()
        {
            CopyList(match);
        }

        public List<KeyPairValueDto> NameAndDate { get; set; } = new List<KeyPairValueDto>();

        public void CopyList(List<Match> entity)
        {
            foreach (var item in entity)
            {
                if (item.Date == null) continue;
                var keyPair = new KeyPairValueDto
                {
                    Name = item.MatchName,
                    Date = (DateTime) item.Date
                };

                var containsItem = NameAndDate.Any(n => n.Name == keyPair.Name);

                if (!containsItem) NameAndDate.Add(keyPair);
            }
        }
    }
}