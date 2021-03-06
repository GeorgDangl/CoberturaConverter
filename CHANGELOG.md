# Changelog

All notable changes to **CoberturaConverter** are documented here.

## v2.0.0
- The .NET Core framework targets were set to v3.1 for the CLI tools
- The NUKE package was updated to be compatible with NUKE 5.0.0

## v1.3.1:
- Fixed a bug that caused an exception when converting from dotCover format and the base path was a drive root. Thanks to GitHub user @sjmelia!

## v1.3.0:
- Update NUKE to 0.19.1

## v1.2.0:
- Update NUKE to 0.6.0

## v1.1.4:
- Downgrade CLI tool from `net471` to `net461` for broaded compatibility (see #3)

## v1.1.3:
- Bugfix: dotCover to Cobertura reported covered statements for total statements (see #2)

## v1.1.2:
- Bugfix: dotCover failed when method did contain double colons `::` (see #1)

## v1.1.1:
- Bugfix: dotCover failed when method name did not contain any parentheses `(` (see #1)

## v1.1.0:
- Update to NUKE 0.4.0

## v1.0.1
- Support nested types when converting from dotCover
- Exclude types without lines when converting from dotCover to Cobertura

## v1.0.0
- Initial Release
