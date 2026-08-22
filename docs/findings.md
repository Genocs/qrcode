# Findings catalog

Severity:

- **P0** — correctness or compliance blocker. Do not ship.
- **P1** — required before calling the library “production”.
- **P2** — quality, operability, or maintainability; schedule after P1.
- **P3** — hygiene and polish.

Evidence paths are relative to the repository root.

---

## P0 — Blockers

### F-01 QR decoder binarization (fixed)

**Component:** `Genocs.QRCodeLibrary` decoder  
**Status:** Fixed (R-01)  
**Evidence:** `src/Genocs.QRCodeLibrary/Decoder/QRDecoder.cs` (`ConvertImageToBlackAndWhite`)

~~The SkiaSharp migration commented out the original `LockBits` conversion. The method now returns `true` immediately and never assigns `BlackWhiteImage`.~~ **Fixed:** `ConvertImageToBlackAndWhite` rasterizes via `SKBitmap.FromImage`, reads pixels through `SKPixmap.GetPixels` / `GetPixelSpan`, applies the original luminance histogram cutoff, and rejects images whose width or height exceeds `QRDecoder.MaxDecodeImageDimension` (8192).

Binarization is covered by `QrDecoderBinarizationTests`. End-to-end decode is covered by `QrDecoderFixtureTests` (versions 1–7, ECC L/M/Q/H, 0° and 15° rotation, light and reduced-contrast dark).

**Impact (remaining):** Camera JPEGs (`image2.jpg` / `image3.jpg`) are still not part of the fixture set. The package tags still advertise “reader”.

### F-02 `SvgQRCode` Color overload infinite recursion

**Status:** Fixed (R-03)  
**Component:** `Genocs.QRCodeLibrary` encoder  
**Evidence:** `src/Genocs.QRCodeLibrary/Encoder/SvgQRCode.cs`

The `GetGraphic(Size, Color, Color, ...)` overload now converts colors to `#RRGGBB` / `#AARRGGBB` and calls the string overload. `GetGraphic(int)` is covered by `SvgQRCodeTests`.

### F-03 Logo QR renderer is incorrect

**Component:** `Genocs.QRCodeLibrary` encoder  
**Evidence:** `src/Genocs.QRCodeLibrary/Encoder/QRCode.cs` (`GetGraphic` with `icon`)

Several independent defects in one method:

- `SKRect` is constructed as if it were `(x, y, width, height)`. SkiaSharp uses `(left, top, right, bottom)`. Modules draw in the wrong place.
- `SKPaint.Style` is `Stroke` with `IsAntialias = true`. QR modules must be **filled, axis-aligned, aliased** rectangles.
- Icon clipping and `DrawImage` are commented out (`SixLabors` leftovers). Passing an icon therefore either skips modules or does not composite the logo.

The simpler `GetGraphic` without icon also enables antialiasing, which softens module edges and reduces scan reliability.

### F-04 License overlay is incomplete

**Component:** repository / NuGet packages  
**Status:** Open (R-06). The barcode Apache license copy was removed; that does not close the finding.  
**Evidence:** root `LICENSE` (MIT), `Directory.Build.props` (`PackageLicenseFile` → `LICENSE`), NuGet `None Include="..\..\LICENSE"`, no `NOTICE` / `THIRD_PARTY`, no Apache or CPOL file in the tree

Current layout:

