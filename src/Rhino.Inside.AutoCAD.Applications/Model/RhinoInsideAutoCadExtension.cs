using Autodesk.AutoCAD.Runtime;
using Rhino.Inside.AutoCAD.Applications;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using Rhino.Inside.AutoCAD.Services;
using System.Globalization;
using System.Reflection;
using Exception = System.Exception;

[assembly: ExtensionApplication(typeof(RhinoInsideAutoCadExtension))]

namespace Rhino.Inside.AutoCAD.Applications;

/// <inheritdoc cref="IRhinoInsideAutoCadApplication"/>
public class RhinoInsideAutoCadExtension : IExtensionApplication
{
    private const string _applicationLoadedSuccessMessage = ApplicationConstants.ApplicationLoadedSuccessMessage;
    private const string _applicationLoadErrorMessageFormat = ApplicationConstants.ApplicationLoadErrorMessageFormat;
    private const string _stackTraceMessageFormat = ApplicationConstants.StackTraceMessageFormat;
    private const string _expiredMessage = ApplicationConstants.ExpiredMessage;

    /// <summary>
    /// The singleton instance of the <see cref="IRhinoInsideAutoCadApplication"/>
    /// </summary>
    public static IRhinoInsideAutoCadApplication? Application { get; private set; }

    /// <summary>
    /// Initialize the <see cref="IRhinoInsideAutoCadApplication"/>
    /// </summary>
    public void Initialize()
    {

        var currentDate = System.DateTime.Now;

        var compliedDate = this.GetCompliedDate();

        //var limitDate = compliedDate.AddDays(90);
        var limitDate = compliedDate.AddMinutes(5);

        if (currentDate > limitDate)
        {
            var editor = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument?.Editor;

            editor?.WriteMessage(_expiredMessage);

            LoggerService.Instance?.LogMessage(_expiredMessage);

            throw new Exception(_expiredMessage);
        }

        try
        {
            // Force RhinoCoreExtension static constructor to run first
            // This sets up the AssemblyResolve handler for RhinoCommon before
            // any code tries to reference RhinoCommon types
            _ = RhinoCoreExtension.Instance;

            Application = new RhinoInsideAutoCadApplication();

            var editor = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument?.Editor;
            editor?.WriteMessage(_applicationLoadedSuccessMessage);
        }
        catch (System.Exception e)
        {
            var editor = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument?.Editor;
            editor?.WriteMessage(string.Format(_applicationLoadErrorMessageFormat, e.Message));
            editor?.WriteMessage(string.Format(_stackTraceMessageFormat, e.StackTrace));

            LoggerService.Instance?.LogError(e);
            throw;
        }
    }

    /// <summary>
    /// Returns the date the assembly was compiled.
    /// </summary>
    private DateTime GetCompliedDate()
    {
        var assembly = Assembly.GetExecutingAssembly();

        const string BuildVersionMetadataPrefix = "+build";

        var attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        if (attribute?.InformationalVersion != null)
        {
            var value = attribute.InformationalVersion;
            var index = value.IndexOf(BuildVersionMetadataPrefix);
            if (index > 0)
            {
                value = value.Substring(index + BuildVersionMetadataPrefix.Length);
                if (DateTime.TryParseExact(value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                {
                    return result;
                }
            }
        }

        return default;
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
