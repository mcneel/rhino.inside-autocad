namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a Register of <see cref="IAutocadLinetypeTableRecord"/>s from the
/// <see cref="IAutocadDocument"/>.
/// </summary>
public interface ILineTypeRegister : IRegister<IAutocadLinetypeTableRecord>
{
    /// <summary>
    /// Tries to add a new <see cref="IAutocadLinetypeTableRecord"/> to the register .
    /// </summary>
    bool TryAddLineType(string name, double patternLength, int numberOfDashes,
        bool scaleToFit, out IAutocadLinetypeTableRecord lineType);

    /// <summary>
    /// Returns the default continuous line type from AutoCAD.
    /// </summary>
    IAutocadLinetypeTableRecord GetDefault();
}