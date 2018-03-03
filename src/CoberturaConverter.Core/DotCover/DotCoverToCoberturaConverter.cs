using System;
using System.Linq;
using System.Collections.Generic;
using CoberturaConverter.Core.Cobertura;

namespace CoberturaConverter.Core.DotCover
{
    public class DotCoverToCoberturaConverter
    {
        private readonly DotCoverReport _report;
        private string _baseDirectory;
        private CoberturaReport _coberturaReport;

        public DotCoverToCoberturaConverter(DotCoverReport report)
        {
            _report = report ?? throw new ArgumentNullException(nameof(report));
        }

        public CoberturaReport ConvertToCobertura()
        {
            GetBaseDirectory();

            _coberturaReport = new CoberturaReport();

            _coberturaReport.BranchesCovered = 0;
            _coberturaReport.BranchesValid = 0;
            _coberturaReport.BranchRate = 0;

            _coberturaReport.Complexity = 0;

            _coberturaReport.LinesValid = _report.CoveredStatements;
            _coberturaReport.LineRate = _report.CoveragePercent * 0.01m;
            _coberturaReport.LinesCovered = _report.CoveredStatements;

            _coberturaReport.Sources = new List<string>
            {
                _baseDirectory
            };

            _coberturaReport.Timestamp = DateTime.UtcNow;
            _coberturaReport.Version = "0";

            _coberturaReport.Packages = _report.Assemblies?
                .Select(GetCoberturaPackage)
                .ToList();

            return _coberturaReport;
        }

        private void GetBaseDirectory()
        {
            if (_report.Files == null)
            {
                _baseDirectory = string.Empty;
            }
            else
            {
                var filePaths = _report.Files
                    .Select(f => f.Name);
                _baseDirectory = DotCoverBaseDirectoryFinder.GetBaseDirectory(filePaths);
            }
        }

        private CoberturaPackage GetCoberturaPackage(DotCoverAssembly dotCoverAssembly)
        {
            var coberturaPackage = new CoberturaPackage();

            coberturaPackage.Name = dotCoverAssembly.Name;
            coberturaPackage.BranchRate = 0;

            coberturaPackage.Complexity = 0;
            coberturaPackage.LineRate = dotCoverAssembly.CoveragePercent * 0.01m;

            coberturaPackage.Classes = GetCoberturaClassesForDotCoverAssembly(dotCoverAssembly);

            return coberturaPackage;
        }

        private List<CoberturaClass> GetCoberturaClassesForDotCoverAssembly(DotCoverAssembly dotCoverAssembly)
        {
            var coberturaClasses = new List<CoberturaClass>();

            foreach (var dotCoverNamespace in dotCoverAssembly.Namespaces)
            {
                var coberturaClassesInNamespace = dotCoverNamespace.Types
                    .Select(t => GetCoberturaClassFromDotCoverType(t, dotCoverNamespace.Name));
                coberturaClasses.AddRange(coberturaClassesInNamespace);
            }

            return coberturaClasses
                .Where(c => !string.IsNullOrWhiteSpace(c.FileName))
                .ToList();
        }

        private CoberturaClass GetCoberturaClassFromDotCoverType(DotCoverType dotCoverType, string namespaceName)
        {
            // TODO Partial classes spread over multiple files are not currently handled

            var coberturaClass = new CoberturaClass();

            coberturaClass.BranchRate = 0;
            coberturaClass.Complexity = 0;

            coberturaClass.FileName = GetRelativeFilenameForDotCoverType(dotCoverType);

            coberturaClass.LineRate = dotCoverType.CoveragePercent * 0.01m;
            coberturaClass.Name = namespaceName + "." + dotCoverType.Name;

            coberturaClass.Methods = dotCoverType.Methods
                .Select(GetCoberturaMethodForDotCoverType)
                .ToList();

            coberturaClass.Lines = coberturaClass.Methods
                .SelectMany(m => m.Lines)
                .OrderBy(l => l.Number)
                .ToList();

            return coberturaClass;
        }

        private string GetRelativeFilenameForDotCoverType(DotCoverType dotCoverType)
        {
            var fileIndex = dotCoverType.Methods
                .SelectMany(m => m.Statements)
                .Select(s => s.FileIndex)
                .FirstOrDefault();
            if (fileIndex == default) // Luckily, default int (0) is not used as file index by dotCover
            {
                if (dotCoverType.NestedTypes != null)
                {
                    foreach (var nestedType in dotCoverType.NestedTypes)
                    {
                        var nestedTypeFileName = GetRelativeFilenameForDotCoverType(nestedType);
                        if (nestedTypeFileName != null)
                        {
                            return nestedTypeFileName;
                        }
                    }
                }

                return null;
            }

            var file = _report.Files.First(f => f.Index == fileIndex);
            var fullFilePath = file.Name;

            var relativePart = fullFilePath
                .Substring(_baseDirectory.Length)
                .TrimStart('/')
                .TrimStart('\\');
            return relativePart;
        }

        private CoberturaMethod GetCoberturaMethodForDotCoverType(DotCoverMethod dotCoverMethod)
        {
            var coberturaMethod = new CoberturaMethod();

            coberturaMethod.BranchRate = 0;
            coberturaMethod.LineRate = dotCoverMethod.CoveragePercent * 0.01m;

            var methodSignature = DotCoverMethodInfoExtractor.GetMethodInfoFromDotCoverMethod(dotCoverMethod.Name);
            coberturaMethod.Name = methodSignature.name;
            coberturaMethod.Signature = methodSignature.signature;

            coberturaMethod.Lines = GetCoberturaLinesForDotCoverMethod(dotCoverMethod);

            return coberturaMethod;
        }

        private List<CoberturaLine> GetCoberturaLinesForDotCoverMethod(DotCoverMethod dotCoverMethod)
        {
            var lineInfo = new Dictionary<int, CoberturaLine>();

            var dotCoverStatements = dotCoverMethod.Statements;

            foreach (var dotCoverStatement in dotCoverStatements)
            {
                for (var i = dotCoverStatement.Line; i <= dotCoverStatement.EndLine; i++)
                {
                    if (lineInfo.ContainsKey(i))
                    {
                        if (dotCoverStatement.IsCovered)
                        {
                            var existingLine = lineInfo[i];
                            existingLine.Hits++;
                        }
                    }
                    else
                    {
                        var newLine = new CoberturaLine();

                        newLine.Hits = dotCoverStatement.IsCovered ? 1 : 0;
                        newLine.Number = i;
                        lineInfo.Add(i, newLine);
                    }
                }
            }

            var coberturaLines = lineInfo
                .Values
                .OrderBy(l => l.Number)
                .ToList();
            return coberturaLines;
        }
    }
}
