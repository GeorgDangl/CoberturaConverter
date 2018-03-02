# CoberturaConverter

[![Build Status](https://jenkins.dangl.me/buildStatus/icon?job=CoberturaConverter.Tests)](https://jenkins.dangl.me/job/CoberturaConverter.Tests/)

[![Built with Nuke](http://nuke.build/rounded)](https://www.nuke.build)

[![NuGet](https://img.shields.io/nuget/v/CoberturaConverter.Core.svg)](https://www.nuget.org/packages/CoberturaConverter.Core)
[![MyGet](https://img.shields.io/myget/dangl/v/CoberturaConverter.Core.svg)](https://www.myget.org/feed/dangl/package/nuget/CoberturaConverter.Core)

[Changelog](./CHANGELOG.md)

This package aims to provide conversion of code coverage reports to the Cobertura format. Currently, it supports
OpenCover and dotCover source formats.
It can be either directly used via the `CoberturaConverter` NuGet package, as `Nuke.CoberturaConverter` for
the [NUKE Build](https://github.com/nuke-build/nuke) system or as command line tool via `CoberturaConverter.CommandLine`.

This project is based on [Daniel Palmes OpenCoverToCobertura Converter](https://github.com/danielpalme/OpenCoverToCoberturaConverter),
which is licensed under the [Apache License](./src/CoberturaConverter.Core/OpenCoverToCoberturaConverterLicense.md).

## Referencing

If this is used in full .Net framework earlier than 4.6.1, please add a reference to
    
    <PackageReference Include="System.ValueTuple" Version="4.4.0" />

## CI Builds

All builds are available on MyGet:

    https://www.myget.org/F/dangl/api/v2
    https://www.myget.org/F/dangl/api/v3/index.json

## NUKE Example

    using static Nuke.CoberturaConverter.CoberturaConverterTasks;

    await DotCoverToCobertura(s => s
        .SetInputFile(OutputDirectory / "coverage.xml")
        .SetOutputFile(OutputDirectory / "cobertura_coverage.xml"));

---
[License](./LICENSE.md)
