namespace Genocs.BarcodeLibrary.Symbologies
{
    /// <summary>
    ///  Postnet encoding
    ///  Written by: Brad Barnhill.
    /// </summary>
    class Postnet : BarcodeCommon, IBarcode
    {
        private readonly string[] POSTNET_Code = {
            "11000",
            "00011",
            "00101",
            "00110",
            "01001",
            "01010",
            "01100",
            "10001",
            "10010",
            "10100"
        };

        public Postnet(string input)
        {
            _rawData = input;
        }

        /// <summary>
        /// Encode the raw data using the PostNet algorithm.
        /// </summary>
        private string Encode_Postnet()
        {
            // remove dashes if present
            _rawData = RawData.Replace("-", string.Empty);

            switch (RawData.Length)
            {
                case 5:
                case 6:
                case 9:
                case 11: break;
                default:
                    Error("EPOSTNET-2: Invalid data length. (5, 6, 9, or 11 digits only)");
                    break;
            }

            // Note: 0 = half bar and 1 = full bar
            // initialize the result with the starting bar
            string result = "1";
            int checkDigitSum = 0;

            foreach (char c in RawData)
            {
                try
                {
                    int index = Convert.ToInt32(c.ToString());
                    result += POSTNET_Code[index];
                    checkDigitSum += index;
                }
                catch (Exception ex)
                {
                    Error("EPOSTNET-2: Invalid data. (Numeric only) --> " + ex.Message);
                }
            }

            // calculate and add check digit
            int temp = checkDigitSum % 10;
            int checkDigit = 10 - (temp == 0 ? 10 : temp);

            result += POSTNET_Code[checkDigit];

            // ending bar
            result += "1";

            return result;
        }

        public string EncodedValue => Encode_Postnet();
    }
}
