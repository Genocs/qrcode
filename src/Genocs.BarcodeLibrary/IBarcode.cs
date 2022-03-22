using System.Collections.Generic;

namespace Genocs.BarcodeLibrary
{
    /// <summary>
    ///  Barcode interface for symbology layout.
    ///  Written by: Brad Barnhill
    /// </summary>
    interface IBarcode
    {
        string Encoded_Value { get; }

        string RawData { get; }

        List<string> Errors { get; }
    }
}
