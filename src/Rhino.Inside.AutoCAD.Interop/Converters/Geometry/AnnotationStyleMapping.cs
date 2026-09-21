using Autodesk.AutoCAD.DatabaseServices;
using Rhino.DocObjects;
using CadArrowType = Rhino.DocObjects.DimensionStyle.ArrowType;
using CadLeaderType = Autodesk.AutoCAD.DatabaseServices.LeaderType;
using RhinoLeaderAngleStyle = Rhino.DocObjects.DimensionStyle.LeaderContentAngleStyle;
using RhinoLeaderCurveStyle = Rhino.DocObjects.DimensionStyle.LeaderCurveStyle;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Translates the annotation appearance enumerations and arrowhead names that AutoCAD and
/// Rhino each use, so the leader and text converters share one copy of every table.
/// </summary>
/// <remarks>
/// The two products model annotation appearance differently: AutoCAD keeps it on the entity
/// backed by a named style, whereas Rhino keeps most of it on a
/// <see cref="DimensionStyle"/>. Nothing here reads or writes a document, so these are pure
/// value translations; resolving an arrowhead block to an
/// <see cref="Autodesk.AutoCAD.DatabaseServices.ObjectId"/> needs a database and is done by
/// the callers.
/// </remarks>
public static class AnnotationStyleMapping
{
    /// <summary>
    /// Creates a dimension style suitable for attaching to a single annotation through
    /// <see cref="Rhino.Geometry.AnnotationBase.SetOverrideDimStyle"/>.
    /// </summary>
    /// <returns>
    /// A copy of the document's current style, so that properties the caller does not set
    /// keep sensible values rather than the bare defaults of a new style.
    /// </returns>
    /// <remarks>
    /// The override must point at the style it overrides, and the annotation it is attached
    /// to must already be parented to a style that lives in the document. Attaching an
    /// override to an annotation built against a free standing style fails silently -
    /// <c>SetOverrideDimStyle</c> returns false and every property written onto the style is
    /// discarded, the text height and dimension scale included, because Rhino stores those
    /// as overrides too. Annotations must therefore be created with
    /// <see cref="GetParentStyle"/> and given the override afterwards.
    /// </remarks>
    /// <seealso cref="GetParentStyle"/>
    public static DimensionStyle CreateOverrideStyle()
    {
        var overrideStyle = new DimensionStyle();

        var parentStyle = GetParentStyle();

        if (parentStyle != null)
        {
            overrideStyle.CopyFrom(parentStyle);
            overrideStyle.ParentId = parentStyle.Id;
        }

        return overrideStyle;
    }

    /// <summary>
    /// Returns the document style that an annotation carrying an override must be parented to.
    /// </summary>
    /// <returns>
    /// The Rhino document's current dimension style, or <see langword="null"/> when there is
    /// no active document.
    /// </returns>
    /// <seealso cref="CreateOverrideStyle"/>
    public static DimensionStyle? GetParentStyle()
    {
        return Rhino.RhinoDoc.ActiveDoc?.DimStyles?.Current;
    }

    /// <summary>
    /// The AutoCAD block names of the standard arrowheads, paired with the Rhino arrow type
    /// that matches each one most closely.
    /// </summary>
    /// <remarks>
    /// Compared without case and without a leading underscore, since drawings are
    /// inconsistent about both. An arrowhead outside this table has no Rhino equivalent and
    /// falls back to the style's own arrowhead rather than being approximated.
    /// </remarks>
    private static readonly IReadOnlyDictionary<string, CadArrowType> _arrowTypesByBlockName =
        new Dictionary<string, CadArrowType>(StringComparer.OrdinalIgnoreCase)
        {
            ["None"] = CadArrowType.None,
            ["Dot"] = CadArrowType.Dot,
            ["DotSmall"] = CadArrowType.Dot,
            ["DotBlank"] = CadArrowType.Dot,
            ["Origin"] = CadArrowType.Dot,
            ["ArchTick"] = CadArrowType.Tick,
            ["Oblique"] = CadArrowType.Tick,
            ["Open"] = CadArrowType.OpenArrow,
            ["Open30"] = CadArrowType.OpenArrow,
            ["Open90"] = CadArrowType.OpenArrow,
            ["BoxFilled"] = CadArrowType.Rectangle,
            ["BoxBlank"] = CadArrowType.Rectangle,
            ["Closed"] = CadArrowType.LongTriangle,
            ["ClosedBlank"] = CadArrowType.LongTriangle,
            ["ClosedFilled"] = CadArrowType.SolidTriangle,
        };

