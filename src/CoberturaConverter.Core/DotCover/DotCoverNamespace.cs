using System.Collections.Generic;

namespace CoberturaConverter.Core.DotCover
{
    public class DotCoverNamespace
    {
        public string Name { get; set; }
        public int CoveredStatements { get; set; }
        public int TotalStatements { get; set; }
        public decimal CoveragePercent { get; set; }
        public List<DotCoverType> Types { get; set; }
    }
}
