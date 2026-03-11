using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A value list component that provides a dropdown of DxfCode values.
/// </summary>
public class DxfCodeValueList : GH_ValueList
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("9f6a4c3b-5d2e-8a1f-7b9c-3e4d6f5a2b1c");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.tertiary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.DxfCodeValueList;

    /// <summary>
    /// Initializes a new instance of the <see cref="DxfCodeValueList"/> class.
    /// </summary>
    public DxfCodeValueList()
    {
        this.Name = "DxfCode";
        this.NickName = "DxfCode";
        this.Description = "Select a DxfCode value";
        this.Category = "AutoCAD";
        this.SubCategory = "Filter";

        this.ListItems.Clear();

        // Common DxfCodes
        this.ListItems.Add(new GH_ValueListItem("Start", "0"));
        this.ListItems.Add(new GH_ValueListItem("Text", "1"));
        this.ListItems.Add(new GH_ValueListItem("AttributeTag", "2"));
        this.ListItems.Add(new GH_ValueListItem("BlockName", "2"));
        this.ListItems.Add(new GH_ValueListItem("SymbolTableName", "2"));
        this.ListItems.Add(new GH_ValueListItem("SymbolTableRecordName", "2"));
        this.ListItems.Add(new GH_ValueListItem("AttributePrompt", "3"));
        this.ListItems.Add(new GH_ValueListItem("Description", "3"));
        this.ListItems.Add(new GH_ValueListItem("TextFontFile", "3"));
        this.ListItems.Add(new GH_ValueListItem("Handle", "5"));
        this.ListItems.Add(new GH_ValueListItem("LinetypeName", "6"));
        this.ListItems.Add(new GH_ValueListItem("TextStyleName", "7"));
        this.ListItems.Add(new GH_ValueListItem("LayerName", "8"));
        this.ListItems.Add(new GH_ValueListItem("XCoordinate", "10"));
        this.ListItems.Add(new GH_ValueListItem("YCoordinate", "20"));
        this.ListItems.Add(new GH_ValueListItem("ZCoordinate", "30"));
        this.ListItems.Add(new GH_ValueListItem("Elevation", "38"));
        this.ListItems.Add(new GH_ValueListItem("Thickness", "39"));
        this.ListItems.Add(new GH_ValueListItem("Real", "40"));
        this.ListItems.Add(new GH_ValueListItem("TxtSize", "40"));
        this.ListItems.Add(new GH_ValueListItem("ViewportHeight", "40"));
        this.ListItems.Add(new GH_ValueListItem("TxtStyleXScale", "41"));
        this.ListItems.Add(new GH_ValueListItem("ViewWidth", "41"));
        this.ListItems.Add(new GH_ValueListItem("TxtStylePSize", "42"));
        this.ListItems.Add(new GH_ValueListItem("ViewLensLength", "42"));
        this.ListItems.Add(new GH_ValueListItem("ViewFrontClip", "43"));
        this.ListItems.Add(new GH_ValueListItem("ShapeXOffset", "44"));
        this.ListItems.Add(new GH_ValueListItem("ViewBackClip", "44"));
        this.ListItems.Add(new GH_ValueListItem("ShapeYOffset", "45"));
        this.ListItems.Add(new GH_ValueListItem("ViewHeight", "45"));
        this.ListItems.Add(new GH_ValueListItem("ShapeScale", "46"));
        this.ListItems.Add(new GH_ValueListItem("PixelScale", "47"));
        this.ListItems.Add(new GH_ValueListItem("LinetypeScale", "48"));
        this.ListItems.Add(new GH_ValueListItem("DashLength", "49"));
        this.ListItems.Add(new GH_ValueListItem("Angle", "50"));
        this.ListItems.Add(new GH_ValueListItem("ViewportSnapAngle", "50"));
        this.ListItems.Add(new GH_ValueListItem("ViewportTwist", "51"));
        this.ListItems.Add(new GH_ValueListItem("Visibility", "60"));
        this.ListItems.Add(new GH_ValueListItem("LayerLinetype", "61"));
        this.ListItems.Add(new GH_ValueListItem("Color", "62"));
        this.ListItems.Add(new GH_ValueListItem("HasSubentities", "66"));
        this.ListItems.Add(new GH_ValueListItem("ViewportVisibility", "67"));
        this.ListItems.Add(new GH_ValueListItem("ViewportActive", "68"));
        this.ListItems.Add(new GH_ValueListItem("ViewportNumber", "69"));
        this.ListItems.Add(new GH_ValueListItem("Int16", "70"));
        this.ListItems.Add(new GH_ValueListItem("RegAppFlags", "71"));
        this.ListItems.Add(new GH_ValueListItem("TxtStyleFlags", "71"));
        this.ListItems.Add(new GH_ValueListItem("ViewMode", "71"));
        this.ListItems.Add(new GH_ValueListItem("CircleSides", "72"));
        this.ListItems.Add(new GH_ValueListItem("LinetypeAlign", "72"));
        this.ListItems.Add(new GH_ValueListItem("LinetypePdc", "73"));
        this.ListItems.Add(new GH_ValueListItem("ViewportZoom", "73"));
        this.ListItems.Add(new GH_ValueListItem("ViewportIcon", "74"));
        this.ListItems.Add(new GH_ValueListItem("ViewportSnap", "75"));
        this.ListItems.Add(new GH_ValueListItem("ViewportGrid", "76"));
        this.ListItems.Add(new GH_ValueListItem("ViewportSnapStyle", "77"));
        this.ListItems.Add(new GH_ValueListItem("ViewportSnapPair", "78"));
        this.ListItems.Add(new GH_ValueListItem("Int32", "90"));
        this.ListItems.Add(new GH_ValueListItem("Subclass", "100"));
        this.ListItems.Add(new GH_ValueListItem("EmbeddedObjectStart", "101"));
        this.ListItems.Add(new GH_ValueListItem("ControlString", "102"));
        this.ListItems.Add(new GH_ValueListItem("DimVarHandle", "105"));
        this.ListItems.Add(new GH_ValueListItem("UcsOrg", "110"));
        this.ListItems.Add(new GH_ValueListItem("UcsOrientationX", "111"));
        this.ListItems.Add(new GH_ValueListItem("UcsOrientationY", "112"));
        this.ListItems.Add(new GH_ValueListItem("XReal", "140"));
        this.ListItems.Add(new GH_ValueListItem("ViewBrightness", "141"));
        this.ListItems.Add(new GH_ValueListItem("ViewContrast", "142"));
        this.ListItems.Add(new GH_ValueListItem("Int64", "160"));
        this.ListItems.Add(new GH_ValueListItem("XInt16", "170"));
        this.ListItems.Add(new GH_ValueListItem("NormalX", "210"));
        this.ListItems.Add(new GH_ValueListItem("NormalY", "220"));
        this.ListItems.Add(new GH_ValueListItem("NormalZ", "230"));
        this.ListItems.Add(new GH_ValueListItem("XXInt16", "270"));
        this.ListItems.Add(new GH_ValueListItem("Int8", "280"));
        this.ListItems.Add(new GH_ValueListItem("RenderMode", "281"));
        this.ListItems.Add(new GH_ValueListItem("Bool", "290"));
        this.ListItems.Add(new GH_ValueListItem("XTextString", "300"));
        this.ListItems.Add(new GH_ValueListItem("BinaryChunk", "310"));
        this.ListItems.Add(new GH_ValueListItem("ArbitraryHandle", "320"));
        this.ListItems.Add(new GH_ValueListItem("SoftPointerId", "330"));
        this.ListItems.Add(new GH_ValueListItem("HardPointerId", "340"));
        this.ListItems.Add(new GH_ValueListItem("SoftOwnershipId", "350"));
        this.ListItems.Add(new GH_ValueListItem("HardOwnershipId", "360"));
        this.ListItems.Add(new GH_ValueListItem("LineWeight", "370"));
        this.ListItems.Add(new GH_ValueListItem("PlotStyleNameType", "380"));
        this.ListItems.Add(new GH_ValueListItem("PlotStyleNameId", "390"));
        this.ListItems.Add(new GH_ValueListItem("ExtendedInt16", "400"));
        this.ListItems.Add(new GH_ValueListItem("LayoutName", "410"));
        this.ListItems.Add(new GH_ValueListItem("ColorRgb", "420"));
        this.ListItems.Add(new GH_ValueListItem("ColorName", "430"));
        this.ListItems.Add(new GH_ValueListItem("Alpha", "440"));
        this.ListItems.Add(new GH_ValueListItem("GradientObjType", "450"));
        this.ListItems.Add(new GH_ValueListItem("GradientPatType", "451"));
        this.ListItems.Add(new GH_ValueListItem("GradientTintType", "452"));
        this.ListItems.Add(new GH_ValueListItem("GradientColCount", "453"));
        this.ListItems.Add(new GH_ValueListItem("GradientAngle", "460"));
        this.ListItems.Add(new GH_ValueListItem("GradientShift", "461"));
        this.ListItems.Add(new GH_ValueListItem("GradientTintVal", "462"));
        this.ListItems.Add(new GH_ValueListItem("GradientColVal", "463"));
        this.ListItems.Add(new GH_ValueListItem("GradientName", "470"));
        this.ListItems.Add(new GH_ValueListItem("Comment", "999"));
        this.ListItems.Add(new GH_ValueListItem("ExtendedDataAsciiString", "1000"));
        this.ListItems.Add(new GH_ValueListItem("ExtendedDataRegAppName", "1001"));
        this.ListItems.Add(new GH_ValueListItem("ExtendedDataControlString", "1002"));
        this.ListItems.Add(new GH_ValueListItem("ExtendedDataLayerName", "1003"));
        this.ListItems.Add(new GH_ValueListItem("ExtendedDataBinaryChunk", "1004"));
        this.ListItems.Add(new GH_ValueListItem("ExtendedDataHandle", "1005"));
        this.ListItems.Add(new GH_ValueListItem("ExtendedDataXCoordinate", "1010"));
        this.ListItems.Add(new GH_ValueListItem("ExtendedDataWorldXCoordinate", "1011"));
        this.ListItems.Add(new GH_ValueListItem("ExtendedDataWorldXDisp", "1012"));
        this.ListItems.Add(new GH_ValueListItem("ExtendedDataWorldXDir", "1013"));
        this.ListItems.Add(new GH_ValueListItem("ExtendedDataYCoordinate", "1020"));
        this.ListItems.Add(new GH_ValueListItem("ExtendedDataWorldYCoordinate", "1021"));
        this.ListItems.Add(new GH_ValueListItem("ExtendedDataWorldYDisp", "1022"));
        this.ListItems.Add(new GH_ValueListItem("ExtendedDataWorldYDir", "1023"));
        this.ListItems.Add(new GH_ValueListItem("ExtendedDataZCoordinate", "1030"));
        this.ListItems.Add(new GH_ValueListItem("ExtendedDataWorldZCoordinate", "1031"));
        this.ListItems.Add(new GH_ValueListItem("ExtendedDataWorldZDisp", "1032"));
        this.ListItems.Add(new GH_ValueListItem("ExtendedDataWorldZDir", "1033"));
        this.ListItems.Add(new GH_ValueListItem("ExtendedDataReal", "1040"));
        this.ListItems.Add(new GH_ValueListItem("ExtendedDataDist", "1041"));
        this.ListItems.Add(new GH_ValueListItem("ExtendedDataScale", "1042"));
        this.ListItems.Add(new GH_ValueListItem("ExtendedDataInteger16", "1070"));
        this.ListItems.Add(new GH_ValueListItem("ExtendedDataInteger32", "1071"));

        // Special/negative codes
        this.ListItems.Add(new GH_ValueListItem("Invalid", "-9999"));
        this.ListItems.Add(new GH_ValueListItem("XDictionary", "-6"));
        this.ListItems.Add(new GH_ValueListItem("PReactors", "-5"));
        this.ListItems.Add(new GH_ValueListItem("Operator", "-4"));
        this.ListItems.Add(new GH_ValueListItem("XDataStart", "-3"));
        this.ListItems.Add(new GH_ValueListItem("FirstEntityId", "-2"));
        this.ListItems.Add(new GH_ValueListItem("HeaderId", "-2"));
        this.ListItems.Add(new GH_ValueListItem("End", "-1"));

        this.SelectItem(0);
    }
}
