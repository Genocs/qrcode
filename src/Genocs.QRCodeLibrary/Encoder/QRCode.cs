using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;

using static Genocs.QRCodeLibrary.Encoder.QRCodeGenerator;

namespace Genocs.QRCodeLibrary.Encoder
{
    public class QRCode : AbstractQRCode, IDisposable
    {
        /// <summary>
        /// Constructor without params to be used in COM Objects connections
        /// </summary>
        public QRCode() { }

        public QRCode(QRCodeData data) : base(data) { }

        public Image GetGraphic(int pixelsPerModule)
        {
            return this.GetGraphic(pixelsPerModule, Color.Black, Color.White, true);
        }

        public Image GetGraphic(int pixelsPerModule, string darkColorHtmlHex, string lightColorHtmlHex, bool drawQuietZones = true)
        {
            return this.GetGraphic(pixelsPerModule, FromHtml(darkColorHtmlHex), FromHtml(lightColorHtmlHex), drawQuietZones);
        }

        public Image GetGraphic(int pixelsPerModule, Color darkColor, Color lightColor, bool drawQuietZones = true)
        {
            var size = (this.QrCodeData.ModuleMatrix.Count - (drawQuietZones ? 0 : 8)) * pixelsPerModule;
            var offset = drawQuietZones ? 0 : 4 * pixelsPerModule;

            IBrush lightBrush = Brushes.Solid(lightColor);
            IBrush darkBrush = Brushes.Solid(darkColor);

            var image = new Image<Rgba32>(size, size);

            for (var x = 0; x < size + offset; x = x + pixelsPerModule)
            {
                for (var y = 0; y < size + offset; y = y + pixelsPerModule)
                {
                    var module = this.QrCodeData.ModuleMatrix[(y + pixelsPerModule) / pixelsPerModule - 1][(x + pixelsPerModule) / pixelsPerModule - 1];

                    if (module)
                    {
                        image.Mutate(i => i.Fill(darkBrush, new Rectangle(x - offset, y - offset, pixelsPerModule, pixelsPerModule)));
                    }
                    else
                    {
                        image.Mutate(i => i.Fill(lightBrush, new Rectangle(x - offset, y - offset, pixelsPerModule, pixelsPerModule)));
                    }
                }
            }

            return image;
        }

        public Image GetGraphic(int pixelsPerModule, Color darkColor, Color lightColor, Image icon = null, int iconSizePercent = 15, int iconBorderWidth = 6, bool drawQuietZones = true)
        {
            var size = (this.QrCodeData.ModuleMatrix.Count - (drawQuietZones ? 0 : 8)) * pixelsPerModule;
            var offset = drawQuietZones ? 0 : 4 * pixelsPerModule;

            var image = new Image<Argb32>(size, size);

            IBrush lightBrush = Brushes.Solid(lightColor);
            IBrush darkBrush = Brushes.Solid(darkColor);


            var drawIconFlag = icon != null && iconSizePercent > 0 && iconSizePercent <= 100;

            //GraphicsPath iconPath = null;
            float iconDestWidth = 0, iconDestHeight = 0, iconX = 0, iconY = 0;

            if (drawIconFlag)
            {
                iconDestWidth = iconSizePercent * image.Width / 100f;
                iconDestHeight = drawIconFlag ? iconDestWidth * icon.Height / icon.Width : 0;
                iconX = (image.Width - iconDestWidth) / 2;
                iconY = (image.Height - iconDestHeight) / 2;

                var centerDest = new SixLabors.ImageSharp.RectangleF(iconX - iconBorderWidth, iconY - iconBorderWidth, iconDestWidth + iconBorderWidth * 2, iconDestHeight + iconBorderWidth * 2);
                //iconPath = this.CreateRoundedRectanglePath(centerDest, iconBorderWidth * 2);
            }

            for (var x = 0; x < size + offset; x = x + pixelsPerModule)
            {
                for (var y = 0; y < size + offset; y = y + pixelsPerModule)
                {
                    var module = this.QrCodeData.ModuleMatrix[(y + pixelsPerModule) / pixelsPerModule - 1][(x + pixelsPerModule) / pixelsPerModule - 1];

                    if (module)
                    {
                        var r = new Rectangle(x - offset, y - offset, pixelsPerModule, pixelsPerModule);

                        if (drawIconFlag)
                        {
                            //var region = new SixLabors.ImageSharp.Drawing.Region(r);
                            //region.Exclude(iconPath);
                            //gfx.FillRegion(darkBrush, region);
                        }
                        else
                        {
                            image.Mutate(c => c.Fill(darkBrush, r));
                        }
                    }
                    else
                    {
                        image.Mutate(c => c.Fill(lightBrush, new Rectangle(x - offset, y - offset, pixelsPerModule, pixelsPerModule)));
                    }
                }
            }

            if (drawIconFlag)
            {
                var iconDestRect = new RectangleF(iconX, iconY, iconDestWidth, iconDestHeight);
                //gfx.DrawImage(icon, iconDestRect, new RectangleF(0, 0, icon.Width, icon.Height), GraphicsUnit.Pixel);
            }


            return image;
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

        public static Color FromHtml(string color)
        {
            return Color.Gainsboro;
        }
    }

    public static class QRCodeHelper
    {
        public static Image GetQRCode(string plainText, int pixelsPerModule, Color darkColor, Color lightColor, ECCLevel eccLevel, bool forceUtf8 = false, bool utf8BOM = false, EciMode eciMode = EciMode.Default, int requestedVersion = -1, Image icon = null, int iconSizePercent = 15, int iconBorderWidth = 6, bool drawQuietZones = true)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(plainText, eccLevel, forceUtf8, utf8BOM, eciMode, requestedVersion);
            using var qrCode = new QRCode(qrCodeData);
            return qrCode.GetGraphic(pixelsPerModule, darkColor, lightColor, icon, iconSizePercent, iconBorderWidth, drawQuietZones);
        }
    }
}
