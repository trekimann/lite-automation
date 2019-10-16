using Magrathea.objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Magrathea.ElementOperations
{
    public class localVariable
    {
        private Dictionary<String, typeObjectPair> dataValuesMap;
        private MagratheaExecutor executor;
        private StringComparison ignoringCase = StringComparison.InvariantCultureIgnoreCase;

        public localVariable(MagratheaExecutor executor)
        {
            this.executor = executor;
            this.dataValuesMap = executor.getDataValuesMap();
        }

        public Dictionary<String, typeObjectPair> setVariable(XElement element)
        {
            // find the name of the phase value to change
            String phaseValueName = element.Attribute("name").Value;

            if (phaseValueName.Equals("cookieButton", ignoringCase))
            {
                Console.WriteLine("mag debug point");
            }

            Object phaseValue = null;
            // check type of variable being made
            String type = null;
            try { type = element.Attribute("type").Value; } catch { }
            typeObjectPair variablePair = null;
            Boolean childVariable = false;
            // check if the set has children
            List<XElement> setChildren = element.Elements().ToList();
            if (setChildren.Count > 0)
            {
                childVariable = true;
                Console.WriteLine("Internal Variable in " + phaseValueName);
                // do child variable to get value
                phaseValue = executor.ExecuteElement(setChildren[0], "");
                Console.WriteLine(phaseValue);
            }

            // Check if variable already exists, if so get it
            typeObjectPair variable = null;
            try
            {
                variable = dataValuesMap[phaseValueName];//doesnt use safe method as exception is used to trigger next action
                type = variable.Cast;
            }
            catch
            {}
            if (variable != null && !childVariable)
            {
                variablePair = executor.DataValuesMap(phaseValueName);
                phaseValue = variablePair.Value;
            }
            else
            {
                variablePair = new typeObjectPair();
            }

            if (!childVariable)
            {
                // Boolean
                if (type.Equals("boolean", ignoringCase))
                {
                    phaseValue = setBool(element);
                }
                // int
                else if (type.Equals("int", ignoringCase))
                {
                    phaseValue = setInt(element);
                }
                // string
                else if (type.Equals("string", ignoringCase))
                {
                    phaseValue = setString(element);
                }
                // dateTime
                else if (type.Equals("dateTime", ignoringCase))
                {
                    phaseValue = setDateTime(element);
                }
            }
            // change in the map, if not there then create it
            variablePair.Cast = type;
            variablePair.Value = phaseValue;
            var blank = new typeObjectPair();
            //check if there
            if (dataValuesMap.TryGetValue(phaseValueName,out blank))
            {
                dataValuesMap[phaseValueName] = variablePair;
            }
            else
            {
                dataValuesMap.Add(phaseValueName, variablePair);
            }

            childVariable = false;
            return dataValuesMap;
        }

        private Object setDateTime(XElement element)
        {
            MagDateTime objectDateTime = new MagDateTime(executor);
            return objectDateTime.dateTime(element);
        }

        private Object setString(XElement element)
        {
            MagString magString = new MagString(executor);
            return magString.stringMethod(element);
        }

        public Object setBool(XElement element)
        {
            MagBool magBool = new MagBool(executor);
            return magBool.boolMethod(element);
        }

        public int setInt(XElement element)
        {
            // check for source value
            String sourceVariable = element.Attribute("source").Value;
            // if its not blank use that variable to populate the new variable
            if (sourceVariable != "" && sourceVariable != null)
            {
                return (int)executor.DataValuesMap(sourceVariable).Value;
            }
            else
            {
                String definedValue = element.FirstNode.ToString();
                return Int32.Parse(definedValue);
            }
        }

        public Dictionary<String, typeObjectPair> disposeVariable(XElement element, Dictionary<String, typeObjectPair> dataValuesMap)
        {
            // find the name of the phase value to change
            String phaseValueName = element.Attribute("name").Value;
            // remove from map
            dataValuesMap.Remove(phaseValueName);

            return dataValuesMap;
        }

        public Object getValue(XElement element)
        {
            //find what type of element called getValue
            String type = element.Name.ToString();

            if (type.Equals("getPhaseValue"))
            {
                //target variable
                String key = element.Attribute("target").Value;
                Object toReturn = null;
                if (key.Equals("SCENARIO_NAME"))
                {
                    toReturn = executor.CurrentScenario;
                }
                else
                {
                    toReturn = executor.DataValuesMap(key).Value;
                }

                return toReturn;
            }
            else if (type.Equals("dateTime"))
            {
                return executor.ExecuteElement(element, "");
            }
            return null;
        }
    }
}
