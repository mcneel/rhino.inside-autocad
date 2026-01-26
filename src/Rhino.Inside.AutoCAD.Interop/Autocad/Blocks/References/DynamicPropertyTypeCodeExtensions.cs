using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Rhino.Inside.AutoCAD.Core;
using System.Globalization;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Extension methods for <see cref="DynamicPropertyTypeCode"/> to handle type conversion.
/// </summary>
public static class DynamicPropertyTypeCodeExtensions
{
    /// <summary>
    /// Attempts to convert the specified value to the type represented by this <see cref="DynamicPropertyTypeCode"/>.
    /// </summary>
    /// <param name="typeCode">The property type code.</param>
    /// <param name="value">The value to convert.</param>
    /// <param name="result">The converted value if successful; otherwise, null.</param>
    /// <returns>True if conversion was successful; otherwise, false.</returns>
    public static bool TryConvertValue(this DynamicPropertyTypeCode typeCode, object value, out object result)
    {
        result = null;

        if (value == null)
        {
            return typeCode == DynamicPropertyTypeCode.Null;
        }

        try
        {
            result = typeCode switch
            {
                DynamicPropertyTypeCode.Null => null,
                DynamicPropertyTypeCode.Real => ConvertToDouble(value),
                DynamicPropertyTypeCode.Int32 => ConvertToInt32(value),
                DynamicPropertyTypeCode.Int16 => ConvertToInt16(value),
                DynamicPropertyTypeCode.Int8 => ConvertToInt8(value),
                DynamicPropertyTypeCode.Text => ConvertToString(value),
                DynamicPropertyTypeCode.BChunk => ConvertToBChunk(value),
                DynamicPropertyTypeCode.Handle => ConvertToHandle(value),
                DynamicPropertyTypeCode.HardOwnershipId => ConvertToObjectId(value),
                DynamicPropertyTypeCode.SoftOwnershipId => ConvertToObjectId(value),
                DynamicPropertyTypeCode.HardPointerId => ConvertToObjectId(value),
                DynamicPropertyTypeCode.SoftPointerId => ConvertToObjectId(value),
                DynamicPropertyTypeCode.Real3 => ConvertToPoint3d(value),
                DynamicPropertyTypeCode.Int64 => ConvertToInt64(value),
                DynamicPropertyTypeCode.NotRecognized => value, // Pass through as-is
                _ => throw new ArgumentOutOfRangeException(nameof(typeCode), typeCode, "Unsupported DynamicPropertyTypeCode")
            };

            return typeCode == DynamicPropertyTypeCode.Null || result != null;
        }
        catch
        {
            result = null;
            return false;
        }
    }

    #region Private Conversion Methods

    private static double ConvertToDouble(object value)
    {
        return value switch
        {
            double d => d,
            float f => f,
            decimal dec => (double)dec,
            int i => i,
            long l => l,
            short s => s,
            sbyte sb => sb,
            string str => double.Parse(str, CultureInfo.InvariantCulture),
            IConvertible conv => conv.ToDouble(CultureInfo.InvariantCulture),
            _ => throw new InvalidCastException($"Cannot convert {value.GetType().Name} to double")
        };
    }

    private static int ConvertToInt32(object value)
    {
        return value switch
        {
            int i => i,
            short s => s,
            sbyte sb => sb,
            long l when l >= int.MinValue && l <= int.MaxValue => (int)l,
            double d when d >= int.MinValue && d <= int.MaxValue && IsWholeNumber(d) => (int)d,
            float f when f >= int.MinValue && f <= int.MaxValue && IsWholeNumber(f) => (int)f,
            string str => int.Parse(str, CultureInfo.InvariantCulture),
            IConvertible conv => conv.ToInt32(CultureInfo.InvariantCulture),
            _ => throw new InvalidCastException($"Cannot convert {value.GetType().Name} to Int32")
        };
    }

