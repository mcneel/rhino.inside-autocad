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
/// in Properties/Resources.resx as Bitmaps, which the compiler stores as BinaryFormatter
/// blobs. The generated Resources class reads them through a stock
/// <see cref="ResourceManager"/>, which needs BinaryFormatter and so fails from .NET 9 on.
/// <see cref="Attach"/> places this manager in the generated class's cache before its first
/// use, so every lookup goes through <see cref="LegacyResourceSet"/> instead. Ported from
/// Rhino.Inside.Revit (mcneel/rhino.inside-revit@d8e0edc).
/// </remarks>
/// <seealso cref="LegacyResourceSet"/>
public sealed class LegacyResourceManager : ResourceManager
{
    private const string _legacyCoreLibraryName = ApplicationConstants.LegacyCoreLibraryName;

    private const string _propertiesResourcesTypeNameFormat = ApplicationConstants.PropertiesResourcesTypeNameFormat;

    private const string _propertiesNamespaceSuffix = ApplicationConstants.PropertiesNamespaceSuffix;

    private const string _propertiesResourcesTypeName = ApplicationConstants.PropertiesResourcesTypeName;

    private const string _resourceManagerFieldName = ApplicationConstants.ResourceManagerFieldName;

    private const BindingFlags _resourceManagerFieldFlags = BindingFlags.Static | BindingFlags.NonPublic;

    private readonly Dictionary<string, ResourceSet> _resourceSets = [];

    /// <summary>
    /// Constructs a new <see cref="LegacyResourceManager"/>.
    /// </summary>
    private LegacyResourceManager(string baseName, Assembly assembly) : base(baseName, assembly)
    {
    }

    /// <summary>
    /// Places a <see cref="LegacyResourceManager"/> in the generated Properties.Resources
    /// class of the assembly, if it was built for .NET Framework and the class has not yet
    /// created its own manager. Otherwise does nothing.
    /// </summary>
    /// <remarks>
    /// Called from an <see cref="AppDomain.AssemblyLoad"/> handler, so it never throws: an
    /// assembly this cannot patch is left exactly as it was loaded.
    /// </remarks>
    public static void Attach(Assembly assembly)
    {
        if (assembly.ReflectionOnly || assembly.IsDynamic)
            return;

        try
        {
            var isLegacyAssembly = assembly.GetReferencedAssemblies()
                .Any(reference => reference.Name == _legacyCoreLibraryName);

            if (isLegacyAssembly == false)
                return;

            var resourcesType = FindResourcesType(assembly);

            var resourceManagerField = resourcesType?.GetField(_resourceManagerFieldName, _resourceManagerFieldFlags);

            if (resourcesType?.FullName is not string baseName || resourceManagerField is null)
                return;

            if (resourceManagerField.FieldType != typeof(ResourceManager) ||
                resourceManagerField.GetValue(null) is not null)
                return;

            var resourceManager = new LegacyResourceManager(baseName, assembly);

            resourceManagerField.SetValue(null, resourceManager);
        }
        catch (Exception)
        {
            // Left unpatched, the assembly behaves exactly as it would without this class.
        }
    }

    /// <summary>
    /// Returns the class Visual Studio generated from Properties/Resources.resx, or null if
    /// the assembly has none.
    /// </summary>
    /// <remarks>
    /// Tries the assembly name as the root namespace first, then scans for the class, since
    /// older plugins often renamed one without the other.
    /// </remarks>
    private static Type? FindResourcesType(Assembly assembly)
    {
        var resourcesTypeName = string.Format(_propertiesResourcesTypeNameFormat, assembly.GetName().Name);

        var resourcesType = assembly.GetType(resourcesTypeName);

        if (resourcesType is not null)
            return resourcesType;

        return GetLoadableTypes(assembly).FirstOrDefault(IsResourcesType);
    }

    /// <summary>
    /// Returns true if the type looks like a generated Properties.Resources class.
    /// </summary>
    private static bool IsResourcesType(Type type)
    {
        return type.Name == _propertiesResourcesTypeName &&
               type.Namespace?.EndsWith(_propertiesNamespaceSuffix, StringComparison.Ordinal) == true &&
               type.GetField(_resourceManagerFieldName, _resourceManagerFieldFlags) is not null;
    }

    /// <summary>
    /// Returns the types of the assembly which load, skipping those whose dependencies are
    /// missing from this process.
    /// </summary>
    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException e)
        {
            return e.Types.OfType<Type>();
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

        return null;
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
