using Aspu.Template.Domain.Common;
using System.Linq.Expressions;
using System.Security.Principal;

namespace Aspu.Template.Application.Interfaces.Infrastructure;

public interface IGenericRepository<TEntity> where TEntity : BaseEntity
{
    Task<TEntity?> AddAsync(TEntity entity, IPrincipal? user = null);
    Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, IPrincipal? user = null);
    Task<TEntity?> GetAsync(object? id);
    Task<IReadOnlyList<TEntity>> GetAllAsync();
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null);
    Task<int> CountAsync(Expression<Func<TEntity, bool>>[]? predicates);
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, IEnumerable<string>? includeProperties = null, bool disableTracking = true);
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>>[]? predicates, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, IEnumerable<string>? includeProperties = null, bool disableTracking = true);
    Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, IEnumerable<string>? includeProperties = null, int? take = null, int? skip = null, bool disableTracking = true);
    Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>>[]? predicates, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, IEnumerable<string>? includeProperties = null, int? take = null, int? skip = null, bool disableTracking = true);
    Task<IReadOnlyList<TResult>> SelectAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, IEnumerable<string>? includeProperties = null, int? take = null, int? skip = null, bool disableTracking = true);
    Task<IReadOnlyList<TResult>> SelectAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>[]? predicates, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, IEnumerable<string>? includeProperties = null, int? take = null, int? skip = null, bool disableTracking = true);
    void Update(TEntity? entity, IPrincipal? user = null);
    void Update(IEnumerable<TEntity> entities, IPrincipal? user = null);
    Task DeleteAsync(object id);
    void Delete(TEntity? entity);
    void DeleteRange(IEnumerable<TEntity> entities);
}
