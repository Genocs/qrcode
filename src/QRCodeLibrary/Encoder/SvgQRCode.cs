using System.Text;
using SkiaSharp;

namespace Genocs.QRCodeLibrary.Encoder;

public class SvgQRCode : AbstractQRCode
{
    public SvgQRCode(QRCodeData data)
        : base(data)
    {
    }

    public string GetGraphic(int pixelsPerModule)
    {
        var viewBox = new SKSize(pixelsPerModule * QrCodeData.ModuleMatrix.Count, pixelsPerModule * QrCodeData.ModuleMatrix.Count);
        return GetGraphic(viewBox, SKColors.Black, SKColors.White);
    }

    public string GetGraphic(int pixelsPerModule, SKColor darkColor, SKColor lightColor, bool drawQuietZones = true, SizingMode sizingMode = SizingMode.WidthHeightAttribute)
    {
        var viewBox = new SKSize(pixelsPerModule * QrCodeData.ModuleMatrix.Count, pixelsPerModule * QrCodeData.ModuleMatrix.Count);
        return GetGraphic(viewBox, darkColor, lightColor, drawQuietZones, sizingMode);
    }

    public string GetGraphic(int pixelsPerModule, string darkColorHex, string lightColorHex, bool drawQuietZones = true, SizingMode sizingMode = SizingMode.WidthHeightAttribute)
    {
        var viewBox = new SKSize(pixelsPerModule * QrCodeData.ModuleMatrix.Count, pixelsPerModule * QrCodeData.ModuleMatrix.Count);
        return GetGraphic(viewBox, darkColorHex, lightColorHex, drawQuietZones, sizingMode);
    }

    public string GetGraphic(SKSize viewBox, bool drawQuietZones = true, SizingMode sizingMode = SizingMode.WidthHeightAttribute)
        => GetGraphic(viewBox, SKColors.Black, SKColors.White, drawQuietZones, sizingMode);

    public string GetGraphic(SKSize viewBox, SKColor darkColor, SKColor lightColor, bool drawQuietZones = true, SizingMode sizingMode = SizingMode.WidthHeightAttribute)
        => GetGraphic(viewBox, ColorToHex(darkColor), ColorToHex(lightColor), drawQuietZones, sizingMode);

    public string GetGraphic(SKSize viewBox, string darkColorHex, string lightColorHex, bool drawQuietZones = true, SizingMode sizingMode = SizingMode.WidthHeightAttribute)
    {
        int offset = drawQuietZones ? 0 : 4;
        int drawableModulesCount = QrCodeData.ModuleMatrix.Count - (drawQuietZones ? 0 : offset * 2);
        double pixelsPerModule = Math.Min(viewBox.Width, viewBox.Height) / (double)drawableModulesCount;
        double qrSize = drawableModulesCount * pixelsPerModule;
        string? svgSizeAttributes = sizingMode == SizingMode.WidthHeightAttribute ? $@"width=""{viewBox.Width}"" height=""{viewBox.Height}""" : $@"viewBox=""0 0 {viewBox.Width} {viewBox.Height}""";
        var svgFile = new StringBuilder($@"<svg version=""1.1"" baseProfile=""full"" shape-rendering=""crispEdges"" {svgSizeAttributes} xmlns=""http://www.w3.org/2000/svg"">");
        svgFile.AppendLine($@"<rect x=""0"" y=""0"" width=""{CleanSvgVal(qrSize)}"" height=""{CleanSvgVal(qrSize)}"" fill=""{lightColorHex}"" />");

        for (int xi = offset; xi < offset + drawableModulesCount; xi++)
        {
            for (int yi = offset; yi < offset + drawableModulesCount; yi++)
            {
                if (QrCodeData.ModuleMatrix[yi][xi])
                {
                    double x = (xi - offset) * pixelsPerModule;
                    double y = (yi - offset) * pixelsPerModule;
                    svgFile.AppendLine($@"<rect x=""{CleanSvgVal(x)}"" y=""{CleanSvgVal(y)}"" width=""{CleanSvgVal(pixelsPerModule)}"" height=""{CleanSvgVal(pixelsPerModule)}"" fill=""{darkColorHex}"" />");
                }
            }
        }

        svgFile.Append(@"</svg>");
        return svgFile.ToString();
    }

    private static string CleanSvgVal(double input)
    {
        // Clean double values for international use/formats
        return input.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    private static string ColorToHex(SKColor color)
    {
        if (color.Alpha == 255)
        {
            return $"#{color.Red:X2}{color.Green:X2}{color.Blue:X2}";
        }

        return $"#{color.Alpha:X2}{color.Red:X2}{color.Green:X2}{color.Blue:X2}";
    }

    public enum SizingMode
    {
        WidthHeightAttribute,
        ViewBoxAttribute
    }
}

public static class SvgQRCodeHelper
{
    /// <summary>
    /// It allows to build a qr code as an SVG image.
    /// </summary>
    /// <param name="plainText"></param>
    /// <param name="pixelsPerModule"></param>
    /// <param name="darkColorHex"></param>
    /// <param name="lightColorHex"></param>
    /// <param name="eccLevel"></param>
    /// <param name="forceUtf8"></param>
    /// <param name="utf8BOM"></param>
    /// <param name="eciMode"></param>
    /// <param name="requestedVersion"></param>
    /// <param name="drawQuietZones"></param>
    /// <param name="sizingMode"></param>
    /// <returns></returns>
    public static string GetQRCode(
        string plainText,
        int pixelsPerModule,
        string darkColorHex,
        string lightColorHex,
        QRCodeGenerator.ECCLevel eccLevel,
        bool forceUtf8 = false,
        bool utf8BOM = false,
        QRCodeGenerator.EciMode eciMode = QRCodeGenerator.EciMode.Default,
        int requestedVersion = -1,
        bool drawQuietZones = true,
        SvgQRCode.SizingMode sizingMode = SvgQRCode.SizingMode.WidthHeightAttribute)
    {
        using var qrCodeData = QRCodeGenerator.CreateQrCode(plainText, eccLevel, forceUtf8, utf8BOM, eciMode, requestedVersion);
        using var qrCode = new SvgQRCode(qrCodeData);
        return qrCode.GetGraphic(pixelsPerModule, darkColorHex, lightColorHex, drawQuietZones, sizingMode);
    }
}
