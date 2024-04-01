using SkiaSharp;

namespace Genocs.QRCodeLibrary.Encoder;

public class Base64QRCode : AbstractQRCode, IDisposable
{
    private QRCode _qrCode;

    /// <summary>
    /// Constructor without params to be used in COM Objects connections.
    /// </summary>
    public Base64QRCode()
    {
        _qrCode = new QRCode();
    }

    public Base64QRCode(QRCodeData data)
        : base(data)
    {
        _qrCode = new QRCode(data);
    }

    public override void SetQRCodeData(QRCodeData data)
    {
        _qrCode.SetQRCodeData(data);
    }

    public string GetGraphic(int pixelsPerModule)
    {
        return this.GetGraphic(pixelsPerModule, SKColor.Parse("0x000000"), SKColor.Parse("0xFFFFFF"), true);
    }

    public string GetGraphic(int pixelsPerModule, string darkColorHtmlHex, string lightColorHtmlHex, bool drawQuietZones = true, ImageType imgType = ImageType.Png)
    {
        return this.GetGraphic(pixelsPerModule, FromHtml(darkColorHtmlHex), FromHtml(lightColorHtmlHex), drawQuietZones, imgType);
    }

    public string GetGraphic(int pixelsPerModule, SKColor darkColor, SKColor lightColor, bool drawQuietZones = true, ImageType imgType = ImageType.Png)
    {
        string base64 = string.Empty;
        using (SKImage image = _qrCode.GetGraphic(pixelsPerModule, darkColor, lightColor, drawQuietZones))
        {
            base64 = BitmapToBase64(image, imgType);
        }

        return base64;
    }

    public string GetGraphic(int pixelsPerModule, SKColor darkColor, SKColor lightColor, SKImage icon, int iconSizePercent = 15, int iconBorderWidth = 6, bool drawQuietZones = true, ImageType imgType = ImageType.Png)
    {
        string base64 = string.Empty;
        using (SKImage image = _qrCode.GetGraphic(pixelsPerModule, darkColor, lightColor, icon, iconSizePercent, iconBorderWidth, drawQuietZones))
        {
            base64 = BitmapToBase64(image, imgType);
        }

        return base64;
    }

    private string BitmapToBase64(SKImage image, ImageType imgType)
    {
        string base64 = string.Empty;

        //IImageEncoder iFormat;
        //switch (imgType)
        //{
        //    case ImageType.Png:
        //        iFormat = new PngEncoder();
        //        break;
        //    case ImageType.Jpeg:
        //        iFormat = new JpegEncoder();
        //        break;
        //    case ImageType.Gif:
        //        iFormat = new GifEncoder();
        //        break;
        //    default:
        //        iFormat = new PngEncoder();
        //        break;
        //}

        //using (MemoryStream memoryStream = new MemoryStream())
        //{
        //    image.Save(memoryStream, iFormat);
        //    base64 = Convert.ToBase64String(memoryStream.ToArray(), Base64FormattingOptions.None);
        //}

        return base64;
    }

    public static SKColor FromHtml(string color)
    {
        return SKColor.Parse(color);
    }

    public enum ImageType
    {
        Gif,
        Jpeg,
        Png
    }
}

public static class Base64QRCodeHelper
{
    public static string GetQRCode(string plainText, int pixelsPerModule, string darkColorHtmlHex, string lightColorHtmlHex, QRCodeGenerator.Encoder.QRCodeGenerator.ECCLevel eccLevel, bool forceUtf8 = false, bool utf8BOM = false, QRCodeGenerator.Encoder.QRCodeGenerator.EciMode eciMode = QRCodeGenerator.Encoder.QRCodeGenerator.EciMode.Default, int requestedVersion = -1, bool drawQuietZones = true, Base64QRCode.ImageType imgType = Base64QRCode.ImageType.Png)
    {
        using (var qrGenerator = new QRCodeGenerator.Encoder.QRCodeGenerator())
        using (var qrCodeData = qrGenerator.CreateQrCode(plainText, eccLevel, forceUtf8, utf8BOM, eciMode, requestedVersion))
        using (var qrCode = new Base64QRCode(qrCodeData))
            return qrCode.GetGraphic(pixelsPerModule, darkColorHtmlHex, lightColorHtmlHex, drawQuietZones, imgType);
    }
}