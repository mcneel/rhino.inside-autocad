using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using AutocadDimension = Autodesk.AutoCAD.DatabaseServices.Dimension;
using RhinoDimension = Rhino.Geometry.Dimension;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD Dimensions.
/// </summary>
public class GH_AutocadDimension : GH_AutocadGeometricGoo<AutocadDimension, RhinoDimension>
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
    private GH_AutocadDimension(AutocadDimension dimension, IObjectId referenceId) : base(dimension, referenceId)
    {
    }

    /// <inheritdoc />
    protected override GH_AutocadGeometricGoo<AutocadDimension, RhinoDimension> CreateClonedInstance(AutocadDimension entity)
    {
        return new GH_AutocadDimension(entity.Clone() as AutocadDimension, this.AutocadReferenceId);
    }

    /// <inheritdoc />
    protected override GH_AutocadGeometricGoo<AutocadDimension, RhinoDimension> CreateInstance(AutocadDimension entity)
    {
        return new GH_AutocadDimension(entity);
    }

    /// <inheritdoc />
    protected override AutocadDimension? Convert(RhinoDimension rhinoType)
    {
        return _geometryConverter.ToAutoCadType(rhinoType);
    }

    /// <inheritdoc />
    protected override RhinoDimension? Convert(AutocadDimension wrapperType)
    {
        return _geometryConverter.ToRhinoType(wrapperType);
    }

    /// <inheritdoc />
    protected override void DrawViewportGeometryWires(GH_PreviewWireArgs args)
    {
        var rhinoGeometry = this.RhinoGeometry;
        if (rhinoGeometry == null) return;

        args.Pipeline.DrawAnnotation(rhinoGeometry, args.Color);
    }

    /// <inheritdoc />
    protected override void DrawViewportGeometryMeshes(GH_PreviewMeshArgs args)
    {
        return;
    }

    /// <inheritdoc />
    public override void DrawAutocadPreview(IGrasshopperPreviewData previewData)
    {
        // Dimensions don't have a dedicated collection in preview data
        // The AutoCAD preview will be handled through the convertible system
    }
}
