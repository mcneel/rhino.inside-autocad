using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A constants class which defines the keys for <see cref="IObjectIdTagRecord"/>s.
/// </summary>
public class TagRecordKeys
{
    /// <summary>
    /// The key for the <see cref="IObjectIdTagRecord"/> containing <see cref=
    /// "IObjectIdTag"/>s relating to created <see cref="IEntity"/>s of <see 
    /// cref="ISheetContent"/>s in the <see cref="IObjectIdTagRecord"/>.
    /// </summary>
    public const string WhiteSpaceEntities = "WHITE_SPACE_ENTITIES";

    /// <summary>
    /// The key for the <see cref="IObjectIdTagRecord"/> containing <see cref=
    /// "IObjectIdTag"/>s relating to created <see cref="IEntity"/>s of <see 
    /// cref="IModelContent"/>s in the <see cref="IObjectIdTagRecord"/>.
    /// </summary>
    public const string ModelSpaceEntities = "MODEL_SPACE_ENTITIES";

    /// <summary>
    /// The key for the <see cref="IObjectIdTagRecord"/> containing <see cref=
    /// "IDataTag"/>s relating to <see cref="ISheetNameIdentity.InternalId"/>s 
    /// of the created <see cref="ISheet"/>s in the active <see cref="IAutocadDocument"/>.
    /// </summary>
    public const string SheetInternalIds = "SHEET_INTERNAL_IDS";

    /// <summary>
    /// The key for the <see cref="IDataTagRecord"/> containing <see cref=
    /// "IDataTag"/>s relating to the <see cref="IModelSpaceIdentity.InternalId"/>s
    /// in the <see cref="IDataTagDatabaseManager.GetProjectWideDatabase"/>.
    /// </summary>
    public const string ModelSpaceInternalIds = "MODEL_SPACE_INTERNAL_IDS";

    /// <summary>
    /// The key for the <see cref="IObjectIdTagRecord"/> containing <see cref=
    /// "IDataTag"/>s relating to the <see cref="ISheetContent.InternalId"/>
    /// in Project Wide Database of the active <see cref="IAutocadDocument"/>.
    /// </summary>
    public const string SheetContentInternalIds = "SHEET_CONTENT_INTERNAL_IDS";

    /// <summary>
    /// The key for the <see cref="IObjectIdTagRecord"/> containing <see cref=
    /// "IDataTag"/>s relating to the <see cref="IModelContent.InternalId"/>
    /// in Project Wide Database of the active <see cref="IAutocadDocument"/>.
    /// </summary>
    public const string ModelContentInternalIds = "MODEL_CONTENT_INTERNAL_IDS";

    /// <summary>
    /// The key for the <see cref="IObjectIdTagRecord"/> containing <see cref=
    /// "IDataTag"/>s relating to the <see cref="ICommitIdentity.InternalId"/>
    /// in Project Wide Database of the active <see cref="IAutocadDocument"/>.
    /// </summary>
    public const string CommitInternalIds = "COMMIT_INTERNAL_IDS";

    /// <summary>
    /// The key for the <see cref="IObjectIdTagRecord"/> containing <see cref=
    /// "IObjectIdTag"/>s relating to all created <see cref="IAutocadLayout"/>s in the
    /// active <see cref="IAutocadDocument"/>.
    /// </summary>
    public const string Layouts = "LAYOUT_NAMES";

    /// <summary>
    /// The key for the <see cref="IDataTagRecord"/> containing <see cref=
    /// "ICeiling3dGroupIdentity.InternalId"/> <see cref="IDataTag"/>s in 
    /// Project Wide Database of the active <see cref="IAutocadDocument"/>.
    /// </summary>
    public const string Ceiling3dGroupIds = "CEILING_ASSEMBLY_IDS";

    /// <summary>
    /// The key for the <see cref="IDataTagRecord"/> containing <see cref="ISheet"/> 
    /// <see cref="IDataTag"/>s in the <see cref="IAutocadLayout"/> instance.
    /// </summary>
    public const string SheetKey = "SHEET";
    
    /// <summary>
    /// The key for the <see cref="ISuspensionGrid"/> layout <see cref="IObjectIdTagRecord"/>
    /// containing <see cref="IObjectIdTag"/>s for <see cref="IDecoratedPositionNumber"/>s.
    /// </summary>
    public const string DecoratedPositionNumberKey = "DecoratedPositionNumber";

