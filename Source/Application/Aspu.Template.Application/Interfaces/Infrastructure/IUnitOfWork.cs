using Aspu.Template.Domain.Common;
using System.Data.Common;

namespace Aspu.Template.Application.Interfaces.Infrastructure;

public interface IUnitOfWork : IDisposable
{
    string ErrorMessage { get; set; }

    IGenericRepository<TEntity>? GetRepository<TEntity>() where TEntity : BaseEntity;
    Task<bool> SaveChanges();
    public DbConnection GetDbConnection();
}
