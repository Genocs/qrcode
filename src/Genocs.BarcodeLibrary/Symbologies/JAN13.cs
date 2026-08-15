namespace Genocs.BarcodeLibrary.Symbologies;

/// <summary>
///  JAN-13 encoding.
/// </summary>
internal class JAN13 : BarcodeEncoding, IBarcode
{
    public JAN13(string input)
    {
        RawData = input;
    }

    /// <summary>
    /// Encode the raw data using the JAN-13 algorithm.
    /// </summary>
    protected override string Encode()
    {
        if (!RawData.StartsWith("49"))
        {
            Error("EJAN13-1: Invalid Country Code for JAN13 (49 required)");
        }

        if (!CheckNumericOnly(RawData))
        {
            Error("EJAN13-2: Numeric Data Only");
        }

        EAN13 ean13 = new EAN13(RawData);
        return ean13.EncodedValue;
    }
}
