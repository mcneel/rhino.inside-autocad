namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Defines a contract for objects that can be baked (persisted) to AutoCAD's model space.
/// </summary>
public interface IAutocadBakeable
{
    /// <summary>
    /// Bakes the object to AutoCAD's model space within the provided transaction.
    /// </summary>
    /// <returns>
    /// The ObjectId of the newly created AutoCAD entity.
    /// </returns>
    List<IObjectId> BakeToAutocad(IAutocadTransactionManager autocadTransactionManager,
        IBakingComponent bakingComponent, IBakeSettings? settings = null);

    /// <summary>
    /// Appends the geometry this object would bake to the signature builder, used by
    /// tracked components to detect input changes between solves. Must fingerprint the
    /// same geometry that <see cref="BakeToAutocad"/> writes so that any change to the
    /// baked output changes the signature.
    /// </summary>
    void AppendInputSignature(IInputSignatureBuilder inputSignatureBuilder);
}