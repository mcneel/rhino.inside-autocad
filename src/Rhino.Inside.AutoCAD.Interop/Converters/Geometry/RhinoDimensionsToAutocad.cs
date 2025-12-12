using Autodesk.AutoCAD.DatabaseServices;
using CadAlignedDimension = Autodesk.AutoCAD.DatabaseServices.AlignedDimension;
using CadDiametricDimension = Autodesk.AutoCAD.DatabaseServices.DiametricDimension;
using CadDimension = Autodesk.AutoCAD.DatabaseServices.Dimension;
using CadLineAngularDimension2 = Autodesk.AutoCAD.DatabaseServices.LineAngularDimension2;
using CadMLeader = Autodesk.AutoCAD.DatabaseServices.MLeader;
using CadOrdinateDimension = Autodesk.AutoCAD.DatabaseServices.OrdinateDimension;
using CadRadialDimension = Autodesk.AutoCAD.DatabaseServices.RadialDimension;
using CadRotatedDimension = Autodesk.AutoCAD.DatabaseServices.RotatedDimension;
using RhinoAngularDimension = Rhino.Geometry.AngularDimension;
using RhinoCentermark = Rhino.Geometry.Centermark;
using RhinoLeader = Rhino.Geometry.Leader;
using RhinoLinearDimension = Rhino.Geometry.LinearDimension;
using RhinoOrdinateDimension = Rhino.Geometry.OrdinateDimension;
using RhinoRadialDimension = Rhino.Geometry.RadialDimension;
using RhinoDimension = Rhino.Geometry.Dimension;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Partial class containing dimension conversion methods from Rhino to AutoCAD.
/// </summary>
public partial class GeometryConverter
{
    /// <summary>
    /// Converts any Rhino <see cref="RhinoDimension"/> to the appropriate AutoCAD dimension type.
    /// </summary>
    public CadDimension? ToAutoCadType(RhinoDimension rhinoDimension)
    {
        return rhinoDimension switch
        {
            RhinoLinearDimension linear => ToAutoCadType(linear),
            RhinoAngularDimension angular => ToAutoCadType(angular),
            RhinoRadialDimension radial => ToAutoCadType(radial),
            RhinoOrdinateDimension ordinate => ToAutoCadType(ordinate),
            _ => null
        };
    }

    /// <summary>
    /// Converts a <see cref="RhinoLinearDimension"/> to a <see cref="CadRotatedDimension"/> or <see cref="CadAlignedDimension"/>.
    /// </summary>
    public CadDimension ToAutoCadType(RhinoLinearDimension rhinoDimension)
    {
        var plane = rhinoDimension.Plane;

        var ext1_2d = rhinoDimension.ExtensionLine1End;
        var ext2_2d = rhinoDimension.ExtensionLine2End;
        var dimPt_2d = rhinoDimension.DimensionLinePoint;

        var ext1_3d = plane.PointAt(ext1_2d.X, ext1_2d.Y);
        var ext2_3d = plane.PointAt(ext2_2d.X, ext2_2d.Y);
        var dimPt_3d = plane.PointAt(dimPt_2d.X, dimPt_2d.Y);

        var xLine1Point = this.ToAutoCadType(ext1_3d);
        var xLine2Point = this.ToAutoCadType(ext2_3d);
        var dimLinePoint = this.ToAutoCadType(dimPt_3d);

        var textHeight = rhinoDimension.DimensionStyle?.TextHeight ?? 2.5;
        var dimtxt = _unitSystemManager.ToAutoCadLength(textHeight * rhinoDimension.DimensionScale);

        if (rhinoDimension.Aligned)
        {
            var alignedDim = new CadAlignedDimension(
                xLine1Point,
                xLine2Point,
                dimLinePoint,
                string.Empty,
                ObjectId.Null);

            alignedDim.Normal = this.ToAutoCadType(plane.ZAxis);
            alignedDim.Dimtxt = dimtxt;

            return alignedDim;
        }
        else
        {
            var direction = ext2_3d - ext1_3d;
            var rotation = Math.Atan2(direction.Y, direction.X);

            var rotatedDim = new CadRotatedDimension(
                rotation,
                xLine1Point,
                xLine2Point,
                dimLinePoint,
                string.Empty,
                ObjectId.Null);

            rotatedDim.Normal = this.ToAutoCadType(plane.ZAxis);
            rotatedDim.Dimtxt = dimtxt;

            return rotatedDim;
        }
    }

    /// <summary>
    /// Converts a <see cref="RhinoAngularDimension"/> to a <see cref="CadLineAngularDimension2"/>.
    /// </summary>
    public CadLineAngularDimension2 ToAutoCadType(RhinoAngularDimension rhinoDimension)
    {
        var plane = rhinoDimension.Plane;

        var centerPt = rhinoDimension.CenterPoint;
        var defPt1 = rhinoDimension.DefPoint1;
        var defPt2 = rhinoDimension.DefPoint2;
        var dimArcPt = rhinoDimension.DimlinePoint;

        var center3d = plane.PointAt(centerPt.X, centerPt.Y);
        var def1_3d = plane.PointAt(defPt1.X, defPt1.Y);
        var def2_3d = plane.PointAt(defPt2.X, defPt2.Y);
        var arc3d = plane.PointAt(dimArcPt.X, dimArcPt.Y);

        var xLine1Start = this.ToAutoCadType(center3d);
        var xLine1End = this.ToAutoCadType(def1_3d);
        var xLine2Start = this.ToAutoCadType(center3d);
        var xLine2End = this.ToAutoCadType(def2_3d);
        var arcPoint = this.ToAutoCadType(arc3d);

        var angularDim = new CadLineAngularDimension2(
            xLine1Start,
            xLine1End,
            xLine2Start,
            xLine2End,
            arcPoint,
            string.Empty,
            ObjectId.Null);

        angularDim.Normal = this.ToAutoCadType(plane.ZAxis);

        var textHeight = rhinoDimension.DimensionStyle?.TextHeight ?? 2.5;
        angularDim.Dimtxt = _unitSystemManager.ToAutoCadLength(textHeight * rhinoDimension.DimensionScale);

        return angularDim;
    }

