using System.IO;
using System.Threading.Tasks;
using static CoberturaConverter.Core.CoberturaConverter;

namespace Nuke.CoberturaConverter
{
    public class CoberturaConverterTasks
    {
        public static async Task OpenCoverToCobertura(OpenCoverConversionSettings settings)
        {
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

        public static async Task DotCoverToCobertura(DotCoverConversionSettings settings)
        {
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
