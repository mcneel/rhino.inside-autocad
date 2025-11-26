using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A constants class which defines the key for a <see cref="IObjectIdTagDatabase"/>.
/// </summary>
public class TagDatabaseKeys
{
    /// <summary>
    /// The key for the <see cref="IObjectIdTagDatabase"/> containing
    /// <see cref="IObjectIdTagRecord"/>s relating to <see cref="IPanelType"/>s.
    /// </summary>
    public const string PanelTypeIdsKey = "PanelTypes";

    /// <summary>
    /// The key for the <see cref="IObjectIdTagDatabase"/> containing
    /// <see cref="IObjectIdTagRecord"/>s relating to <see cref="ISheet"/>s.
    /// </summary>
    public const string Sheets = "Sheets";
}