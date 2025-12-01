namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface which registers and stores pages for the purposes of
/// navigation using a key to identify the page.
/// </summary>
public interface IPageRegister
{
    /// <summary>
    /// Resolves the page in this register using the <paramref name="pageKey"/>.
    /// </summary>
    object Resolve(string pageKey);

    /// <summary>
    /// Registers the <paramref name="pageKey"/> with the corresponding
    /// <paramref name="page"/>.
    /// </summary>
    /// <param name="pageKey"> The page key to register.</param>
    /// <param name="page"> The page instance to register and associate with the key.</param>
    void Register<T>(string pageKey, T page) where T : class;

    /// <summary>
    /// Returns true if this <see cref="IPageRegister"/> contains the
    /// <paramref name="key"/>.
    /// </summary>
    bool ContainsKey(string key);
}