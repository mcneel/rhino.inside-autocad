using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Resolves the DXF names that AutoCAD selection filters match on from the runtime classes
/// of the entities being selected.
/// </summary>
/// <remarks>
/// An entity's DXF name is neither its managed class name nor the command that creates it:
/// an <see cref="MLeader"/> is MULTILEADER, a <see cref="Polyline"/> is LWPOLYLINE and a
/// <see cref="BlockReference"/> is INSERT. Writing the names out by hand is how the leader
/// filter came to spell MLEADER and match no multileader at all - a filter that names a
/// type wrongly does not fail, it silently selects nothing. Read from the runtime class,
/// the name cannot drift.
/// </remarks>
public static class DxfName
{
    /// <summary>
    /// Returns the DXF name registered for the specified AutoCAD type.
    /// </summary>
    /// <typeparam name="TDbObject">The AutoCAD type to resolve the DXF name of.</typeparam>
    /// <returns>The DXF name, for example INSERT for a block reference.</returns>
    public static string Of<TDbObject>() where TDbObject : DBObject
    {
        return RXClass.GetClass(typeof(TDbObject)).DxfName;
    }

    /// <summary>
    /// Returns the DXF names of the specified AutoCAD types as the comma separated list a
    /// selection filter matches a group of entity types with.
    /// </summary>
    /// <param name="types">The AutoCAD types to resolve the DXF names of.</param>
    /// <returns>The DXF names, comma separated.</returns>
    public static string Of(params Type[] types)
    {
        var names = types.Select(type => RXClass.GetClass(type).DxfName);

        return string.Join(",", names);
    }
}
