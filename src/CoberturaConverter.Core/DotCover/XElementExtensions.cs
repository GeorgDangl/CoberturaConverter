using System;
using System.Linq;
using System.Xml.Linq;

namespace CoberturaConverter.Core.DotCover
{
    public static class XElementExtensions
    {
        public static string GetAttributeStringValue(this XElement element, string attributeName)
        {
            var attribute = element.GetAttributeByName(attributeName);
            return attribute?.Value;
        }

        public static int GetAttributeIntValue(this XElement element, string attributeName)
        {
            var attribute = element.GetAttributeByName(attributeName);
            return int.TryParse(attribute?.Value, out var value) ? value : default;
        }

        public static decimal GetAttributeDecimalValue(this XElement element, string attributeName)
        {
            var attribute = element.GetAttributeByName(attributeName);
            return decimal.TryParse(attribute?.Value, out var value) ? value : default;
        }

        public static bool GetAttributeBooleanValue(this XElement element, string attributeName)
        {
            var attribute = element.GetAttributeByName(attributeName);
            return attribute?.Value.Equals("true", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        private static XAttribute GetAttributeByName(this XElement element, string attributeName)
        {
            var attribute = element.Attributes().FirstOrDefault(a => a.Name.LocalName == attributeName);
            return attribute;
        }
    }
}
