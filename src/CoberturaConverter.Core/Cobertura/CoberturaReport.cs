using System;
using System.Collections.Generic;

namespace CoberturaConverter.Core.Cobertura
{
    public class CoberturaReport
    {
        public decimal? LineRate { get; set; }
        public decimal? BranchRate { get; set; }
        public int? LinesCovered { get; set; }
        public int? LinesValid { get; set; }
        public int? BranchesCovered { get; set; }
        public int? BranchesValid { get; set; }
        public int? Complexity { get; set; }
        public string Version { get; set; }
        public DateTime? Timestamp { get; set; }
        public List<string> Sources { get; set; }
        public List<CoberturaPackage> Packages { get; set; }
    }
}
