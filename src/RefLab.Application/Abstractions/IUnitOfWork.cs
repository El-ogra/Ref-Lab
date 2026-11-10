namespace RefLab.Application.Abstractions;

/// <summary>
/// The write side of the persistence store.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>Adds a new entity to the store.</summary>
    void Add<T>(T entity)
        where T : class;

    /// <summary>Marks an existing entity as modified.</summary>
    void Update<T>(T entity)
        where T : class;

    /// <summary>Marks an entity for deletion.</summary>
    void Remove<T>(T entity)
        where T : class;

    /// <summary>Persists every pending change.</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
