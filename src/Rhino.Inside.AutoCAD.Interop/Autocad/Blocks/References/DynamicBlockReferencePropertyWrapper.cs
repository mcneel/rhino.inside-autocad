using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core;
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
    public object[] AllowedValues { get; }

    /// <inheritdoc />
    public bool IsReadOnly { get; }

    /// <inheritdoc />
    public DynamicPropertyTypeCode TypeCode { get; }

    /// <summary>
    /// Constructs a new <see cref="DynamicBlockReferencePropertyWrapper"/>.
    /// </summary>
    public DynamicBlockReferencePropertyWrapper(DynamicBlockReferenceProperty dynamicBlockReferenceProperty)
        : base(dynamicBlockReferenceProperty)
    {
        _dynamicBlockReferenceProperty = dynamicBlockReferenceProperty;

        this.Name = dynamicBlockReferenceProperty.PropertyName;

        this.IsReadOnly = false;

        this.Value = _dynamicBlockReferenceProperty.Value;

        this.IsReadOnly = dynamicBlockReferenceProperty.ReadOnly;

        this.AllowedValues = dynamicBlockReferenceProperty.GetAllowedValues();

        this.TypeCode = (DynamicPropertyTypeCode)_dynamicBlockReferenceProperty.PropertyTypeCode;
    }

    /// <inheritdoc />
    public bool SetValue(object propertyValue, ITransactionManager transactionManager)
    {
        if (this.IsReadOnly) return false;

        var transaction = transactionManager.Unwrap();

        _ = transaction.GetObject(_dynamicBlockReferenceProperty.BlockId, OpenMode.ForWrite) as BlockReference;

        if (this.TypeCode.TryConvertValue(propertyValue, out var convertedValue) == false) return false;

        _dynamicBlockReferenceProperty.Value = convertedValue;
        this.Value = convertedValue!;

        return true;
    }
}

