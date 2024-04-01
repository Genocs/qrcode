using Genocs.QRCodeGenerator.Decoder;
using SkiaSharp;

namespace Genocs.QRCodeLibrary.Decoder.ConsoleApp;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Hello Genocs!");
        EncodeBarcode();

        DecodeQrCode();

        Console.WriteLine("Done!");
    }

    private static void DecodeQrCode()
    {
        try
        {
            var image = SKBitmap.Decode(File.ReadAllBytes("C:\\dev\\image4_out.jpg"));

            QRDecoder decoder = new QRDecoder();

            //var qrCode = decoder.ImageDecoder(image.);
        }
        catch (Exception)
        {
            // handle exception here;
        }
    }

    private static void EncodeBarcode()
    {
        try
        {
            BarcodeLibrary.Barcode barcode = new();
            var img = barcode.Encode(BarcodeLibrary.Type.UpcA, "038000356216", SKColors.Black, SKColors.White, 290, 120);

            //img.Save("C:\\dev\\image4_out.jpg");

        }
        catch (Exception)
        {
            // handle exception here;
        }
    }
}
