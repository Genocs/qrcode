using Genocs.BarcodeLibrary;
using System.Drawing;
using Xunit;

namespace Genocs.QRCodeLibrary.Tests
{
    public class BarCodeUnitTests
    {
        [Fact]
        public void LoadFilexUnitTest()
        {
            Barcode b = new();
            Image img = b.Encode(BarcodeLibrary.TYPE.UPCA, "038000356216", Color.Black, Color.White, 290, 120);
        }
    }
}
