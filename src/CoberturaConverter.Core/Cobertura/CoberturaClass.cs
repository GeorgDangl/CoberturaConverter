using System.Collections.Generic;

namespace CoberturaConverter.Core.Cobertura
{
    public class CoberturaClass
    {
        public string Name { get; set; }
        public string FileName { get; set; }
        public decimal? LineRate { get; set; }
        public decimal? BranchRate { get; set; }
        public int? Complexity { get; set; }
        public List<CoberturaMethod> Methods { get; set; }
        public List<CoberturaLine> Lines { get; set; }
    }
}
