using System;
using CoberturaConverter.Core.DotCover;
using DotCoverConverter.Core.Tests;
using Xunit;

namespace CoberturaConverter.Core.Tests.DotCover
{
    public class DotCoverToCoberturaConverterTests
    {
        [Fact]
        public void ArgumentNullExceptionForNullStream()
        {
            Assert.Throws<ArgumentNullException>("report", () => new DotCoverToCoberturaConverter(null));
        }

        [Fact]
        public void CanConvertEmpty()
        {
            var converter = new DotCoverToCoberturaConverter(new DotCoverReport());
            var conversionResult = converter.ConvertToCobertura();
            Assert.NotNull(conversionResult);
        }

        [Theory]
        [InlineData(TestFile.DanglCalculatorDotCover)]
        [InlineData(TestFile.DanglCommonDotCover)]
        [InlineData(TestFile.LightQueryDotCover)]
        public void CanConvertDotCoverFiles(TestFile dotCoverTestFile)
        {
            using (var xmlStream = TestFilesFactory.GetTestFileStream(dotCoverTestFile))
            {
                var parser = new DotCoverParser(xmlStream);
                var dotCoverReport = parser.ParseDotCoverReport();

                var converter = new DotCoverToCoberturaConverter(dotCoverReport);
                var conversionResult = converter.ConvertToCobertura();
                Assert.NotNull(conversionResult);
                Assert.NotEmpty(conversionResult.Packages);
            }
        }
    }
}
