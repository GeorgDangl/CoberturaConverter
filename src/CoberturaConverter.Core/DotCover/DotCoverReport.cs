using System.Collections.Generic;

namespace CoberturaConverter.Core.DotCover
{
    public class DotCoverReport
    {
        public int CoveredStatements { get; set; }
        public int TotalStatements { get; set; }
        public decimal CoveragePercent { get; set; }
        public string ReportType { get; set; }
        public string DotCoverVersion { get; set; }
        public List<DotCoverFileIndex> Files { get; set; }
        public List<DotCoverAssembly> Assemblies { get; set; }
    }
}
