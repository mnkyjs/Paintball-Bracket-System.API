using BracketSystem.Core.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BracketSystem.Core.Data
{
    public interface IFieldRepo : IGenericRepository<Paintballfield>
    {
        Task<Paintballfield> GetFieldsWithLocation(int fieldId);

        Task<List<Paintballfield>> GetFieldsWithMatches();
    }
}