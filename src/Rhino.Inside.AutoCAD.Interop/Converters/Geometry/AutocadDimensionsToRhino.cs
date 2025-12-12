using Autodesk.AutoCAD.DatabaseServices;
using CadAlignedDimension = Autodesk.AutoCAD.DatabaseServices.AlignedDimension;
using CadArcDimension = Autodesk.AutoCAD.DatabaseServices.ArcDimension;
using CadDiametricDimension = Autodesk.AutoCAD.DatabaseServices.DiametricDimension;
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
using RhinoLeader = Rhino.Geometry.Leader;
using RhinoLinearDimension = Rhino.Geometry.LinearDimension;
using RhinoMeasuredDirection = Rhino.Geometry.OrdinateDimension.MeasuredDirection;
using RhinoOrdinateDimension = Rhino.Geometry.OrdinateDimension;
using RhinoPlane = Rhino.Geometry.Plane;
using RhinoPoint3d = Rhino.Geometry.Point3d;
using RhinoRadialDimension = Rhino.Geometry.RadialDimension;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Partial class containing dimension conversion methods from AutoCAD to Rhino.
/// </summary>
public partial class GeometryConverter
{
    /// <summary>
    /// Converts a <see cref="CadRotatedDimension"/> to a <see cref="RhinoLinearDimension"/>.
    /// </summary>
    public RhinoLinearDimension? ToRhinoType(CadRotatedDimension cadDimension)
    {
        var xLine1Point = this.ToRhinoType(cadDimension.XLine1Point);
        var xLine2Point = this.ToRhinoType(cadDimension.XLine2Point);
        var dimLinePoint = this.ToRhinoType(cadDimension.DimLinePoint);

        var normal = this.ToRhinoType(cadDimension.Normal);
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

        return dimension;
    }

    /// <summary>
    /// Converts a <see cref="CadAlignedDimension"/> to a <see cref="RhinoLinearDimension"/>.
    /// </summary>
    public RhinoLinearDimension? ToRhinoType(CadAlignedDimension cadDimension)
    {
        var xLine1Point = this.ToRhinoType(cadDimension.XLine1Point);
        var xLine2Point = this.ToRhinoType(cadDimension.XLine2Point);
        var dimLinePoint = this.ToRhinoType(cadDimension.DimLinePoint);

        var normal = this.ToRhinoType(cadDimension.Normal);
        var plane = new RhinoPlane(xLine1Point, normal);

        var horizontal = plane.XAxis;

        var dimension = RhinoLinearDimension.Create(
            RhinoAnnotationType.Aligned,
            RhinoDoc.ActiveDoc.DimStyles.Current,
            plane,
            horizontal,
            xLine1Point,
            xLine2Point,
            dimLinePoint,
            0.0);

        return dimension;
    }

    /// <summary>
    /// Converts a <see cref="CadLineAngularDimension2"/> to a <see cref="RhinoAngularDimension"/>.
    /// </summary>
    public RhinoAngularDimension? ToRhinoType(CadLineAngularDimension2 cadDimension)
    {
        var xLine1Start = this.ToRhinoType(cadDimension.XLine1Start);
        var xLine1End = this.ToRhinoType(cadDimension.XLine1End);
        var xLine2Start = this.ToRhinoType(cadDimension.XLine2Start);
        var xLine2End = this.ToRhinoType(cadDimension.XLine2End);
        var arcPoint = this.ToRhinoType(cadDimension.ArcPoint);

        var normal = this.ToRhinoType(cadDimension.Normal);
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

        return dimension;
    }

    /// <summary>
    /// Converts a <see cref="CadPoint3AngularDimension"/> to a <see cref="RhinoAngularDimension"/>.
    /// </summary>
    public RhinoAngularDimension? ToRhinoType(CadPoint3AngularDimension cadDimension)
    {
        var centerPoint = this.ToRhinoType(cadDimension.CenterPoint);
        var xLine1Point = this.ToRhinoType(cadDimension.XLine1Point);
        var xLine2Point = this.ToRhinoType(cadDimension.XLine2Point);
        var arcPoint = this.ToRhinoType(cadDimension.ArcPoint);

        var normal = this.ToRhinoType(cadDimension.Normal);
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

        return dimension;
    }

    /// <summary>
    /// Converts a <see cref="CadArcDimension"/> to a <see cref="RhinoAngularDimension"/>.
    /// </summary>
    /// <remarks>
    /// This is a partial conversion - arc length dimensions are converted to angular dimensions.
    /// </remarks>
    public RhinoAngularDimension? ToRhinoType(CadArcDimension cadDimension)
    {
        var centerPoint = this.ToRhinoType(cadDimension.CenterPoint);
        var xLine1Point = this.ToRhinoType(cadDimension.XLine1Point);
        var xLine2Point = this.ToRhinoType(cadDimension.XLine2Point);
        var arcPoint = this.ToRhinoType(cadDimension.ArcPoint);

        var normal = this.ToRhinoType(cadDimension.Normal);
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

        return dimension;
    }

