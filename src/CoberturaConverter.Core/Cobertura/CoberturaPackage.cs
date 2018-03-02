using System.Collections.Generic;

namespace CoberturaConverter.Core.Cobertura
{
    public class CoberturaPackage
    {
        public string Name { get; set; }
        public decimal? LineRate { get; set; }
        public decimal? BranchRate { get; set; }
        public int? Complexity { get; set; }
        public List<CoberturaClass> Classes { get; set; }
    }
}
