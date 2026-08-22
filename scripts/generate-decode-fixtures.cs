#:project ../src/Genocs.QRCodeLibrary/Genocs.QRCodeLibrary.csproj

using Genocs.QRCodeLibrary.Encoder;
using SkiaSharp;

string outputDir = Path.GetFullPath(
    Path.Combine(
        Directory.GetCurrentDirectory(),
        "src",
        "tests",
        "Genocs.QRCodeLibrary.Tests",
        "DemoFiles"));

Directory.CreateDirectory(outputDir);

(string FileName, string Payload, QRCodeGenerator.ECCLevel Ecc, int Version, float Rotation, bool Dark)[] fixtures =
[
    ("decode-v1-ecc-l-rot0-light.png", "GNX-V1-L", QRCodeGenerator.ECCLevel.L, 1, 0f, false),
    ("decode-v2-ecc-m-rot0-light.png", "GNX-V2-M", QRCodeGenerator.ECCLevel.M, 2, 0f, false),
    ("decode-v3-ecc-q-rot0-light.png", "GNX-V3-Q", QRCodeGenerator.ECCLevel.Q, 3, 0f, false),
    ("decode-v4-ecc-h-rot0-light.png", "GNX-V4-H", QRCodeGenerator.ECCLevel.H, 4, 0f, false),
    ("decode-v5-ecc-m-rot0-light.png", "GNX-V5-M", QRCodeGenerator.ECCLevel.M, 5, 0f, false),
    ("decode-v6-ecc-l-rot0-light.png", "GNX-V6-L", QRCodeGenerator.ECCLevel.L, 6, 0f, false),
    ("decode-v7-ecc-q-rot15-light.png", "GNX-V7-Q-ROT", QRCodeGenerator.ECCLevel.Q, 7, 15f, false),
    ("decode-v3-ecc-m-rot0-dark.png", "GNX-V3-M-DARK", QRCodeGenerator.ECCLevel.M, 3, 0f, true),
];

foreach (var fixture in fixtures)
{
    byte[] png = Render(
        fixture.Payload,
        fixture.Ecc,
        fixture.Version,
        pixelsPerModule: 10,
        fixture.Rotation,
        fixture.Dark);
    string path = Path.Combine(outputDir, fixture.FileName);
    File.WriteAllBytes(path, png);
    Console.WriteLine($"Wrote {path} ({png.Length} bytes)");
}

static byte[] Render(
    string payload,
    QRCodeGenerator.ECCLevel eccLevel,
    int version,
    int pixelsPerModule,
    float rotationDegrees,
    bool dark)
{
    using var generator = new QRCodeGenerator();
    using QRCodeData data = generator.CreateQrCode(
        payload,
        eccLevel,
        requestedVersion: version);

    int modules = data.ModuleMatrix.Count;
    int qrSize = modules * pixelsPerModule;
    int margin = pixelsPerModule * 4;
    int contentSize = qrSize + (margin * 2);
    int canvasSize = rotationDegrees == 0
        ? contentSize
        : (int)Math.Ceiling(contentSize * Math.Sqrt(2.0)) + (pixelsPerModule * 4);

    SKColor moduleColor = dark ? new SKColor(30, 30, 30) : SKColors.Black;
    SKColor backgroundColor = dark ? new SKColor(200, 200, 200) : SKColors.White;

    using var bitmap = new SKBitmap(canvasSize, canvasSize, SKColorType.Bgra8888, SKAlphaType.Opaque);
    using var canvas = new SKCanvas(bitmap);
    canvas.Clear(backgroundColor);
    canvas.Translate(canvasSize / 2f, canvasSize / 2f);
    if (rotationDegrees != 0)
    {
        canvas.RotateDegrees(rotationDegrees);
    }

    canvas.Translate(-qrSize / 2f, -qrSize / 2f);

    using var paint = new SKPaint
    {
        Color = moduleColor,
        Style = SKPaintStyle.Fill,
        IsAntialias = false,
    };

    for (int y = 0; y < modules; y++)
    {
        for (int x = 0; x < modules; x++)
        {
            if (!data.ModuleMatrix[y][x])
            {
                continue;
            }

            canvas.DrawRect(
                SKRect.Create(x * pixelsPerModule, y * pixelsPerModule, pixelsPerModule, pixelsPerModule),
                paint);
        }
    }

    using SKImage image = SKImage.FromBitmap(bitmap);
    using SKData encoded = image.Encode(SKEncodedImageFormat.Png, 100);
    return encoded.ToArray();
}
