namespace Genocs.BarcodeLibrary.Symbologies;

/// <summary>
///  Blank encoding template
///  Written by: Brad Barnhill
/// </summary>
class Blank : BarcodeCommon, IBarcode
{
    public string EncodedValue
    {
        get { throw new NotImplementedException(); }
    }
}
