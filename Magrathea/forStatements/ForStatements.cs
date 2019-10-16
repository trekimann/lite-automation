using Magrathea.ElementOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Magrathea.forStatements
{
    public class ForStatements
    {
        private MagratheaExecutor executor;
        private Dictionary<String, typeObjectPair> dataValuesMap = new Dictionary<string, typeObjectPair>();
        private List<XElement> dataItems;
        private StringComparison ignoringCase = StringComparison.InvariantCultureIgnoreCase;

        public ForStatements(MagratheaExecutor parser, XElement rootPhase)
        {
            this.executor = parser;
            this.dataValuesMap = parser.getDataValuesMap();

            XElement dataElement = rootPhase.Elements("Data").ToList()[0];
            //check for scenario
            if (dataElement.Elements("scenario").Count() >0)
            {
                //get the current scenario and extract the data element from there
                foreach(var scenario in dataElement.Elements())
                {
                    var scenarioName = scenario.Attribute("name").Value;
                    //check the currently running scenario and use that data
                    if (scenarioName.Equals(executor.CurrentScenario))
                    {
                        dataItems = scenario.Elements("phaseValue").ToList();
                        break;
                    }
                }
            }
            else
            {
                dataItems = dataElement.Elements("phaseValue").ToList();
            }
        }

        public void listFor(XElement forElement)
        {
            //check if the test is running form a phase or if its inside a single page.


            String condition = forElement.Attribute("condition").Value;
            String numbertoItterateOverVariable = forElement.FirstNode.ToString().Trim();
            int numberToItterate = (int)(executor.DataValuesMap(numbertoItterateOverVariable).Value);

            // get list element from phase
            String targetList = forElement.Attribute("targetList").Value;
            // find list element
            XElement listElement = null;
            for (int i = 0; i < dataItems.Count; i++)
            {
                String currentElementName = dataItems[i].Attribute("name").Value.Trim();
                if (currentElementName.Equals(targetList, ignoringCase))
                {
                    listElement = dataItems[i];
                    break;
                }
            }

            // build list of strings for quick comparison
            List<String> listNames = new List<string>();
            foreach (XElement element in listElement.Elements())
            {
                listNames.Add(element.Attribute("name").Value.Trim());
            }
            executor.SetListNames(listNames);
            executor.SetInsideFor(true);

            // execute inside for loop
            // check type of for
            if (condition.Equals("lessThan", ignoringCase))
            {
                for (int i = 0; i < numberToItterate; i++)
                {
                    // elements inside the for loop
                    List<XElement> insideForOrig = forElement.Elements().ToList();
                    // List<XElement> cloneInsideFor = cloneElementList(insideForOrig);

                    executor.ExecuteFunction(insideForOrig, i.ToString());
                }
                executor.SetInsideFor(false);
            }
        }
    }
}
