using Autodesk.AutoCAD.DatabaseServices;
using CadAlignedDimension = Autodesk.AutoCAD.DatabaseServices.AlignedDimension;
using CadArcDimension = Autodesk.AutoCAD.DatabaseServices.ArcDimension;
using CadDiametricDimension = Autodesk.AutoCAD.DatabaseServices.DiametricDimension;
using CadDimension = Autodesk.AutoCAD.DatabaseServices.Dimension;
using CadLeader = Autodesk.AutoCAD.DatabaseServices.Leader;
using CadLineAngularDimension2 = Autodesk.AutoCAD.DatabaseServices.LineAngularDimension2;
using CadMLeader = Autodesk.AutoCAD.DatabaseServices.MLeader;
using CadOrdinateDimension = Autodesk.AutoCAD.DatabaseServices.OrdinateDimension;
using CadPoint3AngularDimension = Autodesk.AutoCAD.DatabaseServices.Point3AngularDimension;
using CadRadialDimension = Autodesk.AutoCAD.DatabaseServices.RadialDimension;
using CadRotatedDimension = Autodesk.AutoCAD.DatabaseServices.RotatedDimension;
using RhinoAngularDimension = Rhino.Geometry.AngularDimension;
using RhinoAnnotationType = Rhino.Geometry.AnnotationType;
using RhinoCentermark = Rhino.Geometry.Centermark;
using RhinoDimension = Rhino.Geometry.Dimension;
using RhinoLeader = Rhino.Geometry.Leader;
using RhinoLinearDimension = Rhino.Geometry.LinearDimension;
using RhinoMeasuredDirection = Rhino.Geometry.OrdinateDimension.MeasuredDirection;
using RhinoOrdinateDimension = Rhino.Geometry.OrdinateDimension;
using RhinoPlane = Rhino.Geometry.Plane;
using RhinoPoint3d = Rhino.Geometry.Point3d;
using RhinoRadialDimension = Rhino.Geometry.RadialDimension;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Provides extension methods for converting AutoCAD dimension types to Rhino dimension types.
/// </summary>
public static class AutocadDimensionExtensions
{
    /// <summary>
    /// Converts any AutoCAD <see cref="CadDimension"/> to the appropriate Rhino dimension type.
    /// </summary>
    /// <param name="cadDimension">The AutoCAD dimension to convert.</param>
    /// <returns>A Rhino dimension, or null if the dimension type is not supported.</returns>
    public static RhinoDimension? ToRhinoDimension(this CadDimension cadDimension)
    {
        return cadDimension switch
        {
            CadAlignedDimension aligned => aligned.ToRhinoLinearDimension(),
            CadRotatedDimension rotated => rotated.ToRhinoLinearDimension(),
            CadLineAngularDimension2 lineAngular => lineAngular.ToRhinoAngularDimension(),
            CadPoint3AngularDimension point3Angular => point3Angular.ToRhinoAngularDimension(),
            CadArcDimension arcDim => arcDim.ToRhinoAngularDimension(),
            CadRadialDimension radial => radial.ToRhinoRadialDimension(),
            CadDiametricDimension diametric => diametric.ToRhinoRadialDimension(),
            CadOrdinateDimension ordinate => ordinate.ToRhinoOrdinateDimension(),
            _ => null
        };
    }

    /// <summary>
    /// Converts a <see cref="CadRotatedDimension"/> to a <see cref="RhinoLinearDimension"/>.
    /// </summary>
    /// <param name="cadDimension">The AutoCAD rotated dimension to convert.</param>
    /// <returns>A Rhino linear dimension with coordinates scaled to Rhino units.</returns>
    public static RhinoLinearDimension? ToRhinoLinearDimension(this CadRotatedDimension cadDimension)
    {
        var xLine1Point = cadDimension.XLine1Point.ToRhinoPoint3d();
        var xLine2Point = cadDimension.XLine2Point.ToRhinoPoint3d();
        var dimLinePoint = cadDimension.DimLinePoint.ToRhinoPoint3d();

        var normal = cadDimension.Normal.ToRhinoVector3d();
        var plane = new RhinoPlane(xLine1Point, normal);

        var horizontal = plane.XAxis;

        var dimension = RhinoLinearDimension.Create(
            RhinoAnnotationType.Rotated,
            RhinoDoc.ActiveDoc.DimStyles.Current,
            plane,
            horizontal,
            xLine1Point,
            xLine2Point,
            dimLinePoint,
            cadDimension.Rotation);

        if (dimension != null)
            dimension.DimensionScale = 1.0;

        return dimension;
    }