    /// <summary>
    /// Converts a <see cref="CadRadialDimension"/> to a <see cref="RhinoRadialDimension"/>.
    /// </summary>
    public RhinoRadialDimension? ToRhinoType(CadRadialDimension cadDimension)
    {
        var center = this.ToRhinoType(cadDimension.Center);
        var chordPoint = this.ToRhinoType(cadDimension.ChordPoint);

        var normal = this.ToRhinoType(cadDimension.Normal);
        var plane = new RhinoPlane(center, normal);

        var dimension = RhinoRadialDimension.Create(
            RhinoDoc.ActiveDoc.DimStyles.Current,
            RhinoAnnotationType.Radius,
            plane,
            center,
            chordPoint,
            chordPoint);

        return dimension;
    }

    /// <summary>
    /// Converts a <see cref="CadDiametricDimension"/> to a <see cref="RhinoRadialDimension"/>.
    /// </summary>
    /// <remarks>
    /// The resulting RadialDimension will have IsDiameterDimension set appropriately.
    /// </remarks>
    public RhinoRadialDimension? ToRhinoType(CadDiametricDimension cadDimension)
    {
        var chordPoint = this.ToRhinoType(cadDimension.ChordPoint);
        var farChordPoint = this.ToRhinoType(cadDimension.FarChordPoint);

        var center = new RhinoPoint3d(
            (chordPoint.X + farChordPoint.X) / 2.0,
            (chordPoint.Y + farChordPoint.Y) / 2.0,
            (chordPoint.Z + farChordPoint.Z) / 2.0);

        var normal = this.ToRhinoType(cadDimension.Normal);
        var plane = new RhinoPlane(center, normal);

        var dimension = RhinoRadialDimension.Create(
            RhinoDoc.ActiveDoc.DimStyles.Current,
            RhinoAnnotationType.Diameter,
            plane,
            center,
            chordPoint,
            chordPoint);

        return dimension;
    }

    /// <summary>
    /// Converts a <see cref="CadOrdinateDimension"/> to a <see cref="RhinoOrdinateDimension"/>.
    /// </summary>
    public RhinoOrdinateDimension? ToRhinoType(CadOrdinateDimension cadDimension)
    {
        var origin = this.ToRhinoType(cadDimension.Origin);
        var definingPoint = this.ToRhinoType(cadDimension.DefiningPoint);
        var leaderEndPoint = this.ToRhinoType(cadDimension.LeaderEndPoint);

        var normal = this.ToRhinoType(cadDimension.Normal);
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

        return dimension;
    }

    /// <summary>
    /// Converts a <see cref="CadLeader"/> to a <see cref="RhinoLeader"/>.
    /// </summary>
    public RhinoLeader? ToRhinoType(CadLeader cadLeader)
    {
        var vertices = new List<RhinoPoint3d>();

        for (var i = 0; i < cadLeader.NumVertices; i++)
        {
            var vertex = this.ToRhinoType(cadLeader.VertexAt(i));
            vertices.Add(vertex);
        }

        if (vertices.Count < 2)
            return null;

        var normal = this.ToRhinoType(cadLeader.Normal);
        var plane = new RhinoPlane(vertices[0], normal);

        var dimension = RhinoLeader.Create(
            string.Empty,
            plane,
            RhinoDoc.ActiveDoc.DimStyles.Current,
            vertices.ToArray());

        return dimension;
    }

    /// <summary>
    /// Converts a <see cref="CadMLeader"/> to a <see cref="RhinoLeader"/>.
    /// </summary>
    public RhinoLeader? ToRhinoType(CadMLeader cadMLeader)
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
            var vertex = this.ToRhinoType(cadMLeader.GetVertex(lineIndex, i));
            vertices.Add(vertex);
        }

        if (vertices.Count < 2)
            return null;

        var normal = this.ToRhinoType(cadMLeader.Normal);
        var plane = new RhinoPlane(vertices[0], normal);

        var text = cadMLeader.ContentType == ContentType.MTextContent
            ? cadMLeader.MText?.Text ?? string.Empty
            : string.Empty;

        var dimension = RhinoLeader.Create(
            text,
            plane,
            RhinoDoc.ActiveDoc.DimStyles.Current,
            vertices.ToArray());

        return dimension;
    }

    /// <summary>
    /// Converts a <see cref="CadMLeader"/> used as a center mark to a <see cref="RhinoCentermark"/>.
    /// </summary>
    public RhinoCentermark? ToRhinoTypeCentermark(CadMLeader cadMLeader)
    {
        if (cadMLeader.ContentType != ContentType.BlockContent)
            return null;

        var position = this.ToRhinoType(cadMLeader.BlockPosition);
        var normal = this.ToRhinoType(cadMLeader.Normal);
        var plane = new RhinoPlane(position, normal);

        var centermark = RhinoCentermark.Create(
            RhinoDoc.ActiveDoc.DimStyles.Current,
            plane,
            position,
            1.0);

        return centermark;
    }
}
