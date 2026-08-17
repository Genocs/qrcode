# Architecture snapshot

This note describes the repository **as it exists today**, not the target architecture. Use it as context for the [production readiness](production-readiness.md) report and the [roadmap](roadmap.md).

## Products in the repository

The solution `qrcode.slnx` contains four applications of very different maturity:

| Artifact | Role | Intended consumers |
| --- | --- | --- |
| `Genocs.QRCodeLibrary` | NuGet library: QR encode, QR decode, payload helpers | .NET 8 / 9 / 10 applications |
| `Genocs.BarcodeLibrary` | NuGet library: 1D barcode encode + rasterize | .NET 8 / 9 / 10 applications |
| `src/WebApi` (`Host.csproj`) | Demo HTTP surface over both libraries | Docker / local demos |
| `src/Console` | Scratch encode/decode sample | Maintainers only |

The README positions the libraries as **OS-agnostic**, **Linux-Docker friendly**, and **free of `System.Drawing.Common`**.

## Provenance

Neither library is an original implementation. They are ports of well-known open-source projects, with a migration from `System.Drawing` to `SkiaSharp`.

```text
QRCoder (codebude)          Uzi Granot QR decoder (CodeProject)
        \                              /
         \                            /
          +---- Genocs.QRCodeLibrary ----+
                      Encoder / Decoder

BarcodeLib (Brad Barnhill, Apache-2.0)
          \
           +---- Genocs.BarcodeLibrary
                      Encode + SkiaSharp raster
```

| Area | Origin | Original license (typical) | What the repo ships today |
| --- | --- | --- | --- |
| QR encoder, payloads, SVG/PNG/PS renderers | [QRCoder](https://github.com/codebude/QRCoder) | MIT | MIT (`LICENSE`) — compatible |
| QR decoder | [Uzi Granot, CodeProject](https://www.codeproject.com/Articles/1250071/QR-Code-Encoder-and-Decoder-NET-Framework-Standard/) | CPOL (Code Project Open License) | MIT file only; no CPOL text packed |
| Barcode symbologies + drawing | [BarcodeLib](https://github.com/barnhill/barcodelib) | Apache 2.0 | MIT file only; Apache `LICENSE.txt` was removed from `src/Genocs.BarcodeLibrary/` |

The mixed provenance is a **compliance and attribution** concern, not just a documentation gap. A top-level MIT file does not satisfy Apache-2.0 or CPOL terms. Deleting the Apache copy does not relicense that code; see [F-04](findings.md).

## QR code library

### Encoder

`Genocs.QRCodeLibrary.Encoder` follows the QRCoder split:

1. `QRCodeGenerator` produces `QRCodeData` (module matrix).
2. Renderer classes consume that matrix:
   - `QRCode` — SkiaSharp `SKImage` (PNG via encode)
   - `PngByteQRCode` / `BitmapByteQRCode` — raw bytes
   - `Base64QRCode` — base64 wrapper
   - `SvgQRCode` — SVG string
   - `PostscriptQRCode` — PostScript / EPS
3. `PayloadGenerator` and `Encoder/Payloads/*` build structured payloads (Wi-Fi, Girocode, Swiss QR, Bitcoin, vCard, OTP, and others).

Payload helpers are the **best-tested** part of the repository. Image generation and round-trip scanning are not.

### Decoder

`Genocs.QRCodeLibrary.Decoder.QRDecoder` is a camera-style decoder: finders, alignment, Reed–Solomon, ECI. The public entry point is `ImageDecoder(SKImage)`.

The SkiaSharp port of binarization is restored, and `QrDecoderFixtureTests` covers versions 1–7, ECC L/M/Q/H, 15° rotation, and reduced-contrast “dark” images. Camera-style JPEGs are still outside that set, so decode remains experimental.

The decoder also keeps mutable instance fields (`FinderList`, transform coefficients, matrices). Instances are not safe to share across threads.

## Barcode library

`Genocs.BarcodeLibrary.Barcode` is a façade over ~22 symbology classes under `Symbologies/` (Code 128, Code 39, EAN/UPC, ITF-14, Pharmacode, Postnet, and others). Flow:

1. Select a symbology and encode the payload to a bar/space string.
2. Rasterize with SkiaSharp (`GenerateImage`).
3. Optionally draw a human-readable label (`Labels`).
4. Optionally serialize to XML (`ToXml` / `GetImageFromXML`).

The library is **encode-only**. Package tags still say “reader writer”. There is no barcode decode path.

`IDisposable` is implemented, but `Dispose` does not follow the managed/unmanaged split correctly (font and image disposal happens even when `disposing` is false).

## Demo Web API

`src/WebApi/Program.cs` exposes:

| Endpoint | Behavior |
| --- | --- |
| `GET /` | Welcome text |
| `GET health` | Liveness JSON |
| `GET /BuildQrCode` | PNG from query `payload` |
| `GET /BuildBarcode` | PNG from query `payload` + `barcodeType` |
| `POST /FindQrCode` | Upload an image and attempt QR decode |

The host pulls in `Genocs.Core`, logging, OpenAPI, MediatR, FluentResults, and **MongoDB persistence**, but the endpoints do not use a database or MediatR. Release builds reference **published NuGet packages** instead of project references, so a Docker/Release build can ship a different library version than the source tree.

There is no authentication, no request-size limit on uploads, and errors are often returned as HTTP 200 with a string body.

## Build and packaging

- Target frameworks: `net8.0;net9.0;net10.0`.
- `global.json` pins SDK `10.0.100` with `allowPrerelease: true` and `rollForward: latestMajor`.
- CI workflows install **.NET 8 only**.
- Both libraries set `GeneratePackageOnBuild=true` and hard-code `Version=5.0.0`.
- NuGet publish is a manual workflow that packs **Debug** and pushes with `--no-symbols`.
- Package README files on disk are placeholders (HTTP client / JWT copy) and are not wired as `PackageReadmeFile`.

## Test surface

| Project | What actually runs |
| --- | --- |
| `Genocs.QRCodeLibrary.Tests` | Payload generators; SVG smoke tests; decoder binarization + nine PNG fixtures. |
| `Genocs.BarcodeLibrary.Tests` | Pharmacode goldens plus encode-string and PNG tests for every `BarcodeType`. |

There is no QR PNG scanner/ZXing round-trip, no camera-JPEG decoder fixtures, and no Web API tests.
