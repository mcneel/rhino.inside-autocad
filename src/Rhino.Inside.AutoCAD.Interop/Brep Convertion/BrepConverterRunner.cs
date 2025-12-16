using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IBrepConverterRunner"/>
public class BrepConverterRunner : IBrepConverterRunner
{
    private readonly Queue<IBrepConverterRequest> _requests = new Queue<IBrepConverterRequest>();

    /// <summary>
    /// Converts a <see cref="RhinoBrep"/> to an array of AutoCAD <see cref="CadSolid3d"/>s.
    /// Typically, this is just a single solid representing the Brep, but depending on
    /// the import processing it could be multiple solids.
    /// </summary>
    /// <remarks>
    /// We need to use a AutoCAD document to import the Rhino brep temporarily. It is
    /// deleted after the import so which document we use is not important.
    /// </remarks>
    private IBrepConverterResult ToAutoCadType(IBrepConverterRequest brepRequest)
    {
        var activeDocument = Application.DocumentManager.MdiActiveDocument;

        var editor = activeDocument.Editor;

        var addedObjects = new List<Solid3d>();

        try
        {
            var tempFolder = Path.GetTempPath();

            var pathLocation = $@"{tempFolder}RhinoInsideAutocad\Converters\";

            Directory.CreateDirectory(pathLocation);

            var rhinoFilePath = $@"{pathLocation}rhinoToAutoCad.3dm";

            var result = Rhino.FileIO.File3dm.WriteOneObject(rhinoFilePath, brepRequest.BrepToConvert);

            if (File.Exists(rhinoFilePath) == false || result == false)
            {
                return new BrepConverterResult(addedObjects);
            }

            editor.Command("._IMPORT", rhinoFilePath, "");

            var selectLast = editor.SelectLast();

            var selectedObjects = selectLast?.Value;

            var transaction = activeDocument.Database.TransactionManager.StartTransaction();

            for (var index = 0; index < selectedObjects!.Count; index++)
            {
                var selection = selectedObjects![index];
                var importedObject =
                    transaction.GetObject(selection.ObjectId, OpenMode.ForWrite);

                var clone = importedObject.Clone();

                if (clone is not Solid3d solid3d) continue;

                addedObjects.Add(solid3d);

                importedObject.Erase(true);
            }

            transaction.Commit();
        }
        catch (System.Exception ex)
        {
            editor.WriteMessage($"\nError Converting brep: {ex.Message}");
        }

        return new BrepConverterResult(addedObjects);
    }

    /// <inheritdoc />
    public void Run()
    {
        while (_requests.Count > 0)
        {
            var request = _requests.Dequeue();

            var result = this.ToAutoCadType(request);

            _ = request.Callback.Invoke(result);
        }
    }

    /// <inheritdoc />
    public void EnqueueRequest(IBrepConverterRequest request)
    {
        _requests.Enqueue(request);
    }
}