using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Core.State;
using Rhino.Inside.AutoCAD.Interop;
using Assembly = Autodesk.Civil.DatabaseServices.Assembly;
using CivilSubassembly = Autodesk.Civil.DatabaseServices.Subassembly;
using Surface = Autodesk.Civil.DatabaseServices.Surface;

namespace Rhino.Inside.AutoCAD.Civil.Interop.Naming;

/// <summary>
/// Provides methods for generating unique names for Civil 3D objects.
/// </summary>
public static class AutoNamer
{
    /// <summary>
    /// Generates a unique surface name by checking existing surfaces in the database.
    /// </summary>
    /// <param name="autocadDatabaseWrapper">The database to check for existing surface names.</param>
    /// <param name="prefix">The prefix to use for the surface name.</param>
    /// <returns>A unique surface name in the format "{prefix}_TINSurface_NNN".</returns>
    public static string GenerateUniqueSurfaceName(IAutocadDatabase autocadDatabaseWrapper, string prefix)
    {
        var existingNames = GetAllSurfaceNames(autocadDatabaseWrapper);

        var baseName = $"{prefix}_TINSurface";
        var counter = 1;
        string candidateName;

        do
        {
            candidateName = $"{baseName}_{counter:D3}";
            counter++;
        } while (existingNames.Contains(candidateName));

        return candidateName;
    }

    /// <summary>
    /// Generates a unique volume surface name by checking existing surfaces in the database.
    /// </summary>
    /// <param name="autocadDatabaseWrapper">The database to check for existing surface names.</param>
    /// <param name="prefix">The prefix to use for the volume surface name.</param>
    /// <returns>A unique volume surface name in the format "{prefix}_VolSurface_NNN".</returns>
    public static string GenerateUniqueVolumeSurfaceName(IAutocadDatabase autocadDatabaseWrapper, string prefix)
    {
        var existingNames = GetAllSurfaceNames(autocadDatabaseWrapper);

        var baseName = $"{prefix}_VolSurface";
        var counter = 1;
        string candidateName;

        do
        {
            candidateName = $"{baseName}_{counter:D3}";
            counter++;
        } while (existingNames.Contains(candidateName));

        return candidateName;
    }

    /// <summary>
    /// Generates a unique subassembly name by checking existing subassemblies in the assembly.
    /// </summary>
    /// <param name="transactionManager">The transaction manager for database access.</param>
    /// <param name="assemblyId">The ObjectId of the assembly to check for existing subassembly names.</param>
    /// <param name="prefix">The prefix to use for the subassembly name.</param>
    /// <returns>A unique subassembly name in the format "{prefix}Subassembly_NNN".</returns>
    public static string GenerateUniqueSubassemblyName(IAutocadTransactionManager transactionManager, ObjectId assemblyId, string prefix)
    {
        var existingNames = GetAllSubassemblyNames(transactionManager, assemblyId);

        var baseName = $"{prefix}Subassembly";
        var counter = 1;
        string candidateName;

        do
        {
            candidateName = $"{baseName}_{counter:D3}";
            counter++;
        } while (existingNames.Contains(candidateName));

        return candidateName;
    }

    /// <summary>
    /// Generates a unique alignment name by checking existing alignments in the document.
    /// </summary>
    /// <param name="autocadTransactionManager">The transaction manager for database access.</param>
    /// <param name="prefix">The prefix to use for the alignment name.</param>
    /// <returns>A unique alignment name in the format "{prefix}Alignment_NNN".</returns>
    public static string GenerateUniqueAlignmentName(IAutocadTransactionManager autocadTransactionManager, string prefix)
    {
        var existingNames = GetAllAlignmentNames(autocadTransactionManager);

        var baseName = $"{prefix}Alignment";
        var counter = 1;
        string candidateName;

        do
        {
            candidateName = $"{baseName}_{counter:D3}";
            counter++;
        } while (existingNames.Contains(candidateName));

        return candidateName;
    }

    /// <summary>
    /// Generates a unique feature line name by checking all sited and siteless feature lines.
    /// </summary>
    /// <param name="transactionManager">The transaction manager for database access.</param>
    /// <param name="prefix">The prefix to use for the feature line name.</param>
    /// <returns>A unique feature line name in the format "{prefix}FeatureLine_NNN".</returns>
    public static string GenerateUniqueFeatureLineName(IAutocadTransactionManager transactionManager, string prefix)
    {
        var existingNames = GetAllFeatureLineNames(transactionManager);

        var baseName = $"{prefix}FeatureLine";
        var counter = 1;
        string candidateName;

        do
        {
            candidateName = $"{baseName}_{counter:D3}";
            counter++;
        } while (existingNames.Contains(candidateName));

        return candidateName;
    }

