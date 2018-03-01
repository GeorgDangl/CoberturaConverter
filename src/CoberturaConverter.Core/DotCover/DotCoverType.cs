using System.Collections.Generic;

namespace CoberturaConverter.Core.DotCover
{
    public class DotCoverType
    {
        public string Name { get; set; }
        public int CoveredStatements { get; set; }
        public int TotalStatements { get; set; }
        public decimal CoveragePercent { get; set; }
        public List<DotCoverMethod> Methods { get; set; }
    }
}
