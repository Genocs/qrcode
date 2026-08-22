namespace Genocs.BarcodeLibrary.Symbologies;

/// <summary>
///  ITF-14 encoding.
/// </summary>
internal class ITF14 : BarcodeEncoding, IBarcode
{
    private readonly string[] ITF14_Code = { "NNWWN", "WNNNW", "NWNNW", "WWNNN", "NNWNW", "WNWNN", "NWWNN", "NNNWW", "WNNWN", "NWNWN" };

    public ITF14(string input)
    {
        RawData = input;

        CheckDigit();
    }

    /// <summary>
    /// Encode the raw data using the ITF-14 algorithm.
    /// </summary>
    protected override string Encode()
    {
        // check length of input
        if (RawData.Length > 14 || RawData.Length < 13)
        {
            Error("EITF14-1: Data length invalid. (Length must be 13 or 14)");
        }

        if (!CheckNumericOnly(RawData))
        {
            Error("EITF14-2: Numeric data only.");
        }

        string result = "1010";

        for (var i = 0; i < RawData.Length; i += 2)
        {
            bool bars = true;
            var patternBars = ITF14_Code[int.Parse(RawData[i].ToString())];
            var patSpaces = ITF14_Code[int.Parse(RawData[i + 1].ToString())];
            var patternmMixed = string.Empty;

            // interleave
            while (patternBars.Length > 0)
            {
                patternmMixed += patternBars[0].ToString() + patSpaces[0].ToString();
                patternBars = patternBars.Substring(1);
                patSpaces = patSpaces.Substring(1);
            }

            foreach (var c1 in patternmMixed)
            {
                if (bars)
                {
                    if (c1 == 'N')
                        result += "1";
                    else
                        result += "11";
                }
                else
                {
                    if (c1 == 'N')
                        result += "0";
                    else
                        result += "00";
                }

                bars = !bars;
            }

        }

        // Add ending bars
        result += "1101";
        return result;
    }

    private void CheckDigit()
    {
        // Calculate and include checksum if it is necessary
        if (RawData.Length == 13)
        {
            var total = 0;

            for (var i = 0; i <= RawData.Length - 1; i++)
            {
                var temp = int.Parse(RawData.Substring(i, 1));
                total += temp * ((i == 0 || i % 2 == 0) ? 3 : 1);
            }

            var cs = total % 10;
            cs = 10 - cs;
            if (cs == 10)
                cs = 0;

            RawData += cs.ToString();
        }
    }
}
