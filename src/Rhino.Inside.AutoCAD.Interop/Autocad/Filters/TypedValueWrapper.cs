using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="ITypedValueWrapper"/>
public class TypedValueWrapper : AutocadWrapperBase<TypedValue>, ITypedValueWrapper
{
    /// <inheritdoc />
    public short TypeCode { get; }

    /// <inheritdoc />
    public object Value { get; }

    /// <summary>
    /// Constucts a new <see cref="TypedValueWrapper"/> by wrapping an existing <see
    /// cref="TypedValue"/> instance. The wrapper extracts the type code and value
    /// from the provided <see cref="TypedValue"/> and makes them accessible through
    /// the <see cref="TypeCode"/> and <see cref="Value"/> properties, allowing the
    /// wrapped data to be used in contexts that require an object, such as selection
    /// filters.
    /// </summary>
    public TypedValueWrapper(TypedValue value) : base(value)
    {
        this.TypeCode = value.TypeCode;
        this.Value = value.Value;
    }

    /// <inheritdoc />
    public TypedValue Unwrap() => this.AutocadObject;
}