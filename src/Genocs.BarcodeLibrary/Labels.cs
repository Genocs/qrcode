using SkiaSharp;

namespace Genocs.BarcodeLibrary;

internal class Labels
{
    /// <summary>
    /// Draws Label for ITF-14 barcodes.
    /// </summary>
    /// <param name="barcode">Barcode to draw label for.</param>
    /// <param name="image">Image representation of the barcode without the labels.</param>
    /// <returns>Image representation of the barcode with labels applied.</returns>
    public static SKImage Label_ITF14(Barcode barcode, SKBitmap image)
    {
        if (barcode == null) throw new ArgumentNullException(nameof(barcode));
        try
        {
            var font = barcode.LabelFont;
            string str = barcode.AlternateLabel ?? barcode.RawData;

            font.MeasureText(str, out SKRect textBounds);
            float labelPadding = textBounds.Height / 2f;
            float backY = image.Height - textBounds.Height - (labelPadding * 2f);

            using (var canvas = new SKCanvas(image))
            {
                // draw bounding box side overdrawn by label
                using (var pen = new SKPaint())
                {
                    pen.IsAntialias = true;
                    pen.ColorF = barcode.ForeColor;
                    pen.StrokeWidth = (float)image.Height / 16;

                    canvas.DrawLine(new SKPoint(0, backY - pen.StrokeWidth / 2f),
                        new SKPoint(image.Width, backY - pen.StrokeWidth / 2f), pen);
                }

                // color a box at the bottom of the barcode to hold the string of data
                using (var paint = new SKPaint())
                {
                    paint.IsAntialias = true;
                    paint.ColorF = barcode.BackColor;
                    paint.Style = SKPaintStyle.Fill;

                    var rect = SKRect.Create(0, backY, image.Width, textBounds.Height + labelPadding * 2f);
                    canvas.DrawRect(rect, paint);
                }

                // draw data string under the barcode image
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

    /// <summary>
    /// Draws Label for Generic barcodes.
    /// </summary>
    /// <param name="barcode">Barcode to draw label for.</param>
    /// <param name="img">Image representation of the barcode without the labels.</param>
    /// <returns>Image representation of the barcode with labels applied.</returns>
    public static SKImage Label_Generic(Barcode barcode, SKBitmap img)
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

            // color a background color box at the bottom of the barcode to hold the string of data
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

    /// <summary>
    /// Draws Label for EAN-13 barcodes.
    /// </summary>
    /// <param name="barcode">Barcode to draw label for.</param>
    /// <param name="img">Image representation of the barcode without the labels.</param>
    /// <returns>Image representation of the barcode with labels applied.</returns>
    public static SKImage Label_EAN13(Barcode? barcode, SKBitmap img)
    {
        if (barcode == null) throw new ArgumentNullException(nameof(barcode));

        try
        {
            int iBarWidth = barcode.Width / barcode.EncodedValue.Length;

            int shiftAdjustment = BarcodeCommon.GetAlignmentShiftAdjustment(barcode);

            barcode.LabelFont.Edging = SKFontEdging.SubpixelAntialias;

            string text = barcode.RawData;
            var first = text.Substring(0, 1);
            var second = text.Substring(1, 6);
            var third = text.Substring(7, 6);

            using var g = new SKCanvas(img);

            var font = barcode.LabelFont;
            font.MeasureText(text, out SKRect textBounds);

            // Default alignment for UPCA

            float w1 = iBarWidth * 3; // Width of first block
            float w2 = iBarWidth * 42; // Width of second block
            float w3 = iBarWidth * 42; // Width of third block

            float s1 = shiftAdjustment;
            float s2 = s1 + w1; // Start position of block 2
            float s3 = s2 + w2 + (iBarWidth * 5); // Start position of block 3

            font.MeasureText(first, out SKRect textBounds1);
            font.MeasureText(second, out SKRect textBounds2);
            font.MeasureText(third, out SKRect textBounds3);
            SKRect textBounds4 = new();

            // Draw the background rectangles for each block
            using (var backBrush = new SKPaint())
            {
                backBrush.ColorF = barcode.BackColor;
                backBrush.IsAntialias = true;

                g.DrawRect(new SKRect(s1, img.Height - textBounds1.Height - textBounds1.Height / 4f, s1 + w1, img.Height), backBrush); // first guard bar cover
                g.DrawRect(new SKRect(s3, img.Height - textBounds3.Height * 2f, s3 + w3, img.Height), backBrush); // middle bar cover

                g.DrawRect(new SKRect(s2 + w2, img.Height - textBounds4.Height - textBounds4.Height / 4f, s3, img.Height), backBrush);
                g.DrawRect(new SKRect(s2, img.Height - textBounds2.Height * 2f, s2 + w2, img.Height), backBrush);
            }

            using var foreBrush = new SKPaint
            {
                ColorF = barcode.ForeColor,
                IsAntialias = true,
                IsDither = true,
            };

            g.DrawText(first, s1 + (w1 / 2f - textBounds1.Width / 2f), img.Height, SKTextAlign.Left, font, foreBrush);
            g.DrawText(second, s2 + (w2 / 2f - textBounds2.Width / 2f), img.Height + textBounds2.MidY, SKTextAlign.Left, font, foreBrush);
            g.DrawText(third, s3 + (w3 / 2f - textBounds3.Width / 2f), img.Height + textBounds3.MidY, SKTextAlign.Left, font, foreBrush);

            g.Save();
            return SKImage.FromBitmap(img);
        }
        catch (Exception ex)
        {
            throw new Exception("ELABEL_EAN13-1: " + ex.Message);
        }
    }

    /// <summary>
    /// Draws Label for UPC-A barcodes.
    /// </summary>
    /// <param name="barcode">Barcode to draw the label for.</param>
    /// <param name="img">Image representation of the barcode without the labels.</param>
    /// <returns>Image representation of the barcode with labels applied.</returns>
    public static SKImage Label_UPCA(Barcode? barcode, SKBitmap img)
    {
        if (barcode == null) throw new ArgumentNullException(nameof(barcode));

        try
        {
            int iBarWidth = barcode.Width / barcode.EncodedValue.Length;

            int shiftAdjustment = BarcodeCommon.GetAlignmentShiftAdjustment(barcode);

            barcode.LabelFont.Edging = SKFontEdging.SubpixelAntialias;

            var text = barcode.RawData;
            var first = text.Substring(0, 1);
            var second = text.Substring(1, 5);
            var third = text.Substring(6, 5);
            var fourth = text.Substring(11);

            using var g = new SKCanvas(img);

            var font = barcode.LabelFont;
            font.MeasureText(text, out SKRect textBounds);

            // Default alignment for UPCA

            float w1 = iBarWidth * 3; // Width of first block
            float w2 = iBarWidth * 42; // Width of second block
            float w3 = iBarWidth * 42; // Width of third block
            float w4 = iBarWidth * 3; // Width of fourth block

            float s1 = shiftAdjustment;
            var s2 = s1 + w1; // Start position of block 2
            var s3 = s2 + w2 + iBarWidth * 5; // Start position of block 3
            var s4 = s3 + w3;

            font.MeasureText(first, out SKRect textBounds1);
            font.MeasureText(second, out SKRect textBounds2);
            font.MeasureText(third, out SKRect textBounds3);
            font.MeasureText(fourth, out SKRect textBounds4);

            // Draw the background rectangles for each block
            using (var backBrush = new SKPaint())
            {
                backBrush.ColorF = barcode.BackColor;
                backBrush.IsAntialias = true;

                g.DrawRect(new SKRect(s1, img.Height - textBounds1.Height - textBounds1.Height / 4f, s1 + w1, img.Height), backBrush); // first guard bar cover
                g.DrawRect(new SKRect(s4, img.Height - textBounds4.Height - textBounds4.Height / 4f, s4 + w4, img.Height), backBrush); // end guard bar cover
                g.DrawRect(new SKRect(s3, img.Height - textBounds3.Height * 2f, s3 + w3, img.Height), backBrush); // middle bar cover

                g.DrawRect(new SKRect(s2 + w2, img.Height - textBounds4.Height - textBounds4.Height / 4f, s3, img.Height), backBrush);
                g.DrawRect(new SKRect(s2, img.Height - textBounds2.Height * 2f, s2 + w2, img.Height), backBrush);
            }

            using var foreBrush = new SKPaint
            {
                ColorF = barcode.ForeColor,
                IsAntialias = true,
                IsDither = true,
            };

            g.DrawText(first, s1 + (w1 / 2f - textBounds1.Width / 2f), img.Height, SKTextAlign.Left, font, foreBrush);
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

    private static int GetFontSize(Barcode barcode, int wid, int hgt, string lbl)
    {
        // Returns the optimal font size for the specified dimensions
        int fontSize = 10;

        if (lbl.Length > 0)
        {
            for (int i = 1; i <= 100; i++)
            {
                using (var testFont = new SKFont(SKTypeface.FromFamilyName("Arial", SKFontStyle.Normal), i))
                {
                    testFont.MeasureText(lbl, out SKRect bounds);

                    if (!(bounds.Width > wid) && !(bounds.Height > hgt)) continue;
                    fontSize = i - 1;
                    break;
                }
            }
        }

        return fontSize;
    }
}
