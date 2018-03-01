using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using CoberturaConverter.Core.DotCover;
using DotCoverConverter.Core.Tests;
using Xunit;

namespace CoberturaConverter.Core.Tests.DotCover
{
    public class DotCoverParserTests
    {
        [Fact]
        public void ArgumentNullExceptionForNullStream()
        {
            Assert.Throws<ArgumentNullException>("xmlStream", () => new DotCoverParser(null));
        }

        [Fact]
        public void XmlExceptionForRandomInputStream()
        {
            var random = new Random();
            var bytes = new byte[1024];
            random.NextBytes(bytes);
            var memStream = new MemoryStream(bytes);
            var parser = new DotCoverParser(memStream);
            Assert.Throws<System.Xml.XmlException>(() => parser.ParseDotCoverReport());
        }

        [Fact]
        public void ArgumentExceptionForNonDotCoverReport()
        {
            using (var memStream = new MemoryStream())
            {
                var xDoc = new XDocument();
                xDoc.Declaration = new XDeclaration("1.0", "utf-8", "true");
                xDoc.Add(new XElement("NotWhatYouExpected"));
                xDoc.Save(memStream);
                memStream.Position = 0;
                var parser = new DotCoverParser(memStream);
                Assert.Throws<ArgumentException>(() => parser.ParseDotCoverReport());
            }
        }

        [Theory]
        [InlineData(TestFile.DanglCalculatorDotCover)]
        [InlineData(TestFile.DanglCommonDotCover)]
        [InlineData(TestFile.LightQueryDotCover)]
        public void CanParseDotCoverFiles(TestFile dotCoverTestFile)
        {
            using (var xmlStream = TestFilesFactory.GetTestFileStream(dotCoverTestFile))
            {
                var parser = new DotCoverParser(xmlStream);
                var dotCoverReport = parser.ParseDotCoverReport();
                Assert.NotNull(dotCoverReport);
                Assert.NotEmpty(dotCoverReport.Assemblies);
            }
        }

        [Fact]
        public void ParsesFileCorrect()
        {
            var testFile = TestFile.DanglCalculatorDotCover;
            using (var xmlStream = TestFilesFactory.GetTestFileStream(testFile))
            {
                var parser = new DotCoverParser(xmlStream);
                var dotCoverReport = parser.ParseDotCoverReport();

                Assert.Equal(6, dotCoverReport.Files.Count);
                Assert.Equal(2, dotCoverReport.Assemblies.Count);

                Assert.Equal("Dangl.Calculator", dotCoverReport.Assemblies.First().Name);
                Assert.Equal(203, dotCoverReport.Assemblies.First().CoveredStatements);
                Assert.Single(dotCoverReport.Assemblies.First().Namespaces);
            }
        }
    }
}
