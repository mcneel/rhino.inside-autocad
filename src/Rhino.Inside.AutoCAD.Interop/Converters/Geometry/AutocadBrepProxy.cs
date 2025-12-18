using Rhino.Inside.AutoCAD.Core.Interfaces;
using Surface = Autodesk.AutoCAD.DatabaseServices.Surface;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// We cannot create Breps in Autocad due to only primitive breps being exposed in the APIs.
/// We get around this limitation in two ways, if a true Brep are needed then the <see
/// cref="IBrepConverterRunner"/> should be used, this will use a native Autocad Import
/// function to create the brep, however this cannot be done synchronously and so when synchronous
/// conversions a required this  <see cref="AutocadBrepProxy"/> is the fallback option. In the <see
/// cref="AutocadBrepProxy"/> each face of the Brep is represented as a discrete Nurbs Surface.
/// It is not a Autocad Entity type, but rather a proxy object which can be used to represent a
/// Brep via its faces.
/// </summary>
public class AutocadBrepProxy : IEntity
{
    /// <inheritdoc />
    public IObjectId Id { get; }

    /// <inheritdoc />
    public Type Type { get; }

    /// <inheritdoc />
    public bool IsValid { get; }

    /// <inheritdoc />
    public string LayerName { get; }

    /// <inheritdoc />
    public string TypeName { get; }

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
        var firstFace = faces.FirstOrDefault();

        this.Id = new AutocadObjectId();

        this.Type = typeof(AutocadBrepProxy);

        this.IsValid = this.Id.IsValid;

        this.LayerName = firstFace?.Layer ?? string.Empty;

        this.TypeName = "Solid3d";

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

    /// <inheritdoc />
    public IDbObject ShallowClone()
    {
        var clones = new List<Surface>();
        foreach (var surface in this.Faces)
        {
            clones.Add(surface.Clone() as Surface);
        }

        return new AutocadBrepProxy(clones); ;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        foreach (var surface in this.Faces)
        {
            surface.Dispose();
        }
    }
}