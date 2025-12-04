using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface defining a contract to importing layers from an external
/// template file that adhere to AWI CAD standards.
/// </summary>
public interface ILayerImporter
{
    /// <summary>
    /// Imports the layers from the external layers template file (accessible via a
    /// <see cref="ISettingInput"/> in the UI) and creates them in the active document
    /// using the <see cref="ILayerRepository.TryAddLayer"/> method.
    /// </summary>
    void Import();
}