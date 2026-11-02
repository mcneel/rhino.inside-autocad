using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that returns AutoCAD elements matching a selection filter.
/// </summary>
[ComponentVersion(introduced: "1.0.9", updated: "1.3.9")]
public class GetAutocadObjectsByFilterComponent : RhinoInsideAutocad_ComponentBase, IReferenceComponent
{
    private const string AutoUpdateEnabledKey = GrasshopperKeys.AutoUpdateEnabled;
    private const string AutoUpdateMenuItemText = GrasshopperMessages.AutoUpdateMenuItem;
    private const string AutoUpdateTooltipText = GrasshopperMessages.AutoUpdateFilterTooltip;

    private readonly GooConverter _gooConverter;
    private bool _autoUpdateEnabled;

    // Data dam: in manual mode the query only runs when the Query button is pressed
    // (which sets _manualUpdateRequested). Any other expiration (input change) clears the
    // output until the button is pressed again.
    private bool _manualUpdateRequested;

    /// <summary>
    /// Gets a value indicating whether auto update is enabled.
    /// When disabled, the component will not auto-expire based on document changes.
    /// </summary>
    public bool AutoUpdateEnabled => _autoUpdateEnabled;

    /// <inheritdoc />
    public override Guid ComponentGuid => new("D6E8F0A2-B4C6-4D8E-9F0A-2B4C6D8E0F1A");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.primary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.GetAutocadElementsByFilterComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAutocadObjectsByFilterComponent"/> class.
    /// </summary>
    public GetAutocadObjectsByFilterComponent()
        : base("Get AutoCAD Objects By Filter", "AC-GetByFilter",
            "Returns AutoCAD elements matching a selection filter",
            "AutoCAD", "Filter")
    {
        _gooConverter = new GooConverter();
    }

    /// <inheritdoc />
    public override void CreateAttributes()
    {
        m_attributes = new GetAutocadObjectsByFilterComponentAttributes(this);
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadDocument(GH_ParamAccess.item), "Document",
            "Doc", "An AutoCAD Document. If not provided, the active document will be used.", GH_ParamAccess.item);
        pManager[0].Optional = true;

        pManager.AddParameter(new Param_AutocadFilter(GH_ParamAccess.item), "Filter",
            "F", "The selection filter to use for querying elements.", GH_ParamAccess.item);

        pManager.AddIntegerParameter("Limit", "L",
            "Maximum number of objects to return. Use 0 for unlimited. The default is 100.", GH_ParamAccess.item, 100);
        pManager[2].Optional = true;
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddGenericParameter("Objects", "O", "The AutoCAD objects matching the filter",
            GH_ParamAccess.list);
        pManager.AddIntegerParameter("Count", "C", "The number of objects found",
            GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        // Only run the query when auto-update is on, or when the Query button triggered this
        // solve. Any other expiration (an input change) clears the previous output until the
        // user presses Query again.
        var runQuery = _autoUpdateEnabled || _manualUpdateRequested;
        _manualUpdateRequested = false;

        if (!runQuery)
        {
            DA.SetDataList(0, new List<IGH_Goo>());
            DA.SetData(1, 0);
            return;
        }

        AutocadDocument? autocadDocument = null;
        DA.GetData(0, ref autocadDocument);

        var document = this.GetDocumentOrDefault(autocadDocument);

        if (document is null)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No active AutoCAD document available");
            return;
        }

