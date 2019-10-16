using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Selenium.htmlElementManager
{
    public class ElementIdentifierDictionary
    {
        private Dictionary<String, ElementIdentifierWrapper> MappedIdData;
        private XElement FullDictionary;
        private Boolean needsRepair = false;
        private StringComparison ignoringCase = StringComparison.InvariantCultureIgnoreCase;
        private Boolean DevMode { get; set; } = false;
        public HtmlElementManager manager { get; set; }

        public ElementIdentifierDictionary(Dictionary<String, ElementIdentifierWrapper> MappedIdData, XElement fullDictionary)
        {
            this.MappedIdData = MappedIdData;
            this.FullDictionary = fullDictionary;
        }

        public void XmlRepair(String outputDirectory)
        {
            //dissable repair for non-devs           
            if (outputDirectory.StartsWith(@"O:\") && !DevMode)
            {
                Console.WriteLine("In O Drive, can not repair dictionary");
                return;
            }

            //if(!needsRepair)
            //{
            //    Console.WriteLine("Dictionary does not need repair");
            //    return;
            //}
            Console.WriteLine("Trying to Repair dictionary");

            // get all journey Types
            List<XElement> journeys = FullDictionary.Elements().ToList();
            // loop through map and compare to xmlDictionary
            foreach (String key in MappedIdData.Keys)
            {
                List<XElement> journeyDefinitions = null;

                // find which journey element to get
                String journeyString = MappedIdData[key].getJourney();
                foreach (XElement element in journeys)
                {
                    if (element.Name.ToString().Equals(journeyString, ignoringCase))
                    {
                        journeyDefinitions = element.Elements().ToList();
                        break;
                    }
                }
                //loop though definitions and if they match key, update them
                foreach (XElement definition in journeyDefinitions)
                {
                    String defintionName = definition.FirstNode.ToString().Trim();
                    if (key.Equals(defintionName, ignoringCase))
                    {
                        //update definition
                        try { definition.Element("Id").SetValue(MappedIdData[key].GetElementId()); } catch { }
                        try { definition.Element("Name").SetValue(MappedIdData[key].GetElementName()); } catch { }
                        try { definition.Element("xPath").SetValue(MappedIdData[key].GetElementXpath()); } catch { }
                    }
                }

            }//end of key loop
            try { FullDictionary.Save(outputDirectory + @"\ElementDictionary.xml"); } catch (Exception e) { Console.WriteLine(e.ToString()); }
        }


        public void CheckDictionaryDefinition(String elementIdenfier, IWebElement foundElement)
        {
            //mag debug point
            if (elementIdenfier.Equals("aggPageHastingsPremierImage"))
            {
                Console.WriteLine("Mag debug point");
            }

            ElementIdentifierWrapper deffinition = MappedIdData[elementIdenfier];
            String id = deffinition.GetElementId();
            String name = deffinition.GetElementName();
            String xpath = deffinition.GetElementXpath();

            if (id == null)
            {
                id = "";
            }
            if (name == null)
            {
                name = "";
            }
            if (xpath == null)
            {
                xpath = "";
            }

            if (id.Equals(" ") || id.Equals("") || name.Equals(" ")
                    || name.Equals("") || xpath.Equals(" ") || xpath.Equals("")
                    || needsRepair)
            {
                FindAllIdentifiers(elementIdenfier, foundElement);
                //xmlRepair(@"C:\Users\MANNN\Desktop\SeleniumLite_BETA\DevFiles\XML Experiment");
            }
        }

        private void FindAllIdentifiers(String elementIdenfier, IWebElement foundElement)
        {
            ElementIdentifierWrapper deffinition = MappedIdData[elementIdenfier];
            string id = "";
            string name = "";
                        
            try
            {
                id = foundElement.GetAttribute("id");
            }
            catch(StaleElementReferenceException se)
            {
                //stale element, find it again.
                foundElement = manager.Find(elementIdenfier);
                id = foundElement.GetAttribute("id");
            }

            // check if there is a found replacement
            if (id != null && id != "")
            {
                deffinition.SetElementId(id);
            }
            else { deffinition.SetElementId(id); }
            
            if (foundElement.GetAttribute("name") != null && foundElement.GetAttribute("name") != "")
            {
                deffinition.SetElementName((foundElement.GetAttribute("name")));
            }
            else { deffinition.SetElementName(name); }

            try
            {
                deffinition.SetElementXpath(GenerateXPATH(foundElement, ""));
            }
            catch (Exception e) { }
        }

        private String GenerateXPATH(IWebElement childElement, String current)
        {

            String childTag = childElement.TagName;
            if (childTag.Equals("html"))
            {
                return "/html[1]" + current;
            }
            IWebElement parentElement = childElement.FindElement(By.XPath(".."));
            List<IWebElement> childrenElements = parentElement.FindElements(By.XPath("*")).ToList();
            int count = 0;
            for (int i = 0; i < childrenElements.Count; i++)
            {
                IWebElement childrenElement = childrenElements[i];
                string childrenElementString = childrenElement.ToString();
                string childElementString = childElement.ToString();
                String childrenElementTag = childrenElement.TagName;
                if (childTag.Equals(childrenElementTag))
                {
                    count++;
                }
                if (childElementString.Equals(childrenElementString))
                {
                    return GenerateXPATH(parentElement, "/" + childTag + "[" + count + "]" + current);
                }
            }
            return null;
        }

        public Boolean GetNeedsRepair()
        {
            return needsRepair;
        }

        public void SetNeedsRepair(Boolean needsRepair)
        {
            this.needsRepair = needsRepair;
        }

        public Boolean FindException { get; private set; } = false;
        public String GetId(String ourElement)
        {
            try
            {
                ElementIdentifierWrapper deffinition = MappedIdData[ourElement];
                FindException = false;
                return deffinition.GetElementId();
            }catch(Exception e)
            {
                FindException = true;
                Console.WriteLine("ID was not found in ");
                return ourElement;
            }
        }

        public String GetName(String ourElement)
        {
            try
            {
                ElementIdentifierWrapper deffinition = MappedIdData[ourElement];
            return deffinition.GetElementName();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public String GetXpath(String ourElement)
        {
                try
                {
                    ElementIdentifierWrapper deffinition = MappedIdData[ourElement];
            return deffinition.GetElementXpath();
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}