using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using CivilDocument = Autodesk.Civil.ApplicationServices.CivilDocument;

namespace Rhino.Inside.AutoCAD.Civil.Interop;

/// <summary>
/// Collects Civil 3D Feature Line ids from a drawing.
/// </summary>
/// <remarks>
/// Feature lines live either in a site or in the siteless collection, and Civil 3D has no
/// single collection holding both.
/// </remarks>
public static class CivilFeatureLineCollector
{
    /// <summary>
    /// Gets the ids of every siteless feature line and every feature line in every site.
    /// </summary>
    /// <param name="civilDocument">The Civil document to read.</param>
    /// <param name="transactionManager">The transaction manager of an open transaction, used to open the sites.</param>
    public static List<ObjectId> GetAllFeatureLineIds(CivilDocument civilDocument, TransactionManager transactionManager)
    {
        var ids = GetSitelessFeatureLineIds(civilDocument);

        foreach (ObjectId siteId in civilDocument.GetSiteIds())
        {
            ids.AddRange(GetSiteFeatureLineIds(siteId, transactionManager));
        }

        return ids;
    }

    /// <summary>
    /// Gets the ids of the feature lines that do not belong to a site.
    /// </summary>
    /// <param name="civilDocument">The Civil document to read.</param>
    public static List<ObjectId> GetSitelessFeatureLineIds(CivilDocument civilDocument)
    {
        return ToValidIds(civilDocument.GetSitelessFeatureLineIds());
    }

    /// <summary>
    /// Gets the ids of the feature lines in one site.
    /// </summary>
    /// <param name="siteId">The site to read.</param>
    /// <param name="transactionManager">The transaction manager of an open transaction, used to open the site.</param>
    public static List<ObjectId> GetSiteFeatureLineIds(ObjectId siteId, TransactionManager transactionManager)
    {
        if (siteId.IsNull || siteId.IsErased)
            return new List<ObjectId>();

        if (transactionManager.GetObject(siteId, OpenMode.ForRead) is not Site site)
            return new List<ObjectId>();

        return ToValidIds(site.GetFeatureLineIds());
    }

    private static List<ObjectId> ToValidIds(ObjectIdCollection ids)
    {
        var result = new List<ObjectId>();

        foreach (ObjectId id in ids)
        {
            if (!id.IsNull && !id.IsErased)
                result.Add(id);
        }

        return result;
    }
}
