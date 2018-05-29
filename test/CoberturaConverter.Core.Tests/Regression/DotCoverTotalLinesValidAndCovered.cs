using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace CoberturaConverter.Core.Tests.Regression
{
    public class DotCoverTotalLinesValidAndCovered
    {
        // Issue #2 on GitHub reported the error that the CoberturaReport
        // used "CoveredStatements" instead of "TotalStatements" for the
        // "LinesValid" property in the destination Cobertura report

        [Fact]
        public void UseTotalStatementsForLinesValidInCobertura()
        {
            using (var xmlStream = TestFilesFactory.GetTestFileStream(DotCoverConverter.Core.Tests.TestFile.DanglCommonDotCover))
            {
                using (var convertedCoberturaStream = CoberturaConverter.ConvertDotCoverToCobertura(xmlStream))
                {
                    var coberturaXml = XDocument.Load(convertedCoberturaStream);

                    var actualLinesValid = coberturaXml.Descendants()
                        .Where(d => d.Name.LocalName == "coverage")
                        .Select(d => d.Attribute("lines-valid"))
                        .Select(a => a.Value)
                        .Single();

                    var expected = "500"; // 500 TotalStatements in dotCover report
                    Assert.Equal(expected, actualLinesValid);
                }
            }
        }

        [Fact]
        public void UseCoveredStatementsForLinesCoveredInCobertura()
        {
            using (var xmlStream = TestFilesFactory.GetTestFileStream(DotCoverConverter.Core.Tests.TestFile.DanglCommonDotCover))
            {
                using (var convertedCoberturaStream = CoberturaConverter.ConvertDotCoverToCobertura(xmlStream))
                {
                    var coberturaXml = XDocument.Load(convertedCoberturaStream);

                    var actualLinesCovered = coberturaXml.Descendants()
                        .Where(d => d.Name.LocalName == "coverage")
                        .Select(d => d.Attribute("lines-covered"))
                        .Select(a => a.Value)
                        .Single();

                    var expected = "492"; // 500 TotalStatements in dotCover report
                    Assert.Equal(expected, actualLinesCovered);
                }
            }
        }
    }
}
