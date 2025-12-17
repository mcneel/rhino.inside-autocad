namespace Rhino.Inside.AutoCAD.Core;

/// <summary>
/// Represents potential issues related to versioning in the application.
/// </summary>
[Flags]
public enum VersioningIssues
{
    /// <summary>
    /// No versioning issues detected.
    /// </summary>
    None = 0,

    /// <summary>
    /// Indicates that the version is obsolete and may no longer be supported.
    /// </summary>
    Obsolete = 1,

    /// <summary>
    /// Indicates that the version does not comply with the required application
    /// versions, this is not currently used but reserved for future use.
    /// </summary>
    NotCompliant = 2
}