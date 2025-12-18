using Grasshopper.Kernel;

namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Base interface for all Parameter types in Rhino.Inside.Autocad that reference Autocad
/// elements.
/// </summary>
public interface IReferenceParam : IGH_Param
{
    /// <summary>
    /// Each implementing Parameter must define whether it needs to be expired based on
    /// the given <see cref="IAutocadDocumentChange"/> in the <see cref="IAutocadDocument"/>.
    /// If it does, the Parameter will be expired to trigger computation.
    /// </summary>
    bool NeedsToBeExpired(IAutocadDocumentChange change);
}