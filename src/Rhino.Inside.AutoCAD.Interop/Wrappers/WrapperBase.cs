using System.Text.Json.Serialization;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// The base wrapper class for managed objects.
/// </summary>
public abstract class WrapperBase<T>
{
    protected T _wrappedValue;

    /// <summary>
    /// The underlying entity.
    /// </summary>
    [JsonIgnore]
    public T Internal => _wrappedValue;

    /// <summary>
    /// Constructs a new <see cref="WrapperBase{T}"/>.
    /// </summary>
    protected WrapperBase(T value)
    {
        _wrappedValue = value;
    }
}