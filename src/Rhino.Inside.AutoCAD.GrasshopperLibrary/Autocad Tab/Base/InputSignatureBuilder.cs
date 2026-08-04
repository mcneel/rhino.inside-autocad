using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using System.Security.Cryptography;
using System.Text;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <inheritdoc cref="IInputSignatureBuilder"/>
public class InputSignatureBuilder : IInputSignatureBuilder
{
    private const char _separator = '|';
    private const string _signatureFormatVersion = SignatureConstants.SignatureFormatVersion;
    private const int _hashedSignatureLengthThreshold = SignatureConstants.HashedSignatureLengthThreshold;
    private const string _coordinateFormat = SignatureConstants.CoordinateFormat;

    private readonly StringBuilder _stringBuilder = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="InputSignatureBuilder"/> class.
    /// </summary>
    public InputSignatureBuilder()
    {
        _stringBuilder.Append(_signatureFormatVersion);
        _stringBuilder.Append(_separator);
    }

    /// <inheritdoc />
    public IInputSignatureBuilder Add(string? value)
    {
        _stringBuilder.Append(value ?? string.Empty);
        _stringBuilder.Append(_separator);
        return this;
    }

    /// <inheritdoc />
    public IInputSignatureBuilder Add(int value)
    {
        _stringBuilder.Append(value);
        _stringBuilder.Append(_separator);
        return this;
    }

    /// <inheritdoc />
    public IInputSignatureBuilder Add(double value, int decimals = 6)
    {
        _stringBuilder.Append(Math.Round(value, decimals));
        _stringBuilder.Append(_separator);
        return this;
    }

    /// <inheritdoc />
    public IInputSignatureBuilder Add(IObjectId? objectId)
    {
        _stringBuilder.Append(objectId?.Value ?? 0L);
        _stringBuilder.Append(_separator);
        return this;
    }

    /// <inheritdoc />
    public IInputSignatureBuilder AddCurve(Rhino.Geometry.Curve? curve)
    {
        return this.AddGeometry(curve);
    }

    /// <inheritdoc />
    public IInputSignatureBuilder AddMesh(Rhino.Geometry.Mesh? mesh)
    {
        return this.AddGeometry(mesh);
    }

    /// <summary>
    /// Adds a Rhino point to the signature.
    /// </summary>
    public IInputSignatureBuilder AddPoint(Rhino.Geometry.Point3d point)
    {
        _stringBuilder.Append(point.X.ToString(_coordinateFormat));
        _stringBuilder.Append(',');
        _stringBuilder.Append(point.Y.ToString(_coordinateFormat));
        _stringBuilder.Append(',');
        _stringBuilder.Append(point.Z.ToString(_coordinateFormat));
        _stringBuilder.Append(_separator);
        return this;
    }

    /// <inheritdoc />
    public IInputSignatureBuilder AddGeometry(Rhino.Geometry.GeometryBase? geometry)
    {
        if (geometry == null)
        {
            _stringBuilder.Append("null");
            _stringBuilder.Append(_separator);
            return this;
        }

        // DataCRC hashes the complete geometry data (control points, knots, vertices,
        // faces, orientation), so any edit that affects the baked output - including
        // a direction flip, whose reversed control point order changes the data -
        // produces a different value.
        _stringBuilder.Append(geometry.GetType().Name);
        _stringBuilder.Append(',');
        _stringBuilder.Append(geometry.DataCRC(0));
        _stringBuilder.Append(_separator);
        return this;
    }

    /// <inheritdoc />
    public IInputSignatureBuilder AddPoints(IList<Rhino.Geometry.Point3d>? points)
    {
        if (points == null || points.Count == 0)
        {
            _stringBuilder.Append("empty");
            _stringBuilder.Append(_separator);
            return this;
        }

        _stringBuilder.Append(points.Count);
        _stringBuilder.Append(',');

        foreach (var point in points)
        {
            _stringBuilder.Append(point.X.ToString(_coordinateFormat));
            _stringBuilder.Append(',');
            _stringBuilder.Append(point.Y.ToString(_coordinateFormat));
            _stringBuilder.Append(',');
            _stringBuilder.Append(point.Z.ToString(_coordinateFormat));
            _stringBuilder.Append(',');
        }

        _stringBuilder.Append(_separator);
        return this;
    }

