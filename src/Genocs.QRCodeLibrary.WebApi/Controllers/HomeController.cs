using Aspose.BarCode.BarCodeRecognition;
using Genocs.QRCodeLibrary.Decoder;
using Genocs.QRCodeLibrary.Encoder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace Genocs.QRCodeLibrary.WebApi.Controllers
{

    [ApiController]
    [Route("")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
            => _logger = logger;

        [HttpGet]
        public IActionResult Get()
            => Ok("QRCode Web API Service");

        [HttpGet("ping")]
        public IActionResult GetPing()
            => Ok("pong");

        /// <summary>
        /// It allows to upload a file containing a QRCode
        /// </summary>
        /// <param name="files">files</param>
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

                    using (Bitmap bitmap = new Bitmap(memory))
                    {
                        QRDecoder decoder = new QRDecoder();
                        result = decoder.ImageDecoder(bitmap);
                    }
                }

                // process uploaded files
                // Don't rely on or trust the FileName property without validation.
                //Displaying File Name for verification purposes for now -Rohit
                if (result != null)
                {
                    return Ok(result);
                }

                using (MemoryStream memory = new MemoryStream())
                {
                    using (BarCodeReader reader = new BarCodeReader(memory))
                    {
                        result = new QrCodeResult();
                        var res = reader.ReadBarCodes();
                        if (res != null && res.Length > 0)
                        {
                            result.Results.Add(res[0].CodeText);
                        }
                    }
                }

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
        /// It allows to upload a file containing a QRCode
        /// </summary>
        /// <param name="files">files</param>
        /// <returns>QrCode scanning result</returns>
        [Route("BuildQrCode"), HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetBuildQrCode([FromQuery] string payload)
        {
            try
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                using (Bitmap qrCodeImage = qrCode.GetGraphic(20))
                {
                    return File(ImageToByteArray(qrCodeImage), "image/png");
                }
            }
            catch (Exception exp)
            {
                string message = $"Error on processing file. Message: '{exp.Message}'!";
                return Ok(message);
            }
        }

        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();
        }
    }
}
