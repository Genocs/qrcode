﻿using Genocs.QRCodeGenerator.Decoder;
using Genocs.QRCodeGenerator.Encoder;
using Microsoft.AspNetCore.Mvc;
using SkiaSharp;

namespace Genocs.QRCodeLibrary.WebApi.Controllers;

[Route("")]
[ApiController]
public class HomeController : ControllerBase
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
        => _logger = logger;

    [Route("")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHomeAsync()
    {
        return await Task.Run(() => Ok("Welcome to Genocs QRCode Library WebApi"));
    }

    /// <summary>
    /// It allows to upload a file containing a QRCode.
    /// </summary>
    /// <param name="file">file.</param>
    /// <returns>QrCode scanning result.</returns>
    [Route("FindQrCode")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> PostFindQrCode(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return Content("File not provided or empty");

            QrCodeResult? result = null;

            using (MemoryStream memory = new MemoryStream())
            {
                await file.CopyToAsync(memory);

                SKImage image = SKImage.FromEncodedData(memory);

                QRDecoder decoder = new QRDecoder();
                result = decoder.ImageDecoder(image);
            }

            // process uploaded files
            // Don't rely on or trust the FileName property without validation.
            // Displaying File Name for verification purposes for now -Rohit
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
    /// It allows to build an image containing a QRCode.
    /// </summary>
    /// <param name="payload">The QRCode payload data.</param>
    /// <param name="size">The QRCode size.</param>
    /// <returns>QRCode image result.</returns>
    [Route("BuildQrCode")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetBuildQrCode([FromQuery] string payload, int size = 16)
    {
        try
        {
            QRCodeGenerator.Encoder.QRCodeGenerator qrGenerator = new QRCodeGenerator.Encoder.QRCodeGenerator();

            QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.Encoder.QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            using var image = qrCode.GetGraphic(size);
            return File(image.Encode().AsStream(), "image/png");
        }
        catch (Exception exp)
        {
            string message = $"Error on processing file. Message: '{exp.Message}'!";
            return Ok(message);
        }
    }

    /// <summary>
    /// It allows to build an image containing a Barcode.
    /// </summary>
    /// <param name="barcodeType">The Barcode type <see cref="BarcodeLibrary.BarcodeType"/>.</param>
    /// <param name="payload">The Barcode payload data.</param>
    /// <param name="width">The Barcode width.</param>
    /// <param name="height">The Barcode height.</param>
    /// <returns>Barcode image result as image PNG.</returns>
    [Route("BuildBarcode")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public IActionResult GetBuildBarCode([FromQuery] BarcodeLibrary.BarcodeType barcodeType = BarcodeLibrary.BarcodeType.UpcA, string payload = "038000356216", int width = 290, int height = 120)
    {
        try
        {
            BarcodeLibrary.Barcode barcodeGenerator = new BarcodeLibrary.Barcode();
            barcodeGenerator.IncludeLabel = true;
            barcodeGenerator.LabelFont = new SKFont(SKTypeface.FromFamilyName("Arial"), height / 10f);
            var img = barcodeGenerator.Encode(barcodeType, payload, width, height);

            return File(img.Encode().AsStream(), "image/png");
        }
        catch (Exception exp)
        {
            string message = $"Error on processing file. Message: '{exp.Message}'!";
            return Ok(message);
        }
    }
}
