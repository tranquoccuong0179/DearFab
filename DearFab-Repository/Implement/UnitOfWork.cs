using System.ComponentModel.DataAnnotations;
using DearFab_Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DearFab_Repository.Implement;

public class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext
{
    public TContext Context { get; }
    private Dictionary<Type, object> _repositories;
    private IDbContextTransaction _currentTransaction;

    public UnitOfWork(TContext context)
    {
        Context = context;
    }

    public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class
    {
        _repositories ??= new Dictionary<Type, object>();
        if (_repositories.TryGetValue(typeof(TEntity), out object repository))
        {
            return (IGenericRepository<TEntity>)repository;
        }

        var repo = new GenericRepository<TEntity>(Context);
        _repositories.Add(typeof(TEntity), repo);
        return repo;
    }

    public void Dispose()
    {
        _currentTransaction?.Dispose();
        Context?.Dispose();
    }

    public int Commit()
    {
        TrackChanges();
        return Context.SaveChanges();
    }

    public async Task<int> CommitAsync()
    {
        TrackChanges();
        return await Context.SaveChangesAsync();
    }

    private void TrackChanges()
    {
        var validationErrors = Context.ChangeTracker.Entries<IValidatableObject>()
            .SelectMany(e => e.Entity.Validate(null))
            .Where(e => e != ValidationResult.Success)
            .ToArray();

        if (validationErrors.Any())
        {
            var exceptionMessage = string.Join(Environment.NewLine,
                validationErrors.Select(error => $"Properties {string.Join(", ", error.MemberNames)} Error: {error.ErrorMessage}"));
            throw new Exception(exceptionMessage);
        }
    }
}