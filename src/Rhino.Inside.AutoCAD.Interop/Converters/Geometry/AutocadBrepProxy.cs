using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// We cannot create Breps in Autocad due to only primitive breps being exposed in the APIs.
/// We get around this limitation in two ways, if a true Brep are needed then the <see
/// cref="IBrepConverterRunner"/> should be used, this will use a native Autocad Import
/// function to create the brep, however this cannot be done synchronously and so when synchronous
/// conversions a required this  <see cref="AutocadBrepProxy"/> is the fallback option. In the <see
/// cref="AutocadBrepProxy"/> each face of the Brep is represented as a discrete Nurbs Surface.
/// </summary>
public class AutocadBrepProxy : NurbSurface
{
    /// <summary>
    /// Gets the collection of faces that make up the Brep proxy.
    /// Each face is represented as a discrete NURBS surface.
    /// </summary>
    public List<Surface> Faces { get; } = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="AutocadBrepProxy"/> class with the specified faces.
    /// </summary>
    /// <param name="faces">A list of surfaces representing the faces of the Brep.</param>
    public AutocadBrepProxy(List<Surface> faces) : base()
    {
        foreach (var nurbsSurface in faces)
        {
            this.Add(nurbsSurface);
        }
    }

    /// <summary>
    /// Adds a face to the Brep proxy.
    /// </summary>
    /// <param name="face">The surface representing the face to add.</param>
    private void Add(Surface face)
    {
        this.Faces.Add(face);
    }

    /// <summary>
    /// Overrides the Clone method to create a copy of the Brep proxy.
    /// </summary>
    /// <returns></returns>
    public override object Clone()
    {
        var clones = new List<Surface>();
        foreach (var surface in this.Faces)
        {
            clones.Add(surface.Clone() as Surface);
        }

        return new AutocadBrepProxy(clones);
    }
}