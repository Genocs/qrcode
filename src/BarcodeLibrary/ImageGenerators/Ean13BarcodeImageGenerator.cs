using Genocs.BarcodeLibrary.Labels;
using SkiaSharp;

namespace Genocs.BarcodeLibrary.ImageGenerators;

internal sealed class Ean13BarcodeImageGenerator : BarcodeImageGenerator<Ean13BarcodeLabel>
{
    public override SKBitmap Generate(Barcode barcode)
    {
        ApplyStandardDimensions(barcode);

        var ilHeight = barcode.Height;
        var topLabelAdjustment = 0;
        var shiftAdjustment = BarcodeEncoding.GetAlignmentShiftAdjustment(barcode);

        if (barcode.IncludeLabel)
        {
            if (barcode.AlternateLabel != null)
            {
                topLabelAdjustment = Utils.GetFontHeight(barcode.RawData, barcode.LabelFont);
                ilHeight -= Utils.GetFontHeight(barcode.RawData, barcode.LabelFont);
            }
        }

        var bitmap = new SKBitmap(barcode.Width, barcode.Height);
        var iBarWidth = GetBarWidth(barcode);

        var pos = 0;
        var halfBarWidth = (int)(iBarWidth * 0.5);

        using (var canvas = new SKCanvas(bitmap))
        {
            canvas.Clear((SKColor)barcode.BackColor);

            using (var paint = new SKPaint())
            {
                paint.ColorF = barcode.ForeColor;
                paint.StrokeWidth = iBarWidth;
                while (pos < barcode.EncodedValue.Length)
                {
                    if (barcode.EncodedValue[pos] == '1')
                    {
                        canvas.DrawLine(new SKPoint(pos * iBarWidth + shiftAdjustment + halfBarWidth, topLabelAdjustment), new SKPoint(pos * iBarWidth + shiftAdjustment + halfBarWidth, ilHeight + topLabelAdjustment), paint);
                    }

                    pos++;
                }
            }
        }

        ApplyLabel(barcode, bitmap);

        return bitmap;
    }
}
