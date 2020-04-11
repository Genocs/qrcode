using Aspose.BarCode.BarCodeRecognition;
using Aspose.Pdf;
using Aspose.Pdf.Devices;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Genocs.QRCodeLibrary.Decoder.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Genocs!");
            // load the PDF in PdfConverter

            //test2();
            //test3();
            test6();
            //read();
            Console.WriteLine("Done!");
        }

        private static void Test2()
        {
            // load the PDF in PdfConverter
            using (var converter = new Aspose.Pdf.Facades.PdfConverter())
            {
                converter.BindPdf("template.pdf");
                // initiate conversion
                converter.DoConvert();
                // create TiffSettings & set compression type
                var settings = new Aspose.Pdf.Devices.TiffSettings()
                {
                    Compression = Aspose.Pdf.Devices.CompressionType.None,
                };
                // save PDF as TIFF
                converter.SaveAsTIFF("output.tiff", settings);
            }
        }

        private static void test3()
        {
            // Open document
            Document pdfDocument = new Document(@"C:\Users\Giovanni\OneDrive\UTU\docs\DI038452553.pdf");

            for (int pageCount = 1; pageCount <= pdfDocument.Pages.Count; pageCount++)
            {
                using (FileStream imageStream = new FileStream(@"C:\Users\Giovanni\OneDrive\UTU\docs\DI038452553.jpg", FileMode.Create))
                {
                    // Create JPEG device with specified attributes
                    // Width, Height, Resolution, Quality
                    // Quality [0-100], 100 is Maximum
                    // Create Resolution object
                    Resolution resolution = new Resolution(300);

                    // JpegDevice jpegDevice = new JpegDevice(500, 700, resolution, 100);
                    JpegDevice jpegDevice = new JpegDevice(resolution, 100);

                    // Convert a particular page and save the image to stream
                    jpegDevice.Process(pdfDocument.Pages[pageCount], imageStream);

                    // Close stream
                    imageStream.Close();

                }
            }
        }

        private static void test4()
        {
            // Open document
            SautinSoft.PdfFocus f = new SautinSoft.PdfFocus();
            f.OpenPdf(System.IO.File.ReadAllBytes("template.pdf"));
            //this property is necessary only for registered version
            //f.Serial = "XXXXXXXXXXX";


            if (f.PageCount > 0)
            {
                //set image properties
                f.ImageOptions.ImageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
                f.ImageOptions.Dpi = 300;

                //Let's convert 1st page from PDF document
                byte[] image = f.ToImage(1);

                f.ToImage("image2_out.jpg", 1);

            }
        }


        private static void test5()
        {
            try
            {
                using (var document = PdfiumViewer.PdfDocument.Load(@"template.pdf"))
                {
                    var image = document.Render(0, 300, 300, true);
                    image.Save(@"output.png", ImageFormat.Png);
                }
            }
            catch (Exception ex)
            {
                // handle exception here;
            }
        }

        private static void test6()
        {
            try
            {
                using (Bitmap bitmap = new Bitmap("C:\\dev\\image4_out.jpg"))
                {
                    QRDecoder decoder = new QRDecoder();
                    var qrCode = decoder.ImageDecoder(bitmap);
                    int i = 0;
                }
            }
            catch (Exception ex)
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
