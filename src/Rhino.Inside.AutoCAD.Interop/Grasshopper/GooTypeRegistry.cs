using System.Reflection;
using Autodesk.AutoCAD.DatabaseServices;
using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A registry that discovers and caches Goo type factories for AutoCAD entity conversion.
/// Uses convention-based reflection to automatically register all types inheriting from
/// GH_AutocadGeometricGoo, and any other Goo marked with <see cref="GooEntityTypesAttribute"/>.
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
    /// Registers all Goo types from the specified assembly.
    /// Called by external libraries (e.g., Civil 3D) to register their types.
    /// </summary>
    /// <param name="assembly">The assembly containing Goo types to register.</param>
    public void RegisterAssembly(Assembly assembly)
    {
        if (assembly == null) return;

        var gooTypes = assembly.GetTypes().Where(this.IsRegistrableGoo);

        foreach (var gooType in gooTypes)
        {
            foreach (var entityType in this.GetRegistrationTypes(gooType))
            {
                // Skip if already registered
                if (_exactMatchCache.ContainsKey(entityType)) continue;

                var factory = this.CreateFactory(gooType, entityType);
                if (factory == null) continue;

                this.Register(entityType, factory);
            }
        }

        // Re-sort inheritance chain (most derived first)
        _inheritanceChain.Sort((a, b) =>
            this.GetInheritanceDepth(b.BaseType) - this.GetInheritanceDepth(a.BaseType));
    }

    /// <summary>
    /// Discovers all Goo types in the assembly and registers their factories.
    /// </summary>
    private void DiscoverAndRegisterTypes()
    {
        var assembly = _gooBaseType.Assembly;

        var gooTypes = assembly.GetTypes().Where(this.IsRegistrableGoo);

        foreach (var gooType in gooTypes)
        {
            foreach (var entityType in this.GetRegistrationTypes(gooType))
            {
                var factory = this.CreateFactory(gooType, entityType);
                if (factory == null) continue;

                this.Register(entityType, factory);
            }
        }

        Comparison<(Type BaseType, Func<Entity, IGH_GeometricGoo> Factory)> mostDerivedFirstComparison
            = (a, b) => this.GetInheritanceDepth(b.BaseType) - this.GetInheritanceDepth(a.BaseType);

        _inheritanceChain.Sort(mostDerivedFirstComparison);
    }

    /// <summary>
    /// Checks if a type is a concrete Goo the registry should create factories for: either
    /// a subclass of the geometric Goo base, or a Goo which names its entity types with a
    /// <see cref="GooEntityTypesAttribute"/> because its value is not an AutoCAD entity.
    /// </summary>
    private bool IsRegistrableGoo(Type type)
    {
        if (type.IsAbstract || type.IsGenericTypeDefinition) return false;

        return this.IsSubclassOfGenericType(type, _gooBaseType)
               || type.GetCustomAttribute<GooEntityTypesAttribute>() != null;
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
    /// Returns the AutoCAD entity types the Goo should be registered against: the types
    /// named by its <see cref="GooEntityTypesAttribute"/> when it has one, otherwise its
    /// wrapper type.
    /// </summary>
    /// <param name="gooType">The Goo type being registered.</param>
    /// <returns>The types to key the Goo's factory on, which may be empty.</returns>
    private IEnumerable<Type> GetRegistrationTypes(Type gooType)
    {
        var attribute = gooType.GetCustomAttribute<GooEntityTypesAttribute>();

        if (attribute != null && attribute.EntityTypes.Length > 0)
            return attribute.EntityTypes;

        var wrapperType = this.ExtractWrapperType(gooType);

        return wrapperType == null ? Array.Empty<Type>() : new[] { wrapperType };
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