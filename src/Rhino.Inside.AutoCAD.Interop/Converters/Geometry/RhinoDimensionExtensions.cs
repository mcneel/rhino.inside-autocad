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
using RhinoDimension = Rhino.Geometry.Dimension;
using RhinoLeader = Rhino.Geometry.Leader;
using RhinoLinearDimension = Rhino.Geometry.LinearDimension;
using RhinoOrdinateDimension = Rhino.Geometry.OrdinateDimension;
using RhinoRadialDimension = Rhino.Geometry.RadialDimension;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Provides extension methods for converting Rhino dimension types to AutoCAD dimension types.
/// </summary>
public static class RhinoDimensionExtensions
{
    /// <summary>
    /// Converts any Rhino <see cref="RhinoDimension"/> to the appropriate AutoCAD dimension type.
    /// </summary>
    /// <param name="rhinoDimension">The Rhino dimension to convert.</param>
    /// <returns>An AutoCAD dimension, or null if the dimension type is not supported.</returns>
    public static CadDimension? ToAutocadDimension(this RhinoDimension rhinoDimension)
    {
        return rhinoDimension switch
        {
            RhinoLinearDimension linear => linear.ToAutocadDimension(),
            RhinoAngularDimension angular => angular.ToAutocadLineAngularDimension2(),
            RhinoRadialDimension radial => radial.ToAutocadDimension(),
            RhinoOrdinateDimension ordinate => ordinate.ToAutocadOrdinateDimension(),
            _ => null
        };
    }

    /// <summary>
    /// Converts a <see cref="RhinoLinearDimension"/> to a <see cref="CadRotatedDimension"/> or <see cref="CadAlignedDimension"/>.
    /// </summary>
    /// <param name="rhinoDimension">The Rhino linear dimension to convert.</param>
    /// <returns>An AutoCAD dimension (rotated or aligned) with coordinates scaled to AutoCAD units.</returns>
    public static CadDimension ToAutocadDimension(this RhinoLinearDimension rhinoDimension)
    {
        var plane = rhinoDimension.Plane;

        var ext1_2d = rhinoDimension.ExtensionLine1End;
        var ext2_2d = rhinoDimension.ExtensionLine2End;
        var dimPt_2d = rhinoDimension.DimensionLinePoint;

        var ext1_3d = plane.PointAt(ext1_2d.X, ext1_2d.Y);
        var ext2_3d = plane.PointAt(ext2_2d.X, ext2_2d.Y);
        var dimPt_3d = plane.PointAt(dimPt_2d.X, dimPt_2d.Y);

        var xLine1Point = ext1_3d.ToAutocadPoint3d();
        var xLine2Point = ext2_3d.ToAutocadPoint3d();
        var dimLinePoint = dimPt_3d.ToAutocadPoint3d();

        var textHeight = rhinoDimension.DimensionStyle?.TextHeight ?? 2.5;
        var dimtxt = UnitConverter.ToAutoCadLength(textHeight * rhinoDimension.DimensionScale);

        if (rhinoDimension.Aligned)
        {
            var alignedDim = new CadAlignedDimension(
                xLine1Point,
                xLine2Point,
                dimLinePoint,
                string.Empty,
                ObjectId.Null);

            alignedDim.Normal = plane.ZAxis.ToAutocadVector3d();
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

            rotatedDim.Normal = plane.ZAxis.ToAutocadVector3d();
            rotatedDim.Dimtxt = dimtxt;

            return rotatedDim;
        }
    }

    /// <summary>
    /// Converts a <see cref="RhinoAngularDimension"/> to a <see cref="CadLineAngularDimension2"/>.
    /// </summary>
    /// <param name="rhinoDimension">The Rhino angular dimension to convert.</param>
    /// <returns>An AutoCAD line angular dimension with coordinates scaled to AutoCAD units.</returns>
    public static CadLineAngularDimension2 ToAutocadLineAngularDimension2(this RhinoAngularDimension rhinoDimension)
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

        var xLine1Start = center3d.ToAutocadPoint3d();
        var xLine1End = def1_3d.ToAutocadPoint3d();
        var xLine2Start = center3d.ToAutocadPoint3d();
        var xLine2End = def2_3d.ToAutocadPoint3d();
        var arcPoint = arc3d.ToAutocadPoint3d();

        var angularDim = new CadLineAngularDimension2(
            xLine1Start,
            xLine1End,
            xLine2Start,
            xLine2End,
            arcPoint,
            string.Empty,
            ObjectId.Null);

        angularDim.Normal = plane.ZAxis.ToAutocadVector3d();

        var textHeight = rhinoDimension.DimensionStyle?.TextHeight ?? 2.5;
        angularDim.Dimtxt = UnitConverter.ToAutoCadLength(textHeight * rhinoDimension.DimensionScale);

