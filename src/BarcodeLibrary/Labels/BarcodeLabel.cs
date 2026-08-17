using SkiaSharp;

namespace Genocs.BarcodeLibrary.Labels;

/// <summary>
/// Strategy for drawing human-readable text onto a barcode image.
/// Concrete strategies are created via <see cref="Create{TLabel}"/>.
/// </summary>
internal abstract class BarcodeLabel
{
    /// <summary>
    /// Instantiates a concrete label strategy using its type parameter.
    /// </summary>
    public static TLabel Create<TLabel>()
        where TLabel : BarcodeLabel, new()
        => new TLabel();

    public abstract SKImage Draw(Barcode barcode, SKBitmap image);
}