    /// <summary>
    /// Returns the Rhino arrow type matching an AutoCAD arrowhead block name.
    /// </summary>
    /// <param name="blockName">
    /// The arrowhead block's name. An empty name means the drawing's default arrowhead,
    /// which is the filled closed arrow.
    /// </param>
    /// <returns>
    /// The matching arrow type, or <see langword="null"/> when the arrowhead is a block with
    /// no Rhino equivalent, in which case the caller should leave the style's own arrowhead
    /// alone.
    /// </returns>
    public static CadArrowType? ToRhinoArrowType(string? blockName)
    {
        if (string.IsNullOrWhiteSpace(blockName)) return CadArrowType.SolidTriangle;

        var name = blockName!.TrimStart('_');

        if (_arrowTypesByBlockName.TryGetValue(name, out var arrowType)) return arrowType;

        return null;
    }

    /// <summary>
    /// Returns the AutoCAD arrowhead block name matching a Rhino arrow type.
    /// </summary>
    /// <param name="arrowType">The Rhino arrow type.</param>
    /// <returns>
    /// The block name to look up in the target drawing, or <see langword="null"/> when the
    /// arrowhead should be left unset so the multileader style supplies it.
    /// </returns>
    /// <remarks>
    /// <see cref="CadArrowType.SolidTriangle"/> deliberately returns <see langword="null"/>.
    /// It is what an inherited arrowhead is read back as, and AutoCAD expresses "inherited"
    /// as no arrowhead block at all. Naming the block instead would turn an inherited
    /// arrowhead into an explicit one, which the properties palette reports as a difference
    /// between two otherwise identical leaders.
    /// </remarks>
    public static string? ToAutocadArrowBlockName(CadArrowType arrowType)
    {
        return arrowType switch
        {
            CadArrowType.None => "_None",
            CadArrowType.Dot => "_Dot",
            CadArrowType.Tick => "_ArchTick",
            CadArrowType.OpenArrow => "_Open",
            CadArrowType.Rectangle => "_BoxFilled",
            CadArrowType.LongTriangle => "_Closed",
            CadArrowType.LongerTriangle => "_Closed",
            CadArrowType.ShortTriangle => "_ClosedFilled",
            _ => null,
        };
    }

    /// <summary>
    /// Converts an AutoCAD leader line type to the Rhino leader curve style.
    /// </summary>
    /// <param name="leaderType">The AutoCAD leader line type.</param>
    /// <returns>The matching Rhino leader curve style.</returns>
    public static RhinoLeaderCurveStyle ToRhinoLeaderCurveStyle(CadLeaderType leaderType)
    {
        return leaderType switch
        {
            CadLeaderType.SplineLeader => RhinoLeaderCurveStyle.Spline,
            CadLeaderType.InVisibleLeader => RhinoLeaderCurveStyle.None,
            _ => RhinoLeaderCurveStyle.Polyline,
        };
    }

    /// <summary>
    /// Converts a Rhino leader curve style to the AutoCAD leader line type.
    /// </summary>
    /// <param name="curveStyle">The Rhino leader curve style.</param>
    /// <returns>The matching AutoCAD leader line type.</returns>
    public static CadLeaderType ToAutocadLeaderType(RhinoLeaderCurveStyle curveStyle)
    {
        return curveStyle switch
        {
            RhinoLeaderCurveStyle.Spline => CadLeaderType.SplineLeader,
            RhinoLeaderCurveStyle.None => CadLeaderType.InVisibleLeader,
            _ => CadLeaderType.StraightLeader,
        };
    }

    /// <summary>
    /// Converts an AutoCAD multileader text angle type to the Rhino leader content angle style.
    /// </summary>
    /// <param name="angleType">The AutoCAD text angle type.</param>
    /// <returns>The matching Rhino leader content angle style.</returns>
    public static RhinoLeaderAngleStyle ToRhinoLeaderAngleStyle(TextAngleType angleType)
    {
        return angleType switch
        {
            TextAngleType.InsertAngle => RhinoLeaderAngleStyle.Rotated,
            TextAngleType.AlwaysRightReadingAngle => RhinoLeaderAngleStyle.Aligned,
            _ => RhinoLeaderAngleStyle.Horizontal,
        };
    }

