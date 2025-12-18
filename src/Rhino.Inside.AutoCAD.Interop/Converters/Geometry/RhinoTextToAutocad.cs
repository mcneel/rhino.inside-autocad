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

        // Font table: maps font index (0, 1, 2...) to font name
        var fontTable = new Dictionary<int, string>();

        // Parse font table from RTF
        var fontTableMatch = Regex.Match(richText, @"\{\\fonttbl((?:\{[^}]+\})+)\}");
        if (fontTableMatch.Success)
        {
            var fontEntries = Regex.Matches(fontTableMatch.Groups[1].Value, @"\{\\f(\d+)\s*([^;}]+);?\}");
            foreach (Match entry in fontEntries)
            {
                if (int.TryParse(entry.Groups[1].Value, out var fontIndex))
                {
                    var fontName = entry.Groups[2].Value.Trim();
                    // Handle font names that might have additional RTF codes like \fnil
                    fontName = Regex.Replace(fontName, @"\\[a-z]+\s*", "").Trim();
                    if (!string.IsNullOrEmpty(fontName))
                        fontTable[fontIndex] = fontName;
                }
            }
        }

        // Remove RTF header and font table, get to actual content
        // Pattern: {\rtf1\deff0{\fonttbl...}...actual content...}
        var rtfContent = richText;

        // Remove outer braces
        if (rtfContent.StartsWith("{") && rtfContent.EndsWith("}"))
            rtfContent = rtfContent.Substring(1, rtfContent.Length - 2);

        // Remove \rtf1 and \deff# headers
        rtfContent = Regex.Replace(rtfContent, @"^\\rtf\d*\s*", "");
        rtfContent = Regex.Replace(rtfContent, @"^\\deff\d+\s*", "");

        // Remove font table
        rtfContent = Regex.Replace(rtfContent, @"\{\\fonttbl(?:\{[^}]+\})+\}", "");

        // Remove color table if present
        rtfContent = Regex.Replace(rtfContent, @"\{\\colortbl[^}]*\}", "");

        // Track if we're inside a group that should be skipped (like stylesheet)
        var skipGroupDepth = 0;

        var index = 0;
        while (index < rtfContent.Length)
        {
            var c = rtfContent[index];

            if (skipGroupDepth > 0)
            {
                // We're inside a group to skip
                if (c == '{')
                    skipGroupDepth++;
                else if (c == '}')
                    skipGroupDepth--;
                index++;
                continue;
            }

            if (c == '\\' && index + 1 < rtfContent.Length)
            {
                // Parse RTF control word
                var controlWord = this.ParseRtfControlWord(rtfContent, ref index);
                var lowerWord = controlWord.ToLowerInvariant();

                // Check for font reference \f# (e.g., \f0, \f1)
                if (lowerWord.StartsWith("f") && lowerWord.Length > 1 && char.IsDigit(lowerWord[1]))
                {
                    if (int.TryParse(lowerWord.Substring(1), out var fontIndex) && fontTable.TryGetValue(fontIndex, out var fontName))
                    {
                        currentFormat.FontName = fontName;
                        // Output MText font change
                        result.Append($"\\f{fontName}|b{(currentFormat.Bold ? "1" : "0")}|i{(currentFormat.Italic ? "1" : "0")};");
                    }
                    continue;
                }

                // Check for font size \fs# - skip it
                if (lowerWord.StartsWith("fs") && lowerWord.Length > 2)
                    continue;

                switch (lowerWord)
                {
                    case "b":
                        currentFormat.Bold = true;
                        var boldFont = currentFormat.FontName ?? baseFontName;
                        result.Append($"\\f{boldFont}|b1|i{(currentFormat.Italic ? "1" : "0")};");
                        break;

                    case "b0":
                        currentFormat.Bold = false;
                        var unboldFont = currentFormat.FontName ?? baseFontName;
                        result.Append($"\\f{unboldFont}|b0|i{(currentFormat.Italic ? "1" : "0")};");
                        break;

                    case "i":
                        currentFormat.Italic = true;
                        var italicFont = currentFormat.FontName ?? baseFontName;
                        result.Append($"\\f{italicFont}|b{(currentFormat.Bold ? "1" : "0")}|i1;");
                        break;

                    case "i0":
                        currentFormat.Italic = false;
                        var unitalicFont = currentFormat.FontName ?? baseFontName;
                        result.Append($"\\f{unitalicFont}|b{(currentFormat.Bold ? "1" : "0")}|i0;");
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

                    case "stylesheet":
                    case "info":
                    case "fonttbl":
                    case "colortbl":
                        // Skip these groups entirely - they should have been removed but just in case
                        skipGroupDepth = 1;
                        break;

                    default:
                        // Skip unknown control words (like \deff0, \fs23, etc.)
                        break;
                }
            }
            else if (c == '{')
            {
                // Push current format state - but don't output braces for MText
                // MText uses braces differently than RTF
                formatStack.Push(currentFormat.Clone());
                index++;
            }
            else if (c == '}')
            {
                // Pop format state
                if (formatStack.Count > 0)
                {
                    var previousFormat = currentFormat;
                    currentFormat = formatStack.Pop();

                    // If format changed, output the restored format
                    if (previousFormat.FontName != currentFormat.FontName ||
                        previousFormat.Bold != currentFormat.Bold ||
                        previousFormat.Italic != currentFormat.Italic)
                    {
                        var restoredFont = currentFormat.FontName ?? baseFontName;
                        result.Append($"\\f{restoredFont}|b{(currentFormat.Bold ? "1" : "0")}|i{(currentFormat.Italic ? "1" : "0")};");
                    }

                    if (previousFormat.Underline && !currentFormat.Underline)
                        result.Append("\\l");
                }
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

        mtext.TextHeight = _unitSystemManager.ToAutoCadLength(rhinoText.TextHeight * rhinoText.DimensionScale);

        if (rhinoText.FormatWidth > 0)
            mtext.Width = _unitSystemManager.ToAutoCadLength(rhinoText.FormatWidth * rhinoText.DimensionScale);

        mtext.Location = this.ToAutoCadType(rhinoText.Plane.Origin);

        mtext.Normal = this.ToAutoCadType(rhinoText.Plane.ZAxis);

        var xAxis = rhinoText.Plane.XAxis;
        mtext.Rotation = Math.Atan2(xAxis.Y, xAxis.X);

        mtext.Attachment = this.ConvertJustification(rhinoText.Justification);

        return mtext;
    }
}
