# CoberturaConverter

[![Build Status](https://jenkins.dangl.me/buildStatus/icon?job=GeorgDangl%2FCoberturaConverter%2Fdevelop)](https://jenkins.dangl.me/job/GeorgDangl/job/CoberturaConverter/job/develop/)  
[![Built with Nuke](http://nuke.build/rounded)](https://www.nuke.build)  
[![NuGet](https://img.shields.io/nuget/v/CoberturaConverter.Core.svg)](https://www.nuget.org/packages/CoberturaConverter.Core)
[![MyGet](https://img.shields.io/myget/dangl/v/CoberturaConverter.Core.svg)](https://www.myget.org/feed/dangl/package/nuget/CoberturaConverter.Core)

[Changelog](./CHANGELOG.md)  
[Documentation](https://docs.dangl-it.com/Projects/CoberturaConverter)

This package aims to provide conversion of code coverage reports to the Cobertura format. Currently, it supports
OpenCover and dotCover source formats. It can be either directly used via the `CoberturaConverter` NuGet package,
as `Dangl.Nuke.CoberturaConverter` for the [NUKE Build](https://github.com/nuke-build/nuke) system or as command
line tool via `CoberturaConverter.CommandLine`.

This project is based on [Daniel Palmes OpenCoverToCobertura Converter](https://github.com/danielpalme/OpenCoverToCoberturaConverter),
which is licensed under the [Apache License](./src/CoberturaConverter.Core/OpenCoverToCoberturaConverterLicense.md).

## dotCover Report Types

Because the `filename` attribute is mandatory in the Cobertura format, dotCover reports should always be generated with the
`DetailedXml` setting instead of the regular `Xml` to output filenames. Otherwise, all classes in the dotCover report that do
not have a source file specified are ignored.

## Referencing

If this is used in full .Net framework 4.6.1 and earlier, please add a reference to
    
    <PackageReference Include="System.ValueTuple" Version="4.4.0" />

## CI Builds

All builds are available on MyGet:

    https://www.myget.org/F/dangl/api/v2
    https://www.myget.org/F/dangl/api/v3/index.json

## CLI Usage

You can use the converter from the command line, it is available in the `CoberturaConverter.CommandLine`
NuGet package under `/tools` both for **net461** and **netcoreapp2.0**.

    CoberturaConverter.CommandLine.exe -i <InputFile> -o <OutputFile> -s <DotCover | OpenCover>

| Parameter | Description |
|-----------|-------------|
| -i        | Path to the input file |
| -o        | Path where to save the converted report to |
| -s        | Source report format, can be either `DotCover` or `OpenCover` |
| --help    | Display options |

## NUKE Example

The package is available as `Dangl.Nuke.CoberturaConverter`.

    using static Nuke.CoberturaConverter.CoberturaConverterTasks;

    await DotCoverToCobertura(s => s
        .SetInputFile(OutputDirectory / "coverage.xml")
        .SetOutputFile(OutputDirectory / "cobertura_coverage.xml"));

---
[License](./LICENSE.md)


## Markdown documentation publishing

`BuildDocumentation` also generates `output-markdown/docs-markdown` and `output-markdown/docs-markdown.zip`.
Start with `index.md` (the source README), or `toc.md` for all articles and API pages.
Changelog/license text and linked documentation assets are included; the ZIP has no
enclosing directory.

The Markdown renderer converts the existing published DocFX article HTML, including
API signatures, code examples and anchors. This keeps the current documentation
toolchain and public project boundaries intact, without requiring newer DocFX or
migrating the legacy NUKE build. It does not modify documentation sources.

`UploadDocumentation` uploads HTML first, then sends the Markdown ZIP to
`/API/MarkdownDocuments/Upload` with the same API key and exact version. Generation
or upload errors fail publishing; existing branch gates and version-conflict policies
are preserved. `TestMarkdownDocumentation` exercises packaging, links, assets,
failure handling and the multipart contract using only a local HTTP server.
