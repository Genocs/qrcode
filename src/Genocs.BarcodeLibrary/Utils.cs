using SkiaSharp;

namespace Genocs.BarcodeLibrary;

internal static class Utils
{
    internal static int GetFontHeight(string text, SKFont font)
    {
        font.MeasureText(text, out SKRect textBounds);
        return (int)textBounds.Height;
    }
}
