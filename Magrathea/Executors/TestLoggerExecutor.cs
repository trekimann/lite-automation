using Selenium.testLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Magrathea.Executors
{
    public class TestLoggerExecutor : ITestLoggerExecutor
    {
        private StringComparison ignoringCase = StringComparison.InvariantCultureIgnoreCase;
        public Boolean debugPhase { get; set; } = false;
        private ITestLogger Logger;
        private IMagratheaExecutor executor;

        public TestLoggerExecutor(ITestLogger Logger,IMagratheaExecutor executor)
        {
            this.Logger = Logger;
            this.executor = executor;
            this.debugPhase = executor.debugPhase;
        }

        public void Log(XElement loggerFunction)
        {
            String type = loggerFunction.Attribute("type").Value;
            String infoToLog = "";
            if (type.Equals("screenshot", ignoringCase) && !debugPhase)
            {
                //get an attached message if there is one.
                var message = LogMessage(loggerFunction);
                if(message==null || message=="")
                {
                    message = "Screenshot";
                }
                Logger.screenshot(message);
            }
            else if (type.Equals("recordToFile", ignoringCase))
            {
                infoToLog = LogMessage(loggerFunction);
            }
            Console.WriteLine("Executing Logger. Type: " + loggerFunction.Attribute("type").Value + ". Value: " + infoToLog);

        }

        public string LogMessage(XElement loggerFunction)
        {
            string infoToLog;
            String Status = "info";
            String type = loggerFunction.Attribute("type").Value;
            try
            {
                Status = loggerFunction.Attribute("status").Value;
            }
            catch (Exception e)
            { }
            infoToLog = loggerFunction.Value.ToString().Trim();
            List<XElement> child = loggerFunction.Elements().ToList();
            if (child.Count() >= 1)
            {
                // get value
                Object value = executor.GetPhaseValue(child[0]);
                // check what type of value it is
                String cast = value.GetType().ToString();
                if (cast.Equals("System.String", ignoringCase))
                {
                    infoToLog = infoToLog + " " + (String)value;
                }
                else if (cast.Equals("System.Int32", ignoringCase))
                {
                    infoToLog = infoToLog + " " + (int)value;
                }
                else if (cast.Equals("System.Boolean", ignoringCase))
                {
                    infoToLog = infoToLog + " " + (Boolean)value;
                }
                else if (cast.Equals("System.DateTime", ignoringCase))
                {
                    DateTime time = (DateTime)value;
                    infoToLog = infoToLog + " " + time.ToString("dd-MM-yyyy HH:mm:ss");
                }
            }
            if (!debugPhase&&type.Equals("recordToFile",ignoringCase))
            {
                RecordToLog(infoToLog, Status);
            }
            return infoToLog;
        }

        private void RecordToLog(String log,String Status)
        {
          Logger.recordOutcome(log, Status);
        }
    }
}
