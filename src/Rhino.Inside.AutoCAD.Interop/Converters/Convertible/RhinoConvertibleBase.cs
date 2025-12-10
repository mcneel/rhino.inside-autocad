using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A base class for <see cref="IRhinoConvertibleTyped{T}"/> types.
/// </summary>
public abstract class RhinoConvertibleBase<TRhinoType> : IRhinoConvertibleTyped<TRhinoType>
    where TRhinoType : Rhino.Geometry.GeometryBase
{
    /// <inheritdoc />
    public TRhinoType RhinoGeometry { get; }

    /// <summary>
    /// Constructs a new <see cref="RhinoConvertibleBase{TRhinoType}"/> instance.
    /// </summary>
    protected RhinoConvertibleBase(TRhinoType rhinoGeometry)
    {
        this.RhinoGeometry = rhinoGeometry;
    }

    /// <summary>
    /// Applies the preview settings to the given entity.
    /// </summary>
    private void ApplySettings(IEntity entity, IGeometryPreviewSettings previewSettings)
    {
        var autocadEntity = entity.Unwrap();

        var materialId = previewSettings.MaterialId.Unwrap();

        autocadEntity.ColorIndex = previewSettings.ColorIndex;

        autocadEntity.LineWeight = LineWeight.LineWeight050;

        autocadEntity.Transparency = new Transparency(previewSettings.Transparency);

        if (materialId.IsValid)
        {
            autocadEntity.MaterialId = materialId;
        }
    }

    /// <summary>
    /// Converts the Rhino geometry to AutoCAD entities.
    /// </summary>
    protected abstract List<IEntity> ConvertGeometry(ITransactionManager transactionManager);

    /// <inheritdoc />
    public List<IEntity> Convert(ITransactionManager transactionManager,
        IGeometryPreviewSettings previewSettings)
    {
        var converted = this.ConvertGeometry(transactionManager);

        foreach (var convertedEntity in converted)
        {
            this.ApplySettings(convertedEntity, previewSettings);
        }
        return converted;
    }

    /// <inheritdoc />
    public List<IEntity> Convert(ITransactionManager transactionManager)
    {
        return this.ConvertGeometry(transactionManager);
    }
}