using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Magrathea
{
    public class TestBuilder
    {
        //this class is used to build test scripts, for now it will just build single page tests based on the execution of a test so that any test which use a phase will end up with a single page test which is representative of the test which was run at the time and wont be affected by any changes to the journeys phases
        private StringComparison ignoringCase = StringComparison.InvariantCultureIgnoreCase;

        public XElement StartNewSinglePageTest(String name, String url)
        {
            XElement toReturn = new XElement("TestData");
            XAttribute testName = new XAttribute("name", name);
            XAttribute testUrl = new XAttribute("url", url);

            XElement Data = new XElement("Data");
            XElement Function = new XElement("Function");

            toReturn.Add(Data);
            toReturn.Add(Function);

            return toReturn;
        }

        public XElement AddPhaseValueToDataElement(XElement value, XElement test, String scenario = null)
        {
            //find Data element
            var DataElement = test.Element("Data");
            //check if there is a scenario
            List<XElement> scenarios = DataElement.Elements("scenario").ToList();
            if (scenarios.Count >0 || scenario!=null)
            {
                //check if the scenario is already in the Data Element
                Boolean scenarioExists = false;
                foreach(var foundScenario in scenarios)
                {
                    if(foundScenario.Attribute("name").Value.Equals(scenario,ignoringCase))
                    {
                        scenarioExists = true;
                        foundScenario.Add(value);
                        break;
                    }
                }
                if (!scenarioExists)
                {
                    //if a scenario is neeed, create it
                    var scenarioElement = new XElement("scenario");
                    scenarioElement.Add(new XAttribute("name", scenario));
                    scenarioElement.Add(value);
                    DataElement.Add(scenarioElement);
                }
            }
            else
            {
               //add phasevalue to Data element
                DataElement.Add(value);                
            }
            return test;
        }

        public XElement AddTestElement(XElement testSoFar, XElement elementToAdd)
        {
            //get function section of the test
            var function = testSoFar.Element("Function");

            function.Add(elementToAdd);

            return testSoFar;
        }

        public Object Savetest(XElement test, String targetDirectory)
        {
            dynamic toReturn = new ExpandoObject();
            try
            {
                test.Save(targetDirectory);
                toReturn.Success = true;
            }
            catch (Exception e)
            {
                toReturn.Success = false;
                toReturn.Message = e.ToString();
            }
            return toReturn;
        }
    }
}
