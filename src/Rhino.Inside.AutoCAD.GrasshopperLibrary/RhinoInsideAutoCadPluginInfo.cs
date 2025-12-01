using Grasshopper.Kernel;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Provides metadata about the Rhino.Inside.AutoCAD plugin for Grasshopper.
/// </summary>
public class RhinoInsideAutoCadPluginInfo : GH_AssemblyInfo
{
    /// <summary>
    /// Gets the name of the plugin.
    /// </summary>
    public override string Name => "Rhino.Inside.AutoCAD Plugin";

    /// <summary>
    /// Gets the icon representing the plugin. Returns <c>null</c> as no icon is provided.
    /// </summary>
    public override System.Drawing.Bitmap Icon => null;

    /// <summary>
    /// Gets the description of the plugin.
    /// </summary>
    public override string Description => "A Rhino Inside Plugin";

    /// <summary>
    /// Gets the unique identifier (GUID) of the plugin.
    /// </summary>
    public override Guid Id => new Guid("ABCDEF12-3456-7890-ABCD-EF1234567890");

    /// <summary>
    /// Gets the name of the plugin author.
    /// </summary>
    public override string AuthorName => "Bimorph";

    /// <summary>
    /// Gets the contact information for the plugin author.
    /// </summary>
    public override string AuthorContact => "support@bimorph.com";
}