using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.GraphicsInterface;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IGeometryPreviewSettings"/>
public class GeometryPreviewSettings : IGeometryPreviewSettings
{
    /// <inheritdoc/>
    public int ColorIndex { get; }

    /// <inheritdoc/>
    public byte Transparency { get; }

    /// <inheritdoc/>
    public IObjectId MaterialId { get; private set; }

    /// <inheritdoc/>
    public string MaterialName { get; }

    /// <summary>
    /// Constructs a new <see cref="GeometryPreviewSettings"/>
    /// </summary>
    public GeometryPreviewSettings(byte transparency, string materialName, int colorIndex)
    {
        this.ColorIndex = colorIndex;
        this.Transparency = transparency;
        this.MaterialId = new AutocadObjectId();
        this.MaterialName = materialName;
    }

    /// <inheritdoc/>
    public void CreateMaterial(IAutocadDocument document)
    {
        _ = document.Transaction(transactionManagerWrapper =>
        {
            var transactionManager = transactionManagerWrapper.Unwrap();

            using var dbDictionary =
                (DBDictionary)transactionManager.GetObject(document.Database.Unwrap().MaterialDictionaryId,
                    OpenMode.ForWrite);

            if (dbDictionary.Contains(this.MaterialName))
            {
                var existingMaterialId = dbDictionary.GetAt(this.MaterialName);

                this.MaterialId = new AutocadObjectId(existingMaterialId);
                return true;
            }

            var material = new Material
            {
                Name = this.MaterialName,
            };



            var materialColor =
                new MaterialColor(Method.Override, 1.0,
                    new EntityColor(EntityColor.LookUpRgb(Convert.ToByte(this.ColorIndex))));

            material.Diffuse = new MaterialDiffuseComponent(materialColor, null);

            material.Ambient = materialColor;
            material.Specular =
                new MaterialSpecularComponent(materialColor, new MaterialMap(), 0.5);
            material.Opacity = new MaterialOpacityComponent(0.5, null);

            _ = dbDictionary.SetAt(material.Name, material);
            transactionManager.AddNewlyCreatedDBObject(material, true);

            this.MaterialId = new AutocadObjectId(material.ObjectId);
            return true;
        });
    }
}