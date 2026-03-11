using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using CadColor = Autodesk.AutoCAD.Colors.Color;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A class which represents a register for handling
/// <see cref="IAutocadLayerTableRecord"/>s.
/// </summary>
public class LayerRegister : RegisterBase<IAutocadLayerTableRecord>, ILayerRegister
{
    private readonly string _defaultLayerName = InteropConstants.DefaultLayerName;

    /// <summary>
    /// Constructs a new <see cref="LayerRegister"/>.
    /// </summary>
    public LayerRegister(IAutocadDocument document) : base(document)
    {
    }

    /// <summary>
    /// Creates a new layer in the active document and returns the <see cref="IAutocadLayerTableRecord"/>.
    /// </summary>
    private IAutocadLayerTableRecord CreateLayer(IColor color, string name)
    {
        using var documentLock = _document.Unwrap().LockDocument();

        var layerWrapper = _document.Transaction(transactionManagerWrapper =>
         {
             var transactionManager = transactionManagerWrapper.Unwrap();

             var layerTableRecord = new LayerTableRecord
             {
                 Name = name,
                 Color = CadColor.FromRgb(color.Red, color.Green, color.Blue)
             };

             using var layerTable = (LayerTable)transactionManager.GetObject(
                 _document.Database.LayerTableId.Unwrap(), OpenMode.ForWrite);

             layerTable.Add(layerTableRecord);

             transactionManager.AddNewlyCreatedDBObject(layerTableRecord, true);

             return new AutocadLayerTableRecordWrapper(layerTableRecord);
         });

        _document.Regenerate();

        return layerWrapper;
    }

    /// <summary>
    /// Populates this <see cref="ILayerRegister"/> with <see cref="IAutocadLayerTableRecord"/>s
    /// from the active <see cref="IAutocadDocument"/>.
    /// </summary>
    public override void Repopulate()
    {
        _objects.Clear();

        _ = _document.Transaction(transactionManagerWrapper =>
        {
            var transactionManager = transactionManagerWrapper.Unwrap();

            using var layerTable = (LayerTable)transactionManager.GetObject(
                _document.Database.LayerTableId.Unwrap(), OpenMode.ForRead);

            foreach (var layerId in layerTable)
            {
                var cadLayer = (LayerTableRecord)transactionManager.GetObject(layerId, OpenMode.ForRead);

                var layer = new AutocadLayerTableRecordWrapper(cadLayer);

                _objects.Add(layer.Id, layer);

            }

            return true;
        });
    }

    /// <inheritdoc/>
    public bool TryAddLayer(IColor color, string name, out IAutocadLayerTableRecord layer)
    {
        if (this.TryGetByName(name, out _) == false)
        {
            layer = this.CreateLayer(color, name);

            _objects.Add(layer.Id, layer);

            return true;
        }

        layer = this.GetDefault();
        return false;
    }

    /// <inheritdoc/>
    public IAutocadLayerTableRecord GetDefault()
    {
        _ = this.TryGetByName(_defaultLayerName, out var layer);

        return layer!;
    }
}
