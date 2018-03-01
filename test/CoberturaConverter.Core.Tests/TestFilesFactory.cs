using System;
using System.IO;
using System.Reflection;
using DotCoverConverter.Core.Tests;

namespace CoberturaConverter.Core.Tests
{
    public static class TestFilesFactory
    {
        public static Stream GetTestFileStream(TestFile testFile)
        {
            string fileName;
            switch (testFile)
            {
                case TestFile.DanglCalculatorCobertura:
                    fileName = "Dangl.Calculator.Cobertura.xml";
                    break;
                case TestFile.DanglCalculatorDotCover:
                    fileName = "Dangl.Calculator.DotCover.xml";
                    break;
                case TestFile.DanglCalculatorOpenCover:
                    fileName = "Dangl.Calculator.OpenCover.xml";
                    break;
                case TestFile.DanglCommonCobertura:
                    fileName = "Dangl.Common.Cobertura.xml";
                    break;
                case TestFile.DanglCommonDotCover:
                    fileName = "Dangl.Common.DotCover.xml";
                    break;
                case TestFile.DanglCommonOpenCover:
                    fileName = "Dangl.Common.OpenCover.xml";
                    break;
                case TestFile.LightQueryCobertura:
                    fileName = "LightQuery.Cobertura.xml";
                    break;
                case TestFile.LightQueryDotCover:
                    fileName = "LightQuery.DotCover.xml";
                    break;
                case TestFile.LightQueryOpenCover:
                    fileName = "LightQuery.OpenCover.xml";
                    break;

                default:
                    throw new NotImplementedException();
            }

            return GetResourceStream(fileName);
        }

        private static Stream GetResourceStream(string fileName)
        {
            var resourceName = "CoberturaConverter.Core.Tests.Resources." + fileName;
            var assembly = typeof(TestFilesFactory).GetTypeInfo().Assembly;
            var resourceStream = assembly.GetManifestResourceStream(resourceName);
            return resourceStream;
        }

        /* // TODO Decide if the BaseDirFinder does work in practice
        /// <summary>
        /// dotCover only saves absolute file paths while Cobertura expects a root directory
        /// and relative file paths. Since this can not be accurately determined in case there are
        /// multiple base paths
        /// </summary>
        /// <param name="dotCoverTestFile"></param>
        /// <returns></returns>
        public static string GetBaseDirForDotCoverReport(TestFile dotCoverTestFile)
        {
            string baseDir;
            switch (dotCoverTestFile)
            {
                case TestFile.DanglCalculatorDotCover:
                    baseDir = "D:\\Visual Studio Projects\\Dangl.Calculator";
                    break;
                case TestFile.DanglCommonDotCover:
                    baseDir = "D:\\Visual Studio Projects\\Dangl.Common";
                    break;
                case TestFile.LightQueryDotCover:
                    baseDir = "D:\\Visual Studio Projects\\LightQuery";
                    break;
                default:
                    throw new NotImplementedException();
            }

            return baseDir;
        }
        */
    }
}
