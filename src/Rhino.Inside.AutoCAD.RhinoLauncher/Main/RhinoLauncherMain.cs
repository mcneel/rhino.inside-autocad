using Bimorph.Core.Services.Services;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Core.Interfaces.Applications.Applications;

namespace Rhino.Inside.AutoCAD.RhinoLauncher;

/// The Entry point to the rhino launcher application.
/// <inheritdoc cref="ApplicationMainBase"/>
public class RhinoLauncherMain : ApplicationMainBase
{

    /// <summary>
    /// Constructs a new <see cref="RhinoLauncherMain"/>.
    /// </summary>
    public RhinoLauncherMain(IRhinoInsideAutoCadApplication application, IInteropService interopService)
        : base(application)
    {


    }

    ///<inheritdoc />
    protected override void RegisterTypes()
    {

    }
}
