namespace RefLab.Application.Abstractions;

/// <summary>
/// The read side of the persistence store.
/// </summary>
/// <remarks>
/// <para>
/// The interface exposes <see cref="IQueryable{T}"/> rather than
/// <c>DbSet&lt;T&gt;</c> so that the Application layer stays free of any
/// persistence engine. The Infrastructure layer implements it over EF Core;
/// the test suite implements it over in-memory collections.
/// </para>
/// </remarks>
public interface IAppDbContext
{
    /// <summary>Returns a queryable set of entities of the given type.</summary>
    IQueryable<T> Set<T>()
        where T : class;
}
