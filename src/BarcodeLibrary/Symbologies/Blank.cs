namespace Genocs.BarcodeLibrary.Symbologies;

/// <summary>
/// Blank encoding template.
/// </summary>
internal class Blank : BarcodeEncoding, IBarcode
{
    protected override string Encode()
    {
        throw new NotImplementedException();
    }
}
