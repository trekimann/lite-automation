using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Magrathea.objects
{
    public class MagString
    {
        private MagratheaExecutor parser;
        private StringComparison ignoringCase = StringComparison.InvariantCultureIgnoreCase;

        public MagString(MagratheaExecutor parser)
        {
            this.parser = parser;
        }

        public Object stringMethod(XElement element)
        {
            // find type of element that triggered string
            String elementType = element.Name.ToString();
            Object toReturn = null;

            if (elementType.Equals("string", ignoringCase))
            {
                toReturn = stringFunction(element);
            }
            else if (elementType.Equals("setPhaseValue", ignoringCase))
            {
                toReturn = setValue(element);
            }
            return toReturn;
        }

        private Object stringFunction(XElement element)
        {
            // find the function required
            String type = element.Attribute("type").Value;
            String toReturn = null;
            //operate on the function
            if (type.Equals("join", ignoringCase))
            {
                toReturn = stringJoin(element);
            }
            if (type.Equals("string", ignoringCase))
            {
                toReturn = stringString(element);
            }

            return toReturn;
        }

        private String stringString(XElement element)
        {
            String toReturn = "";
            // get the string source
            String source = null;
            try { source = element.Attribute("source").Value; }
            catch {}
            //check if the string source points somewhere
            if (source != null && source != "")
            {
                //get string value
                toReturn = (String)parser.DataValuesMap(source).Value;
            }
            //add any hardcoded text
            String text = "";
            try { text = element.FirstNode.ToString();
                //remove special character formating
                if (text !="" && text != null)
                {
                    text = text.Replace("&amp;", "&");
                }

            } catch { }
          
            toReturn = toReturn + text;
            return toReturn;
        }

        private String stringJoin(XElement element)
        {
            // get children to operate on
            List<XElement> stringChild = element.Elements().ToList();
            String toReturn = "";

            foreach (XElement stringElement in stringChild)
            {
                Object value = parser.ExecuteElement(stringElement, "");
                //check type of object returned
                String cast = value.GetType().ToString();
                if (cast.Equals("System.String", ignoringCase))
                {
                    toReturn = toReturn + (String)value;
                }
                else if (cast.Equals("System.Int32", ignoringCase))
                {
                    toReturn = toReturn + (Int32)value;
                }
                else if (cast.Equals("System.Boolean", ignoringCase))
                {
                    toReturn = toReturn + (Boolean)value;
                }
                else if (cast.Equals("System.DateTime", ignoringCase))
                {
                    DateTime time = (DateTime)value;
                    toReturn = toReturn + time;
                }
            }
            return toReturn;
        }

        private String setValue(XElement element)
        {
            // check for string to set and type
            //		String type = element.Attribute("type");
            //		String name = element.Attribute("name");
            String toReturn = "";

            // check for children
            Boolean childVariable = false;
            // check if the set has children
            List<XElement> stringChildren = element.Elements().ToList();
            if (stringChildren.Count > 0)
            {
                childVariable = true;
                Console.WriteLine("Internal Variable");
                // do child variable to get value
                toReturn = (String)parser.ExecuteElement(stringChildren[0], "");
                Console.WriteLine(toReturn);
            }

            //if no child, return value
            if (!childVariable)
            {

                try { toReturn = element.FirstNode.ToString(); } catch { }
            }
            return toReturn;
        }
    }
}
