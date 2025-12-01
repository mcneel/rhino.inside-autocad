namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface which registers and stores pages for the purposes of
/// posting notifications to the UI.
/// </summary>
public interface IDialogRegister
{
    /// <summary>
    /// Resolves the page in this register using the <paramref name="key"/>.
    /// </summary>
    object Resolve(string key);

    /// <summary>
    /// Registers the <paramref name="key"/> with the corresponding <paramref name="page"/>.
    /// </summary>
    /// <param name="key"> The key to register.</param>
    /// <param name="page"> The page instance to register and associate with the key.</param>
    void Register(string key, object page);

    /// <summary>
    /// Returns true if this <see cref="IDialogRegister"/> contains
    /// the <paramref name="key"/>.
    /// </summary>
    bool ContainsKey(string key);
}