using Rhino.Inside.AutoCAD.Core.Interfaces;
using CadDimensionStyleTableRecord = Autodesk.AutoCAD.DatabaseServices.DimStyleTableRecord;

namespace Rhino.Inside.AutoCAD.Interop;

///<inheritdoc cref="IDimensionStyleTableRecord"/>
public class DimensionStyleTableRecord : DbObjectWrapper, IDimensionStyleTableRecord
{
    ///<inheritdoc/>
    public string Name { get; }

    /// <summary>
    /// Constructs a new <see cref="DimensionStyleTableRecord"/>.
    /// </summary>
    public DimensionStyleTableRecord(CadDimensionStyleTableRecord value) : base(value)
    {
        this.Name = value.Name;
    }

    /// <summary>
    /// Constructs an invalid <see cref="DimensionStyleTableRecord"/>.
    /// </summary>
    public DimensionStyleTableRecord() : base(new CadDimensionStyleTableRecord())
    {
        this.Name = string.Empty;
    }
}