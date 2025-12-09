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
public abstract class Param_AutocadObjectBase<TGoo, TEntity> : GH_PersistentGeometryParam<TGoo>, IReferenceParam, IGH_PreviewObject
    where TGoo : class, IGH_GeometricGoo, IGH_AutocadReference
    where TEntity : CadEntity
{

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
    protected Param_AutocadObjectBase(
        string name,
        string nickname,
        string description,
        string category,
        string subcategory)
        : base(new GH_InstanceDescription(name, nickname, description, category,
            subcategory))
    {
        this.Hidden = false;
    }

    /// <summary>
    /// Creates the filter to use for selecting objects in AutoCAD.
    /// </summary>
    /// <returns>A filter that implements <see cref="IFilter"/>.</returns>
    protected abstract IFilter CreateSelectionFilter();

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

    /// <inheritdoc />
    protected override GH_GetterResult Prompt_Singular(ref TGoo value)
    {
        var picker = new AutocadObjectPicker();

        var filter = this.CreateSelectionFilter();

        var selectionFilter = filter.GetSelectionFilter();

        var entity = picker.PickObject(selectionFilter, this.SingularPromptMessage);

        if (entity.Unwrap() is TEntity typedEntity)
        {
            value = this.WrapEntity(typedEntity);

            return GH_GetterResult.success;
        }

        value = default;
        return GH_GetterResult.cancel;
    }

    /// <inheritdoc />
    protected override GH_GetterResult Prompt_Plural(ref List<TGoo> values)
    {
        var picker = new AutocadObjectPicker();

        var filter = this.CreateSelectionFilter();

        var selectionFilter = filter.GetSelectionFilter();

        var entities = picker.PickObjects(selectionFilter, this.PluralPromptMessage);

        foreach (var entity in entities)
        {
            if (entity is TEntity typedEntity)
            {
                values.Add(this.WrapEntity(typedEntity));
            }
        }

        return GH_GetterResult.success;
    }

    /// <inheritdoc />
    public bool NeedsToBeExpired(IAutocadDocumentChange change)
    {
        foreach (var autocadId in m_data.AllData(true).OfType<TGoo>())
        {
            if (change.DoesAffectObject(autocadId.AutocadReferenceId))
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

