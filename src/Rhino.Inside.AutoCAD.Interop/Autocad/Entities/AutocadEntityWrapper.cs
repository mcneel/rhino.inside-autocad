using Rhino.Inside.AutoCAD.Core.Interfaces;
using AutoCADEntity = Autodesk.AutoCAD.DatabaseServices.Entity;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IEntity"/>
/// <remarks>
/// Extends <see cref="AutocadDbObjectWrapper"/> to provide entity-specific properties such as
/// <see cref="LayerName"/>. Used throughout the Grasshopper library when working with
/// geometric entities retrieved from AutoCAD selections or filters.
/// </remarks>
/// <seealso cref="AutocadDbObjectWrapper"/>
public class AutocadEntityWrapper : AutocadDbObjectWrapper, IEntity
{
    /// <inheritdoc/>
    public string LayerName { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="AutocadEntityWrapper"/>.
    /// </summary>
    /// <param name="autoCadEntity">
    /// The AutoCAD <see cref="AutoCADEntity"/> to wrap.
    /// </param>
    public AutocadEntityWrapper(AutoCADEntity autoCadEntity) : base(autoCadEntity)
    {
        this.LayerName = autoCadEntity.Layer;
    }
}