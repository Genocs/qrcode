using Genocs.QRCodeLibrary.Decoder;
using Shouldly;
using SkiaSharp;
using Xunit;

namespace Genocs.QRCodeLibrary.Tests;

public class QrDecoderBinarizationTests
{
    [Fact]
    public void convert_image_to_black_and_white_marks_dark_pixels_as_black()
    {
        using SKImage image = CreateImage(32, 32, canvas =>
        {
            canvas.Clear(SKColors.White);
            using var paint = new SKPaint { Color = SKColors.Black, Style = SKPaintStyle.Fill };
            canvas.DrawRect(SKRect.Create(8, 8, 16, 16), paint);
        });

        var decoder = new QRDecoder();

        bool converted = decoder.ConvertImageToBlackAndWhite(image);

        converted.ShouldBeTrue();
        decoder.ImageWidth.ShouldBe(32);
        decoder.ImageHeight.ShouldBe(32);
        decoder.BlackWhiteImage[0, 0].ShouldBeFalse();
        decoder.BlackWhiteImage[16, 16].ShouldBeTrue();
        decoder.BlackWhiteImage[31, 31].ShouldBeFalse();
    }

    [Fact]
    public void convert_image_to_black_and_white_rejects_uniform_image()
    {
        using SKImage image = CreateImage(16, 16, canvas => canvas.Clear(SKColors.White));
        var decoder = new QRDecoder();

        decoder.ConvertImageToBlackAndWhite(image).ShouldBeFalse();
    }

    [Fact]
    public void convert_image_to_black_and_white_rejects_image_wider_than_cap()
    {
        using var bitmap = new SKBitmap(QRDecoder.MaxDecodeImageDimension + 1, 8);
        using SKImage image = SKImage.FromBitmap(bitmap);
        var decoder = new QRDecoder();

        decoder.ConvertImageToBlackAndWhite(image).ShouldBeFalse();
    }

    [Fact]
    public void convert_image_to_black_and_white_rejects_image_taller_than_cap()
    {
        using var bitmap = new SKBitmap(8, QRDecoder.MaxDecodeImageDimension + 1);
        using SKImage image = SKImage.FromBitmap(bitmap);
        var decoder = new QRDecoder();

        decoder.ConvertImageToBlackAndWhite(image).ShouldBeFalse();
    }

    [Fact]
    public void convert_image_to_black_and_white_reads_encoded_png_pixels()
    {
        using SKImage raster = CreateImage(24, 24, canvas =>
        {
            canvas.Clear(SKColors.White);
            using var paint = new SKPaint { Color = SKColors.Black, Style = SKPaintStyle.Fill };
            canvas.DrawRect(SKRect.Create(4, 4, 16, 16), paint);
        });
        using SKData png = raster.Encode(SKEncodedImageFormat.Png, 100);
        using SKImage encoded = SKImage.FromEncodedData(png);
        var decoder = new QRDecoder();

        decoder.ConvertImageToBlackAndWhite(encoded).ShouldBeTrue();
        decoder.BlackWhiteImage[0, 0].ShouldBeFalse();
        decoder.BlackWhiteImage[12, 12].ShouldBeTrue();
    }

    [Fact]
    public void convert_image_to_black_and_white_accepts_image_at_dimension_cap()
    {
        using SKImage image = CreateImage(QRDecoder.MaxDecodeImageDimension, 2, canvas =>
        {
            canvas.Clear(SKColors.White);
            using var paint = new SKPaint { Color = SKColors.Black, Style = SKPaintStyle.Fill };
            canvas.DrawRect(SKRect.Create(0, 0, 1, 2), paint);
        });

        var decoder = new QRDecoder();

        decoder.ConvertImageToBlackAndWhite(image).ShouldBeTrue();
        decoder.ImageWidth.ShouldBe(QRDecoder.MaxDecodeImageDimension);
        decoder.ImageHeight.ShouldBe(2);
        decoder.BlackWhiteImage[0, 0].ShouldBeTrue();
        decoder.BlackWhiteImage[0, QRDecoder.MaxDecodeImageDimension - 1].ShouldBeFalse();
    }

    private static SKImage CreateImage(int width, int height, Action<SKCanvas> draw)
    {
        var info = new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Unpremul);
        using var bitmap = new SKBitmap(info);
        using var canvas = new SKCanvas(bitmap);
        draw(canvas);
        canvas.Flush();
        return SKImage.FromBitmap(bitmap);
    }
}
