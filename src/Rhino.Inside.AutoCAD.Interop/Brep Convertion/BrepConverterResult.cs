using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IBrepConverterResult"/>
public class BrepConverterResult : IBrepConverterResult
{
    /// <inheritdoc />
    public IEntitySet ConvertedSolids { get; }

    /// <inheritdoc />
    public bool Success { get; }

    /// <summary>
    /// Constructs a new <see cref="IBrepConverterResult"/>
    /// </summary>
    public BrepConverterResult(List<Solid3d> solids)
    {
        this.ConvertedSolids = new EntitySet();

        foreach (var solid in solids)
        {
            var entity = new AutocadEntityWrapper(solid);

            this.ConvertedSolids.Add(entity);
        }

        this.Success = solids?.Count > 0;
    }
}