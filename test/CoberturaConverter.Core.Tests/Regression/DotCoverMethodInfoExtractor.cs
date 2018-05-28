using System;
using System.Collections.Generic;
using System.Text;
using CoberturaConverter.Core.DotCover;
using Xunit;

namespace CoberturaConverter.Core.Tests.Regression
{
    public class DotCoverMethodInfoExtractor
    {
        [Fact]
        public void CanRead()
        {
            using (var xmlStream = TestFilesFactory.GetTestFileStream(DotCoverConverter.Core.Tests.TestFile.RegressionDotCoverMethodInfoExtractor))
            {
                var parser = new DotCoverParser(xmlStream);
                var dotCoverReport = parser.ParseDotCoverReport();
                Assert.NotNull(dotCoverReport);
                Assert.NotEmpty(dotCoverReport.Assemblies);
            }
        }
    }
}
