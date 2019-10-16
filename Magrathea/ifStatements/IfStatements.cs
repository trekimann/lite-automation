using Magrathea.ElementOperations;
using Magrathea.Executors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Magrathea.ifStatements
{
    public class IfStatements : IIfStatements
    {
        public IMagratheaExecutor executor { get; set; }
        private StringComparison ignoringCase = StringComparison.InvariantCultureIgnoreCase;
        private XElement ifFunction;
        public Boolean resultOfCompare { get; private set; } = false;

        public String methodsCalled { get; private set; } = "";//used for testing paths calls take

        public IfStatements(IMagratheaExecutor executor)
        {
            this.executor = executor;
        }
        public IfStatements()
        { }

        public void If(XElement ifFunction)
        {
            methodsCalled += "If,";

            this.ifFunction = ifFunction;
            String typeOfIf = ifFunction.Attribute("type").Value;
            String condition = ifFunction.Attribute("condition").Value;
            String variable = ifFunction.FirstNode.ToString().Trim();
            Object value =null;

            if (variable.Equals("feelinglucky", ignoringCase))
            {
                Console.WriteLine("mag debug point");
            }

            typeObjectPair returned = null;
            returned = executor.DataValuesMap(variable);

            if (returned != null)
            {
                value = returned.Value;
            }

            // check if types conditions
            if (typeOfIf.Equals("ifNull", ignoringCase) && returned != null)
            {
                resultOfCompare = NullCompare((Boolean.Parse(condition)), value);
            }

            if (value == null && (!typeOfIf.Equals("ifNull", ignoringCase)))
            {
                return;
            }
            else if (typeOfIf.Equals("ifBoolean", ignoringCase))
            {
                Boolean conditionBool = Boolean.Parse(condition);
                resultOfCompare = BooleanCompare(conditionBool, (Boolean)value);
            }
            else if (typeOfIf.Equals("ifString", ignoringCase))
            {
                String stringToMatch = "";
                try
                {//if its a fixed variable
                    stringToMatch = ifFunction.Attribute("stringsToMatch").Value;
                }
                catch (Exception e) { }
                try
                {//if its a phaseValue
                    String phaseValue = ifFunction.Attribute("phaseValueToMatch").Value;
                    stringToMatch = executor.DataValuesMap(phaseValue).Value.ToString();
                }
                catch (Exception e) { }

                resultOfCompare = StringCompare(condition, (String)value, stringToMatch);
            }
            else if (typeOfIf.Equals("ifInt", ignoringCase))
            {
                String intAt = ifFunction.Attribute("int").Value;
                Console.WriteLine(intAt);
                int intToCompare = Int32.Parse(intAt);
                resultOfCompare = IntCompare(condition, (int)value, intToCompare);
            }            
        }

        public Boolean BooleanCompare(Boolean desiredCondition, Boolean actualValue)
        {
            methodsCalled += "BooleanCompare,";
            if (desiredCondition == actualValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public Boolean StringCompare(String condition, String value, String stringToMatch)
        {
            methodsCalled += "StringCompare,";
            Boolean orSplit = false;
            // Boolean andSplit = false;
            List<String> stringsToMatch = new List<string>();

            // check if string need splitting
            if (stringToMatch.Contains("||"))
            { // contains or
                stringsToMatch = stringToMatch.Split(new String[] { "||" },StringSplitOptions.None).ToList();
                orSplit = true;
            }
            // else if (stringToMatch.Contains("&&")) { // contains &&
            // stringsToMatch = Arrays.asList(stringToMatch.split("\\&\\&"));
            // andSplit = true;
            // }

            // or compare
            Boolean orMatch = false;
            Boolean andMatch;
            if (orSplit)
            {
                foreach (String stringValue in stringsToMatch)
                {
                    orMatch = SingleStringCompare(condition, value, stringValue);
                    if (orMatch)
                    {
                        return true;
                    }
                }
            }

            // and compare
            // else if (andSplit) {
            // for (String string : stringsToMatch) {
            // andMatch = StringCompare(condition, value, string);
            // if (!andMatch) {
            // return false;
            // }
            // return andMatch;
            // }
            // }

            // single string
            else
            {
                return SingleStringCompare(condition, value, stringToMatch);
            }
            return false;
        }

        public Boolean SingleStringCompare(String condition, String value, String stringToMatch)
        {
            methodsCalled += "SingleStringCompare,";
            if (condition.Equals("equals", ignoringCase))
            {
                if (value.Equals(stringToMatch))
                {
                    return true;
                }
            }
            else if(condition.Equals("doesNotEqual",ignoringCase))
            {
                if(!value.Equals(stringToMatch))
                {
                    return true;
                }
            }
            else if (condition.Equals("startsWith", ignoringCase))
            {
                if (value.StartsWith(stringToMatch))
                {
                    return true;
                }
            }else if(condition.Equals("contains", ignoringCase))
            {
                if(value.Contains(stringToMatch))
                {
                    return true;
                }
            }else if (condition.Equals("doesNotContain",ignoringCase))
            {
                if(!value.Contains(stringToMatch))
                {
                    return true;
                }
            }
            return false;
        }

        public Boolean NullCompare(Boolean condition, Object value)
        {
            methodsCalled += "NullCompare,";
            if (condition)
            {
                if (value == null)
                {
                    return true;
                }
            }
            else
            {
                if (value != null)
                {
                    return true;
                }
            }
            return false;
        }

        public Boolean IntCompare(String condition, int valueFromMap, int intToCompareAgainst)
        {
            methodsCalled += "IntCompare,";
            if (condition.Equals("lessThan", ignoringCase))
            {
                if (valueFromMap < intToCompareAgainst)
                {
                    return true;
                }
            }
            else if (condition.Equals("lessThanOrEqual", ignoringCase))
            {
                if (valueFromMap <= intToCompareAgainst)
                {
                    return true;
                }
            }
            else if (condition.Equals("moreThan", ignoringCase))
            {
                if (valueFromMap > intToCompareAgainst)
                {
                    return true;
                }
            }
            else if (condition.Equals("moreThanOrEqual", ignoringCase))
            {
                if (valueFromMap >= intToCompareAgainst)
                {
                    return true;
                }
            }
            else if (condition.Equals("equalTo", ignoringCase))
            {
                if (valueFromMap == intToCompareAgainst)
                {
                    return true;
                }
            }
            else if (condition.Equals("notEqualTo", ignoringCase))
            {
                if (valueFromMap != intToCompareAgainst)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
