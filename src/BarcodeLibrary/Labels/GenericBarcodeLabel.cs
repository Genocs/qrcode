using SkiaSharp;

namespace Genocs.BarcodeLibrary.Labels;

internal sealed class GenericBarcodeLabel : BarcodeLabel
{
    public override SKImage Draw(Barcode barcode, SKBitmap img)
    {
        try
        {
            using var g = new SKCanvas(img);

            string text = barcode.AlternateLabel ?? barcode.RawData;
            var font = barcode.LabelFont;

            font.MeasureText(text, out SKRect textBounds);
            float labelPadding = textBounds.Height / 2f;

            float labelX = (img.Width / 2f) - (textBounds.Width / 2f);
            float labelY = img.Height - textBounds.Height + labelPadding;
            float backY = img.Height - textBounds.Height - (labelPadding * 2f);

            using var backBrush = new SKPaint
            {
                ColorF = barcode.BackColor,
                Style = SKPaintStyle.Fill
            };

            g.DrawRect(SKRect.Create(0, backY, img.Width, textBounds.Height + (labelPadding * 2f)), backBrush);

            using var foreBrush = new SKPaint
            {
                ColorF = barcode.ForeColor,
                IsAntialias = true,
                IsDither = true,
            };

            g.DrawText(text, labelX, labelY, SKTextAlign.Left, font, foreBrush);

            g.Save();
            return SKImage.FromBitmap(img);
        }
        catch (Exception ex)
        {
            throw new Exception("ELABEL_GENERIC-1: " + ex.Message);
        }
    }
}
