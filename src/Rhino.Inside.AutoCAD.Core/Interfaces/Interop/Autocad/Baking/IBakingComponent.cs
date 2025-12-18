using System.Collections;

namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A component that supports baking to AutoCAD, it must have an output parameter which
/// contains a list of <see cref="IObjectId"/>s which represents the baked geometry.
/// </summary>
public interface IBakingComponent
{
    /// <summary>
    /// The index of the output parameter of the <see cref="IObjectId"/> of the baked
    /// geometry.
    /// </summary>
    int OutputParamTargetIndex { get; }

    /// <summary>
    /// This method appends a list of data to the specified output parameter at the given
    /// index outside the normal solution process, this is useful for components that need
    /// to output data asynchronously or in response to external events. For example, it
    /// is used to append baked breps in the Bake component into AutoCAD which involves
    /// an asynchronous operation.
    /// </summary>
    bool AppendDataList(IEnumerable list);

    /// <summary>
    /// Adds a warning message to the component's runtime messages.
    /// </summary>
    void AddWarningMessage(string message);
}