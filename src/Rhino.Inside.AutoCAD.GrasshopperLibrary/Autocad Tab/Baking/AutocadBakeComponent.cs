using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using System.Collections;
using Exception = System.Exception;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that bakes AutoCAD objects to the model space.
/// The bake is triggered by the Bake button shown on the component when the
/// "Driven Button" context menu toggle is enabled (the default), or by the "Bake"
/// boolean input that takes the button's place when the toggle is disabled.
/// </summary>
[ComponentVersion(introduced: "1.0.0", updated: "1.3.11")]
public class AutocadBakeComponent : RhinoInsideAutocad_ComponentBase, IBakingComponent
{
    private const string DrivenButtonEnabledKey = GrasshopperKeys.DrivenButtonEnabled;
    private const string DrivenButtonMenuItemText = GrasshopperMessages.DrivenButtonMenuItem;
    private const string DrivenButtonTooltipText = GrasshopperMessages.DrivenButtonTooltip;
    private const int SettingsParamIndex = 2;
    private const int BakeParamIndex = 3;

    private bool _drivenButtonEnabled = true;

    // Data dam: the Bake button only runs the bake on the solve it triggered
    // (which sets _manualRunRequested). Any other expiration (input change) is
    // driven by the Bake input alone.
    private bool _manualRunRequested;

    /// <summary>
    /// Gets a value indicating whether the Bake button is shown on the component.
    /// The button and the Bake input are alternatives: when the button is shown the
    /// input is not registered, and when it is hidden the input takes its place.
    /// </summary>
    public bool DrivenButtonEnabled => _drivenButtonEnabled;

    /// <summary>
    /// Gets a value indicating whether the Bake input is currently registered.
    /// </summary>
    /// <remarks>
    /// The input is the last of the component's parameters, so its presence is the
    /// only thing that can lengthen the list beyond <see cref="BakeParamIndex"/>.
    /// </remarks>
    private bool HasBakeInput => this.Params.Input.Count > BakeParamIndex;

    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("C5D7E9F1-A3B5-4C7D-9E1F-3A5B7C9D1E3F");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.AutocadBakeComponent;

    /// <inheritdoc />
    public int OutputParamTargetIndex => 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutocadBakeComponent"/> class.
    /// </summary>
    public AutocadBakeComponent()
        : base("Bake to AutoCAD", "AC-Bake",
            "Bakes objects to AutoCAD's model space",
            "AutoCAD", "Baking")
    {
    }

    /// <inheritdoc />
    public override void CreateAttributes()
    {
        m_attributes = new AutocadBakeComponentAttributes(this);
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadDocument(GH_ParamAccess.item), "Document",
            "Doc", "The AutoCAD document to bake to. If not provided, the active document will be used.", GH_ParamAccess.item);
        pManager[0].Optional = true;

        pManager.AddGenericParameter("Objects", "O",
            "The objects to bake to AutoCAD (curves, points, meshes, solids)",
            GH_ParamAccess.list);

        pManager.AddParameter(new Param_BakeSettings(GH_ParamAccess.list), "Settings",
            "S", BakeSettingsMatcher.SettingsDescription, GH_ParamAccess.list);
        pManager[2].Optional = true;