- Genocs-authored packaging and docs are MIT (`LICENSE`).
- `Genocs.BarcodeLibrary` is still a port of [BarcodeLib](https://github.com/barnhill/barcodelib) (**Apache-2.0**). The former `src/Genocs.BarcodeLibrary/LICENSE.txt` was deleted. Apache redistribution still requires the Apache license text, attribution, and NOTICE contents for that code.
- The QR decoder still originates from a CodeProject article typically under **CPOL**, which is not MIT and has source-display obligations. There is no CPOL file in the repo.
- Packed NuGet artifacts still ship only the root MIT `LICENSE`.

Removing the Apache file made the tree look single-licensed. It did not relicense BarcodeLib or the decoder. Legal review is still required before commercial redistribution.

### F-05 No image-level proof that codes scan

**Component:** tests  
**Evidence:** `src/tests/Genocs.QRCodeLibrary.Tests/SvgQRCodeTests.cs`, `QrDecoderFixtureTests.cs`, `Genocs.BarcodeLibrary.Tests/BarcodeSymbologyTests.cs`

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

### F-06 CI SDK and target frameworks diverge

**Evidence:** `.github/workflows/dotnet.yml`, `build_and_test.yml`, `nuget-publish.yml`, `global.json`

- Workflows: `dotnet-version: 8.0.x`
- Libraries: `net8.0;net9.0;net10.0`
- `global.json`: SDK `10.0.100`, `allowPrerelease: true`, `rollForward: latestMajor`

CI cannot honestly claim the advertised TFMs. `allowPrerelease` plus `latestMajor` makes local builds non-reproducible.

### F-07 Duplicate and leftover workflows

**Evidence:** `.github/workflows/`

- `dotnet.yml` and `build_and_test.yml` both build/test on push to `main` (one also packs).
- `manual.yml` prints “Hello World” and run numbers.
- `dockerhub-publish.yml` builds `webapi.dockerfile`, which is not in the repo (`src/WebApi/Dockerfile` exists instead).
- NuGet publish packs **Debug** and pushes `--no-symbols`.

### F-08 Package metadata is wrong or incomplete

**Evidence:** `*.csproj`, package `README.md` files

- `Genocs.BarcodeLibrary` description: “QRCode library for .NET Core.”
- QR package README: “Core NuGet package http client implementation…”
- Barcode package README: “handling authorization logic as JWT.”
- `PackageReleaseNotes`: “Removed reference”
- `PackageReadmeFile` is not set, so nuget.org will not show the README even after it is fixed.
- No SourceLink, no snupkg, `GeneratePackageOnBuild` on every local build.
- Tags claim “reader writer”; barcode is encode-only.

### F-09 Changelog and contributing artifacts are missing or false

**Evidence:** `CHANGELOG.md`, root `README.md`

- Changelog is generated for `Genocs/telegram-integration` and version `0.0.1`.
- README links `CHANGELOGS.md` and `CONTRIBUTING.md`; neither file exists.
- Cloud deploy buttons point at **heartexlabs/label-studio**.
- README contains a broken nested ` ``` bash ` fence.

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

### F-17 Helper paths are Windows-only and mis-cased

**Status:** Fixed. `HelperUnitTests` now uses `Path.Combine(..., "DemoFiles", filename)`.

### F-18 Analyzers are installed but not enforced

`TreatWarningsAsErrors` and `CodeAnalysisTreatWarningsAsErrors` are false. StyleCop CS1591 is disabled. The ruleset exists, but CI will not fail on it.

### F-19 Empty `NuGet.config`

`NuGet.config` declares empty `packageSources` / `packageSourceCredentials`. Harmless in some environments, confusing in others (locked-down agents). Prefer deleting it or explicitly listing nuget.org.

### F-20 Versioning is not SemVer-from-git

`Version` and `MinClientVersion` are both `5.0.0` in the csproj. `MinClientVersion` is a **NuGet client** constraint, not a package version — the coincidence is confusing. There is no `GitVersion`, MinVer, or tag-driven version. Manual workflow default is always `5.0.0`.

---

## P3 — Hygiene

### F-21 Dead and copied content

- `infrastructure/azure-pipelines.yml` alongside GitHub Actions.
- Docker compose + XSL/FO templates under `infrastructure/containers/docker/data` look copied from another Genocs host (PDF/fonts).
- `Barcode` enum members such as `MsiMod11Mod10` lack XML comments while neighbors have them.
- `QRDecoder.VersionNumber = "Rev 2.1.0 - 2019-07-22"` is upstream’s date, not Genocs.

### F-22 Documentation file generation without public API ship

`GenerateDocumentationFile` is true, but there is no DocFX / doc site, and many public members have no comments (CS1591 silenced). XML docs in the nupkg will be sparse.

### F-23 Dependabot only covers NuGet at `/`

GitHub Actions versions (`actions/checkout@v4`, `setup-dotnet@v4`) are not updated. No `github-actions` ecosystem entry.

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
