using Autodesk.AutoCAD.DatabaseServices;
using NaturalSort.Extension;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Services;
using System.Collections;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A class which represents a repository for handling
/// <see cref="IAutocadLayer"/>s.
/// </summary>
public class LayerRepository : Disposable, ILayerRepository
{
    private readonly InternalColorConverter _internalColorConverter = InternalColorConverter.Instance;
    private readonly string _defaultLayerName = InteropConstants.DefaultLayerName;
    private readonly SortedDictionary<string, IAutocadLayer> _layers;

    private readonly IAutocadDocument _document;
    private readonly ILinePatternCache _linePatternCache;

    /// <inheritdoc/>
    public event EventHandler<ILayerAddedEventArgs>? LayerAdded;

    /// <summary>
    /// Constructs a new <see cref="LayerRepository"/>.
    /// </summary>
    public LayerRepository(IAutocadDocument document, ILinePatternCache linePatternCache)
    {
        _document = document;

        _linePatternCache = linePatternCache;

        var comparer = StringComparison.OrdinalIgnoreCase.WithNaturalSort();

        _layers = new SortedDictionary<string, IAutocadLayer>(comparer);

        this.Populate();
    }

    /// <summary>
    /// Creates a new layer in the active document and returns the <see cref="IAutocadLayer"/>. 
    /// </summary>
    private IAutocadLayer CreateLayer(IColor color, string name)
    {
        var layer = _document.Transaction(transactionManagerWrapper =>
        {
            var transactionManager = transactionManagerWrapper.Unwrap();

            var database = _document.Database.Unwrap();

            var layerTableRecord = new LayerTableRecord
            {
                Name = name,
                Color = _internalColorConverter.ToCadColor(color)
            };

            using var layerTable = (LayerTable)transactionManager.GetObject(database.LayerTableId, OpenMode.ForWrite);

            layerTable.Add(layerTableRecord);

            transactionManager.AddNewlyCreatedDBObject(layerTableRecord, true);

            var linePatternId = new ObjectId(layerTableRecord.LinetypeObjectId);

            var linePattern = _linePatternCache.GetById(linePatternId);

            var layerWrapper = new AutocadLayerWrapper(layerTableRecord, linePattern);

            return layerWrapper;
        });

        return layer;
    }

    /// <summary>
    /// Populates this <see cref="ILayerRepository"/> with <see cref="IAutocadLayer"/>s
    /// from the active <see cref="IAutocadDocument"/>.
    /// </summary>
    private void Populate()
    {
        _layers.Clear();

        _ = _document.Transaction(transactionManagerWrapper =>
        {
            var transactionManager = transactionManagerWrapper.Unwrap();

            var database = _document.Database.Unwrap();

            using var layerTable = (LayerTable)transactionManager.GetObject(database.LayerTableId, OpenMode.ForRead);

            foreach (var layerId in layerTable)
            {
                var cadLayer = (LayerTableRecord)transactionManager.GetObject(layerId, OpenMode.ForRead);

                var linePatternId = new ObjectId(cadLayer.LinetypeObjectId);

                var linePattern = _linePatternCache.GetById(linePatternId);

                var layer = new AutocadLayerWrapper(cadLayer, linePattern);

                var layerName = layer.Name;
                if (_layers.ContainsKey(layerName) == false)
                    _layers[layerName] = layer;
            }

            return true;
        });
    }

    /// <inheritdoc/>
    public bool Exists(IAutocadLayer layer)
    {
        return _layers.ContainsKey(layer.Name);
    }

    /// <inheritdoc/>
    public bool TryGetByName(string name, out IAutocadLayer? layer)
    {
        var containsLayer = _layers.TryGetValue(name, out layer);

        return containsLayer;
    }

    /// <inheritdoc/>
    public IAutocadLayer GetByNameOrDefault(string name)
    {
        return this.TryGetByName(name, out var layer) ? layer! : this.GetDefault();
    }

    /// <inheritdoc/>
    public void TryAddLayer(IColor color, string name)
    {
        if (_layers.ContainsKey(name) == false)
        {
            var layer = this.CreateLayer(color, name);

            _layers[layer.Name] = layer;

            var layerAddedEventArgs = new LayerAddedEventArgs(layer);

            LayerAdded?.Invoke(this, layerAddedEventArgs);
        }
    }

    /// <inheritdoc/>
    public IAutocadLayer GetDefault()
    {
        _ = _layers.TryGetValue(_defaultLayerName, out var layer);

        return layer!;
    }

    /// <inheritdoc/>
    public IList<IAutocadLayer> GetLayers(IList<string> names)
    {
        var layers = new List<IAutocadLayer>();

        foreach (var name in names)
        {
            if (this.TryGetByName(name, out var layer))
                layers.Add(layer!);
        }

        return layers;
    }

    /// <inheritdoc/>
    public IEnumerator<IAutocadLayer> GetEnumerator()
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
            foreach (var layer in _layers.Values)
                layer.Dispose();

            _disposed = true;
        }
    }
}