    /// <summary>
    /// Converts a Rhino leader content angle style to the AutoCAD multileader text angle type.
    /// </summary>
    /// <param name="angleStyle">The Rhino leader content angle style.</param>
    /// <returns>The matching AutoCAD text angle type.</returns>
    public static TextAngleType ToAutocadTextAngleType(RhinoLeaderAngleStyle angleStyle)
    {
        return angleStyle switch
        {
            RhinoLeaderAngleStyle.Rotated => TextAngleType.InsertAngle,
            RhinoLeaderAngleStyle.Aligned => TextAngleType.AlwaysRightReadingAngle,
            _ => TextAngleType.HorizontalAngle,
        };
    }

    /// <summary>
    /// Converts an AutoCAD multileader text attachment to the Rhino leader text vertical
    /// alignment.
    /// </summary>
    /// <param name="attachmentType">The AutoCAD text attachment type.</param>
    /// <returns>The matching Rhino vertical alignment.</returns>
    public static TextVerticalAlignment ToRhinoTextVerticalAlignment(TextAttachmentType attachmentType)
    {
        return attachmentType switch
        {
            TextAttachmentType.AttachmentTopOfTop => TextVerticalAlignment.Top,
            TextAttachmentType.AttachmentMiddleOfTop => TextVerticalAlignment.MiddleOfTop,
            TextAttachmentType.AttachmentBottomOfTop => TextVerticalAlignment.BottomOfTop,
            TextAttachmentType.AttachmentBottomOfTopLine => TextVerticalAlignment.BottomOfTop,
            TextAttachmentType.AttachmentMiddleOfBottom => TextVerticalAlignment.MiddleOfBottom,
            TextAttachmentType.AttachmentBottomOfBottom => TextVerticalAlignment.Bottom,
            TextAttachmentType.AttachmentBottomLine => TextVerticalAlignment.Bottom,
            _ => TextVerticalAlignment.Middle,
        };
    }

    /// <summary>
    /// Converts a Rhino leader text vertical alignment to the AutoCAD multileader text
    /// attachment.
    /// </summary>
    /// <param name="alignment">The Rhino vertical alignment.</param>
    /// <returns>The matching AutoCAD text attachment type.</returns>
    public static TextAttachmentType ToAutocadTextAttachmentType(TextVerticalAlignment alignment)
    {
        return alignment switch
        {
            TextVerticalAlignment.Top => TextAttachmentType.AttachmentTopOfTop,
            TextVerticalAlignment.MiddleOfTop => TextAttachmentType.AttachmentMiddleOfTop,
            TextVerticalAlignment.BottomOfTop => TextAttachmentType.AttachmentBottomOfTop,
            TextVerticalAlignment.MiddleOfBottom => TextAttachmentType.AttachmentMiddleOfBottom,
            TextVerticalAlignment.Bottom => TextAttachmentType.AttachmentBottomOfBottom,
            TextVerticalAlignment.BottomOfBoundingBox => TextAttachmentType.AttachmentBottomOfBottom,
            _ => TextAttachmentType.AttachmentMiddle,
        };
    }

    /// <summary>
    /// Converts an AutoCAD multileader text alignment to the Rhino leader text horizontal
    /// alignment.
    /// </summary>
    /// <param name="alignmentType">The AutoCAD text alignment type.</param>
    /// <returns>The matching Rhino horizontal alignment.</returns>
    public static TextHorizontalAlignment ToRhinoTextHorizontalAlignment(TextAlignmentType alignmentType)
    {
        return alignmentType switch
        {
            TextAlignmentType.CenterAlignment => TextHorizontalAlignment.Center,
            TextAlignmentType.RightAlignment => TextHorizontalAlignment.Right,
            _ => TextHorizontalAlignment.Left,
        };
    }

    /// <summary>
    /// Converts a Rhino leader text horizontal alignment to the AutoCAD multileader text
    /// alignment.
    /// </summary>
    /// <param name="alignment">The Rhino horizontal alignment.</param>
    /// <returns>The matching AutoCAD text alignment type.</returns>
    public static TextAlignmentType ToAutocadTextAlignmentType(TextHorizontalAlignment alignment)
    {
        return alignment switch
        {
            TextHorizontalAlignment.Center => TextAlignmentType.CenterAlignment,
            TextHorizontalAlignment.Right => TextAlignmentType.RightAlignment,
            _ => TextAlignmentType.LeftAlignment,
        };
    }
}
