using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.Civil.Interop;

/// <summary>
/// Wraps properties extracted from a Civil 3D Grading.
/// </summary>
/// <remarks>
/// The data is captured at construction time from a <see cref="Grading"/>. Only the
/// members inherited from the Civil 3D entity base are available through the .NET API.
/// </remarks>
public record CivilGradingProperties : ICivilGradingProperties
{
    private readonly ObjectId _gradingId;

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public string Description { get; }

    /// <inheritdoc />
    public INamedId Style { get; }

    /// <inheritdoc />
    public IObjectId GradingId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CivilGradingProperties"/> class
    /// by extracting data from a given <see cref="Grading"/>.
    /// </summary>
    /// <param name="grading">The Civil 3D grading to extract properties from.</param>
    public CivilGradingProperties(Grading grading)
    {
        _gradingId = grading.Id;

        this.Name = grading.Name ?? string.Empty;
        this.Description = grading.Description ?? string.Empty;
        this.Style = new NamedId(grading.StyleName, grading.StyleId);
        this.GradingId = new AutocadObjectIdWrapper(grading.Id);
    }

    /// <inheritdoc />
    public ICivilGradingProperties Update(IAutocadTransactionManager transactionManager,
        string newName, string newDescription)
    {
        var grading = transactionManager.Unwrap().GetObject(_gradingId, OpenMode.ForWrite) as Grading;

        if (grading == null)
        {
            return this;
        }

        if (grading.Name != newName)
            grading.Name = newName;

        if (grading.Description != newDescription)
            grading.Description = newDescription;

        return new CivilGradingProperties(grading);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Grading Properties: {this.Name}";
    }
}
