using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using CadLayout = Autodesk.AutoCAD.DatabaseServices.Layout;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IAutocadLayout"/>
/// <remarks>
/// Wraps an AutoCAD <see cref="CadLayout"/> to expose layout properties such as
/// <see cref="TabOrder"/> and <see cref="BlockTableRecordId"/>.
/// Used by the Grasshopper library in layout components including
/// <c>AutocadLayoutComponent</c>, <c>GetAutocadLayoutsComponent</c>, and <c>CreateAutocadLayoutComponent</c>.
/// </remarks>
/// <seealso cref="ILayoutRegister"/>
public class AutocadLayoutWrapper : AutocadDbObjectWrapper, IAutocadLayout
{
    private readonly CadLayout _layout;

    /// <inheritdoc/>
    public int TabOrder { get; }

    /// <inheritdoc/>
    public IObjectId BlockTableRecordId { get; }

    /// <inheritdoc/>
    public string Name { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="AutocadLayoutWrapper"/>.
    /// </summary>
    /// <param name="layout">
    /// The AutoCAD <see cref="CadLayout"/> to wrap.
    /// </param>
    public AutocadLayoutWrapper(CadLayout layout) : base(layout)
    {
        _layout = layout;

        this.TabOrder = layout.TabOrder;

        this.BlockTableRecordId = new AutocadObjectIdWrapper(layout.BlockTableRecordId);

        this.Name = layout.LayoutName;
    }

    /// <inheritdoc/>
    public override IDbObject ShallowClone()
    {
        return new AutocadLayoutWrapper(_layout);
    }

    /// <summary>
    /// Creates a new layer in the active document and returns the <see cref="IAutocadLayout"/>.
    /// </summary>
    public static IAutocadLayout Create(IAutocadDocument document, string name)
    {
        var transactionManagerWrapper = document.CreateTransactionManager();

        var layoutWrapper = transactionManagerWrapper.PerformTask(() =>
        {
            var transactionManager = transactionManagerWrapper.Unwrap();

            var layout = new CadLayout()
            {
                LayoutName = name
            };

            var layoutDictionary = (DBDictionary)transactionManager.GetObject(
                document.AutocadDatabase.LayoutDictionaryId.Unwrap(), OpenMode.ForWrite);

            layoutDictionary[name] = layout;

            transactionManager.AddNewlyCreatedDBObject(layout, true);

            return new AutocadLayoutWrapper(layout);
        });

        document.Regenerate();

        return layoutWrapper;
    }
}