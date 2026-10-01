using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Services;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IPreviewServer"/>
/// <remarks>
/// Each registered object is previewed by one <see cref="IPreviewDrawable"/>, registered with
/// AutoCAD as a single transient, so the transient count stays at the number of registered
/// objects however many items they hold. The <see cref="IObjectRegister"/> holds the strong
/// reference which keeps each drawable alive while it is registered as a transient.
/// </remarks>
public class PreviewServer : IPreviewServer
{
    private readonly IGeometryPreviewSettings _previewSettings;
    private readonly IGeometryPreviewSettings _selectedPreviewSettings;
    private readonly IPreviewDrawableBuilder _previewDrawableBuilder;
    private readonly int _subDrawingMode = 0;
    private readonly IntegerCollection _emptyIntegerCollection = [];
    private readonly TransientDrawingMode _transientDrawingMode = TransientDrawingMode.Main;
    private int _maxEntityCount;

    /// <inheritdoc/>
    public IObjectRegister ObjectRegister { get; }

    /// <inheritdoc/>
    public bool Visible { get; private set; } = true;

    /// <inheritdoc/>
    public int MaxEntityCount
    {
        get => _maxEntityCount;
        set
        {
            _maxEntityCount = value;

            this.EvictOldestObjects(0);
        }
    }

    /// <summary>
    /// Constructs a new <see cref="IPreviewServer"/>
    /// </summary>
    public PreviewServer(IGeometryPreviewSettings previewSettings, IGeometryPreviewSettings selectedPreviewSettings,
        IPreviewDrawableBuilder previewDrawableBuilder, int maxEntityCount)
    {
        _previewSettings = previewSettings;
        _selectedPreviewSettings = selectedPreviewSettings;
        _previewDrawableBuilder = previewDrawableBuilder;
        _maxEntityCount = maxEntityCount;
        this.ObjectRegister = new ObjectRegister();
    }

    /// <summary>
    /// Removes and disposes the oldest registered objects until
    /// <paramref name="incomingItemCount"/> more preview items fit within
    /// <see cref="MaxEntityCount"/>.
    /// </summary>
    private void EvictOldestObjects(int incomingItemCount)
    {
        var evictedCount = 0;

        while (this.ObjectRegister.ItemCount + incomingItemCount > _maxEntityCount &&
               this.ObjectRegister.TryGetOldest(out var oldestId, out var oldestDrawable))
        {
            this.ObjectRegister.RemoveObject(oldestId);

            if (this.Visible)
            {
                this.EraseTransient(oldestDrawable!);
            }

            oldestDrawable!.Dispose();

            evictedCount++;
        }

        if (evictedCount > 0)
        {
            LoggerService.Instance.LogMessage(
                $"Preview limit of {_maxEntityCount} entities reached: removed the {evictedCount} oldest preview(s).");
        }
    }

    /// <summary>
    /// Registers the given drawable with AutoCAD as a transient, so it is drawn.
    /// </summary>
    private void AddTransient(IPreviewDrawable previewDrawable)
    {
        if (previewDrawable is not Drawable drawable) return;

        var transientManager = TransientManager.CurrentTransientManager;

        if (transientManager.AddTransient(drawable, _transientDrawingMode,
                _subDrawingMode, _emptyIntegerCollection) == false)
        {
            LoggerService.Instance.LogMessage("Unable to create Transient element");
        }
    }

    /// <summary>
    /// Erases the given drawable's transient from AutoCAD, so it is no longer drawn.
    /// </summary>
    /// <remarks>
    /// TransientManager can throw if the drawable was already erased, or while the
    /// application is closing (for example Civil 3D shutdown, when
    /// <see cref="TransientManager.CurrentTransientManager"/> itself throws). The exception
    /// is swallowed so the caller can still dispose the drawable.
    /// </remarks>
    private void EraseTransient(IPreviewDrawable previewDrawable)
    {
        if (previewDrawable is not Drawable drawable) return;

        try
        {
            var transientManager =
                TransientManager
                    .CurrentTransientManager; //Throws here in Civil Application Shutdown

            transientManager.EraseTransient(drawable, _emptyIntegerCollection);
        }
        catch (Exception exception)
        {
            System.Diagnostics.Debug.WriteLine($"PreviewServer.EraseTransient() failed: {exception.Message}");
        }
    }

    /// <summary>
    /// Asks AutoCAD to draw the given drawable's transient again, so a change to its
    /// appearance or selection state is seen.
    /// </summary>
    private void UpdateTransient(IPreviewDrawable previewDrawable)
    {
        if (previewDrawable is not Drawable drawable) return;

        var transientManager = TransientManager.CurrentTransientManager;

        transientManager.UpdateTransient(drawable, _emptyIntegerCollection);
    }

