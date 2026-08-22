using Genocs.BarcodeLibrary.Labels;
using SkiaSharp;

namespace Genocs.BarcodeLibrary.ImageGenerators;

/// <summary>
/// Owns barcode rasterization. Concrete generators are created via <see cref="Create{TGenerator}"/>.
/// </summary>
internal abstract class BarcodeImageGenerator
{
    /// <summary>
    /// Instantiates a concrete image generator using its type parameter.
    /// </summary>
    public static TGenerator Create<TGenerator>()
        where TGenerator : BarcodeImageGenerator, new()
        => new TGenerator();

    public abstract SKBitmap Generate(Barcode barcode);

    protected static void ApplyStandardDimensions(Barcode barcode)
    {
        barcode.Width = barcode.BarWidth * barcode.EncodedValue.Length ?? barcode.Width;
        barcode.Height = (int?)(barcode.Width / barcode.AspectRatio) ?? barcode.Height;
    }

    protected static int GetBarWidth(Barcode barcode)
    {
        int barWidth = barcode.Width / barcode.EncodedValue.Length;
        if (barWidth <= 0)
        {
            throw new Exception("EGENERATE_IMAGE-2: Image size specified not large enough to draw image. (Bar size determined to be less than 1 pixel)");
        }

        return barWidth;
    }
}

/// <summary>
/// Image generator that applies a <see cref="BarcodeLabel"/> strategy selected by type parameter.
/// </summary>
internal abstract class BarcodeImageGenerator<TLabel> : BarcodeImageGenerator
    where TLabel : BarcodeLabel, new()
{
    protected static void ApplyLabel(Barcode barcode, SKBitmap bitmap)
    {
        if (!barcode.IncludeLabel)
        {
            return;
        }

        BarcodeLabel.Create<TLabel>().Draw(barcode, bitmap);
    }
}
