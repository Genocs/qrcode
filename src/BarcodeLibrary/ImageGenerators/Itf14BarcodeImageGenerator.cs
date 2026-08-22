using Genocs.BarcodeLibrary.Labels;
using SkiaSharp;

namespace Genocs.BarcodeLibrary.ImageGenerators;

internal sealed class Itf14BarcodeImageGenerator : BarcodeImageGenerator<Itf14BarcodeLabel>
{
    public override SKBitmap Generate(Barcode barcode)
    {
        // Automatically calculate the Width if applicable. Quite confusing with this
        // barcode type, and it seems this method overestimates the minimum width. But
        // at least it's deterministic and does't produce too small of a value.
        if (barcode.BarWidth.HasValue)
        {
            // Width = (BarWidth * EncodedValue.Length) + bearerwidth + iquietzone
            // Width = (BarWidth * EncodedValue.Length) + 2*Width/12.05 + 2*Width/20
            // Width - 2*Width/12.05 - 2*Width/20 = BarWidth * EncodedValue.Length
            // Width = (BarWidth * EncodedValue.Length)/(1 - 2/12.05 - 2/20)
            // Width = (BarWidth * EncodedValue.Length)/((241 - 40 - 24.1)/241)
            // Width = BarWidth * EncodedValue.Length / 176.9 * 241
            // Rounding error? + 1
            barcode.Width = (int)((241 / 176.9 * barcode.EncodedValue.Length * barcode.BarWidth.Value) + 1);
        }

        barcode.Height = (int?)(barcode.Width / barcode.AspectRatio) ?? barcode.Height;

        int canvasHeight = barcode.Height;

        if (barcode.IncludeLabel)
        {
            canvasHeight -= Utils.GetFontHeight(barcode.RawData, barcode.LabelFont);
        }

        var bitmap = new SKBitmap(barcode.Width, barcode.Height);

        int bearerWidth = (int)(bitmap.Width / 12.05);
        int quietZone = Convert.ToInt32(bitmap.Width * 0.05);
        int barWidth = (bitmap.Width - (bearerWidth * 2) - (quietZone * 2)) / barcode.EncodedValue.Length;
        int shiftAdjustment = ((bitmap.Width - (bearerWidth * 2) - (quietZone * 2)) % barcode.EncodedValue.Length) / 2;

        if (barWidth <= 0 || quietZone <= 0)
        {
            throw new Exception("EGENERATE_IMAGE-3: Image size specified not large enough to draw image. (Bar size determined to be less than 1 pixel or quiet zone determined to be less than 1 pixel)");
        }

        int pos = 0;

        var canvas = new SKCanvas(bitmap);

        // Fill background
        canvas.Clear(barcode.BackColor);

        // Lines are barWidth wide so draw the appropriate color line vertically
        using (var paint = new SKPaint())
        {
            paint.ColorF = barcode.ForeColor;
            paint.StrokeWidth = barWidth;

            while (pos < barcode.EncodedValue.Length)
            {
                if (barcode.EncodedValue[pos] == '1')
                {
                    canvas.DrawLine(new SKPoint((pos * barWidth) + shiftAdjustment + bearerWidth + quietZone, 0), new SKPoint((pos * barWidth) + shiftAdjustment + bearerWidth + quietZone, barcode.Height), paint);
                }

                pos++;
            }

            // Bearer bars
            paint.StrokeWidth = (float)canvasHeight / 8;
            paint.ColorF = barcode.ForeColor;

            canvas.DrawLine(new SKPoint(0, 0), new SKPoint(bitmap.Width, 0), paint);
            canvas.DrawLine(new SKPoint(0, canvasHeight), new SKPoint(bitmap.Width, canvasHeight), paint);
            canvas.DrawLine(new SKPoint(0, 0), new SKPoint(0, canvasHeight), paint);
            canvas.DrawLine(new SKPoint(bitmap.Width, 0), new SKPoint(bitmap.Width, canvasHeight), paint);
        }

        ApplyLabel(barcode, bitmap);

        return bitmap;
    }
}
