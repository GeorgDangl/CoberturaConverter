using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace CoberturaConverter.Core.DotCover
{
    public class DotCoverParser
    {
        private readonly Stream _xmlStream;
        private XDocument _xDoc;
        private XElement _root;

        public DotCoverParser(Stream xmlStream)
        {
            _xmlStream = xmlStream ?? throw new ArgumentNullException(nameof(xmlStream));
        }

        public DotCoverReport ParseDotCoverReport()
        {
            _xDoc = XDocument.Load(_xmlStream);

            if (_xDoc.Root.Name.LocalName != "Root")
            {
                throw new ArgumentException("The DotCover report is expected to have a root element with name \"Root\".");
            }

            _root = _xDoc.Root;

            var report = new DotCoverReport();
            report.CoveragePercent = _root.GetAttributeDecimalValue("CoveragePercent");
            report.CoveredStatements = _root.GetAttributeIntValue("CoveredStatements");
            report.DotCoverVersion = _root.GetAttributeStringValue("DotCoverVersion");
            report.ReportType = _root.GetAttributeStringValue("ReportType");
            report.TotalStatements = _root.GetAttributeIntValue("TotalStatements");

            report.Assemblies = GetAssemblies();
            report.Files = GetFileIndices();

            return report;
        }

        private List<DotCoverFileIndex> GetFileIndices()
        {
            var fileIndices = _xDoc.Descendants()
                .Where(d => d.Name.LocalName == "File" && d.Parent?.Name.LocalName == "FileIndices");
            var dotCoverFiles = fileIndices.Select(fileElement => new DotCoverFileIndex
                {
                    Name = fileElement.GetAttributeStringValue("Name"),
                    Checksum = fileElement.GetAttributeStringValue("Checksum"),
                    ChecksumAlgorithm = fileElement.GetAttributeStringValue("ChecksumAlgorithm"),
                    Index = fileElement.GetAttributeIntValue("Index"),
                })
                .ToList();
            return dotCoverFiles;
        }

        private List<DotCoverAssembly> GetAssemblies()
        {
            var assemblyElements = _xDoc.Root.Elements()
                .Where(d => d.Name.LocalName == "Assembly")
                .Select(GetAssemblyFromElement)
                .ToList();
            return assemblyElements;
        }

        private DotCoverAssembly GetAssemblyFromElement(XElement assemblyElement)
        {
            var dotCoverAssembly = new DotCoverAssembly
            {
                CoveragePercent = assemblyElement.GetAttributeDecimalValue("CoveragePercent"),
                CoveredStatements = assemblyElement.GetAttributeIntValue("CoveredStatements"),
                TotalStatements = assemblyElement.GetAttributeIntValue("TotalStatements"),
                Name = assemblyElement.GetAttributeStringValue("Name"),
                Namespaces = assemblyElement.Elements()
                    .Where(e => e.Name.LocalName == "Namespace")
                    .Select(GetNamespaceFromElement)
                    .ToList()
            };
            return dotCoverAssembly;
        }

        private DotCoverNamespace GetNamespaceFromElement(XElement namespaceElement)
        {
            var dotCoverNamespace = new DotCoverNamespace
            {
                CoveragePercent = namespaceElement.GetAttributeDecimalValue("CoveragePercent"),
                CoveredStatements = namespaceElement.GetAttributeIntValue("CoveredStatements"),
                TotalStatements = namespaceElement.GetAttributeIntValue("TotalStatements"),
                Name = namespaceElement.GetAttributeStringValue("Name"),
                Types = namespaceElement.Elements()
                    .Where(e => e.Name.LocalName == "Type")
                    .Select(GetTypeFromElement)
                    .ToList()
            };
            return dotCoverNamespace;
        }

        private DotCoverType GetTypeFromElement(XElement typeElement)
        {
            var dotCoverType = new DotCoverType
            {
                CoveragePercent = typeElement.GetAttributeDecimalValue("CoveragePercent"),
                CoveredStatements = typeElement.GetAttributeIntValue("CoveredStatements"),
                TotalStatements = typeElement.GetAttributeIntValue("TotalStatements"),
                Name = typeElement.GetAttributeStringValue("Name"),
                Methods = typeElement.Elements()
                    .Where(e => e.Name.LocalName == "Method")
                    .Select(GetMethodFromElement)
                    .ToList()
            };
            return dotCoverType;
        }

        private DotCoverMethod GetMethodFromElement(XElement methodElement)
        {
            var dotCoverMethod = new DotCoverMethod
            {
                CoveragePercent = methodElement.GetAttributeDecimalValue("CoveragePercent"),
                CoveredStatements = methodElement.GetAttributeIntValue("CoveredStatements"),
                TotalStatements = methodElement.GetAttributeIntValue("TotalStatements"),
                Name = methodElement.GetAttributeStringValue("Name"),
                Statements = methodElement.Elements()
                    .Where(e => e.Name.LocalName == "Statement")
                    .Select(GetStatementFromElement)
                    .ToList()
            };
            return dotCoverMethod;
        }

        private DotCoverStatement GetStatementFromElement(XElement statementElement)
        {
            var dotCoverStatement = new DotCoverStatement
            {
                Column = statementElement.GetAttributeIntValue("Column"),
                EndColumn = statementElement.GetAttributeIntValue("EndColumn"),
                EndLine = statementElement.GetAttributeIntValue("EndLine"),
                FileIndex = statementElement.GetAttributeIntValue("FileIndex"),
                IsCovered = statementElement.GetAttributeBooleanValue("Covered"),
                Line = statementElement.GetAttributeIntValue("Line")
            };
            return dotCoverStatement;
        }
    }
}
