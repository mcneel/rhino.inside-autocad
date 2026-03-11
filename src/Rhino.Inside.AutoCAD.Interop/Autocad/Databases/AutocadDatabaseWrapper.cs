using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IDatabase"/>
/// <remarks>
/// Wraps an AutoCAD <see cref="Database"/> to expose symbol table IDs for blocks,
/// layers, linetypes, and layouts. Owned by <see cref="IAutocadDocument"/> and
/// provides access to the core database structures needed by repositories.
/// </remarks>
/// <seealso cref="IAutocadDocument"/>
/// <seealso cref="IBlockTableRecordRegister"/>
/// <seealso cref="ILayerRegister"/>
public class AutocadDatabaseWrapper : AutocadWrapperDisposableBase<Database>, IDatabase
{
    /// <inheritdoc/>
    public IObjectId BlockTableId { get; }

    /// <inheritdoc/>
    public IObjectId LinetypeTableId { get; }

    /// <inheritdoc/>
    public IObjectId LayerTableId { get; }

    /// <inheritdoc/>
    public IObjectId LayoutDictionaryId { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="AutocadDatabaseWrapper"/>.
    /// </summary>
    /// <param name="database">
    /// The AutoCAD <see cref="Database"/> to wrap.
    /// </param>
    public AutocadDatabaseWrapper(Database database) : base(database)
    {
        this.BlockTableId = new AutocadObjectIdWrapper(database.BlockTableId);

        this.LinetypeTableId = new AutocadObjectIdWrapper(database.LinetypeTableId);

        this.LayerTableId = new AutocadObjectIdWrapper(database.LayerTableId);

        this.LayoutDictionaryId = new AutocadObjectIdWrapper(database.LayoutDictionaryId);
    }
}