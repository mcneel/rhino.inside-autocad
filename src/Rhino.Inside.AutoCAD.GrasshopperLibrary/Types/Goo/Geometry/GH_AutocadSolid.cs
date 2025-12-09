using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using AutocadSolid = Autodesk.AutoCAD.DatabaseServices.Solid3d;
using RhinoBrep = Rhino.Geometry.Brep;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD solids.
/// </summary>
public class GH_AutocadSolid : GH_AutocadGeometricGoo<AutocadSolid, RhinoBrep>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadSolid"/> class with no value.
    /// </summary>
    public GH_AutocadSolid()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadSolid"/> class with the
    /// specified AutoCAD solid. Internally, the curve is cloned, but the autocad
    /// reference ID is maintained.
    /// </summary>
    /// <param name="solid">The AutoCAD solid to wrap.</param>
    public GH_AutocadSolid(AutocadSolid solid) : base(solid)
    {

    }

    /// <summary>
    /// A private constructor used to create a reference Goo which is a clone of the
    /// input curve.
    /// </summary>
    private GH_AutocadSolid(AutocadSolid curve, IObjectId referenceId) : base(curve, referenceId)
    {
    }

    /// <inheritdoc />
    protected override GH_AutocadGeometricGoo<AutocadSolid, RhinoBrep> CreateClonedInstance(AutocadSolid entity)
    {
        return new GH_AutocadSolid(entity.Clone() as AutocadSolid, this.AutocadReferenceId);
    }

    /// <inheritdoc />
    protected override GH_AutocadGeometricGoo<AutocadSolid, RhinoBrep> CreateInstance(AutocadSolid entity)
    {
        return new GH_AutocadSolid(entity);
    }

    /// <inheritdoc />
    protected override AutocadSolid? Convert(RhinoBrep rhinoType)
    {
        //TODO: This should be updated when we have a proper AutoCAD to Rhino Brep Conversation implementation

        return _geometryConverter.ToAutoCadType(rhinoType)[0];
    }

    /// <inheritdoc />
    protected override RhinoBrep? Convert(AutocadSolid wrapperType)
    {
        //TODO: This should be updated when we have a proper AutoCAD to Rhino Brep Conversation implementation

        return _geometryConverter.ToRhinoType(wrapperType)[0];
    }

    /// <inheritdoc />
    protected override void DrawViewportGeometryWires(GH_PreviewWireArgs args)
    {

        args.Pipeline.DrawBrepWires(this.RhinoGeometry, args.Color);
    }

    /// <inheritdoc />
    protected override void DrawViewportGeometryMeshes(GH_PreviewMeshArgs args)
    {
        args.Pipeline.DrawBrepShaded(this.RhinoGeometry, args.Material);
    }
}

