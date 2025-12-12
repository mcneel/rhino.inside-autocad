using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Geometry;
using System.Text;
using System.Text.RegularExpressions;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A class which provides conversion methods between AutoCAD and Rhino Text types,
/// </summary>
public partial class GeometryConverter
{
    /// <summary>
    /// Converts a Rhino <see cref="TextJustification"/> to an AutoCAD <see cref="AttachmentPoint"/>.
    /// </summary>
    private AttachmentPoint ConvertJustification(TextJustification justification)
    {
        return justification switch
        {
            TextJustification.TopLeft => AttachmentPoint.TopLeft,
            TextJustification.TopCenter => AttachmentPoint.TopCenter,
            TextJustification.TopRight => AttachmentPoint.TopRight,
            TextJustification.MiddleLeft => AttachmentPoint.MiddleLeft,
            TextJustification.MiddleCenter => AttachmentPoint.MiddleCenter,
            TextJustification.MiddleRight => AttachmentPoint.MiddleRight,
            TextJustification.BottomLeft => AttachmentPoint.BottomLeft,
            TextJustification.BottomCenter => AttachmentPoint.BottomCenter,
            TextJustification.BottomRight => AttachmentPoint.BottomRight,
            _ => AttachmentPoint.BottomLeft
        };
    }

    /// <summary>
    /// Converts Rhino rich text (RTF format) to AutoCAD MText formatting codes.
    /// </summary>
    /// <param name="rhinoText">The Rhino text entity to convert.</param>
    /// <returns>A string containing MText-formatted content.</returns>
    private string ConvertRichTextToMText(TextEntity rhinoText)
    {
        var richText = rhinoText.RichText;

        // If no rich text or it equals plain text, return plain text
        if (string.IsNullOrEmpty(richText) || richText == rhinoText.PlainText)
            return rhinoText.PlainText;

        // Check if it's actually RTF format
        if (!richText.StartsWith("{\\rtf", StringComparison.Ordinal))
            return rhinoText.PlainText;

        var result = new StringBuilder();
        var formatStack = new Stack<FormatState>();
        var currentFormat = new FormatState();
        var baseFontName = rhinoText.DimensionStyle?.Font?.FamilyName ?? "Arial";

        // Remove RTF header wrapper - extract content between {\rtf1 ... }
        var rtfContent = richText;
        if (rtfContent.StartsWith("{\\rtf1", StringComparison.Ordinal))
        {
            // Find the matching closing brace
            var depth = 0;
            var startIndex = 0;
            for (var i = 0; i < rtfContent.Length; i++)
            {
                if (rtfContent[i] == '{')
                {
                    if (depth == 0) startIndex = i + 1;
                    depth++;
                }
                else if (rtfContent[i] == '}')
                {
                    depth--;
                    if (depth == 0)
                    {
                        rtfContent = rtfContent.Substring(startIndex, i - startIndex);
                        break;
                    }
                }
            }
        }

        // Skip past {\rtf1 header to actual content
        var headerMatch = Regex.Match(rtfContent, @"^\\rtf1\s*");
        if (headerMatch.Success)
            rtfContent = rtfContent.Substring(headerMatch.Length);

        var index = 0;
        while (index < rtfContent.Length)
        {
            var c = rtfContent[index];

            if (c == '\\' && index + 1 < rtfContent.Length)
            {
                // Parse RTF control word
                var controlWord = this.ParseRtfControlWord(rtfContent, ref index);

                switch (controlWord.ToLowerInvariant())
                {
                    case "b":
                        currentFormat.Bold = true;
                        result.Append($"\\F{baseFontName}|b1|i{(currentFormat.Italic ? "1" : "0")}|c0|p0;");
                        break;

                    case "b0":
                        currentFormat.Bold = false;
                        result.Append($"\\F{baseFontName}|b0|i{(currentFormat.Italic ? "1" : "0")}|c0|p0;");
                        break;

                    case "i":
                        currentFormat.Italic = true;
                        result.Append($"\\F{baseFontName}|b{(currentFormat.Bold ? "1" : "0")}|i1|c0|p0;");
                        break;

                    case "i0":
                        currentFormat.Italic = false;
                        result.Append($"\\F{baseFontName}|b{(currentFormat.Bold ? "1" : "0")}|i0|c0|p0;");
                        break;

                    case "ul":
                        currentFormat.Underline = true;
                        result.Append("\\L");
                        break;

                    case "ulnone":
                        currentFormat.Underline = false;
                        result.Append("\\l");
                        break;

                    case "par":
                        result.Append("\\P");
                        break;

                    case "\\":
                        result.Append("\\\\");
                        break;

                    case "{":
                        result.Append("\\{");
                        break;

                    case "}":
                        result.Append("\\}");
                        break;

                    default:
                        // Skip unknown control words
                        break;
                }
            }
            else if (c == '{')
            {
                // Push current format state
                formatStack.Push(currentFormat.Clone());
                result.Append('{');
                index++;
            }
            else if (c == '}')
            {
                // Pop format state
                if (formatStack.Count > 0)
                    currentFormat = formatStack.Pop();
                result.Append('}');
                index++;
            }
            else if (c == '\r' || c == '\n')
            {
                // Skip newlines in RTF (they're formatting artifacts)
                index++;
            }
            else
            {
                // Regular character
                result.Append(c);
                index++;
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// Parses an RTF control word starting at the current index.
    /// </summary>
    /// <param name="content">
    /// The RTF content string.
    /// </param>
    /// <param name="index">
    /// The current position (should be at backslash). Updated to after the control word.
    /// </param>
    /// <returns>
    /// The control word without the leading backslash.
    /// </returns>
    private string ParseRtfControlWord(string content, ref int index)
    {
        // Skip the backslash
        index++;

        if (index >= content.Length)
            return string.Empty;

        // Check for special escaped characters
        var firstChar = content[index];
        if (firstChar == '\\' || firstChar == '{' || firstChar == '}')
        {
            index++;
            return firstChar.ToString();
        }

        // Parse alphabetic control word
        var sb = new StringBuilder();
        while (index < content.Length && char.IsLetter(content[index]))
        {
            sb.Append(content[index]);
            index++;
        }

        // Parse optional numeric parameter
        if (index < content.Length && (char.IsDigit(content[index]) || content[index] == '-'))
        {
            while (index < content.Length && (char.IsDigit(content[index]) || content[index] == '-'))
            {
                sb.Append(content[index]);
                index++;
            }
        }

        // Skip optional space delimiter
        if (index < content.Length && content[index] == ' ')
            index++;

        return sb.ToString();
    }

    /// <summary>
    /// Converts a Rhino <see cref="TextEntity"/> to an Autocad <see cref="MText"/>.
    /// </summary>
    public MText ToAutoCadType(TextEntity rhinoText)
    {
        var mtext = new MText();

        var content = this.ConvertRichTextToMText(rhinoText);
        mtext.Contents = content;

        mtext.Height = _unitSystemManager.ToAutoCadLength(rhinoText.TextHeight * rhinoText.DimensionScale);

        if (rhinoText.FormatWidth > 0)
            mtext.Width = _unitSystemManager.ToAutoCadLength(rhinoText.FormatWidth);

        mtext.Location = this.ToAutoCadType(rhinoText.Plane.Origin);

        mtext.Normal = this.ToAutoCadType(rhinoText.Plane.ZAxis);

        var xAxis = rhinoText.Plane.XAxis;
        mtext.Rotation = Math.Atan2(xAxis.Y, xAxis.X);

        mtext.Attachment = this.ConvertJustification(rhinoText.Justification);

        return mtext;
    }
}
