using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Services;

/// <inheritdoc cref="IStartUpLogger"/>
public class StartUpLogger : IStartUpLogger
{
    private readonly List<string> _errorMessages = new();

    /// <inheritdoc />
    public bool HasError => _errorMessages.Count > 0;

    /// <inheritdoc />
    public void AddError(string message)
    {
        _errorMessages.Add(message);
    }

    /// <inheritdoc />
    public string GetLastErrorMessage()
    {
        return this.HasError ? _errorMessages.Last() : string.Empty;
    }
}