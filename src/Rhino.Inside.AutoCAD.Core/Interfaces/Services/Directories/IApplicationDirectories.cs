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