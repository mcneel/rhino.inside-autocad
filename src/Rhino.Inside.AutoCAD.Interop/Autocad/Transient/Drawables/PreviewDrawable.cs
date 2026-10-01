using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Services;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IPreviewDrawable"/>
/// <remarks>
/// Derives from <see cref="Transient"/> rather than <see cref="Drawable"/>: constructing a
/// direct <see cref="Drawable"/> subclass throws in the native wrapper's constructor, while
/// <see cref="Transient"/> supplies a supported parameterless constructor and seals the
/// identity members (<see cref="Drawable.Id"/>, <see cref="Drawable.IsPersistent"/>) a
/// transient needs.
/// <para>
/// The polylines, points and shells are drawn directly from <see cref="IPreviewGeometryBuffer"/>
/// so they create no AutoCAD entities. The traits are read from the current
/// <see cref="IGeometryPreviewSettings"/> on every draw, so a settings change only needs the
/// transient to be updated, not rebuilt. Each buffer collection is unwrapped to the native
/// AutoCAD collection it wraps, so drawing copies and allocates no collections.
/// </para>
/// </remarks>
public class PreviewDrawable : Transient, IPreviewDrawable
{
    private readonly IPreviewGeometryBuffer _buffers;
    private readonly IGeometryPreviewSettings _unselectedSettings;
    private readonly IGeometryPreviewSettings _selectedSettings;
    private readonly EdgeData _edgeData;
    private readonly FaceData _faceData;
    private readonly VertexData _vertexData;
    private bool _isSelected;
    private bool _disposed;

    /// <inheritdoc/>
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;

            this.RefreshAppearance();
        }
    }

    /// <inheritdoc/>
    public int ItemCount => _buffers.ItemCount;

    /// <summary>
    /// Constructs a new <see cref="PreviewDrawable"/>.
    /// </summary>
    /// <param name="buffers">The primitive buffers to draw. The drawable takes ownership of
    /// them and disposes them with itself.</param>
    /// <param name="unselectedSettings">The settings drawn with while not selected.</param>
    /// <param name="selectedSettings">The settings drawn with while selected.</param>
    /// <param name="isSelected">Whether the drawable starts out selected.</param>
    public PreviewDrawable(IPreviewGeometryBuffer buffers, IGeometryPreviewSettings unselectedSettings,
        IGeometryPreviewSettings selectedSettings, bool isSelected)
    {
        _buffers = buffers;
        _unselectedSettings = unselectedSettings;
        _selectedSettings = selectedSettings;
        _isSelected = isSelected;

        var edgeData = new EdgeData();
        var faceData = new FaceData();
        var vertexData = new VertexData();

        _edgeData = edgeData;
        _faceData = faceData;
        _vertexData = vertexData;

        this.RefreshAppearance();
    }

    /// <summary>
    /// Returns the settings the drawable currently draws with.
    /// </summary>
    private IGeometryPreviewSettings GetCurrentSettings()
    {
        return _isSelected ? _selectedSettings : _unselectedSettings;
    }

    /// <summary>
    /// Applies the material of the given settings to the traits when it references a live
    /// material in the working database, mirroring <see cref="IGeometryPreviewSettings.ApplyTo"/>.
    /// </summary>
    private void ApplyMaterial(SubEntityTraits traits, IGeometryPreviewSettings settings)
    {
        try
        {
            var materialId = settings.MaterialId.Unwrap();

            if (materialId is { IsNull: false, IsValid: true, IsErased: false } &&
                materialId.Database == HostApplicationServices.WorkingDatabase)
            {
                traits.Material = materialId;
            }
        }
        catch (Autodesk.AutoCAD.Runtime.Exception exception)
        {
            LoggerService.Instance.LogMessage(
                $"Unable to draw preview material '{settings.MaterialName}': {exception.Message}");
        }
    }

    /// <inheritdoc/>
    public void RefreshAppearance()
    {
        var settings = this.GetCurrentSettings();

        foreach (var entity in _buffers.FallbackEntities)
        {
            if (entity.Unwrap().IsDisposed) continue;

            settings.ApplyTo(entity);
        }
    }

    /// <inheritdoc/>
    protected override int SubSetAttributes(DrawableTraits traits)
    {
        return (int)DrawableAttributes.None;
    }

    /// <inheritdoc/>
    protected override bool SubWorldDraw(WorldDraw wd)
    {
        if (_disposed) return true;

        var settings = this.GetCurrentSettings();

        var traits = wd.SubEntityTraits;

        var transparency = new Transparency(settings.Transparency);

        traits.Color = (short)settings.ColorIndex;
        traits.Transparency = transparency;
        traits.LineWeight = LineWeight.LineWeight050;

        this.ApplyMaterial(traits, settings);

        var geometry = wd.Geometry;

        foreach (var polyline in _buffers.Polylines)
        {
            var cadPolyline = polyline.Unwrap();

            geometry.Polyline(cadPolyline, Vector3d.ZAxis, IntPtr.Zero);
        }

        if (_buffers.Points.Count > 0)
        {
            var cadPoints = _buffers.Points.Unwrap();
            var cadPointNormals = _buffers.PointNormals.Unwrap();
            var cadPointSubEntityMarkers = _buffers.PointSubEntityMarkers.Unwrap();

            geometry.Polypoint(cadPoints, cadPointNormals, cadPointSubEntityMarkers);
        }

        if (_buffers.Shells.Count > 0)
        {
            traits.FillType = FillType.FillAlways;

            foreach (var shell in _buffers.Shells)
            {
                var cadVertices = shell.Vertices.Unwrap();
                var cadFaces = shell.Faces.Unwrap();

                geometry.Shell(cadVertices, cadFaces, _edgeData, _faceData, _vertexData, true);
            }

            traits.FillType = FillType.FillNever;
        }

        foreach (var entity in _buffers.FallbackEntities)
        {
            var cadEntity = entity.Unwrap();

            if (cadEntity.IsDisposed) continue;

            geometry.Draw(cadEntity);
        }

        return true;
    }

    /// <inheritdoc/>
    protected override void SubViewportDraw(ViewportDraw vd)
    {
    }

    /// <inheritdoc/>
    /// <remarks>
    /// The buffers are only disposed on an explicit dispose: their fallback entities are
    /// AutoCAD objects which must not be released from the finalizer thread.
    /// </remarks>
    protected override void Dispose(bool disposing)
    {
        if (disposing && _disposed == false)
        {
            _disposed = true;

            _buffers.Dispose();
        }

        base.Dispose(disposing);
    }
}
