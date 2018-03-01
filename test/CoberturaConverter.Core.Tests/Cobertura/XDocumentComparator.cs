using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace CoberturaConverter.Core.Tests.Cobertura
{
    public class XDocumentComparator
    {
        public XDocumentComparator(XDocument expectedDocument, XDocument actualDocument)
        {
            _expectedDocument = expectedDocument;
            _actualDocument = actualDocument;
        }

        private readonly XDocument _expectedDocument;
        private readonly XDocument _actualDocument;
        private string _currentLocation = string.Empty;

        public void AssertXDocumentsAreEqual()
        {
            var expectedRoot = _expectedDocument.Root;
            var actualRoot = _actualDocument.Root;
            CompareElement(expectedRoot, actualRoot);
        }

        private void CompareElement(XElement expected, XElement actual)
        {
            SetCurrentLocationToElement(expected);
            CompareNamespace(expected, actual);
            CompareElementName(expected, actual);
            CompareElementContent(expected, actual);
            CompareElementAttributes(expected, actual);
            RemoveElementFromCurrentLocation(expected);
        }

        private void CompareNamespace(XElement expected, XElement actual)
        {
            var expectedNamespace = expected.Name.Namespace;
            var actualNamespace = actual.Name.Namespace;
            var namespacesAreEqual = expectedNamespace == actualNamespace;
            Assert.True(namespacesAreEqual, GetMessageForCurrentLocation("Namespace", expectedNamespace.ToString(), actualNamespace.ToString()));
        }

        private void CompareElementName(XElement expected, XElement actual)
        {
            var expectedName = expected.Name.LocalName;
            var actualName = actual.Name.LocalName;
            var namesAreEqual = expectedName == actualName;
            Assert.True(namesAreEqual, GetMessageForCurrentLocation("Name", expectedName, actualName));
        }

        private void CompareElementContent(XElement expected, XElement actual)
        {
            if (expected.Elements().Any())
            {
                var expectedElements = expected.Elements().ToList();
                var actualElements = actual.Elements().ToList();
                var hasSameNumberOfElements = expectedElements.Count == actualElements.Count;
                Assert.True(hasSameNumberOfElements, GetMessageForCurrentLocation("ElementChildCount", expectedElements.Count.ToString(), actualElements.Count.ToString()));
                for (var i = 0; i < expectedElements.Count; i++)
                {
                    var expectedChild = expectedElements[i];
                    var actualChild = actualElements[i];
                    CompareElement(expectedChild, actualChild);
                }
            }
            else
            {
                if (actual.Elements().Any())
                {
                    Assert.True(false, GetMessageForCurrentLocation("ElementChildCount", "0", actual.Elements().Count().ToString()));
                }

                var expectedContent = expected.Value;
                var actualContent = expected.Value;
                var contentIsEqual = expectedContent == actualContent;
                Assert.True(contentIsEqual, GetMessageForCurrentLocation("ElementContent", expectedContent, actualContent));
            }
        }

        private void CompareElementAttributes(XElement expected, XElement actual)
        {
            if (expected.Attributes().Any())
            {
                var expectedAttributes = expected.Attributes().ToList();
                var actualAttributes = actual.Attributes().ToList();
                var hasSameNumberOfAttributes = expectedAttributes.Count == actualAttributes.Count;
                Assert.True(hasSameNumberOfAttributes, GetMessageForCurrentLocation("AttributesCount", expectedAttributes.Count.ToString(), actualAttributes.Count.ToString()));
                foreach (var expectedAttribute in expectedAttributes)
                {
                    CheckAttributeIsPresent(expectedAttribute, actualAttributes);
                }
            }
            else
            {
                if (actual.Attributes().Any())
                {
                    Assert.True(false, GetMessageForCurrentLocation("AttributesCount", "0", actual.Attributes().Count().ToString()));
                }
            }
        }

        private void CheckAttributeIsPresent(XAttribute expectedAttribute, List<XAttribute> actualAttributes)
        {
            var actualAttribute = actualAttributes.FirstOrDefault(a => a.Name == expectedAttribute.Name);
            var attributeIsPresent = actualAttribute != null;
            Assert.True(attributeIsPresent, GetMessageForCurrentLocation("AttributePresent", actualAttribute.Name.LocalName, string.Empty));
            var expectedValue = expectedAttribute.Value;
            var actualValue = actualAttribute.Value;
            var attributeValueIsCorrect = expectedValue == actualValue;
            Assert.True(attributeValueIsCorrect, GetMessageForCurrentLocation("AttributeValue", expectedValue, actualValue));
        }

        private string GetMessageForCurrentLocation(string checkType, string expected, string actual)
        {
            var message = $"Error encountered at \"{_currentLocation}\"." + Environment.NewLine
                                                                          + $"Check performed for: {checkType}" + Environment.NewLine
                                                                          + $"Expected value: {expected}" + Environment.NewLine
                                                                          + $"Actual value: {actual}";
            return message;
        }

        private void SetCurrentLocationToElement(XElement expectedElement)
        {
            _currentLocation += $"//{expectedElement.Name.LocalName}";
        }

        private void RemoveElementFromCurrentLocation(XElement expectedElement)
        {
            var elementNameLength = $"//{expectedElement.Name.LocalName}".Length;
            _currentLocation = _currentLocation.Substring(0, _currentLocation.Length - elementNameLength);
        }
    }
}
