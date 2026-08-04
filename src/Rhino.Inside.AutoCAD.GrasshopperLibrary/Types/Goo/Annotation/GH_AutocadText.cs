using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using AutocadDBText = Autodesk.AutoCAD.DatabaseServices.DBText;
using AutocadText = Autodesk.AutoCAD.DatabaseServices.MText;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD Texts.
/// </summary>
public class GH_AutocadText : GH_AutocadGeometricGoo<AutocadText, RhinoGeometryAdapter<TextEntity>>
{
    /// <summary>
    /// Creates a GH_AutocadText from an AutoCAD text entity, which can be either a DBText or an MText.
    /// If the input is a DBText, it will be converted to an MText for consistency.
    /// </summary>
    public static GH_AutocadText CreateFromTextEntity(IEntity textEntity)
    {
        switch (textEntity)
        {
            case AutocadDBText text:
                {
                    var mtext = text.ConvertToMText();

                    return new GH_AutocadText(mtext);
                }
            case AutocadText mText:
                return new GH_AutocadText(mText);
            default:
                throw new ArgumentException($"Unsupported text entity type: {textEntity.GetType().FullName}");
        }
    }

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
    /// <param name="text">The AutoCAD Text to wrap.</param>
    public GH_AutocadText(AutocadText text) : base(text)
    {

    }

    /// <summary>
    /// A private constructor used to create a reference Goo which is a clone of the
    /// input curve.
    /// </summary>
    private GH_AutocadText(AutocadText text, IAutocadReferenceId referenceId) : base(text, referenceId)
    {
    }

    /// <inheritdoc />
    protected override GH_AutocadGeometricGoo<AutocadText, RhinoGeometryAdapter<TextEntity>> CreateClonedInstance(AutocadText entity)
    {
        return new GH_AutocadText(entity.Clone() as AutocadText, this.Reference);
    }

    /// <inheritdoc />
    protected override GH_AutocadGeometricGoo<AutocadText, RhinoGeometryAdapter<TextEntity>> CreateInstance(AutocadText entity)
    {
        return new GH_AutocadText(entity);
    }

    /// <inheritdoc />
    protected override AutocadText? Convert(RhinoGeometryAdapter<TextEntity> rhinoType)
    {
        return rhinoType.Geometry?.ToAutocadMText();
    }

    /// <inheritdoc />
    protected override RhinoGeometryAdapter<TextEntity>? Convert(AutocadText wrapperType)
    {
        return new RhinoGeometryAdapter<TextEntity>(wrapperType.ToRhinoTextEntity());
    }

    /// <inheritdoc />
    protected override void DrawViewportGeometryWires(GH_PreviewWireArgs args)
    {
        var geometry = this.RhinoGeometry?.Geometry;
        if (geometry != null)
            args.Pipeline.DrawText(geometry, args.Color, geometry.DimensionScale);
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

        previewData.Texts.Add(geometry);
    }
}

