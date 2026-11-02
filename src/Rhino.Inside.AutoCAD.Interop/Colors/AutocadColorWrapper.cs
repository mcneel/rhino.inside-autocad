using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using CadColor = Autodesk.AutoCAD.Colors.Color;
using ColorMethod = Autodesk.AutoCAD.Colors.ColorMethod;

namespace Rhino.Inside.AutoCAD.Interop;


/// <summary>
/// Represents an AutoCAD color, supporting RGB, ByLayer, ByBlock, and ACI modes.
/// </summary>
/// <remarks>
/// AutoCAD uses special color index values for ByLayer (256) and ByBlock (0).
/// This class provides methods to create and manipulate colors with these
/// special values, as well as standard RGB and ACI (AutoCAD Color Index) colors.
/// </remarks>
public class AutocadColorWrapper : AutocadWrapperBase<CadColor>, IAutocadColor
{
    /// <summary>
    /// The layer of the entity this color came from, used to resolve ByLayer. Read when
    /// the wrapper is constructed because the entity may be closed by the time the color
    /// is resolved.
    /// </summary>
    private readonly ObjectId? _associatedLayerId;

    /// <summary>
    /// Creates a ByLayer color.
    /// </summary>
    /// <returns>An <see cref="AutocadColorWrapper"/> representing ByLayer color.</returns>
    public static AutocadColorWrapper CreateByLayer()
        => new(CadColor.FromColorIndex(ColorMethod.ByLayer, ByLayerIndex));

    /// <summary>
    /// Creates a ByBlock color.
    /// </summary>
    /// <returns>An <see cref="AutocadColorWrapper"/> representing ByBlock color.</returns>
    public static AutocadColorWrapper CreateFromIndex(short index)
        => new(CadColor.FromColorIndex(ColorMethod.ByAci, index));


    /// <summary>
    /// Creates a ByBlock color.
    /// </summary>
    /// <returns>An <see cref="AutocadColorWrapper"/> representing ByBlock color.</returns>
    public static AutocadColorWrapper CreateByBlock()
        => new(CadColor.FromColorIndex(ColorMethod.ByBlock, ByBlockIndex));


    /// <summary>
    /// Creates a color from RGB values.
    /// </summary>
    /// <param name="r">Red component (0-255).</param>
    /// <param name="g">Green component (0-255).</param>
    /// <param name="b">Blue component (0-255).</param>
    /// <returns>An <see cref="AutocadColorWrapper"/> representing the RGB color.</returns
    public static AutocadColorWrapper CreateFromRgb(byte r, byte g, byte b)
        => new(CadColor.FromRgb(r, g, b));

    /// <summary>
    /// ColorIndex 256 = ByLayer
    /// </summary>
    public const short ByLayerIndex = 256;

    /// <summary>
    /// ColorIndex 0 = ByBlock
    /// </summary>
    public const short ByBlockIndex = 0;

    /// <inheritdoc/>
    public short ColorIndex { get; }

    /// <inheritdoc/>
    public bool IsByLayer => this.ColorIndex == ByLayerIndex;

    /// <inheritdoc/>
    public bool IsByBlock => this.ColorIndex == ByBlockIndex;

    /// <summary>
    /// Constructs a new <see cref="InternalColor"/> from <see cref="Autodesk.AutoCAD.Colors.Color"/>.
    /// </summary>
    public AutocadColorWrapper(Entity associatedObject) : base(associatedObject.Color)
    {
        _associatedLayerId = associatedObject.LayerId;

        this.ColorIndex = associatedObject.Color.ColorIndex;
    }

    /// <summary>
    /// Constructs a new <see cref="InternalColor"/> from <see cref="Autodesk.AutoCAD.Colors.Color"/>.
    /// </summary>
    public AutocadColorWrapper(CadColor color) : base(color)
    {
        _associatedLayerId = null;

        this.ColorIndex = color.ColorIndex;
    }

    public IColor ResolveColor(IAutocadTransactionManager transactionManager)
    {
        if (_associatedLayerId == null)
            return new InternalColor(this.AutocadObject);

        if (this.IsByLayer)
        {
            var layer =
                transactionManager.Unwrap().GetObject(_associatedLayerId.Value,
                    OpenMode.ForRead, false) as LayerTableRecord;
            if (layer != null)
            {
                return new InternalColor(layer.Color);
            }
        }

        return new InternalColor(this.AutocadObject);

    }

    /// <inheritdoc/>
    public bool IsEqualTo(IAutocadColor other)
    {
        var otherColor = other.Unwrap();

        return this.ColorIndex == other.ColorIndex
               && this.AutocadObject.ColorValue == otherColor.ColorValue;
    }
}
