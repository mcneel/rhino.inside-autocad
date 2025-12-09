using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Services;
using System.Collections;

namespace Rhino.Inside.AutoCAD.Interop;

///<inheritdoc cref="ILayerRepository"/>
public class LayoutRepository : Disposable, ILayoutRepository
{
    private readonly Dictionary<string, IAutocadLayout> _layouts = new();
    private readonly Document _document;

    /// <summary>
    /// Constructs a new <see cref="LayoutRepository"/>.
    /// </summary>
    public LayoutRepository(Document document)
    {
        _document = document;

        this.Populate();

        this.SubscribeToModifyEvent();
    }

    private void SubscribeToModifyEvent()
    {
        var database = _document.Database;

        using var transactionManager = database.TransactionManager;

        using var transaction = transactionManager.StartTransaction();

        using var layerTable = (DBDictionary)transactionManager.GetObject(database.LayoutDictionaryId, OpenMode.ForRead);

        layerTable.Modified += this.LayoutTable_Modified;

        transaction.Commit();
    }

    /// <summary>
    /// Handles the LayoutTable Modified event.
    /// </summary>
    private void LayoutTable_Modified(object sender, EventArgs e)
    {
        this.Populate();
    }

    private void UnsubscribeToModifyEvent()
    {
        var database = _document.Database;

        using var transactionManager = database.TransactionManager;

        using var transaction = transactionManager.StartTransaction();

        using var layerTable = (DBDictionary)transactionManager.GetObject(database.LayoutDictionaryId, OpenMode.ForRead);

        layerTable.Modified -= this.LayoutTable_Modified;

        transaction.Commit();
    }

    ///<inheritdoc />
    public bool TryGetByName(string name, out IAutocadLayout? layout) =>
        _layouts.TryGetValue(name, out layout);

    ///<inheritdoc />
    public void Populate()
    {
        _layouts.Clear();

        var database = _document.Database;

        using var transactionManager = database.TransactionManager;

        using var transaction = transactionManager.StartTransaction();

        using var layouts = (DBDictionary)transactionManager
            .GetObject(database.LayoutDictionaryId, OpenMode.ForRead);

        foreach (var entity in layouts)
        {
            var layout = (Layout)entity.Value.GetObject(OpenMode.ForRead)!;

            var layoutName = layout.LayoutName;

            var layoutWrapper = new AutocadLayoutWrapper(layout);

            _layouts.Add(layoutName, layoutWrapper);
        }

        transaction.Commit();

    }

    /// <summary>
    /// Creates a new layer in the active document and returns the <see cref="IAutocadLayout"/>. 
    /// </summary>
    private IAutocadLayout CreateLayout(string name)
    {
        using var documentLock = _document.LockDocument();

        this.UnsubscribeToModifyEvent();

        var database = _document.Database;

        using var transactionManager = database.TransactionManager;

        using var transaction = transactionManager.StartTransaction();

        var layout = new Layout()
        {
            LayoutName = name
        };

        using var layoutDictionary = (DBDictionary)transactionManager.GetObject(database.LayoutDictionaryId, OpenMode.ForWrite);

        layoutDictionary[name] = layout;

        transactionManager.AddNewlyCreatedDBObject(layout, true);

        var layerWrapper = new AutocadLayoutWrapper(layout);

        transaction.Commit();

        this.SubscribeToModifyEvent();

        return layerWrapper;

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
            this.UnsubscribeToModifyEvent();

            foreach (var layer in _layouts.Values)
                layer.Dispose();

            _disposed = true;
        }
    }
}