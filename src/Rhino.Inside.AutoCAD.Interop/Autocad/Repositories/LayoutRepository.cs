using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Services;
using System.Collections;

namespace Rhino.Inside.AutoCAD.Interop;

///<inheritdoc cref="ILayerRepository"/>
public class LayoutRepository : Disposable, ILayoutRepository
{
    private readonly Dictionary<string, IAutocadLayout> _layouts = new();
    private readonly IAutocadDocument _document;

    /// <summary>
    /// Constructs a new <see cref="LayoutRepository"/>.
    /// </summary>
    public LayoutRepository(IAutocadDocument document)
    {
        _document = document;

        this.Repopulate();
    }

    ///<inheritdoc />
    public bool TryGetByName(string name, out IAutocadLayout? layout) =>
        _layouts.TryGetValue(name, out layout);

    ///<inheritdoc />
    public void Repopulate()
    {
        _layouts.Clear();

        _ = _document.Transaction(transactionManagerWrapper =>
        {
            var transactionManager = transactionManagerWrapper.Unwrap();

            using var layouts = (DBDictionary)transactionManager
                .GetObject(_document.Database.LayoutDictionaryId.Unwrap(), OpenMode.ForRead);

            foreach (var entity in layouts)
            {
                var layout = (Layout)entity.Value.GetObject(OpenMode.ForRead)!;

                var layoutName = layout.LayoutName;

                var layoutWrapper = new AutocadLayoutWrapper(layout);

                _layouts.Add(layoutName, layoutWrapper);
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

        return layoutWrapper;
    }

    /// <inheritdoc/>
    public bool TryAddLayout(string name, out IAutocadLayout? layout)
    {
        if (_layouts.ContainsKey(name) == false)
        {
            layout = this.CreateLayout(name);

            _layouts[layout.Name] = layout;

            return true;
        }

        layout = null;
        return false;
    }

    ///<inheritdoc/>
    public IEnumerator<IAutocadLayout> GetEnumerator() => _layouts.Values.GetEnumerator();

    ///<inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    /// <summary>
    /// Disposes the <see cref="LayerRepository"/> and all its resources.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            foreach (var layer in _layouts.Values)
                layer.Dispose();

            _disposed = true;
        }
    }
}