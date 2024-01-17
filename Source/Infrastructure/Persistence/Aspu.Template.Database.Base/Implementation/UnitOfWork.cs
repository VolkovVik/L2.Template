using Aspu.L2.DAL;
using Aspu.Template.Application.Interfaces.Infrastructure;
using Aspu.Template.Domain.Common;
using Aspu.Template.Persistence.Base.Implementation.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Data.Common;

namespace Aspu.Template.Persistence.Base.Implementation;

public class UnitOfWork(AppDbContext appDbContext) : IUnitOfWork
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private Hashtable _repositories = [];

    public string ErrorMessage { get; set; } = string.Empty;

    ~UnitOfWork() => Dispose(disposing: false);

    public async Task<bool> SaveChanges()
    {
        var result = true;
        await using var transaction = await _appDbContext.Database.BeginTransactionAsync();
        try
        {
            await _appDbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            result = false;
            ErrorMessage = ex.InnerException?.Message ?? ex.Message;
            await transaction.RollbackAsync();
        }
        return result;
    }

    public IGenericRepository<TEntity>? GetRepository<TEntity>() where TEntity : BaseEntity
    {
        var type = typeof(TEntity).Name;

        _repositories ??= [];
        if (!_repositories.ContainsKey(type))
        {
            var repositiryType = typeof(GenericRepository<>);
            var repositoryInstance = Activator.CreateInstance(repositiryType.MakeGenericType(typeof(TEntity)), _appDbContext);
            _repositories.Add(type, repositoryInstance);
        }
        return _repositories[type] as IGenericRepository<TEntity>;
    }

    public DbConnection GetDbConnection() => _appDbContext.Database.GetDbConnection();


    private bool _disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _repositories.Clear();
            _appDbContext.Dispose();
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}