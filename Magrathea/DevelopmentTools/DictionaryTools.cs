using Selenium;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Magrathea.DevelopmentTools
{
    public class DictionaryTools
    {
        public Dictionary<String, Dictionary<String, ElementIdentifierWrapper>> mainDictionary { get; private set; } = new Dictionary<String, Dictionary<String, ElementIdentifierWrapper>>();
        public XElement FullDictionary;
        public IXmlParser parse { get; set; }

        private String dictionaryDirectory = @"O:\SeleniumLite\EXPERIMENTAL\SeleniumBackEnd\Element Dictionary\ElementDictionary.xml";

        public DictionaryTools(String dictionaryDirectory)
        {
            this.dictionaryDirectory = dictionaryDirectory;
            //parse dictionary
            parse = new XmlParser();
            FullDictionary = parse.ParseXml(dictionaryDirectory);

            if (FullDictionary != null)
            {
                //set up journey based dictionarys
                var subDictionarys = FullDictionary.Elements().ToList();

                foreach (var journey in subDictionarys)
                {
                    String dictionaryName = journey.Name.ToString();
                    var parsedDictionary = parse.ParseDictionary(dictionaryDirectory, dictionaryName);
                    mainDictionary.Add(dictionaryName, parsedDictionary);
                }
            }
        }

        public dynamic CheckForDefinition(String term)
        {
            return CheckForDefinition(term, "all");
        }

        public dynamic CheckForDefinition(String term, String Journey)
        {
            dynamic toReturn = new ExpandoObject();
            toReturn.Found = false;
            List<String> journeys = new List<string>();
            if (Journey.Equals("all"))
            {
                journeys = mainDictionary.Keys.ToList();
            }
            else
            { journeys.Add(Journey);
              journeys.Add("misc"); }
            //list of dictionarys

            String Result = term + ": " + Environment.NewLine;
            if (term != null)
            {
                //load up dictionary
                foreach (var journey in journeys)
                {
                    var dictionary = mainDictionary[journey];
                    //first check keys
                    if (dictionary.ContainsKey(term))
                    {
                        Result += "Element Identifier: " + term + "\n";
                        Result += "journey: " + journey + "\n";
                        Result += dictionary[term].ToString() + Environment.NewLine;
                        toReturn.Found = true;
                    }
                    else
                    {
                        var keys = dictionary.Keys.ToList();
                        //check contents of keys 
                        foreach (var key in keys)
                        {
                            var definition = dictionary[key];
                            String junk = "---***---***---";
                            String id = definition.elementId;
                            String name = definition.elementName;
                            String xPath = definition.elementXpath;

                            if (id == null)
                            {
                                id = junk;
                            }
                            if (name == null)
                            {
                                name = junk;
                            }
                            if (xPath == null)
                            {
                                xPath = junk;
                            }

                            if (id.Equals(term) || name.Equals(term) || xPath.Equals(term))
                            {
                                Result += "Element Identifier: " + key + "\n";
                                Result += "journey: " + journey + "\n";
                                Result += dictionary[key].ToString() + Environment.NewLine;
                                toReturn.Found = true;
                            }
                        }
                    }
                }
            }
            toReturn.Result = Result;
            return toReturn;
        }

        public dynamic CheckForDefintionInPhases(List<String> elementIds, String PhasesDirectory)
        {
            dynamic toReturn = new ExpandoObject();
            toReturn.Found = false;
            toReturn.Result = "Not found in any phases";
            //load up list of folders in phase directory
            String rootDir = PhasesDirectory;
            string[] journeys = Directory.GetDirectories(rootDir);

            String Result = "";

            //loop though each folder and load up list of files
            foreach (var journey in journeys)
            {
                string[] phases = Directory.GetFiles(journey);

                foreach (var phase in phases)
                {
                    //open each file and load its contents
                    string readContents;
                    using (StreamReader streamReader = new StreamReader(phase, Encoding.UTF8))
                    {
                        readContents = streamReader.ReadToEnd();
                    }
                    //check each one for each id
                    foreach (var elementid in elementIds)
                    {
                        string phasename = phase.Replace(rootDir + "\\", "");
                        if (readContents.Contains(elementid))
                        {
                            //add found pahse details to return
                            Result += elementid + " : Located in " + phasename + Environment.NewLine;
                            toReturn.Found = true;
                        }
                    }
                }
            }
            toReturn.Result = Result;

            return toReturn;
        }

        public dynamic AddTermToDictionary(String Identifier, ElementIdentifierWrapper definition, Boolean save = true)
        {
            String junk = "------*****------";//junk string which is used in case of blank fields

            dynamic toReturn = new ExpandoObject();
            toReturn.AddedTerm = false;
            toReturn.Result = "Not added to dictionary";

            String Id = definition.elementId;
            String name = definition.elementName;
            String xPath = definition.elementXpath;
            String Journey = definition.journey;

            String file = dictionaryDirectory;

            if (!((Boolean)CheckForDefinition(Id).Found))
            {
                if (!((Boolean)CheckForDefinition(Identifier).Found))
                {
                    if (!((Boolean)CheckForDefinition(name).Found))
                    {

                        if (name == junk|| name==null)
                        {
                            name = "";
                        }
                        if (Id == junk)
                        {
                            Id = "";
                        }
                        if(xPath==null)
                        {
                            xPath = "";
                        }
                        //create deffinition
                        Console.WriteLine("Making the definition for "+ Identifier);
                        XElement Definition = new XElement("Definition");//root element
                        Definition.Value = Identifier;

                        XElement idElement = new XElement("Id");
                        idElement.Value = Id;

                        XElement nameElement = new XElement("Name");
                        nameElement.Value = name;

                        XElement XPath = new XElement("xPath");
                        XPath.Value = xPath;

                        Definition.Add(idElement);
                        Definition.Add(nameElement);
                        Definition.Add(XPath);

                        //find the journey to add the defintion to
                        List<XElement> journeys = FullDictionary.Elements().ToList();
                        foreach (var journey in journeys)
                        {
                            String jName = journey.Name.ToString();
                            if (jName.Equals(Journey))
                            {
                                journey.Add(Definition);
                                journey.Add(Environment.NewLine);

                                try
                                {
                                    if (save)
                                    {
                                        FullDictionary.Save(file);
                                    }
                                    toReturn.AddedTerm = true;
                                    toReturn.Result = "Definition for " + Identifier + " added";
                                }
                                catch (Exception ex)
                                {
                                    toReturn.Result = "Exception in saving file";
                                    Console.WriteLine(ex.ToString());
                                }
                                break;
                            }
                        }
                    }
                    else { toReturn.Result = "Name Already in dictionary"; }
                }
                else { toReturn.Result = "Identifier Already in dictionary"; }
            }
            else { toReturn.Result = "Id Already in dictionary"; }


            return toReturn;
        }

        public void Merge(String secondDirectory)
        {
            //merges a second dictionary into the one used to create this class

            //parse xml for second dictionary
            XmlParser parse = new XmlParser();
            XElement SecondDictionaryRaw = parse.ParseXml(secondDirectory);

            //save backup of OG main dictionary
            var backupDirectory = dictionaryDirectory.Replace("ElementDictionary.xml", "ElementDictionary_MERGEBACKUP"+(DateTime.Now.ToShortDateString().Replace("/","-"))+".xml");
            FullDictionary.Save(backupDirectory);

            Dictionary<String, Dictionary<String, ElementIdentifierWrapper>> SecondDictionary = new Dictionary<string, Dictionary<string, ElementIdentifierWrapper>>();

            //set up journey based dictionarys
            var SecondSubDictionarys = SecondDictionaryRaw.Elements().ToList();

            foreach (var journey in SecondSubDictionarys)
            {
                String dictionaryName = journey.Name.ToString();
                var parsedDictionary = parse.ParseDictionary(secondDirectory, dictionaryName);
                SecondDictionary.Add(dictionaryName, parsedDictionary);
            }

            //compare journeys, if there are extras in the SECOND, add them to the first
            var mainKeyList = mainDictionary.Keys.ToList();
            var secondKeyList = SecondDictionary.Keys.ToList();

            foreach (String secondKey in secondKeyList)
            {
                if (!mainKeyList.Contains(secondKey))
                {
                    mainDictionary.Add(secondKey,SecondDictionary[secondKey]);
                }
            }

            //now that the main dictionary has all the journeys, check if it has all the definitions. Need to go through second dictionary journeys one by one and try to add to dictionary

            int elementcount = 0;
            foreach(String journey in secondKeyList)
            {
                //get element from dictionary
                var elementsInJourney = SecondDictionary[journey].ToList();

                foreach(var element in elementsInJourney)
                {
                    var added = AddTermToDictionary(element.Key, element.Value,false);
                    if (added.AddedTerm == true)
                    {
                        elementcount++;
                    }
                }
            }
            Console.WriteLine("Definitions in second dictionary added: "+elementcount);
            //save new dictionary in location of second
            FullDictionary.Save(dictionaryDirectory);
            //SaveDictionaryAsXml(mainDictionary, dictionaryDirectory);
        }

        public void SaveDictionaryAsXml(Dictionary<String, Dictionary<String, ElementIdentifierWrapper>> dictionary,string outputDirectory)
        {
            XElement Root = new XElement("ElementDictionary");

            //make mourney Elements
            //get journey names
            List<String> journeyNames = dictionary.Keys.ToList();
            List<XElement> journeyElements = new List<XElement>();
            foreach(var journey in journeyNames)
            {
                var journeyElement = new XElement(journey);
                //for each journey, add the definitions
                List<String> definitions = dictionary[journey].Keys.ToList();

                foreach (string definition in definitions)
                {
                    var deffintionWrapper = dictionary[journey][definition];

                    var Id = deffintionWrapper.elementId;
                    var Name = deffintionWrapper.elementName;
                    var xPath = deffintionWrapper.elementXpath;

                    if (Id == null)
                    { Id = ""; }
                    if (Name == null)
                    { Name = ""; }
                    if (xPath == null)
                    { xPath = ""; }

                    var DeffinitionElement = new XElement("Definition");
                    DeffinitionElement.Value = definition;

                    var IdElement = new XElement("Id");
                    IdElement.Value = Id;
                    DeffinitionElement.Add(IdElement);

                    var NameElement = new XElement("Name");
                    NameElement.Value = Name;
                    DeffinitionElement.Add(NameElement);

                    var XpathElement = new XElement("xPath");
                    XpathElement.Value = xPath;
                    DeffinitionElement.Add(XpathElement);

                    journeyElement.Add(DeffinitionElement);
                }
                Root.Add(journeyElement);
            }
            try
            {
                Root.Save(outputDirectory);
                Console.WriteLine("Saved new dictionary");
            }
            catch(Exception e)
            {
                Console.WriteLine("Failed to save new dictionary> "+e.ToString());
            }
        }
    }
}
