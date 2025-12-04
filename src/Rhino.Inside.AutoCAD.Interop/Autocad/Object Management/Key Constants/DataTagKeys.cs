using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A constants class which defines the keys for <see cref="IDataTag"/>s.
/// </summary>
public class DataTagKeys
{
    #region Application Keys
    /// <summary>
    /// The Registered Application name key. The name is the identifier for the application
    /// for extended data and should not be changed.
    /// </summary>
    public const GroupCodeValue ApplicationNameKey = GroupCodeValue._1001;
    #endregion

    #region IDocument Keys
    /// <summary>
    /// The <see cref="IAutocadDocument.Id"/> key for extended data.
    /// </summary>
    public const GroupCodeValue DocumentIdKey = GroupCodeValue._1000;
    #endregion

    #region IPanelTypeEdge Keys
    /// <summary>
    /// <see cref="IPanelTypeEdge"/> <see cref="IDataTag"/> key for storing its
    /// <see cref="IEdgeCode.TsCode"/> in the underlying Curve form the
    /// <see cref="IBlockTableRecord"/>.  
    /// </summary>
    public const GroupCodeValue CurveTsCodeKey = GroupCodeValue._470;

    /// <summary>
    /// <see cref="IPanelTypeEdge"/> <see cref="IDataTag"/> key for storing its
    /// <see cref="PanelEdgeCutType"/> in the underlying Curve form the
    /// <see cref="IBlockTableRecord"/>.  
    /// </summary>
    public const GroupCodeValue CurveCutTypeKey = GroupCodeValue._92;

    /// <summary>
    /// <see cref="IPanelTypeEdge"/> <see cref="IDataTag"/> key for storing its
    /// <see cref="IPanelTypeEdge.SpringArray"/> locations in the underlying curve
    /// from the <see cref="IBlockTableRecord"/>. As there are typically more than
    /// one spring per edge (2 ~ 3 per edge) multiple <see cref="IDataTag"/>s are
    /// added to the curve. 
    /// </summary>
    public const GroupCodeValue SpringLocationKey = GroupCodeValue._11;

    /// <summary>
    /// The <see cref="ISpring.Direction"/> key. The direction is converted to a
    /// <see cref="IPoint3d"/> before being stored in extensible storage.
    /// </summary>
    public const GroupCodeValue SpringDirectionKey = GroupCodeValue._14;

    /// <summary>
    /// The <see cref="IPanelTypeEdgeLoop"/>'s <see cref="IPanelProfileExtents2d"/> Width key.
    /// </summary>
    public const GroupCodeValue PerforationExtentsWidthKey = GroupCodeValue._40;

    /// <summary>
    /// The <see cref="IPanelTypeEdgeLoop"/>'s <see cref="IPanelProfileExtents2d"/> Height key.
    /// </summary>
    public const GroupCodeValue PerforationExtentsHeightKey = GroupCodeValue._41;

    /// <summary>
    /// The <see cref="IPanelTypeEdgeLoop"/>'s <see cref="IPanelProfileExtents2d"/> Origin key.
    /// </summary>
    public const GroupCodeValue PerforationExtentsOriginKey = GroupCodeValue._13;

    /// <summary>
    /// The <see cref="ISpringDefinition.Id"/> key which can be used to obtain
    /// the definition from <see cref="IEdgeCodeDefinitionCache"/> when
    /// reconstructing the <see cref="ISpringArray2d"/>.
    /// </summary>
    public const GroupCodeValue SpringTypeIdKey = GroupCodeValue._90;

    /// <summary>
    /// The <see cref="IGrainDirection.GrainType"/> key which can be used
    /// to reconstruct a <see cref="IGrainDirection"/>.
    /// </summary>
    public const GroupCodeValue PanelGrainTypeCodeKey = GroupCodeValue._91;

    /// <summary>
    /// The <see cref="IGrainDirection"/> rotation key which can be used
    /// to reconstruct a <see cref="IGrainDirection"/>. This is to differentiate
    /// the <see cref="BiGrainDirection"/> which can rotate and the
    /// <see cref="CathedralGrainDirection"/> which cannot.
    /// </summary>
    public const GroupCodeValue PanelGrainRotationCodeKey = GroupCodeValue._291;

    /// <summary>
    /// An int key for storing the <see cref="IPerforationBoundary"/>'s Curve's
    /// <see cref="IObjectId"/> as an integer.
    /// /// </summary>
    public const GroupCodeValue PerforationCurveIdKey = GroupCodeValue._330;

    /// <summary>
    /// The key for <see cref="IPositionNumber"/> CAD text Id as integer.
    /// </summary>
    public const GroupCodeValue PositionNumberTagCodeKey = GroupCodeValue._93;

