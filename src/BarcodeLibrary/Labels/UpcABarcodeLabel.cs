using SkiaSharp;

namespace Genocs.BarcodeLibrary.Labels;

internal sealed class UpcABarcodeLabel : BarcodeLabel
{
    public override SKImage Draw(Barcode barcode, SKBitmap img)
    {
        try
        {
            int barWidth = barcode.Width / barcode.EncodedValue.Length;

            int shiftAdjustment = BarcodeEncoding.GetAlignmentShiftAdjustment(barcode);

            barcode.LabelFont.Edging = SKFontEdging.SubpixelAntialias;

            string text = barcode.RawData;
            string first = text.Substring(0, 1);
            string second = text.Substring(1, 5);
            string third = text.Substring(6, 5);
            string fourth = text.Substring(11);

            using var g = new SKCanvas(img);

            var font = barcode.LabelFont;
            font.MeasureText(text, out SKRect textBounds);

            float w1 = barWidth * 3;
            float w2 = barWidth * 42;
            float w3 = barWidth * 42;
            float w4 = barWidth * 3;

            float s1 = shiftAdjustment;
            float s2 = s1 + w1;
            float s3 = s2 + w2 + (barWidth * 5);
            float s4 = s3 + w3;

            font.MeasureText(first, out SKRect textBounds1);
            font.MeasureText(second, out SKRect textBounds2);
            font.MeasureText(third, out SKRect textBounds3);
            font.MeasureText(fourth, out SKRect textBounds4);

            using (var backBrush = new SKPaint())
            {
                backBrush.ColorF = barcode.BackColor;
                backBrush.IsAntialias = true;

                g.DrawRect(new SKRect(s1, img.Height - textBounds1.Height - textBounds1.Height / 4f, s1 + w1, img.Height), backBrush);
                g.DrawRect(new SKRect(s4, img.Height - textBounds4.Height - textBounds4.Height / 4f, s4 + w4, img.Height), backBrush);
                g.DrawRect(new SKRect(s3, img.Height - textBounds3.Height * 2f, s3 + w3, img.Height), backBrush);

                g.DrawRect(new SKRect(s2 + w2, img.Height - textBounds4.Height - textBounds4.Height / 4f, s3, img.Height), backBrush);
                g.DrawRect(new SKRect(s2, img.Height - textBounds2.Height * 2f, s2 + w2, img.Height), backBrush);
            }

            using var foreBrush = new SKPaint
            {
                ColorF = barcode.ForeColor,
                IsAntialias = true,
                IsDither = true,
            };

            g.DrawText(first, s1 + ((w1 / 2f) - (textBounds1.Width / 2f)), img.Height, SKTextAlign.Left, font, foreBrush);
            g.DrawText(second, s2 + (w2 / 2f - textBounds2.Width / 2f), img.Height + textBounds2.MidY, SKTextAlign.Left, font, foreBrush);
            g.DrawText(third, s3 + (w3 / 2f - textBounds3.Width / 2f), img.Height + textBounds3.MidY, SKTextAlign.Left, font, foreBrush);
            g.DrawText(fourth, s4 + (w4 / 2f - textBounds4.Width / 2f), img.Height, SKTextAlign.Left, font, foreBrush);

            g.Save();
            return SKImage.FromBitmap(img);
        }
        catch (Exception ex)
        {
            throw new Exception("ELABEL_UPCA-1: " + ex.Message);
        }
    }
}
