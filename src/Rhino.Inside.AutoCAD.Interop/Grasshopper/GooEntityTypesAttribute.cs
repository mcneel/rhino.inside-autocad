namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Declares the AutoCAD entity types a Goo should be registered against in
/// <see cref="GooTypeRegistry"/>.
/// </summary>
/// <remarks>
/// The registry normally keys a Goo on its wrapper type, the first generic argument of
/// its base class. That is wrong for a Goo whose wrapper type is broader than the
/// entities it actually represents: a Goo wrapping
/// <see cref="Autodesk.AutoCAD.DatabaseServices.Entity"/> would otherwise become the
/// fallback for every entity type no other Goo claims. Applying this attribute registers
/// the Goo against the listed types instead, and leaves every Goo without it unaffected.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class GooEntityTypesAttribute : Attribute
{
    /// <summary>
    /// The AutoCAD entity types the Goo should be created for.
    /// </summary>
    public Type[] EntityTypes { get; }

    /// <summary>
    /// Constructs a new <see cref="GooEntityTypesAttribute"/> instance.
    /// </summary>
    /// <param name="entityTypes">
    /// The AutoCAD entity types the Goo should be created for. Each one must have a
    /// matching public single argument constructor on the Goo.
    /// </param>
    public GooEntityTypesAttribute(params Type[] entityTypes)
    {
        this.EntityTypes = entityTypes;
    }
}
