using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace CoberturaConverter.Core.Cobertura
{
    public class XmlExporter
    {
        private readonly CoberturaReport _report;

        public XmlExporter(CoberturaReport report)
        {
            _report = report ?? throw new ArgumentNullException(nameof(report));
        }

        public XDocument GetCoberturaDocument()
        {
            var xDoc = new XDocument();

            xDoc.Declaration = new XDeclaration("1.0", "utf-8", null);

            var root = GetRootElement();
            xDoc.Add(root);
            return xDoc;
        }

        private XElement GetRootElement()
        {
            var coverageElement = new XElement("coverage");

            coverageElement.AddAttributeIfItHasValue("line-rate", _report.LineRate);
            coverageElement.AddAttributeIfItHasValue("branch-rate", _report.BranchRate);
            coverageElement.AddAttributeIfItHasValue("lines-covered", _report.LinesCovered);
            coverageElement.AddAttributeIfItHasValue("lines-valid", _report.LinesValid);
            coverageElement.AddAttributeIfItHasValue("branches-covered", _report.BranchesCovered);
            coverageElement.AddAttributeIfItHasValue("branches-valid", _report.BranchesValid);
            coverageElement.AddAttributeIfItHasValue("complexity", _report.Complexity);
            coverageElement.AddAttributeIfItHasValue("version", _report.Version);
            coverageElement.AddAttributeIfItHasValue("timestamp", _report.Timestamp);

            AddSourcesIfPresent(coverageElement);

            AddPackagesIfPresent(coverageElement);

            return coverageElement;
        }

        private void AddSourcesIfPresent(XElement root)
        {
            if (_report.Sources?.Any() == true)
            {
                var sourcesElement = new XElement("sources");
                foreach (var source in _report.Sources)
                {
                    sourcesElement.Add(new XElement("source", source));
                }

                root.Add(sourcesElement);
            }
        }

        private void AddPackagesIfPresent(XElement root)
        {
            if (_report.Packages?.Any() == true)
            {
                var sourcesElement = new XElement("packages");
                foreach (var package in _report.Packages)
                {
                    var packageElement = GetPackageElement(package);
                    sourcesElement.Add(packageElement);
                }

                root.Add(sourcesElement);
            }
        }

        private XElement GetPackageElement(CoberturaPackage package)
        {
            var packageElement = new XElement("package");

            packageElement.AddAttributeIfItHasValue("name", package.Name);
            packageElement.AddAttributeIfItHasValue("line-rate", package.LineRate);
            packageElement.AddAttributeIfItHasValue("branch-rate", package.BranchRate);
            packageElement.AddAttributeIfItHasValue("complexity", package.Complexity);

            if (package.Classes?.Any() == true)
            {
                var classesElement = GetClassesElement(package);
                packageElement.Add(classesElement);
            }

            return packageElement;
        }

        private XElement GetClassesElement(CoberturaPackage package)
        {
            var classesElement = new XElement("classes");

            foreach (var @class in package.Classes)
            {
                var classElement = GetClassElement(@class);
                classesElement.Add(classElement);
            }

            return classesElement;
        }

        private XElement GetClassElement(CoberturaClass coberturaClass)
        {
            var classElement = new XElement("class");

            classElement.AddAttributeIfItHasValue("name", coberturaClass.Name);
            classElement.AddAttributeIfItHasValue("filename", coberturaClass.FileName);
            classElement.AddAttributeIfItHasValue("line-rate", coberturaClass.LineRate);
            classElement.AddAttributeIfItHasValue("branch-rate", coberturaClass.BranchRate);
            classElement.AddAttributeIfItHasValue("complexity", coberturaClass.Complexity);

            if (coberturaClass.Methods?.Any() == true)
            {
                var methodsElement = GetMethodsElement(coberturaClass);
                classElement.Add(methodsElement);
            }

            if (coberturaClass.Lines?.Any() == true)
            {
                var linesElement = GetLinesElement(coberturaClass.Lines);
                classElement.Add(linesElement);
            }

            return classElement;
        }

        private XElement GetMethodsElement(CoberturaClass coberturaClass)
        {
            var methodsElement = new XElement("methods");

            foreach (var method in coberturaClass.Methods)
            {
                var methodElement = new XElement("method");

                methodElement.AddAttributeIfItHasValue("name", method.Name);
                methodElement.AddAttributeIfItHasValue("signature", method.Signature);
                methodElement.AddAttributeIfItHasValue("line-rate", method.LineRate);
                methodElement.AddAttributeIfItHasValue("branch-rate", method.BranchRate);

                if (method.Lines?.Any() == true)
                {
                    var linesElement = GetLinesElement(method.Lines);
                    methodElement.Add(linesElement);
                }

                methodsElement.Add(methodElement);
            }

            return methodsElement;
        }

        private XElement GetLinesElement(List<CoberturaLine> lines)
        {
            var linesElement = new XElement("lines");

            foreach (var line in lines)
            {
                var lineElement = new XElement("line");

                lineElement.AddAttributeIfItHasValue("number", line.Number);
                lineElement.AddAttributeIfItHasValue("hits", line.Hits);
                lineElement.AddAttributeIfItHasValue("branch", line.IsBranch);

                linesElement.Add(lineElement);
            }

            return linesElement;
        }
    }
}
