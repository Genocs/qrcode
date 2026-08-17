using Genocs.QRCodeLibrary.Encoder;
using Shouldly;
using SkiaSharp;
using Xunit;

namespace Genocs.QRCodeLibrary.UnitTests;

public class SvgQRCodeTests
{
    [Fact]
    public void get_graphic_with_module_size_returns_svg_markup()
    {
        using var qrCode = new SvgQRCode(QRCodeGenerator.CreateQrCode("svg-smoke-test", QRCodeGenerator.ECCLevel.M));

        string svg = qrCode.GetGraphic(20);

        svg.ShouldStartWith("<svg");
        svg.ShouldContain("</svg>");
        svg.ShouldContain("fill=\"#000000\"");
        svg.ShouldContain("fill=\"#FFFFFF\"");
    }

    [Fact]
    public void get_graphic_color_overload_uses_hex_and_does_not_recurse()
    {
        using var qrCode = new SvgQRCode(QRCodeGenerator.CreateQrCode("svg-color-test", QRCodeGenerator.ECCLevel.M));

        string svg = qrCode.GetGraphic(8, SKColor.Parse("0A141E"), SKColor.Parse("C8D2DC"));

        svg.ShouldStartWith("<svg");
        svg.ShouldContain("fill=\"#0A141E\"");
        svg.ShouldContain("fill=\"#C8D2DC\"");
    }
}
