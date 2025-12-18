using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IDynamicBlockReferencePropertyWrapper"/>
public class DynamicBlockReferencePropertyWrapper : WrapperBase<DynamicBlockReferenceProperty>,
    IDynamicBlockReferencePropertyWrapper
{
    private readonly DynamicBlockReferenceProperty _dynamicBlockReferenceProperty;

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public object Value { get; private set; }

    /// <inheritdoc />
    public bool IsReadOnly { get; }

    /// <summary>
    /// Constructs a new <see cref="DynamicBlockReferencePropertyWrapper"/>.
    /// </summary>
    public DynamicBlockReferencePropertyWrapper(DynamicBlockReferenceProperty dynamicBlockReferenceProperty)
        : base(dynamicBlockReferenceProperty)
    {
        _dynamicBlockReferenceProperty = dynamicBlockReferenceProperty;

        this.Name = dynamicBlockReferenceProperty.PropertyName;

        this.IsReadOnly = false;

        this.SetValue(dynamicBlockReferenceProperty.Value);

        this.IsReadOnly = dynamicBlockReferenceProperty.ReadOnly;
    }

    /// <inheritdoc />
    public bool SetValue(object propertyValue)
    {
        if (this.IsReadOnly) return false;

        var type = _dynamicBlockReferenceProperty.Value.GetType();

        var convertedValue = Convert.ChangeType(propertyValue, type);

        if (convertedValue == null) return false;

        this.Value = convertedValue!;

        return true;
    }
}