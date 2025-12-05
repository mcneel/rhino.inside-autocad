using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Rhino.Inside.AutoCAD.Applications;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using CadEntity = Autodesk.AutoCAD.DatabaseServices.Entity;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <inheritdoc cref="IAutocadObjectPicker"/>
public class AutocadObjectPicker : IAutocadObjectPicker
{
    private readonly IAutocadDocument _document;

    /// <summary>
    /// Constructs a new <see cref="IAutocadObjectPicker"/> instance.
    /// </summary>
    public AutocadObjectPicker()
    {
        var rhinoInsideApplication = RhinoInsideAutoCadExtension.Application!;

        var activeDocument = rhinoInsideApplication.RhinoInsideManager.AutoCadInstance.ActiveDocument;

        _document = activeDocument;
    }

    /// <inheritdoc/>
    public IEntity? PickObject(ISelectionFilter filter, string message)
    {
        Application.MainWindow.Focus();

        return _document.Transaction((transactionManager) =>
        {
            var entities = new List<IEntity>();
            var options = new PromptSelectionOptions()
            {
                AllowDuplicates = false,
                AllowSubSelections = true,
                ForceSubSelections = false,
                MessageForAdding = message,
                MessageForRemoval = message,
                SingleOnly = true,
            };

            var selectionFilter = filter.Unwrap();

            var promptSelectionResult = _document.Unwrap().Editor.GetSelection(options, selectionFilter);

            if (promptSelectionResult.Status != PromptStatus.OK) return null;

            var transaction = transactionManager.Unwrap();

            var selectionSet = promptSelectionResult.Value;

            foreach (SelectedObject selectedObject in selectionSet)
            {
                if (selectedObject == null) continue;

                var entity = transaction.GetObject(selectedObject.ObjectId,
                    OpenMode.ForRead) as CadEntity;

                var wrappedEntity = new EntityWrapper(entity);

                entities.Add(wrappedEntity);
            }

            return entities.FirstOrDefault();

        });
    }

    /// <inheritdoc/>
    public IList<IEntity> PickObjects(ISelectionFilter filter, string message)
    {
        Application.MainWindow.Focus();

        return _document.Transaction((transactionManager) =>
        {
            var entities = new List<IEntity>();
            var options = new PromptSelectionOptions()
            {
                AllowDuplicates = false,
                AllowSubSelections = true,
                ForceSubSelections = false,
                MessageForAdding = message,
                MessageForRemoval = message,
                SingleOnly = false,
            };

            var selectionFilter = filter.Unwrap();

            var promptSelectionResult = _document.Unwrap().Editor.GetSelection(options, selectionFilter);

            if (promptSelectionResult.Status != PromptStatus.OK) return entities;

            var transaction = transactionManager.Unwrap();

            var selectionSet = promptSelectionResult.Value;

            foreach (SelectedObject selectedObject in selectionSet)
            {
                if (selectedObject == null) continue;

                var entity = transaction.GetObject(selectedObject.ObjectId,
                    OpenMode.ForRead) as CadEntity;

                var wrappedEntity = new EntityWrapper(entity);

                entities.Add(wrappedEntity);
            }

            return entities;

        });
    }
}