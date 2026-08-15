namespace Genocs.BarcodeLibrary.Symbologies;

/// <summary>
/// Code 11 encoding.
/// </summary>
internal class Code11 : BarcodeEncoding, IBarcode
{
    private readonly string[] C11_Code = { "101011", "1101011", "1001011", "1100101", "1011011", "1101101", "1001101", "1010011", "1101001", "110101", "101101", "1011001" };

    public Code11(string input)
    {
        RawData = input;
    }

    /// <summary>
    /// Encode the raw data using the Code 11 algorithm.
    /// </summary>
    protected override string Encode()
    {
        if (!CheckNumericOnly(RawData.Replace("-", string.Empty)))
        {
            Error("EC11-1: Numeric data and '-' Only");
        }

        // Calculate the checksums
        var weight = 1;
        var cTotal = 0;
        var dataToEncodeWithChecksums = RawData;

        // Figure the C checksum
        for (var i = RawData.Length - 1; i >= 0; i--)
        {
            // C checksum weights go 1-10
            if (weight == 10) weight = 1;

            if (RawData[i] != '-')
                cTotal += Int32.Parse(RawData[i].ToString()) * weight++;
            else
                cTotal += 10 * weight++;
        }

        var checksumC = cTotal % 11;

        dataToEncodeWithChecksums += checksumC.ToString();

        // K checksums are recommended on any message length greater than or equal to 10
        if (RawData.Length >= 10)
        {
            weight = 1;
            var kTotal = 0;

            //calculate K checksum
            for (var i = dataToEncodeWithChecksums.Length - 1; i >= 0; i--)
            {
                //K checksum weights go 1-9
                if (weight == 9) weight = 1;

                if (dataToEncodeWithChecksums[i] != '-')
                    kTotal += Int32.Parse(dataToEncodeWithChecksums[i].ToString()) * weight++;
                else
                    kTotal += 10 * weight++;
            }

            var checksumK = kTotal % 11;
            dataToEncodeWithChecksums += checksumK.ToString();
        }

        // Encode data
        const string space = "0";
        string result = C11_Code[11] + space; // start-stop char + interchar space

        foreach (char c in dataToEncodeWithChecksums)
        {
            int index = c == '-' ? 10 : int.Parse(c.ToString());
            result += C11_Code[index];

            // inter-character space
            result += space;
        }

        // Stop bars
        result += C11_Code[11];

        return result;
    }
}