    private static short ConvertToInt16(object value)
    {
        return value switch
        {
            short s => s,
            sbyte sb => sb,
            int i when i >= short.MinValue && i <= short.MaxValue => (short)i,
            long l when l >= short.MinValue && l <= short.MaxValue => (short)l,
            double d when d >= short.MinValue && d <= short.MaxValue && IsWholeNumber(d) => (short)d,
            string str => short.Parse(str, CultureInfo.InvariantCulture),
            IConvertible conv => conv.ToInt16(CultureInfo.InvariantCulture),
            _ => throw new InvalidCastException($"Cannot convert {value.GetType().Name} to Int16")
        };
    }

    private static sbyte ConvertToInt8(object value)
    {
        return value switch
        {
            sbyte sb => sb,
            byte b when b <= sbyte.MaxValue => (sbyte)b,
            short s when s >= sbyte.MinValue && s <= sbyte.MaxValue => (sbyte)s,
            int i when i >= sbyte.MinValue && i <= sbyte.MaxValue => (sbyte)i,
            double d when d >= sbyte.MinValue && d <= sbyte.MaxValue && IsWholeNumber(d) => (sbyte)d,
            string str => sbyte.Parse(str, CultureInfo.InvariantCulture),
            IConvertible conv => conv.ToSByte(CultureInfo.InvariantCulture),
            _ => throw new InvalidCastException($"Cannot convert {value.GetType().Name} to Int8")
        };
    }

    private static long ConvertToInt64(object value)
    {
        return value switch
        {
            long l => l,
            int i => i,
            short s => s,
            sbyte sb => sb,
            double d when d >= long.MinValue && d <= long.MaxValue && IsWholeNumber(d) => (long)d,
            string str => long.Parse(str, CultureInfo.InvariantCulture),
            IConvertible conv => conv.ToInt64(CultureInfo.InvariantCulture),
            _ => throw new InvalidCastException($"Cannot convert {value.GetType().Name} to Int64")
        };
    }

    private static string ConvertToString(object value)
    {
        return value switch
        {
            string s => s,
            IFormattable f => f.ToString(null, CultureInfo.InvariantCulture),
            _ => value.ToString()
        };
    }

    private static byte[] ConvertToBChunk(object value)
    {
        return value switch
        {
            byte[] bytes => bytes,
            string base64 => Convert.FromBase64String(base64),
            IEnumerable<byte> enumerable => new List<byte>(enumerable).ToArray(),
            _ => throw new InvalidCastException($"Cannot convert {value.GetType().Name} to byte[]")
        };
    }

    private static Handle ConvertToHandle(object value)
    {
        return value switch
        {
            Handle h => h,
            long l => new Handle(l),
            int i => new Handle(i),
            string str when long.TryParse(str, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var hex) => new Handle(hex),
            string str when long.TryParse(str, out var dec) => new Handle(dec),
            _ => throw new InvalidCastException($"Cannot convert {value.GetType().Name} to Handle")
        };
    }

    private static ObjectId ConvertToObjectId(object value)
    {
        return value switch
        {
            ObjectId id => id,
            long l => new ObjectId((IntPtr)l),
            int i => new ObjectId((IntPtr)i),
            IntPtr ptr => new ObjectId(ptr),
            _ => throw new InvalidCastException($"Cannot convert {value.GetType().Name} to ObjectId")
        };
    }

    private static Point3d ConvertToPoint3d(object value)
    {
        return value switch
        {
            Point3d p => p,
            Vector3d v => new Point3d(v.X, v.Y, v.Z),
            Point2d p2 => new Point3d(p2.X, p2.Y, 0),
            double[] arr when arr.Length >= 3 => new Point3d(arr[0], arr[1], arr[2]),
            double[] arr when arr.Length == 2 => new Point3d(arr[0], arr[1], 0),
            object[] arr when arr.Length >= 3 => new Point3d(
                Convert.ToDouble(arr[0], CultureInfo.InvariantCulture),
                Convert.ToDouble(arr[1], CultureInfo.InvariantCulture),
                Convert.ToDouble(arr[2], CultureInfo.InvariantCulture)),
            _ => throw new InvalidCastException($"Cannot convert {value.GetType().Name} to Point3d")
        };
    }

    private static bool IsWholeNumber(double value)
    {
        return Math.Abs(value - Math.Round(value)) < 1e-9;
    }

    #endregion
}