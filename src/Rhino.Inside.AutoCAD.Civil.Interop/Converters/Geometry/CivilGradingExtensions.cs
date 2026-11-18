using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using CadCurve = Autodesk.AutoCAD.DatabaseServices.Curve;
using CadEntity = Autodesk.AutoCAD.DatabaseServices.Entity;
using CadDBObject = Autodesk.AutoCAD.DatabaseServices.DBObject;
using RhinoCurve = Rhino.Geometry.Curve;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Provides extension methods for converting Civil 3D Gradings to Rhino geometry.
/// </summary>
/// <remarks>
/// The Civil 3D .NET API exposes no grading geometry, so the grading is exploded into its
/// displayed AutoCAD entities and the curves among them are converted. The result depends
/// on the grading style: components hidden by the style are not returned.
/// </remarks>
public static class CivilGradingExtensions
{
    private const int _maxBlockDepth = 4;

    /// <summary>
    /// Converts a Civil 3D Grading to the Rhino curves of its displayed geometry,
    /// applying unit conversion.
    /// </summary>
    /// <param name="grading">The grading to convert.</param>
    /// <returns>The converted curves; empty if the grading could not be exploded.</returns>
    public static List<RhinoCurve> ToRhinoCurves(this Grading grading)
    {
        var curves = new List<RhinoCurve>();

        try
        {
            ExplodeToCurves(grading, curves, 0);
        }
        catch
        {
            // Return whatever was collected
        }

        return curves;
    }

    /// <summary>
    /// Explodes an entity and converts the resulting curves. Exploded block references are
    /// exploded again, up to a fixed depth. Exploded objects are not database resident, so
    /// they are disposed here.
    /// </summary>
    private static void ExplodeToCurves(CadEntity entity, List<RhinoCurve> curves, int depth)
    {
        using var exploded = new DBObjectCollection();

        entity.Explode(exploded);

        foreach (CadDBObject dbObject in exploded)
        {
            try
            {
                switch (dbObject)
                {
                    case CadCurve curve:
                        var rhinoCurve = curve.ToRhinoCurve();
                        if (rhinoCurve != null)
                            curves.Add(rhinoCurve);
                        break;

                    case BlockReference blockReference when depth < _maxBlockDepth:
                        ExplodeToCurves(blockReference, curves, depth + 1);
                        break;
                }
            }
            catch
            {
                // Skip entities that fail to convert
            }
            finally
            {
                dbObject.Dispose();
            }
        }
    }
}
