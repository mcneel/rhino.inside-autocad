using Autodesk.AutoCAD.DatabaseServices;
using DimensionStyle = Rhino.DocObjects.DimensionStyle;
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
    /// The text height used when a Rhino annotation has no dimension style to read one from.
    /// </summary>
    private const double DefaultTextHeight = 2.5;

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
    /// <returns>
    /// An AutoCAD MLeader with coordinates scaled to AutoCAD units, or <see
    /// langword="null"/> when the leader has no points to build a leader line from.
    /// </returns>
    public static CadMLeader? ToAutocadMLeader(this RhinoLeader rhinoLeader)
    {
        var plane = rhinoLeader.Plane;
        var points2d = rhinoLeader.Points2D;

        if (points2d == null || points2d.Length == 0) return null;

        var mleader = new CadMLeader();

        // Database defaults populate the MLeader style, text style and linetype, without
        // which the MLeader is not valid to bake or to preview.
        mleader.SetDatabaseDefaults();

        // The content type is set before the leader lines are added: changing it afterwards
        // rebuilds the MLeader's content and discards them. Database defaults make the
        // MLeader adopt CMLEADERSTYLE, whose content type is not necessarily MText, so this
        // assignment is a real transition rather than the no-op it was on a bare MLeader.
        mleader.ContentType = ContentType.MTextContent;

        // The overall scale is set first. AutoCAD recomputes its scale dependent properties
        // when the scale changes, so any size written beforehand is rescaled out from under
        // us - which is why a leader at scale 1 round tripped correctly and one at any other
        // scale did not.
        mleader.Scale = rhinoLeader.DimensionScale;

        var dimensionStyle = rhinoLeader.DimensionStyle;

        if (dimensionStyle != null)
            ApplyLeaderStyle(mleader, dimensionStyle);

        mleader.EnableFrameText = rhinoLeader.MaskFrame != DimensionStyle.MaskFrame.NoFrame;

        var leaderIndex = mleader.AddLeader();
        var lineIndex = mleader.AddLeaderLine(leaderIndex);

        foreach (var pt2d in points2d)
        {
            var pt3d = plane.PointAt(pt2d.X, pt2d.Y);
            var cadPt = pt3d.ToAutocadPoint3d();
            mleader.AddLastVertex(lineIndex, cadPt);
        }

        var textHeight = rhinoLeader.TextHeight > 0.0
            ? rhinoLeader.TextHeight
            : dimensionStyle?.TextHeight ?? DefaultTextHeight;

        var lastPoint2d = points2d[points2d.Length - 1];

        var mtext = new MText();
        mtext.SetDatabaseDefaults();

        mtext.Contents = rhinoLeader.PlainText ?? string.Empty;

        mtext.Location = plane.PointAt(lastPoint2d.X, lastPoint2d.Y).ToAutocadPoint3d();
        mtext.Rotation = rhinoLeader.TextRotationRadians;
        mtext.TextHeight = UnitConverter.ToAutoCadLength(textHeight);
        mleader.MText = mtext;

        mleader.TextHeight = UnitConverter.ToAutoCadLength(textHeight);

        return mleader;
    }

    /// <summary>
    /// Writes the leader properties Rhino keeps on a dimension style onto an AutoCAD
    /// multileader.
    /// </summary>
    /// <param name="mleader">The multileader to write the properties onto.</param>
    /// <param name="dimensionStyle">The Rhino dimension style to read them from.</param>
    private static void ApplyLeaderStyle(CadMLeader mleader, DimensionStyle dimensionStyle)
    {
        mleader.LeaderLineType =
            AnnotationStyleMapping.ToAutocadLeaderType(dimensionStyle.LeaderCurveType);
        mleader.TextAngleType =
            AnnotationStyleMapping.ToAutocadTextAngleType(dimensionStyle.LeaderContentAngleType);
        // Only the left attachment is written; the right one has no Rhino counterpart and is
        // left to the multileader style rather than being forced to match the left.
        mleader.SetTextAttachmentType(
            AnnotationStyleMapping.ToAutocadTextAttachmentType(dimensionStyle.LeaderTextVerticalAlignment),
            LeaderDirectionType.LeftLeader);
        mleader.TextAlignmentType =
            AnnotationStyleMapping.ToAutocadTextAlignmentType(dimensionStyle.LeaderTextHorizontalAlignment);

        mleader.ArrowSize = UnitConverter.ToAutoCadLength(dimensionStyle.LeaderArrowLength);
        mleader.EnableLanding = dimensionStyle.LeaderHasLanding;

        // "Landing distance" is the dogleg; "Landing gap" is the gap to the text.
        mleader.DoglegLength = UnitConverter.ToAutoCadLength(dimensionStyle.LeaderLandingLength);
        mleader.LandingGap = UnitConverter.ToAutoCadLength(dimensionStyle.TextGap);

        var arrowBlockName = AnnotationStyleMapping.ToAutocadArrowBlockName(dimensionStyle.LeaderArrowType);

        var arrowBlockId = ResolveArrowBlockId(arrowBlockName);

        // Left unset when the drawing has no such arrowhead block, so the multileader style
        // supplies its own rather than the leader ending up with no arrowhead at all.
        if (arrowBlockId.IsNull == false)
            mleader.ArrowSymbolId = arrowBlockId;
    }

    /// <summary>
    /// Finds an arrowhead block by name in the working database.
    /// </summary>
    /// <param name="blockName">The arrowhead block's name.</param>
    /// <returns>
    /// The block's id, or <see cref="ObjectId.Null"/> when the name is empty or the drawing
    /// does not contain it.
    /// </returns>
    /// <remarks>
    /// The multileader being built is not database resident, so there is no database to read
    /// through it. The working database is the one <c>SetDatabaseDefaults</c> has already
    /// taken this multileader's defaults from.
    /// </remarks>
    private static ObjectId ResolveArrowBlockId(string? blockName)
    {
        if (string.IsNullOrEmpty(blockName)) return ObjectId.Null;

        var database = HostApplicationServices.WorkingDatabase;

        if (database == null) return ObjectId.Null;

        using var transaction = database.TransactionManager.StartTransaction();

        var blockTable = transaction.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable;

        var arrowBlockId = blockTable != null && blockTable.Has(blockName)
            ? blockTable[blockName]
            : ObjectId.Null;

        transaction.Commit();

        return arrowBlockId;
    }
}
