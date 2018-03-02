using CommandLine;

namespace CoberturaConverter.CommandLine
{
    public class CoberturaConverterOptions
    {
        [Option('i', "input", Required = true, HelpText = "Relative or absolute path to the input coverage report")]
        public string InputFilePath { get; set; }

        [Option('o', "output", Required = true, HelpText = "Relative or absolute path to the output Cobertura coverage report")]
        public string OutputFilePath { get; set; }

        [Option('s', "source", Required = true, HelpText = "The source report format. Values: DotCover, OpenCover")]
        public SourceReportFormat Target { get; set; }
    }
}
