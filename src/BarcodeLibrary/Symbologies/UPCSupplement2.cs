namespace Genocs.BarcodeLibrary.Symbologies;

/// <summary>
/// UPC Supplement-2 encoding.
/// </summary>
internal class UPCSupplement2 : BarcodeEncoding, IBarcode
{
    private readonly string[] EAN_CodeA = { "0001101", "0011001", "0010011", "0111101", "0100011", "0110001", "0101111", "0111011", "0110111", "0001011" };
    private readonly string[] EAN_CodeB = { "0100111", "0110011", "0011011", "0100001", "0011101", "0111001", "0000101", "0010001", "0001001", "0010111" };
    private readonly string[] UPC_SUPP_2 = { "aa", "ab", "ba", "bb" };

    public UPCSupplement2(string input)
    {
        RawData = input;
    }

    /// <summary>
    /// Encode the raw data using the UPC Supplemental 2-digit algorithm.
    /// </summary>
    protected override string Encode()
    {
        if (RawData.Length != 2)
        {
            Error("EUPC-SUP2-1: Invalid data length. (Length = 2 required)");
        }

        if (!CheckNumericOnly(RawData))
        {
            Error("EUPC-SUP2-2: Numeric Data Only");
        }

        string pattern = string.Empty;

        try
        {
            pattern = UPC_SUPP_2[int.Parse(RawData.Trim()) % 4];
        }
        catch
        {
            Error("EUPC-SUP2-3: Invalid Data. (Numeric only)");
        }

        string result = "1011";

        int pos = 0;
        foreach (char c in pattern)
        {
            if (c == 'a')
            {
                // Encode using odd parity
                result += EAN_CodeA[int.Parse(RawData[pos].ToString())];
            }
            else if (c == 'b')
            {
                // Encode using even parity
                result += EAN_CodeB[int.Parse(RawData[pos].ToString())];
            }

            if (pos++ == 0)
            {
                result += "01"; // Inter-character separator
            }
        }

        return result;
    }
}
