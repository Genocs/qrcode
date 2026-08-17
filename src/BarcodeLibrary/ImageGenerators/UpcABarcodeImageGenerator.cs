using Genocs.BarcodeLibrary.Labels;
using SkiaSharp;

namespace Genocs.BarcodeLibrary.ImageGenerators;

internal sealed class UpcABarcodeImageGenerator : BarcodeImageGenerator<UpcABarcodeLabel>
{
    public override SKBitmap Generate(Barcode barcode)
    {
        ApplyStandardDimensions(barcode);

        int canvasHeight = barcode.Height;
        int topLabelAdjustment = 0;

        int iBarWidth = GetBarWidth(barcode);
        int shiftAdjustment = BarcodeEncoding.GetAlignmentShiftAdjustment(barcode);

        var bitmap = new SKBitmap(barcode.Width, barcode.Height);

        int pos = 0;
        int halfBarWidth = (int)(iBarWidth * 0.5);

        using (var canvas = new SKCanvas(bitmap))
        {
            canvas.Clear(barcode.BackColor);

            using (var paintFore = new SKPaint())
            {
                paintFore.ColorF = barcode.ForeColor;
                paintFore.StrokeWidth = iBarWidth;
                while (pos < barcode.EncodedValue.Length)
                {
                    if (barcode.EncodedValue[pos] == '1')
                    {
                        canvas.DrawLine(new SKPoint(pos * iBarWidth + shiftAdjustment + halfBarWidth, topLabelAdjustment), new SKPoint(pos * iBarWidth + shiftAdjustment + halfBarWidth, canvasHeight + topLabelAdjustment), paintFore);
                    }

                    pos++;
                }
            }
        }

        ApplyLabel(barcode, bitmap);

        return bitmap;
    }
}
