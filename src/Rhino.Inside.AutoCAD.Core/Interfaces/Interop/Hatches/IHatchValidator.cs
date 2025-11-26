namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface which represents a validator for <see cref="IHatch"/> objects.
/// Valid <see cref="IHatch"/> entities can be used in the various AWI solvers.
/// Returns any warning messages if the <see cref="IHatch"/> is invalid.
/// </summary>
public interface IHatchValidator
{
    /// <summary>
    /// If the <see cref="IHatch"/> is invalid due to any of its <see cref="IHatchLoop"/>s
    /// being invalid, this property stores the first invalid
    /// <see cref="IHatchLoop.OutlineType"/> for quick access. Returns
    /// <see cref="HatchOutlineType.Closed"/> if all the <see cref="IHatchLoop"/>s
    /// are closed.
    /// </summary>
 //   HatchOutlineType OutlineType { get; }

    /// <summary>
    /// Returns true if the <see cref="IHatch"/> has no validation errors, indicating
    /// all the <see cref="IHatch.Loops"/> are <see cref="HatchOutlineType.Closed"/>.
    /// </summary>
    bool IsValid { get; }

    /// <summary>
    /// Returns the error message if the <see cref="IHatch"/> is invalid.
    /// </summary>
    string GetErrorMessage();
}