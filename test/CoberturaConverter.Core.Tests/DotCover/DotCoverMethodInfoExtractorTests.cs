using System;
using CoberturaConverter.Core.DotCover;
using Xunit;

namespace CoberturaConverter.Core.Tests.DotCover
{
    public class DotCoverMethodInfoExtractorTests
    {
        [Fact]
        public void ArgumentNullExceptionForNullInput()
        {
            Assert.Throws<ArgumentNullException>("dotCoverMethodName", () => DotCoverMethodInfoExtractor.GetMethodInfoFromDotCoverMethod(null));
        }

        [Theory]
        [InlineData(".ctor():System.Void", ".ctor", "()")]
        [InlineData("get_ErrorMessage():System.String", "get_ErrorMessage", "()")]
        [InlineData("Calculate(System.String):Dangl.Calculator.CalculationResult", "Calculate", "(System.String)")]
        public void ExtractSignatures(string dotCoverMethodName, string expectedName, string expectedSignature)
        {
            var actual = DotCoverMethodInfoExtractor.GetMethodInfoFromDotCoverMethod(dotCoverMethodName);
            Assert.Equal(expectedName, actual.name);
            Assert.Equal(expectedSignature, actual.signature);
        }
    }
}
