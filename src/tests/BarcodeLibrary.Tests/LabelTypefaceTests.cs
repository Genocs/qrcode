using SkiaSharp;
using Xunit;

namespace Genocs.BarcodeLibrary.UnitTests;

public class LabelTypefaceTests
{
    [Fact]
    public void create_label_typeface_has_glyphs()
    {
        using SKTypeface typeface = Barcode.CreateLabelTypeface();

        Assert.True(typeface.GlyphCount > 0);
    }

    [Fact]
    public void create_falls_back_when_container_ubuntu_font_is_missing()
    {
        string? previousContainer = Environment.GetEnvironmentVariable(LabelTypefaceFactory.ContainerEnvironmentVariable);
        string? previousFont = Environment.GetEnvironmentVariable(LabelTypefaceFactory.FontFamilyEnvironmentVariable);
        try
        {
            Environment.SetEnvironmentVariable(LabelTypefaceFactory.ContainerEnvironmentVariable, "true");
            Environment.SetEnvironmentVariable(LabelTypefaceFactory.FontFamilyEnvironmentVariable, "Ubuntu");

            using SKTypeface typeface = Barcode.CreateLabelTypeface();

            Assert.True(typeface.GlyphCount > 0);
        }
        finally
        {
            Environment.SetEnvironmentVariable(LabelTypefaceFactory.ContainerEnvironmentVariable, previousContainer);
            Environment.SetEnvironmentVariable(LabelTypefaceFactory.FontFamilyEnvironmentVariable, previousFont);
        }
    }

    [Fact]
    public void include_label_draws_text_on_the_image()
    {
        using var unlabeledBarcode = new Barcode { IncludeLabel = false };
        using SKImage unlabeled = unlabeledBarcode.Encode(BarcodeType.Code128, "Hello-128", width: 400, height: 140);

        using var labeledBarcode = new Barcode { IncludeLabel = true };
        using SKImage labeled = labeledBarcode.Encode(BarcodeType.Code128, "Hello-128", width: 400, height: 140);

        using SKData unlabeledPng = unlabeled.Encode(SKEncodedImageFormat.Png, 100);
        using SKData labeledPng = labeled.Encode(SKEncodedImageFormat.Png, 100);

        Assert.NotEqual(unlabeledPng.ToArray(), labeledPng.ToArray());
    }
}
