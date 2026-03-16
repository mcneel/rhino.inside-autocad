using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IAutocadDocumentId"/>
public class AutocadDocumentId : IAutocadDocumentId
{
    private const string _applicationName = InteropConstants.ApplicationName;
    private const short _applicationNameKey = XRecordKeys.ApplicationNameKey;
    private const short _documentIdKey = XRecordKeys.DocumentIdKey;

    public Guid Id { get; }

    public AutocadDocumentId(IAutocadDocument document)
    {
        this.RegisterApplication(document);

        if (this.TryGetExistingId(document, out var documentId) == false)
        {
            documentId = this.CreateNewId(document);
        }

        this.Id = documentId;
    }

    /// <summary>
    /// Retrieves the document's unique identifier from model space XData if it already
    /// exists, if not it returns false and an empty guid.
    /// </summary>
    private bool TryGetExistingId(IAutocadDocument document, out Guid id)
    {
        id = document.Transaction(transactionManager =>
        {
            var blockModelSpace = transactionManager.GetModelSpace().Unwrap();

            var xData = blockModelSpace.XData == null
                ? new ResultBuffer()
                : blockModelSpace.XData;

            var idKey = (short)_documentIdKey;

            var typedValues = xData.AsArray().Where(v => v.TypeCode == idKey);

            var documentId = Guid.Empty;
            foreach (var typedValue in typedValues)
            {
                if (Guid.TryParse(typedValue.Value.ToString(), out documentId))
                    break;
            }

            return documentId;

        });

        return id.Equals(Guid.Empty);
    }

    /// <summary>
    /// Creates a document's unique identifier and stores it in the model space XData.
    /// </summary>
    private Guid CreateNewId(IAutocadDocument document)
    {
        return document.Transaction(transactionManager =>
        {
            var blockModelSpace = transactionManager.GetModelSpace().Unwrap();

            var xData = blockModelSpace.XData == null
                ? new ResultBuffer()
                : blockModelSpace.XData;

            var idKey = (short)_documentIdKey;

            var documentId = Guid.NewGuid();

            xData.Add(new Autodesk.AutoCAD.DatabaseServices.TypedValue((short)_applicationNameKey, _applicationName));
            xData.Add(new Autodesk.AutoCAD.DatabaseServices.TypedValue(idKey, documentId.ToString()));

            blockModelSpace.UpgradeOpen();

            blockModelSpace.XData = xData;

            transactionManager.SaveDatabase(document);

            return documentId;

        });

    }

    /// <summary>
    /// Registers Rhino.Inside.AutoCAD in the <see cref="RegAppTable"/>.
    /// </summary>
    /// <remarks>
    /// Required before writing XData to the database.
    /// </remarks>
    private void RegisterApplication(IAutocadDocument document)
    {
        document.Transaction(transactionManager =>
        {
            var transaction = transactionManager.Unwrap();

            var regAppTableId = transactionManager.RegAppTableId.Unwrap();

            var regAppTable = (RegAppTable)transaction.GetObject(regAppTableId, OpenMode.ForRead);

            if (regAppTable.Has(_applicationName)) return true;

            regAppTable.UpgradeOpen();

            var regAppTableRecord = new RegAppTableRecord();

            regAppTableRecord.Name = _applicationName;

            regAppTable.Add(regAppTableRecord);

            transaction.AddNewlyCreatedDBObject(regAppTableRecord, true);

            return true;

        });
    }
}