namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a Repository of <see cref="IAutocadLinetypeTableRecord"/>s from the
/// <see cref="IAutocadDocument"/>.
/// </summary>
public interface ILineTypeRepository : IRepository<IAutocadLinetypeTableRecord>, IEnumerable<IAutocadLinetypeTableRecord>
{
    /// <summary>
    /// Returns the <see cref="IAutocadLinetypeTableRecord"/> that matches the
    /// <paramref name="linePatternId"/> otherwise returns the default solid
    /// (CONTINUOUS) pattern.
    /// </summary>
    IAutocadLinetypeTableRecord GetById(IObjectId linePatternId);

    /// <summary>
    /// Tries to add a new <see cref="IAutocadLinetypeTableRecord"/> to the repository.
    /// </summary>
    bool TryAddLineType(string name, double patternLength, int numberOfDashes,
        bool scaleToFit, out IAutocadLinetypeTableRecord lineType);
}