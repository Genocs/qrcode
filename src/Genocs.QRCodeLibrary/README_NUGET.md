# Genocs.QRCodeLibrary

![Genocs Library Banner](https://raw.githubusercontent.com/Genocs/genocs-library/main/assets/genocs-library-banner.png)

Encode QR codes as SkiaSharp images, SVG, PNG bytes, and related formats. Optional image decode is included. Supports `net10.0`, `net9.0`, and `net8.0`.

## Installation

```bash
dotnet add package Genocs.QRCodeLibrary
```

## Getting Started

Use this package to build a QR module matrix with `QRCodeGenerator`, then render it with a format-specific class (`QRCode`, `SvgQRCode`, `PngByteQRCode`, and others). Payload helpers under `PayloadGenerator` produce structured strings (Wi-Fi, vCard, Girocode, and more) that you pass into `CreateQrCode`.

Rendering uses SkiaSharp and does not depend on `System.Drawing.Common`.

```csharp
using Genocs.QRCodeLibrary.Encoder;
using SkiaSharp;

using var generator = new QRCodeGenerator();
using QRCodeData data = generator.CreateQrCode("Hello, World!", QRCodeGenerator.ECCLevel.M);
using var qrCode = new QRCode(data);
using SKImage image = qrCode.GetGraphic(20, SKColors.Black, SKColors.White, drawQuietZones: true);

using var stream = File.OpenWrite("qrcode.png");
image.Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream);
```

To read a QR image, use `QRDecoder.ImageDecoder(SKImage)`. Decode is experimental: crisp generated PNGs are covered by tests; camera-style JPEGs are not a supported path yet.

Avoid the `GetGraphic` overload that takes an icon or logo overlay.

## Main Entry Points

- `QRCodeGenerator`
- `QRCode` / `QRCode.GetGraphic`
- `SvgQRCode`
- `PngByteQRCode` / `BitmapByteQRCode` / `Base64QRCode`
- `PayloadGenerator`
- `QRDecoder.ImageDecoder`

## Warning Policy

This package follows the quality gate in [build_and_test.yml](https://github.com/Genocs/qrcode/blob/main/.github/workflows/build_and_test.yml):

- Libraries must build for `net10.0`, `net9.0`, and `net8.0`.
- Unit tests must pass before merging QR encode or decode changes.

## Support

- Documentation Portal: https://genocs-blog.netlify.app/
- Documentation: https://github.com/Genocs/qrcode/tree/main/docs
- Repository: https://github.com/Genocs/qrcode

## Release Notes

- CHANGELOG: https://github.com/Genocs/qrcode/blob/main/CHANGELOG.md
- Releases: https://github.com/Genocs/qrcode/releases
