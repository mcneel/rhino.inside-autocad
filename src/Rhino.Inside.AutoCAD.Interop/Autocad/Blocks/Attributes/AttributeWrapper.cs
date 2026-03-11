using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IAttributeWrapper"/>
public class AttributeWrapper : AutocadEntityWrapper, IAttributeWrapper
{
    private readonly AttributeReference _attribute;

    /// <inheritdoc />
    public string Tag { get; }

    /// <inheritdoc />
    public string Text { get; }

    /// <inheritdoc />
    public bool IsMultiline { get; }

    /// <inheritdoc />
    public Point3d AlignmentPoint { get; }

    /// <summary>
    /// Constructs a new <see cref="DynamicBlockReferencePropertyWrapper"/>.
    /// </summary>
    public AttributeWrapper(AttributeReference attributeReference)
        : base(attributeReference)
    {
        _attribute = attributeReference;

        this.Tag = attributeReference.Tag;

        this.AlignmentPoint = attributeReference.AlignmentPoint.ToRhinoPoint3d();

        this.Text = attributeReference.TextString;

        this.IsMultiline = attributeReference.IsMTextAttribute;

        if (this.IsMultiline)
        {
            var mText = attributeReference.MTextAttribute;

            if (mText is not null)
                this.Text = mText.Contents;
        }
    }
}