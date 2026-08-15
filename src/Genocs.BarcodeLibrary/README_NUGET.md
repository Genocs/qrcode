# Genocs.BarcodeLibrary

![Genocs Library Banner](https://raw.githubusercontent.com/Genocs/genocs-library/main/assets/genocs-library-banner.png)

Encode common 1D barcode symbologies to SkiaSharp images. Supports `net10.0`, `net9.0`, and `net8.0`. There is no barcode reader in this package.

## Installation

```bash
dotnet add package Genocs.BarcodeLibrary
```

## Getting Started

Use this package to rasterize Code 128, Code 39, EAN/UPC, ITF-14, Pharmacode, and other 1D types without `System.Drawing.Common`.

`Barcode.Encode` selects a symbology, encodes the payload to a bar/space pattern, then draws an `SKImage`. Set `IncludeLabel` when you want human-readable text under the bars.

This library is encode-only. Scanning or decoding 1D barcodes is not supported.

```csharp
using Genocs.BarcodeLibrary;
using SkiaSharp;

using var barcode = new Barcode { IncludeLabel = true };
using SKImage image = barcode.Encode(
    BarcodeType.UpcA,
    "038000356216",
    SKColors.Black,
    SKColors.White,
    width: 290,
    height: 120);

using var stream = File.OpenWrite("barcode.png");
image.Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream);
```

Static helpers such as `Barcode.DoEncode` wrap the same path for one-shot generation.

## Main Entry Points

- `Barcode`
- `Barcode.Encode`
- `Barcode.DoEncode`
- `BarcodeType`
- `Barcode.SaveImage` / `Barcode.GetImageData`

## Warning Policy

This package follows the quality gate in [build_and_test.yml](https://github.com/Genocs/qrcode/blob/main/.github/workflows/build_and_test.yml):

- Libraries must build for `net10.0`, `net9.0`, and `net8.0`.
- Unit tests must pass before merging barcode changes.

## Support

- Documentation Portal: https://genocs-blog.netlify.app/
- Documentation: https://github.com/Genocs/qrcode/tree/main/docs
- Repository: https://github.com/Genocs/qrcode

## Release Notes

- CHANGELOG: https://github.com/Genocs/qrcode/blob/main/CHANGELOG.md
- Releases: https://github.com/Genocs/qrcode/releases
