namespace Rhino.Inside.AutoCAD.Services;

/// <summary>
/// The disposable base class.
/// </summary>
public abstract class Disposable
{
    protected bool _disposed;

    // Protected implementation of Dispose pattern.
    protected abstract void Dispose(bool disposing);

    /// <summary>
    /// Public implementation of Dispose pattern callable by consumers.
    /// </summary>
    public void Dispose()
    {
        this.Dispose(true);

        GC.SuppressFinalize(this);
    }
}