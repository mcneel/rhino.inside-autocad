using Grasshopper.Kernel;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A comparer for comparing assembly stubs based on their names.
/// </summary>
public class GH_AssemblyInfoStubComparer : IEqualityComparer<GH_AssemblyInfo>
{
    /// <summary>
    /// Determines whether the specified assemblies are equal by comparing their names.
    /// </summary>
    /// <param name="x">The first assembly to compare.</param>
    /// <param name="y">The second assembly to compare.</param>
    /// <returns>True if the assembly names are equal; otherwise, false.</returns>
    public bool Equals(GH_AssemblyInfo? x, GH_AssemblyInfo? y)
    {
        if (x == null || y == null || x.Assembly == null)
            return false;

        return string.Equals(x.Assembly.GetName().Name, y.Name, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Returns a hash code for the specified assembly based on its name.
    /// </summary>
    /// <param name="obj">The assembly for which a hash code is to be returned.</param>
    /// <returns>A hash code for the assembly name.</returns>
    public int GetHashCode(GH_AssemblyInfo obj)
    {
        return obj.Name?.GetHashCode() ?? 0;
    }
}