    #endregion

    #region IPanelType Keys
    /// <summary>
    /// <see cref="IPanelType"/> <see cref="IDataTag"/> key for storing its
    /// <see cref="IPanelDefinition.Shape"/> in the underlying
    /// <see cref="IBlockTableRecord"/>. 
    /// </summary>
    public const GroupCodeValue PanelShapeKey = GroupCodeValue._1;

    /// <summary>
    /// <see cref="IPanelType"/> <see cref="IDataTag"/> key for storing its
    /// <see cref="IPanelTypeIdentity.SpecDesignationId"/> in the underlying
    /// <see cref="IBlockTableRecord"/>. 
    /// </summary>
    public const GroupCodeValue PanelSpecIdKey = GroupCodeValue._2;

    /// <summary>
    /// <see cref="IPanelType"/> <see cref="IDataTag"/> key for storing its
    /// <see cref="IPanelTypeIdentity.PositionNumber"/> in the underlying
    /// <see cref="IBlockTableRecord"/>. 
    /// </summary>
    public const GroupCodeValue PositionNumberKey = GroupCodeValue._90;

    #endregion

    #region Sheet Template Name Record

    /// <summary>
    /// The <see cref="IDataTag"/> key used to store the <see cref=
    /// "ISheetTemplateDefinition.Name"/> in the <see cref=
    /// "TagRecordKeys.SheetTemplateDefinition"/> <see cref="IDataTagRecord"/>
    /// of the <see cref="IDataTagDatabaseManager.GetProjectWideDatabase"/>.
    /// </summary>
    public const GroupCodeValue SheetDefinitionNameKey = GroupCodeValue._1;

    #endregion

    #region Model Space Record / Model Space Internal Ids Record / Model Content Record

    /// <summary>
    /// The <see cref="IDataTag"/> key used to store the <see cref=
    /// "IModelSpaceIdentity.InternalId"/> in the <see cref=
    /// "TagRecordKeys.ModelSpaceInternalIds"/> and <see cref=
    /// "ICommitIdentity.ModelContentInternalId"/> <see cref="IDataTagRecord"/>s
    /// of the <see cref="IDataTagDatabaseManager.GetProjectWideDatabase"/>.
    /// </summary>
    public const GroupCodeValue ModelSpaceInternalId = GroupCodeValue._1;

    #endregion

    #region Model Space Record

    /// <summary>
    /// The <see cref="IDataTag"/> key used to store the <see cref=
    /// "IModelSpace.Type"/> of the created <see cref="IModelSpace{TContentType}"/>.
    /// </summary>
    public const GroupCodeValue ModelSpaceType = GroupCodeValue._2;

    /// <summary>
    /// The <see cref="IDataTag"/> key for storing a <see cref="IGridDimensions.Origin"/>
    /// of the <see cref= "IGrid"/>.
    /// </summary>
    public const GroupCodeValue GridOrigin = GroupCodeValue._10;

    /// <summary>
    /// The <see cref="IDataTag"/> key for storing a <see cref="IGridDimensions.Width"/>
    /// of the <see cref= "IGrid"/>.
    /// </summary>
    public const GroupCodeValue GridWidth = GroupCodeValue._40;

    /// <summary>
    /// The <see cref="IDataTag"/> key for storing a <see cref="IGridDimensions.Height"/>
    /// of the <see cref= "IGrid"/>.
    /// </summary>
    public const GroupCodeValue GridHeight = GroupCodeValue._41;

    /// <summary>
    /// The <see cref="IDataTag"/> key for storing a <see cref="IGridDimensions.TotalRows"/>
    /// of the <see cref= "IGrid"/>.
    /// </summary>
    public const GroupCodeValue GridTotalRows = GroupCodeValue._90;

    /// <summary>
    /// The <see cref="IDataTag"/> key for storing a <see cref="IGridDimensions.TotalColumns"/>
    /// of the <see cref= "IGrid"/>.
    /// </summary>
    public const GroupCodeValue GridTotalColumns = GroupCodeValue._91;

    #endregion

    #region Sheet Content Internal Ids Record

    /// <summary>
    /// The <see cref="IDataTag"/> key used to store the <see cref=
    /// "ICommitIdentity.SheetContentInternalId"/> in the <see cref=
    /// "TagRecordKeys.SheetContentInternalIds"/> <see cref="IDataTagRecord"/>
    /// of the <see cref="IDataTagDatabaseManager.GetProjectWideDatabase"/>.
    /// </summary>
    public const GroupCodeValue SheetContentInternalId = GroupCodeValue._1;

    #endregion

