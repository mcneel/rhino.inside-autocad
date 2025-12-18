using GH_IO.Serialization;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Reflection;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <inheritdoc cref="IComponentVersion"/>
public class ComponentVersion : IComponentVersion
{
    /// <summary>
    /// This is the version of the component when it was last saved.
    /// </summary>
    private Version _componentVersion;

    /// <inheritdoc />
    public Version CurrentVersion { get; }

    /// <summary>
    /// Creates a new <see cref="IComponentVersion"/> from its current Version.
    /// </summary>
    public ComponentVersion(Version currentVersion)
    {
        this.CurrentVersion = currentVersion;
        _componentVersion = currentVersion;
    }

    /// <inheritdoc />
    public bool Read(GH_IReader reader, string componentName)
    {
        var version = "0.0.0.0";
        reader.TryGetString("ComponentVersion", ref version);
        _componentVersion = Version.TryParse(version, out var componentVersion) ?
            componentVersion : new Version(0, 0, 0, 0);

        if (_componentVersion > this.CurrentVersion)
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title;
            reader.AddMessage
            (
                $"Component '{componentName}' was saved with a newer version.\n" +
                "Some information may be lost\n" +
                $"Please update '{assemblyName}' to version {_componentVersion} or above.",
                GH_Message_Type.warning
            );
        }

        return true;
    }

    /// <inheritdoc />
    public bool Write(GH_IWriter writer)
    {
        writer.SetString("ComponentVersion", this.CurrentVersion.ToString());

        return true;
    }
}