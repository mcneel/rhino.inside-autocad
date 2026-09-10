namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Provides constants for building input signatures, used by the Create and Tracked
/// Bake components to detect input changes between solves.
/// </summary>
public class SignatureConstants
{
    /// <summary>
    /// The version prefix embedded in every signature. Bumping this value invalidates
    /// all previously persisted signatures, forcing a one-time re-bake of saved
    /// definitions after the signature format changes.
    /// </summary>
    public const string SignatureFormatVersion = "v2";

    /// <summary>
    /// Raw signatures longer than this are replaced by an MD5 hash to keep the
    /// serialized value in the Grasshopper file reasonable.
    /// </summary>
    public const int HashedSignatureLengthThreshold = 1000;

    /// <summary>
    /// The numeric format used for coordinate values that enter the signature
    /// directly (points, scales) rather than through a geometry data CRC.
    /// </summary>
    public const string CoordinateFormat = "F6";
}
