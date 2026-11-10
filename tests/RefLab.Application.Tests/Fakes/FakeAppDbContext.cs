using RefLab.Application.Abstractions;

namespace RefLab.Application.Tests.Fakes;

/// <summary>
/// An in-memory stand-in for the persistence store.
/// </summary>
/// <remarks>
/// The Application layer never references a persistence engine, so the whole
/// use-case suite can be exercised without a database.
/// </remarks>
public sealed class FakeAppDbContext : IAppDbContext
{
    private interface IEntitySet
    {
        void Add(object entity);

        void Update(object entity);

        void Remove(object entity);
    }

    private sealed class EntitySet<T> : IEntitySet
        where T : class
    {
        public List<T> Items { get; } = new();

        public void Add(object entity) => Items.Add((T)entity);

        public void Update(object entity)
        {
            var index = Items.FindIndex(existing => SameEntity(existing, (T)entity));

            if (index >= 0)
            {
                Items[index] = (T)entity;
            }
            else
            {
                Items.Add((T)entity);
            }
        }

        public void Remove(object entity)
        {
            var index = Items.FindIndex(existing => SameEntity(existing, (T)entity));

            if (index >= 0)
            {
                Items.RemoveAt(index);
            }
        }
    }

    private readonly Dictionary<Type, IEntitySet> _sets = new();

    /// <summary>The number of times the unit of work was committed.</summary>
    public int SaveChangesCallCount { get; private set; }

    /// <inheritdoc />
    public IQueryable<T> Set<T>()
        where T : class => Items<T>().AsQueryable();

    /// <summary>Adds the supplied entities to the store.</summary>
    public void Seed(params object[] entities)
    {
        foreach (var entity in entities)
        {
            AddEntity(entity);
        }
    }

    /// <summary>Returns every stored entity of the given type.</summary>
    public List<T> Items<T>()
        where T : class
    {
        if (!_sets.TryGetValue(typeof(T), out var stored))
        {
            stored = new EntitySet<T>();
            _sets[typeof(T)] = stored;
        }

        return ((EntitySet<T>)stored).Items;
    }

    /// <summary>Adds an entity to the store.</summary>
    public void AddEntity(object entity) => GetOrCreate(entity.GetType()).Add(entity);

    /// <summary>Marks an entity as modified.</summary>
    public void UpdateEntity(object entity) => GetOrCreate(entity.GetType()).Update(entity);

    /// <summary>Marks an entity for deletion.</summary>
    public void RemoveEntity(object entity) => GetOrCreate(entity.GetType()).Remove(entity);

    /// <summary>Records a commit.</summary>
    public int RecordSaveChanges()
    {
        SaveChangesCallCount++;
        return 1;
    }

    private IEntitySet GetOrCreate(Type type)
    {
        if (!_sets.TryGetValue(type, out var stored))
        {
            var setType = typeof(EntitySet<>).MakeGenericType(type);
            stored = (IEntitySet)Activator.CreateInstance(setType)!;
            _sets[type] = stored;
        }

        return stored;
    }

    private static bool SameEntity(object left, object right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        var leftKey = KeyOf(left);
        var rightKey = KeyOf(right);

        return leftKey is not null && rightKey is not null && leftKey.Equals(rightKey);
    }

    private static object? KeyOf(object entity) =>
        entity.GetType().GetProperty("Id")?.GetValue(entity);
}
