using Aspu.L2.DAL;
using Aspu.Template.Application.Interfaces.Infrastructure;
using Aspu.Template.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Principal;

namespace Aspu.Template.Persistence.Base.Implementation.Repositories;

public class GenericRepository<TEntity>(AppDbContext appDbContext) : IGenericRepository<TEntity> where TEntity : BaseAuditableEntity
{
    protected readonly AppDbContext _appDbContext = appDbContext;
    protected readonly DbSet<TEntity> _dbSet = appDbContext.Set<TEntity>();

    public virtual async Task<TEntity?> AddAsync(TEntity entity, IPrincipal? user = null)
    {
        if (entity == null) return entity;

        var date = DateTime.UtcNow;
        var name = user?.Identity?.Name ?? "default";
        entity.CreatedOn = date;
        entity.CreatedBy = name;
        entity.ModifiedOn = date;
        entity.ModifiedBy = name;

        await _dbSet.AddAsync(entity);
        return entity;
    }

    public virtual async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, IPrincipal? user = null)
    {
        if (entities?.Any() != true) return Enumerable.Empty<TEntity>();

        var date = DateTime.UtcNow;
        var name = user?.Identity?.Name ?? "default";
        var items = entities.ToList();
        foreach (var entity in items)
        {
            entity.CreatedOn = date;
            entity.CreatedBy = name;
            entity.ModifiedOn = date;
            entity.ModifiedBy = name;
        }
        await _dbSet.AddRangeAsync(items);
        return items;
    }

    public virtual async Task<TEntity?> GetAsync(object? id) => id == null ? default : await _dbSet.FindAsync(id);
    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync() => await _dbSet.ToListAsync();
    public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null) =>
        await CountAsync(GetArray(predicate));

    public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>>[]? predicates)
    {
        IQueryable<TEntity> query = _dbSet;
        query = query.AsNoTracking();
        query = ApplyPredicateAsync(query, predicates);
        var count = await query.CountAsync();
        return count;
    }

    public virtual async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        IEnumerable<string>? includeProperties = null, bool disableTracking = true) =>
        await FirstOrDefaultAsync(GetArray(predicate), orderBy, includeProperties, disableTracking);

    public virtual async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>>[]? predicates,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        IEnumerable<string>? includeProperties = null, bool disableTracking = true)
    {
        IQueryable<TEntity> query = _dbSet;

        query = !disableTracking ? query : query.AsNoTracking();
        query = ApplyPredicateAsync(query, predicates);

        var properties = includeProperties?.ToArray() ?? [];
        Array.ForEach(properties, x => query = query.Include(x));

        query = orderBy == null ? query : orderBy(query);
        var value = await query.FirstOrDefaultAsync();
        return value;
    }

    public virtual async Task<IReadOnlyList<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        IEnumerable<string>? includeProperties = null, int? take = null, int? skip = null, bool disableTracking = true) =>
         await FindAsync(GetArray(predicate), orderBy, includeProperties, take, skip, disableTracking);

    public virtual async Task<IReadOnlyList<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>>[]? predicates,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        IEnumerable<string>? includeProperties = null, int? take = null, int? skip = null, bool disableTracking = true)
    {
        IQueryable<TEntity> query = _dbSet;

        query = !disableTracking ? query : query.AsNoTracking();
        query = ApplyPredicateAsync(query, predicates);
        query = !take.HasValue || take.Value < 1 ? query : query.Take(take.Value);
        query = !skip.HasValue || skip.Value < 1 ? query : query.Skip(skip.Value);

        var properties = includeProperties?.ToArray() ?? [];
        Array.ForEach(properties, x => query = query.Include(x));

        var value = await (orderBy == null ? query : orderBy(query)).ToListAsync();
        return value;
    }

    public virtual async Task<IReadOnlyList<TResult>> SelectAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        IEnumerable<string>? includeProperties = null, int? take = null, int? skip = null, bool disableTracking = true) =>
        await SelectAsync(selector, GetArray(predicate), orderBy, includeProperties, take, skip, disableTracking);

    public virtual async Task<IReadOnlyList<TResult>> SelectAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>[]? predicates,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        IEnumerable<string>? includeProperties = null, int? take = null, int? skip = null, bool disableTracking = true)
    {
        IQueryable<TEntity> query = _dbSet;

        query = !disableTracking ? query : query.AsNoTracking();
        query = ApplyPredicateAsync(query, predicates);
        query = !take.HasValue || take.Value < 1 ? query : query.Take(take.Value);
        query = !skip.HasValue || skip.Value < 1 ? query : query.Skip(skip.Value);

        var properties = includeProperties?.ToArray() ?? [];
        Array.ForEach(properties, x => query = query.Include(x));

        var value = await (orderBy == null ? query.Select(selector) : orderBy(query).Select(selector)).ToListAsync();
        return value;
    }

    private static T[] GetArray<T>(T? item) => item == null ? [] : new[] { item };

    private static IQueryable<TEntity> ApplyPredicateAsync(IQueryable<TEntity> query, Expression<Func<TEntity, bool>>[]? predicates)
    {
        if (!(predicates?.Length > 0)) return query;

        foreach (var predicate in predicates.Where(x => x != null))
        {
            query = query.Where(predicate);
        }
        return query;
    }

    public virtual void Update(TEntity? entity, IPrincipal? user = null)
    {
        if (entity == null) return;

        entity.ModifiedOn = DateTime.UtcNow;
        entity.ModifiedBy = user?.Identity?.Name ?? "default";

        _dbSet.Attach(entity);
        _appDbContext.Entry(entity).State = EntityState.Modified;
    }

    public virtual void Update(IEnumerable<TEntity> entities, IPrincipal? user = null)
    {
        if (entities?.Any() != true) return;

        foreach (var entity in entities)
        {
            Update(entity, user);
        }
    }

    public virtual async Task DeleteAsync(object id)
    {
        var entity = await _dbSet.FindAsync(id);
        Delete(entity);
    }

    public virtual void Delete(TEntity? entity)
    {
        if (entity == null) return;

        if (_appDbContext.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }
        _dbSet.Remove(entity);
    }

    public virtual void DeleteRange(IEnumerable<TEntity> entities)
    {
        if (entities?.Any() != true) return;

        foreach (var entity in entities)
        {
            if (_appDbContext.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
        }
        _dbSet.RemoveRange(entities);
    }
}
