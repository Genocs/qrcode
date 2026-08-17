namespace Genocs.BarcodeLibrary.Symbologies;

/// <summary>
/// UPC Supplement-5 encoding.
/// </summary>
internal class UPCSupplement5 : BarcodeEncoding, IBarcode
{
    private readonly string[] EAN_CodeA = { "0001101", "0011001", "0010011", "0111101", "0100011", "0110001", "0101111", "0111011", "0110111", "0001011" };
    private readonly string[] EAN_CodeB = { "0100111", "0110011", "0011011", "0100001", "0011101", "0111001", "0000101", "0010001", "0001001", "0010111" };
    private readonly string[] UPC_SUPP_5 = { "bbaaa", "babaa", "baaba", "baaab", "abbaa", "aabba", "aaabb", "ababa", "abaab", "aabab" };

    public UPCSupplement5(string input)
    {
        RawData = input;
    }

    /// <summary>
    /// Encode the raw data using the UPC Supplemental 5-digit algorithm.
    /// </summary>
    protected override string Encode()
    {
        if (RawData.Length != 5)
        {
            Error("EUPC-SUP5-1: Invalid data length. (Length = 5 required)");
        }

        if (!CheckNumericOnly(RawData))
        {
            Error("EUPCA-2: Numeric Data Only");
        }

        // Calculate the checksum digit
        var even = 0;
        var odd = 0;

        // Odd
        for (var i = 0; i <= 4; i += 2)
        {
            odd += Int32.Parse(RawData.Substring(i, 1)) * 3;
        }

        // Even
        for (var i = 1; i < 4; i += 2)
        {
            even += Int32.Parse(RawData.Substring(i, 1)) * 9;
        }

        var total = even + odd;
        var cs = total % 10;

        var pattern = UPC_SUPP_5[cs];

        var result = "";

        var pos = 0;
        foreach (var c in pattern)
        {
            // Inter-character separator
            if (pos == 0) result += "1011";
            else result += "01";

            switch (c)
            {
                case 'a':
                    // Encode using odd parity
                    result += EAN_CodeA[int.Parse(RawData[pos].ToString())];
                    break;
                case 'b':
                    // Encode using even parity
                    result += EAN_CodeB[int.Parse(RawData[pos].ToString())];
                    break;
            }

            pos++;
        }

        return result;
    }

}
