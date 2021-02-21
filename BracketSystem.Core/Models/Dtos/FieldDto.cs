using BracketSystem.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BracketSystem.Core.Models.Dtos
{
    public class FieldDto
    {
        public FieldDto()
        {
        }

        public FieldDto(Paintballfield paintballField) : this()
        {
            Id = paintballField.Id;
            FromEntity(paintballField);
        }

        public int? Id { get; set; }
        public string Name { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public int PostalCode { get; set; }
        public string Place { get; set; }
        public string PhoneNumber { get; set; }
        public int LocationId { get; set; }

        public List<KeyPairValueDto> NameAndDate { get; set; } = new List<KeyPairValueDto>();

        public void FromEntity(Paintballfield entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Street = entity.Street;
            HouseNumber = entity.HouseNumber;
            PostalCode = entity.PostalCode;
            Place = entity.Place;
            PhoneNumber = entity.PhoneNumber;
            LocationId = entity.LocationId;
            foreach (var item in entity.Matches)
            {
                if (item.Date == null) continue;
                var keyPair = new KeyPairValueDto
                {
                    Name = item.MatchName,
                    Guid = item.Guid,
                    Date = (DateTime) item.Date
                };

                var containsItem = NameAndDate.Any(n => n.Name == keyPair.Name);

                if (!containsItem) NameAndDate.Add(keyPair);
            }
        }
    }
}