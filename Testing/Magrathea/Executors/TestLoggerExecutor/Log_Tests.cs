using Magrathea.Executors;
using Moq;
using NUnit.Framework;
using Selenium.testLogger;
using System;
using System.Xml.Linq;

namespace Testing.Magrathea.Executors.TestLoggerExecutor_Tests
{
    [TestFixture]
    class Log_Tests
    {
        TestLoggerExecutor loggerExecutor;
        XElement loggerFunction;

        Mock<ITestLogger> MockTestLogger;
        Mock<IMagratheaExecutor> MockMagExe;

        ITestLogger Logger;
        IMagratheaExecutor MagExe;

        [SetUp]
        public void Setup()
        {
            MockTestLogger = new Mock<ITestLogger>();
            Logger = MockTestLogger.Object;

            MockMagExe = new Mock<IMagratheaExecutor>();
            MagExe = MockMagExe.Object;

            loggerExecutor = new TestLoggerExecutor(Logger, MagExe);
        }

        [TearDown]
        public void Tear()
        {
            MockTestLogger = null;
            loggerExecutor = null;
            loggerFunction = null;
            Logger = null;
            MagExe = null;
        }

        public void BuildLogFunction(String type, String TextToRecord = null, String Status = null, XElement getPhaseValue = null)
        {
            loggerFunction = new XElement("testLogger");
            loggerFunction.Add(new XAttribute("type", type));

            if (TextToRecord != null)
            {
                loggerFunction.Value = TextToRecord;
            }
            if (Status != null)
            {
                loggerFunction.Add(new XAttribute("status", Status));
            }
            if (getPhaseValue != null)
            {
                loggerFunction.Add(getPhaseValue);
            }
        }

        [Test]
        public void TypeIsScreenShot_LoggerScreenShotIsCalled()
        {
            String type = "screenshot";
            BuildLogFunction(type);

            loggerExecutor.Log(loggerFunction);

            MockTestLogger.Verify(m => m.screenshot(It.IsAny<String>()));
        }

        [Test]
        public void TypeIsRecordToFile_Logger_recordToFile_IsCalled()
        {
            String type = "recordToFile";
            String messageToLog = "test Message";
            BuildLogFunction(type,messageToLog);

            loggerExecutor.Log(loggerFunction);

            MockTestLogger.Verify(m => m.recordOutcome(It.IsAny<String>(), (It.IsAny<String>())));
        }

        [Test]
        public void TypeIsScreenshot_Logger_ScreenshotIsCalledWithAttachedMessage()
        {
            String type = "screenshot";
            String messageToLog = "test Message";
            BuildLogFunction(type, messageToLog);

            loggerExecutor.Log(loggerFunction);

            MockTestLogger.Verify(m => m.screenshot(messageToLog));
        }
    }
}