    /// <summary>
    /// Converts a <see cref="CadAlignedDimension"/> to a <see cref="RhinoLinearDimension"/>.
    /// </summary>
    /// <param name="cadDimension">The AutoCAD aligned dimension to convert.</param>
    /// <returns>A Rhino linear dimension with coordinates scaled to Rhino units.</returns>
    public static RhinoLinearDimension? ToRhinoLinearDimension(this CadAlignedDimension cadDimension)
    {
        var xLine1Point = cadDimension.XLine1Point.ToRhinoPoint3d();
        var xLine2Point = cadDimension.XLine2Point.ToRhinoPoint3d();
        var dimLinePoint = cadDimension.DimLinePoint.ToRhinoPoint3d();

        var normal = cadDimension.Normal.ToRhinoVector3d();
        var plane = new RhinoPlane(xLine1Point, normal);

        var horizontal = xLine2Point - xLine1Point;
        horizontal.Unitize();

        var dimension = RhinoLinearDimension.Create(
            RhinoAnnotationType.Aligned,
            RhinoDoc.ActiveDoc.DimStyles.Current,
            plane,
            horizontal,
            xLine1Point,
            xLine2Point,
            dimLinePoint,
            0.0);

        if (dimension != null)
            dimension.DimensionScale = 1.0;

        return dimension;
    }

    /// <summary>
    /// Converts a <see cref="CadLineAngularDimension2"/> to a <see cref="RhinoAngularDimension"/>.
    /// </summary>
    /// <param name="cadDimension">The AutoCAD line angular dimension to convert.</param>
    /// <returns>A Rhino angular dimension with coordinates scaled to Rhino units.</returns>
    public static RhinoAngularDimension? ToRhinoAngularDimension(this CadLineAngularDimension2 cadDimension)
    {
        var xLine1Start = cadDimension.XLine1Start.ToRhinoPoint3d();
        var xLine1End = cadDimension.XLine1End.ToRhinoPoint3d();
        var xLine2Start = cadDimension.XLine2Start.ToRhinoPoint3d();
        var xLine2End = cadDimension.XLine2End.ToRhinoPoint3d();
        var arcPoint = cadDimension.ArcPoint.ToRhinoPoint3d();

        var normal = cadDimension.Normal.ToRhinoVector3d();
        var plane = new RhinoPlane(xLine1Start, normal);

        var horizontal = plane.XAxis;

        var dimension = RhinoAngularDimension.Create(
            RhinoDoc.ActiveDoc.DimStyles.Current,
            plane,
            horizontal,
            xLine1Start,
            xLine1End,
            xLine2End,
            arcPoint);

        if (dimension != null)
            dimension.DimensionScale = 1.0;

        return dimension;
    }

    /// <summary>
    /// Converts a <see cref="CadPoint3AngularDimension"/> to a <see cref="RhinoAngularDimension"/>.
    /// </summary>
    /// <param name="cadDimension">The AutoCAD 3-point angular dimension to convert.</param>
    /// <returns>A Rhino angular dimension with coordinates scaled to Rhino units.</returns>
    public static RhinoAngularDimension? ToRhinoAngularDimension(this CadPoint3AngularDimension cadDimension)
    {
        var centerPoint = cadDimension.CenterPoint.ToRhinoPoint3d();
        var xLine1Point = cadDimension.XLine1Point.ToRhinoPoint3d();
        var xLine2Point = cadDimension.XLine2Point.ToRhinoPoint3d();
        var arcPoint = cadDimension.ArcPoint.ToRhinoPoint3d();

        var normal = cadDimension.Normal.ToRhinoVector3d();
        var plane = new RhinoPlane(centerPoint, normal);

        var horizontal = plane.XAxis;

        var dimension = RhinoAngularDimension.Create(
            RhinoDoc.ActiveDoc.DimStyles.Current,
            plane,
            horizontal,
            centerPoint,
            xLine1Point,
            xLine2Point,
            arcPoint);

        if (dimension != null)
            dimension.DimensionScale = 1.0;

        return dimension;
    }

