using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IAutocadDocumentChangeEventArgs"/>
public class AutocadDocumentChangeEventArgs : EventArgs, IAutocadDocumentChangeEventArgs
{
    /// <inheritdoc />
    public IAutocadDocumentChange Change { get; }

    /// <summary>
    /// Constructs a new <see cref="IAutocadDocumentChangeEventArgs"/>.
    /// </summary>
    public AutocadDocumentChangeEventArgs(IAutocadDocumentChange documentChange)
    {
        this.Change = documentChange;
    }
}