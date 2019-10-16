using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace restClient_0
{
    public class DataObject
    {
        public Dictionary<string, Dictionary<string, Dictionary<string, string>>> dataContainerObject { get; set; } = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
        public List<string> phaseOrder { get; set; } = new List<string>();

        public void addPhaseAttributetoDataContainer(string phase, Dictionary<string, Dictionary<string, string>> elementDictionary)
        {
            if (!(dataContainerObject.ContainsKey(phase)))
            {
                dataContainerObject.Add(phase, elementDictionary);
            }

        }

        public void removeEllementFromDictionary(string phase, string element)
        {
            if (dataContainerObject[phase].ContainsKey(element))
            {

                dataContainerObject[phase].Remove(element);


            }
            
        }

        public void addDataElementToDataObject(string phase, string element, Dictionary<string, string> attributeDictionary)
        {
            if (dataContainerObject[phase].ContainsKey(element))
            {

                dataContainerObject[phase][element] = attributeDictionary;
               

            }
            else
            {

                dataContainerObject[phase].Add(element, attributeDictionary);

            }
        }

        public void addAtributeToElement(string phase, string element, string attributeKey, string attributeValue)
        {

            var phaseDict = dataContainerObject[phase];
            var ellementDict = phaseDict[element];

            if (dataContainerObject[phase][element].ContainsKey(attributeKey))
            {

                dataContainerObject[phase][element][attributeKey] = attributeValue;

            }
            else
            {

                dataContainerObject[phase][element].Add(attributeKey, attributeValue);

            }
        }

        private void printOutDictionary(Dictionary<string, Dictionary<string, string>> dict)
        {
            foreach (var phase in dict)
            {
                Console.WriteLine(phase.Key);
                foreach (var dataItem in phase.Value)
                {
                    Console.WriteLine("     " + dataItem.Key + " " + dataItem.Value);
                }
            }
            Console.WriteLine("complete dictionary remove happy path");
        }

        public Dictionary<string, Dictionary<string, Dictionary<string, string>>> cleanData()
        {
            Dictionary<string, Dictionary<string, Dictionary<string, string>>> tempdataContainerObjectn = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
            foreach (var phase in dataContainerObject)
            {
                var phaseData = dataContainerObject[phase.Key];
                if ((dataContainerObject[phase.Key].ContainsKey("happyDefault")))
                {
                    if ((dataContainerObject[phase.Key]["happyDefault"]["value"].Equals("true")))
                    {
                        if (!(tempdataContainerObjectn.ContainsKey(phase.Key)))
                        {
                            tempdataContainerObjectn.Add(phase.Key, new Dictionary<string, Dictionary<string, string>>());
                        }

                        foreach (var dataItem in phase.Value)
                        {

                            if (!(dataItem.Key.Equals("happyDefault")))
                            {
                                tempdataContainerObjectn[phase.Key].Add(dataItem.Key, new Dictionary<string, string>());
                                foreach (var dataItemElements in dataContainerObject[phase.Key][dataItem.Key])
                                {
                                    tempdataContainerObjectn[phase.Key][dataItem.Key].Add(dataItemElements.Key, dataItemElements.Value);
                                }
                            }
                           
                        }
                    }
   
                }
               

            }
            //printOutDictionary(tempdataContainerObjectn);
                return tempdataContainerObjectn;
              

            }
        }
    }