    /// <summary>
    /// Converts a <see cref="CadArcDimension"/> to a <see cref="RhinoAngularDimension"/>.
    /// </summary>
    /// <param name="cadDimension">The AutoCAD arc dimension to convert.</param>
    /// <returns>A Rhino angular dimension with coordinates scaled to Rhino units.</returns>
    /// <remarks>
    /// This is a partial conversion - arc length dimensions are converted to angular dimensions.
    /// </remarks>
    public static RhinoAngularDimension? ToRhinoAngularDimension(this CadArcDimension cadDimension)
    {
        var centerPoint = cadDimension.CenterPoint.ToRhinoPoint3d();
        var xLine1Point = cadDimension.XLine1Point.ToRhinoPoint3d();
        var xLine2Point = cadDimension.XLine2Point.ToRhinoPoint3d();
        var arcPoint = cadDimension.ArcPoint.ToRhinoPoint3d();

        var normal = cadDimension.Normal.ToRhinoVector3d();
        var plane = new RhinoPlane(centerPoint, normal);

        var horizontal = plane.XAxis;

        var dimension = RhinoAngularDimension.Create(
            RhinoDoc.ActiveDoc.DimStyles.Current,
            plane,
            horizontal,
            centerPoint,
            xLine1Point,
            xLine2Point,
            arcPoint);

        if (dimension != null)
            dimension.DimensionScale = 1.0;

        return dimension;
    }

    /// <summary>
    /// Converts a <see cref="CadRadialDimension"/> to a <see cref="RhinoRadialDimension"/>.
    /// </summary>
    /// <param name="cadDimension">The AutoCAD radial dimension to convert.</param>
    /// <returns>A Rhino radial dimension with coordinates scaled to Rhino units.</returns>
    public static RhinoRadialDimension? ToRhinoRadialDimension(this CadRadialDimension cadDimension)
    {
        var center = cadDimension.Center.ToRhinoPoint3d();
        var chordPoint = cadDimension.ChordPoint.ToRhinoPoint3d();

        var normal = cadDimension.Normal.ToRhinoVector3d();
        var plane = new RhinoPlane(center, normal);

        var dimension = RhinoRadialDimension.Create(
            RhinoDoc.ActiveDoc.DimStyles.Current,
            RhinoAnnotationType.Radius,
            plane,
            center,
            chordPoint,
            chordPoint);

        if (dimension != null)
            dimension.DimensionScale = 1.0;

        return dimension;
    }

    /// <summary>
    /// Converts a <see cref="CadDiametricDimension"/> to a <see cref="RhinoRadialDimension"/>.
    /// </summary>
    /// <param name="cadDimension">The AutoCAD diametric dimension to convert.</param>
    /// <returns>A Rhino radial dimension (diameter type) with coordinates scaled to Rhino units.</returns>
    /// <remarks>
    /// The resulting RadialDimension will have IsDiameterDimension set appropriately.
    /// </remarks>
    public static RhinoRadialDimension? ToRhinoRadialDimension(this CadDiametricDimension cadDimension)
    {
        var chordPoint = cadDimension.ChordPoint.ToRhinoPoint3d();
        var farChordPoint = cadDimension.FarChordPoint.ToRhinoPoint3d();

        var center = new RhinoPoint3d(
            (chordPoint.X + farChordPoint.X) / 2.0,
            (chordPoint.Y + farChordPoint.Y) / 2.0,
            (chordPoint.Z + farChordPoint.Z) / 2.0);

        var normal = cadDimension.Normal.ToRhinoVector3d();
        var plane = new RhinoPlane(center, normal);

        var dimension = RhinoRadialDimension.Create(
            RhinoDoc.ActiveDoc.DimStyles.Current,
            RhinoAnnotationType.Diameter,
            plane,
            center,
            chordPoint,
            chordPoint);

        if (dimension != null)
            dimension.DimensionScale = 1.0;

        return dimension;
    }

    /// <summary>
    /// Converts a <see cref="CadOrdinateDimension"/> to a <see cref="RhinoOrdinateDimension"/>.
    /// </summary>
    /// <param name="cadDimension">The AutoCAD ordinate dimension to convert.</param>
    /// <returns>A Rhino ordinate dimension with coordinates scaled to Rhino units.</returns>
    public static RhinoOrdinateDimension? ToRhinoOrdinateDimension(this CadOrdinateDimension cadDimension)
    {
        var origin = cadDimension.Origin.ToRhinoPoint3d();
        var definingPoint = cadDimension.DefiningPoint.ToRhinoPoint3d();
        var leaderEndPoint = cadDimension.LeaderEndPoint.ToRhinoPoint3d();

        var normal = cadDimension.Normal.ToRhinoVector3d();
        var plane = new RhinoPlane(origin, normal);

        var direction = cadDimension.UsingXAxis
            ? RhinoMeasuredDirection.Xaxis
            : RhinoMeasuredDirection.Yaxis;

        var dimension = RhinoOrdinateDimension.Create(
            RhinoDoc.ActiveDoc.DimStyles.Current,
            plane,
            direction,
            origin,
            definingPoint,
            leaderEndPoint,
            0.0,
            0.0);

        if (dimension != null)
            dimension.DimensionScale = 1.0;

        return dimension;
    }

