using SkiaSharp;

namespace Genocs.BarcodeLibrary;

internal static class Utils
{
    internal static int GetFontHeight(string text, SKFont font)
    {
        font.MeasureText(text, out SKRect textBounds);
        return (int)textBounds.Height;
    }

    private static int GetFontSize(Barcode barcode, int width, int height, string label)
    {
        // Returns the optimal font size for the specified dimensions
        int fontSize = 10;

        if (label.Length > 0)
        {
            for (int i = 1; i <= 100; i++)
            {
                using var testFont = new SKFont(SKTypeface.FromFamilyName("Arial", SKFontStyle.Normal), i);
                testFont.MeasureText(label, out SKRect bounds);

                if (!(bounds.Width > width) && !(bounds.Height > height)) continue;
                fontSize = i - 1;
                break;
            }
        }

        return fontSize;
    }
}
