using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using CoberturaConverter.Core.Cobertura;
using Xunit;

namespace CoberturaConverter.Core.Tests.Cobertura
{
    public class XmlExporterTests
    {
        [Fact]
        public void ArgumentNullExceptionForNullReport()
        {
            Assert.Throws<ArgumentNullException>("report", () => new XmlExporter(null));
        }

        [Fact]
        public void GeneratesEmptyXDocumentForEmptyReport()
        {
            var exporter = new XmlExporter(new CoberturaReport());
            var xDoc = exporter.GetCoberturaDocument();
            Assert.NotNull(xDoc);
            Assert.EndsWith("<coverage />", xDoc.ToString());
        }

        [Fact]
        public void HasXmlSignatureInEmptyReport()
        {
            var exporter = new XmlExporter(new CoberturaReport());
            var xDoc = exporter.GetCoberturaDocument();
            using (var memStream = new MemoryStream())
            {
                xDoc.Save(memStream);
                memStream.Position = 0;
                using (var streamReader = new StreamReader(memStream))
                {
                    var xml = streamReader.ReadToEnd();
                    Assert.Contains("<?xml version=\"1.0\" encoding=\"utf-8\"?>", xml);
                }
            }
        }

        [Fact]
        public void GeneratesCorrectXDocumentForReport()
        {
            var report = GenerateReport();
            var exporter = new XmlExporter(report);
            var actualDoc = exporter.GetCoberturaDocument();
            var expectedDoc = GetExpectedXDocument();
            var comparator = new XDocumentComparator(expectedDoc, actualDoc);
            comparator.AssertXDocumentsAreEqual();
        }

        private CoberturaReport GenerateReport()
        {
            var report = new CoberturaReport
            {
                LineRate = 1,
                BranchRate = 0.958333333333333m,
                LinesCovered = 567,
                LinesValid = 567,
                BranchesCovered = 69,
                BranchesValid = 72,
                Complexity = 0,
                Version = "0",
                Timestamp = new DateTime(2018, 3, 1, 14, 30, 30, DateTimeKind.Utc),
                Sources = new List<string> {"D:\\Visual Studio Projects\\Dangl.Calculator"},
                Packages = new List<CoberturaPackage>
                {
                    new CoberturaPackage
                    {
                        Name = "Dangl.Calculator",
                        LineRate = 1,
                        BranchRate = 0.958333333333333m,
                        Complexity = 0,
                        Classes = new List<CoberturaClass>
                        {
                            new CoberturaClass
                            {
                                Name = "Dangl.Calculator.CalculationResult",
                                FileName = "src\\Dangl.Calculator\\CalculationResult.cs",
                                LineRate = 1,
                                BranchRate = 1,
                                Complexity = 0,
                                Lines = new List<CoberturaLine>
                                {
                                    new CoberturaLine {Number = 17, Hits = 161, IsBranch = false}
                                },
                                Methods = new List<CoberturaMethod>
                                {
                                    new CoberturaMethod
                                    {
                                        Name = ".ctor",
                                        Signature = "()",
                                        LineRate = 1,
                                        BranchRate = 1,
                                        Lines = new List<CoberturaLine>
                                        {
                                            new CoberturaLine {Number = 17, Hits = 161, IsBranch = false}
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
            return report;
        }

        private XDocument GetExpectedXDocument()
        {
            var xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE coverage SYSTEM ""http://cobertura.sourceforge.net/xml/coverage-04.dtd"">
<coverage line-rate=""1"" branch-rate=""0.958333333333333"" lines-covered=""567"" lines-valid=""567"" branches-covered=""69"" branches-valid=""72"" complexity=""0"" version=""0"" timestamp=""1519914630"">
  <sources>
    <source>D:\Visual Studio Projects\Dangl.Calculator</source>
  </sources>
  <packages>
    <package name=""Dangl.Calculator"" line-rate=""1"" branch-rate=""0.958333333333333"" complexity=""0"">
      <classes>
        <class name=""Dangl.Calculator.CalculationResult"" filename=""src\Dangl.Calculator\CalculationResult.cs"" line-rate=""1"" branch-rate=""1"" complexity=""0"">
          <methods>
            <method name="".ctor"" signature=""()"" line-rate=""1"" branch-rate=""1"">
              <lines>
                <line number=""17"" hits=""161"" branch=""false"" />
              </lines>
            </method>
          </methods>
          <lines>
            <line number=""17"" hits=""161"" branch=""false"" />
          </lines>
        </class>
      </classes>
    </package>
 </packages>
</coverage>";
            var xDoc = XDocument.Parse(xml);
            return xDoc;
        }
    }
}
