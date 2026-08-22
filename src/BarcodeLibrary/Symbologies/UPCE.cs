namespace Genocs.BarcodeLibrary.Symbologies;

/// <summary>
///  UPC-E encoding.
/// </summary>
internal class UPCE : BarcodeEncoding, IBarcode
{
    private readonly string[] EAN_Code_A = { "0001101", "0011001", "0010011", "0111101", "0100011", "0110001", "0101111", "0111011", "0110111", "0001011" };
    private readonly string[] EAN_Code_B = { "0100111", "0110011", "0011011", "0100001", "0011101", "0111001", "0000101", "0010001", "0001001", "0010111" };
    private readonly string[] EAN_Pattern = { "aaaaaa", "aababb", "aabbab", "aabbba", "abaabb", "abbaab", "abbbaa", "ababab", "ababba", "abbaba" };
    private readonly string[] UPC_E_Code0 = { "bbbaaa", "bbabaa", "bbaaba", "bbaaab", "babbaa", "baabba", "baaabb", "bababa", "babaab", "baabab" };
    private readonly string[] UPC_E_Code1 = { "aaabbb", "aababb", "aabbab", "aabbba", "abaabb", "abbaab", "abbbaa", "ababab", "ababba", "abbaba" };

    /// <summary>
    /// Encodes a UPC-E symbol.
    /// </summary>
    /// <param name="input">Data to encode.</param>
    public UPCE(string input)
    {
        RawData = input;
    }

    /// <summary>
    /// Encode the raw data using the UPC-E algorithm.
    /// </summary>
    protected override string Encode()
    {
        if (RawData.Length != 6 && RawData.Length != 8 && RawData.Length != 12)
        {
            Error("EUPCE-1: Invalid data length. (8 or 12 numbers only)");
        }

        if (!CheckNumericOnly(RawData))
        {
            Error("EUPCE-2: Numeric only.");
        }

        // Check for a valid number system
        int numberSystem = int.Parse(RawData[0].ToString());
        if (numberSystem != 0 && numberSystem != 1)
            Error("EUPCE-3: Invalid Number System (only 0 & 1 are valid)");

        int checkDigit = int.Parse(RawData[RawData.Length - 1].ToString());

        // Convert to UPC-E from UPC-A if necessary
        if (RawData.Length == 12)
        {
            string upc_e_code = string.Empty;

            // Break apart into components
            string manufacturer = RawData.Substring(1, 5);
            string productCode = RawData.Substring(6, 5);

            if (manufacturer.EndsWith("000") || manufacturer.EndsWith("100") || manufacturer.EndsWith("200") && Int32.Parse(productCode) <= 999)
            {
                // Rule 1
                upc_e_code += manufacturer.Substring(0, 2); // first two of manufacturer
                upc_e_code += productCode.Substring(2, 3); // last three of product
                upc_e_code += manufacturer[2].ToString(); // third of manufacturer
            }
            else if (manufacturer.EndsWith("00") && int.Parse(productCode) <= 99)
            {
                // Rule 2
                upc_e_code += manufacturer.Substring(0, 3); // first three of manufacturer
                upc_e_code += productCode.Substring(3, 2); // last two of product
                upc_e_code += "3"; // number 3
            }
            else if (manufacturer.EndsWith("0") && int.Parse(productCode) <= 9)
            {
                // Rule 3
                upc_e_code += manufacturer.Substring(0, 4); // first four of manufacturer
                upc_e_code += productCode[4]; // last digit of product
                upc_e_code += "4"; // number 4
            }
            else if (!manufacturer.EndsWith("0") && int.Parse(productCode) <= 9 && int.Parse(productCode) >= 5)
            {
                // Rule 4
                upc_e_code += manufacturer; // manufacturer
                upc_e_code += productCode[4]; // last digit of product
            }
            else
            {
                Error("EUPCE-4: Illegal UPC-A entered for conversion.  Unable to convert.");
            }

            RawData = upc_e_code;
        }

        // Get encoding pattern
        string pattern = string.Empty;

        if (numberSystem == 0)
        {
            pattern = UPC_E_Code0[checkDigit];
        }
        else
        {
            pattern = UPC_E_Code1[checkDigit];
        }

        // Encode the data
        string result = "101";

        int pos = 0;
        foreach (char c in pattern)
        {
            int i = int.Parse(RawData[pos++].ToString());
            if (c == 'a')
            {
                result += EAN_Code_A[i];
            }
            else if (c == 'b')
            {
                result += EAN_Code_B[i];
            }
        }

        // Guard + End bars
        result += "010101";

        return result;
    }
}
