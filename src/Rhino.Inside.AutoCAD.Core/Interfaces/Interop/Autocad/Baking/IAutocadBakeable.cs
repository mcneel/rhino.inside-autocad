namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Defines a contract for objects that can be baked (persisted) to an AutoCAD block table
/// record, which is model space unless another record is supplied.
/// </summary>
public interface IAutocadBakeable
{
    /// <summary>
    /// Bakes the object to AutoCAD within the provided transaction.
    /// </summary>
    /// <param name="autocadTransactionManager">
    /// The transaction the entities are appended within.
    /// </param>
    /// <param name="bakingComponent">
    /// The component driving the bake, used to report warnings and to receive the
    /// ObjectIds of any entities produced asynchronously.
    /// </param>
    /// <param name="settings">
    /// Optional bake settings (layer, linetype, color) applied to the baked entities.
    /// </param>
    /// <param name="targetBlockTableRecord">
    /// The block table record to append the entities to. When null the entities are
    /// appended to model space, which is the behaviour used by the bake components.
    /// Supply a block definition to bake the geometry into a block instead.
    /// </param>
    /// <returns>
    /// The ObjectIds of the newly created AutoCAD entities. Bakeables that convert
    /// asynchronously return an empty list and report their ids through
    /// <see cref="IBakingComponent.AppendDataList"/> once the conversion completes.
    /// </returns>
    List<IObjectId> BakeToAutocad(IAutocadTransactionManager autocadTransactionManager,
        IBakingComponent bakingComponent, IBakeSettings? settings = null,
        IAutocadBlockTableRecord? targetBlockTableRecord = null);

    /// <summary>
    /// Appends the geometry this object would bake to the signature builder, used by
    /// tracked components to detect input changes between solves. Must fingerprint the
    /// same geometry that <see cref="BakeToAutocad"/> writes so that any change to the
    /// baked output changes the signature.
    /// </summary>
    void AppendInputSignature(IInputSignatureBuilder inputSignatureBuilder);
}
