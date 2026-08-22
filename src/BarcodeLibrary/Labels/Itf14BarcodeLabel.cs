using SkiaSharp;

namespace Genocs.BarcodeLibrary.Labels;

internal sealed class Itf14BarcodeLabel : BarcodeLabel
{
    public override SKImage Draw(Barcode barcode, SKBitmap image)
    {
        try
        {
            var font = barcode.LabelFont;
            string str = barcode.AlternateLabel ?? barcode.RawData;

            font.MeasureText(str, out SKRect textBounds);
            float labelPadding = textBounds.Height / 2f;
            float backY = image.Height - textBounds.Height - (labelPadding * 2f);

            using (var canvas = new SKCanvas(image))
            {
                using (var pen = new SKPaint())
                {
                    pen.IsAntialias = true;
                    pen.ColorF = barcode.ForeColor;
                    pen.StrokeWidth = (float)image.Height / 16;

                    canvas.DrawLine(new SKPoint(0, backY - pen.StrokeWidth / 2f),
                        new SKPoint(image.Width, backY - pen.StrokeWidth / 2f), pen);
                }

                using (var paint = new SKPaint())
                {
                    paint.IsAntialias = true;
                    paint.ColorF = barcode.BackColor;
                    paint.Style = SKPaintStyle.Fill;

                    var rect = SKRect.Create(0, backY, image.Width, textBounds.Height + labelPadding * 2f);
                    canvas.DrawRect(rect, paint);
                }

                using (var foreBrush = new SKPaint())
                {
                    foreBrush.IsAntialias = true;
                    foreBrush.ColorF = barcode.ForeColor;

                    float labelX = image.Width / 2f;
                    float labelY = image.Height - textBounds.Height + labelPadding;

                    canvas.DrawText(str, labelX, labelY, SKTextAlign.Center, font, foreBrush);
                }

                canvas.Save();
            }

            return SKImage.FromBitmap(image);
        }
        catch (Exception ex)
        {
            throw new Exception($"ELABEL_ITF14-1: {ex.Message}");
        }
    }
}
