namespace BaseRz.Core.Utilities;

/// <summary>
/// Id of an optional part (label, description, error message) that a root references while the part is mounted.
/// The last part to register owns it; only the owner can clear it.
/// </summary>
internal sealed class RegisteredId(Action changed)
{
    private object? _owner;

    public string? Value { get; private set; }

    public void Set(object owner, string id)
    {
        _owner = owner;
        Update(id);
    }

    public void Clear(object owner)
    {
        if (ReferenceEquals(_owner, owner))
        {
            _owner = null;
            Update(null);
        }
    }

    private void Update(string? id)
    {
        if (string.Equals(Value, id, StringComparison.Ordinal))
        {
            return;
        }

        Value = id;
        changed();
    }
}
