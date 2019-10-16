using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Magrathea.whileStatements
{
    public class WhileStatements
    {
        private MagratheaExecutor executor;
        private StringComparison ignoringCase = StringComparison.InvariantCultureIgnoreCase;

        public WhileStatements(MagratheaExecutor xmlParser)
        {
            this.executor = xmlParser;
        }

        public void execute(XElement element)
        {
            Console.WriteLine("Starting while");
            // find type of while loop
            String type = element.Attribute("type").Value;

            // operate on the type of while loop
            if (type.Equals("whileBoolean", ignoringCase))
            {
                booleanWhile(element);
            }
            Console.WriteLine("While ended");
        }

        private void booleanWhile(XElement element)
        {
            // find the target boolean condition
            String target = element.FirstNode.ToString().Trim();
            String condition = element.Attribute("condition").Value;
            Boolean whileBool = Boolean.Parse(condition);
            //		Boolean whileBool = (Boolean) executor.getDataValuesMap()[target).getValue();

            // check the condition to match
            if (condition.Equals("true", ignoringCase))
            {
                while (whileBool)
                {
                    // get child list to execute
                    List<XElement> inside = element.Elements().ToList();
                    executor.ExecuteFunction(inside, "");
                    whileBool = (Boolean)executor.DataValuesMap(target).Value;
                }
            }
            else if (condition.Equals("false", ignoringCase))
            {
                while (!whileBool)
                {
                    // get child list to execute
                    List<XElement> inside = element.Elements().ToList();
                    executor.ExecuteFunction(inside, "");
                    whileBool = (Boolean)executor.DataValuesMap(target).Value;
                }
            }
        }
    }
}
