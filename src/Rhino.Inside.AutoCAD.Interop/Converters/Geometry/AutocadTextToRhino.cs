using Autodesk.AutoCAD.DatabaseServices;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Text;
using CadTextStyleTableRecord = Autodesk.AutoCAD.DatabaseServices.TextStyleTableRecord;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A class which provides conversion methods between AutoCAD and Rhino Text types,
/// </summary>
public partial class GeometryConverter
{

    /// <summary>
    /// Parses a font code string to extract font information such as font name, bold,
    /// and italic attributes.
    /// </summary>
    /// <param name="content">
    /// The string containing the font code to parse.
    /// </param>
    /// <param name="index">
    /// A reference to the current position in the string. This value is updated to
    /// point to the next character after the parsed font code.
    /// </param>
    /// <returns
    /// >A <see cref="FontInfo"/> object containing the parsed font name
    /// and style attributes.
    /// </returns>
    private FontInfo ParseFontCode(string content, ref int index)
    {
        var info = new FontInfo();
        var sb = new StringBuilder();

        while (index < content.Length && content[index] != ';')
        {
            sb.Append(content[index]);
            index++;
        }

        if (index < content.Length) index++; // Skip semicolon

        // Parse: FontName|b1|i0 or FontName|b0|i1
        var parts = sb.ToString().Split('|');
        if (parts.Length >= 1)
            info.FontName = parts[0];

        foreach (var part in parts.Skip(1))
        {
            if (part.StartsWith("b", StringComparison.OrdinalIgnoreCase))
                info.Bold = part.Length > 1 && part[1] == '1';
            else if (part.StartsWith("i", StringComparison.OrdinalIgnoreCase))
                info.Italic = part.Length > 1 && part[1] == '1';
        }

        return info;
    }

    /// <summary>
    /// Parses a stacked text string (e.g., fractions) and converts it into a plain
    /// text representation.
    /// </summary>
    /// <param name="content">
    /// The string containing the stacked text to parse.
    /// </param>
    /// <param name="index">
    /// A reference to the current position in the string. This value is updated to
    /// point to the next character after the parsed stacked text.
    /// </param>
    /// <returns>
    /// A plain text representation of the stacked text.
    /// </returns>
    private string ParseStackedText(string content, ref int index)
    {
        var stringBuilder = new StringBuilder();
        var stackChar = ' ';

        while (index < content.Length && content[index] != ';')
        {
            var character = content[index];
            if (character == '^' || character == '/' || character == '#')
            {
                stackChar = character;
                stringBuilder.Append(character == '#' ? ' ' : character); // # is horizontal, show as space
            }
            else
            {
                stringBuilder.Append(character);
            }
            index++;
        }

        if (index < content.Length) index++; // Skip semicolon

        return stringBuilder.ToString();
    }

    /// <summary>
    /// Advances the index in the string until a semicolon (';') is encountered.
    /// </summary>
    /// <param name="content">
    /// The string to scan.
    /// </param>
    /// <param name="index">
    /// A reference to the current position in the string. This value is updated to point
    /// to the next character after the semicolon.
    /// </param>
    private void SkipUntilSemicolon(string content, ref int index)
    {
        while (index < content.Length && content[index] != ';')
            index++;
        if (index < content.Length) index++; // Skip the semicolon
    }

    /// <summary>
    /// Advances the index in the string until a semicolon (';') or a space (' ') is
    /// encountered.
    /// </summary>
    /// <param name="content">
    /// The string to scan.
    /// </param>
    /// <param name="index">
    /// A reference to the current position in the string. This value is updated to
    /// point to the next character after the semicolon or space.
    /// </param>
    private void SkipUntilSemicolonOrSpace(string content, ref int index)
    {
        while (index < content.Length && content[index] != ';' && content[index] != ' ')
            index++;
        if (index < content.Length && content[index] == ';') index++;
    }

