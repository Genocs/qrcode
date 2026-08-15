# Production roadmap

Goal: make `Genocs.QRCodeLibrary` and `Genocs.BarcodeLibrary` honest, testable, and safe to consume from NuGet — or decide to wrap upstream instead of maintaining a fork.

Work is ordered. **Do not start P2 feature work while P0 is open.**

Estimated effort assumes one experienced .NET engineer familiar with SkiaSharp. Calendar time will be longer if legal review or scanner hardware is slow.

```text
P0  Correctness & license     2–4 weeks     ship blocker
P1  Production baseline       4–8 weeks     first supported preview
P2  Hardening                 4–6 weeks     1.0 / 5.1 quality
P3  Maturity                  ongoing       docs, governance, extras
```

---

## Decision gate (week 0)

Before writing more code, choose a strategy:

| Option | When to pick it | Consequence |
| --- | --- | --- |
| **A. Maintain the fork** | You need a SkiaSharp-only, multi-targeted Genocs package and can staff tests + releases. | Execute P0–P2 below. |
| **B. Thin wrapper** | You only need a Genocs-branded façade. | Depend on QRCoder + BarcodeLib (or ZXing) and delete the copied encoder/decoder. Fastest path to production. |
| **C. Archive** | The Web API demo is the only remaining value. | Mark NuGet as deprecated; keep the host as a sample. |

If you pick **A**, the rest of this document applies. If you pick **B**, skip encoder/decoder rewrites; still do license, CI, and Web API hardening.

---

## Phase 0 — Blockers (must fix before any “supported” label)

**Exit criteria:** QR encode produces scannable PNGs; SVG does not crash; decode either works on a fixture set or is removed from the public API; licenses are legally redistributable; CI fails if those tests fail.

| ID | Work item | Addresses | Notes |
| --- | --- | --- | --- |
| R-01 | Restore binarization in `QRDecoder.ConvertImageToBlackAndWhite` using `SKBitmap.GetPixels` / `SKPixmap`. Cap max width/height. | F-01, F-14 | **Done.** Histogram cutoff restored; images larger than `MaxDecodeImageDimension` (8192) are rejected. |
| R-02 | Add decoder fixtures: at least 8 images (version 1–7, ECC L/M/Q/H, rotated ~0/15°, light/dark). Assert payload. Run on Ubuntu. | F-01, F-05, F-17 | **Done.** Nine PNG fixtures in `DemoFiles/` (v1–v7, L/M/Q/H, 0° and 15°, light and dark). `QrDecoderFixtureTests` asserts payloads. Passed on Windows and Ubuntu (`dotnet sdk:10.0`). Regenerate with `dotnet run --file scripts/generate-decode-fixtures.cs`. |
| R-03 | Fix `SvgQRCode` Color overload to convert to hex and call the string overload. Add a test that `GetGraphic(20)` returns SVG starting with `<svg`. | F-02 | **Done.** `ColorToHex` delegates to the string overload; covered by `SvgQRCodeTests`. |
| R-04 | Rewrite `QRCode.GetGraphic`: filled aliased rects; `SKRect.Create` or explicit right/bottom; implement or **remove** the icon overload from the public API until it works. | F-03 | Prefer removing logo support in 5.x over shipping a stub. |
| R-05 | Golden-file or ZXing round-trip tests for PNG (and SVG via rasterize). Cover ECC Q, versions 2 and 10, UTF-8 payload. | F-05 | Do not trust visual inspection alone. |
| R-06 | Third-party notice: restore Apache-2.0 LICENSE + NOTICE for BarcodeLib, CPOL or replacement for the decoder, SPDX in csproj (`PackageLicenseExpression` **or** multi-license file). Legal review. | F-04 | **Not done.** Deleting `src/Genocs.BarcodeLibrary/LICENSE.txt` does not close this. If CPOL is unacceptable, replace the decoder with ZXing. |
| R-07 | Hide or obsolete `QRDecoder` until R-01/R-02 pass, so NuGet 5.x does not advertise a broken reader. | F-01 | `EditorBrowsable` / `[Obsolete]` is enough for a preview. |

**Suggested first preview after P0:** `5.0.1-preview` with README stating encode-only (or encode + experimental decode).

---

## Phase 1 — Production baseline

**Exit criteria:** CI tests all TFMs in Release; NuGet metadata describes this product; changelog is real; Web API is clearly a demo (or is locked down); public API no longer contains port leftovers.

