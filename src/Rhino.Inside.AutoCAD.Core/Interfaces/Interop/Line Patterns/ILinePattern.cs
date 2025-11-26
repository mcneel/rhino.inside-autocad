namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A geometrical representation of a line pattern from AutoCAD used for
/// display purposes.
/// </summary>
public interface ILinePattern : IDisposable
{
    /// <summary>
    /// The pattern of this <see cref="ILinePattern"/>.
    /// </summary>
  //  IList<ILineCurve> Pattern { get; }

    /// <summary>
    /// The <see cref="IObjectId"/> of this <see cref="ILinePattern"/>.
    /// </summary>
    IObjectId Id { get; }

    /// <summary>
    /// The name of this <see cref="ILinePattern"/> and underlying line style
    /// in AutoCAD.
    /// </summary>
    string Name { get; }
}