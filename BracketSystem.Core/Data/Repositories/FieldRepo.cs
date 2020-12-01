using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using il_y.BracketSystem.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace il_y.BracketSystem.Core.Data.Repositories
{
    public class FieldRepo : GenericRepository<Paintballfield>, IFieldRepo
    {
        public FieldRepo(BracketContext context)
            : base(context)
        {
        }

        #region BracketContext

        private BracketContext BracketContext => DbContext as BracketContext;

        #endregion BracketContext

        public async Task<Paintballfield> GetFieldsWithLocation(int fieldId)
        {
            var field = await BracketContext.Paintballfields.Where(x => x.Id == fieldId).Include(l => l.Location)
                .FirstOrDefaultAsync();
            return field;
        }

        public async Task<List<Paintballfield>> GetFieldsWithMatches()
        {
            var field = await BracketContext.Paintballfields.Include(m => m.Matches)
                .Where(x => x.Matches.Count > 0).ToListAsync();
            var fieldListForView = field.Select(newField => new Paintballfield
            {
                Id = newField.Id,
                Name = newField.Name,
                Matches = newField.Matches.Where(dt => dt.Date >= DateTime.Today).OrderBy(d => d.Date).ToList()
            }).ToList();

            return fieldListForView;
        }
    }
}