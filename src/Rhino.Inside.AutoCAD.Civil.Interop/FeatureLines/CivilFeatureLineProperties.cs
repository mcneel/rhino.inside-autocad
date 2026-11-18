using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using CivilDocument = Autodesk.Civil.ApplicationServices.CivilDocument;

namespace Rhino.Inside.AutoCAD.Civil.Interop;

/// <summary>
/// Wraps properties extracted from a Civil 3D Feature Line.
/// </summary>
/// <remarks>
/// This is a simple data wrapper class that holds extracted feature line property information.
/// The data is captured at construction time from a <see cref="FeatureLine"/>.
/// </remarks>
public record CivilFeatureLineProperties : ICivilFeatureLineProperties
{
    private readonly ObjectId _featureLineId;

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public string Description { get; }

    /// <inheritdoc />
    public INamedId Style { get; }

    /// <inheritdoc />
    public INamedId Site { get; }

    /// <inheritdoc />
    public double Length2D { get; }

    /// <inheritdoc />
    public double Length3D { get; }

    /// <inheritdoc />
    public double MinElevation { get; }

    /// <inheritdoc />
    public double MaxElevation { get; }

    /// <inheritdoc />
    public double MinGrade { get; }

    /// <inheritdoc />
    public double MaxGrade { get; }

    /// <inheritdoc />
    public int PIPointCount { get; }

    /// <inheritdoc />
    public int ElevationPointCount { get; }

    /// <inheritdoc />
    public int CurveCount { get; }

    /// <inheritdoc />
    public IObjectId FeatureLineId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CivilFeatureLineProperties"/> class
    /// by extracting data from a given <see cref="FeatureLine"/>.
    /// </summary>
    /// <param name="featureLine">The Civil 3D feature line to extract properties from.</param>
    public CivilFeatureLineProperties(FeatureLine featureLine)
    {
        _featureLineId = featureLine.Id;

        this.Name = featureLine.Name ?? string.Empty;
        this.Description = featureLine.Description ?? string.Empty;
        this.Style = new NamedId(featureLine.StyleName, featureLine.StyleId);
        this.Site = GetSite(featureLine.SiteId);
        this.Length2D = featureLine.Length2D;
        this.Length3D = featureLine.Length3D;
        this.MinElevation = featureLine.MinElevation;
        this.MaxElevation = featureLine.MaxElevation;
        this.MinGrade = featureLine.MinGrade;
        this.MaxGrade = featureLine.MaxGrade;
        this.PIPointCount = featureLine.PIPointsCount;
        this.ElevationPointCount = featureLine.ElevationPointsCount;
        this.CurveCount = featureLine.CurvesCount;
        this.FeatureLineId = new AutocadObjectIdWrapper(featureLine.Id);
    }

    /// <summary>
    /// Resolves the owning site name. FeatureLine exposes only the site id, so the
    /// site is opened through the current top transaction when one is available.
    /// </summary>
    private static INamedId GetSite(ObjectId siteId)
    {
        if (siteId.IsNull || siteId.IsErased)
            return NamedId.Empty;

        try
        {
            var transaction = siteId.Database?.TransactionManager.TopTransaction;

            if (transaction?.GetObject(siteId, OpenMode.ForRead) is Site site)
                return new NamedId(site.Name, siteId);
        }
        catch
        {
            // Fall through to an unnamed reference
        }

        return new NamedId(string.Empty, siteId);
    }

    /// <inheritdoc />
    public ICivilFeatureLineProperties Update(IAutocadTransactionManager transactionManager,
        string newName, string newDescription, string newStyleName)
    {
        var featureLine = transactionManager.Unwrap().GetObject(_featureLineId, OpenMode.ForWrite) as FeatureLine;

        if (featureLine == null)
        {
            return this;
        }

        if (featureLine.Name != newName)
            featureLine.Name = newName;

        if (featureLine.Description != newDescription)
            featureLine.Description = newDescription;

        if (!string.IsNullOrWhiteSpace(newStyleName) && featureLine.StyleName != newStyleName)
        {
            var civilDocument = CivilDocument.GetCivilDocument(featureLine.Database);
            var styles = civilDocument.Styles.FeatureLineStyles;

            if (!styles.Contains(newStyleName))
                throw new ArgumentException($"Feature line style '{newStyleName}' does not exist in the drawing.");

            featureLine.StyleId = styles[newStyleName];
        }

        return new CivilFeatureLineProperties(featureLine);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Feature Line Properties: {this.Name} (Length: {this.Length3D:F2})";
    }
}
