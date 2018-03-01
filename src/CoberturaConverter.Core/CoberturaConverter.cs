using System.IO;
using CoberturaConverter.Core.OpenCover;

namespace CoberturaConverter.Core
{
    public class CoberturaConverter
    {
        public static Stream ConvertDotCoverToCobertura(Stream dotCoverDetailedXml)
        {
            var dotCoverParser = new DotCover.DotCoverParser(dotCoverDetailedXml);
            var dotCoverReport = dotCoverParser.ParseDotCoverReport();

            var coberturaConverter = new DotCover.DotCoverToCoberturaConverter(dotCoverReport);
            var coberturaReport = coberturaConverter.ConvertToCobertura();

            var coberturaXmlExporter = new Cobertura.XmlExporter(coberturaReport);
            var coberturaXDoc = coberturaXmlExporter.GetCoberturaDocument();

            var memStream = new MemoryStream();
            coberturaXDoc.Save(memStream);
            memStream.Position = 0;
            return memStream;
        }

        public static Stream ConvertOpenCoverToCobertura(Stream openCoverXml)
        {
            return OpenCoverCoberturaConverter.ConvertToCobertura(openCoverXml);
        }
    }
}
