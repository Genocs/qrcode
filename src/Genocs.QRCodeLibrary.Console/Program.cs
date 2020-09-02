using Aspose.BarCode.BarCodeRecognition;
using System;
using System.Drawing;

namespace Genocs.QRCodeLibrary.Decoder.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Genocs!");

            DecodeQrCode();
            //read();
            Console.WriteLine("Done!");
        }




        private static void DecodeQrCode()
        {
            try
            {
                using (Bitmap bitmap = new Bitmap("C:\\dev\\image4_out.jpg"))
                {
                    QRDecoder decoder = new QRDecoder();
                    var qrCode = decoder.ImageDecoder(bitmap);
   
                }
            }
            catch (Exception)
            {
                // handle exception here;
            }
        }


        private static void read()
        {
            using (BarCodeReader reader = new BarCodeReader("image1_out.jpg"))
            {
                var result = reader.ReadBarCodes();
            }
        }
    }
}
