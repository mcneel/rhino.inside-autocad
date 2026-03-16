using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IXRecord"/>
public class XRecordWrapper : AutocadDbObjectWrapper, IXRecord
{
    private readonly Xrecord _xrecord;
    private readonly List<(short TypeCode, object Value)> _data;

    /// <inheritdoc/>
    public IReadOnlyList<(short TypeCode, object Value)> Data => _data;

    /// <summary>
    /// Constructs a new <see cref="XRecordWrapper"/>.
    /// </summary>
    public XRecordWrapper(Xrecord xrecord) : base(xrecord)
    {
        _xrecord = xrecord;
        _data = ExtractData(xrecord);
    }

    /// <summary>
    /// Extracts the typed values from the Xrecord's ResultBuffer.
    /// </summary>
    private static List<(short TypeCode, object Value)> ExtractData(Xrecord xrecord)
    {
        var result = new List<(short TypeCode, object Value)>();

        var data = xrecord.Data;
        if (data == null)
            return result;

        foreach (var typedValue in data)
        {
            result.Add((typedValue.TypeCode, typedValue.Value));
        }

        return result;
    }

    /// <inheritdoc/>
    public new IXRecord ShallowClone()
    {
        return new XRecordWrapper(_xrecord);
    }
}