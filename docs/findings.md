# Findings catalog

Severity:

- **P0** — correctness or compliance blocker. Do not ship.
- **P1** — required before calling the library “production”.
- **P2** — quality, operability, or maintainability; schedule after P1.
- **P3** — hygiene and polish.

Evidence paths are relative to the repository root.

---

## P0 — Blockers

### F-05 No image-level proof that codes scan

**Component:** tests  
**Evidence:** `tests/Genocs.QRCodeLibrary.Tests/SvgQRCodeTests.cs`, `QrDecoderFixtureTests.cs`, `Genocs.BarcodeLibrary.Tests/BarcodeSymbologyTests.cs`

| Area | Automated proof |
| --- | --- |
| QR PNG/SVG/PS output | SVG smoke tests; PNG still lacks a scanner round-trip |
| QR decode fixtures | Nine synthetic PNGs (`QrDecoderFixtureTests`) |
| Barcode raster | PNG encode for every `BarcodeType` (`BarcodeSymbologyTests`) |
| Barcode encode strings | All symbologies; Pharmacode goldens (4 cases) |
| Payload URI/vCard/Swiss QR strings | Extensive (`PayloadGeneratorTests`) |

A library whose job is to produce scannable images cannot be production-grade without golden images or a decode round-trip (ZXing or this decoder, once fixed).

---

## P1 — Production baseline

### F-08 Package metadata is wrong or incomplete

**Evidence:** `*.csproj`, package `README.md` files

- `Genocs.BarcodeLibrary` description: “QRCode library for .NET Core.”
- QR package README: “Core NuGet package http client implementation…”
- Barcode package README: “handling authorization logic as JWT.”
- `PackageReleaseNotes`: “Removed reference”
- `PackageReadmeFile` is not set, so nuget.org will not show the README even after it is fixed.
- No SourceLink, no snupkg, `GeneratePackageOnBuild` on every local build.
- Tags claim “ writer”; barcode is encode-only.

### F-10 Public API is an unfinished port

**Evidence:** encoder/barcode sources

- `/* Unmerged change from project 'Genocs.QRCodeLibrary (net8.0)' */` blocks in `QRCodeGenerator.cs`.
- XML docs still say `QRCoder.Exceptions.DataTooLongException`.
- `SvgQRCode` / `PostscriptQRCode` still depend on `System.Drawing.Color` / `Size`.
- `Barcode.DoEncode` overloads take `System.Drawing.Color`.
- `Barcode.Dispose` disposes managed `SKFont`/`SKImage` in the finalizer path; comments still say “TODO”.
- `QRCodeGenerator` implements `IDisposable` with an empty instance constructor — typical QRCoder leftover, confusing for consumers.

### F-11 Exception model is not production-grade

**Evidence:** `BarcodeCommon.Error`, payload classes, decoder

- Barcode path: `throw new Exception(errorMessage)` after appending to `Errors`.
- OTP / several payloads: `throw new Exception(...)`.
- Decoder: `ApplicationException` with typos (“assinment”, “DataLengh”), swallowed by empty `catch` in `ImageDecoderRaw`.

Callers cannot filter barcode vs QR vs argument errors. Empty catches hide defects (this is how F-01 stays green in CI).

### F-12 Demo Web API is unsafe to expose

**Evidence:** `src/WebApi/Program.cs`, `Host.csproj`, `appsettings.json`

- No authentication; OpenAPI `includeSecurity: false`.
- `POST /FindQrCode` copies the whole upload into memory with no size cap (DoS).
- Catch blocks return `Results.Ok(message)` — HTTP 200 for failures, including exception text.
- `GET /BuildQrCode` has no payload length cap (QR versions go to 40; still a CPU/RAM knob).
- Release configuration references `Genocs.BarcodeLibrary` / `Genocs.QRCodeLibrary` **NuGet 5.0.\*** instead of project references — Docker Release builds can ship stale packages.
- Host references MongoDB, MediatR, FluentResults without using them in the endpoint code.
- Dockerfile comments describe a “Fonet PDF Web API”; native deps are installed in the **SDK** stage, not the runtime stage.
- `web-api.rest` posts JSON to `/findQrCode` while the endpoint expects `IFormFile`.

### F-13 Console sample is not a contract

**Evidence:** `src/Console/Program.cs`

Hard-coded `C:\dev\image4_out.jpg`, empty `catch`, PNG written with a `.jpg` extension. Fine as a scratchpad; dangerous if copied into docs or samples.

---

## P2 — Hardening

### F-14 Thread safety and resource use (decoder)

`QRDecoder` stores all working state on the instance (`BlackWhiteImage`, finder lists, transform doubles). It is not re-entrant. `BitonalFromBitmap` is `unsafe` and assumes 32-bit BGRA without validating `SKColorType`. Large images allocate full-size bool matrices with no cap.

### F-15 SkiaSharp native assets are Linux-centric

Both libraries reference `SkiaSharp.NativeAssets.Linux.NoDependencies`. Windows/macOS often work via the default SkiaSharp assets, but Alpine, Windows Server Nano, and some CI images still fail at runtime. Document supported RIDs and add RID-specific test jobs.

### F-16 Regex compiled on every numeric check

`BarcodeCommon.CheckNumericOnly` uses `Regex.IsMatch(..., RegexOptions.Compiled)` per call. Use a generated regex or `ReadOnlySpan` digit scan.

### F-18 Analyzers are installed but not enforced

`TreatWarningsAsErrors` and `CodeAnalysisTreatWarningsAsErrors` are false. StyleCop CS1591 is disabled. The ruleset exists, but CI will not fail on it.

### F-20 Versioning is not SemVer-from-git

`Version` and `MinClientVersion` are both `5.0.0` in the csproj. `MinClientVersion` is a **NuGet client** constraint, not a package version — the coincidence is confusing. There is no `GitVersion`, MinVer, or tag-driven version. Manual workflow default is always `5.0.0`.

---

## P3 — Hygiene

### F-22 Documentation file generation without public API ship

`GenerateDocumentationFile` is true, but there is no DocFX / doc site, and many public members have no comments (CS1591 silenced). XML docs in the nupkg will be sparse.

---

## Issue count by severity

| Severity | Count |
| --- | --- |
| P0 | 5 |
| P1 | 8 |
| P2 | 7 |
| P3 | 3 |
| **Total** | **23** |

IDs are stable for the roadmap (`F-01` … `F-23`). When an item is fixed, mark it in the changelog rather than renumbering.
