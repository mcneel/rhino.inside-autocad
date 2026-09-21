using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using CadEntity = Autodesk.AutoCAD.DatabaseServices.Entity;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Base class for AutoCAD object parameters in Grasshopper.
/// </summary>
/// <typeparam name="TGoo">The Grasshopper Goo type that wraps the AutoCAD entity.</typeparam>
/// <typeparam name="TEntity">The AutoCAD entity type.</typeparam>
public abstract class Param_AutocadObjectBase<TGoo, TEntity> : GH_PersistentGeometryParam<TGoo>,
    IReferenceParam, IGH_PreviewObject
    where TGoo : class, IGH_GeometricGoo, IGH_AutocadReference
    where TEntity : CadEntity
{
    private const string SkippedSelectionSingleFormat = GrasshopperMessages.SkippedSelectionSingleFormat;
    private const string SkippedSelectionFormat = GrasshopperMessages.SkippedSelectionFormat;

    /// <summary>
    /// The number of objects picked in AutoCAD during the last prompt that could not be
    /// converted to <typeparamref name="TGoo"/>. Reported as a runtime warning when the
    /// parameter next collects its data.
    /// </summary>
    private int _skippedSelectionCount;

    /// <inheritdoc />
    public BoundingBox ClippingBox => this.Preview_ComputeClippingBox();

    /// <inheritdoc />
    public bool IsPreviewCapable => true;

    /// <inheritdoc />
    public bool Hidden { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadObjectBase{TGoo, TEntity}"/> class.
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="nickname">The nickname of the parameter.</param>
    /// <param name="description">The description of the parameter.</param>
    /// <param name="category">The category of the parameter.</param>
    /// <param name="subcategory">The subcategory of the parameter.</param>
    protected Param_AutocadObjectBase(string name, string nickname,
        string description, string category, string subcategory)
        : base(new GH_InstanceDescription(name, nickname, description, category,
            subcategory))
    {
        this.Hidden = false;
    }

    /// <summary>
    /// Creates the filter to use for selecting objects in AutoCAD.
    /// </summary>
    /// <returns>A filter that implements <see cref="IObjectFilter"/>.</returns>
    protected abstract IObjectFilter CreateSelectionFilter();

    /// <summary>
    /// Gets the message to display when prompting for a single object.
    /// </summary>
    protected abstract string SingularPromptMessage { get; }

    /// <summary>
    /// Gets the message to display when prompting for multiple objects.
    /// </summary>
    protected abstract string PluralPromptMessage { get; }

    /// <summary>
    /// Wraps an AutoCAD entity in the appropriate Grasshopper Goo type.
    /// </summary>
    /// <param name="entity">The entity to wrap.</param>
    /// <returns>The wrapped entity as a Grasshopper Goo object.</returns>
    protected abstract TGoo WrapEntity(TEntity entity);

    /// <summary>
    /// Gives the opportunity to convert a support object into the desired TGoo type
    /// during selection.
    /// </summary>
    /// <remarks>
    /// The entity is the wrapper around the picked AutoCAD object, so an implementation
    /// must call <see cref="InteropConverter.Unwrap(IEntity)"/> before testing it against
    /// an AutoCAD type. Testing the wrapper directly never matches.
    /// </remarks>
    protected virtual bool ConvertSupportObject(IEntity entity, out TGoo supportedGoo)
    {
        supportedGoo = null!;
        return false;
    }

    /// <summary>
    /// Resolves an entity picked in AutoCAD into this parameter's Goo type, either
    /// directly when it is already a <typeparamref name="TEntity"/>, or through <see
    /// cref="ConvertSupportObject"/> when it is a supported alternative type.
    /// </summary>
    /// <param name="entity">The picked entity, which may be <see langword="null"/>.</param>
    /// <param name="goo">
    /// The resolved Goo when this method returns <see langword="true"/>; otherwise
    /// <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the entity was resolved; otherwise <see langword="false"/>.
    /// </returns>
    private bool TryResolveGoo(IEntity? entity, out TGoo goo)
    {
        goo = null!;

        if (entity == null) return false;

        if (entity.Unwrap() is TEntity typedEntity)
        {
            goo = this.WrapEntity(typedEntity);

            return true;
        }

        // A misbehaving override returning true with a null Goo would put a null into
        // the persistent data, which is the silent empty parameter all over again.
        return this.ConvertSupportObject(entity, out goo) && goo != null;
    }

    /// <inheritdoc />
    protected override GH_GetterResult Prompt_Singular(ref TGoo value)
    {
        _skippedSelectionCount = 0;

        var picker = new AutocadObjectPicker();

        var filter = this.CreateSelectionFilter();

        var selectionFilter = filter.GetSelectionFilter();

        var entity = picker.PickObject(selectionFilter, this.SingularPromptMessage);

        if (this.TryResolveGoo(entity, out var goo))
        {
            value = goo;

            return GH_GetterResult.success;
        }

        // A picked object that resolves to nothing would otherwise leave the parameter
        // silently empty, so report it. Grasshopper's menu handler does not expire the
        // parameter when a prompt cancels, so the warning needs a solution of its own.
        if (entity != null)
            this.ReportSkippedSelection(1);

        value = default;
        return GH_GetterResult.cancel;
    }

    /// <inheritdoc />
    protected override GH_GetterResult Prompt_Plural(ref List<TGoo> values)
    {
        _skippedSelectionCount = 0;

        var picker = new AutocadObjectPicker();

        var filter = this.CreateSelectionFilter();

        var selectionFilter = filter.GetSelectionFilter();

        var entities = picker.PickObjects(selectionFilter, this.PluralPromptMessage);

        var newValues = new List<TGoo>();
        var skipped = 0;

        foreach (var entity in entities)
        {
            if (this.TryResolveGoo(entity, out var goo))
            {
                newValues.Add(goo);

                continue;
            }

            if (entity != null)
                skipped++;
        }

        if (skipped > 0)
            this.ReportSkippedSelection(skipped);

        // Cancelling the pick, or picking nothing convertible, must leave the existing
        // values alone rather than clearing the parameter.
        if (newValues.Count == 0) return GH_GetterResult.cancel;

        values = newValues;

        return GH_GetterResult.success;
    }

    /// <summary>
    /// Records that objects picked in AutoCAD could not be converted and forces a new
    /// solution, so that the warning raised by <see cref="PostProcessData"/> is shown
    /// without the user having to touch the definition.
    /// </summary>
    /// <param name="count">The number of objects that were skipped.</param>
    private void ReportSkippedSelection(int count)
    {
        _skippedSelectionCount = count;

        this.ExpireSolution(true);
    }

    /// <inheritdoc />
    /// <remarks>
    /// The warning is raised here rather than in the prompts because Grasshopper clears
    /// a parameter's runtime messages at the start of every solution, which runs after
    /// the prompt has returned.
    /// </remarks>
    public override void PostProcessData()
    {
        base.PostProcessData();

        if (_skippedSelectionCount == 0) return;

        var message = _skippedSelectionCount == 1
            ? string.Format(SkippedSelectionSingleFormat, this.TypeName)
            : string.Format(SkippedSelectionFormat, _skippedSelectionCount, this.TypeName);

        this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, message);
    }

    /// <inheritdoc />
    public bool NeedsToBeExpired(IAutocadDocumentChange change, bool includeModified = true)
    {
        foreach (var autocadId in m_data.AllData(true).OfType<TGoo>())
        {
            if (change.DoesEffectObject(autocadId.Reference.ObjectId, includeModified))
                return true;
        }

        return false;
    }

    /// <inheritdoc />
    public void DrawViewportWires(IGH_PreviewArgs args) =>
        this.Preview_DrawWires(args);

    /// <inheritdoc />
    public void DrawViewportMeshes(IGH_PreviewArgs args) =>
        this.Preview_DrawMeshes(args);

}

