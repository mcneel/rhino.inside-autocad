namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Defines a contract for objects that can be baked (persisted) to AutoCAD's model space.
/// </summary>
public interface IAutocadBakeable
{
    /// <summary>
    /// Bakes the object to AutoCAD's model space within the provided transaction.
    /// </summary>
    /// <returns>The ObjectId of the newly created AutoCAD entity.</returns>
    List<IObjectId> BakeToAutocad(ITransactionManager transactionManager,
        IBakingComponent bakingComponent, IBakeSettings? settings = null);
}

