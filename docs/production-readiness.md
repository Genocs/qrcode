# Production readiness assessment

**Verdict: not ready for general production use.**

The repository is a useful **porting experiment** and a **demo host**. It is not yet a library that a production team should depend on for scannable codes, legal compliance, or a public HTTP API.

Conditional use is possible only in a narrow case: **barcode encoding of a single, self-verified symbology**, after you add tests for that symbology and treat the rest of the package as unsupported. QR **decode must not be used**. QR **encode** should not be used until rendering bugs are fixed and images are validated against real scanners.

## Why this verdict

Production barcode/QR libraries are judged on correctness under scanners, a stable public API, test evidence, license clarity, and a repeatable release pipeline. This repository currently fails several of those bars at once:

1. **QR decode is unproven on camera photos.** Synthetic PNG fixtures pass (v1–v7, rotation, contrast). Do not use decode in production until real-world images are covered.
2. **QR encode still has rendering bugs.** SVG color overloads are fixed; logo overlay uses the wrong `SKRect` constructor and stroke/antialias settings that hurt scan reliability. PNG has no scanner round-trip.
3. **Automated evidence is incomplete** for the product promise. Barcode encode strings and PNG raster are covered for every symbology. QR decode has synthetic fixtures. QR PNG still lacks a scanner/ZXing round-trip.
4. **CI does not match the product.** Workflows install .NET 8 while the libraries multi-target 8/9/10 and `global.json` requires SDK 10.
5. **Packaging and docs would mislead NuGet consumers.** Package READMEs describe unrelated products (HTTP client, JWT). Changelog belongs to another repository. Version `5.0.0` is hardcoded and not driven by tags.
6. **License provenance is unresolved.** The tree is MIT-only (`LICENSE`); BarcodeLib remains Apache-2.0 (its license file was removed) and the decoder remains typically CPOL. See [F-04](findings.md).
7. **The Web API is a demo, not a service.** Unauthenticated generation, unbounded upload, errors mapped to HTTP 200, unused MongoDB/MediatR stack, Release builds consume NuGet instead of the local projects.

## Maturity scores

Scores are 0–10. They reflect *this* codebase, not the original upstream projects.

| Dimension | Score | Comment |
| --- | --- | --- |
| Feature breadth | 7 | Encoder payloads, many 1D symbologies, several output formats exist on disk. |
| Cross-platform direction | 5 | SkiaSharp + Linux native assets is the right bet; `System.Drawing` leftovers and incomplete ports remain. |
| Correctness | 4 | Decoder fixtures pass on synthetic PNGs; SVG color path fixed; logo QR and PNG scanner round-trip still open. |
| Test evidence | 4 | Barcode encode + PNG for all symbologies; QR decode fixtures; QR PNG still unproven against a scanner. |
| Public API quality | 4 | Large surface, mixed `Color`/`SKColor`, empty `IDisposable`, unmerged-comment leftovers. |
| Security (libraries) | 5 | Encode libraries are mostly pure compute; unsafe decoder pointer code exists. |
| Security (Web API) | 2 | No auth, no upload limits, information-leaking 200 responses. |
| CI / release | 3 | Duplicate workflows, wrong SDK, Debug packs, missing Docker file in publish workflow. |
| Documentation | 2 | Root README has broken markup and wrong deploy buttons; package READMEs are placeholders. |
| License / provenance | 3 | MIT packaged; Apache license copy removed; no NOTICE / CPOL / third-party bill of materials. |
| **Overall** | **3** | Prototype / demo quality. |

## Go / no-go by use case

| Use case | Decision | Notes |
| --- | --- | --- |
| Generate QR images for customers, tickets, payments | **No** | Rendering defects + no scanner golden tests. Prefer QRCoder or ZXing.Net until this fork is validated. |
| Decode QR from camera / uploads | **No** | Synthetic PNG fixtures pass; camera JPEGs are still unproven. |
| Generate Swiss QR / Girocode / Wi-Fi payload *strings* | **Maybe** | Payload tests exist, but they do not prove a scannable QR. |
| Generate 1D barcodes (Code 128, EAN, UPC, …) | **Maybe, per symbology** | Encode-string and PNG tests exist for every type. Re-validate against a scanner or GS1 samples before shipping. |
| Expose the Web API on the public internet | **No** | Treat as a local demo only. |
| Ship NuGet 5.0.0 as a supported product | **No** | Metadata, symbols, versioning, and docs are not at product quality. |

## What is in good shape

Give credit where it is due; these pieces are a foundation, not a pass:

- Multi-targeting `net8.0;net9.0;net10.0` and a SkiaSharp drawing stack are the right long-term shape for Linux containers.
- Payload generator coverage is broad (Bitcoin, calendar, mail, Swiss QR, BezahlCode, and more).
- StyleCop / Roslynator / `.editorconfig` show an intent to keep a consistent house style.
- Dependabot is configured for NuGet.
- The solution layout (libraries vs host vs tests) is understandable.

## Comparison to mature alternatives

If you need production codes *now*, the usual .NET choices are still safer:

| Need | Mature option | Why it wins today |
| --- | --- | --- |
| QR encode | [QRCoder](https://github.com/codebude/QRCoder) | This encoder *is* a QRCoder port, minus years of fixes and tests. |
| QR decode | ZXing.Net, ZXing.Net.Bindings.SkiaSharp | Maintained decoder, real image tests. |
| 1D barcodes | [BarcodeLib](https://github.com/barnhill/barcodelib) or a commercial GS1 library | This barcode package *is* BarcodeLib, now without its Apache license file in-tree. |

A Genocs fork is justified only if you need a **single vendor surface**, **SkiaSharp-only** binaries, or payload/host integration that upstream does not offer — and only after the roadmap’s P0/P1 work is done.

## Recommended posture

1. Label NuGet packages and the GitHub README as **preview / experimental** until P1 is complete.
2. Do not market decode, SVG, or logo QR as supported.
3. Freeze the public API; stop adding payload types until encode/decode tests exist.
4. Execute the [roadmap](roadmap.md) in order. Do not start with new features.

Detailed evidence lives in [findings.md](findings.md).
