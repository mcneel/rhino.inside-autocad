using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Text;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IEntityValidationResult"/>
public class EntityValidationResult : IEntityValidationResult
{
    private readonly List<IEntityValidationError> _errors = [];

    /// <inheritdoc />
    public bool IsValid => _errors.Count == 0;

    /// <inheritdoc />
    public void AddError(IEntityValidationError error)
    {
        _errors.Add(error);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (this.IsValid) return "All entities valid.";

        var sb = new StringBuilder();
        sb.AppendLine($"Found {_errors.Count} invalid entities:");
        foreach (var error in _errors)
        {
            sb.AppendLine($" {error.EntityType} (Handle: {error.Handle}): {error.Message}");
        }
        return sb.ToString();
    }
}

