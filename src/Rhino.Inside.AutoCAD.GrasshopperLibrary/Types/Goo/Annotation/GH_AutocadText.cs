using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using AutocadText = Autodesk.AutoCAD.DatabaseServices.MText;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD Texts.
/// </summary>
public class GH_AutocadText : GH_AutocadGeometricGoo<AutocadText, TextEntity>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadText"/> class with no value.
    /// </summary>
    public GH_AutocadText()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadText"/> class with the
    /// specified AutoCAD Text. Internally, the curve is cloned, but the autocad
    /// reference ID is maintained.
    /// </summary>
    /// <param name="Text">The AutoCAD Text to wrap.</param>
    public GH_AutocadText(AutocadText Text) : base(Text)
    {

    }

    /// <summary>
    /// A private constructor used to create a reference Goo which is a clone of the
    /// input curve.
    /// </summary>
    private GH_AutocadText(AutocadText curve, IAutocadReferenceId referenceId) : base(curve, referenceId)
    {
    }

    /// <inheritdoc />
    protected override GH_AutocadGeometricGoo<AutocadText, TextEntity> CreateClonedInstance(AutocadText entity)
    {
        return new GH_AutocadText(entity.Clone() as AutocadText, this.Reference);
    }

    /// <inheritdoc />
    protected override GH_AutocadGeometricGoo<AutocadText, TextEntity> CreateInstance(AutocadText entity)
    {
        return new GH_AutocadText(entity);
    }

    /// <inheritdoc />
    protected override AutocadText? Convert(TextEntity rhinoType)
    {
        return _geometryConverter.ToAutoCadType(rhinoType);
    }

    /// <inheritdoc />
    protected override TextEntity? Convert(AutocadText wrapperType)
    {
        return _geometryConverter.ToRhinoType(wrapperType);
    }

    /// <inheritdoc />
    protected override void DrawViewportGeometryWires(GH_PreviewWireArgs args)
    {
        args.Pipeline.DrawText(this.RhinoGeometry, args.Color, this.RhinoGeometry.DimensionScale);
    }

    /// <inheritdoc />
    protected override void DrawViewportGeometryMeshes(GH_PreviewMeshArgs args)
    {
        return;
    }

    /// <inheritdoc />
    public override void DrawAutocadPreview(IGrasshopperPreviewData previewData)
    {
        var rhinoGeometry = this.RhinoGeometry;

        if (rhinoGeometry == null) return;

        previewData.Texts.Add(rhinoGeometry);

    }
}

