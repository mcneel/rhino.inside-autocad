using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A constants class which defines the keys for Extensible Dictionaries in Autocad.
/// </summary>
public class XRecordKeys
{
    /// <summary>
    /// The Registered Application name key. The name is the identifier for the application
    /// for extended data and should not be changed.
    /// </summary>
    public const short ApplicationNameKey = 1001;

    /// <summary>
    /// The <see cref="IAutocadDocument.Id"/> key for extended data.
    /// </summary>
    public const short DocumentIdKey = 1000;
}