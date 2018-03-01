using System;
using System.Collections.Generic;
using CoberturaConverter.Core.DotCover;
using Xunit;

namespace CoberturaConverter.Core.Tests.DotCover
{
    public class DotCoverBaseDirectoryFinderTests
    {
        [Fact]
        public void ArgumentNullExceptionForNullInput()
        {
            Assert.Throws<ArgumentNullException>("filePaths", () => DotCoverBaseDirectoryFinder.GetBaseDirectory(null));
        }

        [Theory]
        [MemberData(nameof(BaseDirectoriesTestData))]
        public void FindBaseDirectories(string expectedBaseDirectory, string[] filePaths)
        {
            var actualBaseDirectory = DotCoverBaseDirectoryFinder.GetBaseDirectory(filePaths);
            Assert.Equal(expectedBaseDirectory, actualBaseDirectory);
        }

        public static List<object[]> BaseDirectoriesTestData => new List<object[]>
        {
            new object[]
            {
                "D:\\Visual Studio Projects\\Dangl.Calculator\\src\\Dangl.Calculator", new[]
                {
                    "D:\\Visual Studio Projects\\Dangl.Calculator\\src\\Dangl.Calculator\\CalculationResult.cs",
                    "D:\\Visual Studio Projects\\Dangl.Calculator\\src\\Dangl.Calculator\\CalculatorErrorListener.cs",
                    "D:\\Visual Studio Projects\\Dangl.Calculator\\src\\Dangl.Calculator\\obj\\Debug\\net45\\CalculatorParser.cs",
                    "D:\\Visual Studio Projects\\Dangl.Calculator\\src\\Dangl.Calculator\\obj\\Debug\\netstandard1.1\\CalculatorParser.cs"
                }
            },
            new object[]
            {
                "D:\\Visual Studio Projects\\Dangl.Common\\src\\Dangl.Common", new[]
                {
                    "D:\\Visual Studio Projects\\Dangl.Common\\src\\Dangl.Common\\StringEncryptionExtensions.cs"
                }
            },
            new object[]
            {
                string.Empty, new[]
                {
                    "D:\\Visual Studio Projects\\Dangl.Common\\src\\Dangl.Common\\StringEncryptionExtensions.cs",
                    "C:\\Visual Studio Projects\\Dangl.Common\\src\\Dangl.Common\\StringEncryptionExtensions.cs"
                }
            },
            new object[]
            {
                string.Empty, new string[0]
            }
        };
    }
}
