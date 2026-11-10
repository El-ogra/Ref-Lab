namespace RefLab.Application.Common;

/// <summary>
/// Raised when a record was modified by another user between the read and the
/// write.
/// </summary>
public sealed class ConcurrencyException : ApplicationException
{
    /// <summary>Initialises a new exception.</summary>
    public ConcurrencyException(string entityName, object key)
        : base($"{entityName} '{key}' was modified by another user and could not be saved.")
    {
        EntityName = entityName;
        Key = key;
    }

    /// <summary>The name of the entity that conflicted.</summary>
    public string EntityName { get; }

    /// <summary>The key of the entity that conflicted.</summary>
    public object Key { get; }
}
