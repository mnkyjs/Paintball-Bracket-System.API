using System.Collections.Generic;
using System.Threading.Tasks;
using il_y.BracketSystem.Core.Models.Entities;

namespace il_y.BracketSystem.Core.Data
{
    public interface IFieldRepo : IGenericRepository<Paintballfield>
    {
        Task<Paintballfield> GetFieldsWithLocation(int fieldId);

        Task<List<Paintballfield>> GetFieldsWithMatches();
    }
}