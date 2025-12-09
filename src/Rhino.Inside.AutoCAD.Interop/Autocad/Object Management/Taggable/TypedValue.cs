using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="ITypedValue"/>
public class TypedValue : ITypedValue
{
    /// <inheritdoc/>
    public GroupCodeValue GroupCode { get; }

    /// <inheritdoc/>
    public object Value { get; }

    /// <summary>
    /// Constructs a new <see cref="TypedValue"/> from a <see cref="Autodesk.AutoCAD.DatabaseServices.TypedValue"/>.
    /// </summary>
    public TypedValue(GroupCodeValue groupCode, object value)
    {
        this.GroupCode = groupCode;

        this.Value = value;
    }
}