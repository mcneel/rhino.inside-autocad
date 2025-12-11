using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD bake settings.
/// </summary>
public class GH_BakeSettings : GH_Goo<BakeSettings>
{
    /// <inheritdoc />
    public override bool IsValid => this.Value != null;

    /// <inheritdoc />
    public override string TypeName => nameof(BakeSettings);

    /// <inheritdoc />
    public override string TypeDescription => "Represents bake settings for AutoCAD objects";

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_BakeSettings"/> class with no value.
    /// </summary>
    public GH_BakeSettings()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_BakeSettings"/> class with the
    /// specified bake settings.
    /// </summary>
    /// <param name="settings">The bake settings to wrap.</param>
    public GH_BakeSettings(BakeSettings settings) : base(settings)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_BakeSettings"/> class by copying
    /// another instance.
    /// </summary>
    /// <param name="other">The instance to copy.</param>
    public GH_BakeSettings(GH_BakeSettings other)
    {
        this.Value = other.Value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_BakeSettings"/> class from
    /// the interface.
    /// </summary>
    /// <param name="settings">The bake settings interface.</param>
    public GH_BakeSettings(IBakeSettings settings)
        : base((settings as BakeSettings)!)
    {
    }

    /// <inheritdoc />
    public override IGH_Goo Duplicate()
    {
        return new GH_BakeSettings(this);
    }

    /// <inheritdoc />
    public override bool CastFrom(object source)
    {
        if (source is GH_BakeSettings goo)
        {
            this.Value = goo.Value;
            return true;
        }

        if (source is BakeSettings settings)
        {
            this.Value = settings;
            return true;
        }

        if (source is IBakeSettings iSettings)
        {
            this.Value = (BakeSettings)iSettings;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override bool CastTo<Q>(ref Q target)
    {
        if (typeof(Q).IsAssignableFrom(typeof(BakeSettings)))
        {
            target = (Q)(object)this.Value;
            return true;
        }

        if (typeof(Q).IsAssignableFrom(typeof(IBakeSettings)))
        {
            target = (Q)(object)this.Value;
            return true;
        }

        if (typeof(Q).IsAssignableFrom(typeof(GH_BakeSettings)))
        {
            target = (Q)(object)new GH_BakeSettings(this.Value);
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (this.Value == null)
            return "Null Bake Settings";

        var parts = new List<string>();
        if (this.Value.Layer != null)
            parts.Add($"Layer: {this.Value.Layer.Name}");
        if (this.Value.LineType != null)
            parts.Add($"LineType: {this.Value.LineType.Name}");
        if (this.Value.Color != null)
            parts.Add($"Color: RGB({this.Value.Color.Red},{this.Value.Color.Green},{this.Value.Color.Blue})");

        return parts.Count > 0
            ? $"Bake Settings [{string.Join(", ", parts)}]"
            : "Bake Settings [Default]";
    }
}
