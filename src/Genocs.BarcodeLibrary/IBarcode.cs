namespace Genocs.BarcodeLibrary;

/// <summary>
///  Barcode interface for symbology layout.
/// </summary>
internal interface IBarcode
{
    string EncodedValue { get; }

    string RawData { get; }

    List<string> Errors { get; }
}
