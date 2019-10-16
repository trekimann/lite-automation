using HttpWebServices;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using AventStack.ExtentReports;

namespace Selenium.testLogger
{
    public class TestLogger : ITestLogger
    {
        public IWebDriver driver { get; set; }
        private TestInformation testInfo;
        public HtmlReporter Reporter { get; private set; }
        private HttpWebOutgoing networkMessaging;
        private int screenshotCount;
        private String browserBeingRun;
        private String newDirectory;
        private int testPort = 26001;
        private int NetworkAttempts = 0;
        private int NetworkAttemptsToMake = 3;

        private String ip = "0.0.0.0";
        public String username { get; private set; } = Environment.UserName;
        private string[] ipParts;
        private List<String> test = new List<string> { "T", "e", "s", "t" };
        private String intro = "run by:";
        private String TestName;

        public void SetTestPort(int portNumber)
        {
            testPort = portNumber;
        }

        private void UserIp()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ipAdd in host.AddressList)
            {
                if (ipAdd.AddressFamily == AddressFamily.InterNetwork)
                {
                    ip = ipAdd.ToString();
                }
            }
            ipParts = ip.Split('.');
        }

        public TestLogger(TestInformation testInfo, HttpWebOutgoing networkMessaging, HtmlReporter Reporter)
        {            
            this.testInfo = testInfo;
            this.screenshotCount = 1;
            this.browserBeingRun = testInfo.WebBrowser;
            this.networkMessaging = networkMessaging;
            this.Reporter = Reporter;
            this.TestName = testInfo.TestName + "_" + testInfo.WebBrowser;
            SetupDirectory();
            UserIp();
        }

        public void SetupDirectory()
        {
            newDirectory = testInfo.OutputDirectory + "\\" + testInfo.TestName + "\\" + browserBeingRun;
            Directory.CreateDirectory(newDirectory);
        }

        public void SendTextToClient(String logToSend)//send log info to a specific port
        {
            string uri = @"Test/Message/" + testInfo.TestNumber;
            networkMessaging.SendStringInBody(testPort, uri, "Log Message", logToSend, testInfo.TestNumber.ToString());
        }

        public void recordOutcome(String infoToLog, String statusString = null)
        {
            String wholeLog = DateTime.Now + " -- " + infoToLog;

            //get the status type
            Status status = Reporter.StringToStatus(statusString);
            //save the log to the HtmlReport
            Reporter.LogMessage(TestName, status, infoToLog);

            // records data to txt file
            //StreamWriter file = new StreamWriter(newDirectory + "\\" + testInfo.TestName + ".txt", true);
            //file.WriteLine(wholeLog);
            if (NetworkAttempts < NetworkAttemptsToMake)
            {
                try { SendTextToClient(wholeLog); }
                catch (Exception e)
                {
                    //file.WriteLine(DateTime.Now + " -- unable to send over network. " + e.ToString());
                    Reporter.LogMessage(TestName, Status.Warning, "unable to send over network.");
                    Reporter.LogException(TestName, e);
                    NetworkAttempts++; 
                }
            }
            //file.Close();
            Console.WriteLine(wholeLog);
        }

        public void recordOutcome(Exception e)
        {
            Reporter.LogException(TestName, e);
            Console.WriteLine(e);
        }

        public void sendSystemMessage(String message)
        {
            string uri = @"Test/Message/" + testInfo.TestNumber;
            networkMessaging.SendStringInBody(testPort, uri, "System Message", message, testInfo.TestNumber.ToString());
        }

        public String screenshot(String Message = "Screenshot")
        {
            byte[] screenBytes = ((ITakesScreenshot)driver).GetScreenshot().AsByteArray;
            var testName = testInfo.TestName;
            string fileName = testName + "_" + browserBeingRun + "_"
                    + this.screenshotCount.ToString("000") + ".png";
            string DestFile = newDirectory + "\\" + fileName;

            int fontSize = 10;
            var ms = new MemoryStream(screenBytes);
            var img = Image.FromStream(ms);
            Graphics g = Graphics.FromImage(img);
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            Font fontTouse = new Font("Fixedsys", fontSize, FontStyle.Bold);

            int rColor = screenshotCount;
            if (screenshotCount >= 255)
            {
                rColor = screenshotCount - 255;
            }

            PointF introLocation = new PointF(fontSize + screenshotCount, fontSize);
            PointF nameLocation = new PointF(fontSize + screenshotCount, 5 + (fontSize * 2));

            var generalBrush = new SolidBrush(Color.FromArgb(rColor, rColor, rColor));
            var encodeBrush = new SolidBrush(Color.FromArgb(rColor, rColor, rColor));
            //loop to draw letters in each colour
            int pos = 0;
            foreach (String letter in test)
            {
                // encode ip address
                encodeBrush = new SolidBrush(Color.FromArgb(255, rColor, Int32.Parse(ipParts[pos]), rColor));
                g.DrawString(letter, fontTouse, encodeBrush, introLocation);
                introLocation.X += fontSize - 1;
                pos++;
            }
            g.DrawString(intro, fontTouse, generalBrush, introLocation);
            g.DrawString(username, fontTouse, generalBrush, nameLocation);
            img.Save(DestFile, ImageFormat.Png);

            //insert screenshot into Report
            Reporter.LogMessage(TestName, Status.Info, Message+ "_"+screenshotCount, DestFile);

            this.screenshotCount++;
            return DestFile;
        }

        public String GetInfoFromLog(String Directory)
        {
            return GetInfoFromLog(DateTime.Now.AddSeconds(-1), DateTime.Now.AddSeconds(1), Directory);
        }

        public String GetInfoFromLog(DateTime TimeRequestedStart, DateTime TimeRequestedEnd, String directory)
        {
            String timeRequestedStart = TimeRequestedStart.ToString("yyyy-MM-dd hh:mm:ss");
            String timeRequestedEnd = TimeRequestedEnd.ToString("yyyy-MM-dd hh:mm:ss");

            //start filestream
            FileStream LogFileStream = new FileStream(directory, FileMode.Open, FileAccess.Read);
            StreamReader LogStream = new StreamReader(LogFileStream);
            //get time at start of log
            String startTimeOfLog = LogStream.ReadLine().Split(',')[0].Replace("[", "").Replace("]", "");
            //get endtime of log
            String endTimeOfLog = "";
            for (long pos = LogFileStream.Length - 2; pos > 0; --pos)
            {
                LogFileStream.Seek(pos, SeekOrigin.Begin);
                endTimeOfLog = LogStream.ReadToEnd();
                int eol = endTimeOfLog.IndexOf("\n");
                if (eol >= 0)
                {
                    endTimeOfLog = endTimeOfLog.Substring(eol + 1);
                }
            }
            endTimeOfLog = endTimeOfLog.Split(',')[0].Replace("[", "").Replace("]", "");
            
            return null;
        }

        public String getOutputDirectory()
        {
            return testInfo.OutputDirectory;
        }

        public String getBrowserType()
        {
            return browserBeingRun;
        }
    }
}
