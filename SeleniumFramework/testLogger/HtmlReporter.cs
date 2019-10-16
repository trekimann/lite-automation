using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Configuration;
//using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Selenium
{
    public class HtmlReporter
    {
        public ExtentReports extent { get; private set; } = new ExtentReports();
        private ExtentHtmlReporter Reporter;
        private StringComparison ignoringCase = StringComparison.InvariantCultureIgnoreCase;
        private Dictionary<String, ExtentTest> tests = new Dictionary<string, ExtentTest>();
        public string FileLocation { get; private set; }
        private string directory;
        public Boolean debug { get; set; } = false;
        private String username = Environment.UserName;

        public void StartOverarchingReport(String directory)
        {
            //add date to directory
            String date = DateTime.Now.ToLongDateString();
            this.directory = directory;
            FileLocation = directory + @"\" + date + ".html";

            Reporter = new ExtentHtmlReporter(FileLocation);
            Reporter.AppendExisting = true;
            Reporter.Configuration().Theme = (Theme.Dark);
            extent.AttachReporter(Reporter);
            Reporter.Start();
        }

        public void LogException(String testname, Exception e)
        {
            //find the test to add message to
            var test = tests[testname];
                test.Warning(e);
            try
            {
                extent.Flush();
            }
            catch(Exception ex)
            { }
        }

        public Status StringToStatus(String statusString)
        {
            //Status's: Debug, Error, Fail, Fatal, Info, Pass, Skip, Warning
            if (statusString != null)
            {
                foreach (var status in Enum.GetValues(typeof(Status)))
                {
                    if (status.ToString().Equals(statusString, ignoringCase))
                    {
                        return (Status)status;
                    }
                }
            }
            return Status.Info;
        }

        public void EndReport()
        {
            extent.Flush();
            Reporter.Stop();
            makeScreenshotPathsRelitive();
            //OpenReport();
        }

        //public void OpenReport()
        //{
        //    var p = new Process();
        //    p.StartInfo = new ProcessStartInfo(FileLocation)
        //    {
        //        UseShellExecute = true
        //    };
        //    p.Start();
        //}

        public void AddTest(String testname, String description = null)
        {
            var test = extent.CreateTest(testname, description);
            test.AssignAuthor(username);
            tests.Add(testname, test);
        }

        public void LogMessage(String testname, Status status, String message, String screenshot = null)
        {
            //find the test to add message to
            var test = tests[testname];
            
            //if screenshot, add it
            if (screenshot != null)
            {
                //var ScreenShot = MediaEntityBuilder.CreateScreenCaptureFromPath(screenshot).Build();
                test.Log(status, message).AddScreenCaptureFromPath(screenshot);
                makeScreenshotPathsRelitive();
            }
            else
            {
                test.Log(status, message);
            }
            try
            {
                extent.Flush();
            }
            catch (Exception e)
            { }
        }
       
        public void makeScreenshotPathsRelitive()
        {
            try
            {
                var testAsText = File.ReadAllText(FileLocation);
                //replace directory with relitive path stuff
                testAsText = testAsText.Replace(directory, @".\\");
                //save file back to html

                //add styling to class for images 'step-img'
                string classStyling = @"<style>.step-img {max-width: 100%;}</style>";
                testAsText = testAsText.Replace(@"</html>", classStyling + @"</html>");

                File.WriteAllText(FileLocation,testAsText);
             }
            catch(Exception e)
            { }
        }
    }
}
