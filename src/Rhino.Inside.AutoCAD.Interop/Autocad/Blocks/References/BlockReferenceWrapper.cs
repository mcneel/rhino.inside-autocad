using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IBlockReference"/>
public class BlockReferenceWrapper : EntityWrapper, IBlockReference
{
    private readonly BlockReference _blockReference;

    /// <inheritdoc />
    public ICustomPropertySet CustomProperties { get; }

    /// <inheritdoc />
    public double Rotation => _blockReference.Rotation;

    /// <inheritdoc />
    public string Name { get; }

    /// <summary>
    /// Constructs a new <see cref="BlockReferenceWrapper"/>.
    /// </summary>
    public BlockReferenceWrapper(BlockReference blockReference) : base(blockReference)
    {
        _blockReference = blockReference;

        this.Name = blockReference.Name;

        this.CustomProperties = new CustomPropertySet();
    }

    /// <summary>
    /// Obtains the <see cref="DynamicBlockReferenceProperty"/>s of the provided 
    /// <see cref="BlockReference"/>.
    /// </summary>
    private IDictionary<string, DynamicBlockReferenceProperty> DynamicBlockReferenceProperties(BlockReference blockReference)
    {
        var blockReferenceProperties = new Dictionary<string, DynamicBlockReferenceProperty>();

        foreach (DynamicBlockReferenceProperty dynamicProperty in blockReference.DynamicBlockReferencePropertyCollection)
        {
            if (dynamicProperty.ReadOnly) continue;

            blockReferenceProperties[dynamicProperty.PropertyName] = dynamicProperty;
        }

        return blockReferenceProperties;
    }

    /// <inheritdoc />
    public void AddCustomProperties(ICustomPropertySet customProperties)
    {
        foreach (var customProperty in customProperties)
        {
            this.CustomProperties.Add(customProperty);
        }
    }

    /// <inheritdoc />
    public void CommitCustomProperties()
    {
        var blockReferenceProperties = this.DynamicBlockReferenceProperties(_blockReference);

        foreach (var property in this.CustomProperties)
        {
            var propertyName = property.Name.Name;

            var propertyValue = property.Value.Value;

            if (blockReferenceProperties.TryGetValue(propertyName, out var dynamicProperty) == false) continue;

            var type = dynamicProperty.Value.GetType();

            var convertedValue = Convert.ChangeType(propertyValue, type);

            if (convertedValue == null) continue;

            dynamicProperty.Value = convertedValue;
        }
    }
}