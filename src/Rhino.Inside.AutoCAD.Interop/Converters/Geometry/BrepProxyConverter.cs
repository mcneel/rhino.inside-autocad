using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using RhinoBrep = Rhino.Geometry.Brep;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Converts between <see cref="AutocadBrepProxy"/> and <see cref="RhinoBrep"/>
/// and <see cref="Solid3d"/>. See <see cref="AutocadBrepProxy"/> for more information.
/// </summary>
/// <remarks>
/// Note the conversion between <see cref="AutocadBrepProxy"/> and <see cref="Solid3d"/>
/// is deliberately one way only, as we cannot create true Breps in Autocad via the APIs.
/// If this is required then the <see cref="IBrepConverterRunner"/> should be used.
/// </remarks>
public partial class GeometryConverter
{
    /// <summary>
    /// Converts a <see cref="AutocadBrepProxy"/> to a <see cref="RhinoBrep"/>.
    /// </summary>
    public AutocadBrepProxy? ToProxyType(RhinoBrep rhinoBrep)
    {
        var faces = new List<Surface>();

        foreach (var surface in rhinoBrep.Faces)
        {
            var nurbsSurface = surface.ToNurbsSurface();

            var cadNurbsSurface = this.ToAutoCadType(nurbsSurface);

            faces.Add(cadNurbsSurface);
        }

        return new AutocadBrepProxy(faces);
    }

    /// <summary>
    /// Converts a <see cref="AutocadBrepProxy"/> to a <see cref="RhinoBrep"/>.
    /// </summary>
    public RhinoBrep? FromProxyType(AutocadBrepProxy brepProxy)
    {
        var breps = new List<RhinoBrep>();

        foreach (var surface in brepProxy.Faces)
        {
            var rhinoSurfaces = this.ToRhinoType(surface);

            foreach (var rhinoSurface in rhinoSurfaces)
            {
                var rhinoBrep = RhinoBrep.CreateFromSurface(rhinoSurface);

                breps.Add(rhinoBrep);
            }
        }

        var joinedBrep = RhinoBrep.JoinBreps(breps, _zeroTolerance);

        return joinedBrep.FirstOrDefault();
    }

    /// <summary>
    /// Converts a <see cref="AutocadBrepProxy"/> to a <see cref="RhinoBrep"/>.
    /// </summary>
    public AutocadBrepProxy? ToProxyType(Solid3d cadSolid3d)
    {
        if (cadSolid3d == null)
            throw new ArgumentNullException(nameof(cadSolid3d));

        var faces = new List<Surface>();

        var explodedObjects = new DBObjectCollection();
        cadSolid3d.Explode(explodedObjects);

        foreach (DBObject obj in explodedObjects)
        {
            if (obj is Region region)
            {
                var planeSurface = new PlaneSurface();
                planeSurface.CreateFromRegion(region);

                faces.Add(planeSurface);

                region.Dispose();
            }
            else if (obj is Surface surface)
            {
                faces.Add(surface);
            }
        }
        return new AutocadBrepProxy(faces);
    }
}