using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

///<inheritdoc cref="IBlockTableRecordAttribute"/>
public class BlockTableRecordAttribute : IBlockTableRecordAttribute
{
    ///<inheritdoc />
    public string Tag { get; set; }
}