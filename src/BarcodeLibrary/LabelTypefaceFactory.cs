using SkiaSharp;

namespace Genocs.BarcodeLibrary;

/// <summary>
/// Resolves a typeface that can render barcode labels on Windows and Linux.
/// SkiaSharp cannot draw text when the requested family (typically Arial) is missing,
/// which is the default situation in slim container images.
/// </summary>
internal static class LabelTypefaceFactory
{
    private static readonly string[] FamilyFallbacks =
    {
        "Arial",
        "Liberation Sans",
        "DejaVu Sans",
        "FreeSans",
        "Nimbus Sans"
    };

    /// <summary>
    /// Creates a usable label typeface, preferring Arial and then common Linux equivalents.
    /// </summary>
    /// <param name="fontStyle">Optional style. Defaults to bold.</param>
    /// <returns>A typeface with glyphs, or <see cref="SKTypeface.Default"/> as a last resort.</returns>
    public static SKTypeface Create(SKFontStyle? fontStyle = null)
    {
        fontStyle ??= SKFontStyle.Bold;

        foreach (string family in FamilyFallbacks)
        {
            SKTypeface typeface = SKTypeface.FromFamilyName(family, fontStyle);
            if (IsUsable(typeface))
            {
                return typeface;
            }

            typeface.Dispose();
        }

        return SKTypeface.Default;
    }

    private static bool IsUsable(SKTypeface typeface)
        => typeface.GlyphCount > 0;
}
