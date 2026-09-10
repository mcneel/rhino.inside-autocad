using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using AutocadDimension = Autodesk.AutoCAD.DatabaseServices.Dimension;
using RhinoCurve = Rhino.Geometry.Curve;
using RhinoDimension = Rhino.Geometry.Dimension;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD Dimensions.
/// </summary>
public class GH_AutocadDimension : GH_AutocadGeometricGoo<AutocadDimension, RhinoGeometryAdapter<RhinoDimension>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadDimension"/> class with no value.
    /// </summary>
    public GH_AutocadDimension()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadDimension"/> class with the
    /// specified AutoCAD Dimension. Internally, the dimension is cloned, but the AutoCAD
    /// reference ID is maintained.
    /// </summary>
    /// <param name="dimension">The AutoCAD Dimension to wrap.</param>
    public GH_AutocadDimension(AutocadDimension dimension) : base(dimension)
    {
    }

    /// <summary>
    /// A private constructor used to create a reference Goo which is a clone of the
    /// input dimension.
    /// </summary>
    private GH_AutocadDimension(AutocadDimension dimension, IAutocadReferenceId referenceId) : base(dimension, referenceId)
    {
    }

    /// <inheritdoc />
    protected override GH_AutocadGeometricGoo<AutocadDimension, RhinoGeometryAdapter<RhinoDimension>> CreateClonedInstance(AutocadDimension entity)
    {
        return new GH_AutocadDimension(entity.Clone() as AutocadDimension, this.Reference);
    }

    /// <inheritdoc />
    protected override GH_AutocadGeometricGoo<AutocadDimension, RhinoGeometryAdapter<RhinoDimension>> CreateInstance(AutocadDimension entity)
    {
        return new GH_AutocadDimension(entity);
    }

    /// <inheritdoc />
    protected override AutocadDimension? Convert(RhinoGeometryAdapter<RhinoDimension> rhinoType)
    {
        return rhinoType.Geometry?.ToAutocadDimension();
    }

    /// <inheritdoc />
    protected override RhinoGeometryAdapter<RhinoDimension>? Convert(AutocadDimension wrapperType)
    {
        return new RhinoGeometryAdapter<RhinoDimension>(wrapperType.ToRhinoDimension());
    }

    /// <inheritdoc />
    protected override void DrawViewportGeometryWires(GH_PreviewWireArgs args)
    {
        var geometry = this.RhinoGeometry?.Geometry;
        if (geometry == null) return;

        args.Pipeline.DrawAnnotation(geometry, args.Color);
    }

    /// <inheritdoc />
    protected override void DrawViewportGeometryMeshes(GH_PreviewMeshArgs args)
    {
        return;
    }

    /// <inheritdoc />
    public override void AppendInputSignature(IInputSignatureBuilder inputSignatureBuilder)
    {
        var geometry = this.RhinoGeometry?.Geometry;

        if (geometry == null)
        {
            inputSignatureBuilder.Add(this.ToString());
            return;
        }

        inputSignatureBuilder.AddGeometry(geometry);
    }

    /// <inheritdoc />
    public override void DrawAutocadPreview(IGrasshopperPreviewData previewData)
    {
        var geometry = this.RhinoGeometry?.Geometry;
        if (geometry == null) return;

        var geometryBases = geometry.Explode();

        foreach (var geometryBase in geometryBases)
        {
            if (geometryBase is RhinoCurve curve)
            {
                previewData.Wires.Add(curve);
                continue;
            }

            if (geometryBase is TextEntity textEntity)
            {
                previewData.Texts.Add(textEntity);
                continue;
            }
        }
    }
}
