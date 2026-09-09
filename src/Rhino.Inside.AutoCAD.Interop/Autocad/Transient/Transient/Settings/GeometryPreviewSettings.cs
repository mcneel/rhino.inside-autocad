using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.GraphicsInterface;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Services;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IGeometryPreviewSettings"/>
public class GeometryPreviewSettings : IGeometryPreviewSettings
{
    private readonly string _materialBaseName;

    private int _colorIndex;

    /// <inheritdoc/>
    /// <remarks>
    /// Clamped to the range AutoCAD accepts, so a color index typed into the user settings
    /// file by hand cannot make <see cref="EntityColor"/> throw somewhere far from here.
    /// </remarks>
    public int ColorIndex
    {
        get => _colorIndex;
        set
        {
            var colorIndex = this.Clamp(value);

            if (colorIndex == _colorIndex)
                return;

            _colorIndex = colorIndex;

            // The material held here was created for the previous color and is named after
            // it, so it is dropped until CreateMaterial makes one for the new color. Previews
            // drawn in the meantime skip the material rather than use the wrong one.
            this.MaterialId = AutocadObjectIdWrapper.DefaultId;
        }
    }

    /// <inheritdoc/>
    public byte Transparency { get; }

    /// <inheritdoc/>
    public IObjectId MaterialId { get; private set; }

    /// <inheritdoc/>
    /// <remarks>
    /// The color index is part of the name because <see cref="CreateMaterial"/> reuses any
    /// material already in the document's dictionary under this name. Were the name fixed, a
    /// color the user changed would never reach shaded previews in a document the old
    /// material had already been created in.
    /// </remarks>
    public string MaterialName => $"{_materialBaseName}.{this.ColorIndex}";

    /// <summary>
    /// Constructs a new <see cref="GeometryPreviewSettings"/>
    /// </summary>
    /// <param name="transparency">The transparency to draw previews with.</param>
    /// <param name="materialBaseName">
    /// The name of the preview material, which <see cref="MaterialName"/> qualifies with the
    /// current <see cref="ColorIndex"/>.
    /// </param>
    /// <param name="colorIndex">The AutoCAD Color Index to draw previews in.</param>
    public GeometryPreviewSettings(byte transparency, string materialBaseName, int colorIndex)
    {
        _materialBaseName = materialBaseName;
        _colorIndex = this.Clamp(colorIndex);
        this.Transparency = transparency;
        this.MaterialId = AutocadObjectIdWrapper.DefaultId;
    }

    /// <summary>
    /// Returns the given color index constrained to the range of AutoCAD Color Index values
    /// which name a color.
    /// </summary>
    private int Clamp(int colorIndex) =>
        Math.Min(ApplicationConstants.MaxAciColorIndex,
            Math.Max(ApplicationConstants.MinAciColorIndex, colorIndex));

    /// <inheritdoc/>
    public void CreateMaterial(IAutocadDocument document)
    {
        var transactionManagerWrapper = document.CreateTransactionManager();

        _ = transactionManagerWrapper.PerformTask(() =>
        {
            var transactionManager = transactionManagerWrapper.Unwrap();

            var materialDictionaryId = document.AutocadDatabase.Unwrap().MaterialDictionaryId;

            var dbDictionary =
                (DBDictionary)transactionManager.GetObject(materialDictionaryId, OpenMode.ForWrite);

            if (dbDictionary.Contains(this.MaterialName))
            {
                var existingMaterialId = dbDictionary.GetAt(this.MaterialName);

                // An erased entry can linger in the dictionary (e.g. after UNDO); fall
                // through to recreate the material so we never cache an erased id.
                if (existingMaterialId.IsErased == false)
                {
                    this.MaterialId = new AutocadObjectIdWrapper(existingMaterialId);
                    return true;
                }
            }

            var material = new Material
            {
                Name = this.MaterialName,
            };

            var entityColor = new EntityColor(this.ColorIndex);

            var materialColor = new MaterialColor(Method.Override, 1.0, entityColor);

            var diffuseComponent = new MaterialDiffuseComponent(materialColor, null);

            var materialMap = new MaterialMap();

            var specularComponent =
                new MaterialSpecularComponent(materialColor, materialMap, 0.5);

            var opacityComponent = new MaterialOpacityComponent(0.5, null);

            material.Diffuse = diffuseComponent;
            material.Ambient = materialColor;
            material.Specular = specularComponent;
            material.Opacity = opacityComponent;

            _ = dbDictionary.SetAt(material.Name, material);
            transactionManager.AddNewlyCreatedDBObject(material, true);

            this.MaterialId = new AutocadObjectIdWrapper(material.ObjectId);
            return true;
        });
    }

    /// <inheritdoc/>
    public bool HasMaterialFor(IAutocadDocument document)
    {
        var materialId = this.MaterialId.Unwrap();

        return materialId is { IsNull: false, IsValid: true, IsErased: false } &&
               materialId.Database == document.AutocadDatabase.Unwrap();
    }

    /// <inheritdoc/>
    /// <remarks>
    /// The cached preview material id can go stale (document switched or closed, UNDO past
    /// the material creation, or PURGE erasing the unreferenced material), so it is only
    /// applied when it is valid, not erased and belongs to the current working database.
    /// Applying the material is cosmetic and must never throw into the calling event handler.
    /// </remarks>
    public void ApplyTo(IEntity entity)
    {
        var autocadEntity = entity.Unwrap();

        autocadEntity.ColorIndex = this.ColorIndex;

        autocadEntity.LineWeight = LineWeight.LineWeight050;

        autocadEntity.Transparency = new Transparency(this.Transparency);

        try
        {
            var materialId = this.MaterialId.Unwrap();

            if (materialId is { IsNull: false, IsValid: true, IsErased: false } &&
                materialId.Database == HostApplicationServices.WorkingDatabase)
            {
                autocadEntity.MaterialId = materialId;
            }
        }
        catch (Autodesk.AutoCAD.Runtime.Exception exception)
        {
            LoggerService.Instance.LogMessage(
                $"Unable to apply preview material '{this.MaterialName}': {exception.Message}");
        }
    }
}
