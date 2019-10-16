using System.Collections.Generic;
using System.Xml.Linq;
using Selenium;

namespace Magrathea
{
    public interface IXmlParser
    {
        XElement FailedParsing_test(string exception);
        void FillDictionary(string journeyName, List<XElement> deffinitions, Dictionary<string, ElementIdentifierWrapper> mappedDictionary);
        Dictionary<string, ElementIdentifierWrapper> ParseDictionary(string dictionaryDirectory, string journeyName);
        XElement ParseXml(string directory);
    }
}