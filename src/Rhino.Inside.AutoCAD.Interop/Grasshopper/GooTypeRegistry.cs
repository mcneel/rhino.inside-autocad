using Autodesk.AutoCAD.DatabaseServices;
using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A registry that discovers and caches Goo type factories for AutoCAD entity conversion.
/// Uses convention-based reflection to automatically register all types inheriting from
/// GH_AutocadGeometricGoo.
/// </summary>
public class GooTypeRegistry
{
    private const string _gooBaseTypeName = InteropConstants.GooBaseTypeName;
    private const string _gooBaseTypeNotFound = MessageConstants.GooBaseTypeNotFound;

    private static GooTypeRegistry? _instance;

    /// <summary>
    /// Gets the singleton instance of the registry.
    /// Returns null if Initialize() has not been called.
    /// </summary>
    public static GooTypeRegistry? Instance => _instance;

    /// <summary>
    /// Cache for exact type matches and discovered derived type mappings.
    /// Uses ConcurrentDictionary for thread-safe runtime caching of inheritance lookups.
    /// </summary>
    private readonly ConcurrentDictionary<Type, Func<Entity, IGH_GeometricGoo>?> _exactMatchCache;

    /// <summary>
    /// Inheritance chain sorted by specificity (most derived first).
    /// Used for fallback lookup when exact match is not found.
    /// </summary>
    private readonly List<(Type BaseType, Func<Entity, IGH_GeometricGoo> Factory)> _inheritanceChain;

    /// <summary>
    /// The generic base type definition for geometric Goo types.
    /// </summary>
    private readonly Type _gooBaseType;

    /// <summary>
    /// Private constructor - use Initialize() to create the singleton instance.
    /// </summary>
    private GooTypeRegistry()
    {
        _exactMatchCache = new ConcurrentDictionary<Type, Func<Entity, IGH_GeometricGoo>?>();
        _inheritanceChain = new List<(Type, Func<Entity, IGH_GeometricGoo>)>();

        _gooBaseType = Type.GetType(_gooBaseTypeName)
            ?? throw new InvalidOperationException(_gooBaseTypeNotFound);

        this.DiscoverAndRegisterTypes();
    }

    /// <summary>
    /// Initializes the singleton instance. This should be called once during application startup.
    /// Subsequent calls are no-ops.
    /// </summary>
    public static void Initialize()
    {
        if (_instance != null) return;
        _instance = new GooTypeRegistry();
    }

    /// <summary>
    /// Discovers all Goo types in the assembly and registers their factories.
    /// </summary>
    private void DiscoverAndRegisterTypes()
    {
        var assembly = _gooBaseType.Assembly;

        var gooTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract &&
                        !t.IsGenericTypeDefinition &&
                        this.IsSubclassOfGenericType(t, _gooBaseType));

        foreach (var gooType in gooTypes)
        {
            var wrapperType = this.ExtractWrapperType(gooType);
            if (wrapperType == null) continue;

            var factory = this.CreateFactory(gooType, wrapperType);
            if (factory == null) continue;

            this.Register(wrapperType, factory);
        }

        Comparison<(Type BaseType, Func<Entity, IGH_GeometricGoo> Factory)> mostDerivedFirstComparison
            = (a, b) => this.GetInheritanceDepth(b.BaseType) - this.GetInheritanceDepth(a.BaseType);

        _inheritanceChain.Sort(mostDerivedFirstComparison);
    }

    /// <summary>
    /// Checks if a type is a subclass of a generic type definition.
    /// </summary>
    private bool IsSubclassOfGenericType(Type type, Type genericBase)
    {
        var current = type;
        while (current != null && current != typeof(object))
        {
            if (current.IsGenericType && current.GetGenericTypeDefinition() == genericBase)
                return true;
            current = current.BaseType;
        }
        return false;
    }

    /// <summary>
    /// Extracts the TWrapperType (first generic argument) from a Goo type's base class.
    /// Assuming the Goo type inherits from <see cref="GH_AutocadGeometricGoo{TWrapperType,TRhinoType}"/>.
    /// </summary>
    private Type? ExtractWrapperType(Type gooType)
    {
        var current = gooType;
        while (current != null)
        {
            if (current.IsGenericType &&
                current.GetGenericTypeDefinition() == _gooBaseType)
            {
                return current.GetGenericArguments()[0];
            }
            current = current.BaseType;
        }
        return null;
    }

    /// <summary>
    /// Creates a compiled factory delegate for the specified Goo type.
    /// Uses expression trees for optimal performance.
    /// </summary>
    private Func<Entity, IGH_GeometricGoo>? CreateFactory(Type gooType, Type wrapperType)
    {

        var constructor = gooType.GetConstructor([wrapperType]);

        if (constructor == null) return null;

        var parameter = Expression.Parameter(typeof(Entity), "entity");

        var castParameter = Expression.Convert(parameter, wrapperType);

        var newExpression = Expression.New(constructor, castParameter);

        var castResult = Expression.Convert(newExpression, typeof(IGH_GeometricGoo));

        return Expression.Lambda<Func<Entity, IGH_GeometricGoo>>(castResult, parameter).Compile();
    }

    /// <summary>
    /// Registers a type mapping in both the exact match cache and inheritance chain.
    /// </summary>
    private void Register(Type wrapperType, Func<Entity, IGH_GeometricGoo> factory)
    {
        _exactMatchCache[wrapperType] = factory;

        _inheritanceChain.Add((wrapperType, factory));
    }

    /// <summary>
    /// Calculates the inheritance depth of a type (distance from System.Object).
    /// </summary>
    private int GetInheritanceDepth(Type type)
    {
        var depth = 0;
        var current = type;
        while (current != null && current != typeof(object))
        {
            depth++;
            current = current.BaseType;
        }
        return depth;
    }

    /// <summary>
    /// Creates a Goo instance for the specified AutoCAD entity.
    /// </summary>
    /// <param name="entity">The AutoCAD entity to wrap in a Goo type.</param>
    /// <returns>
    /// An <see cref="IGH_GeometricGoo"/> instance wrapping the entity,
    /// or <see langword="null"/> if no suitable Goo type is registered.
    /// </returns>
    public IGH_GeometricGoo? CreateGeometryGoo(IEntity entity)
    {
        var cadEntity = entity.Unwrap();

        var entityType = cadEntity.GetType();

        if (_exactMatchCache.TryGetValue(entityType, out var cachedFactory))
            return cachedFactory?.Invoke(cadEntity);

        foreach (var (baseType, factory) in _inheritanceChain)
        {
            if (baseType.IsAssignableFrom(entityType) == false) continue;

            _exactMatchCache[entityType] = factory;

            return factory(cadEntity);
        }

        _exactMatchCache[entityType] = null;

        return null;
    }
}