using Magrathea.ElementOperations;
using Magrathea.Executors;
using Moq;
using NUnit.Framework;
using Selenium.testLogger;
using System;
using System.Xml.Linq;

namespace Testing.Magrathea.Executors.TestLoggerExecutor_Tests
{
    [TestFixture]
    class LogMessage_Tests
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

        public void BuildLogFunction(String type, String TextToRecord = null, String Status = null,XElement getPhaseValue=null)
        {
            loggerFunction = new XElement("testLogger");
            loggerFunction.Add(new XAttribute("type", type));

            if (TextToRecord!=null)
            {
                loggerFunction.Value = TextToRecord;
            }
            if (Status!=null)
            {
                loggerFunction.Add(new XAttribute("status", Status));
            }
            if(getPhaseValue!=null)
            {
                loggerFunction.Add(getPhaseValue);
            }
        }

        public void BuildDataValue(Object value,String cast,XElement getPhaseValue=null)
        {
            typeObjectPair toReturn = new typeObjectPair(cast, value);

            if(getPhaseValue != null)
            {
                MockMagExe.Setup(m => m.GetPhaseValue(getPhaseValue)).Returns(toReturn.Value);
            }
            else
            {
                MockMagExe.Setup(M => M.GetPhaseValue(It.IsAny<XElement>())).Returns(toReturn.Value);
            }
        }

        [Test]
        public void LoggedMessageContainsTheInfoToLog()
        {
            String type = "recordToFile";
            String messageToLog = "test Message";
            BuildLogFunction(type,messageToLog);

            var result  = loggerExecutor.LogMessage(loggerFunction);

            //verify that TestLogger.recordOutcome was called with correct data
            MockTestLogger.Verify(m => m.recordOutcome(messageToLog,It.IsAny<String>()));
            Assert.AreEqual(messageToLog,result);
        }

        [Test]
        public void IfStatusSpecified_UsedInLog()
        {
            String type = "recordToFile";
            String messageToLog = "test Message";
            String status = "DifferentStatus";
            BuildLogFunction(type,messageToLog,status);

            var result  = loggerExecutor.LogMessage(loggerFunction);

            //verify that TestLogger.recordOutcome was called with correct data
            MockTestLogger.Verify(m => m.recordOutcome(messageToLog,status));
        }

        [Test]
        public void IfStatusNotSpecified_info_UsedInLog()
        {
            String type = "recordToFile";
            String messageToLog = "test Message";
            BuildLogFunction(type,messageToLog);

            var result  = loggerExecutor.LogMessage(loggerFunction);

            //verify that TestLogger.recordOutcome was called with correct data
            MockTestLogger.Verify(m => m.recordOutcome(messageToLog,"info"));
        }

        //---------Testing child value writing----------
        [Test]
        public void NestedValue_IsString_LogsString()
        {
            String type = "recordToFile";
            String messageToLog = "";

            String storedValue = "StoredValueToReturn";
            String storedCast = "System.String";

            var getPhaseValue = new XElement("getPhaseValue");
            getPhaseValue.Add(new XAttribute("target", "none"));

            BuildDataValue(storedValue, storedCast);
            BuildLogFunction(type,messageToLog,null, getPhaseValue);

            var result  = loggerExecutor.LogMessage(loggerFunction);

            //verify that TestLogger.recordOutcome was called with correct data
            MockTestLogger.Verify(m => m.recordOutcome(" " + storedValue, It.IsAny<String>()));
            Assert.AreEqual(" "+storedValue, result);
        }

        [Test]
        public void NestedValue_IsInt_LogsInt()
        {
            String type = "recordToFile";
            String messageToLog = "";

            int storedValue = 100;
            String storedCast = "System.Int32";

            var getPhaseValue = new XElement("getPhaseValue");
            getPhaseValue.Add(new XAttribute("target", "none"));

            BuildDataValue(storedValue, storedCast);
            BuildLogFunction(type,messageToLog,null, getPhaseValue);

            var result  = loggerExecutor.LogMessage(loggerFunction);

            //verify that TestLogger.recordOutcome was called with correct data
            MockTestLogger.Verify(m => m.recordOutcome(" " + storedValue, It.IsAny<String>()));
            Assert.AreEqual(" "+storedValue, result);
        }

