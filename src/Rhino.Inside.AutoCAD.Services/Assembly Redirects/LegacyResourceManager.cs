using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Rhino.Inside.AutoCAD.Services;

/// <summary>
/// A <see cref="ResourceManager"/> which serves a .NET Framework assembly's resources on
/// runtimes without BinaryFormatter.
/// </summary>
/// <remarks>
/// Grasshopper plugins built from the classic .NET Framework template embed their icons
/// in .resx files as Bitmaps, which the compiler stores as BinaryFormatter blobs. The
/// class generated from each .resx reads them through a stock <see cref="ResourceManager"/>,
/// which needs BinaryFormatter and so fails from .NET 9 on. <see cref="Attach"/> replaces
/// the manager cached in every such generated class - C# Properties.Resources, extra .resx
/// files and VB.NET My.Resources alike - so every lookup goes through
/// <see cref="LegacyResourceSet"/> instead. Ported from Rhino.Inside.Revit
/// (mcneel/rhino.inside-revit@d8e0edc).
/// <para>
/// Grasshopper swallows exceptions thrown by component icon getters, so a failure here
/// shows only as a blank icon. Debug builds therefore throw on failure; release builds log
/// it and leave the assembly as it was.
/// </para>
/// </remarks>
/// <seealso cref="LegacyResourceSet"/>
public sealed class LegacyResourceManager : ResourceManager
{
    private const string _legacyCoreLibraryName = ApplicationConstants.LegacyCoreLibraryName;

    private const string _resourceManagerFieldName = ApplicationConstants.ResourceManagerFieldName;

    private const string _resourceManagerPropertyName = ApplicationConstants.ResourceManagerPropertyName;

    private const string _legacyResourcesNotFound = MessageConstants.LegacyResourcesNotFound;

    private const string _legacyResourcesAttachFailed = MessageConstants.LegacyResourcesAttachFailed;

    private const BindingFlags _resourceManagerFieldFlags = BindingFlags.Static | BindingFlags.NonPublic;

    private const BindingFlags _resourceManagerPropertyFlags =
        BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

    private readonly Dictionary<string, ResourceSet> _resourceSets = [];

    private bool _missingResourcesReported;

    /// <summary>
    /// Constructs a new <see cref="LegacyResourceManager"/>.
    /// </summary>
    private LegacyResourceManager(string baseName, Assembly assembly) : base(baseName, assembly)
    {
    }

    /// <summary>
    /// Places a <see cref="LegacyResourceManager"/> in every class generated from a .resx
    /// file in the assembly, if it was built for .NET Framework. Classes which have already
    /// created their own manager are left alone.
    /// </summary>
    /// <remarks>
    /// Called from an <see cref="AppDomain.AssemblyLoad"/> handler. In release builds it
    /// never throws: a class this cannot patch is logged and left exactly as it was loaded.
    /// In debug builds the failure is rethrown so it surfaces at the plugin load.
    /// </remarks>
    public static void Attach(Assembly assembly)
    {
        if (assembly.ReflectionOnly || assembly.IsDynamic)
            return;

        Type[] types;

        try
        {
            var isLegacyAssembly = assembly.GetReferencedAssemblies()
                .Any(reference => reference.Name == _legacyCoreLibraryName);

            if (isLegacyAssembly == false)
                return;

            types = GetLoadableTypes(assembly);
        }
        catch (Exception e)
        {
            ReportAttachFailed(assembly.FullName, e);

            return;
        }

        foreach (var type in types)
        {
            AttachToType(assembly, type);
        }
    }

