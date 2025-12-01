namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A service to replace button icons in the AutoCAD ribbon.
/// </summary>
public interface IButtonIconReplacer
{
    /// <summary>
    /// The Button Id of the button to replace the icon for. This can be
    /// found in the RhinoInsideAutocad_Toolbar.cuix via CUI editor in AutoCAD.
    /// </summary>
    string ButtonId { get; }

    /// <summary>
    /// Replaces the icon of the button with the icon located at <paramref
    /// name="buttonFilePath"/>.
    /// </summary>
    void Replace(string buttonFilePath);
}