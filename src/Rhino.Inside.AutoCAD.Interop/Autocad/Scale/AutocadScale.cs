using Autodesk.AutoCAD.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IAutocadScale"/>
public class AutocadScale : IAutocadScale
{

    /// <inheritdoc />
    public double X { get; }

    /// <inheritdoc />
    public double Y { get; }

    /// <inheritdoc />
    public double Z { get; }

    /// <summary>
    /// Constructs a new <see cref="IAutocadScale"/>.
    /// </summary>
    public AutocadScale(Scale3d scale3d)
    {
        this.X = scale3d.X;
        this.Y = scale3d.Y;
        this.Z = scale3d.Z;
    }

    /// <summary>
    /// Constructs a new <see cref="IAutocadScale"/>.
    /// </summary>
    public AutocadScale(double uniform)
    {
        this.X = uniform;
        this.Y = uniform;
        this.Z = uniform;
    }

    /// <summary>
    /// Constructs a new <see cref="IAutocadScale"/>.
    /// </summary>
    public AutocadScale(double x, double y, double z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Scale(X: {this.X}, Y: {this.Y}, Z: {this.Z})";
    }
}