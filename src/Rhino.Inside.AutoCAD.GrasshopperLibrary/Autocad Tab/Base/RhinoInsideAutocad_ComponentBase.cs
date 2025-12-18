using GH_IO.Serialization;
using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary.Autocad_Tab.Base;

/// <summary>
/// The base class for all Rhino.Inside.AutoCAD Grasshopper components.
/// This class extends the <see cref="GH_Component"/> class and provides mechanisms
/// to update component versions based on changes to the component itself.
/// </summary>
public abstract class RhinoInsideAutocad_ComponentBase : GH_Component
{
    private readonly VersioningIssues _versioningStatus = VersioningIssues.None;

    /// <summary>
    /// The version of the component.
    /// </summary>
    protected IComponentVersion Version { get; private set; }

#if DEBUG
    /// <summary>
    /// Adds versioning information to the instance description in debug builds.
    /// </summary>
    public override string InstanceDescription =>
        $"{base.InstanceDescription}\n{this.GetFullVersionDescription()}";
#endif

    /// <summary>
    /// Overrides the Obsolete property to determine if the component is obsolete
    /// </summary>
    public override bool Obsolete => _versioningStatus.HasFlag(VersioningIssues.Obsolete) || base.Obsolete;

    /// <summary>
    /// Constructs a new instance of the <see cref="RhinoInsideAutocad_ComponentBase"/> class.
    /// </summary>
    protected RhinoInsideAutocad_ComponentBase(
        string name,
        string nickname,
        string description,
        string category,
        string subCategory) : base(name, nickname, description, category, subCategory)
    {
        this.Version = this.GetCurrentVersion();

        ComponentVersionAttribute.TryGetVersionHistory(this.GetType(), out var versionHistory);
        if (this.Obsolete || versionHistory!.IsDeprecated) _versioningStatus |= VersioningIssues.Obsolete;
    }

    /// <summary>
    /// Gets a full version description of the component, including its introduction and deprecation details.
    /// </summary>
    private string GetFullVersionDescription()
    {
        ComponentVersionAttribute.TryGetVersionHistory(this.GetType(), out var versionHistory);

        var versionDescription = string.Empty;

        versionDescription += $"Introduced in v{versionHistory!.Introduced}\n";

        if (this.Obsolete)
        {
            if (versionHistory.TryGetDepreciatedVersion(out var depreciatedVersion))
                versionDescription += $"Obsolete since v{depreciatedVersion}\n";

            foreach (var attribute in this.GetType().GetCustomAttributes(typeof(ObsoleteAttribute), false).Cast<ObsoleteAttribute>())
            {
                if (string.IsNullOrWhiteSpace(attribute.Message) == false)
                    versionDescription += $"{attribute.Message}\n";
            }
        }

        return versionDescription;

    }

    /// <summary>
    /// Gets the current version of the component based on its type and the types of its
    /// input and output parameters.
    /// </summary>
    private IComponentVersion GetCurrentVersion()
    {
        var current = ComponentVersionAttribute.GetCurrentVersion(this.GetType());

        foreach (var input in this.Params.Input)
        {
            var version = ComponentVersionAttribute.GetCurrentVersion(input.GetType());
            if (version > current) current = version;
        }

        foreach (var output in this.Params.Output)
        {
            var version = ComponentVersionAttribute.GetCurrentVersion(output.GetType());
            if (version > current) current = version;
        }

        return new ComponentVersion(current);
    }

    /// <inheritdoc />
    public override bool Read(GH_IReader reader)
    {
        if (!base.Read(reader))
            return false;

        this.Version.Read(reader, this.Name);

        return true;
    }

    /// <inheritdoc />
    public override bool Write(GH_IWriter writer)
    {
        if (!base.Write(writer))
            return false;

        this.Version.Write(writer);

        return true;
    }
}
