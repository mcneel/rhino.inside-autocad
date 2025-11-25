using Bimorph.Core.Services.Services;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace AWI.SectionDetailsTool;

/// The Entry point to the rhino launcher application.
/// <inheritdoc cref="ApplicationMainBase"/>
public class GrasshopperLauncherMain : ApplicationMainBase
{
    /// <summary>
    /// Constructs a new <see cref="GrasshopperLauncherMain"/>.
    /// </summary>
    public GrasshopperLauncherMain(IRhinoInsideAutoCadApplication application)
        : base(application)
    {

    }

    ///<inheritdoc />
    protected override void RegisterTypes()
    {

    }
}
