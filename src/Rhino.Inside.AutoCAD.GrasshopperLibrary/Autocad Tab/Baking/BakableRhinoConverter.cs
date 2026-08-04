using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using RhinoGeometryBase = Rhino.Geometry.GeometryBase;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A class that bakes Rhino convertible objects to AutoCAD.
/// </summary>
public class BakableRhinoConverter : IAutocadBakeable
{
    private readonly IRhinoConvertible rhinoConvertible;
    private readonly RhinoGeometryBase _sourceGeometry;

    /// <summary>
    /// Constructs a new <see cref="BakableRhinoConverter"/> instance.
    /// </summary>
    /// <param name="rhinoConvertible">The convertible that produces the baked AutoCAD entities.</param>
    /// <param name="sourceGeometry">The Rhino geometry the convertible wraps, used for input signatures.</param>
    public BakableRhinoConverter(IRhinoConvertible rhinoConvertible, RhinoGeometryBase sourceGeometry)
    {
        this.rhinoConvertible = rhinoConvertible;
        _sourceGeometry = sourceGeometry;
    }

    /// <summary>
    /// Applies the given settings to the block reference.
    /// </summary>
    protected void ApplySettings(IBakeSettings? settings, Entity entity)
    {
        if (settings is null) return;

        if (settings.Layer != null)
            entity.LayerId = settings.Layer.Id.Unwrap();

        if (settings?.LineType != null)
            entity.LinetypeId = settings.LineType.Id.Unwrap();

        if (settings?.Color != null)
        {
            var color = settings.Color;
            entity.Color = color.Unwrap();
        }

        if (settings?.LinetypeScale is double linetypeScale)
            entity.LinetypeScale = linetypeScale;
    }

    /// <inheritdoc />
    public List<IObjectId> BakeToAutocad(IAutocadTransactionManager autocadTransactionManager,
        IBakingComponent bakingComponent, IBakeSettings? settings = null)
    {
        var convert = rhinoConvertible.Convert(autocadTransactionManager);

        var transaction = autocadTransactionManager.Unwrap();

        var modelSpace = autocadTransactionManager.GetModelSpace(openForWrite: true);

        var modelSpaceRecord = modelSpace.Unwrap();

        var idList = new List<IObjectId>();
        foreach (var entity in convert)
        {

            var cadEntity = entity.Unwrap();

            this.ApplySettings(settings, cadEntity);

            var objectId = modelSpaceRecord.AppendEntity(cadEntity);

            idList.Add(new AutocadObjectIdWrapper(objectId));

            transaction.AddNewlyCreatedDBObject(cadEntity, true);
        }

        return idList;
    }

    /// <inheritdoc />
    public void AppendInputSignature(IInputSignatureBuilder inputSignatureBuilder)
    {
        inputSignatureBuilder.AddGeometry(_sourceGeometry);
    }
}