    /// <summary>
    /// The key for the <see cref="IObjectIdTagRecord"/> which stores the <see cref="IObjectIdTag"/>
    /// for the <see cref="IPanelGrid"/> <see cref="IGridInstanceType"/> in the document.
    /// </summary>
    public const string PanelGridTypeTagKey = "PanelGridTypeTag";

    /// <summary>
    /// The key for the <see cref="IObjectIdTagRecord"/> which stores the <see cref="IObjectIdTag"/>
    /// for the <see cref="ISuspensionGrid"/> <see cref="IGridInstanceType"/> in the document. 
    /// </summary>
    public const string SuspensionGridTypeTagKey = "SuspensionGridTypeTag";

    /// <summary>
    /// The key for the <see cref="IObjectIdTagRecord"/> which stores the <see cref="IObjectIdTag"/>
    /// for the <see cref="IPanelGrid"/> <see cref="IGridInstance"/> in the document. This is always
    /// a single <see cref="IEntity"/> in the <see cref="IAutocadDocument"/>. 
    /// </summary>
    public const string PanelGridTagKey = "PanelTags";

    /// <summary>
    /// The key for the <see cref="IObjectIdTagRecord"/> which stores the <see cref="IObjectIdTag"/>
    /// for the <see cref="ISuspensionGrid"/> <see cref="IGridInstance"/> in the document. This is
    /// always a single <see cref="IEntity"/> in the <see cref="IAutocadDocument"/>. 
    /// </summary>
    public const string SuspensionGridTagKey = "SuspensionTags";

    /// <summary>
    /// The key for the <see cref="IObjectIdTagRecord"/> containing
    /// <see cref="IObjectIdTag"/>s relating to <see cref="IPanelType.BlockTableRecord"/>s.
    /// </summary>
    public const string PanelTypeTagsKey = "PanelTypeBlockTags";

    /// <summary>
    /// The key for the <see cref="IDataTagRecord"/> containing <see cref="IPanelType"/>
    /// <see cref="IPanelDefinition"/> <see cref="IDataTag"/>s such as the
    /// <see cref="PanelShape"/> and <see cref="IPanelTypeIdentity.PositionNumber"/>.
    /// </summary>
    public const string PanelTypeDefinitionTagsKey = "PanelTypeDefinitionTags";

    /// <summary>
    /// The key for the <see cref="IDataTagRecord"/> containing <see cref="IPanelType"/>
    /// <see cref="IGlobalGrainDirection"/> <see cref="IDataTag"/>s such as the
    /// <see cref="IGlobalGrainDirection.GrainType"/> and
    /// <see cref="IGlobalGrainDirection.ActualDirection"/>.
    /// </summary>
    public const string PanelTypeGrainTagsKey = "PanelTypeGrainDirectionTagsKey";

    /// <summary>
    /// The key for the <see cref="IDataTagRecord"/> containing the Id of the Text Object for
    /// <see cref="IPanelTypePositionNumber"/>
    /// </summary>
    public const string PositionNumberTextTagsKey = "PositionNumberTextTagsKey";

    /// <summary>
    /// The key for the <see cref="IDataTagRecord"/> containing the Curve Id of the <see
    /// cref="IPerforationBoundary"/> Curves.
    /// </summary>
    public const string PerforationCurveTagsKey = "PerforationCurveTagsKey";

    /// <summary>
    /// The key for the <see cref="IDataTagRecord"/> containing <see cref="IPanelTypeEdgeLoop"/>
    /// <see cref="IDataTag"/>s such as the  <see cref="IPanelProfileExtents3d"/>.
    /// </summary>
    public const string PanelTypePerforationExtentsKey = "PanelTypeEdgeLoopTagsKey";

    /// <summary>
    /// The key for the <see cref="IDataTagRecord"/> containing <see cref="IDataTag"/>
    /// relating to the active <see cref="ISheetTemplateDefinition"/>.
    /// </summary>
    public const string SheetTemplateDefinition = "SheetTemplateDefinitionTags";

    /// <summary>
    /// The key for the <see cref="IFinishedEdge3d"/> <see cref="IObjectIdTagRecord"/>.
    /// </summary>
    public const string TrimEdgeKey = "TrimEdges";

    /// <summary>
    /// The key for the <see cref="IAccessory"/> <see cref="IObjectIdTagRecord"/>.
    /// </summary>
    public const string ClipAccessoriesKey = "ClipAccessories";
}