        [Test]
        public void NestedValue_IsBool_LogsBool()
        {
            String type = "recordToFile";
            String messageToLog = "";

            Boolean storedValue = true;
            String storedCast = "System.Boolean";

            var getPhaseValue = new XElement("getPhaseValue");
            getPhaseValue.Add(new XAttribute("target", "none"));

            BuildDataValue(storedValue, storedCast);
            BuildLogFunction(type,messageToLog,null, getPhaseValue);

            var result  = loggerExecutor.LogMessage(loggerFunction);

            //verify that TestLogger.recordOutcome was called with correct data
            MockTestLogger.Verify(m => m.recordOutcome(" " + storedValue, It.IsAny<String>()));
            Assert.AreEqual(" "+storedValue, result);
        }

        [Test]
        public void NestedValue_IsDateTime_LogsDateTime()
        {
            String type = "recordToFile";
            String messageToLog = "";

            String formatedDate = "20-12-2018 12:40:00";
            String dateToParse = formatedDate.Replace("-", "/");
            DateTime storedValue = DateTime.Parse(dateToParse);
            String storedCast = "System.DateTime";

            var getPhaseValue = new XElement("getPhaseValue");
            getPhaseValue.Add(new XAttribute("target", "none"));

            BuildDataValue(storedValue, storedCast);
            BuildLogFunction(type,messageToLog,null, getPhaseValue);

            var result  = loggerExecutor.LogMessage(loggerFunction);

            //verify that TestLogger.recordOutcome was called with correct data
            MockTestLogger.Verify(m => m.recordOutcome(" " + formatedDate, It.IsAny<String>()));
            Assert.AreEqual(" "+ formatedDate, result);
        }

        [Test]
        public void NestedValueAndMessageInPlainText_LogsCombined()
        {
            String type = "recordToFile";
            String messageToLog = "This Part First.";

            String storedValue = "This Part Second";
            String storedCast = "System.String";

            var getPhaseValue = new XElement("getPhaseValue");
            getPhaseValue.Add(new XAttribute("target", "none"));

            BuildDataValue(storedValue, storedCast);
            BuildLogFunction(type, messageToLog, null, getPhaseValue);

            var result = loggerExecutor.LogMessage(loggerFunction);

            //verify that TestLogger.recordOutcome was called with correct data
            MockTestLogger.Verify(m => m.recordOutcome(messageToLog+" " + storedValue, It.IsAny<String>()));
            Assert.AreEqual(messageToLog + " " + storedValue, result);
        }

        //---------Testing child value writing----------

        [Test]
        public void ifDebugPhase_True_RecordOutcomeNOTcalled()
        {
            String type = "recordToFile";
            String messageToLog = "test Message";
            BuildLogFunction(type, messageToLog);

            loggerExecutor.debugPhase = true;

            var result = loggerExecutor.LogMessage(loggerFunction);

            //verify that TestLogger.recordOutcome was called with correct data
            MockTestLogger.Verify(m => m.recordOutcome(messageToLog, "info"),Times.Never);
            Assert.AreEqual(messageToLog, result);
        }
        [Test]
        public void iftypeNOT_recordToFile_RecordOutcomeNOTcalled()
        {
            String type = "screenshot";
            String messageToLog = "test Message";
            BuildLogFunction(type, messageToLog);

            loggerExecutor.debugPhase = true;

            var result = loggerExecutor.LogMessage(loggerFunction);

            //verify that TestLogger.recordOutcome was called with correct data
            MockTestLogger.Verify(m => m.recordOutcome(messageToLog, "info"), Times.Never);
            Assert.AreEqual(messageToLog, result);
        }

    }
}
