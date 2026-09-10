using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IAutocadDatabase"/>
/// <remarks>
/// Wraps an AutoCAD <see cref="Database"/> to expose symbol table IDs for blocks,
/// layers, linetypes, and layouts, providing access to the core database structures
/// needed by repositories.
/// </remarks>
/// <remarks>
/// Derived from the non-disposable wrapper base deliberately: the database wrapped here
/// belongs to an open <see cref="Document"/>, which AutoCAD owns and destroys itself.
/// </remarks>
/// <seealso cref="IAutocadDocument"/>
/// <seealso cref="IBlockTableRecordRegister"/>
/// <seealso cref="ILayerRegister"/>
public class AutocadDatabaseWrapper : AutocadWrapperBase<Database>, IAutocadDatabase
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