﻿using System.Drawing;

namespace Genocs.QRCodeGenerator.Encoder;

public class PostscriptQRCode : AbstractQRCode
{
    public PostscriptQRCode(QRCodeData data)
        : base(data)
    {
    }

    public string GetGraphic(int pointsPerModule, bool epsFormat = false)
    {
        var viewBox = new Size(pointsPerModule * QrCodeData.ModuleMatrix.Count, pointsPerModule * QrCodeData.ModuleMatrix.Count);
        return GetGraphic(viewBox, Color.Black, Color.White, true, epsFormat);
    }

    public string GetGraphic(int pointsPerModule, Color darkColor, Color lightColor, bool drawQuietZones = true, bool epsFormat = false)
    {
        var viewBox = new Size(pointsPerModule * QrCodeData.ModuleMatrix.Count, pointsPerModule * QrCodeData.ModuleMatrix.Count);
        return GetGraphic(viewBox, darkColor, lightColor, drawQuietZones, epsFormat);
    }

    public string GetGraphic(int pointsPerModule, string darkColorHex, string lightColorHex, bool drawQuietZones = true, bool epsFormat = false)
    {
        var viewBox = new Size(pointsPerModule * QrCodeData.ModuleMatrix.Count, pointsPerModule * QrCodeData.ModuleMatrix.Count);
        return GetGraphic(viewBox, darkColorHex, lightColorHex, drawQuietZones, epsFormat);
    }

    public string GetGraphic(Size viewBox, bool drawQuietZones = true, bool epsFormat = false)
    {
        return GetGraphic(viewBox, Color.Black, Color.White, drawQuietZones, epsFormat);
    }

    public string GetGraphic(Size viewBox, string darkColorHex, string lightColorHex, bool drawQuietZones = true, bool epsFormat = false)
    {
        return GetGraphic(viewBox, FromHtml(darkColorHex), FromHtml(lightColorHex), drawQuietZones, epsFormat);
    }

    public string GetGraphic(Size viewBox, Color darkColor, Color lightColor, bool drawQuietZones = true, bool epsFormat = false)
    {
        var offset = drawQuietZones ? 0 : 4;
        var drawableModulesCount = QrCodeData.ModuleMatrix.Count - (drawQuietZones ? 0 : offset * 2);
        var pointsPerModule = Math.Min(viewBox.Width, viewBox.Height) / (double)drawableModulesCount;

        string psFile = string.Format(psHeader, new object[] {
            DateTime.Now.ToString("s"), CleanSvgVal(viewBox.Width), CleanSvgVal(pointsPerModule),
            epsFormat ? "EPSF-3.0" : string.Empty
        });
        psFile += string.Format(psFunctions, new object[] {
            CleanSvgVal(darkColor.R /255.0), CleanSvgVal(darkColor.G /255.0), CleanSvgVal(darkColor.B /255.0),
            CleanSvgVal(lightColor.R /255.0), CleanSvgVal(lightColor.G /255.0), CleanSvgVal(lightColor.B /255.0),
            drawableModulesCount
        });

        for (int xi = offset; xi < offset + drawableModulesCount; xi++)
        {
            if (xi > offset)
                psFile += "nl\n";
            for (int yi = offset; yi < offset + drawableModulesCount; yi++)
            {
                psFile += QrCodeData.ModuleMatrix[xi][yi] ? "f " : "b ";
            }

            psFile += "\n";
        }

        return psFile + psFooter;
    }

    private string CleanSvgVal(double input)
    {
        // Clean double values for international use/formats
        return input.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    public static Color FromHtml(string color)
    {
        return Color.Gainsboro;
    }

    private const string psHeader = @"%!PS-Adobe-3.0 {3}
%%Creator: QRCoder.NET
%%Title: QRCode
%%CreationDate: {0}
%%DocumentData: Clean7Bit
%%Origin: 0
%%DocumentMedia: Default {1} {1} 0 () ()
%%BoundingBox: 0 0 {1} {1}
%%LanguageLevel: 2 
%%Pages: 1
%%Page: 1 1
%%EndComments
%%BeginConstants
/sz {1} def
/sc {2} def
%%EndConstants
%%BeginFeature: *PageSize Default
<< /PageSize [ sz sz ] /ImagingBBox null >> setpagedevice
%%EndFeature
";

    private const string psFunctions = @"%%BeginFunctions 
/csquare {{
    newpath
    0 0 moveto
    0 1 rlineto
    1 0 rlineto
    0 -1 rlineto
    closepath
    setrgbcolor
    fill
}} def
/f {{ 
    {0} {1} {2} csquare
    1 0 translate
}} def
/b {{ 
    1 0 translate
}} def 
/background {{ 
    {3} {4} {5} csquare 
}} def
/nl {{
    -{6} -1 translate
}} def
%%EndFunctions
%%BeginBody
0 0 moveto
gsave
sz sz scale
background
grestore
gsave
sc sc scale
0 {6} 1 sub translate
";

    private const string psFooter = @"%%EndBody
grestore
showpage   
%%EOF
";
}

public static class PostscriptQRCodeHelper
{
    public static string GetQRCode(string plainText, int pointsPerModule, string darkColorHex, string lightColorHex, QRCodeGenerator.ECCLevel eccLevel, bool forceUtf8 = false, bool utf8BOM = false, QRCodeGenerator.EciMode eciMode = QRCodeGenerator.EciMode.Default, int requestedVersion = -1, bool drawQuietZones = true, bool epsFormat = false)
    {
        using (var qrGenerator = new QRCodeGenerator())
        using (var qrCodeData = qrGenerator.CreateQrCode(plainText, eccLevel, forceUtf8, utf8BOM, eciMode, requestedVersion))
        using (var qrCode = new PostscriptQRCode(qrCodeData))
            return qrCode.GetGraphic(pointsPerModule, darkColorHex, lightColorHex, drawQuietZones, epsFormat);
    }
}