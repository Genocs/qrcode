using System.Drawing;
using Genocs.QRCodeLibrary.Encoder;
using Shouldly;
using Xunit;

namespace Genocs.QRCodeLibrary.Tests;

public class SvgQRCodeTests
{
    [Fact]
    public void get_graphic_with_module_size_returns_svg_markup()
    {
        using var generator = new QRCodeGenerator();
        using var qrCode = new SvgQRCode(generator.CreateQrCode("svg-smoke-test", QRCodeGenerator.ECCLevel.M));

        string svg = qrCode.GetGraphic(20);

        svg.ShouldStartWith("<svg");
        svg.ShouldContain("</svg>");
        svg.ShouldContain("fill=\"#000000\"");
        svg.ShouldContain("fill=\"#FFFFFF\"");
    }

    [Fact]
    public void get_graphic_color_overload_uses_hex_and_does_not_recurse()
    {
        using var generator = new QRCodeGenerator();
        using var qrCode = new SvgQRCode(generator.CreateQrCode("svg-color-test", QRCodeGenerator.ECCLevel.M));

        string svg = qrCode.GetGraphic(8, Color.FromArgb(255, 10, 20, 30), Color.FromArgb(255, 200, 210, 220));

        svg.ShouldStartWith("<svg");
        svg.ShouldContain("fill=\"#0A141E\"");
        svg.ShouldContain("fill=\"#C8D2DC\"");
    }
}
