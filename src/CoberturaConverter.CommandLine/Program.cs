using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using static CoberturaConverter.Core.CoberturaConverter;

namespace CoberturaConverter.CommandLine
{
    class Program
    {
        static async Task Main(string[] args)
        {
            HeadingInfo.Default.WriteMessage("Based on https://github.com/danielpalme/OpenCoverToCoberturaConverter");
            HeadingInfo.Default.WriteMessage("Find out more at github.com/GeorgDangl/CoberturaConverter");
            var parsedOptions = Parser.Default.ParseArguments<CoberturaConverterOptions>(args);
            if (parsedOptions.Tag == ParserResultType.Parsed)
            {
                var converterOptions = (Parsed<CoberturaConverterOptions>) parsedOptions;
                try
                {
                    await PerformConversion(converterOptions.Value);
                    Console.WriteLine("Conversion finished");
                }
                catch (Exception e)
                {
                    DisplayExceptionDetails(e);
                }
            }
        }

        private static void DisplayExceptionDetails(Exception e)
        {
            Console.Write(e.ToString());
            Console.WriteLine();
        }

        private static Task PerformConversion(CoberturaConverterOptions options)
        {
            switch (options.Target)
            {
                case SourceReportFormat.DotCover:
                    return DotCoverToCobertura(options);
                case SourceReportFormat.OpenCover:
                    return OpenCoverToCobertura(options);
                default:
                    throw new NotImplementedException("The specified source report format is not currently implemented");
            }
        }

        private static async Task OpenCoverToCobertura(CoberturaConverterOptions options)
        {
            using (var inputStream = File.OpenRead(options.InputFilePath))
            {
                using (var coberturaStream = ConvertOpenCoverToCobertura(inputStream))
                {
                    using (var outputStream = File.Create(options.OutputFilePath))
                    {
                        await coberturaStream.CopyToAsync(outputStream);
                    }
                }
            }
        }

        private static async Task DotCoverToCobertura(CoberturaConverterOptions options)
        {
            using (var inputStream = File.OpenRead(options.InputFilePath))
            {
                using (var coberturaStream = ConvertDotCoverToCobertura(inputStream))
                {
                    using (var outputStream = File.Create(options.OutputFilePath))
                    {
                        await coberturaStream.CopyToAsync(outputStream);
                    }
                }
            }
        }
    }
}