    /// <summary>
    /// Parses MText content and converts to plain text and RTF rich text.
    /// </summary>
    private (string plainText, string richText) ConvertMTextContent(string mTextContent)
    {
        if (string.IsNullOrEmpty(mTextContent))
            return (string.Empty, string.Empty);

        var plainBuilder = new StringBuilder();
        var richBuilder = new StringBuilder();
        var formatStack = new Stack<FormatState>();
        var currentFormat = new FormatState();

        var i = 0;
        while (i < mTextContent.Length)
        {
            var c = mTextContent[i];

            if (c == '\\' && i + 1 < mTextContent.Length)
            {
                var next = mTextContent[i + 1];

                switch (next)
                {
                    case 'P': // Paragraph break
                        plainBuilder.AppendLine();
                        richBuilder.Append("\\par ");
                        i += 2;
                        break;

                    case 'L': // Underline on
                        currentFormat.Underline = true;
                        richBuilder.Append("{\\ul ");
                        i += 2;
                        break;

                    case 'l': // Underline off
                        currentFormat.Underline = false;
                        richBuilder.Append("}");
                        i += 2;
                        break;

                    case 'O': // Overline on (no Rhino equivalent)
                    case 'o': // Overline off
                        i += 2;
                        break;

                    case 'K': // Strikethrough on (limited Rhino support)
                    case 'k': // Strikethrough off
                        i += 2;
                        break;

                    case 'f': // Font change: \fArial|b1|i0;
                        i += 2;
                        var fontInfo = this.ParseFontCode(mTextContent, ref i);
                        if (fontInfo.Bold) richBuilder.Append("{\\b ");
                        if (fontInfo.Italic) richBuilder.Append("{\\i ");
                        currentFormat.Bold = fontInfo.Bold;
                        currentFormat.Italic = fontInfo.Italic;
                        currentFormat.FontName = fontInfo.FontName;
                        break;

                    case 'H': // Height change: \H2.5; or \H1.5x;
                        i += 2;
                        this.SkipUntilSemicolon(mTextContent, ref i);
                        break;

                    case 'W': // Width factor: \W1.2;
                        i += 2;
                        this.SkipUntilSemicolon(mTextContent, ref i);
                        break;

                    case 'C': // Color by ACI: \C1;
                        i += 2;
                        this.SkipUntilSemicolon(mTextContent, ref i);
                        // Rhino rich text doesn't support inline colors
                        break;

                    case 'c': // True color: \c16711680;
                        i += 2;
                        this.SkipUntilSemicolon(mTextContent, ref i);
                        break;

                    case 'S': // Stacked fraction: \S1/2;
                        i += 2;
                        var fractionText = this.ParseStackedText(mTextContent, ref i);
                        plainBuilder.Append(fractionText);
                        richBuilder.Append(fractionText);
                        break;

                    case '\\': // Escaped backslash
                        plainBuilder.Append('\\');
                        richBuilder.Append("\\\\");
                        i += 2;
                        break;

                    case '{': // Escaped brace
                        plainBuilder.Append('{');
                        richBuilder.Append("\\{");
                        i += 2;
                        break;

                    case '}': // Escaped brace
                        plainBuilder.Append('}');
                        richBuilder.Append("\\}");
                        i += 2;
                        break;

                    default:
                        // Unknown code, skip
                        i += 2;
                        this.SkipUntilSemicolonOrSpace(mTextContent, ref i);
                        break;
                }
            }
            else if (c == '{')
            {
                // Push current format state
                formatStack.Push(currentFormat.Clone());
                richBuilder.Append('{');
                i++;
            }
            else if (c == '}')
            {
                // Pop format state
                if (formatStack.Count > 0)
                    currentFormat = formatStack.Pop();
                richBuilder.Append('}');
                i++;
            }
            else
            {
                // Regular character
                plainBuilder.Append(c);

                // Escape special RTF characters
                if (c == '\\' || c == '{' || c == '}')
                    richBuilder.Append('\\');
                richBuilder.Append(c);
                i++;
            }
        }

        var plain = plainBuilder.ToString();
        var rich = richBuilder.ToString();

        var hasFormatting = rich.Contains("{\\") || rich.Contains("\\par");

        if (hasFormatting)
        {
            rich = $"{{\\rtf1 {rich}}}";
        }
        else
        {
            rich = string.Empty;
        }

        return (plain, rich);
    }

