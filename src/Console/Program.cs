using Genocs.QRCodeLibrary.Decoder;
using Genocs.QRCodeLibrary.Encoder;
using SkiaSharp;

Console.WriteLine("Hello Genocs!");
EncodeBarcode();
EncodeQrCode();

DecodeQrCode();

Console.WriteLine("Done!");
static void DecodeQrCode()
{
    try
    {
        var image = SKBitmap.Decode(File.ReadAllBytes("C:\\dev\\image4_out.jpg"));

        SKImage sKImage = SKImage.FromBitmap(image);

        QRDecoder decoder = new QRDecoder();

        var qrCode = decoder.ImageDecoder(sKImage);
    }
    catch (Exception)
    {
        // handle exception here;
    }
}

static void EncodeBarcode()
{
    try
    {
        Genocs.BarcodeLibrary.Barcode barcode = new();
        var img = barcode.Encode(Genocs.BarcodeLibrary.BarcodeType.UpcA, "038000356216", SKColors.Black, SKColors.White, 290, 120);

        using FileStream fileStream = new FileStream(@".\image4_out.jpg", FileMode.Create, FileAccess.Write);
        img.Encode(SKEncodedImageFormat.Png, 100).SaveTo(fileStream);

    }
    catch (Exception)
    {
        // handle exception here;
    }
}

static void EncodeQrCode()
{
    try
    {
        QRCodeGenerator qrCodeGenerator = new();

        var qrcodeData = qrCodeGenerator.CreateQrCode("Hello, World!", QRCodeGenerator.ECCLevel.M);

        QRCode qrCode = new(qrcodeData);

        var image = qrCode.GetGraphic(40, SKColors.Black, SKColors.White, true);

        using FileStream fileStream = new FileStream(@".\image5_out.jpg", FileMode.Create, FileAccess.Write);
        image.Encode(SKEncodedImageFormat.Png, 100).SaveTo(fileStream);

    }
    catch (Exception)
    {
        // handle exception here;
    }
}
