namespace CoberturaConverter.Core.DotCover
{
    public class DotCoverStatement
    {
        public int FileIndex { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }
        public int EndLine { get; set; }
        public int EndColumn { get; set; }
        public bool IsCovered { get; set; }
    }
}
