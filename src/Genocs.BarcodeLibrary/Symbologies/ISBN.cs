namespace Genocs.BarcodeLibrary.Symbologies
{
    /// <summary>
    ///  ISBN encoding
    ///  Written by: Brad Barnhill
    /// </summary>
    class ISBN : BarcodeCommon, IBarcode
    {
        public ISBN(string input)
        {
            _RawData = input;
        }
        /// <summary>
        /// Encode the raw data using the Bookland/ISBN algorithm.
        /// </summary>
        private string Encode_ISBN_Bookland()
        {
            if (!CheckNumericOnly(RawData))
                Error("EBOOKLANDISBN-1: Numeric Data Only");

            var type = "UNKNOWN";
            switch (RawData.Length)
            {
                case 10:
                case 9:
                    {
                        if (RawData.Length == 10) _RawData = RawData.Remove(9, 1);
                        _RawData = "978" + RawData;
                        type = "ISBN"; 
                        break;
                    }
                case 12 when RawData.StartsWith("978"):
                    type = "BOOKLAND-NOCHECKDIGIT"; //else if
                    break;
                case 13 when RawData.StartsWith("978"):
                    type = "BOOKLAND-CHECKDIGIT";
                    _RawData = RawData.Remove(12, 1); //else if
                    break;
            }

            //check to see if its an unknown type
            if (type == "UNKNOWN") Error("EBOOKLANDISBN-2: Invalid input.  Must start with 978 and be length must be 9, 10, 12, 13 characters.");

            var ean13 = new EAN13(RawData);
            return ean13.EncodedValue;
        }

        public string EncodedValue => Encode_ISBN_Bookland();
    }
}
