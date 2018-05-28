using System;
using System.Linq;
using System.Text.RegularExpressions;

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

            // The regex should only match on single but not on double colons, eg it should match:
            // MethodName():void -> "MethodName()" and "void"
            // but not for double colons:
            // MethodName(Foo::Bar):void -> "MethodName(Foo::Bar)" and "void"
            var methodDefinition = Regex.Split(dotCoverMethodName, "(?<!:):(?!:)")
                .First();

            var delimiterIndex = methodDefinition.IndexOf('(');
            if (delimiterIndex < 0)
            {
                // The delimiter index should indicate at which position
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
