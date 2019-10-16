using Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Magrathea
{
    public class XmlParser : IXmlParser
    {
        public XElement ParseXml(String directory)
        {
            XElement toReturn;
            //need to put some error handling here for dud xml.
            try
            {
                XDocument parsedXml = XDocument.Load(directory, LoadOptions.PreserveWhitespace);
                toReturn = parsedXml.Root;
            }
            catch (XmlException xmlException)
            {
                string fileName = Path.GetFileName(directory);
                Console.WriteLine(xmlException);
                if (fileName.StartsWith("US"))
                { toReturn = FailedParsing_test(xmlException.ToString()); }
                else toReturn = null;
            }
            return toReturn;
        }

        public Dictionary<String, ElementIdentifierWrapper> ParseDictionary(String dictionaryDirectory, String journeyName)
        {
            //loop though deffintions and build map
            Dictionary<String, ElementIdentifierWrapper> mappedDictionary = new Dictionary<String, ElementIdentifierWrapper>();


            //get dictionary as one element
            XElement dictionary = ParseXml(dictionaryDirectory);

            //put bad dictionary handling here

            if (journeyName.Equals("All"))
            {
                //get number of elements in the full dictionary
                var subDictionarys = dictionary.Elements().ToList();
                foreach (var subDictionary in subDictionarys)
                {
                    String dictionaryJourneyName = subDictionary.Name.ToString();
                    List<XElement> deffinitions = subDictionary.Elements().ToList();
                    FillDictionary(dictionaryJourneyName, deffinitions, mappedDictionary);
                }
            }
            else
            {
                //get Element for the journey you are on
                XElement journeyDictionary = dictionary.Element(journeyName);
                //build list of definitions
                List<XElement> deffinitions = journeyDictionary.Elements().ToList();
                FillDictionary(journeyName, deffinitions, mappedDictionary);

            }
            return mappedDictionary;
        }

        public void FillDictionary(string journeyName, List<XElement> deffinitions, Dictionary<string, ElementIdentifierWrapper> mappedDictionary)
        {
            foreach (XElement deffinition in deffinitions)
            {
                ElementIdentifierWrapper wrapper = new ElementIdentifierWrapper();
                String identifier = deffinition.FirstNode.ToString().Trim();

                var idCheck = deffinition.Element("Id").FirstNode;
                String id = null;
                if (idCheck != null)
                {
                    id = idCheck.ToString();
                }
                wrapper.SetElementId(id);

                var nameCheck = deffinition.Element("Name").FirstNode;
                String name = null;
                if (nameCheck != null)
                {
                    name = nameCheck.ToString();
                }
                wrapper.SetElementName(name);

                var xpathCheck = deffinition.Element("xPath").FirstNode;
                String xPath = null;
                if (xpathCheck != null)
                {
                    xPath = xpathCheck.ToString();
                }
                wrapper.SetElementXpath(xPath);

                wrapper.SetJourney(journeyName);

                //check if already in the dictionary
                if (mappedDictionary.ContainsKey(identifier))
                {
                    Console.WriteLine("Key " + identifier + " already in dictionary");
                    String alreadyIn = mappedDictionary[identifier].ToString();
                    Console.WriteLine("value already in: " + alreadyIn);
                    String newWrapper = wrapper.ToString();
                    Console.WriteLine("value being added: " + newWrapper);
                    if (alreadyIn.Equals(newWrapper))
                    {
                        Console.WriteLine("value for " + identifier + " already in dictionary");
                    }
                }
                else
                {
                    mappedDictionary.Add(identifier, wrapper);
                }
            }
        }

        public XElement FailedParsing_test(String exception)
        {
            XElement toReturn = new XElement("TestData");
            toReturn.Add(new XAttribute("name", "FAILED_TO_PARSE"));
            toReturn.Add(new XAttribute("url", "http://www.FAILED_TO_PARSE.com"));
            toReturn.Add(new XAttribute("singlePage", "true"));

            toReturn.Add(new XElement("Data"));

            XElement Function = new XElement("Function");
            XElement RecordOutcome = new XElement("testLogger");
            RecordOutcome.Add(new XAttribute("type", "recordToFile"));
            RecordOutcome.Add(new XAttribute("status", "Fail"));
            RecordOutcome.Value = exception;

            Function.Add(RecordOutcome);
            toReturn.Add(Function);

            return toReturn;
        }
    }
}