        // Registered only when the button is not shown: the field initialiser has already
        // run by the time the base constructor calls this, so the toggle is readable here.
        if (_drivenButtonEnabled == false)
        {
            var bakeParam = CreateBakeParam();
            pManager.AddParameter(bakeParam);
        }
    }

    /// <summary>
    /// Creates the Bake input parameter.
    /// </summary>
    /// <remarks>
    /// The parameter is registered and unregistered as the Driven Button toggle changes,
    /// so it is described here rather than inline in <see cref="RegisterInputParams"/>
    /// where only the initial registration would see it.
    /// </remarks>
    private static Param_Boolean CreateBakeParam()
    {
        var bakeParam = new Param_Boolean
        {
            Name = "Bake",
            NickName = "Bake",
            Description = "A boolean when true the Objects will be baked to AutoCAD",
            Access = GH_ParamAccess.item,
            Optional = true
        };

        return bakeParam;
    }

    /// <summary>
    /// Registers or unregisters the Bake input so that exactly one of the input and the
    /// Bake button is present on the component.
    /// </summary>
    private void SyncBakeInput()
    {
        var bakeInputRequired = _drivenButtonEnabled == false;

        if (bakeInputRequired == this.HasBakeInput)
            return;

        if (bakeInputRequired)
        {
            var bakeParam = CreateBakeParam();
            this.Params.RegisterInputParam(bakeParam, BakeParamIndex);
        }
        else
        {
            var bakeParam = this.Params.Input[BakeParamIndex];
            this.Params.UnregisterInputParameter(bakeParam);
        }

        this.Params.OnParametersChanged();
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadObjectId(GH_ParamAccess.list), "ObjectIds",
            "Ids", "The ObjectIds of the baked objects", GH_ParamAccess.list);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        // The Bake button triggers a bake on the solve it requested; the Bake input,
        // which is only registered when the button is hidden, drives every other solve.
        // The request is cleared in AfterSolveInstance, not here, so a click bakes every
        // iteration of the solve (one per branch of the Objects and Settings trees).
        var run = _manualRunRequested;

        if (this.HasBakeInput)
        {
            var bakeInput = false;
            DA.GetData(BakeParamIndex, ref bakeInput);

            run |= bakeInput;
        }

        if (run == false)
        {
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

        var objects = new List<object>();
        if (!DA.GetDataList(1, objects) || objects.Count == 0)
            return;

        var settingsGoos = new List<GH_BakeSettings?>();
        DA.GetDataList(2, settingsGoos);

        var settings = settingsGoos.Select(goo => goo?.Value).ToList();

        if (BakeSettingsMatcher.IsMismatched(objects.Count, settings))
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark,
                BakeSettingsMatcher.MismatchMessage(objects.Count, settings));
        }

        var converterFactory = new RhinoConvertibleFactory();
        var bakeableExtractor = new BakeableExtractor(converterFactory);

        // Settings are paired by the object's original index, so an unbakeable object
        // does not shift the settings of the objects after it.
        var bakeables = new List<(IAutocadBakeable Bakeable, IBakeSettings? Settings)>();
        for (var i = 0; i < objects.Count; i++)
        {
            var obj = objects[i];
            var bakeable = bakeableExtractor.ExtractBakeable(obj);
            if (bakeable != null)
            {
                var objectSettings = BakeSettingsMatcher.ForIndex(objects.Count, settings, i);
                bakeables.Add((bakeable, objectSettings));
            }
            else
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
                    $"Object of type {obj?.GetType().Name ?? "null"} is not bakeable");
            }
        }

        if (bakeables.Count == 0)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No bakeable objects provided");
            return;
        }

        var bakedIds = new List<GH_AutocadObjectId>();

        var transactionManagerWrapper = document.CreateTransactionManager();

        _ = transactionManagerWrapper.PerformTask(() =>
         {
             foreach (var (bakeable, objectSettings) in bakeables)
             {
                 try
                 {
                     var objectIds = bakeable.BakeToAutocad(transactionManagerWrapper, this, objectSettings);

                     foreach (var objectId in objectIds)
                     {
                         bakedIds.Add(new GH_AutocadObjectId(objectId));
                     }
                 }
                 catch (Exception ex)
                 {
                     this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
                         $"Failed to bake object: {ex.Message}");
                 }
             }

             return true;
         });

        DA.SetDataList(0, bakedIds);
    }

    /// <summary>
    /// Appends additional menu items to the component's context menu.
    /// </summary>
    protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
    {
        base.AppendAdditionalComponentMenuItems(menu);
        Menu_AppendSeparator(menu);

        var drivenButtonItem = Menu_AppendItem(
            menu,
            DrivenButtonMenuItemText,
            this.OnDrivenButtonMenuClick,
            true,
            _drivenButtonEnabled
        );
        drivenButtonItem.ToolTipText = DrivenButtonTooltipText;
    }

    /// <summary>
    /// Handles the click event for the Driven Button menu item, swapping the Bake button
    /// for the Bake input or back.
    /// </summary>
    /// <remarks>
    /// Recorded as an undo event because hiding the input removes it, taking any wire
    /// with it. The solution is not recomputed: the newly registered input holds no data
    /// and recomputing could otherwise re-bake with the Bake input held true.
    /// </remarks>
    private void OnDrivenButtonMenuClick(object? sender, EventArgs e)
    {
        this.RecordUndoEvent(DrivenButtonMenuItemText);

        _drivenButtonEnabled = !_drivenButtonEnabled;

        this.SyncBakeInput();

        this.Attributes?.ExpireLayout();
        Grasshopper.Instances.ActiveCanvas?.Invalidate();
    }

    /// <inheritdoc />
    protected override void AfterSolveInstance()
    {
        base.AfterSolveInstance();

        // The Bake button's request covers every iteration of the solve it triggered.
        _manualRunRequested = false;
    }

    /// <summary>
    /// Triggers a manual bake of the component.
    /// Called by the custom attributes when the Bake button is clicked.
    /// </summary>
    public void TriggerManualRun()
    {
        _manualRunRequested = true;
        this.ExpireSolution(true);
    }

    /// <inheritdoc />
    public override bool Read(GH_IReader reader)
    {
        if (!base.Read(reader))
            return false;

        _drivenButtonEnabled = true;
        reader.TryGetBoolean(DrivenButtonEnabledKey, ref _drivenButtonEnabled);

        // Files written before 1.3.11 store Settings as item access.
        BakeSettingsMatcher.UpgradeSettingsParam(this.Params.Input[SettingsParamIndex]);

        this.ReconcileBakeInputWithButton();

        return true;
    }

    /// <summary>
    /// Reconciles the restored parameters with the restored Driven Button state.
    /// </summary>
    /// <remarks>
    /// Files written before the input and the button became alternatives hold both. The
    /// input wins when something is wired to it, because dropping the parameter would
    /// silently delete that wire; otherwise the button wins and the input is dropped.
    /// Sources are still proxies while the document is being read, so both counts are
    /// checked.
    /// </remarks>
    private void ReconcileBakeInputWithButton()
    {
        if (_drivenButtonEnabled && this.HasBakeInput)
        {
            var bakeParam = this.Params.Input[BakeParamIndex];
            var isWired = bakeParam.SourceCount > 0 || bakeParam.ProxySourceCount > 0;

            if (isWired)
            {
                _drivenButtonEnabled = false;
                return;
            }
        }

        this.SyncBakeInput();
    }

    /// <inheritdoc />
    public override bool Write(GH_IWriter writer)
    {
        if (!base.Write(writer))
            return false;

        writer.SetBoolean(DrivenButtonEnabledKey, _drivenButtonEnabled);

        return true;
    }

    /// <inheritdoc />
    public void AddWarningMessage(string message)
    {
        this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, message);
    }

    /// <inheritdoc />
    public bool AppendDataList(IEnumerable list)
    {
        var ghParam = this.Params.Output[this.OutputParamTargetIndex];

        var path = new GH_Path(0);

        if (ghParam.VolatileData.PathCount > 0)
        {
            var lastPath = ghParam.VolatileData.Paths[ghParam.VolatileData.PathCount - 1];

            var indices = lastPath.Indices;

            indices[indices.Length - 1]++;

            path = new GH_Path(indices);
        }

        var result = ghParam.AddVolatileDataList(path, list);

        if (result)
        {
            ghParam.ExpireSolution(false);

            this.OnPingDocument()?.NewSolution(false);
        }

        return result;
    }
}
