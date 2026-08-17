namespace Genocs.BarcodeLibrary.Symbologies;

/// <summary>
///  Standard 2 of 5 encoding.
/// </summary>
internal class Standard2of5 : BarcodeEncoding, IBarcode
{
    private static readonly string[] Standard2of5Codes = { "10101110111010", "11101010101110", "10111010101110", "11101110101010", "10101110101110", "11101011101010", "10111011101010", "10101011101110", "11101010111010", "10111010111010" };

    private readonly BarcodeType _encodedType = BarcodeType.Unspecified;

    public Standard2of5(string input, BarcodeType encodedType)
    {
        RawData = input;
        _encodedType = encodedType;
    }

    /// <summary>
    /// Encode the raw data using the Standard 2 of 5 algorithm.
    /// </summary>
    protected override string Encode()
    {
        if (!CheckNumericOnly(RawData))
            Error("ES25-1: Numeric Data Only");

        string result = "11011010";

        for (int i = 0; i < RawData.Length; i++)
        {
            result += Standard2of5Codes[(int)char.GetNumericValue(RawData, i)];
        }

        result += _encodedType == BarcodeType.Standard2Of5Mod10 ? Standard2of5Codes[CalculateMod10CheckDigit()] : string.Empty;

        // Add ending bars
        result += "1101011";
        return result;
    }

    private int CalculateMod10CheckDigit()
    {
        int sum = 0;
        bool even = true;
        for (int i = RawData.Length - 1; i >= 0; --i)
        {
            // convert numeric in char format to integer and
            // multiply by 3 or 1 based on if an even index from the end
            sum += (RawData[i] - '0') * (even ? 3 : 1);
            even = !even;
        }

        return (10 - (sum % 10)) % 10;
    }
}
