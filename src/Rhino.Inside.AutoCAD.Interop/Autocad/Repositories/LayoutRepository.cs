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

        this.Update();
    }

    ///<inheritdoc />
    public bool TryGetByName(string name, out IAutocadLayout? layout) =>
        _layouts.TryGetValue(name, out layout);

    ///<inheritdoc />
    public void Update()
    {
        _layouts.Clear();

        _ = _document.Transaction(transactionManagerWrapper =>
        {
            var database = _document.Database.Unwrap();

            var transactionManager = transactionManagerWrapper.Unwrap();

            var layoutDictionaryId = database.LayoutDictionaryId;

            using var layouts = (DBDictionary)transactionManager
                .GetObject(layoutDictionaryId, OpenMode.ForRead);

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