namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Manages the preview of a Rhino object in AutoCAD using transient entities.
/// </summary>
public interface IPreviewServer
{
    /// <summary>
    /// The <see cref="IObjectRegister"/> used to track the entities being previewed.
    /// </summary>
    IObjectRegister ObjectRegister { get; }

    /// <summary>
    /// Adds the provided <paramref name="rhinoConvertibleSet"/> into this <see cref=
    /// "ITransientManager"/>.
    /// </summary>
    void AddObject(Guid rhinoObjectId, IRhinoConvertibleSet rhinoConvertibleSet);

    /// <summary>
    /// Removes the provided <paramref name="rhinoObjectId"/> from this <see cref=
    /// "ITransientManager"/>.
    /// </summary>
    void RemoveObject(Guid rhinoObjectId);

    /// <summary>
    /// Removes all the transient entities which are in the <see cref="IObjectRegister"/>
    /// from the AutoCAD drawing.
    /// </summary>
    void ClearServer();

    /// <summary>
    /// Adds all the transient entities which are in the <see cref="IObjectRegister"/>
    /// from the AutoCAD drawing.
    /// </summary>
    void PopulateServer();
}
