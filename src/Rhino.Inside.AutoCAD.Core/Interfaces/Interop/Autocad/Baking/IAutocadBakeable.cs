namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Defines a contract for objects that can be baked (persisted) to AutoCAD's model space.
/// </summary>
public interface IAutocadBakeable
{
    /// <summary>
    /// Bakes the object to AutoCAD's model space within the provided transaction.
    /// </summary>
    /// <param name="transactionManager">The transaction manager to use for the bake operation.</param>
    /// <param name="settings">Optional bake settings (layer, linetype, color, etc.).</param>
    /// <returns>The ObjectId of the newly created AutoCAD entity.</returns>
    List<IObjectId> BakeToAutocad(ITransactionManager transactionManager, IBakeSettings? settings = null);
}
