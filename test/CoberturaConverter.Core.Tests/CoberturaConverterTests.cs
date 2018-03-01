using System.Xml.Linq;
using DotCoverConverter.Core.Tests;
using Xunit;

namespace CoberturaConverter.Core.Tests
{
    public class CoberturaConverterTests
    {
        [Theory]
        [InlineData(TestFile.DanglCalculatorDotCover)]
        [InlineData(TestFile.DanglCommonDotCover)]
        [InlineData(TestFile.LightQueryDotCover)]
        public void CanConvertFromDotCover(TestFile dotCoverTestFile)
        {
            using (var xmlStream = TestFilesFactory.GetTestFileStream(dotCoverTestFile))
            {
                var conversionResult = CoberturaConverter.ConvertDotCoverToCobertura(xmlStream);
                Assert.NotNull(xmlStream);
                Assert.True(xmlStream.Length > 0);
                // Checking if it's valid XML output,
                // otherwise the Load() method should throw
                XDocument.Load(conversionResult);
            }
        }

        [Theory]
        [InlineData(TestFile.DanglCalculatorOpenCover)]
        [InlineData(TestFile.DanglCommonOpenCover)]
        [InlineData(TestFile.LightQueryOpenCover)]
        public void CanConvertFromOpenCover(TestFile openCoverTestFile)
        {
            using (var xmlStream = TestFilesFactory.GetTestFileStream(openCoverTestFile))
            {
                var conversionResult = CoberturaConverter.ConvertOpenCoverToCobertura(xmlStream);
                Assert.NotNull(xmlStream);
                Assert.True(xmlStream.Length > 0);
                // Checking if it's valid XML output,
                // otherwise the Load() method should throw
                XDocument.Load(conversionResult);
            }
        }
    }
}