        return angularDim;
    }

    /// <summary>
    /// Converts a <see cref="RhinoRadialDimension"/> to a <see cref="CadRadialDimension"/> or <see cref="CadDiametricDimension"/>.
    /// </summary>
    /// <param name="rhinoDimension">The Rhino radial dimension to convert.</param>
    /// <returns>An AutoCAD dimension (radial or diametric) with coordinates scaled to AutoCAD units.</returns>
    public static CadDimension ToAutocadDimension(this RhinoRadialDimension rhinoDimension)
    {
        var plane = rhinoDimension.Plane;

        var centerPt = rhinoDimension.CenterPoint;
        var radiusPt = rhinoDimension.RadiusPoint;
        var dimPt = rhinoDimension.DimlinePoint;

        var center3d = plane.PointAt(centerPt.X, centerPt.Y);
        var radius3d = plane.PointAt(radiusPt.X, radiusPt.Y);
        var dim3d = plane.PointAt(dimPt.X, dimPt.Y);

        var center = center3d.ToAutocadPoint3d();
        var chordPoint = radius3d.ToAutocadPoint3d();

        var textHeight = rhinoDimension.DimensionStyle?.TextHeight ?? 2.5;
        var dimtxt = UnitConverter.ToAutoCadLength(textHeight * rhinoDimension.DimensionScale);

        if (rhinoDimension.IsDiameterDimension)
        {
            var farChordPoint = (center3d + (center3d - radius3d)).ToAutocadPoint3d();

            var diametricDim = new CadDiametricDimension(
                chordPoint,
                farChordPoint,
                0.0,
                string.Empty,
                ObjectId.Null);

            diametricDim.Normal = plane.ZAxis.ToAutocadVector3d();
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

            radialDim.Normal = plane.ZAxis.ToAutocadVector3d();
            radialDim.Dimtxt = dimtxt;

            return radialDim;
        }
    }

    /// <summary>
    /// Converts a <see cref="RhinoOrdinateDimension"/> to a <see cref="CadOrdinateDimension"/>.
    /// </summary>
    /// <param name="rhinoDimension">The Rhino ordinate dimension to convert.</param>
    /// <returns>An AutoCAD ordinate dimension with coordinates scaled to AutoCAD units.</returns>
    public static CadOrdinateDimension ToAutocadOrdinateDimension(this RhinoOrdinateDimension rhinoDimension)
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
            def3d.ToAutocadPoint3d(),
            leader3d.ToAutocadPoint3d(),
            string.Empty,
            ObjectId.Null);

        ordinateDim.Origin = basePt.ToAutocadPoint3d();
        ordinateDim.Normal = plane.ZAxis.ToAutocadVector3d();

        var textHeight = rhinoDimension.DimensionStyle?.TextHeight ?? 2.5;
        ordinateDim.Dimtxt = UnitConverter.ToAutoCadLength(textHeight * rhinoDimension.DimensionScale);

        return ordinateDim;
    }

    /// <summary>
    /// Converts a <see cref="RhinoCentermark"/> to a <see cref="CadMLeader"/>.
    /// </summary>
    /// <param name="rhinoCentermark">The Rhino centermark to convert.</param>
    /// <returns>An AutoCAD MLeader as a placeholder for center mark.</returns>
    /// <remarks>
    /// AutoCAD doesn't have a direct center mark dimension type; this creates an MLeader as a placeholder.
    /// </remarks>
    public static CadMLeader ToAutocadMLeader(this RhinoCentermark rhinoCentermark)
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
    /// <param name="rhinoLeader">The Rhino leader to convert.</param>
    /// <returns>An AutoCAD MLeader with coordinates scaled to AutoCAD units.</returns>
    public static CadMLeader ToAutocadMLeader(this RhinoLeader rhinoLeader)
    {
        var plane = rhinoLeader.Plane;
        var points2d = rhinoLeader.Points2D;

        var mleader = new CadMLeader();

        var leaderIndex = mleader.AddLeader();
        var lineIndex = mleader.AddLeaderLine(leaderIndex);

        foreach (var pt2d in points2d)
        {
            var pt3d = plane.PointAt(pt2d.X, pt2d.Y);
            var cadPt = pt3d.ToAutocadPoint3d();
            mleader.AddLastVertex(lineIndex, cadPt);
        }

        mleader.ContentType = ContentType.MTextContent;

        var textHeight = rhinoLeader.DimensionStyle?.TextHeight ?? 2.5;

        var mtext = new MText();
        mtext.Contents = rhinoLeader.PlainText ?? string.Empty;
        mtext.Location = plane.PointAt(points2d[points2d.Length - 1].X, points2d[points2d.Length - 1].Y).ToAutocadPoint3d();
        mtext.TextHeight = UnitConverter.ToAutoCadLength(textHeight * rhinoLeader.DimensionScale);
        mleader.MText = mtext;

        return mleader;
    }
}
