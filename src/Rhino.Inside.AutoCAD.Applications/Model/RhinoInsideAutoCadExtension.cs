using Autodesk.AutoCAD.Runtime;
using Bimorph.Core.Services.Services;
using Rhino.Inside.AutoCAD.Applications;
using Rhino.Inside.AutoCAD.Core.Interfaces.Applications.Applications;
using Rhino.Inside.AutoCAD.Interop;

[assembly: ExtensionApplication(typeof(RhinoInsideAutoCadExtension))]

namespace Rhino.Inside.AutoCAD.Applications;

/// <inheritdoc cref="IRhinoInsideAutoCadApplication"/>
public class RhinoInsideAutoCadExtension : IExtensionApplication
{
    /// <summary>
    /// The singleton instance of the <see cref="IRhinoInsideAutoCadApplication"/>
    /// </summary>
    public static IRhinoInsideAutoCadApplication? Application { get; private set; }

    /// <summary>
    /// Initialize the <see cref="IRhinoInsideAutoCadApplication"/>
    /// </summary>
    public void Initialize()
    {
        try
        {

            Application = new RhinoInsideAutoCadApplication();

            var rhinoInstance = RhinoInsideExtension.Instance;

            var editor = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument?.Editor;
            editor?.WriteMessage("\nRhino.Inside.AutoCAD loaded successfully.");
        }
        catch (System.Exception e)
        {
            var editor = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument?.Editor;
            editor?.WriteMessage($"\nERROR loading Rhino.Inside.AutoCAD: {e.Message}\n");
            editor?.WriteMessage($"\nStack trace: {e.StackTrace}\n");

            LoggerService.Instance?.LogError(e);
            throw;
        }
    }

    /// <summary>
    /// Terminate the <see cref="IRhinoInsideAutoCadApplication"/>
    /// </summary>
    public void Terminate()
    {
        try
        {
            Application?.Terminate();

        }
        catch (System.Exception e)
        {
            LoggerService.Instance?.LogError(e);
        }
    }
}
