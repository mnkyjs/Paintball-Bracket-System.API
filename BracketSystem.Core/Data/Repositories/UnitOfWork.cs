using BracketSystem.Core.Models.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BracketSystem.Core.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BracketContext _context;
        private readonly ILogger<UnitOfWork> _logger;
        private FieldRepo _fieldRepo;
        private GenericRepository<Location> _locationRepo;
        private MatchScheduleRepo _matchScheduleRepo;
        private TeamRepo _teamRepo;
        private UserRepo _userRepo;
        public UnitOfWork(BracketContext context, ILogger<UnitOfWork> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IFieldRepo Fields => _fieldRepo ??= new FieldRepo(_context);

        public IGenericRepository<Location> Locations =>
                    _locationRepo ??= new GenericRepository<Location>(_context);

        public IMatchScheduleRepo Matches =>
                    _matchScheduleRepo ??= new MatchScheduleRepo(_context);

        // Fields
        public ITeamRepo Teams => _teamRepo ??= new TeamRepo(_context);
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