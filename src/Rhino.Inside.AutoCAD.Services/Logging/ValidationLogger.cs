using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Services;

/// <inheritdoc cref="IValidationLogger"/>
public struct ValidationLogger : IValidationLogger
{
    private readonly List<string> _messages;

    /// <inheritdoc/>
    public bool HasValidationErrors { get; private set; }

    /// <summary>
    /// Constructs a new <see cref="ValidationLogger"/>
    /// </summary>
    public ValidationLogger()
    {
        _messages = [];

        this.HasValidationErrors = false;
    }

    /// <inheritdoc/>
    public void AddMessage(string message)
    {
        _messages.Add(message);

        this.HasValidationErrors = true;
    }

    /// <inheritdoc/>
    public string GetMessage()
    {
        return _messages.Count > 0 ? _messages.Last() : string.Empty;
    }
}