    /// <summary>
    /// Converts a <see cref="CadLeader"/> to a <see cref="RhinoLeader"/>.
    /// </summary>
    /// <param name="cadLeader">The AutoCAD leader to convert.</param>
    /// <returns>A Rhino leader with coordinates scaled to Rhino units.</returns>
    public static RhinoLeader? ToRhinoLeader(this CadLeader cadLeader)
    {
        var vertices = new List<RhinoPoint3d>();

        for (var i = 0; i < cadLeader.NumVertices; i++)
        {
            var vertex = cadLeader.VertexAt(i).ToRhinoPoint3d();
            vertices.Add(vertex);
        }

        if (vertices.Count < 2)
            return null;

        var normal = cadLeader.Normal.ToRhinoVector3d();
        var plane = new RhinoPlane(vertices[0], normal);

        var dimension = RhinoLeader.Create(
            string.Empty,
            plane,
            RhinoDoc.ActiveDoc.DimStyles.Current,
            vertices.ToArray());

        if (dimension != null)
            dimension.DimensionScale = 1.0;

        return dimension;
    }

    /// <summary>
    /// Converts a <see cref="CadMLeader"/> to a <see cref="RhinoLeader"/>.
    /// </summary>
    /// <param name="cadMLeader">The AutoCAD multileader to convert.</param>
    /// <returns>A Rhino leader with coordinates scaled to Rhino units.</returns>
    public static RhinoLeader? ToRhinoLeader(this CadMLeader cadMLeader)
    {
        if (cadMLeader.LeaderLineCount == 0)
            return null;

        var vertices = new List<RhinoPoint3d>();

        var leaderIndexes = cadMLeader.GetLeaderIndexes();
        if (leaderIndexes.Count == 0)
            return null;

        var leaderIndex = (int)leaderIndexes[0];
        var lineIndexes = cadMLeader.GetLeaderLineIndexes(leaderIndex);
        if (lineIndexes.Count == 0)
            return null;

        var lineIndex = (int)lineIndexes[0];

        for (var i = 0; i < cadMLeader.VerticesCount(lineIndex); i++)
        {
            var vertex = cadMLeader.GetVertex(lineIndex, i).ToRhinoPoint3d();
            vertices.Add(vertex);
        }

        if (vertices.Count < 2)
            return null;

        var normal = cadMLeader.Normal.ToRhinoVector3d();
        var plane = new RhinoPlane(vertices[0], normal);

        var text = cadMLeader.ContentType == ContentType.MTextContent
            ? cadMLeader.MText?.Text ?? string.Empty
            : string.Empty;

        var dimension = RhinoLeader.Create(
            text,
            plane,
            RhinoDoc.ActiveDoc.DimStyles.Current,
            vertices.ToArray());

        if (dimension != null)
            dimension.DimensionScale = 1.0;

        return dimension;
    }

    /// <summary>
    /// Converts a <see cref="CadMLeader"/> used as a center mark to a <see cref="RhinoCentermark"/>.
    /// </summary>
    /// <param name="cadMLeader">The AutoCAD multileader (as center mark) to convert.</param>
    /// <returns>A Rhino centermark with coordinates scaled to Rhino units.</returns>
    public static RhinoCentermark? ToRhinoCentermark(this CadMLeader cadMLeader)
    {
        if (cadMLeader.ContentType != ContentType.BlockContent)
            return null;

        var position = cadMLeader.BlockPosition.ToRhinoPoint3d();
        var normal = cadMLeader.Normal.ToRhinoVector3d();
        var plane = new RhinoPlane(position, normal);

        var centermark = RhinoCentermark.Create(
            RhinoDoc.ActiveDoc.DimStyles.Current,
            plane,
            position,
            1.0);

        if (centermark != null)
            centermark.DimensionScale = 1.0;

        return centermark;
    }
}
