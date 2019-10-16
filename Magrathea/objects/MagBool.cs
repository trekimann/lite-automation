using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Magrathea.objects
{
    public class MagBool
    {
        private MagratheaExecutor executor;
        private StringComparison ignoringCase = StringComparison.InvariantCultureIgnoreCase;

        public MagBool(MagratheaExecutor parser)
        {
            this.executor = parser;
        }

        public Object boolMethod(XElement boolElement)
        {
            // find type of element that called the Boolean object
            String elementType = boolElement.Name.ToString();
            Object toReturn = null;

            if (elementType.Equals("boolean"))
            {
                toReturn = boolFunction(boolElement);
            }
            else if (elementType.Equals("setPhaseValue"))
            {
                toReturn = setValue(boolElement);
            }
            return toReturn;

        }

        private Object setValue(XElement element)
        {
            Boolean? toReturn = null;

            Boolean childVariable = false;
            // check if the set has children
            List<XElement> stringChildren = element.Elements().ToList();
            if (stringChildren.Count > 0)
            {
                childVariable = true;
                Console.WriteLine("Internal Bool Variable");
                // do child variable to get value
                toReturn = (Boolean)executor.ExecuteElement(stringChildren[0], "");
                Console.WriteLine(toReturn);
            }
            // if no child, return value
            if (!childVariable)
            {
                toReturn = Boolean.Parse(element.FirstNode.ToString());
            }
            return toReturn;
        }

        private Object boolFunction(XElement element)
        {
            // find the type of function needed
            String type = element.Attribute("type").Value;
            Console.WriteLine(type);
            Object toReturn = null;
            // operate on specific function
            if (type.Equals("booleanCompare", ignoringCase))
            {
                toReturn = compareBooleans(element);
            }
            return toReturn;
        }

        private Object compareBooleans(XElement element)
        {
            // get target variable to set when compared

            String target;
            try { target = element.Attribute("target").Value; } catch { }

            // get condition to compare to
            Boolean condition = Boolean.Parse(element.Attribute("condition").Value);

            // get things to compare
            List<Boolean> boolsToCompare = new List<bool>();
            List<XElement> childElements = element.Elements().ToList();
            Boolean toReturn = false;
            foreach (XElement variable in childElements)
            {
                Boolean value = (Boolean)executor.ExecuteElement(variable, "");
                boolsToCompare.Add(value);
            }

            // find type of comparison
            String compare = element.Attribute("comparator").Value;
            if (compare.Equals("and", ignoringCase))
            {
                foreach (Boolean value in boolsToCompare)
                {
                    if (value == !condition)
                    {
                        toReturn = false;
                        break;
                    }
                    else
                    {
                        toReturn = true;
                    }
                }
            }
            else if (compare.Equals("or", ignoringCase))
            {
                foreach (Boolean value in boolsToCompare)
                {
                    if (value == condition)
                    {
                        toReturn = true;
                        break;
                    }
                    else
                    {
                        toReturn = false;
                    }
                }
            }
            return toReturn;
        }
    }
}
