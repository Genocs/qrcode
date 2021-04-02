﻿using System;
using System.Drawing;

namespace Genocs.QRCodeLibrary.Decoder.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Genocs!");
            EncodeBarcode();

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


        private static void EncodeBarcode()
        {
            try
            {
                BarcodeLibrary.Barcode b = new();
                Image img = b.Encode(BarcodeLibrary.TYPE.UPCA, "038000356216", Color.Black, Color.White, 290, 120);

                img.Save("C:\\dev\\image4_out.jpg");

            }
            catch (Exception)
            {
                // handle exception here;
            }
        }
    }
}
