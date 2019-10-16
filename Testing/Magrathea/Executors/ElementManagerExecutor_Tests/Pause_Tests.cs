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
    class Pause_Tests
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

        //--------------------------------------------------------------------------

        //------------------------------------Happy---------------------------------

        [Test]
        public void pause_withIntNumber_pausesForSpecificedTime()
        {
            String type = "pause";
            String target = "TargetElement";
            String phaseValue = "phaseValue";

            string value = "1000";

            BuildElementFunction(type, target, phaseValue);
            managerElement.Add(new XAttribute("value", value));

            ElementExe.ElementExecution(managerElement);
            MockElementManager.Verify(m => m.DirtyPause(1000));
        }       

        [Test]
        public void pause_indefinite_sendsSystemMessage()
        {
            String type = "pause";
            String target = "TargetElement";
            String phaseValue = "phaseValue";

            string value = "indefinite";

            BuildElementFunction(type, target, phaseValue);
            managerElement.Add(new XAttribute("value", value));

            ElementExe.ElementExecution(managerElement);
            MockTestLogger.Verify(m => m.sendSystemMessage(It.IsAny<string>()));
        }

        [Test]
        public void pause_indefinite_calledRecordOutomce()
        {
            String type = "pause";
            String target = "TargetElement";
            String phaseValue = "phaseValue";

            string value = "indefinite";

            BuildElementFunction(type, target, phaseValue);
            managerElement.Add(new XAttribute("value", value));

            ElementExe.ElementExecution(managerElement);
            MockTestLogger.Verify(m => m.recordOutcome(It.IsAny<string>(), It.IsAny<string>()));
        }

        [Test]
        public void pause_indefinite_callsSwitchPause()
        {
            String type = "pause";
            String target = "TargetElement";
            String phaseValue = "phaseValue";

            string value = "indefinite";

            BuildElementFunction(type, target, phaseValue);
            managerElement.Add(new XAttribute("value", value));

            ElementExe.ElementExecution(managerElement);
            MockMagExe.Verify(m => m.switchPause());
        }

        //------------------------------------Happy---------------------------------

        //---------------------------------Problems---------------------------------

        [Test]
        public void pause_withNonIntNumber_pausesIndefinately()
        {
            String type = "pause";
            String target = "TargetElement";
            String phaseValue = "phaseValue";

            string value = "1000.5";

            BuildElementFunction(type, target, phaseValue);
            managerElement.Add(new XAttribute("value", value));

            ElementExe.ElementExecution(managerElement);
            MockElementManager.Verify(m => m.DirtyPause(It.IsAny<Int32>()), Times.Never);//verify that the pause is not called

            MockTestLogger.Verify(m => m.recordOutcome(It.IsAny<string>(), "Warning"));//make sure warning is logged

            MockMagExe.Verify(m => m.switchPause());
        }

        [Test]
        public void pause_stringValue_pausesIndefinately()
        {
            String type = "pause";
            String target = "TargetElement";
            String phaseValue = "phaseValue";

            string value = "incorrectValueType";

            BuildElementFunction(type, target, phaseValue);
            managerElement.Add(new XAttribute("value", value));

            ElementExe.ElementExecution(managerElement);
            MockElementManager.Verify(m => m.DirtyPause(It.IsAny<Int32>()), Times.Never);//verify that the pause is not called

            MockTestLogger.Verify(m => m.recordOutcome(It.IsAny<string>(), "Warning"));//make sure warning is logged

            MockMagExe.Verify(m => m.switchPause());
        }
        //---------------------------------Problems---------------------------------
    }
}
