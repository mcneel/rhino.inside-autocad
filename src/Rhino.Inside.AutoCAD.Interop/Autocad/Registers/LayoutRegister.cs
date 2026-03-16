using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

///<inheritdoc cref="ILayerRegister"/>
public class LayoutRegister : RegisterBase<IAutocadLayout>, ILayoutRegister
{
    /// <summary>
    /// Constructs a new <see cref="LayoutRegister"/>.
    /// </summary>
    public LayoutRegister(IAutocadDocument document) : base(document)
    {
    }

    ///<inheritdoc />
    public override void Repopulate()
    {
        _objects.Clear();

        _ = _document.Transaction(transactionManagerWrapper =>
        {
            var transactionManager = transactionManagerWrapper.Unwrap();

            using var layouts = (DBDictionary)transactionManager
                .GetObject(_document.Database.LayoutDictionaryId.Unwrap(), OpenMode.ForRead);

            foreach (var entity in layouts)
            {
                var layout = (Layout)entity.Value.GetObject(OpenMode.ForRead)!;

                var layoutWrapper = new AutocadLayoutWrapper(layout);

                _objects.Add(layoutWrapper.Id, layoutWrapper);
            }

            return true;
        });
    }

    /// <summary>
    /// Creates a new layer in the active document and returns the <see cref="IAutocadLayout"/>.
    /// </summary>
    private IAutocadLayout CreateLayout(string name)
    {
        using var documentLock = _document.Unwrap().LockDocument();

        var layoutWrapper = _document.Transaction(transactionManagerWrapper =>
        {
            var transactionManager = transactionManagerWrapper.Unwrap();

            var layout = new Layout()
            {
                LayoutName = name
            };

            using var layoutDictionary = (DBDictionary)transactionManager.GetObject(
                _document.Database.LayoutDictionaryId.Unwrap(), OpenMode.ForWrite);

            layoutDictionary[name] = layout;

            transactionManager.AddNewlyCreatedDBObject(layout, true);

            return new AutocadLayoutWrapper(layout);
        });

        _document.Regenerate();

        return layoutWrapper;
    }

    /// <inheritdoc/>
    public bool TryAddLayout(string name, out IAutocadLayout? layout)
    {
        if (this.TryGetByName(name, out _) == false)
        {
            layout = this.CreateLayout(name);

            _objects.Add(layout.Id, layout);

            return true;
        }

        layout = null;
        return false;
    }
}