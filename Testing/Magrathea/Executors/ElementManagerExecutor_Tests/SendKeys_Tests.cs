using Magrathea.ElementOperations;
using Magrathea.Executors;
using Moq;
using NUnit.Framework;
using Selenium.htmlElementManager;
using Selenium.testLogger;
using System;
using System.Xml.Linq;

namespace Testing.Magrathea.Executors.ElementManagerExecutor_Tests
{
    [TestFixture]
    class SendKeys_Tests
    {
        IHtmlElementManager ElementManager;
        IMagratheaExecutor MagExe;
        ITestLogger Logger;
        ElementManagerExecutor ElementExe;
        
        Mock<IHtmlElementManager> MockElementManager;
        Mock<IMagratheaExecutor> MockMagExe;
        Mock<ITestLogger> MockTestLogger;

        //XElement managerElement;

        [SetUp]
        public void Setup()
        {
            MockElementManager = new Mock<IHtmlElementManager>();
            MockMagExe = new Mock<IMagratheaExecutor>();
            MockTestLogger = new Mock<ITestLogger>();

            ElementManager = MockElementManager.Object;
            MagExe = MockMagExe.Object;
            Logger = MockTestLogger.Object;

            ElementExe = new ElementManagerExecutor(Logger, MagExe, ElementManager);
        }

        [TearDown]
        public void Tear()
        {
            ElementManager=null;
            MagExe = null;
            Logger = null;
            ElementExe = null;

            MockElementManager = null;
            MockMagExe = null;
            MockTestLogger = null;
        }

        public void BuildDataValue(Object value, String cast, String PhaseValue = null)
        {
            typeObjectPair toReturn = new typeObjectPair(cast, value);

            if (PhaseValue != null)
            {
                MockMagExe.Setup(m => m.DataValuesMap(PhaseValue)).Returns(toReturn);
            }
            else
            {
                MockMagExe.Setup(M => M.DataValuesMap(It.IsAny<String>())).Returns(toReturn);
            }
        }
        //public void BuildElementFunction(String type, String target,String phaseValue=null)
        //{
        //    managerElement = new XElement("elementManager");
        //    managerElement.Add(new XAttribute("type", type));
        //    managerElement.Add(new XAttribute("target", target));
        //    if (phaseValue != null)
        //    {
        //        managerElement.Value = phaseValue;
        //    }
        //}

        //--------------------Testing converting phasevalues into strings
        [Test]
        public void BooleanPhaseValue_CorrectlyTurnedIntoString()
        {
            String type = "sendKeys";
            String target = "ElementToInteractWith";
            String variable = "PhaseValueToSend";

            Boolean value = true;
            String cast = "boolean";
            BuildDataValue(value, cast, variable);

            var result = ElementExe.SendKeys(target, variable, type);

            Assert.AreEqual("True", result);
        }

        [Test]
        public void IntPhaseValue_CorrectlyTurnedIntoString()
        {
            String type = "sendKeys";
            String target = "ElementToInteractWith";
            String variable = "PhaseValueToSend";

            int value = 100;
            String cast = "int";
            BuildDataValue(value, cast, variable);

            var result = ElementExe.SendKeys(target, variable, type);

            Assert.AreEqual("100", result);
        }
        [Test]
        public void StringPhaseValue_CorrectlyTurnedIntoString()
        {
            String type = "sendKeys";
            String target = "ElementToInteractWith";
            String variable = "PhaseValueToSend";

            String value = "String To Send";
            String cast = "string";
            BuildDataValue(value, cast, variable);

            var result = ElementExe.SendKeys(target, variable, type);

            Assert.AreEqual("String To Send", result);
        }
        
        //--------------------Testing converting phasevalues into strings


        //---------------Testing all the different type of sends keys paths------------
        [Test]
        public void SendKeysClear_CallsSendKeysClear_WithCorrectTarget()
        {
            String type = "sendKeysClear";
            String target = "ElementToInteractWith";
            String variable = "PhaseValueToSend";

            ElementExe.SendKeys(target, variable, type);

            MockElementManager.Verify(m => m.SendKeysClear(target));
        }       

        [Test]
        public void SendKeys_CallsSendKeys_WithCorrectTarget_CorrectValue()
        {
            String type = "sendKeys";
            String target = "ElementToInteractWith";
            String variable = "PhaseValueToSend";

            String value = "String Value";
            String cast = "string";
            BuildDataValue(value,cast,variable);

            ElementExe.SendKeys(target, variable, type);

            MockElementManager.Verify(m => m.SendKeys(target,value));
        }
        [Test]
        public void SendKeysTab_CallsSendKeys_WithCorrectTarget_CorrectValue()
        {
            String type = "sendKeysTab";
            String target = "ElementToInteractWith";
            String variable = "PhaseValueToSend";

            String value = "String Value";
            String cast = "string";
            BuildDataValue(value,cast,variable);

            ElementExe.SendKeys(target, variable, type);

            MockElementManager.Verify(m => m.SendKeysTab(target,value));
        }
        [Test]
        public void SendKeysNoCheck_CallsSendKeysNoCheck_WithCorrectTarget_CorrectValue()
        {
            String type = "SendKeysNoCheck";
            String target = "ElementToInteractWith";
            String variable = "PhaseValueToSend";

            String value = "String Value";
            String cast = "string";
            BuildDataValue(value,cast,variable);

            ElementExe.SendKeys(target, variable, type);

            MockElementManager.Verify(m => m.SendKeysNoCheck(target,value));
        }
        [Test]
        public void SendKeysSlow_CallsSendKeysSlow_WithCorrectTarget_CorrectValue()
        {
            String type = "SendKeysSlow";
            String target = "ElementToInteractWith";
            String variable = "PhaseValueToSend";

            String value = "String Value";
            String cast = "string";
            BuildDataValue(value,cast,variable);

            ElementExe.SendKeys(target, variable, type);

            MockElementManager.Verify(m => m.SendKeysSlow(target,value));
        }
    }
}