    /// <summary>
    /// Places a <see cref="LegacyResourceManager"/> in the type if it is a class generated
    /// from a .resx file which has not yet created its own manager. Otherwise does nothing.
    /// </summary>
    /// <remarks>
    /// The base name is read from the manager the generated class creates itself, since it
    /// is the name the resources were embedded under. It differs from the class's full name
    /// when the class was moved to another namespace after generation, and for VB.NET
    /// My.Resources, whose class is &lt;RootNamespace&gt;.My.Resources.Resources but whose
    /// resources are &lt;RootNamespace&gt;.Resources. Constructing that manager reads no
    /// resources, so it does not need BinaryFormatter.
    /// </remarks>
    private static void AttachToType(Assembly assembly, Type type)
    {
        try
        {
            var resourceManagerField = type.GetField(_resourceManagerFieldName, _resourceManagerFieldFlags);

            if (resourceManagerField is null ||
                resourceManagerField.FieldType != typeof(ResourceManager) ||
                resourceManagerField.GetValue(null) is not null)
                return;

            var resourceManagerProperty = type.GetProperty(_resourceManagerPropertyName, _resourceManagerPropertyFlags);

            if (resourceManagerProperty?.GetValue(null) is not ResourceManager generatedResourceManager)
                return;

            var resourceManager = new LegacyResourceManager(generatedResourceManager.BaseName, assembly);

            resourceManagerField.SetValue(null, resourceManager);
        }
        catch (Exception e)
        {
            ReportAttachFailed(type.FullName, e);
        }
    }

    /// <summary>
    /// Logs that a type or assembly could not be patched. Rethrows in debug builds.
    /// </summary>
    private static void ReportAttachFailed(string? name, Exception exception)
    {
        var message = string.Format(_legacyResourcesAttachFailed, name);

        if (LoggerService.IsInitialized)
            LoggerService.Instance.LogError(exception, message);

#if DEBUG
        throw new InvalidOperationException(message, exception);
#endif
    }

    /// <summary>
    /// Returns the types of the assembly which load, skipping those whose dependencies are
    /// missing from this process.
    /// </summary>
    private static Type[] GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException e)
        {
            return e.Types.OfType<Type>().ToArray();
        }
    }

    /// <summary>
    /// Returns the <see cref="LegacyResourceSet"/> for the culture, reading it from the main
    /// assembly on first request. Falls back to the invariant resources, which is where a
    /// Properties/Resources.resx lives.
    /// </summary>
    protected override ResourceSet? InternalGetResourceSet(CultureInfo culture,
        bool createIfNotExists, bool tryParents)
    {
        lock (_resourceSets)
        {
            if (_resourceSets.TryGetValue(culture.Name, out var cachedResourceSet))
                return cachedResourceSet;
        }

        if (createIfNotExists == false || this.MainAssembly is null)
            return null;

        CultureInfo[] candidateCultures = [culture, CultureInfo.InvariantCulture];

        foreach (var candidateCulture in candidateCultures)
        {
            var resourceFileName = this.GetResourceFileName(candidateCulture);

            var stream = this.MainAssembly.GetManifestResourceStream(resourceFileName);

            if (stream is null)
                continue;

            var resourceSet = new LegacyResourceSet(stream);

            return this.CacheResourceSet(culture, resourceSet);
        }

        this.ReportResourcesNotFound();

        return null;
    }

    /// <summary>
    /// Logs, once per manager, that the assembly holds no resources under the base name.
    /// Throws in debug builds.
    /// </summary>
    private void ReportResourcesNotFound()
    {
        var message = string.Format(_legacyResourcesNotFound, this.BaseName, this.MainAssembly?.FullName);

        lock (_resourceSets)
        {
            if (_missingResourcesReported == false && LoggerService.IsInitialized)
                LoggerService.Instance.LogError(message);

            _missingResourcesReported = true;
        }

#if DEBUG
        throw new MissingManifestResourceException(message);
#endif
    }

    /// <summary>
    /// Caches the resource set against the culture and returns it. If another thread
    /// cached one first, disposes this one and returns theirs.
    /// </summary>
    private ResourceSet CacheResourceSet(CultureInfo culture, ResourceSet resourceSet)
    {
        lock (_resourceSets)
        {
            if (_resourceSets.TryGetValue(culture.Name, out var cachedResourceSet))
            {
                resourceSet.Dispose();

                return cachedResourceSet;
            }

            _resourceSets.Add(culture.Name, resourceSet);

            return resourceSet;
        }
    }
}
