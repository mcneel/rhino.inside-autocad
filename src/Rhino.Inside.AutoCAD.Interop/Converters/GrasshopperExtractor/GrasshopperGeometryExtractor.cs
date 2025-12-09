using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IGrasshopperGeometryExtractor"/>
public class GrasshopperGeometryExtractor : IGrasshopperGeometryExtractor
{
    /// <summary>
    /// Adds the curves and meshes from a Brep to the preview data.
    /// </summary>
    private void AddBrep(IGrasshopperPreviewData data, Brep brep)
    {
        if (brep is null) return;

        var curves = brep.Curves3D;
        data.Wires.AddRange(curves);

        var meshes = Mesh.CreateFromBrep(brep, MeshingParameters.Default);
        data.Meshes.AddRange(meshes);
    }

    /// <summary>
    /// Extracts geometry data from a Grasshopper parameter and adds it to the preview data.
    /// </summary>
    /// <param name="param">The Grasshopper parameter to extract geometry from.</param>
    /// <param name="data">The container for the extracted preview data.</param>
    private void ExtractFromParameter(IGH_Param param, IGrasshopperPreviewData data)
    {

        foreach (var goo in param.VolatileData.AllData(true))
        {
            if (goo is IGH_PreviewData == false) continue;

            if (goo is IGH_AutocadGeometryPreview nativeGoo)
            {
                nativeGoo.DrawAutocadPreview(data);
                continue;
            }

            if (goo is GH_Point point)
            {
                data.Points.Add(point.Value);
                continue;
            }

            if (goo is GH_Line line)
            {
                data.Wires.Add(new LineCurve(line.Value));
                continue;
            }

            if (goo is GH_Arc arc)
            {
                data.Wires.Add(new ArcCurve(arc.Value));
                continue;
            }

            if (goo is GH_Circle circle)
            {
                data.Wires.Add(new ArcCurve(circle.Value));
                continue;
            }

            if (goo is GH_Rectangle rectangle3d)
            {
                data.Wires.Add(rectangle3d.Value.ToNurbsCurve());
                continue;
            }

            if (goo is GH_Curve curve)
            {
                data.Wires.Add(curve.Value);
            }

            if (goo is GH_Brep gh_brep)
            {
                this.AddBrep(data, gh_brep.Value);
            }

            if (goo is GH_Box box)
            {
                var brep = box.Value.ToBrep();
                this.AddBrep(data, brep);

            }

            if (goo is GH_Surface surface)
            {
                var brep = surface.Value;
                this.AddBrep(data, brep);

            }

            if (goo is GH_SubD subD)
            {
                var brep = subD.Value.ToBrep();
                this.AddBrep(data, brep);
            }

            if (goo is GH_Mesh mesh)
            {
                var polylines = mesh.Value.GetNakedEdges();
                foreach (var polyline in polylines)
                {
                    data.Wires.Add(new PolylineCurve(polyline));
                }

                data.Meshes.Add(mesh.Value);
            }
        }
    }

    /// <inheritdoc />
    public IGrasshopperPreviewData ExtractPreviewGeometry(IGH_DocumentObject ghDocumentObject)
    {
        var previewGeometryData = new GrasshopperPreviewData();

        if (ghDocumentObject is not IGH_PreviewObject { Hidden: false })
            return previewGeometryData;

        if (ghDocumentObject is IGH_Component component)
        {
            foreach (var outputParam in component.Params.Output)
            {
                this.ExtractFromParameter(outputParam, previewGeometryData);
            }

            return previewGeometryData;
        }

        if (ghDocumentObject is IGH_Param param)
        {
            this.ExtractFromParameter(param, previewGeometryData);
        }

        return previewGeometryData;
    }
}