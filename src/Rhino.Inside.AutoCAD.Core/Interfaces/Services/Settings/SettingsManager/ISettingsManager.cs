namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// The settings manager, This setting manager is used to import the <see cref="ISettingsCore"/>
/// and the <see cref="IApplicationSettings"/>. The <see cref="ISettingsCore"/> is the location
/// of the core settings which are common for all applications. The <typeparamref name=
/// "TApplicationSettings"/> is specific to the host application and will be different for
/// each application.
/// </summary>
public interface ISettingsManager<out TApplicationSettings, out TUserSettings>
    where TApplicationSettings : IApplicationSettings
    where TUserSettings : IUserSettings
{
    /// <summary>
    /// The core settings shared by all applications.
    /// </summary>
    public ISettingsCore Core { get; }

    /// <summary>
    /// The application specific settings.
    /// </summary>
    public TApplicationSettings Application { get; }

    /// <summary>
    /// The user specific settings.
    /// </summary>
    public TUserSettings User { get; }
}