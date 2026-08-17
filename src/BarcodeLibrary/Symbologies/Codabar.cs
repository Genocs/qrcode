using System.Text;

namespace Genocs.BarcodeLibrary.Symbologies;

/// <summary>
/// Codabar encoding.
/// </summary>
internal class Codabar : BarcodeEncoding, IBarcode
{
    private static readonly System.Collections.Hashtable CodabarCodes = new();

    public Codabar(string input)
    {
        RawData = input;

        // Populate the hashtable to begin the process
        Init();
    }

    /// <summary>
    /// Encode the raw data using the Codabar algorithm.
    /// </summary>
    protected override string Encode()
    {
        if (RawData.Length < 2)
        {
            Error("ECODABAR-1: Data format invalid. (Invalid length)");
        }

        // Check first char to make sure its a start/stop char
        switch (RawData[0].ToString().ToUpper().Trim())
        {
            case "A": break;
            case "B": break;
            case "C": break;
            case "D": break;
            default:
                Error("ECODABAR-2: Data format invalid. (Invalid START character)");
                break;
        }

        // Check the ending char to make sure its a start/stop char
        switch (RawData[RawData.Trim().Length - 1].ToString().ToUpper().Trim())
        {
            case "A": break;
            case "B": break;
            case "C": break;
            case "D": break;
            default:
                Error("ECODABAR-3: Data format invalid. (Invalid STOP character)");
                break;
        }

        // Replace non-numeric VALID chars with empty strings before checking for all numerics
        string temp = RawData;

        foreach (char c in CodabarCodes.Keys)
        {
            if (!CheckNumericOnly(c.ToString()))
            {
                temp = temp.Replace(c, '1');
            }
        }

        // Now that all the valid non-numeric chars have been replaced with a number check if all numeric exist
        if (!CheckNumericOnly(temp))
        {
            Error("ECODABAR-4: Data contains invalid  characters.");
        }

        StringBuilder stringBuilder = new StringBuilder();

        foreach (char c in RawData)
        {
            stringBuilder.Append(CodabarCodes[c]);
            stringBuilder.Append("0"); // inter-character space
        }

        string result = stringBuilder.ToString();

        // Remove the extra 0 at the end of the result
        result = result[..^1];

        // clears the hashtable so it no longer takes up memory
        CodabarCodes.Clear();

        // change the Raw_Data to strip out the start stop chars for label purposes
        RawData = RawData.Trim()[1..^1];

        return result;
    }

    private static void Init()
    {
        CodabarCodes.Clear();
        CodabarCodes.Add('0', "101010011"); // "101001101101");
        CodabarCodes.Add('1', "101011001"); // "110100101011");
        CodabarCodes.Add('2', "101001011"); // "101100101011");
        CodabarCodes.Add('3', "110010101"); // "110110010101");
        CodabarCodes.Add('4', "101101001"); // "101001101011");
        CodabarCodes.Add('5', "110101001"); // "110100110101");
        CodabarCodes.Add('6', "100101011"); // "101100110101");
        CodabarCodes.Add('7', "100101101"); // "101001011011");
        CodabarCodes.Add('8', "100110101"); // "110100101101");
        CodabarCodes.Add('9', "110100101"); // "101100101101");
        CodabarCodes.Add('-', "101001101"); // "110101001011");
        CodabarCodes.Add('$', "101100101"); // "101101001011");
        CodabarCodes.Add(':', "1101011011"); // "110110100101");
        CodabarCodes.Add('/', "1101101011"); // "101011001011");
        CodabarCodes.Add('.', "1101101101"); // "110101100101");
        CodabarCodes.Add('+', "101100110011"); // "101101100101");
        CodabarCodes.Add('A', "1011001001"); // "110110100101");
        CodabarCodes.Add('B', "1010010011"); // "101011001011");
        CodabarCodes.Add('C', "1001001011"); // "110101100101");
        CodabarCodes.Add('D', "1010011001"); // "101101100101");
        CodabarCodes.Add('a', "1011001001"); // "110110100101");
        CodabarCodes.Add('b', "1010010011"); // "101011001011");
        CodabarCodes.Add('c', "1001001011"); // "110101100101");
        CodabarCodes.Add('d', "1010011001"); // "101101100101");
    }
}