| ID | Work item | Addresses |
| --- | --- | --- |
| R-10 | Single GitHub Actions workflow: matrix `net8.0 / net9.0 / net10.0` on `ubuntu-latest` (+ one Windows job for SkiaSharp). Install SDK 10, pin `global.json` (`allowPrerelease: false`, `rollForward: latestFeature`). | F-06, F-07 |
| R-11 | Pack **Release** only; SourceLink; snupkg; `dotnet nuget push` with symbols. Version from git tag (`MinVer` or `Nerdbank.GitVersioning`). Stop `GeneratePackageOnBuild` in Debug. | F-07, F-20 |
| R-12 | Rewrite package READMEs, descriptions, tags, release notes. Set `PackageReadmeFile`. Remove “reader” until decode ships. | F-08 |
| R-13 | Replace `CHANGELOG.md` with Keep a Changelog for *this* repo. Add `CONTRIBUTING.md`. Remove label-studio deploy buttons and fix README fences. | F-09 |
| R-14 | Finish SkiaSharp migration: no `System.Drawing` in public API (use `SKColor`, a small `QrSize` record, or hex strings). Delete “Unmerged change” comments. Fix XML `cref`s. | F-10 |
| R-15 | Typed exceptions: `BarcodeException`, keep `DataTooLongException`, stop `throw new Exception`. Do not swallow in the decoder; return `null` only for “not found”. | F-11 |
| R-16 | Barcode tests: encode-string tests for Code128, Code39, EAN-13, EAN-8, UPC-A, ITF-14, plus one PNG round-trip each. Port cases from upstream BarcodeLib where license allows. | F-05 | **Done.** `BarcodeSymbologyTests` covers every `BarcodeType` except `Unspecified`: binary `EncodedValue`, PNG raster, aliases, and invalid payloads. Pharmacode goldens remain in `PharmaCodeUnitTests`. |
| R-17 | Web API: treat as sample. Project-reference libraries in **all** configurations. Add `MultipartBodyLengthLimit`. Map errors to 4xx/5xx. Remove unused MongoDB/MediatR packages **or** actually use them. Fix Dockerfile (runtime native deps, Release, correct comments). Point docker publish at `src/WebApi/Dockerfile`. Delete `manual.yml`. | F-12 |
| R-18 | Replace console sample with a documented `examples/` project using relative paths. | F-13 |

**Suggested release:** `5.1.0` — first version you are willing to support with SemVer.

---

## Phase 2 — Hardening

**Exit criteria:** documented RID support; analyzer gate in CI; decoder (if shipped) has size limits and is not shared across threads; API reviewed.

| ID | Work item | Addresses |
| --- | --- | --- |
| R-20 | Document supported RIDs. Add `SkiaSharp.NativeAssets.*` as needed. Optional Alpine test job. | F-15 |
| R-21 | Decoder: max image dimension, `SKColorType` validation, no `unsafe` unless benchmarked, thread-safety note in XML docs. | F-14 |
| R-22 | `GeneratedRegex` / span digit check for `CheckNumericOnly`. | F-16 |
| R-23 | `TreatWarningsAsErrors` on libraries in CI (keep StyleCop noise down with a reviewed ruleset). | F-18 |
| R-24 | Public API analyzer (`Microsoft.CodeAnalysis.PublicApiAnalyzers`) so accidental surface changes fail the build. | F-10 |
| R-25 | Dependabot for `github-actions`. Remove empty `NuGet.config` or list nuget.org explicitly. | F-19, F-23 |
| R-26 | Decide XML barcode serialization: keep with tests, or obsolete. Same for PostScript renderer. | F-10 |

---

## Phase 3 — Maturity (ongoing)

| ID | Work item | Addresses |
| --- | --- | --- |
| R-30 | DocFX or similar from XML comments; examples for QR PNG, SVG, Code128. | F-22 |
| R-31 | SECURITY.md, supported versions table, vulnerability contact. | governance |
| R-32 | Remove unused `infrastructure/` PDF/FO assets **or** document why they belong. | F-21 |
| R-33 | Optional: GS1 / ISO/IEC 18004 compliance checklist and scanner lab notes (iOS Camera, Android, dedicated hardware). | F-05 |
| R-34 | Performance budget: encode 1000 QR version-5 / s on a reference VM; decode p95 for 1 MP images. | operability |
| R-35 | If decode remains weak, wrap ZXing for read and keep Genocs for write — document the split. | strategy |

---

## Suggested 90-day plan (option A)

| Window | Focus | Tangible output |
| --- | --- | --- |
| Days 1–14 | R-03, R-04, R-05, R-06 start | SVG/PNG do not crash; first scanner round-trip in CI |
| Days 15–30 | R-01, R-02, R-07 | Decode preview **or** obsolete reader APIs |
| Days 31–50 | R-10 … R-13, R-16 | Honest NuGet metadata; TFM matrix; barcode tests |
| Days 51–70 | R-14, R-15, R-17, R-18 | Clean API; demo host; examples |
| Days 71–90 | R-11 tags, `5.1.0` | First supported release **or** documented preview with a support window |

## What “done” means for production

A consuming team can say yes when **all** of the following are true:

1. PNG (and SVG, if advertised) round-trip tests pass on Linux CI for the documented ECC levels.
2. Decode is either tested on fixtures **or** not in the public surface.
3. Each advertised barcode symbology has encode tests; top retail symbologies have a raster check.
4. NuGet page matches the product; license files satisfy Apache/CPOL (or those parts were replaced).
5. Release artifacts are Release-mode, with symbols, versioned from tags.
6. Breaking changes follow SemVer; `PublicAPI.Shipped.txt` exists.
7. The Web API is not implied to be a supported SaaS unless it has auth, limits, and probes.

Until then, treat published `5.0.0` packages as **unsupported snapshots**.
