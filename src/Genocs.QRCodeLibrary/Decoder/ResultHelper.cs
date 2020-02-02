namespace Genocs.QRCodeLibrary.Decoder
{
    public static class ResultHelper
    {
        public static QrCodeResult ToResult(this QRDecoder decoder)
        {
            return new QrCodeResult();
        }
    }
}
