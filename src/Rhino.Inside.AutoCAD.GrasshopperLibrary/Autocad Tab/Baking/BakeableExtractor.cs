using Rhino.Inside.AutoCAD.Core.Interfaces;
using RhinoArc = Rhino.Geometry.Arc;
using RhinoArcCurve = Rhino.Geometry.ArcCurve;
using RhinoBox = Rhino.Geometry.Box;
using RhinoBrep = Rhino.Geometry.Brep;
using RhinoCircle = Rhino.Geometry.Circle;
using RhinoGeometryBase = Rhino.Geometry.GeometryBase;
using RhinoLine = Rhino.Geometry.Line;
using RhinoLineCurve = Rhino.Geometry.LineCurve;
using RhinoPoint = Rhino.Geometry.Point;
using RhinoPoint3d = Rhino.Geometry.Point3d;
using RhinoRectangle3d = Rhino.Geometry.Rectangle3d;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Extracts <see cref="IAutocadBakeable"/> instances from arbitrary Grasshopper input
/// objects. Shared by the baking components (<see cref="AutocadBakeComponent"/> and
/// <see cref="TrackedBakeComponent"/>).
/// </summary>
public class BakeableExtractor
{
    private readonly IRhinoConvertibleFactory _factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="BakeableExtractor"/> class.
    /// </summary>
    /// <param name="factory">The factory used to convert Rhino geometry into bakeables.</param>
    public BakeableExtractor(IRhinoConvertibleFactory factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Wraps a Rhino <see cref="RhinoGeometryBase"/> in an <see cref="IAutocadBakeable"/>
    /// using the convertible factory, or returns <c>null</c> if it cannot be converted.
    /// </summary>
    private IAutocadBakeable? Convert(RhinoGeometryBase? geometry)
    {
        if (geometry is null)
            return null;

        if (_factory.MakeConvertible(geometry, out var rhinoConvertible) == false)
            return null;

        return new BakableRhinoConverter(rhinoConvertible!, geometry);
    }

    /// <summary>
    /// Extracts an <see cref="IAutocadBakeable"/> from the input object.
    /// </summary>
    public IAutocadBakeable? ExtractBakeable(object? obj)
    {
        if (obj is IAutocadBakeable bakeable)
            return bakeable;

        if (obj is Grasshopper.Kernel.Types.IGH_Goo goo)
        {
            var valueProperty = goo.GetType().GetProperty("Value");

            if (valueProperty != null)
            {
                var value = valueProperty.GetValue(goo);

                if (value is IAutocadBakeable valueBakeable)
                    return valueBakeable;

                // Several Grasshopper primitives expose a value-type struct as their Value
                // (Line, Arc, Circle, Rectangle3d, Point3d, Box) which is not a GeometryBase.
                // Normalize these into the appropriate bakeable, mirroring the conversions used
                // by GrasshopperGeometryExtractor for previews. Breps (and Boxes, which become
                // Breps) bake via GH_AutocadBrepProxy; everything else via the convertible factory.
                var valueBakeableResult = value switch
                {
                    RhinoLine line => this.Convert(new RhinoLineCurve(line)),
                    RhinoArc arc => this.Convert(new RhinoArcCurve(arc)),
                    RhinoCircle circle => this.Convert(new RhinoArcCurve(circle)),
                    RhinoRectangle3d rectangle => this.Convert(rectangle.ToNurbsCurve()),
                    RhinoPoint3d point => this.Convert(new RhinoPoint(point)),
                    RhinoBox box => new GH_AutocadBrepProxy(box.ToBrep()),
                    RhinoBrep brep => new GH_AutocadBrepProxy(brep),
                    RhinoGeometryBase nativeGeometry => this.Convert(nativeGeometry),
                    _ => null
                };

                if (valueBakeableResult is not null)
                    return valueBakeableResult;
            }

            if (goo is IAutocadBakeable gooBakeable)
                return gooBakeable;

        }

        return null;
    }
}
