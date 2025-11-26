namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a type that provides <see cref="ICustomProperty"/>s. This is used to
/// obtain custom properties from domain types to display this information on generated
/// <see cref="IDrawing"/>s.
/// </summary>
public interface ICustomPropertyProvider
{
    /// <summary>
    /// Returns <see cref="ICustomProperty"/>s of the <see cref="ICustomPropertyProvider"/>.
    /// </summary>
    ICustomPropertySet GetCustomProperties();
}