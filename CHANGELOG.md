# Changelog

All notable changes to **CoberturaConverter** are documented here.

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
