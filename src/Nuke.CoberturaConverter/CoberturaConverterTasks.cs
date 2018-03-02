using System.IO;
using System.Threading.Tasks;
using Nuke.Core.Tooling;
using static CoberturaConverter.Core.CoberturaConverter;

namespace Nuke.CoberturaConverter
{
    public class CoberturaConverterTasks
    {
        public static async Task OpenCoverToCobertura(Configure<OpenCoverConversionSettings> settingsConfigurator)
        {
            var settings = settingsConfigurator(new OpenCoverConversionSettings());
            using (var inputStream = File.OpenRead(settings.InputFile))
            {
                using (var coberturaStream = ConvertOpenCoverToCobertura(inputStream))
                {
                    using (var outputStream = File.Create(settings.OutputFile))
                    {
                        await coberturaStream.CopyToAsync(outputStream);
                    }
                }
            }
        }

        public static async Task DotCoverToCobertura(Configure<DotCoverConversionSettings> settingsConfigurator)
        {
            var settings = settingsConfigurator(new DotCoverConversionSettings());
            using (var inputStream = File.OpenRead(settings.InputFile))
            {
                using (var coberturaStream = ConvertDotCoverToCobertura(inputStream))
                {
                    using (var outputStream = File.Create(settings.OutputFile))
                    {
                        await coberturaStream.CopyToAsync(outputStream);
                    }
                }
            }
        }
    }
}
