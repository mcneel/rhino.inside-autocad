using Bimorph.Core.Services.Services;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace AWI.SectionDetailsTool;

/// The Entry point to the rhino launcher application.
/// <inheritdoc cref="ApplicationMainBase"/>
public class RhinoLauncherMain : ApplicationMainBase
{

    /// <summary>
    /// Constructs a new <see cref="RhinoLauncherMain"/>.
    /// </summary>
    public RhinoLauncherMain(IRhinoInsideAutoCadApplication application)
        : base(application)
    {

    }

    ///<inheritdoc />
    protected override void RegisterTypes()
    {

    }
}
