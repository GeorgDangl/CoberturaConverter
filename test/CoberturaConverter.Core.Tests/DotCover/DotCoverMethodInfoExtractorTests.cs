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
        [InlineData("get_ErrorMessage()", "get_ErrorMessage", "()")]
        [InlineData("get_ErrorMessage", "get_ErrorMessage", "")]
        [InlineData("Calculate(System.String):Dangl.Calculator.CalculationResult", "Calculate", "(System.String)")]
        [InlineData("Attribute<MyCompany::Simplifiable>():Simplifiable", "Attribute<MyCompany::Simplifiable>", "()")]
        [InlineData("MethodName(Foo::Bar):void", "MethodName", "(Foo::Bar)")]
        [InlineData("MethodName(Foo::Bar, Fizz::Buzz):void", "MethodName", "(Foo::Bar, Fizz::Buzz)")]
        public void ExtractSignatures(string dotCoverMethodName, string expectedName, string expectedSignature)
        {
            var actual = DotCoverMethodInfoExtractor.GetMethodInfoFromDotCoverMethod(dotCoverMethodName);
            Assert.Equal(expectedName, actual.name);
            Assert.Equal(expectedSignature, actual.signature);
        }
    }
}
