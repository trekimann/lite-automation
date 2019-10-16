using System;
using OpenQA.Selenium;

namespace Selenium.testLogger
{
    public interface ITestLogger
    {
        IWebDriver driver { get; set; }
        HtmlReporter Reporter { get; }

        string username { get; }
        string getBrowserType();
        string GetInfoFromLog(DateTime TimeRequestedStart, DateTime TimeRequestedEnd, string directory);
        string GetInfoFromLog(string Directory);
        string getOutputDirectory();
        void recordOutcome(Exception e);
        void recordOutcome(string infoToLog, string statusString = null);
        string screenshot(string Message = "Screenshot");
        void sendSystemMessage(string message);
        void SendTextToClient(string logToSend);
        void SetTestPort(int portNumber);
        void SetupDirectory();
    }
}