    /// <summary>
    /// Removes transient elements from display but keeps them in the register for later re-use.
    /// Used for visibility toggling (preview on/off).
    /// </summary>
    /// <remarks>
    /// Does nothing when the server is already hidden, as its drawables are not registered
    /// as transients then.
    /// </remarks>
    public void ClearServer()
    {
        if (this.Visible == false) return;

        this.Visible = false;

        foreach (var drawable in this.ObjectRegister)
        {
            this.EraseTransient(drawable);
        }
    }

    /// <summary>
    /// Removes all transient elements, disposes the drawables and empties the register.
    /// Used during application shutdown to ensure clean disposal.
    /// </summary>
    public void ClearAndDisposeAll()
    {
        System.Diagnostics.Debug.WriteLine("PreviewServer.ClearAndDisposeAll() - disposing drawables");

        var drawables = this.ObjectRegister.ToList();

        this.ObjectRegister.Clear();

        foreach (var drawable in drawables)
        {
            if (this.Visible)
            {
                this.EraseTransient(drawable);
            }

            drawable.Dispose();
        }

        System.Diagnostics.Debug.WriteLine("PreviewServer.ClearAndDisposeAll() - complete");
    }

    /// <summary>
    /// Updates the transient elements visibility based on the current state.
    /// </summary>
    /// <remarks>
    /// Does nothing when the server is already visible, as its drawables are already
    /// registered as transients then.
    /// </remarks>
    public void PopulateServer()
    {
        if (this.Visible) return;

        this.Visible = true;

        foreach (var drawable in this.ObjectRegister)
        {
            this.AddTransient(drawable);
        }
    }

    /// <inheritdoc/>
    public void AddObject(Guid rhinoObjectId, IRhinoConvertibleSet rhinoConvertibleSet, bool selected)
    {
        if (rhinoConvertibleSet.Any)
        {
            // Anything already registered under this id is replaced, so it must not count
            // against the room the new drawable needs.
            this.RemoveObject(rhinoObjectId);

            var drawable = _previewDrawableBuilder.Build(rhinoConvertibleSet, _previewSettings,
                _selectedPreviewSettings, selected, _maxEntityCount);

            if (drawable.ItemCount >= _maxEntityCount)
            {
                LoggerService.Instance.LogMessage(
                    $"Preview limit of {_maxEntityCount} entities reached: preview may be incomplete.");
            }

            this.EvictOldestObjects(drawable.ItemCount);

            this.ObjectRegister.RegisterObject(rhinoObjectId, drawable);

            // Only draw when the server is visible; hidden servers keep the drawable
            // registered so PopulateServer can display it when visibility returns.
            if (this.Visible)
            {
                this.AddTransient(drawable);
            }
        }
    }

    /// <inheritdoc/>
    public void RemoveObject(Guid rhinoObjectId)
    {
        if (this.ObjectRegister.TryGetObject(rhinoObjectId, out var drawable))
        {
            this.ObjectRegister.RemoveObject(rhinoObjectId);

            if (this.Visible)
            {
                this.EraseTransient(drawable!);
            }

            drawable!.Dispose();
        }
    }

    /// <inheritdoc/>
    public bool SetSelected(Guid rhinoObjectId, bool selected)
    {
        if (this.ObjectRegister.TryGetObject(rhinoObjectId, out var drawable) == false) return false;

        if (drawable!.IsSelected == selected) return true;

        drawable.IsSelected = selected;

        if (this.Visible)
        {
            this.UpdateTransient(drawable);
        }

        return true;
    }

    /// <inheritdoc />
    public void DeselectAll()
    {
        foreach (var drawable in this.ObjectRegister)
        {
            if (drawable.IsSelected == false) continue;

            drawable.IsSelected = false;

            if (this.Visible)
            {
                this.UpdateTransient(drawable);
            }
        }
    }

    /// <inheritdoc />
    /// <remarks>
    /// The drawables read their traits from the settings on every draw, so nothing is rebuilt:
    /// each drawable re-applies the settings to anything which caches them and its transient
    /// is updated so AutoCAD draws it again. Hidden drawables are only re-applied, and are
    /// drawn with the current settings when the server is next populated.
    /// </remarks>
    public void RefreshAppearance()
    {
        foreach (var drawable in this.ObjectRegister)
        {
            drawable.RefreshAppearance();

            if (this.Visible)
            {
                this.UpdateTransient(drawable);
            }
        }
    }
}
