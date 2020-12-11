using BracketSystem.Core.Models.Entities;
using System;
using System.Threading.Tasks;

namespace BracketSystem.Core.Data
{
    public interface IUnitOfWork : IDisposable
    {
        IFieldRepo Fields { get; }
        IGenericRepository<Location> Locations { get; }
        IMatchScheduleRepo Matches { get; }
        ITeamRepo Teams { get; }
        IUserRepo Users { get; }

        Task CompleteAsync();
    }
}