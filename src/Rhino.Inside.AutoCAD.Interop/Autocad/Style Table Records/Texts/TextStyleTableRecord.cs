using Rhino.Inside.AutoCAD.Core.Interfaces;
using CadTextStyleTableRecord = Autodesk.AutoCAD.DatabaseServices.TextStyleTableRecord;

namespace Rhino.Inside.AutoCAD.Interop;

///<inheritdoc cref="ITextStyleTableRecord"/>
public class TextStyleTableRecord : DbObject, ITextStyleTableRecord
{
    ///<inheritdoc />
    public string Name { get; }

    /// <summary>
    /// Constructs a new <see cref="TextStyleTableRecord"/>.
    /// </summary>
    /// <param name="value"></param>
    public TextStyleTableRecord(CadTextStyleTableRecord value) : base(value)
    {
        this.Name = value.Name;
    }

    /// <summary>
    /// Constructs a new <see cref="TextStyleTableRecord"/>.
    /// </summary>
    public TextStyleTableRecord() : base(new CadTextStyleTableRecord())
    {
        this.Name = string.Empty;
    }
}