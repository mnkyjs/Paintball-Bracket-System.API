
using BracketSystem.Core.Models.Entities;

namespace BracketSystem.Core.Models.Dtos
{
    public class LocationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static LocationDto FromEntity(Location entity)
        {
            return new LocationDto
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }
    }
}