    /// <summary>
    /// Converts an AutoCAD TextStyle to a Rhino DimensionStyle.
    /// </summary>
    public DimensionStyle? ConvertTextStyle(ObjectId textStyleId, ITransactionManager transactionManager)
    {
        if (textStyleId.IsNull)
            return null;

        var transaction = transactionManager.Unwrap();

        var textStyle = (CadTextStyleTableRecord)transaction.GetObject(textStyleId, OpenMode.ForRead);

        if (textStyle == null)
            return null;

        var dimStyle = new DimensionStyle();

        // Set the font
        var fontName = textStyle.Font.TypeFace;

        if (string.IsNullOrEmpty(fontName))
            fontName = textStyle.FileName; // SHX font fallback

        var font = new Rhino.DocObjects.Font(
            fontName,
            textStyle.Font.Bold ? Rhino.DocObjects.Font.FontWeight.Bold : Rhino.DocObjects.Font.FontWeight.Normal,
            textStyle.Font.Italic ? Rhino.DocObjects.Font.FontStyle.Italic : Rhino.DocObjects.Font.FontStyle.Upright,
            false, // underlined - not stored in text style
            false  // strikethrough
        );

        dimStyle.Font = font;

        // Text height (if specified in style)
        if (textStyle.TextSize > 0)
            dimStyle.TextHeight = textStyle.TextSize;

        // Check for vertical text using bitwise operation
        // Vertical flag is bit 4 (0x04) in TextStyleTableRecord.FlagBits
        const byte verticalFlag = 0x04;
        var isVertical = (textStyle.FlagBits & verticalFlag) != 0;

        if (isVertical)
            dimStyle.TextVerticalAlignment = TextVerticalAlignment.Top;

        return dimStyle;

    }

    /// <summary>
    /// Converts an AutoCAD <see cref="AttachmentPoint"/> to a Rhino <see cref="TextJustification"/>.
    /// </summary>
    private TextJustification ConvertAttachmentPoint(AttachmentPoint attachment)
    {
        return attachment switch
        {
            AttachmentPoint.TopLeft => TextJustification.TopLeft,
            AttachmentPoint.TopCenter => TextJustification.TopCenter,
            AttachmentPoint.TopRight => TextJustification.TopRight,
            AttachmentPoint.MiddleLeft => TextJustification.MiddleLeft,
            AttachmentPoint.MiddleCenter => TextJustification.MiddleCenter,
            AttachmentPoint.MiddleRight => TextJustification.MiddleRight,
            AttachmentPoint.BottomLeft => TextJustification.BottomLeft,
            AttachmentPoint.BottomCenter => TextJustification.BottomCenter,
            AttachmentPoint.BottomRight => TextJustification.BottomRight,
            _ => TextJustification.BottomLeft
        };
    }

    /// <summary>
    /// Converts an AutoCAD <see cref="MText"/> to a Rhino <see cref="TextEntity"/>.
    /// </summary>
    public TextEntity ToRhinoType(MText cadText)
    {
        var rhinoText = new TextEntity();

        var (plainText, richText) = this.ConvertMTextContent(cadText.Contents);

        if (!string.IsNullOrEmpty(richText))
            rhinoText.RichText = richText;
        else
            rhinoText.PlainText = plainText;

        rhinoText.TextHeight = cadText.Height / RhinoDoc.ActiveDoc.ModelSpaceTextScale;

        if (cadText.Width > 0)
            rhinoText.FormatWidth = cadText.Width;

        var origin = this.ToRhinoType(cadText.Location);

        var normal = this.ToRhinoType(cadText.Normal);

        var plane = new Plane(origin, normal);

        plane.Rotate(cadText.Rotation, plane.ZAxis);
        rhinoText.Plane = plane;

        rhinoText.Justification = this.ConvertAttachmentPoint(cadText.Attachment);

        return rhinoText;
    }
}