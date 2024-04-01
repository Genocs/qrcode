using SkiaSharp;
using static Genocs.QRCodeGenerator.Encoder.QRCodeGenerator;

namespace Genocs.QRCodeLibrary.Encoder;

public class QRCode : AbstractQRCode, IDisposable
{
    /// <summary>
    /// Constructor without params to be used in COM Objects connections.
    /// </summary>
    public QRCode()
    {
    }

    public QRCode(QRCodeData data)
        : base(data)
    {
    }

    public SKImage GetGraphic(int pixelsPerModule)
    {
        return this.GetGraphic(pixelsPerModule, SKColors.Black, SKColors.White, true);
    }

    public SKImage GetGraphic(int pixelsPerModule, string darkColorHtmlHex, string lightColorHtmlHex, bool drawQuietZones = true)
    {
        return this.GetGraphic(pixelsPerModule, FromHtml(darkColorHtmlHex), FromHtml(lightColorHtmlHex), drawQuietZones);
    }

    public SKImage GetGraphic(int pixelsPerModule, SKColor darkColor, SKColor lightColor, bool drawQuietZones = true)
    {
        int size = (QrCodeData!.ModuleMatrix.Count - (drawQuietZones ? 0 : 8)) * pixelsPerModule;
        int offset = drawQuietZones ? 0 : 4 * pixelsPerModule;

        var darkBrush = new SKPaint
        {
            Color = darkColor,
            StrokeWidth = 1,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        int padding = 1;

        // Create an image and fill it blue
        SKBitmap image = new(size, size);
        using SKCanvas canvas = new(image);
        canvas.Clear(lightColor);

        for (int x = 0; x < size + offset; x = x + pixelsPerModule)
        {
            for (int y = 0; y < size + offset; y = y + pixelsPerModule)
            {
                bool module = QrCodeData.ModuleMatrix[((y + pixelsPerModule) / pixelsPerModule) - 1][((x + pixelsPerModule) / pixelsPerModule) - 1];

                if (module)
                {
                    float fL = x - offset + padding;
                    float fT = y - offset + padding;
                    float fR = pixelsPerModule + (x - offset) - padding;
                    float fB = pixelsPerModule + (y - offset) - padding;

                    SKRect rect = new SKRect(fL, fT, fR, fB);
                    canvas.DrawRect(rect, darkBrush);
                }
            }
        }

        return SKImage.FromBitmap(image);
    }

    public SKImage GetGraphic(int pixelsPerModule, SKColor darkColor, SKColor lightColor, SKImage? icon = null, int iconSizePercent = 15, int iconBorderWidth = 6, bool drawQuietZones = true)
    {
        int size = (QrCodeData.ModuleMatrix.Count - (drawQuietZones ? 0 : 8)) * pixelsPerModule;
        int offset = drawQuietZones ? 0 : 4 * pixelsPerModule;

        // Create an image and fill it blue
        SKBitmap image = new(size, size);
        using SKCanvas canvas = new(image);
        canvas.Clear(lightColor);

        var darkBrush = new SKPaint
        {
            Color = darkColor,
            StrokeWidth = 1,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke
        };

        bool drawIconFlag = icon != null && iconSizePercent > 0 && iconSizePercent <= 100;

        // GraphicsPath iconPath = null;
        float iconDestWidth = 0, iconDestHeight = 0, iconX = 0, iconY = 0;

        if (drawIconFlag)
        {
            iconDestWidth = iconSizePercent * image.Width / 100f;
            iconDestHeight = drawIconFlag ? iconDestWidth * icon.Height / icon.Width : 0;
            iconX = (image.Width - iconDestWidth) / 2;
            iconY = (image.Height - iconDestHeight) / 2;

            // var centerDest = new SixLabors.ImageSharp.RectangleF(iconX - iconBorderWidth, iconY - iconBorderWidth, iconDestWidth + iconBorderWidth * 2, iconDestHeight + iconBorderWidth * 2);
            // iconPath = this.CreateRoundedRectanglePath(centerDest, iconBorderWidth * 2);
        }

        for (int x = 0; x < size + offset; x = x + pixelsPerModule)
        {
            for (int y = 0; y < size + offset; y = y + pixelsPerModule)
            {
                bool module = this.QrCodeData.ModuleMatrix[((y + pixelsPerModule) / pixelsPerModule) - 1][((x + pixelsPerModule) / pixelsPerModule) - 1];

                if (module)
                {
                    var r = new SKRect(x - offset, y - offset, pixelsPerModule, pixelsPerModule);

                    if (drawIconFlag)
                    {
                        //var region = new SixLabors.ImageSharp.Drawing.Region(r);
                        //region.Exclude(iconPath);
                        //gfx.FillRegion(darkBrush, region);
                    }
                    else
                    {
                        canvas.DrawRect(r, darkBrush);
                    }
                }

                //else
                //{
                //    canvas.DrawRect(r, darkBrush);

                //    image.Mutate(c => c.Fill(lightBrush, new Rectangle(x - offset, y - offset, pixelsPerModule, pixelsPerModule)));
                //}
            }
        }

        if (drawIconFlag)
        {
            var iconDestRect = new SKRect(iconX, iconY, iconDestWidth, iconDestHeight);
            //gfx.DrawImage(icon, iconDestRect, new RectangleF(0, 0, icon.Width, icon.Height), GraphicsUnit.Pixel);
        }

        return SKImage.FromBitmap(image);
    }

    //internal GraphicsPath CreateRoundedRectanglePath(RectangleF rect, int cornerRadius)
    //{
    //    var roundedRect = new GraphicsPath();
    //    roundedRect.AddArc(rect.X, rect.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
    //    roundedRect.AddLine(rect.X + cornerRadius, rect.Y, rect.Right - cornerRadius * 2, rect.Y);
    //    roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
    //    roundedRect.AddLine(rect.Right, rect.Y + cornerRadius * 2, rect.Right, rect.Y + rect.Height - cornerRadius * 2);
    //    roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y + rect.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
    //    roundedRect.AddLine(rect.Right - cornerRadius * 2, rect.Bottom, rect.X + cornerRadius * 2, rect.Bottom);
    //    roundedRect.AddArc(rect.X, rect.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
    //    roundedRect.AddLine(rect.X, rect.Bottom - cornerRadius * 2, rect.X, rect.Y + cornerRadius * 2);
    //    roundedRect.CloseFigure();
    //    return roundedRect;
    //}

    public static SKColor FromHtml(string color)
    {
        return SKColor.Parse(color);
    }
}

public static class QRCodeHelper
{
    public static SKImage GetQRCode(string plainText, int pixelsPerModule, SKColor darkColor, SKColor lightColor, ECCLevel eccLevel, bool forceUtf8 = false, bool utf8BOM = false, EciMode eciMode = EciMode.Default, int requestedVersion = -1, SKImage? icon = null, int iconSizePercent = 15, int iconBorderWidth = 6, bool drawQuietZones = true)
    {
        using var qrGenerator = new Genocs.QRCodeGenerator.Encoder.QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(plainText, eccLevel, forceUtf8, utf8BOM, eciMode, requestedVersion);
        using var qrCode = new QRCode(qrCodeData);
        return qrCode.GetGraphic(pixelsPerModule, darkColor, lightColor, icon, iconSizePercent, iconBorderWidth, drawQuietZones);
    }
}
