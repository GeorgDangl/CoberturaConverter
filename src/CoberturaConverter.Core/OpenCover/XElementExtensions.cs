using System;
using System.Xml.Linq;

// This code is initially based on Daniel Palmes OpenCoverToCoberturaConverter
// Please see its license in this projects root directory
// https://github.com/danielpalme/OpenCoverToCoberturaConverter

/*
   Copyright 2013 Daniel Palme

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

namespace CoberturaConverter.Core.OpenCover
{
    public static class XElementExtensions
    {
        /// <summary>
        /// Determines whether a <see cref="XElement"/> has an <see cref="XAttribute"/> with the given value..
        /// </summary>
        /// <param name="element">The <see cref="XElement"/>.</param>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <param name="attributeValue">The attribute value.</param>
        /// <returns>
        ///   <c>true</c> if <see cref="XElement"/> has an <see cref="XAttribute"/> with the given value; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasAttributeWithValue(this XElement element, XName attributeName, string attributeValue)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            XAttribute attribute = element.Attribute(attributeName);

            if (attribute == null)
            {
                return false;
            }
            else
            {
                return string.Equals(attribute.Value, attributeValue, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
