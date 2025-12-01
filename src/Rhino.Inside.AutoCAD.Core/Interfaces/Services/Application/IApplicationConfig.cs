namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// The application configuration interface. This interface provides basic information about the application
/// </summary>
public interface IApplicationConfig
{
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

    /// <summary>
    /// The folder name used for application data. This can be used in multiple places
    /// for example under the user's AppData\Roaming folder or in the program files folder.
    /// </summary>
    string ClientFolderName { get; }

    /// <summary>
    /// The root installation directory of the application.
    /// </summary>
    string RootInstallDirectory { get; }

    /// <summary>
    /// The prefix name used for packages, for example "Bimorph.".
    /// </summary>
    string PackagePrefixName { get; }
}