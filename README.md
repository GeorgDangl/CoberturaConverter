# CoberturaConverter

[![Build Status](https://jenkins.dangl.me/buildStatus/icon?job=CoberturaConverter/develop)](https://jenkins.dangl.me/job/CoberturaConverter/job/develop/)  
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