    /// <summary>
    /// Gets all existing feature line names, sited and siteless, from the database.
    /// </summary>
    private static HashSet<string> GetAllFeatureLineNames(IAutocadTransactionManager transactionManager)
    {
        var existingNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (ApplicationState.IsShuttingDown)
            return existingNames;

        var database = transactionManager.AutocadDatabase.Unwrap();
        var civilDoc = CivilDocument.GetCivilDocument(database);

        if (civilDoc == null)
            return existingNames;

        try
        {
            var transaction = transactionManager.Unwrap();

            foreach (var featureLineId in CivilFeatureLineCollector.GetAllFeatureLineIds(civilDoc, transaction))
            {
                if (transaction.GetObject(featureLineId, OpenMode.ForRead) is FeatureLine featureLine)
                {
                    existingNames.Add(featureLine.Name);
                }
            }
        }
        catch (System.Exception)
        {
            // Return whatever names were collected
        }

        return existingNames;
    }

    /// <summary>
    /// Gets all existing surface names (TIN and Volume) from the database.
    /// </summary>
    /// <param name="autocadDatabaseWrapper">The database to check.</param>
    /// <returns>A set of all existing surface names.</returns>
    private static HashSet<string> GetAllSurfaceNames(IAutocadDatabase autocadDatabaseWrapper)
    {
        var existingNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (ApplicationState.IsShuttingDown)
            return existingNames;

        var database = autocadDatabaseWrapper.Unwrap();
        var civilDoc = CivilApplication.ActiveDocument;

        if (civilDoc == null)
            return existingNames;

        try
        {
            foreach (ObjectId surfaceId in civilDoc.GetSurfaceIds())
            {
                if (surfaceId.IsValid && !surfaceId.IsNull && !surfaceId.IsErased)
                {
                    using var transaction = database.TransactionManager.StartTransaction();
                    var surface = transaction.GetObject(surfaceId, OpenMode.ForRead) as Surface;
                    if (surface != null)
                    {
                        existingNames.Add(surface.Name);
                    }

                    transaction.Commit();
                }
            }
        }
        catch (System.Exception)
        {
            // Handle disposal during iteration - return whatever names we collected
        }

        return existingNames;
    }

    /// <summary>
    /// Gets all existing alignment names from the Civil 3D document.
    /// </summary>
    /// <param name="civilDoc">The Civil 3D document to check.</param>
    /// <returns>A set of all existing alignment names.</returns>
    private static HashSet<string> GetAllAlignmentNames(IAutocadTransactionManager transactionManager)
    {
        var database = transactionManager.AutocadDatabase.Unwrap();
        var civilDoc = CivilDocument.GetCivilDocument(database);

        var existingNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (ApplicationState.IsShuttingDown)
            return existingNames;

        if (civilDoc == null)
            return existingNames;

        try
        {
            var alignmentIds = civilDoc.GetAlignmentIds();

            using var transaction = database.TransactionManager.StartTransaction();
            foreach (ObjectId alignmentId in alignmentIds)
            {
                if (alignmentId.IsValid && !alignmentId.IsNull && !alignmentId.IsErased)
                {
                    var alignment = transaction.GetObject(alignmentId, OpenMode.ForRead) as Alignment;
                    if (alignment != null)
                    {
                        existingNames.Add(alignment.Name);
                    }
                }
            }
            transaction.Commit();
        }
        catch (System.Exception)
        {
            // Handle disposal during iteration - return whatever names we collected
        }

        return existingNames;
    }

    /// <summary>
    /// Gets all existing subassembly names from the specified assembly.
    /// </summary>
    /// <param name="transactionManager">The transaction manager for database access.</param>
    /// <param name="assemblyId">The ObjectId of the assembly to check.</param>
    /// <returns>A set of all existing subassembly names in the assembly.</returns>
    private static HashSet<string> GetAllSubassemblyNames(IAutocadTransactionManager transactionManager, ObjectId assemblyId)
    {
        var existingNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (ApplicationState.IsShuttingDown)
            return existingNames;

        if (assemblyId.IsNull || !assemblyId.IsValid || assemblyId.IsErased)
            return existingNames;

        try
        {
            var transaction = transactionManager.Unwrap();
            var assembly = transaction.GetObject(assemblyId, OpenMode.ForRead) as Assembly;

            if (assembly == null)
                return existingNames;

            foreach (var group in assembly.Groups)
            {
                foreach (ObjectId subassemblyId in group.GetSubassemblyIds())
                {
                    if (subassemblyId.IsValid && !subassemblyId.IsNull && !subassemblyId.IsErased)
                    {
                        var subassembly = transaction.GetObject(subassemblyId, OpenMode.ForRead) as CivilSubassembly;
                        if (subassembly != null)
                        {
                            existingNames.Add(subassembly.Name);
                        }
                    }
                }
            }
        }
        catch (System.Exception)
        {
            // Handle disposal during iteration - return whatever names we collected
        }

        return existingNames;
    }
}
