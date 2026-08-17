namespace Genocs.BarcodeLibrary.Symbologies;

/// <summary>
/// Pharmacode encoding.
/// </summary>
internal class Pharmacode : BarcodeEncoding, IBarcode
{
    private readonly string _thinBar = "1";
    private readonly string _gap = "00";
    private readonly string _thickBar = "111";

    /// <summary>
    /// Encodes with Pharmacode.
    /// </summary>
    /// <param name="input">Data to encode.</param>
    public Pharmacode(string input)
    {
        RawData = input;

        if (!CheckNumericOnly(RawData))
        {
            Error("EPHARM-1: Data contains invalid characters (non-numeric).");
        }
        else if (RawData.Length > 6)
        {
            Error("EPHARM-2: Data too long (invalid data input length).");
        }
    }

    /// <summary>
    /// Encode the raw data using the Pharmacode algorithm.
    /// </summary>
    protected override string Encode()
    {
        if (!int.TryParse(RawData, out int num))
        {
            Error("EPHARM-3: Input is unparseable.");
        }

        if (num < 3 || num > 131070)
        {
            Error("EPHARM-4: Data contains invalid characters (invalid numeric range). The lenght must be between 3 and 131070.");
        }

        List<string> bars = new List<string>();

        do
        {
            if ((num & 1) == 0)
            {
                bars.Add(_thickBar);
                num = (num - 2) / 2;
            }
            else
            {
                bars.Add(_thinBar);
                num = (num - 1) / 2;
            }
        }
        while (num != 0);

        bars.Reverse();
        return string.Join(_gap, bars);
    }
}
