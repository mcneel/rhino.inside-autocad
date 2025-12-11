using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using AutocadCurve = Autodesk.AutoCAD.DatabaseServices.Curve;
using AutocadMesh = Autodesk.AutoCAD.DatabaseServices.PolyFaceMesh;
using AutocadPoint = Autodesk.AutoCAD.DatabaseServices.DBPoint;
using AutocadSolid = Autodesk.AutoCAD.DatabaseServices.Solid3d;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Provides methods to convert objects to specific types or retrieve their identifiers.
/// </summary>
public class GooConverter
{
    /// <summary>
    /// Attempts to convert the specified source object to the target type <typeparamref
    /// name="TTarget"/>.
    /// </summary>
    /// <typeparam name="TTarget">The type to which the source object should be converted.
    /// </typeparam>
    /// <param name="source">The object to be converted.</param>
    /// <param name="target">The converted object if the conversion is successful;
    /// otherwise, the default value of <typeparamref name="TTarget"/>.</param>
    /// <returns>
    /// <see langword="true"/> if the conversion is successful; otherwise,
    /// <see langword="false"/>.
    /// </returns>
    public bool TryConvertFromGoo<TTarget>(object source, out TTarget? target)
    {
        if (source is TTarget isCast)
        {
            target = isCast;
            return true;
        }

        if (source is IGH_AutocadReferenceObject ghAutocadReferenceObject)
        {
            var dbObject = ghAutocadReferenceObject.ObjectValue;
            if (dbObject is TTarget targetCast)
            {
                target = targetCast;
                return true;
            }
        }

        if (source is IDbObject autocadReferenceObject)
        {
            var dbObject = autocadReferenceObject;
            if (dbObject is TTarget targetCast)
            {
                target = targetCast;
                return true;
            }
        }

        target = default;
        return false;
    }

    /// <summary>
    /// Attempts to retrieve the <see cref="IObjectId"/> from the specified source object.
    /// </summary>
    /// <param name="source">The object from which to retrieve the identifier.</param>
    /// <param name="target">The retrieved <see cref="IObjectId"/> if successful; otherwise,
    /// <see langword="null"/>.</param>
    /// <returns>
    /// <see langword="true"/> if the identifier is successfully retrieved; otherwise,
    /// <see langword="false"/>.
    /// </returns>
    public bool TryConvertGetId(object source, out IObjectId? target)
    {
        if (source is IGH_AutocadReferenceObject ghAutocadReferenceObject)
        {
            target = ghAutocadReferenceObject.ObjectValue.Id;
            return true;
        }

        if (source is IDbObject autocadReferenceObject)
        {
            target = autocadReferenceObject.Id;
            return true;
        }

        target = null;
        return false;
    }

    /// <summary>
    /// A converter which converts AutoCAD entities to their corresponding Grasshopper Goo types.
    /// </summary>
    public IGH_GeometricGoo? ToGeometricGoo(IEntity autocadEntity)
    {
        switch (autocadEntity.Unwrap())
        {
            case AutocadCurve curve:
                return new GH_AutocadCurve(curve);
            case AutocadMesh mesh:
                return new GH_AutocadMesh(mesh);
            case AutocadPoint point:
                return new GH_AutocadPoint(point);
            case AutocadSolid solid:
                return new GH_AutocadSolid(solid);

            default: return null;
        }
    }
}