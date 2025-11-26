using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IDataTag"/>
public class DataTag : IDataTag
{
    /// <inheritdoc/>
    public GroupCodeValue GroupCode { get; }

    /// <inheritdoc/>
    public object Value { get; }

    /// <summary>
    /// Constructs a new <see cref="DataTag"/> from a <see cref="TypedValue"/>.
    /// </summary>
    public DataTag(GroupCodeValue groupCode, object value)
    {
        this.GroupCode = groupCode;

        this.Value = value;
    }
}