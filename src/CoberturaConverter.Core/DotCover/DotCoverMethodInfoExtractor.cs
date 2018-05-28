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
            if (delimiterIndex < 0)
            {
                // The delimiter index should indate at which position
                // the opening bracket of the method starts, so that the
                // method info can be split into name an signature, e.g.:
                // name: 'GetMethodInfoFromDotCoverMethod'
                // signature: '(string dotCoverMethodName)'
                delimiterIndex = methodDefinition.Length;
            }
            var name = methodDefinition.Substring(0, delimiterIndex);
            var signature = methodDefinition.Substring(delimiterIndex);
            return (name, signature);
        }
    }
}
