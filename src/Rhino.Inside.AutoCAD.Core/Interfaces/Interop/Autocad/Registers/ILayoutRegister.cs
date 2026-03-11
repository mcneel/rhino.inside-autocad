namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A register storing all <see cref="IAutocadLayout"/>s
/// in the active <see cref="IAutocadDocument"/>.
/// </summary>
public interface ILayoutRegister : IRegister<IAutocadLayout>
{
    /// <summary>
    /// Tries to add a new <see cref="IAutocadLayout"/> to the register.
    /// </summary>
    bool TryAddLayout(string name, out IAutocadLayout? layout);
}