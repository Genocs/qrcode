using BarcodeLib.Symbologies;

namespace Genocs.BarcodeLibrary.Symbologies;

/// <summary>
///  JAN-13 encoding
///  Written by: Brad Barnhill
/// </summary>
class JAN13 : BarcodeCommon, IBarcode
{
    public JAN13(string input)
    {
        _rawData = input;
    }

    /// <summary>
    /// Encode the raw data using the JAN-13 algorithm.
    /// </summary>
    private string Encode_JAN13()
    {
        if (!RawData.StartsWith("49")) Error("EJAN13-1: Invalid Country Code for JAN13 (49 required)");
        if (!CheckNumericOnly(RawData))
            Error("EJAN13-2: Numeric Data Only");

        EAN13 ean13 = new EAN13(RawData);
        return ean13.EncodedValue;
    }

    public string EncodedValue => Encode_JAN13();
}
