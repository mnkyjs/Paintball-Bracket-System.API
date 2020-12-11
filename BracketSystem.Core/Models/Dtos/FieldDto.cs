using BracketSystem.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BracketSystem.Core.Models.Dtos
{
    // TODO Maybe add the colletion of matches, if there is an error
    public class FieldDto : BaseDto<Paintballfield>
    {
        public FieldDto()
        {
        }

        public FieldDto(Paintballfield paintballField) : this()
        {
            Id = paintballField.Id;
            FromEntity(paintballField);
        }

        public int Id { get; set; }
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
                    Date = (DateTime) item.Date
                };

                var containsItem = NameAndDate.Any(n => n.Name == keyPair.Name);

                if (!containsItem) NameAndDate.Add(keyPair);
            }
        }

        public override void UpdateEntity(Paintballfield entity)
        {
            entity.Name = Name;
            entity.Street = Street;
            entity.HouseNumber = HouseNumber;
            entity.PostalCode = PostalCode;
            entity.Place = Place;
            entity.PhoneNumber = PhoneNumber;
            entity.LocationId = LocationId;
        }
    }
}