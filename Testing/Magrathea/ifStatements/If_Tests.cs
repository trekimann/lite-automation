using Magrathea.ElementOperations;
using Magrathea.Executors;
using Magrathea.ifStatements;
using Moq;
using NUnit.Framework;
using System;
using System.Xml.Linq;

namespace Testing.Magrathea.ifStatements
{
    [TestFixture]
    class If_Tests
    {
        IfStatements ifs;
        XElement ifFunction;
        Mock<IMagratheaExecutor> MockExecutor;
        
        [SetUp]
        public void init()
        {
            ifs = new IfStatements();
        }

        [TearDown]
        public void tear()
        {
            ifs = null;
            ifFunction = null;
            MockExecutor = null;
        }

        public void makeIfElement(String type,String condition,object variableValue,String variableCast,String stringTomatch=null,string intToCompare = null)
        {
            ifFunction = new XElement("if");
            ifFunction.Add(new XAttribute("type", type));
            ifFunction.Add(new XAttribute("condition", condition));

            if(stringTomatch!=null)
            {
                ifFunction.Add(new XAttribute("stringsToMatch", stringTomatch));
            }
            if(intToCompare!=null)
            {
                ifFunction.Add(new XAttribute("int", intToCompare));
            }

            if(variableValue==null)
            {
                variableValue = "null";
            }

            ifFunction.Value = variableValue.ToString();

            if(variableValue=="null")
            {
                variableValue = null;
            }
            //setup moq
            MockExecutor = new Mock<IMagratheaExecutor>();
            typeObjectPair toReturn = new typeObjectPair(variableCast, variableValue);

            MockExecutor.Setup(m => m.DataValuesMap (It.IsAny<String>() ) ).Returns(toReturn);

            ifs.executor = MockExecutor.Object;
        }

        [Test]
        public void IfTypeIsString_StringCompareMethodIsCalled()
        {
            String type = "ifString";
            String condition = "equals";
            String variableValue = "Match";
            String variableCast = "string";
            String stringTomatch = "Match";

            makeIfElement(type, condition, variableValue, variableCast, stringTomatch);

            ifs.If(ifFunction);

            //check that the StringCompare function was called
            String desiredMethod = "StringCompare";
            Boolean methodCalled = ifs.methodsCalled.Contains(desiredMethod);
            Assert.AreEqual(true, methodCalled);
        }

        [Test]
        public void IfTypeIsInt_IntCompareMethodIsCalled()
        {
            String type = "ifInt";
            String condition = "equalTo";
            int variableValue = 10;
            String variableCast = "int";
            String intToCompare = "10";

            makeIfElement(type, condition, variableValue, variableCast,null,intToCompare);

            ifs.If(ifFunction);

            //check that the StringCompare function was called
            String desiredMethod = "IntCompare";
            Boolean methodCalled = ifs.methodsCalled.Contains(desiredMethod);
            Assert.AreEqual(true, methodCalled);
        }

        [Test]
        public void IfTypeIsBoolean_IntCompareMethodIsCalled()
        {
            String type = "ifBoolean";
            String condition = "true";
            Boolean variableValue = true;
            String variableCast = "boolean";

            makeIfElement(type, condition, variableValue, variableCast);

            ifs.If(ifFunction);

            //check that the StringCompare function was called
            String desiredMethod = "BooleanCompare";
            Boolean methodCalled = ifs.methodsCalled.Contains(desiredMethod);
            Assert.AreEqual(true, methodCalled);
        }

        [Test]
        public void IfTypeIsNull_NullCompareMethodIsCalled()
        {
            String type = "ifNull";
            String condition = "true";
            Object variableValue = null;
            String variableCast = "null";

            makeIfElement(type, condition, variableValue, variableCast);

            ifs.If(ifFunction);

            //check that the StringCompare function was called
            String desiredMethod = "NullCompare";
            Boolean methodCalled = ifs.methodsCalled.Contains(desiredMethod);
            Assert.AreEqual(true, methodCalled);
        }
    }
}
