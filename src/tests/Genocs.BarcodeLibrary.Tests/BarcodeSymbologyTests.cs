using SkiaSharp;
using Xunit;

namespace Genocs.BarcodeLibrary.Tests;

public class BarcodeSymbologyTests
{
    public static TheoryData<BarcodeType, string> ValidSymbologies { get; } = new()
    {
        { BarcodeType.UpcA, "038000356216" },
        { BarcodeType.Ucc12, "038000356216" },
        { BarcodeType.UpcE, "012345" },
        { BarcodeType.UpcSupplemental2Digit, "12" },
        { BarcodeType.UpcSupplemental5Digit, "51234" },
        { BarcodeType.Ean13, "4006381333931" },
        { BarcodeType.Ucc13, "4006381333931" },
        { BarcodeType.Ean8, "90311017" },
        { BarcodeType.Interleaved2Of5, "123456" },
        { BarcodeType.Interleaved2Of5Mod10, "12345" },
        { BarcodeType.Standard2Of5, "1234567" },
        { BarcodeType.Standard2Of5Mod10, "1234567" },
        { BarcodeType.Industrial2Of5, "1234567" },
        { BarcodeType.Industrial2Of5Mod10, "1234567" },
        { BarcodeType.Code39, "CODE39" },
        { BarcodeType.Logmars, "CODE39" },
        { BarcodeType.Code39Extended, "Code39!" },
        { BarcodeType.Code39Mod43, "CODE39" },
        { BarcodeType.Codabar, "A123456A" },
        { BarcodeType.PostNet, "12345" },
        { BarcodeType.Isbn, "978020137962" },
        { BarcodeType.Bookland, "978020137962" },
        { BarcodeType.Jan13, "4901234567894" },
        { BarcodeType.MsiMod10, "1234567" },
        { BarcodeType.Msi2Mod10, "1234567" },
        { BarcodeType.MsiMod11, "1234567" },
        { BarcodeType.MsiMod11Mod10, "1234567" },
        { BarcodeType.ModifiedPlessey, "1234567" },
        { BarcodeType.Code11, "123-45" },
        { BarcodeType.Usd8, "123-45" },
        { BarcodeType.Code128, "Hello-128" },
        { BarcodeType.Code128A, "HELLO" },
        { BarcodeType.Code128B, "Hello-128" },
        { BarcodeType.Code128C, "123456" },
        { BarcodeType.Itf14, "1540014128876" },
        { BarcodeType.Code93, "CODE93" },
        { BarcodeType.Telepen, "TELEPEN" },
        { BarcodeType.Fim, "A" },
        { BarcodeType.Pharmacode, "1234" },
    };

    [Fact]
    public void valid_cases_cover_every_encoded_symbology()
    {
        BarcodeType[] covered = ValidSymbologies
            .Select(row => row.Data.Item1)
            .Distinct()
            .ToArray();

        BarcodeType[] missing = Enum.GetValues<BarcodeType>()
            .Where(type => type != BarcodeType.Unspecified && !covered.Contains(type))
            .ToArray();

        Assert.True(missing.Length == 0, "Uncovered BarcodeType values: " + string.Join(", ", missing));
    }

    [Theory]
    [MemberData(nameof(ValidSymbologies))]
    public void generate_barcode_returns_binary_pattern(BarcodeType type, string data)
    {
        using var barcode = new Barcode(data, type);

        Assert.False(string.IsNullOrEmpty(barcode.EncodedValue));
        Assert.Matches("^[01]+$", barcode.EncodedValue);
        Assert.Contains('0', barcode.EncodedValue);
        Assert.Contains('1', barcode.EncodedValue);
        Assert.True(barcode.EncodedValue.Length >= 8, barcode.EncodedValue);
    }

    [Theory]
    [MemberData(nameof(ValidSymbologies))]
    public void encode_returns_png_with_bars(BarcodeType type, string data)
    {
        using var barcode = new Barcode();
        using SKImage image = barcode.Encode(type, data, width: 2400, height: 120);

        Assert.True(image.Width > 0);
        Assert.True(image.Height > 0);
        Assert.False(string.IsNullOrEmpty(barcode.EncodedValue));

        using SKData png = image.Encode(SKEncodedImageFormat.Png, 100);
        Assert.True(png.Size > 32);
    }

    [Theory]
    [InlineData(BarcodeType.Code128, "Hello-128")]
    [InlineData(BarcodeType.Code39, "CODE39")]
    [InlineData(BarcodeType.Ean13, "4006381333931")]
    [InlineData(BarcodeType.Ean8, "90311017")]
    [InlineData(BarcodeType.UpcA, "038000356216")]
    [InlineData(BarcodeType.Itf14, "1540014128876")]
    public void encode_is_deterministic_for_core_symbologies(BarcodeType type, string data)
    {
        using var first = new Barcode(data, type);
        using var second = new Barcode(data, type);

        Assert.Equal(first.EncodedValue, second.EncodedValue);
    }

    [Theory]
    [InlineData(BarcodeType.UpcA, BarcodeType.Ucc12, "038000356216")]
    [InlineData(BarcodeType.Ean13, BarcodeType.Ucc13, "4006381333931")]
    [InlineData(BarcodeType.Code39, BarcodeType.Logmars, "CODE39")]
    [InlineData(BarcodeType.Isbn, BarcodeType.Bookland, "978020137962")]
    [InlineData(BarcodeType.Code11, BarcodeType.Usd8, "123-45")]
    [InlineData(BarcodeType.Standard2Of5, BarcodeType.Industrial2Of5, "1234567")]
    public void alias_types_produce_the_same_pattern(BarcodeType left, BarcodeType right, string data)
    {
        using var first = new Barcode(data, left);
        using var second = new Barcode(data, right);

        Assert.Equal(first.EncodedValue, second.EncodedValue);
    }

    [Fact]
    public void unspecified_type_throws()
    {
        using var barcode = new Barcode();
        barcode.EncodedType = BarcodeType.Unspecified;

        Exception ex = Assert.Throws<Exception>(() => barcode.GenerateBarcode("12345"));
        Assert.Contains("EENCODE-2", ex.Message);
    }

    [Fact]
    public void blank_data_throws()
    {
        using var barcode = new Barcode();
        barcode.EncodedType = BarcodeType.Code128;

        Exception ex = Assert.Throws<Exception>(() => barcode.GenerateBarcode("   "));
        Assert.Contains("EENCODE-1", ex.Message);
    }

    [Theory]
    [InlineData(BarcodeType.Ean13, "123")]
    [InlineData(BarcodeType.Ean8, "12")]
    [InlineData(BarcodeType.UpcA, "123")]
    [InlineData(BarcodeType.Itf14, "123")]
    [InlineData(BarcodeType.Code39, "lower")]
    [InlineData(BarcodeType.Codabar, "123456")]
    [InlineData(BarcodeType.Jan13, "4006381333931")]
    [InlineData(BarcodeType.Fim, "Z")]
    public void invalid_payload_throws(BarcodeType type, string data)
    {
        using var barcode = new Barcode();
        barcode.EncodedType = type;

        Assert.Throws<Exception>(() => barcode.GenerateBarcode(data));
    }
}
