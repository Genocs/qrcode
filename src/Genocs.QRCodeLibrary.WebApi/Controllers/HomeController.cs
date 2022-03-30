using Genocs.QRCodeLibrary.Decoder;
using Genocs.QRCodeLibrary.Encoder;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;

namespace Genocs.QRCodeLibrary.WebApi.Controllers
{

    [Route("")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
            => _logger = logger;

        /// <summary>
        /// It allows to upload a file containing a QRCode
        /// </summary>
        /// <param name="file">file</param>
        /// <returns>QrCode scanning result</returns>
        [Route("FindQrCode"), HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PostFindQrCode(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return Content("File not provided or empty");

                QrCodeResult result = null;

                using (MemoryStream memory = new MemoryStream())
                {
                    await file.CopyToAsync(memory);

                    Image image = Image.Load(memory);

                    QRDecoder decoder = new QRDecoder();
                    result = decoder.ImageDecoder(image);
                }

                // process uploaded files
                // Don't rely on or trust the FileName property without validation.
                //Displaying File Name for verification purposes for now -Rohit
                if (result != null)
                {
                    return Ok(result);
                }

                return Ok(new { file.Length, file.FileName, message = "QRCode not find" });
            }
            catch (Exception exp)
            {
                string message = $"Error on processing file. Message: '{exp.Message}'!";
                return Ok(message);
            }
        }

        /// <summary>
        /// It allows to build an image containing a QRCode
        /// </summary>
        /// <param name="payload">The QRCode payload data</param>
        /// <param name="size">The QRCode size</param>
        /// <returns>QRCode image result</returns>
        [Route("BuildQrCode"), HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetBuildQrCode([FromQuery] string payload, int size = 20)
        {
            try
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();

                QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                using Image image = qrCode.GetGraphic(size);
                using MemoryStream ms = new MemoryStream();
                image.Save(ms, new PngEncoder());
                return File(ms.ToArray(), "image/png");

            }
            catch (Exception exp)
            {
                string message = $"Error on processing file. Message: '{exp.Message}'!";
                return Ok(message);
            }
        }

        /// <summary>
        /// It allows to build an image containing a Barcode
        /// </summary>
        /// <param name="payload">The Barcode payload data</param>
        /// <param name="width">The Barcode width</param>
        /// <param name="height">The Barcode height</param>

        /// <returns>Barcode image result</returns>
        [Route("BuildBarcode"), HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetBuildBarCode([FromQuery] BarcodeLibrary.TYPE barcodeType = BarcodeLibrary.TYPE.UPCA, string payload = "038000356216", int width = 290, int height = 120)
        {
            try
            {
                BarcodeLibrary.Barcode barcodeGenerator = new BarcodeLibrary.Barcode();

                Image img = barcodeGenerator.Encode(barcodeType, payload, width, height);

                using MemoryStream ms = new MemoryStream();
                img.Save(ms, new PngEncoder());
                return File(ms.ToArray(), "image/png");

            }
            catch (Exception exp)
            {
                string message = $"Error on processing file. Message: '{exp.Message}'!";
                return Ok(message);
            }
        }
    }
}
