using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using CadEntity = Autodesk.AutoCAD.DatabaseServices.Entity;
using Entity = Rhino.Inside.AutoCAD.Interop.Entity;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary.Params_Tab.ObjectPicker;

public class AutocadObjectPicker
{
    private IAutoCadDocument _document;
    public AutocadObjectPicker(IAutoCadDocument document)
    {
        _document = document;
    }

    public IEntity? PickObject(ISelectionFilter filter, string message)
    {

        return _document.Transaction((transactionManager) =>
        {
            var entities = new List<IEntity>();
            var options = new PromptSelectionOptions()
            {
                AllowDuplicates = false,
                AllowSubSelections = false,
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

                var wrappedEntity = new Entity(entity);

                entities.Add(wrappedEntity);
            }

            return entities.FirstOrDefault();

        });
    }

    public IList<IEntity> PickObjects(ISelectionFilter filter, string message)
    {

        return _document.Transaction((transactionManager) =>
        {
            var entities = new List<IEntity>();
            var options = new PromptSelectionOptions()
            {
                AllowDuplicates = false,
                AllowSubSelections = false,
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

                var wrappedEntity = new Entity(entity);

                entities.Add(wrappedEntity);
            }

            return entities;

        });
    }
}