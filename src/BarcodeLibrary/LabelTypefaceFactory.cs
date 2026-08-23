using SkiaSharp;

namespace Genocs.BarcodeLibrary;

/// <summary>
/// Resolves a typeface that can render barcode labels on Windows and Linux.
/// SkiaSharp cannot draw text when the requested family (typically Arial) is missing,
/// which is the default situation in slim container images.
/// </summary>
internal static class LabelTypefaceFactory
{
    internal const string FontFamilyEnvironmentVariable = "GENOCS_BARCODE_LABEL_FONT";
    internal const string ContainerEnvironmentVariable = "DOTNET_RUNNING_IN_CONTAINER";

    private const string UbuntuFamilyName = "Ubuntu";

    private static readonly string[] UbuntuBoldFiles =
    {
        "/usr/share/fonts/truetype/ubuntu/Ubuntu-B.ttf",
        "/usr/share/fonts/truetype/ubuntu/Ubuntu[wdth,wght].ttf"
    };

    private static readonly string[] UbuntuRegularFiles =
    {
        "/usr/share/fonts/truetype/ubuntu/Ubuntu-R.ttf",
        "/usr/share/fonts/truetype/ubuntu/Ubuntu[wdth,wght].ttf"
    };

    private static readonly string[] FamilyFallbacks =
    {
        "Arial",
        "Liberation Sans",
        "DejaVu Sans",
        "FreeSans",
        "Nimbus Sans"
    };

    /// <summary>
    /// Creates a usable label typeface.
    /// In Docker (or when <c>GENOCS_BARCODE_LABEL_FONT=Ubuntu</c>) this prefers the Ubuntu font
    /// installed in the container; otherwise Arial and common Linux equivalents are used.
    /// </summary>
    /// <param name="fontStyle">Optional style. Defaults to bold.</param>
    /// <returns>A typeface with glyphs, or <see cref="SKTypeface.Default"/> as a last resort.</returns>
    public static SKTypeface Create(SKFontStyle? fontStyle = null)
    {
        fontStyle ??= SKFontStyle.Bold;

        string? preferredFamily = GetPreferredFamily();
        if (!string.IsNullOrWhiteSpace(preferredFamily))
        {
            SKTypeface? preferred = TryCreatePreferred(preferredFamily, fontStyle);
            if (preferred != null)
            {
                return preferred;
            }
        }

        foreach (string family in FamilyFallbacks)
        {
            SKTypeface? typeface = TryFromFamily(family, fontStyle);
            if (typeface != null)
            {
                return typeface;
            }
        }

        return SKTypeface.Default;
    }

    internal static bool IsRunningInContainer()
        => string.Equals(
            Environment.GetEnvironmentVariable(ContainerEnvironmentVariable),
            "true",
            StringComparison.OrdinalIgnoreCase);

    private static string? GetPreferredFamily()
    {
        string? configured = Environment.GetEnvironmentVariable(FontFamilyEnvironmentVariable);
        if (!string.IsNullOrWhiteSpace(configured))
        {
            return configured.Trim();
        }

        return IsRunningInContainer() ? UbuntuFamilyName : null;
    }

    private static SKTypeface? TryCreatePreferred(string family, SKFontStyle fontStyle)
    {
        if (family.Equals(UbuntuFamilyName, StringComparison.OrdinalIgnoreCase))
        {
            SKTypeface? fromFile = TryFromFiles(GetUbuntuFiles(fontStyle));
            if (fromFile != null)
            {
                return fromFile;
            }
        }

        return TryFromFamily(family, fontStyle);
    }

    private static string[] GetUbuntuFiles(SKFontStyle fontStyle)
        => fontStyle.Weight >= (int)SKFontStyleWeight.SemiBold ? UbuntuBoldFiles : UbuntuRegularFiles;

    private static SKTypeface? TryFromFiles(IEnumerable<string> paths)
    {
        foreach (string path in paths)
        {
            if (!File.Exists(path))
            {
                continue;
            }

            SKTypeface? typeface = SKTypeface.FromFile(path);
            if (IsUsable(typeface))
            {
                return typeface;
            }

            typeface?.Dispose();
        }

        return null;
    }

    private static SKTypeface? TryFromFamily(string family, SKFontStyle fontStyle)
    {
        SKTypeface typeface = SKTypeface.FromFamilyName(family, fontStyle);
        if (IsUsable(typeface))
        {
            return typeface;
        }

        typeface.Dispose();
        return null;
    }

    private static bool IsUsable(SKTypeface? typeface)
        => typeface is { GlyphCount: > 0 };
}
