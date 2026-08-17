namespace Genocs.BarcodeLibrary.Symbologies;

/// <summary>
///  Interleaved 2 of 5 encoding.
/// </summary>
internal class Interleaved2of5 : BarcodeEncoding, IBarcode
{
    private readonly string[] _i25Code = { "NNWWN", "WNNNW", "NWNNW", "WWNNN", "NNWNW", "WNWNN", "NWWNN", "NNNWW", "WNNWN", "NWNWN" };
    private readonly BarcodeType _encodedType;

    public Interleaved2of5(string input, BarcodeType encodedType)
    {
        _encodedType = encodedType;
        RawData = input;
    }

    /// <summary>
    /// Encode the raw data using the Interleaved 2 of 5 algorithm.
    /// </summary>
    protected override string Encode()
    {
        // Check length of input (only even if no checkdigit, else with check digit odd)
        if (RawData.Length % 2 != (_encodedType == BarcodeType.Interleaved2Of5Mod10 ? 1 : 0))
        {
            Error("EI25-1: Data length invalid.");
        }

        if (!CheckNumericOnly(RawData))
        {
            Error("EI25-2: Numeric Data Only");
        }

        string result = "1010";
        string data = RawData + (_encodedType == BarcodeType.Interleaved2Of5Mod10 ? CalculateMod10CheckDigit().ToString() : "");

        for (int i = 0; i < data.Length; i += 2)
        {
            bool bars = true;
            string patternBars = _i25Code[(int)char.GetNumericValue(data, i)];
            string patternSpaces = _i25Code[(int)char.GetNumericValue(data, i + 1)];
            string patternMixed = string.Empty;

            // Interleave
            while (patternBars.Length > 0)
            {
                patternMixed += patternBars[0].ToString() + patternSpaces[0].ToString();
                patternBars = patternBars.Substring(1);
                patternSpaces = patternSpaces.Substring(1);
            }

            foreach (char c1 in patternMixed)
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

    private int CalculateMod10CheckDigit()
    {
        int sum = 0;
        bool even = true;
        for (int i = RawData.Length - 1; i >= 0; --i)
        {
            // Convert numeric in char format to integer and
            // multiply by 3 or 1 based on if an even index from the end
            sum += (RawData[i] - '0') * (even ? 3 : 1);
            even = !even;
        }

        return (10 - (sum % 10)) % 10;
    }
}
