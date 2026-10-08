using Genocs.BarcodeLibrary.Symbologies;
using Xunit;

namespace Genocs.BarcodeLibrary.UnitTests;

public class PharmaCodeUnitTests
{
    [Theory]
    [InlineData("3", "1001")]
    [InlineData("10", "10011100111")]
    [InlineData("13", "11100111001")]
    [InlineData("131070", "111001110011100111001110011100111001110011100111001110011100111001110011100111")]
    public void pharma_code_valid_values_should_encode_valid_values(string data, string expected)
    {
        // Arrange
        var barcode = new Pharmacode(data);

        // Act
        string encodedValue = barcode.EncodedValue;

        // Assert
        Assert.NotNull(encodedValue);
        Assert.Equal(expected, encodedValue);
    }

    [Theory]
    [InlineData("2")]
    [InlineData("131072")]
    public void pharma_code_invalid_values_should_throw_exception(string data)
    {
        // Arrange
        var barcode = new Pharmacode(data);

        // Act & Assert
        Assert.Throws<Exception>(() => barcode.EncodedValue);
    }
}
