<!-- PROJECT SHIELDS -->
[![License][license-shield]][license-url]
[![Build][build-shield]][build-url]
[![DownloadsBarcode][downloads-br-shield]][downloads-br-url]
[![DownloadsQRCode][downloads-qr-shield]][downloads-qr-url]
[![Issues][issues-shield]][issues-url]
[![Discord][discord-shield]][discord-url]

[license-shield]: https://img.shields.io/github/license/Genocs/qrcode?color=2da44e&style=flat-square
[license-url]: https://github.com/Genocs/qrcode/blob/main/LICENSE
[build-shield]: https://github.com/Genocs/qrcode/actions/workflows/build_and_test.yml/badge.svg?branch=main
[build-url]: https://github.com/Genocs/qrcode/actions/workflows/build_and_test.yml
[downloads-br-shield]: https://img.shields.io/nuget/dt/Genocs.BarcodeLibrary.svg?color=2da44e&label=downloads_barcode&logo=nuget
[downloads-br-url]: https://www.nuget.org/packages/Genocs.BarcodeLibrary
[downloads-qr-shield]: https://img.shields.io/nuget/dt/Genocs.QRCodeLibrary.svg?color=2da44e&label=downloads_qrcode&logo=nuget
[downloads-qr-url]: https://www.nuget.org/packages/Genocs.QRCodeLibrary
[issues-shield]: https://img.shields.io/github/issues/Genocs/qrcode?style=flat-square
[issues-url]: https://github.com/Genocs/qrcode/issues
[discord-shield]: https://img.shields.io/discord/1106846706512953385?color=%237289da&label=Discord&logo=discord&logoColor=%237289da&style=flat-square
[discord-url]: https://discord.com/invite/fWwArnkV

# Genocs QR code and barcode libraries

.NET libraries to **encode QR codes** and **1D barcodes** as images, without a dependency on `System.Drawing.Common`, so the same code can run on Windows, Linux, and Linux containers.