    #region Model Content Internal Ids Record

    /// <summary>
    /// The <see cref="IDataTag"/> key used to store the <see cref=
    /// "ICommitIdentity.ModelContentInternalId"/> in the <see cref=
    /// "TagRecordKeys.ModelContentInternalIds"/> <see cref="IDataTagRecord"/>
    /// of the <see cref="IDataTagDatabaseManager.GetProjectWideDatabase"/>.
    /// </summary>
    public const GroupCodeValue ModelContentInternalId = GroupCodeValue._1;

    #endregion

    #region Sheet Internal Ids Record

    /// <summary>
    /// The <see cref="IDataTag"/> key used to store the <see cref=
    /// "ISheetNameIdentity.InternalId"/> in the <see cref=
    /// "TagRecordKeys.SheetInternalIds"/> <see cref="IDataTagRecord"/>
    /// of the <see cref="IDataTagDatabaseManager.GetProjectWideDatabase"/>.
    /// </summary>
    public const GroupCodeValue SheetInternalId = GroupCodeValue._1;

    #endregion

    #region Commit Internal Ids Record

    /// <summary>
    /// The <see cref="IDataTag"/> key used to store the <see cref=
    /// "ICommitIdentity.InternalId"/> in the <see cref=
    /// "TagRecordKeys.CommitInternalIds"/> <see cref="IDataTagRecord"/>
    /// of the <see cref="IDataTagDatabaseManager.GetProjectWideDatabase"/>.
    /// </summary>
    public const GroupCodeValue CommitInternalId = GroupCodeValue._1;

    #endregion

    #region Sheet Record / Sheet Content Record / Commit Locator Record 

    /// <summary>
    /// The <see cref="IDataTag"/> key used to store <see cref= "ISheetNameIdentity.Name"/>.
    /// </summary>
    public const GroupCodeValue SheetName = GroupCodeValue._1;

    /// <summary>
    /// The <see cref="IDataTag"/> key used to store <see cref= "ISheetNameIdentity.Title"/>.
    /// </summary>
    public const GroupCodeValue SheetTitle = GroupCodeValue._2;

    #endregion

    #region Commit Locator Record 

    /// <summary>
    /// The <see cref="IDataTag"/> key used to store <see cref= "ICommitLocator.IsCommitted"/>
    /// </summary>
    public const GroupCodeValue IsCommitted = GroupCodeValue._290;

    /// <summary>
    /// The <see cref="IDataTag"/> key used to store <see cref= "ICommitLocator.IsOrphaned"/>
    /// </summary>
    public const GroupCodeValue IsOrphaned = GroupCodeValue._291;

    #endregion

    #region Sheet Content Record

    /// <summary>
    /// The <see cref="IDataTag"/> key used to store the <see cref=
    /// "ISheet.Type"/> of the created <see cref="ISheet"/> within the
    /// active <see cref= "IAutocadDocument"/>.
    /// </summary>
    public const GroupCodeValue SheetType = GroupCodeValue._3;

    /// <summary>
    /// The <see cref="IDataTag"/> key used to store the <see cref= "ISheetContentName.Name"/>
    /// of the created <see cref="ISheetContent"/> within the active <see cref= "IAutocadDocument"/>.
    /// </summary>
    public const GroupCodeValue SheetContentName = GroupCodeValue._4;

    /// <summary>
    /// The <see cref="IDataTag"/> key used to store the <see cref= "ISheetContentName.Title"/>
    /// of the created <see cref="ISheetContent"/> within the active <see cref= "IAutocadDocument"/>.
    /// </summary>
    public const GroupCodeValue SheetContentTitle = GroupCodeValue._5;

    /// <summary>
    /// The <see cref="IDataTag"/> key used to store the <see cref= "ISheetContent.Type"/>
    /// of the created <see cref="ISheetContent"/> within the active <see cref= "IAutocadDocument"/>.
    /// </summary>
    public const GroupCodeValue SheetContentType = GroupCodeValue._410;

    /// <summary>
    /// The <see cref="IDataTag"/> key for storing a <see cref="IModelSpaceCrop.Min"/>
    /// of the <see cref= "ISheetContent"/> that's related to, within the active
    /// <see cref= "IAutocadDocument"/>.
    /// </summary>
    public const GroupCodeValue ModelSpaceCropMin = GroupCodeValue._12;

    /// <summary>
    /// The <see cref="IDataTag"/> key for storing a <see cref="IModelSpaceCrop.Max"/>
    /// of the <see cref= "ISheetContent"/> that's related to, within the active
    /// <see cref= "IAutocadDocument"/>.
    /// </summary>
    public const GroupCodeValue ModelSpaceCropMax = GroupCodeValue._13;

