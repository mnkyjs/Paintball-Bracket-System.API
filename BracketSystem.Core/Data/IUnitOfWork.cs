using System;
using System.Threading.Tasks;
using il_y.BracketSystem.Core.Models.Entities;
using Role = il_y.BracketSystem.Core.Models.Role;

namespace il_y.BracketSystem.Core.Data
{
    public interface IUnitOfWork : IDisposable
    {
        ITeamRepo Teams { get; }
        IFieldRepo Fields { get; }
        IGenericRepository<Location> Locations { get; }
        IMatchScheduleRepo Matches { get; }
        IUserRepo Users { get; }

        Task CompleteAsync();
    }
}