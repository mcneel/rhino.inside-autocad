using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD selection filters.
/// </summary>
public class GH_AutocadFilter : GH_Goo<IFilter>
{
    /// <inheritdoc />
    public override bool IsValid => this.Value != null;

    /// <inheritdoc />
    public override string TypeName => "Filter";

    /// <inheritdoc />
    public override string TypeDescription => "Represents an AutoCAD selection filter";

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadFilter"/> class with no value.
    /// </summary>
    public GH_AutocadFilter()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadFilter"/> class with the
    /// specified filter.
    /// </summary>
    /// <param name="filter">The filter to wrap.</param>
    public GH_AutocadFilter(IFilter filter) : base(filter)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadFilter"/> class by copying
    /// another instance.
    /// </summary>
    /// <param name="other">The instance to copy.</param>
    public GH_AutocadFilter(GH_AutocadFilter other)
    {
        this.Value = other.Value;
    }

    /// <inheritdoc />
    public override IGH_Goo Duplicate()
    {
        return new GH_AutocadFilter(this);
    }

    /// <inheritdoc />
    public override bool CastFrom(object source)
    {
        if (source is GH_AutocadFilter goo)
        {
            this.Value = goo.Value;
            return true;
        }

        if (source is IFilter filter)
        {
            this.Value = filter;
            return true;
        }

        if (source is GH_String ghString)
        {
            return this.TryParseFilterName(ghString.Value);
        }

        if (source is string filterName)
        {
            return this.TryParseFilterName(filterName);
        }

        return false;
    }

    /// <summary>
    /// Attempts to parse a filter name string and create the corresponding filter.
    /// </summary>
    /// <param name="filterName">The filter name to parse.</param>
    /// <returns>True if the filter was successfully created; otherwise, false.</returns>
    private bool TryParseFilterName(string filterName)
    {
        var filter = filterName.ToLowerInvariant() switch
        {
            "curve" => new CurveFilter() as IFilter,
            "point" => new PointFilter(),
            "mesh" => new MeshFilter(),
            "solid" => new SolidFilter(),
            "hatch" => new HatchFilter(),
            "dimension" => new DimensionFilter(),
            "text" => new TextFilter(),
            "leader" => new LeaderFilter(),
            "block" => new BlockReferenceFilter(),
            _ => null
        };

        if (filter == null)
            return false;

        this.Value = filter;
        return true;
    }

    /// <inheritdoc />
    public override bool CastTo<Q>(ref Q target)
    {
        if (typeof(Q).IsAssignableFrom(typeof(IFilter)))
        {
            target = (Q)this.Value;
            return true;
        }

        if (typeof(Q).IsAssignableFrom(typeof(GH_AutocadFilter)))
        {
            target = (Q)(object)new GH_AutocadFilter(this.Value);
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (this.Value == null)
            return "Null Filter";

        return $"Filter [{this.Value.GetType().Name.Replace("Filter", "")}]";
    }
}
