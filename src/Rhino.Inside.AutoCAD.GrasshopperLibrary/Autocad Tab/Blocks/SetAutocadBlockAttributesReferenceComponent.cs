using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that set properties from an AutoCAD dynamic block reference property.
/// </summary>
[ComponentVersion(introduced: "1.0.16")]
public class SetAutocadBlockAttributesReferenceComponent : RhinoInsideAutocad_ComponentBase
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("e4d554b7-4c7d-42f9-b3c0-9166228d6001");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.quinary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.SetAutocadBlockAttributesReferenceComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="SetAutocadBlockAttributesReferenceComponent"/> class.
    /// </summary>
    public SetAutocadBlockAttributesReferenceComponent()
        : base("Set Block Attributes", "AC-SetAttr",
            "Sets the properties of an AutoCAD Block Attribute Reference",
            "AutoCAD", "Blocks")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_BlockAttributeReference(GH_ParamAccess.item),
            "Attribute", "Attr", "A Dynamic Block Attribute Reference", GH_ParamAccess.item);

        pManager.AddTextParameter("Value", "Value", "The Text of the property",
            GH_ParamAccess.item);
        pManager[1].Optional = true;

        pManager.AddBooleanParameter("Is Multiline", "MText", "A boolean indicating if the property should use the Multiline (MText)", GH_ParamAccess.item);
        pManager[2].Optional = true;

    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_BlockAttributeReference(GH_ParamAccess.item),
            "Attribute", "Attr", "A Dynamic Block Attribute Reference", GH_ParamAccess.item);
        pManager.AddTextParameter("Tag", "T", "The tag of the attribute", GH_ParamAccess.item);
        pManager.AddTextParameter("Value", "V", "The value of the attribute", GH_ParamAccess.item);
        pManager.AddBooleanParameter("Is Multiline", "MText", "Whether the property is a Multiline (MText)", GH_ParamAccess.item);
        pManager.AddPointParameter("Location", "Loc", "The location of attribute reference, converted to Rhino's Units.", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        AttributeWrapper? attribute = null;

        if (!DA.GetData(0, ref attribute) || attribute is null)
            return;

        var value = attribute.Text;
        var isMText = attribute.IsMultiline;

        DA.GetData(1, ref value);
        DA.GetData(2, ref isMText);

        var activeDocument = Application.DocumentManager.MdiActiveDocument;

        using var documentLock = activeDocument.LockDocument();

        var database = activeDocument.Database;

        var transactionManager = database.TransactionManager;
        var transaction = transactionManager.StartTransaction();

        var cadAttributeReference = transactionManager.GetObject(attribute.Id.Unwrap(), OpenMode.ForWrite) as AttributeReference;

        cadAttributeReference.IsMTextAttribute = isMText;

        if (isMText)
        {
            var mText = cadAttributeReference.MTextAttribute.Clone() as MText;
            mText.Contents = value;
            cadAttributeReference.MTextAttribute = mText;

        }
        else
        {
            cadAttributeReference.TextString = value;
        }

        transaction.Commit();

        var newWrapper = new AttributeWrapper(cadAttributeReference);

        var goo = new GH_BlockAttributeReference(newWrapper);

        DA.SetData(0, goo);
        DA.SetData(1, newWrapper.Tag);
        DA.SetData(2, newWrapper.Text);
        DA.SetData(3, newWrapper.IsMultiline);
        DA.SetData(4, newWrapper.AlignmentPoint);
    }
}