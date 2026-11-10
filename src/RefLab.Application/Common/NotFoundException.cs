namespace RefLab.Application.Common;

/// <summary>
/// Raised when a required record does not exist.
/// </summary>
public sealed class NotFoundException : ApplicationException
{
    /// <summary>Initialises a new exception.</summary>
    public NotFoundException(string entityName, object key)
        : base($"{entityName} '{key}' was not found.")
    {
        EntityName = entityName;
        Key = key;
    }

    /// <summary>The name of the entity that was not found.</summary>
    public string EntityName { get; }

    /// <summary>The key that was looked up.</summary>
    public object Key { get; }
}
