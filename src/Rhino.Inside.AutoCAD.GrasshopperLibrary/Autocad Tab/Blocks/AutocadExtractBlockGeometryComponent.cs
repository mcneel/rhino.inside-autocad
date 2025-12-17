using Autodesk.AutoCAD.ApplicationServices;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.GrasshopperLibrary.Autocad_Tab.Base;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that extracts the geometry from an AutoCAD block.
/// </summary>
[ComponentVersion(introduced: "1.0.0")]
public class AutocadExtractBlockGeometryComponent : RhinoInsideAutocad_Component
{
    private readonly GooTypeRegistry _gooConverterRegister = GooTypeRegistry.Instance!;

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.secondary;

    /// <inheritdoc />
    public override Guid ComponentGuid => new("390c61af-4b81-475e-9907-598a42d95634");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.AutocadExtractBlockGeometryComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutocadExtractBlockGeometryComponent"/> class.
    /// </summary>
    public AutocadExtractBlockGeometryComponent()
        : base("Extract Block Geometry", "AC-ExtBlk",
            "Extracts the geometry from an AutoCAD Block Table Record or Block Reference",
            "AutoCAD", "Blocks")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddGenericParameter("Block", "Block",
            "The AutoCAD Block Table Record or Block Reference to extract geometry from", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddGeometryParameter("BlockObjects", "Objects",
             "The objects in the block", GH_ParamAccess.list);
    }

    /// <summary>
    /// Loads the geometry from a Block Table Record or Block Reference.
    /// </summary>
    private IEnumerable<IGH_GeometricGoo> LoadBlockObjects(Func<TransactionManagerWrapper, IEntityCollection> getObjectsFunc)
    {
        var activeDocument = Application.DocumentManager.MdiActiveDocument;

        using var documentLock = activeDocument.LockDocument();

        var database = activeDocument.Database;

        using var transactionManagerWrapper = new TransactionManagerWrapper(database);

        using var transaction = transactionManagerWrapper.Unwrap().StartTransaction();

        var objects = getObjectsFunc.Invoke(transactionManagerWrapper);

        transaction.Commit();

        var blockObject = new List<IGH_GeometricGoo>();

        foreach (var entityObject in objects)
        {
            var goo = _gooConverterRegister.CreateGoo(entityObject);

            blockObject.Add(goo);
        }

        return blockObject;
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        object? generic = null;

        if (!DA.GetData(0, ref generic)
            || generic is null) return;

        var gooObjects = new List<IGH_GeometricGoo>();

        switch (generic)
        {
            case GH_AutocadBlockReference gooBlockReference:
                gooObjects.AddRange(this.LoadBlockObjects(gooBlockReference.Value.GetObjects));
                break;
            case GH_AutocadBlockTableRecord gooBlockTableRecord:
                gooObjects.AddRange(this.LoadBlockObjects(gooBlockTableRecord.Value.GetObjects));
                break;
            case BlockReferenceWrapper blockReferenceWrapper:
                gooObjects.AddRange(this.LoadBlockObjects(blockReferenceWrapper.GetObjects));
                break;
            case BlockTableRecordWrapper blockTableRecordWrapper:
                gooObjects.AddRange(this.LoadBlockObjects(blockTableRecordWrapper.GetObjects));
                break;
            default:
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
                    "Input must be a Block Reference or Block Table Record");
                return;
        }

        DA.SetDataList(0, gooObjects.ToList());
    }
}