    /// <inheritdoc />
    public IInputSignatureBuilder AddScale(IAutocadScale scale)
    {
        _stringBuilder.Append(scale.X.ToString(_coordinateFormat));
        _stringBuilder.Append(',');
        _stringBuilder.Append(scale.Y.ToString(_coordinateFormat));
        _stringBuilder.Append(',');
        _stringBuilder.Append(scale.Z.ToString(_coordinateFormat));
        _stringBuilder.Append(_separator);
        return this;
    }

    /// <inheritdoc />
    public IInputSignatureBuilder AddColor(IAutocadColor? color)
    {
        if (color == null)
        {
            _stringBuilder.Append("null");
        }
        else
        {
            var cadColor = color.Unwrap();

            _stringBuilder.Append(color.ColorIndex);
            _stringBuilder.Append(',');
            _stringBuilder.Append(cadColor.Red);
            _stringBuilder.Append(',');
            _stringBuilder.Append(cadColor.Green);
            _stringBuilder.Append(',');
            _stringBuilder.Append(cadColor.Blue);
        }
        _stringBuilder.Append(_separator);
        return this;
    }

    /// <inheritdoc />
    public IInputSignatureBuilder AddDoubles(IReadOnlyList<double>? values, int decimals = 6)
    {
        if (values == null || values.Count == 0)
        {
            _stringBuilder.Append("empty");
            _stringBuilder.Append(_separator);
            return this;
        }

        _stringBuilder.Append(values.Count);
        _stringBuilder.Append(',');

        foreach (var value in values)
        {
            _stringBuilder.Append(Math.Round(value, decimals));
            _stringBuilder.Append(',');
        }

        _stringBuilder.Append(_separator);
        return this;
    }

    /// <inheritdoc />
    public IInputSignatureBuilder AddScales(IReadOnlyList<IAutocadScale?>? scales)
    {
        if (scales == null || scales.Count == 0)
        {
            _stringBuilder.Append("empty");
            _stringBuilder.Append(_separator);
            return this;
        }

        _stringBuilder.Append(scales.Count);
        _stringBuilder.Append(',');

        foreach (var scale in scales)
        {
            if (scale == null)
            {
                _stringBuilder.Append("null");
            }
            else
            {
                _stringBuilder.Append(scale.X.ToString(_coordinateFormat));
                _stringBuilder.Append(',');
                _stringBuilder.Append(scale.Y.ToString(_coordinateFormat));
                _stringBuilder.Append(',');
                _stringBuilder.Append(scale.Z.ToString(_coordinateFormat));
            }
            _stringBuilder.Append(',');
        }

        _stringBuilder.Append(_separator);
        return this;
    }

    /// <inheritdoc />
    public IInputSignatureBuilder AddObjectIds(IReadOnlyList<IObjectId?>? objectIds)
    {
        if (objectIds == null || objectIds.Count == 0)
        {
            _stringBuilder.Append("empty");
            _stringBuilder.Append(_separator);
            return this;
        }

        _stringBuilder.Append(objectIds.Count);
        _stringBuilder.Append(',');

        foreach (var objectId in objectIds)
        {
            _stringBuilder.Append(objectId?.Value ?? 0L);
            _stringBuilder.Append(',');
        }

        _stringBuilder.Append(_separator);
        return this;
    }

    /// <inheritdoc />
    public IInputSignatureBuilder AddColors(IReadOnlyList<IAutocadColor?>? colors)
    {
        if (colors == null || colors.Count == 0)
        {
            _stringBuilder.Append("empty");
            _stringBuilder.Append(_separator);
            return this;
        }

        _stringBuilder.Append(colors.Count);
        _stringBuilder.Append(',');

        foreach (var color in colors)
        {
            if (color == null)
            {
                _stringBuilder.Append("null");
            }
            else
            {
                var cadColor = color.Unwrap();

                _stringBuilder.Append(color.ColorIndex);
                _stringBuilder.Append(',');
                _stringBuilder.Append(cadColor.Red);
                _stringBuilder.Append(',');
                _stringBuilder.Append(cadColor.Green);
                _stringBuilder.Append(',');
                _stringBuilder.Append(cadColor.Blue);
            }
            _stringBuilder.Append(',');
        }

        _stringBuilder.Append(_separator);
        return this;
    }

    /// <inheritdoc />
    public string Build()
    {
        var raw = _stringBuilder.ToString();

        // For large signatures, use MD5 hash to keep serialization reasonable
        if (raw.Length > _hashedSignatureLengthThreshold)
        {
            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(raw));
            return Convert.ToBase64String(hash);
        }

        return raw;
    }
}
