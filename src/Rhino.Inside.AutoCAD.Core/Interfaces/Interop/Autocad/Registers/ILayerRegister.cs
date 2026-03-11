namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Provides access to layers in an <see cref="IAutocadDocument"/> with support for layer creation.
/// </summary>
/// <remarks>
/// Extends <see cref="IRegister {T}"/> with layer-specific operations including creation and
/// default layer access. Accessed via <see cref="IAutocadDocument.LayerRegister "/>. Used by
/// Grasshopper components such as GetAutocadLayersComponent, GetAutocadLayerByNameComponent,
/// and CreateAutocadLayerComponent.
/// </remarks>
/// <seealso cref="IAutocadLayerTableRecord"/>
/// <seealso cref="IRegister {T}"/>
/// <seealso cref="IAutocadDocument.LayerRegister "/>
public interface ILayerRegister : IRegister<IAutocadLayerTableRecord>
{
    /// <summary>
    /// Creates a new layer in the document or retrieves an existing layer with the same name.
    /// </summary>
    /// <param name="color">
    /// The <see cref="IColor"/> to assign as the layer's default color.
    /// </param>
    /// <param name="name">
    /// The name for the new layer. Must be unique within the document.
    /// </param>
    /// <param name="layer">
    /// When this method returns, contains the created or existing layer.
    /// </param>
    /// <returns>
    /// <c>true</c> if a new layer was created; <c>false</c> if a layer with the same name
    /// already existed.
    /// </returns>
    /// <remarks>
    /// If a layer with the specified name already exists, no modification is made to the
    /// existing layer (including color), and that layer is returned.
    /// </remarks>
    bool TryAddLayer(IColor color, string name, out IAutocadLayerTableRecord layer);

    /// <summary>
    /// Retrieves the default layer "0" that exists in every AutoCAD drawing.
    /// </summary>
    /// <returns>
    /// The <see cref="IAutocadLayerTableRecord"/> for layer "0".
    /// </returns>
    /// <remarks>
    /// Layer "0" is always present and cannot be deleted or renamed. It is commonly used
    /// as a fallback when no specific layer is required.
    /// </remarks>
    IAutocadLayerTableRecord GetDefault();
}