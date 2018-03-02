using System.Collections.Generic;

namespace CoberturaConverter.Core.Cobertura
{
    public class CoberturaMethod
    {
        public string Name { get; set; }
        public string Signature { get; set; }
        public decimal? LineRate { get; set; }
        public decimal? BranchRate { get; set; }
        public List<CoberturaLine> Lines { get; set; }
    }
}
