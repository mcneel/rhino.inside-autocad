using Rhino.Inside.AutoCAD.Core.Interfaces;
using CadPlotSettings = Autodesk.AutoCAD.DatabaseServices.PlotSettings;

namespace Rhino.Inside.AutoCAD.Interop;

///<inheritdoc cref="IPlotSettings"/>
public class PlotSettings : WrapperDisposableBase<CadPlotSettings>, IPlotSettings
{
    ///<inheritdoc />
    public IObjectId Id { get; }

    ///<inheritdoc />
    public string Name { get; set; }

    /// <summary>
    /// Constructs a new <see cref="PlotSettings"/>.
    /// </summary>
    public PlotSettings(CadPlotSettings plotSettings) : base(plotSettings)
    {
        this.Id = new ObjectId(plotSettings.Id);

        this.Name = plotSettings.PlotSettingsName;
    }
}