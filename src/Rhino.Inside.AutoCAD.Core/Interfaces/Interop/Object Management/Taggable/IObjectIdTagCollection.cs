namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a complex taggable object that made of several primitives within the
/// active <see cref="IAutocadDocument"/>. E.g. <see cref="ILegend"/> containts lines, texts,
/// and blocks primitives, all these entities have to be registered. The <see cref=
/// "IObjectIdTagCollection"/> is used to register <see cref="IObjectIdTag"/>s to <see 
/// cref="IObjectIdTagRecord"/>.
/// </summary>
public interface IObjectIdTagCollection : IEnumerable<IObjectIdTag>, IDisposable;