The work is a SkiaSharp-based port of existing open-source projects ([QRCoder](https://github.com/codebude/QRCoder), [BarcodeLib](https://github.com/barnhill/barcodelib), and a [CodeProject QR decoder](https://www.codeproject.com/Articles/1250071/QR-Code-Encoder-and-Decoder-NET-Framework-Standard/)). Target frameworks are **.NET 8, 9, and 10**.

> [!IMPORTANT]
> **Preview / experimental — not ready for general production use.**
> Published NuGet packages (`5.0.0`) are unsupported snapshots. Do not use this stack for customer-facing tickets, payments, or a public HTTP API until the [production-readiness](docs/production-readiness.md) blockers are closed. Prefer [QRCoder](https://github.com/codebude/QRCoder), [ZXing.Net](https://github.com/micjahn/ZXing.Net), or [BarcodeLib](https://github.com/barnhill/barcodelib) if you need production codes today.

## Aim

Provide a **single Genocs surface** for:

| Package | Intended capability |
| --- | --- |
| [`Genocs.QRCodeLibrary`](https://www.nuget.org/packages/Genocs.QRCodeLibrary) | Build QR payloads and rasterize them (PNG / SVG / PostScript). Optionally decode QR images. |
| [`Genocs.BarcodeLibrary`](https://www.nuget.org/packages/Genocs.BarcodeLibrary) | Encode common 1D symbologies (Code 128, Code 39, EAN/UPC, ITF-14, Pharmacode, and others) to a SkiaSharp image. Encode-only — there is no barcode reader. |

The repository also includes a **demo Web API** and a console scratchpad. Those hosts are samples, not a supported service.

## Current status

Assessed 16 August 2026. Full write-up: [docs/](docs/README.md).

| Area | Status |
| --- | --- |
| QR PNG encode | **Experimental.** Renderer exists; antialiasing and logo overlay are incorrect. No scanner round-trip tests. |
| QR SVG / PostScript | **SVG encode works** for `GetGraphic(int)` / hex colors. PostScript is still unfinished. |
| QR decode | **Experimental.** Fixture tests pass for crisp PNGs (v1–v7, L/M/Q/H, 15° rotation, reduced contrast). Camera JPEGs are still unproven. |
| Structured payloads (Wi-Fi, Swiss QR, Girocode, vCard, …) | **Best-tested path** — string generation only, not a scannable image. |
| 1D barcode encode | **Experimental.** Encode-string and PNG tests cover every `BarcodeType`. Not yet proven against a hardware scanner. |
| Demo Web API | **Local demo only.** No auth, no upload limits, errors often returned as HTTP 200. |
| NuGet 5.0.0 | **Do not treat as a supported product.** Package READMEs and changelog are stale. |

Go / no-go by use case is in [docs/production-readiness.md](docs/production-readiness.md). The catch-up plan is [docs/roadmap.md](docs/roadmap.md).

## Install (preview)

```bash
dotnet add package Genocs.QRCodeLibrary
dotnet add package Genocs.BarcodeLibrary
```

Pin an exact version. Do not use floating `5.0.*` in production builds.

## Usage (illustrative)

These snippets match the current public API. They are **not** a guarantee that the output will scan until the P0 work in the roadmap is done.

QR encode (PNG):

```csharp
using Genocs.QRCodeLibrary.Encoder;
using SkiaSharp;

using var generator = new QRCodeGenerator();
using var data = generator.CreateQrCode("Hello, World!", QRCodeGenerator.ECCLevel.M);
using var qrCode = new QRCode(data);
using var image = qrCode.GetGraphic(20, SKColors.Black, SKColors.White, drawQuietZones: true);

using var stream = File.OpenWrite("qrcode.png");
image.Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream);
```

Barcode encode (UPC-A):

```csharp
using Genocs.BarcodeLibrary;
using SkiaSharp;

using var barcode = new Barcode { IncludeLabel = true };
using var image = barcode.Encode(BarcodeType.UpcA, "038000356216", SKColors.Black, SKColors.White, 290, 120);

using var stream = File.OpenWrite("barcode.png");
image.Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream);
```

Avoid the QR `GetGraphic` overload that takes an icon/logo. `SvgQRCode` is usable for basic PNG-equivalent SVG output.

## Build and test

Requires the .NET 10 SDK (`global.json`). The libraries also compile for `net8.0` and `net9.0`.

```bash
dotnet build
dotnet test
```

CI currently installs **.NET 8 only**; that gap is tracked in the [findings](docs/findings.md) (F-06).

### Demo host (optional)

The Web API under `src/WebApi` is a local demo (`GET /BuildQrCode`, `GET /BuildBarcode`, `POST /FindQrCode`). Do not expose it on the public internet.

```bash
./scripts/run-on-docker.sh
```

## Documentation

| Document | Purpose |
| --- | --- |
| [Production readiness](docs/production-readiness.md) | Verdict, maturity scores, go / no-go |
| [Findings](docs/findings.md) | Issue catalog (F-01 … F-23) |
| [Roadmap](docs/roadmap.md) | P0–P3 plan to a supportable release |
| [Architecture](docs/architecture.md) | Layout, origins, intended vs actual capabilities |

## License

This repository is published under the [MIT license](LICENSE). NuGet packages pack that same file (`PackageLicenseFile`).

Upstream code is not all MIT:

- 1D barcodes are a port of [BarcodeLib](https://github.com/barnhill/barcodelib) (**Apache 2.0**). The Apache license copy previously at `src/Genocs.BarcodeLibrary/LICENSE.txt` is no longer in the tree.
- The QR decoder originates from a CodeProject article (typically **CPOL**). There is no CPOL file in the repo.

A single MIT file does not satisfy those upstream terms. That remains a shipping blocker: [F-04](docs/findings.md).

## Acknowledgements

- QR encoder and payload helpers: [QRCoder](https://github.com/codebude/QRCoder) by Raffael Herrmann
- 1D barcode symbologies: [BarcodeLib](https://github.com/barnhill/barcodelib) by Brad Barnhill
- QR decoder: [Uzi Granot on CodeProject](https://www.codeproject.com/Articles/1250071/QR-Code-Encoder-and-Decoder-NET-Framework-Standard/)

## Community

- Discord [@genocs](https://discord.com/invite/fWwArnkV)
- Issues: [github.com/Genocs/qrcode/issues](https://github.com/Genocs/qrcode/issues)

## Support

If this project helped you:

- Star the repository
- Open issues with scanner samples (image + expected payload)
- [Buy me a coffee](https://www.buymeacoffee.com/genocs)

[![buy-me-a-coffee](https://raw.githubusercontent.com/Genocs/qrcode/main/assets/buy-me-a-coffee.png "buy-me-a-coffee")](https://www.buymeacoffee.com/genocs)
