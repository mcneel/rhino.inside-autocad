namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Provides a generic register pattern for accessing AutoCAD symbol table records.
/// </summary>
/// <typeparam name="T">
/// The type of wrapped AutoCAD object managed by this register.
/// </typeparam>
/// <remarks>
/// Repositories cache AutoCAD symbol table records (layers, line types, layouts, blocks)
/// and provide lookup by name or ObjectId. The cache can be refreshed via <see cref="Repopulate"/>
/// when the underlying document changes. Accessed through <see cref="IAutocadDocument"/> properties.
/// </remarks>
/// <seealso cref="ILayerRegister"/>
/// <seealso cref="ILineTypeRegister"/>
/// <seealso cref="ILayoutRegister"/>
/// <seealso cref="IBlockTableRecordRegister"/>
public interface IRegister<T> : IEnumerable<T>, IDisposable
{
    /// <summary>
    /// Attempts to retrieve an item by its <see cref="IObjectId"/>.
    /// </summary>
    /// <param name="id">
    /// The <see cref="IObjectId"/> of the item to retrieve.
    /// </param>
    /// <param name="value">
    /// When this method returns, contains the item if found; otherwise, <c>null</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the item was found; otherwise, <c>false</c>.
    /// </returns>
    bool TryGetById(IObjectId id, out T? value);

    /// <summary>
    /// Attempts to retrieve an item by its name.
    /// </summary>
    /// <param name="name">
    /// The name to search for (e.g., "0" for the default layer, "Continuous" for line types).
    /// </param>
    /// <param name="dbObject">
    /// When this method returns, contains the item if found; otherwise, <c>null</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the item was found; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// Name lookups are case-insensitive, matching AutoCAD's behavior for symbol table records.
    /// </remarks>
    bool TryGetByName(string name, out T? dbObject);

    /// <summary>
    /// Clears and repopulates the register from the current document state.
    /// </summary>
    /// <remarks>
    /// Call this method after external changes to the document (e.g., layer creation via
    /// AutoCAD commands) to ensure the register reflects the current symbol table contents.
    /// </remarks>
    void Repopulate();
}