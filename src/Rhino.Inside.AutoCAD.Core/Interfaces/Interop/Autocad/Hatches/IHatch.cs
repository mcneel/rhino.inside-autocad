namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface which wraps a hatch entity from an AutoCAD
/// <see cref="IAutocadDocument"/>.
/// </summary>
public interface IHatch : IEntity
{
    /// <summary>
    /// The origin of this <see cref="IHatch"/>.
    /// </summary>
   // IPoint2d Origin { get; }

    /// <summary>
    /// The list of <see cref="IHatchLoop"/>s that make up the bounds of
    /// this <see cref="IHatch"/>.
    /// </summary>
    /// <remarks>
    /// The outermost loop is the first item in the list. Inner loops are
    /// inside the outermost loop, unless the loops intersect, in which
    /// case the loop is invalid.
    /// </remarks>
    IHatchLoopSet Loops { get; }

    /// <summary>
    /// The <see cref="IHatchValidator"/> for this <see cref="IHatch"/>.
    /// </summary>
    IHatchValidator Validator { get; }

    /// <summary>
    /// The <see cref="IPlane"/> of this <see cref="IHatch"/>.
    /// </summary>
    //IPlane Plane { get; }
}