    /// <summary>
    /// Converts a <see cref="RhinoRadialDimension"/> to a <see cref="CadRadialDimension"/> or <see cref="CadDiametricDimension"/>.
    /// </summary>
    public CadDimension ToAutoCadType(RhinoRadialDimension rhinoDimension)
    {
        var plane = rhinoDimension.Plane;

        var centerPt = rhinoDimension.CenterPoint;
        var radiusPt = rhinoDimension.RadiusPoint;
        var dimPt = rhinoDimension.DimlinePoint;

        var center3d = plane.PointAt(centerPt.X, centerPt.Y);
        var radius3d = plane.PointAt(radiusPt.X, radiusPt.Y);
        var dim3d = plane.PointAt(dimPt.X, dimPt.Y);

        var center = this.ToAutoCadType(center3d);
        var chordPoint = this.ToAutoCadType(radius3d);

        var textHeight = rhinoDimension.DimensionStyle?.TextHeight ?? 2.5;
        var dimtxt = _unitSystemManager.ToAutoCadLength(textHeight * rhinoDimension.DimensionScale);

        if (rhinoDimension.IsDiameterDimension)
        {
            var farChordPoint = this.ToAutoCadType(
                center3d + (center3d - radius3d));

            var diametricDim = new CadDiametricDimension(
                chordPoint,
                farChordPoint,
                0.0,
                string.Empty,
                ObjectId.Null);

            diametricDim.Normal = this.ToAutoCadType(plane.ZAxis);
            diametricDim.Dimtxt = dimtxt;

            return diametricDim;
        }
        else
        {
            var radialDim = new CadRadialDimension(
                center,
                chordPoint,
                0.0,
                string.Empty,
                ObjectId.Null);

            radialDim.Normal = this.ToAutoCadType(plane.ZAxis);
            radialDim.Dimtxt = dimtxt;

            return radialDim;
        }
    }

    /// <summary>
    /// Converts a <see cref="RhinoOrdinateDimension"/> to a <see cref="CadOrdinateDimension"/>.
    /// </summary>
    public CadOrdinateDimension ToAutoCadType(RhinoOrdinateDimension rhinoDimension)
    {
        var plane = rhinoDimension.Plane;

        var basePt = plane.Origin;

        var defPt = rhinoDimension.DefPoint;

        var leaderPt = rhinoDimension.LeaderPoint;

        var def3d = plane.PointAt(defPt.X, defPt.Y);

        var leader3d = plane.PointAt(leaderPt.X, leaderPt.Y);

        var usingXAxis = rhinoDimension.Direction == RhinoOrdinateDimension.MeasuredDirection.Xaxis;

        var ordinateDim = new CadOrdinateDimension(
            usingXAxis,
            this.ToAutoCadType(def3d),
            this.ToAutoCadType(leader3d),
            string.Empty,
            ObjectId.Null);

        ordinateDim.Origin = this.ToAutoCadType(basePt);
        ordinateDim.Normal = this.ToAutoCadType(plane.ZAxis);

        var textHeight = rhinoDimension.DimensionStyle?.TextHeight ?? 2.5;
        ordinateDim.Dimtxt = _unitSystemManager.ToAutoCadLength(textHeight * rhinoDimension.DimensionScale);

        return ordinateDim;
    }

    /// <summary>
    /// Converts a <see cref="RhinoCentermark"/> to a <see cref="CadMLeader"/>.
    /// </summary>
    /// <remarks>
    /// AutoCAD doesn't have a direct center mark dimension type; this creates an MLeader as a placeholder.
    /// </remarks>
    public CadMLeader ToAutoCadType(RhinoCentermark rhinoCentermark)
    {
        var plane = rhinoCentermark.Plane;
        var center = plane.Origin;

        var mleader = new CadMLeader();
        mleader.ContentType = ContentType.NoneContent;

        return mleader;
    }

    /// <summary>
    /// Converts a <see cref="RhinoLeader"/> to a <see cref="CadMLeader"/>.
    /// </summary>
    public CadMLeader ToAutoCadType(RhinoLeader rhinoLeader)
    {
        var plane = rhinoLeader.Plane;
        var points2d = rhinoLeader.Points2D;

        var mleader = new CadMLeader();

        var leaderIndex = mleader.AddLeader();
        var lineIndex = mleader.AddLeaderLine(leaderIndex);

        foreach (var pt2d in points2d)
        {
            var pt3d = plane.PointAt(pt2d.X, pt2d.Y);
            var cadPt = this.ToAutoCadType(pt3d);
            mleader.AddLastVertex(lineIndex, cadPt);
        }

        mleader.ContentType = ContentType.MTextContent;

        var textHeight = rhinoLeader.DimensionStyle?.TextHeight ?? 2.5;

        var mtext = new MText();
        mtext.Contents = rhinoLeader.PlainText ?? string.Empty;
        mtext.Location = this.ToAutoCadType(plane.PointAt(points2d[points2d.Length - 1].X, points2d[points2d.Length - 1].Y));
        mtext.TextHeight = _unitSystemManager.ToAutoCadLength(textHeight * rhinoLeader.DimensionScale);
        mleader.MText = mtext;

        return mleader;
    }
}
