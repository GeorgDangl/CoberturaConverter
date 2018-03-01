using System;
using System.Linq;

namespace CoberturaConverter.Core.DotCover
{
    public static class DotCoverMethodInfoExtractor
    {
        public static (string name, string signature) GetMethodInfoFromDotCoverMethod(string dotCoverMethodName)
        {
            if (string.IsNullOrWhiteSpace(dotCoverMethodName))
            {
                throw new ArgumentNullException(nameof(dotCoverMethodName));
            }

            var methodDefinition = dotCoverMethodName
                .Split(':')
                .First();
            var delimiterIndex = methodDefinition.IndexOf('(');
            var name = methodDefinition.Substring(0, delimiterIndex);
            var signature = methodDefinition.Substring(delimiterIndex);
            return (name, signature);
        }
    }
}
