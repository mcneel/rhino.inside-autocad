using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IEntityValidationError"/>
public class EntityValidationError : IEntityValidationError
{
    /// <inheritdoc/>
    public string EntityType { get; set; }

    /// <inheritdoc/>
    public string Handle { get; set; }

    /// <inheritdoc/>
    public string Message { get; set; }
}