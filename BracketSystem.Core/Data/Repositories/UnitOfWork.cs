using System;
using System.Threading.Tasks;
using il_y.BracketSystem.Core.Models.Entities;
using Microsoft.Extensions.Logging;

namespace il_y.BracketSystem.Core.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BracketContext _context;
        private readonly ILogger<UnitOfWork> _logger;
        private FieldRepo _fieldRepo;
        private GenericRepository<Location> _locationRepo;
        private UserRepo _userRepo;
        private MatchScheduleRepo _matchScheduleRepo;
        private TeamRepo _teamRepo;

        public UnitOfWork(BracketContext context, ILogger<UnitOfWork> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Fields
        public ITeamRepo Teams => _teamRepo ??= new TeamRepo(_context);

        public IMatchScheduleRepo Matches =>
            _matchScheduleRepo ??= new MatchScheduleRepo(_context);

        public IFieldRepo Fields => _fieldRepo ??= new FieldRepo(_context);

        public IGenericRepository<Location> Locations =>
            _locationRepo ??= new GenericRepository<Location>(_context);
        
        public IUserRepo Users =>
            _userRepo ??= new UserRepo(_context);


        public async Task CompleteAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Something went wrong: {ex}");
                Console.WriteLine(ex);
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}