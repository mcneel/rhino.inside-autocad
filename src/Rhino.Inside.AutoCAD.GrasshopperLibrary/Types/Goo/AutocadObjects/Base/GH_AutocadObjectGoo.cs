using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using GH_IO.Serialization;
using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD ObjectIds.
/// </summary>
public abstract class GH_AutocadObjectGoo<TWrapperType> : GH_Goo<TWrapperType>, IGH_AutocadReferenceDatabaseObject
where TWrapperType : IDbObject
{
    private const string _referenceHandleDictionaryName = "AutocadReferenceHandle";

    /// <inheritdoc />
    public IAutocadReferenceId Reference => new AutocadReferenceId(this.Value);

    /// <inheritdoc />
    public IDbObject ObjectValue => this.Value;

    /// <inheritdoc />
    public override bool IsValid => this.Value != null && this.Value.IsValid;

    /// <inheritdoc />
    public override string TypeName => $"AutoCAD {this.GetCadType().Name}";

    /// <inheritdoc />
    public override string TypeDescription => $"Represents an {this.TypeName}";

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadObject"/> class with no value.
    /// </summary>
    protected GH_AutocadObjectGoo()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadObject"/> class with the
    /// specified AutoCAD Object.
    /// </summary>
    /// <param name="dbObject">The AutoCAD Object to wrap.</param>
    protected GH_AutocadObjectGoo(TWrapperType dbObject) : base(dbObject)
    {
    }

    /// <summary>
    /// Returns the Type of AutoCAD object wrapped the Wrapper of this Goo.
    /// e.g. for GH_AutocadBlockReference it would return typeof(BlockReference)
    /// </summary>
    /// <returns></returns>
    protected abstract Type GetCadType();

    /// <summary>
    /// News up a new <see cref="IGH_Goo"/> instance wrapping the specified
    /// <see cref="IDbObject"/>
    /// </summary>
    protected abstract IGH_Goo CreateInstance(IDbObject dbObject);

    /// <inheritdoc />
    public override IGH_Goo Duplicate()
    {
        var clone = this.Value.ShallowClone();

        return this.CreateInstance(clone);
    }

    /// <inheritdoc />
    public override bool CastFrom(object source)
    {
        var converter = new GooConverter();

        if (converter.TryConvertFromGoo(source, out GH_AutocadObjectGoo<TWrapperType> gooTarget))
        {
            this.Value = gooTarget.Value;
            return true;
        }

        if (converter.TryConvertFromGoo(source, out TWrapperType target))
        {
            this.Value = target;
            return true;
        }

        if (converter.TryConvertFromGoo(source, out GH_AutocadObject genericObject))
        {
            if (genericObject.Value.Type == this.GetCadType())
            {
                var newWrapper = (GH_AutocadObjectGoo<TWrapperType>)this.CreateInstance(genericObject.Value);

                this.Value = newWrapper.Value;
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc />
    public override bool CastTo<Q>(ref Q target)
    {
        if (typeof(Q).IsAssignableFrom(typeof(TWrapperType)))
        {
            target = (Q)(object)this.Value;
            return true;
        }

        if (typeof(Q).IsAssignableFrom(typeof(GH_AutocadObjectGoo<TWrapperType>)))
        {
            target = (Q)(object)this.CreateInstance(this.Value);
            return true;
        }
        return false;
    }

    /// <inheritdoc />
    public void GetUpdatedObject()
    {
        var picker = new AutocadObjectPicker();
        if (picker.TryGetUpdatedObject(this.Reference.ObjectId, out var entity))
        {
            this.Value = (TWrapperType?)entity;
        }
    }

    /// <inheritdoc />
    public override bool Read(GH_IReader reader)
    {
        var referenceHandle = string.Empty;

        reader.TryGetString(_referenceHandleDictionaryName, ref referenceHandle);

        if (string.IsNullOrEmpty(referenceHandle))
            return true;

        var activeDocument = Application.DocumentManager.MdiActiveDocument;

        var database = activeDocument.Database;

        var handle = new Handle(Convert.ToInt64(referenceHandle, 16));

        var transaction = database.TransactionManager.StartTransaction();

        var newId = database.GetObjectId(false, handle, 0);

        if (newId.IsValid == false) return true;

        var referencedObject = transaction.GetObject(newId, OpenMode.ForRead);

        var wrapper = new DbObjectWrapper(referencedObject);

        var newWrapper = (GH_AutocadObjectGoo<TWrapperType>)this.CreateInstance(wrapper);

        this.Value = newWrapper.Value;

        transaction.Commit();

        return true;
    }

    /// <inheritdoc />
    public override bool Write(GH_IWriter writer)
    {
        if (this.Reference.ObjectId.IsValid && this.Value is not null)
            writer.SetString(_referenceHandleDictionaryName, this.Reference.GetSerializedValue());

        return true;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (this.Value == null)
            return $"Null {this.TypeName}";

        return $"{this.TypeName} [Id: {this.Reference} ]";
    }
}