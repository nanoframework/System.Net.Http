# Copilot Instructions for nanoframework/System.Net.Http

## Repository Overview

This repository implements the `System.Net.Http` class library for [.NET nanoFramework](https://nanoframework.net/) — a free, open-source platform that enables running C# code on constrained embedded/IoT devices (microcontrollers). It is **not** standard .NET; it targets a stripped-down CLR with significant limitations.

The library is published as three separate NuGet packages:
- `nanoFramework.System.Net.Http` — full HTTP library (client + server + shared types)
- `nanoFramework.System.Net.Http.Client` — client-only subset
- `nanoFramework.System.Net.Http.Server` — server-only subset

## Key nanoFramework Constraints

These constraints are critical to understand before making any code changes:

- **No `async`/`await`**: nanoFramework does not support the Task Parallel Library or `async`/`await`. All APIs are synchronous. Method names intentionally drop the `Async` suffix.
- **No generics**: Generic types and methods are not supported.
- **No LINQ**: LINQ is not available.
- **No `Task`/`CancellationToken`**: These concepts do not exist in the nanoFramework runtime.
- **Limited reflection**: Only a very limited subset of reflection is available.
- **Limited memory**: Devices may have as little as 16 KB of RAM; always dispose `HttpResponseMessage` after use.
- **No `async` streams**: All I/O is blocking and synchronous.
- **Target framework**: `v1.0` (nanoFramework's own TFM, not .NET 1.0).
- **No `HttpClient.SendAsync`**: Replaced by synchronous `HttpClient.Send` / convenience methods (`Get`, `Post`, `Put`, `Delete`).

## Repository Structure

```
/
├── nanoFramework.System.Net.Http/         # Main library project (full)
│   ├── Http/                              # All source files
│   │   ├── Headers/                       # HTTP header types
│   │   ├── HttpClient.cs                  # HttpClient implementation
│   │   ├── HttpContent.cs, StringContent.cs, ByteArrayContent.cs, StreamContent.cs
│   │   ├── HttpRequestMessage.cs, HttpResponseMessage.cs, HttpMethod.cs
│   │   ├── HttpMessageHandler.cs, HttpClientHandler.cs, HttpMessageInvoker.cs
│   │   ├── System.Net.HttpWebRequest.cs   # Lower-level WebRequest implementation
│   │   ├── System.Net.HttpListener*.cs    # HTTP server listener
│   │   └── System.Net.*.cs               # Supporting System.Net types (Uri, WebRequest, etc.)
│   ├── System.Net.Http.nfproj             # nanoFramework project file (MSBuild)
│   ├── key.snk                            # Strong-name signing key
│   └── packages.config / packages.lock.json
│
├── nanoFramework.System.Net.Http.Client/  # Client-only subset project
│   ├── Http/Headers/                      # Client-specific headers (mostly .gitkeep)
│   └── System.Net.Http.Client.nfproj
│
├── nanoFramework.System.Net.Http.Server/  # Server-only subset project
│   └── System.Net.Http.Server.nfproj
│
├── Tests/
│   └── HttpUnitTests/                     # Unit test project (nanoFramework test runner)
│       ├── HttpClientTest.cs              # HttpClient tests (skipped on WIN32 nanoCLR)
│       ├── HttpContentTest.cs, ByteArrayContentTest.cs, StreamContentTest.cs, StringContentTest.cs
│       ├── MediaTypeHeaderValueTest.cs
│       ├── HttpUtilityTest.cs
│       ├── UriUnitTests.cs
│       ├── LoopbackServer.cs
│       ├── MockContent.cs
│       └── HttpUnitTests.nfproj
│
├── nanoFramework.System.Net.Http.sln      # Visual Studio solution
├── nanoFramework.System.Net.Http.nuspec  # NuGet spec (full package)
├── nanoFramework.System.Net.Http.Client.nuspec
├── nanoFramework.System.Net.Http.Server.nuspec
├── version.json                           # Nerdbank.GitVersioning config (semver 2.0)
├── NuGet.Config                           # Points to nuget.org
├── azure-pipelines.yml                    # CI/CD build pipeline (Azure Pipelines)
├── .github/workflows/                     # GitHub Actions workflows
│   ├── pr-checks.yml                      # PR validation (package lock + NuGet version checks)
│   ├── update-dependencies.yml
│   ├── update-dependencies-develop.yml
│   └── generate-changelog.yml
└── .editorconfig                          # Code style configuration
```

## Project File Format

All library and test projects use `.nfproj` files — a custom MSBuild project format for nanoFramework. These are **not** standard `.csproj` files. The project type GUID is `{11A8DD76-328B-46DF-9F39-F559912D0360}`.

- Dependencies are declared via `packages.config` (not `PackageReference`).
- Package restore locks are stored in `packages.lock.json`.
- In CI (`TF_BUILD=True`), packages are restored in locked mode.
- The nanoFramework MSBuild SDK is referenced via `$(MSBuildExtensionsPath)\nanoFramework\v1.0\`.

## Coding Conventions

From `.editorconfig` and existing code:

- **File encoding**: UTF-8 with BOM (`utf-8-bom`), CRLF line endings.
- **Indentation**: 4 spaces for C#, 2 spaces for XML/YAML/config files.
- **Namespaces**: `System.Net` and `System.Net.Http` (mirroring .NET's BCL namespaces).
- **Naming**:
  - Private/internal fields: `_camelCase` (underscore prefix)
  - Static private/internal fields: `s_camelCase`
  - Constants: `PascalCase`
- **Braces**: Allman style (open brace on new line) for C#.
- **No `var`**: Explicit types are strongly preferred.
- **License header**: Every `.cs` file starts with:
  ```csharp
  //
  // Copyright (c) .NET Foundation and Contributors
  // Portions Copyright (c) Microsoft Corporation.  All rights reserved.
  // See LICENSE file in the project root for full license information.
  //
  ```
- **XML doc comments**: All public APIs must have `<summary>`, `<param>`, `<returns>`, `<exception>` etc.
- **Assembly signing**: The main library assembly is strong-name signed using `key.snk`.

## Testing

- Tests use `nanoFramework.TestFramework` (attributes: `[TestClass]`, `[TestMethod]`, `[Setup]`).
- The test project targets the nanoFramework nanoCLR runtime.
- **`HttpClientTest` is always skipped** on the WIN32 nanoCLR (no network support); `Assert.SkipTest(...)` is called in `[Setup]`.
- Network-dependent tests cannot be run in a standard CI environment — only on real hardware or an emulator with network access.
- Tests that can run on WIN32 nanoCLR (no network): `HttpUtilityTest`, `UriUnitTests`, `ByteArrayContentTest`, `StreamContentTest`, `StringContentTest`, `HttpContentTest`, `MediaTypeHeaderValueTest`.
- The test runner binary is `nanoFramework.UnitTestLauncher.exe` from the `nanoFramework.TestFramework` package.
- Run settings: `Tests/HttpUnitTests/nano.runsettings`.

## Building

- **Build toolchain**: Visual Studio 2022 on Windows with the nanoFramework extension installed.
- **CI build**: Azure Pipelines (`azure-pipelines.yml`) runs on `windows-latest`, builds `Release|Any CPU`.
- The build is orchestrated via templates from the `nanoframework/nf-tools` repository.
- Three NuGet packages are produced: full, client, server.
- Package versioning is managed by `Nerdbank.GitVersioning` (`version.json`), current major.minor: `1.5`.
- There is **no standard `dotnet build`** CLI workflow — builds require the nanoFramework MSBuild extensions.

## CI/CD Workflows

### Azure Pipelines (`azure-pipelines.yml`)
- Triggers on push to `main`, `develop`, `release-*`, and version tags (`v*`).
- Two jobs: `Build_Library` and `Update_Dependents`.
- Publishes to NuGet and creates GitHub releases from `main`.
- Downstream repos updated on tag: `nanoFramework.WebServer`, `nanoFramework.Azure.Devices`, `System.Net.WebSockets`.
- Build failures are reported to Discord via webhook.

### GitHub Actions
- `pr-checks.yml`: Validates `packages.lock.json` consistency and that NuGet packages are up to date.
- `update-dependencies.yml` / `update-dependencies-develop.yml`: Automated dependency update PRs.
- `generate-changelog.yml`: Auto-generates `CHANGELOG.md`.

## Key API Design Decisions

- `HttpClient` is designed to be instantiated once and reused (same as .NET).
- `HttpResponseMessage` **must always be disposed** — device memory is very limited.
- HTTPS requires manually providing the CA root certificate via `HttpClient.HttpsAuthentCert` (an `X509Certificate`).
- The library mirrors .NET `System.Net.Http` APIs as closely as possible, with synchronous equivalents of async methods.
- `HttpClient` convenience methods: `Get(string)`, `Post(string, HttpContent)`, `Put(string, HttpContent)`, `Delete(string)`.
- `HttpContent` subtypes: `StringContent`, `ByteArrayContent`, `StreamContent`.
- Header types mirror .NET: `HttpRequestHeaders`, `HttpResponseHeaders`, `HttpContentHeaders`, `MediaTypeHeaderValue`, etc.

## Known Limitations / Workarounds

- When sending consecutive requests to a development machine through a reverse proxy (e.g., iisexpress-proxy), `SocketException`s may occur. A retry mechanism in Debug mode is the recommended workaround.
- No `CancellationToken` support — timeouts are set via `HttpClient.Timeout`.
- No multipart form data support.
- The `System.Net.Http.Client` and `System.Net.Http.Server` projects currently have empty `Http/` directories (only `.gitkeep`) — they reference the full library assembly. Check `.nfproj` files to understand what each package actually packages.

## Versioning

- Version is managed by `Nerdbank.GitVersioning`.
- Current version line: `1.5` (see `version.json`).
- Release branches follow pattern: `release-v{version}`.
- Public release branches: `main`, `develop`, `v\d+(\.\d+)?`.
- NuGet packages use SemVer 2.0.

## Dependencies (NuGet packages)

- `nanoFramework.CoreLibrary` (mscorlib)
- `nanoFramework.Runtime.Events`
- `nanoFramework.System.Collections`
- `nanoFramework.System.Text`
- `nanoFramework.System.Net`
- `nanoFramework.System.Threading`
- `nanoFramework.System.IO.Streams`
- `Nerdbank.GitVersioning` (build-time only)
