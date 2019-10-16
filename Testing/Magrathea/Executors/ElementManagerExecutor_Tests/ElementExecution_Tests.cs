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
    class ElementExecution_Tests
    {
        IHtmlElementManager ElementManager;
        IMagratheaExecutor MagExe;
        ITestLogger Logger;
        ElementManagerExecutor ElementExe;

        Mock<IHtmlElementManager> MockElementManager;
        Mock<IMagratheaExecutor> MockMagExe;
        Mock<ITestLogger> MockTestLogger;

        XElement managerElement;

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
            ElementManager = null;
            MagExe = null;
            Logger = null;
            ElementExe = null;

            MockElementManager = null;
            MockMagExe = null;
            MockTestLogger = null;
        }
                
        public void BuildElementFunction(String type, String target, String phaseValue = null)
        {
            managerElement = new XElement("elementManager");
            managerElement.Add(new XAttribute("type", type));
            managerElement.Add(new XAttribute("target", target));
            if (phaseValue != null)
            {
                managerElement.Value = phaseValue;
            }
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


        //-----------------Checking the type directs to the right option
        [Test]
        public void typeStartsWith_SendKeys_DirectsTo_SendKeys()
        {
            String type = "SendKeys";
            String target = "TargetElement";
            String phaseValue = "phaseValue";

            Boolean value = true;
            String cast = "boolean";
            BuildDataValue(value, cast, phaseValue);

            BuildElementFunction(type, target,phaseValue);
            ElementExe.ElementExecution(managerElement);
            MockElementManager.Verify(m => m.SendKeys(target, It.IsAny<String>()));
        }

        [Test]
        public void typeSelectFirstOption_DirectsTo_SelectFirstOption()
        {
            String type = "SelectFirstOption";
            String target = "TargetElement";
            String phaseValue = "phaseValue";

            Boolean value = true;
            String cast = "boolean";
            //BuildDataValue(value, cast, phaseValue); sdfgdg

            BuildElementFunction(type, target,phaseValue);
            ElementExe.ElementExecution(managerElement);
            MockElementManager.Verify(m => m.FirstSelection(managerElement));
        }

        [Test]
        public void type_click_DirectsTo_click()
        {
            String type = "click";
            String target = "TargetElement";
            String phaseValue = "phaseValue";

            Boolean value = true;
            String cast = "boolean";
            //BuildDataValue(value, cast, phaseValue); sdfgdg

            BuildElementFunction(type, target, phaseValue);
            ElementExe.ElementExecution(managerElement);
            MockElementManager.Verify(m => m.Click(target));
        }

        [Test]
        public void type_pause_DirectsTo_pause()
        {
            String type = "pause";
            String target = "TargetElement";
            String phaseValue = "phaseValue";

            string value = "1000";

            BuildElementFunction(type, target, phaseValue);
            managerElement.Add(new XAttribute("value", value));

            ElementExe.ElementExecution(managerElement);
            MockElementManager.Verify(m => m.DirtyPause(It.IsAny<Int32>()));
        }

        [Test]
        public void type_pageCheck_DirectsTo_allChecks()
        {
            String type = "pageCheck";
            String target = "TargetElement";
            String phaseValue = "phaseValue";

            BuildElementFunction(type, target, phaseValue);
            ElementExe.ElementExecution(managerElement);
            MockElementManager.Verify(m => m.allChecks());
        }

        [Test]
        public void type_getText_DirectsTo_getText()
        {
            String type = "getText";
            String target = "TargetElement";
            String phaseValue = "phaseValue";

            BuildElementFunction(type, target, phaseValue);
            ElementExe.ElementExecution(managerElement);
            MockElementManager.Verify(m => m.GetText(managerElement));
        }

        [Test]
        public void type_elementAvailable_DirectsTo_elementAvailable()
        {
            String type = "elementAvailable";
            String target = "TargetElement";
            String phaseValue = "phaseValue";

            BuildElementFunction(type, target, phaseValue);
            ElementExe.ElementExecution(managerElement);
            MockElementManager.Verify(m => m.ElementAvailable(target));
        }

    }
}
