using Rhino.Inside.AutoCAD.Core.Interfaces;
using CadLayout = Autodesk.AutoCAD.DatabaseServices.Layout;

namespace Rhino.Inside.AutoCAD.Interop;

///<inheritdoc cref="IAutocadLayout"/>
public class AutocadLayoutWrapper : DbObject, IAutocadLayout
{
    private readonly CadLayout _layout;

    ///<inheritdoc />
    public string Name { get; }

    /// <summary>
    /// Constructs a new <see cref="AutocadLayoutWrapper"/>.
    /// </summary>
    public AutocadLayoutWrapper(CadLayout layout) : base(layout)
    {
        _layout = layout;
        this.Name = layout.LayoutName;
    }

    ///<inheritdoc />
    public IObjectIdTag GetTag()
    {
        return ObjectIdTag.CreateExisting(this.Id.Value);
    }

    ///<inheritdoc />
    public IAutocadLayout ShallowClone()
    {
        return new AutocadLayoutWrapper(_layout);
    }
}