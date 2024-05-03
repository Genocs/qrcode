namespace Genocs.BarcodeLibrary.Symbologies;

/// <summary>
///  Blank encoding template
///  Written by: Brad Barnhill.
/// </summary>
internal class Blank : BarcodeCommon, IBarcode
{
    public string EncodedValue
    {
        get { throw new NotImplementedException(); }
    }
}
