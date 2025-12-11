using Rhino.Inside.AutoCAD.Core.Interfaces;
using CadColor = Autodesk.AutoCAD.Colors.Color;
using SystemColor = System.Drawing.Color;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A converter class for converting between <see cref="IColor"/> and
/// <see cref="Autodesk.AutoCAD.Colors.Color"/> or <see cref="System.Drawing.Color"/>.
/// </summary>
public class AutocadColorConverter
{
    /// <summary>
    /// Returns the <see cref="AutocadColorConverter"/> singleton.
    /// </summary>
    public static AutocadColorConverter Instance { get; }

    /// <summary>
    /// Static constructor that initializes the <see cref="AutocadColorConverter"/> singleton.
    /// </summary>
    static AutocadColorConverter()
    {
        AutocadColorConverter.Instance = new AutocadColorConverter();
    }

    /// <summary>
    /// Constructs a singleton instance of the <see cref="AutocadColorConverter"/>.
    /// </summary>
    private AutocadColorConverter() { }

    /// <summary>
    /// Converts a <see cref="IColor"/> into a <see cref="Autodesk.AutoCAD.Colors.Color"/>.
    /// </summary>
    public CadColor ToCadColor(IColor color)
    {
        return CadColor.FromRgb(color.Red, color.Green, color.Blue);
    }

    /// <summary>
    /// Converts a <see cref="IColor"/> into a <see cref="System.Drawing.Color"/>.
    /// </summary>
    public SystemColor ToSystemColor(IColor color)
    {
        return SystemColor.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);
    }

    /// <summary>
    /// Converts a <see cref="Autodesk.AutoCAD.Colors.Color"/> into a <see cref="IColor"/>.
    /// </summary>
    public IColor Convert(CadColor color)
    {
        return new InternalColor(color);
    }

    /// <summary>
    /// Converts a <see cref="System.Drawing.Color"/> into a <see cref="IColor"/>.
    /// </summary>
    public IColor Convert(SystemColor color)
    {
        return new InternalColor(color);
    }
}