using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using NaturalSort.Extension;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Services;
using System.Collections;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A class which represents a repository for handling
/// <see cref="IAutocadLayerTableRecord"/>s.
/// </summary>
public class LayerRepository : Disposable, ILayerRepository
{
    private readonly InternalColorConverter _internalColorConverter = InternalColorConverter.Instance;
    private readonly string _defaultLayerName = InteropConstants.DefaultLayerName;
    private readonly SortedDictionary<string, IAutocadLayerTableRecord> _layers;

    private readonly Document _document;

    /// <inheritdoc/>
    public event EventHandler<ILayerAddedEventArgs>? LayerAdded;

    /// <inheritdoc/>
    public event EventHandler? LayerTableModified;

    /// <summary>
    /// Constructs a new <see cref="LayerRepository"/>.
    /// </summary>
    public LayerRepository(IAutocadDocument document) : this(document.Unwrap())
    {
    }

    /// <summary>
    /// Constructs a new <see cref="LayerRepository"/>.
    /// </summary>
    public LayerRepository(Document document)
    {
        _document = document;

        var comparer = StringComparison.OrdinalIgnoreCase.WithNaturalSort();

        _layers = new SortedDictionary<string, IAutocadLayerTableRecord>(comparer);

        this.SubscribeToModifyEvent();

        this.Populate();
    }

    /// <summary>
    /// Creates a new layer in the active document and returns the <see cref="IAutocadLayerTableRecord"/>. 
    /// </summary>
    private IAutocadLayerTableRecord CreateLayer(IColor color, string name)
    {
        using var documentLock = _document.LockDocument();

        this.UnsubscribeToModifyEvent();

        var database = _document.Database;

        using var transactionManager = database.TransactionManager;

        using var transaction = transactionManager.StartTransaction();

        var layerTableRecord = new LayerTableRecord
        {
            Name = name,
            Color = _internalColorConverter.ToCadColor(color)
        };

        using var layerTable = (LayerTable)transactionManager.GetObject(database.LayerTableId, OpenMode.ForWrite);

        layerTable.Add(layerTableRecord);

        transactionManager.AddNewlyCreatedDBObject(layerTableRecord, true);

        var layerWrapper = new AutocadLayerTableRecordWrapper(layerTableRecord);

        transaction.Commit();

        this.SubscribeToModifyEvent();

        return layerWrapper;

    }

    /// <summary>
    /// Handles the LayerTable Modified event.
    /// </summary>
    private void LayerTable_Modified(object sender, EventArgs e)
    {
        this.Populate();
        LayerTableModified?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Subscribes to the LayerTable Modified event.
    /// </summary>
    private void SubscribeToModifyEvent()
    {
        var database = _document.Database;

        using var transactionManager = database.TransactionManager;

        using var transaction = transactionManager.StartTransaction();

        using var layerTable = (LayerTable)transactionManager.GetObject(database.LayerTableId, OpenMode.ForRead);

        layerTable.Modified += this.LayerTable_Modified;

        transaction.Commit();
    }

    /// <summary>
    /// Unsubscribes from the LayerTable Modified event.
    /// </summary>
    private void UnsubscribeToModifyEvent()
    {
        var database = _document.Database;

        using var transactionManager = database.TransactionManager;

        using var transaction = transactionManager.StartTransaction();

        using var layerTable = (LayerTable)transactionManager.GetObject(database.LayerTableId, OpenMode.ForRead);

        layerTable.Modified -= this.LayerTable_Modified;

        transaction.Commit();
    }

    /// <summary>
    /// Populates this <see cref="ILayerRepository"/> with <see cref="IAutocadLayerTableRecord"/>s
    /// from the active <see cref="IAutocadDocument"/>.
    /// </summary>
    private void Populate()
    {
        _layers.Clear();

        var database = _document.Database;

        using var transactionManager = database.TransactionManager;

        using var transaction = transactionManager.StartTransaction();

        using var layerTable = (LayerTable)transactionManager.GetObject(database.LayerTableId, OpenMode.ForRead);

        foreach (var layerId in layerTable)
        {
            var cadLayer = (LayerTableRecord)transactionManager.GetObject(layerId, OpenMode.ForRead);

            var layer = new AutocadLayerTableRecordWrapper(cadLayer);

            var layerName = layer.Name;
            if (_layers.ContainsKey(layerName) == false)
                _layers[layerName] = layer;
        }

        transaction.Commit();
    }

    /// <inheritdoc/>
    public bool Exists(IAutocadLayerTableRecord layer)
    {
        return _layers.ContainsKey(layer.Name);
    }

    /// <inheritdoc/>
    public bool TryGetByName(string name, out IAutocadLayerTableRecord? layer)
    {
        var containsLayer = _layers.TryGetValue(name, out layer);

        return containsLayer;
    }

    /// <inheritdoc/>
    public IAutocadLayerTableRecord GetByNameOrDefault(string name)
    {
        return this.TryGetByName(name, out var layer) ? layer! : this.GetDefault();
    }

    /// <inheritdoc/>
    public bool TryAddLayer(IColor color, string name, out IAutocadLayerTableRecord layer)
    {
        if (_layers.ContainsKey(name) == false)
        {
            layer = this.CreateLayer(color, name);

            _layers[layer.Name] = layer;

            var layerAddedEventArgs = new LayerAddedEventArgs(layer);

            LayerAdded?.Invoke(this, layerAddedEventArgs);

            return true;
        }

        layer = this.GetDefault();
        return false;
    }

    /// <inheritdoc/>
    public IAutocadLayerTableRecord GetDefault()
    {
        _ = _layers.TryGetValue(_defaultLayerName, out var layer);

        return layer!;
    }

    /// <inheritdoc/>
    public IList<IAutocadLayerTableRecord> GetLayers(IList<string> names)
    {
        var layers = new List<IAutocadLayerTableRecord>();

        foreach (var name in names)
        {
            if (this.TryGetByName(name, out var layer))
                layers.Add(layer!);
        }

        return layers;
    }

    /// <inheritdoc/>
    public IEnumerator<IAutocadLayerTableRecord> GetEnumerator()
    {
        foreach (var entity in _layers.Values)
        {
            yield return entity;
        }
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

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

            foreach (var layer in _layers.Values)
                layer.Dispose();

            _disposed = true;
        }
    }
}
