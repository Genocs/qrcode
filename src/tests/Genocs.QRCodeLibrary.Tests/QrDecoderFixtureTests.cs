using Genocs.QRCodeLibrary.Decoder;
using Shouldly;
using SkiaSharp;
using Xunit;

namespace Genocs.QRCodeLibrary.Tests;

public class QrDecoderFixtureTests
{
    public static TheoryData<string, string> DecodeFixtures { get; } = new()
    {
        { "decode-v1-ecc-l-rot0-light.png", "GNX-V1-L" },
        { "decode-v2-ecc-m-rot0-light.png", "GNX-V2-M" },
        { "decode-v3-ecc-q-rot0-light.png", "GNX-V3-Q" },
        { "decode-v4-ecc-h-rot0-light.png", "GNX-V4-H" },
        { "decode-v5-ecc-m-rot0-light.png", "GNX-V5-M" },
        { "decode-v6-ecc-l-rot0-light.png", "GNX-V6-L" },
        { "decode-v7-ecc-q-rot15-light.png", "GNX-V7-Q-ROT" },
        { "decode-v3-ecc-m-rot0-dark.png", "GNX-V3-M-DARK" },
        { "gnx-qr-fixture-v1.png", "GNX-QR-FIXTURE-v1" },
    };

    [Theory]
    [MemberData(nameof(DecodeFixtures))]
    public void image_decoder_reads_expected_payload_from_fixture(string fileName, string expectedPayload)
    {
        string fixturePath = HelperUnitTests.GetDemoFile(fileName);
        File.Exists(fixturePath).ShouldBeTrue($"Fixture not found: {fixturePath}");

        byte[] bytes = File.ReadAllBytes(fixturePath);
        using SKImage? image = SKImage.FromEncodedData(bytes);
        image.ShouldNotBeNull($"SkiaSharp could not decode fixture: {fixturePath}");

        var decoder = new QRDecoder();
        QrCodeResult? result = decoder.ImageDecoder(image);

        result.ShouldNotBeNull($"Decoder returned no result for {fileName}");
        result.Results.ShouldNotBeEmpty($"Decoder returned empty results for {fileName}");
        result.Results.ShouldContain(expectedPayload);
    }
}
