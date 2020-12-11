
using BracketSystem.Core.Models.Entities;

namespace BracketSystem.Core.Models.Dtos
{
    public class LocationDto : BaseDto<Location>
    {
        public LocationDto()
        {
        }

        public LocationDto(Location location) : this()
        {
            Id = location.Id;
            FromEntity(location);
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public static LocationDto FromEntity(Location entity)
        {
            LocationDto vm = null;

            if (entity != null)
            {
                vm = new LocationDto();
                CopyProperties(entity, vm);
            }

            return vm;
        }

        public override void UpdateEntity(Location entity)
        {
            entity.Name = Name;
        }
    }
}