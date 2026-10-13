using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.Civil.Interop;

/// <summary>
/// Wraps properties extracted from a Civil 3D Alignment.
/// </summary>
/// <remarks>
/// This is a simple data wrapper class that holds extracted alignment property information.
/// The data is captured at construction time from an <see cref="Alignment"/>.
/// </remarks>
public record CivilAlignmentProperties : ICivilAlignmentProperties
{
    private readonly Alignment _alignment;

    /// <inheritdoc />
    public string Name { get; } = string.Empty;

    /// <inheritdoc />
    public string Description { get; } = string.Empty;

    /// <inheritdoc />
    public double StartStation { get; }

    /// <inheritdoc />
    public double EndStation { get; }

    /// <inheritdoc />
    public double Length { get; }

    /// <inheritdoc />
    public Core.CivilAlignmentType CivilAlignmentType { get; }

    /// <inheritdoc />
    public INamedId Site { get; }

    /// <inheritdoc />
    public INamedId Style { get; }

    /// <inheritdoc />
    public INamedId DesignCheckSet { get; }

    /// <inheritdoc />
    public ICivilReferenceStation ReferenceStation { get; }

    /// <inheritdoc />
    public ICivilDesignSpeeds DesignSpeeds { get; }

    /// <inheritdoc />
    public ICivilConnectedAlignmentInfo ConnectedAlignmentInfo { get; }

    /// <inheritdoc />
    public ICivilOffsetAlignmentInfo OffsetAlignmentInfo { get; }

    /// <inheritdoc />
    public ICivilRailAlignmentInfo RailAlignmentInfo { get; }

    /// <inheritdoc />
    public IObjectId AlignmentId { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="CivilAlignmentProperties"/> with a transaction.
    /// </summary>
    /// <param name="alignment">The Civil 3D alignment to extract properties from.</param>
    public CivilAlignmentProperties(Alignment alignment)
    {
        _alignment = alignment;

        this.Name = alignment.Name;
        this.Description = alignment.Description ?? string.Empty;
        this.StartStation = alignment.StartingStation;
        this.EndStation = alignment.EndingStation;
        this.Length = alignment.Length;
        this.CivilAlignmentType = alignment.AlignmentType.ToRhinoInsideAlignmentType();

        this.Site = new NamedId(alignment.SiteName, alignment.SiteId);

        this.Style = new NamedId(alignment.StyleName, alignment.StyleId);

        this.AlignmentId = new AutocadObjectIdWrapper(alignment.Id);

        this.DesignCheckSet = new NamedId(alignment.DesignCheckSetName, alignment.DesignCheckSetId);

        this.ReferenceStation = new CivilReferenceStation(alignment);
        this.DesignSpeeds = new CivilDesignSpeeds(alignment);

        this.ConnectedAlignmentInfo = alignment.IsConnectedAlignment
        ? new CivilConnectedAlignmentInfo(alignment.ConnectedAlignmentInfo)
            : CivilConnectedAlignmentInfo.Empty;

        this.OffsetAlignmentInfo = alignment.IsOffsetAlignment
            ? new CivilOffsetAlignmentInfo(alignment.OffsetAlignmentInfo)
            : CivilOffsetAlignmentInfo.Empty;

        this.RailAlignmentInfo = alignment.SuperelevationType == SuperelevationType.Cant
            ? new CivilRailAlignmentInfo(alignment.RailAlignmentInfo)
            : CivilRailAlignmentInfo.Empty;
    }

    /// <inheritdoc />
    public ICivilCantInfo GetCantInfo(IAutocadTransactionManager transactionManager)
    {
        var alignment = transactionManager.Unwrap().GetObject(_alignment.Id, OpenMode.ForRead)
            as Alignment;

        return new CivilCantInfo(alignment);
    }

    /// <inheritdoc />
    public ICivilAlignmentProperties Update(IAutocadTransactionManager transactionManager,
        string newName, string newDescription)
    {
        var alignment = transactionManager.Unwrap().GetObject(_alignment.Id, OpenMode.ForWrite) as Alignment;

        if (alignment == null)
        {
            return this;
        }

        alignment.Name = newName;
        alignment.Description = newDescription;

        return new CivilAlignmentProperties(alignment);
    }

    /// <summary>
    /// Creates a duplicate of this alignment properties wrapper.
    /// </summary>
    /// <returns>A new instance with copied data.</returns>
    public CivilAlignmentProperties ShallowClone()
    {
        return new CivilAlignmentProperties(_alignment);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Alignment Properties: {this.Name} (Sta: {this.StartStation:F2} - {this.EndStation:F2}, Length: {this.Length:F2})";
    }
}