        GH_AutocadFilter? filterGoo = null;
        if (!DA.GetData(1, ref filterGoo) || filterGoo?.Value == null)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "A valid filter is required");
            return;
        }

        var filter = filterGoo.Value;

        var limit = 100;
        DA.GetData(2, ref limit);

        var returnAllFilterObjects = limit <= 0;

        var selectionFilter = filter.GetSelectionFilter().Unwrap();

        var cadDocument = document.Unwrap();
        var promptResult = cadDocument.Editor.SelectAll(selectionFilter);

        if (promptResult.Status != PromptStatus.OK)
        {
            DA.SetDataList(0, new List<IGH_Goo>());
            DA.SetData(1, 0);
            return;
        }

        var selectionSet = promptResult.Value;

        if (returnAllFilterObjects)
        {
            limit = selectionSet.Count;
        }

        var count = Math.Min(selectionSet.Count, limit);

        var transactionManagerWrapper = document.CreateTransactionManager();

        var elements = transactionManagerWrapper.PerformTask(() =>
       {
           var result = new List<IGH_Goo>();
           var transaction = transactionManagerWrapper.Unwrap();
           var processedCount = 0;

           foreach (SelectedObject selectedObject in selectionSet)
           {
               if (selectedObject == null) continue;
               if (processedCount >= limit) break;

               var entity = transaction.GetObject(selectedObject.ObjectId, OpenMode.ForRead) as DBObject;
               if (entity == null) continue;

               processedCount++;

               var wrapped = new AutocadDbObjectWrapper(entity);

               var goo = _gooConverter.CreateGoo(wrapped);

               if (goo != null)
               {
                   result.Add(goo);
               }
           }
           return result;
       });

        DA.SetDataList(0, elements);
        DA.SetData(1, count);
    }

    /// <inheritdoc />
    public bool NeedsToBeExpired(IAutocadDocumentChange change, bool includeModified = true)
    {
        // If auto update is disabled, never auto-expire
        if (!_autoUpdateEnabled)
            return false;

        // Check output params for referenced objects that may have changed
        foreach (var ghParam in this.Params.Output.OfType<IReferenceParam>())
        {
            if (ghParam.NeedsToBeExpired(change, includeModified)) return true;
        }

        // Check if any objects currently in the output were affected by the change
        var outputParam = this.Params.Output[0];
        foreach (var goo in outputParam.VolatileData.AllData(true).OfType<IGH_AutocadReference>())
        {
            if (change.DoesEffectObject(goo.Reference.ObjectId, includeModified))
                return true;
        }

        // Check input filter - ask the filter if this change is relevant to its criteria
        var filterParam = this.Params.Input[1];
        foreach (var data in filterParam.VolatileData.AllData(true))
        {
            if (data is GH_AutocadFilter filterGoo && filterGoo.Value?.IsAffectedByChange(change) == true)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Appends additional menu items to the component's context menu.
    /// </summary>
    protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
    {
        base.AppendAdditionalComponentMenuItems(menu);
        Menu_AppendSeparator(menu);

        var autoUpdateItem = Menu_AppendItem(
            menu,
            AutoUpdateMenuItemText,
            this.OnAutoUpdateMenuClick,
            true,
            _autoUpdateEnabled
        );
        autoUpdateItem.ToolTipText = AutoUpdateTooltipText;
    }

    /// <summary>
    /// Handles the click event for the Auto Update menu item.
    /// </summary>
    private void OnAutoUpdateMenuClick(object? sender, EventArgs e)
    {
        _autoUpdateEnabled = !_autoUpdateEnabled;
        this.ExpireSolution(true);
    }

    /// <summary>
    /// Triggers a manual update of the component.
    /// Called by the custom attributes when the Query button is clicked.
    /// </summary>
    public void TriggerManualUpdate()
    {
        _manualUpdateRequested = true;
        this.ExpireSolution(true);
    }

    /// <inheritdoc />
    public override bool Read(GH_IReader reader)
    {
        if (!base.Read(reader))
            return false;

        _autoUpdateEnabled = false;
        reader.TryGetBoolean(AutoUpdateEnabledKey, ref _autoUpdateEnabled);

        return true;
    }

    /// <inheritdoc />
    public override bool Write(GH_IWriter writer)
    {
        if (!base.Write(writer))
            return false;

        writer.SetBoolean(AutoUpdateEnabledKey, _autoUpdateEnabled);

        return true;
    }
}
