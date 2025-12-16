namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A service which manages and executes BREP conversion requests. Autocad does not exposed
/// methods to create Brep objects directly from geometry data in either of the .Net or
/// ObjectArx libraries so this service is responsible  for converting Brep via the Import
/// command. It works in bulk by queuing requests and once the Autocad
/// RHINO_INSIDE_CONVERT_BREP Command is run, all requests in the queue are processed.
/// </summary>
public interface IBrepConverterRunner
{
    /// <summary>
    /// Adds a BREP conversion request to the internal queue to be processed. Not this will
    /// not convert the brep immediately, it will only be converted when the <see cref="Run"/>
    /// method is called. 
    /// </summary>
    void EnqueueRequest(IBrepConverterRequest request);

    /// <summary>
    /// The method which processes all queued BREP conversion requests. This method must
    /// only be called from within the context of a Autocad command, otherwise an exception
    /// cause the import to fail. The command RHINO_INSIDE_CONVERT_BREP is provided for
    /// this purpose. 
    /// </summary>
    void Run();
}