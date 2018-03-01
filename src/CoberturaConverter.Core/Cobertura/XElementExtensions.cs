using System;
using System.Xml.Linq;

namespace CoberturaConverter.Core.Cobertura
{
    internal static class XElementExtensions
    {
        public static void AddAttributeIfItHasValue(this XElement element, string attributeName, string attributeValue)
        {
            if (!string.IsNullOrWhiteSpace(attributeValue))
            {
                element.SetAttributeValue(attributeName, attributeValue);
            }
        }

        public static void AddAttributeIfItHasValue(this XElement element, string attributeName, int? attributeValue)
        {
            if (attributeValue != null)
            {
                element.SetAttributeValue(attributeName, attributeValue.Value);
            }
        }

        public static void AddAttributeIfItHasValue(this XElement element, string attributeName, decimal? attributeValue)
        {
            if (attributeValue != null)
            {
                element.SetAttributeValue(attributeName, attributeValue.Value);
            }
        }

        public static void AddAttributeIfItHasValue(this XElement element, string attributeName, DateTime? attributeValue)
        {
            if (attributeValue != null)
            {
                var unixTimestamp = ((DateTimeOffset) attributeValue.Value.ToUniversalTime()).ToUnixTimeSeconds();
                element.SetAttributeValue(attributeName, unixTimestamp);
            }
        }

        public static void AddAttributeIfItHasValue(this XElement element, string attributeName, bool? attributeValue)
        {
            if (attributeValue != null)
            {
                element.SetAttributeValue(attributeName, attributeValue.Value);
            }
        }
    }
}
