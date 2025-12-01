namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface defining directories to application resources and assembly
/// files.
/// </summary>
public interface IApplicationDirectories
{
    /// <summary>
    /// The directory of the application root install location.
    /// </summary>
    string RootInstall { get; }

    /// <summary>
    /// The application resources directory in the <see cref="RootInstall"/> location.
    /// </summary>
    string Resources { get; }

    /// <summary>
    /// The directory of the application assembly files in the <see cref="RootInstall"/>
    /// location.
    /// </summary>
    string Assemblies { get; }

    /// <summary>
    /// The directory for local user settings and resources.
    /// </summary>
    string UserLocal { get; }

    /// <summary>
    /// The Bimorph application data folder, this is the folder under the user's
    /// AppData\Roaming where application data can be stored. This is typically used by
    /// the logging, testing and reporting functionality of Bimorph applications.
    /// </summary>
    string BimorphAppData { get; }

    /// <summary>
    /// The folder containing the user tests for the application, these are the test which
    /// the user can run form the Bimorph Reporter application for this application. It is
    /// also used to create the <see cref="IScopeItemSet"/> for <see cref="IScopeId"/> which
    /// will be used when logging. This is typically the "BimorphAppData/ApplicationName/Tests"
    /// folder.
    /// </summary>
    string UserTests { get; }

    /// <summary>
    /// The support application folder, this is the folder where the <see cref="ISupportedApplication"/>
    /// json files are stored. This is the "BimorphAppData/SupportApplication".
    /// folder.
    /// </summary>
    string SupportedApplication { get; }

    /// <summary>
    /// The folder which contains the usage registers for the application. This is typically
    ///  the "BimorphAppData/ApplicationName/Usage" folder.
    /// </summary>
    string UsageMetrics { get; }

    /// <summary>
    /// The name of the current application. This is name of the application which get
    /// displayed to the user. Typically, it will contain a space, e.g. "Bimorph Scope
    /// Builder".
    /// </summary>
    string ApplicationName { get; }

    /// <summary>
    /// The Product name of the current application. This is name of the application which get
    /// is used to identify the application in file and folder names. Typically, it will contain
    /// dot for the space in the application name, e.g. "Bimorph.Scope.Builder". This must
    /// match the Product name in the application assembly info (Build.props).
    /// </summary>
    string ProductName { get; }
}