    #endregion

    #region Sheet Content & Model Content Records

    /// <summary>
    /// The <see cref="IDataTag"/> key for storing a <see cref="IGridRange.TopLeft"/>
    /// of the <see cref= "IGridRange"/>.
    /// </summary>
    public const GroupCodeValue GridRangeTopLeft = GroupCodeValue._10;

    /// <summary>
    /// The <see cref="IDataTag"/> key for storing a <see cref="IGridRange.BottomRight"/>
    /// of the <see cref= "IGridRange"/>.
    /// </summary>
    public const GroupCodeValue GridRangeBottomRight = GroupCodeValue._11;

    #endregion

    #region Ceiling Assembly Keys

    /// <summary>
    /// The <see cref="IDataTag"/> key used to store a <see cref=
    /// "ICeiling3dGroupIdentity.InternalId"/>.
    /// </summary>
    public const GroupCodeValue Ceiling3dGroupInternalId = GroupCodeValue._1;

    /// <summary>
    /// The <see cref="IDataTag"/> key used to store a <see cref=
    /// "ICeiling3dGroupIdentity.Name"/>.
    /// </summary>
    public const GroupCodeValue Ceiling3dGroupName = GroupCodeValue._2;

    /// <summary>
    /// The <see cref="IDataTag"/> key used to store <see cref=
    /// "ICeilingInstance.Id"/>s within the <see cref="ICeiling3dGroup"/>.
    /// </summary>
    public const GroupCodeValue CeilingInstanceIds = GroupCodeValue._3;

    /// <summary>
    /// The <see cref="IDataTag"/> key used to store the <see cref=
    /// "ICeiling3dGroup.IsSelected"/>.
    /// </summary>
    public const GroupCodeValue CeilingAssemblyIsSelected = GroupCodeValue._290;

    #endregion

    #region IProjectInputs Keys
    /// <summary>
    /// <see cref="IGridSystemType"/> <see cref="IDataTag"/> key for storing the selected
    /// <see cref="IGridSystemType.Type"/> in the
    /// <see cref="IDataTagDatabaseManager.GetProjectWideDatabase"/>.
    /// </summary>
    public const GroupCodeValue GridSystemTypeNameKey = GroupCodeValue._1;

    /// <summary>
    /// <see cref="IPerforationDefinition"/> <see cref="IDataTag"/> key for storing the
    /// selected <see cref="IPerforationDefinition.Name"/> in the
    /// <see cref="IDataTagDatabaseManager.GetProjectWideDatabase"/>.
    /// </summary>
    public const GroupCodeValue PerforationDefinitionNameKey = GroupCodeValue._2;

    /// <summary>
    /// <see cref="IFleeceDefinition"/> <see cref="IDataTag"/> key for storing the
    /// selected <see cref="IFleeceDefinition.Name"/> in the
    /// <see cref="IDataTagDatabaseManager.GetProjectWideDatabase"/>.
    /// </summary>
    public const GroupCodeValue FleeceDefinitionNameKey = GroupCodeValue._3;

    /// <summary>
    /// <see cref="IFinishColorInput"/> <see cref="IDataTag"/> key for storing the
    /// selected <see cref="IFinishColorInput.DisplayName"/> in the
    /// <see cref="IDataTagDatabaseManager.GetProjectWideDatabase"/>.
    /// </summary>
    public const GroupCodeValue FinishColorDisplayNameKey = GroupCodeValue._4;

    /// <summary>
    /// <see cref="IFinishColorInput"/> <see cref="IDataTag"/> key for storing the
    /// selected <see cref="IFinishColorInput.UseCustom"/> value in the
    /// <see cref="IDataTagDatabaseManager.GetProjectWideDatabase"/>.
    /// </summary>
    public const GroupCodeValue FinishColorUseCustomNameKey = GroupCodeValue._290;

    /// <summary>
    /// <see cref="IMaterialType"/> <see cref="IDataTag"/> key for storing the
    /// selected <see cref="IMaterialType.Type"/> enum in the
    /// <see cref="IDataTagDatabaseManager.GetProjectWideDatabase"/>.
    /// </summary>
    public const GroupCodeValue MaterialTypeNameKey = GroupCodeValue._470;

    /// <summary>
    /// <see cref="IMaterialType"/> <see cref="IDataTag"/> key for storing the
    /// selected <see cref="IMaterialType.Thickness"/> value in the
    /// <see cref="IDataTagDatabaseManager.GetProjectWideDatabase"/>.
    /// </summary>
    public const GroupCodeValue MaterialTypeThicknessKey = GroupCodeValue._460;
    #endregion
}
