using Genocs.BarcodeLibrary;
using Genocs.BarcodeLibrary.WebApi.Exceptions;
using Genocs.BarcodeLibrary.WebApi.OpenApi;
using Genocs.Core.Builders;
using Genocs.Logging;
using Genocs.QRCodeLibrary.Decoder;
using Genocs.QRCodeLibrary.Encoder;
using Genocs.WebApi;
using Genocs.WebApi.OpenApi;
using Serilog;
using SkiaSharp;

StaticLogger.EnsureInitialized();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseLogging();

IGenocsBuilder genocs = builder
    .AddGenocs()
    .AddErrorHandler<ExceptionToResponseMapper>()
    .AddWebApi()
    .AddOpenApiDocs();

// add services to DI container
var services = builder.Services;

services.ConfigureSwaggerGen(options =>
    options.DocumentFilter<EndpointDescriptionsDocumentFilter>());

var app = builder.Build();
genocs.Build(app.Services);

app.UseGenocs();
app.UseErrorHandler();
app.UseOpenApiDocs();

app.UseEndpoints(static endpoints =>
{
    endpoints.Get(
        "/",
        async context =>
            await context.Response.Ok("Welcome to Genocs QRCode Library WebApi"),
        endpoint: route => route
            .WithSummary("Home")
            .WithDescription("Returns the welcome message for the QRCode Web API.")
            .WithTags("System"));

    endpoints.Get(
        "health",
        async context =>
            await context.Response.Ok(new { status = "ok", service = "Genocs.QRCodeLibrary.WebApi" }),
        endpoint: route => route
            .WithSummary("Health check")
            .WithDescription(
                "Returns a lightweight liveness payload for the Genocs.QRCodeLibrary.WebApi host. " +
                "Use this endpoint for container/orchestrator probes and basic connectivity checks.")
            .WithTags("System"));
});

app.MapPost(
    "/FindQrCode",
    async Task<IResult> (IFormFile? file) =>
    {
        try
        {
            if (file == null || file.Length == 0)
                return Results.Content("File not provided or empty");

            QrCodeResult? result = null;

            await using (MemoryStream memory = new MemoryStream())
            {
                await file.CopyToAsync(memory);

                SKImage image = SKImage.FromEncodedData(memory);

                QRDecoder decoder = new QRDecoder();
                result = decoder.ImageDecoder(image);
            }

            if (result != null)
            {
                return Results.Ok(result);
            }

            return Results.Ok(new { file.Length, file.FileName, message = "QRCode not find" });
        }
        catch (Exception exp)
        {
            string message = $"Error on processing file. Message: '{exp.Message}'!";
            return Results.Ok(message);
        }
    })
    .WithSummary("Find QRCode")
    .WithDescription("Allows uploading a file containing a QRCode and returns the decoded result.")
    .WithTags("QRCode");

app.MapGet(
    "/BuildQrCode",
    (string payload, int size = 16) =>
    {
        try
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            using var image = qrCode.GetGraphic(size);
            return Results.File(image.Encode().AsStream(), "image/png");
        }
        catch (Exception exp)
        {
            string message = $"Error on processing file. Message: '{exp.Message}'!";
            return Results.Ok(message);
        }
    })
    .WithSummary("Build QRCode")
    .WithDescription("Builds a PNG image containing a QRCode from the supplied payload.")
    .WithTags("QRCode");

app.MapGet(
    "/BuildBarcode",
    (BarcodeType barcodeType = BarcodeType.UpcA, string payload = "038000356216", int width = 290, int height = 120) =>
    {
        try
        {
            Barcode barcodeGenerator = new Barcode
            {
                IncludeLabel = true,
                LabelFont = new SKFont(SKTypeface.FromFamilyName("Arial"), height / 10f)
            };
            var img = barcodeGenerator.Encode(barcodeType, payload, width, height);

            return Results.File(img.Encode().AsStream(), "image/png");
        }
        catch (Exception exp)
        {
            string message = $"Error on processing file. Message: '{exp.Message}'!";
            return Results.Ok(message);
        }
    })
    .WithSummary("Build Barcode")
    .WithDescription("Builds a PNG image containing a barcode from the supplied payload.")
    .WithTags("Barcode");

app.Run();

Log.CloseAndFlush();