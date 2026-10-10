using Genocs.QRCodeLibrary.Encoder;
using Shouldly;
using Xunit;

namespace Genocs.QRCodeLibrary.UnitTests;

public class BitmapByteQRCodeTests
{
    [Fact]
    public void get_graphic_reverses_rgb_channels_without_mutating_input_colors()
    {
        byte[] darkColor = [1, 2, 3];
        byte[] lightColor = [4, 5, 6];
        using var qrCodeData = QRCodeGenerator.CreateQrCode("bitmap-color-test", QRCodeGenerator.ECCLevel.M);
        using var qrCode = new BitmapByteQRCode(qrCodeData);

        byte[] graphic = qrCode.GetGraphic(1, darkColor, lightColor);
        bool firstModuleIsDark = qrCodeData.ModuleMatrix[^1][0];

        graphic.Skip(26).Take(3).ToArray().ShouldBe(firstModuleIsDark ? [3, 2, 1] : [6, 5, 4]);
        darkColor.ShouldBe([1, 2, 3]);
        lightColor.ShouldBe([4, 5, 6]);
    }
}
