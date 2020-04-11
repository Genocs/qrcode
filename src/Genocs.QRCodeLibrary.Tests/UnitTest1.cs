using Genocs.QRCodeLibrary.Decoder;
using System.Drawing;
using Xunit;

namespace Genocs.QRCodeLibrary.Tests
{
    public class QrCodeUnitTest
    {
        [Fact]
        public void LoadFilexUnitTest()
        {
            string demoFilePath = HelperUnitTest.GetDemoFile("image2.jpg");

            using (Bitmap bitmap = new Bitmap(demoFilePath))
            {
                QRDecoder decoder = new QRDecoder();
                var qrCode = decoder.ImageDecoder(bitmap);
                Assert.NotNull(qrCode);
                Assert.NotNull(qrCode.Results);
                Assert.Single(qrCode.Results);
            }
        }
    }
}
