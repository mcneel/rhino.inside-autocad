using Rhino.Inside.AutoCAD.Core.Interfaces;
using AutoCADEntity = Autodesk.AutoCAD.DatabaseServices.Entity;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A class which represents a <see cref="AutoCADEntity"/>.
/// </summary>
public class AutocadEntityWrapper : DbObjectWrapper, IEntity
{
    /// <inheritdoc/>
    public string TypeName { get; }

    /// <inheritdoc/>
    public string LayerName { get; }

    /// <summary>
    /// Constructs a new <see cref="AutocadEntityWrapper"/> based on an
    /// <see cref="Autodesk.AutoCAD.DatabaseServices.Entity"/>.
    /// </summary>
    public AutocadEntityWrapper(AutoCADEntity autoCadEntity) : base(autoCadEntity)
    {
        this.LayerName = autoCadEntity.Layer;

        this.TypeName = autoCadEntity.GetRXClass().Name;
    }
}