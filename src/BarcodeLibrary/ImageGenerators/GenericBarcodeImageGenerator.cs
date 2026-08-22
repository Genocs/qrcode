using Genocs.BarcodeLibrary.Labels;
using SkiaSharp;

namespace Genocs.BarcodeLibrary.ImageGenerators;

internal sealed class GenericBarcodeImageGenerator : BarcodeImageGenerator<GenericBarcodeLabel>
{
    public override SKBitmap Generate(Barcode barcode)
    {
        ApplyStandardDimensions(barcode);

        var ilHeight = barcode.Height;

        var bitmap = new SKBitmap(barcode.Width, barcode.Height);
        var iBarWidth = GetBarWidth(barcode);
        var iBarWidthModifier = 1;

        if (barcode.EncodedType == BarcodeType.PostNet)
            iBarWidthModifier = 2;

        var shiftAdjustment = BarcodeEncoding.GetAlignmentShiftAdjustment(barcode);

        var pos = 0;
        var halfBarWidth = (int)Math.Round(iBarWidth * 0.5);

        using (var canvas = new SKCanvas(bitmap))
        {
            canvas.Clear((SKColor)barcode.BackColor);

            var barWidth = iBarWidth / iBarWidthModifier;

            using (var backPaint = new SKPaint())
            {
                backPaint.ColorF = barcode.BackColor;
                backPaint.StrokeWidth = barWidth;
                using var forePaint = new SKPaint();
                forePaint.ColorF = barcode.ForeColor;
                forePaint.StrokeWidth = barWidth;
                while (pos < barcode.EncodedValue.Length)
                {
                    if (barcode.EncodedType == BarcodeType.PostNet)
                    {
                        float y = 0f;
                        if (barcode.EncodedValue[pos] == '0')
                            y = ilHeight - ilHeight * 0.4f;

                        canvas.DrawLine(new SKPoint(pos * iBarWidth + shiftAdjustment + halfBarWidth, ilHeight), new SKPoint(pos * iBarWidth + shiftAdjustment + halfBarWidth, y), forePaint);
                    }
                    else if (barcode.EncodedValue[pos] == '1')
                    {
                        canvas.DrawLine(new SKPoint(pos * iBarWidth + shiftAdjustment + halfBarWidth, 0f), new SKPoint(pos * iBarWidth + shiftAdjustment + halfBarWidth, ilHeight), forePaint);
                    }

                    pos++;
                }
            }
        }

        ApplyLabel(barcode, bitmap);

        return bitmap;
    }
}
