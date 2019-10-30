using HttpWebServices;
using Selenium;
using Selenium.testLogger;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading;
using System.Xml.Linq;

namespace Magrathea.Executors
{
    class ThreadGroup
    {
        public Thread TestThread { get; set;}
        public JourneyExecutor Task { get; set; }

        public ThreadGroup(Thread thread,JourneyExecutor task)
        {
            TestThread = thread;
            Task = task;
        }
    }    


public class MultiThreadedExecutor
    {
        public List<TestInformation> TestList { get; set; }
        public int NumberOfThreads { get; private set; } = 1;
        public static int TestNumber { get; private set; } = 0;
        public int TestsFinished { get; private set; } = 0;
        private static Dictionary<String, ThreadGroup> ThreadMap = new Dictionary<string, ThreadGroup>();
        private HttpWebOutgoing networkMessaging;
        public HtmlReporter Reporter { get; private set; } = new HtmlReporter();
        public int apiPort { get; private set; }


        public MultiThreadedExecutor(HttpWebOutgoing networkMessaging, int port)
        {
            this.networkMessaging = networkMessaging;
            this.apiPort = port;
        }

        public void ExecuteThreadedTests()
        {         

            foreach(TestInformation testInfo in TestList)
            {
                JourneyExecutor executor = new JourneyExecutor(this,networkMessaging);
                Thread testThread = new Thread(() =>executor.RunTest(testInfo));

                string testName=testInfo.TestName + testInfo.WebBrowser;
                if(testInfo.MobileEmulation!=null)
                {
                    testName = testName + testInfo.MobileEmulation.Replace(" ", "");
                }
                else
                {
                    testName = testName + "Desktop";
                }
                ThreadMap.Add(testName, new ThreadGroup(testThread, executor));
            }

            //get reporting location
            String fileDirectory = TestList[0].OutputDirectory;
            Reporter.StartOverarchingReport(fileDirectory);

            for(int i = 0; i < NumberOfThreads; i++)
            {
                ExecuteNextTest();
                try { Thread.Sleep(1000); }
                catch (Exception e) { }
            }
        }

        public void ClearThreadList()
        {
            ThreadMap.Clear();
            TestNumber = 0;
            networkMessaging.SendStringInBody(apiPort, "", "System Message", "All Completed");
        }
        
        public void ExecuteNextTest()
        {
            Console.WriteLine((TestNumber+1) +":" +TestList.Count);
            if(TestNumber<TestList.Count)
            {
                var test = TestList[TestNumber];
                string testName = test.TestName + test.WebBrowser;
                if (test.MobileEmulation != null)
                {
                    testName = testName + test.MobileEmulation.Replace(" ", "");
                }
                else
                {
                    testName = testName + "Desktop";
                }
                test.TestNumber = TestNumber;
                TestNumber++;
                Thread toRun = ThreadMap[testName].TestThread;
                toRun.Start();
            }
            else
            {
                if (TestsFinished == TestList.Count)
                {
                    Reporter.EndReport();
                    ClearThreadList();
                    networkMessaging.SendStringInBody(apiPort, "", "System Message", "All Completed");
                }
            }
        }

        public Object Screenshot(int testNumber)
        {
            var Test = TestList[testNumber];
            string testName = Test.TestName + Test.WebBrowser;
            if (Test.MobileEmulation != null)
            {
                testName = testName + Test.MobileEmulation.Replace(" ", "");
            }
            else
            {
                testName = testName + "Desktop";
            }
            var journeyExecutor = ThreadMap[testName].Task;
            dynamic toReturn = new ExpandoObject();
            toReturn.TestName = Test.TestName;
            toReturn.WebBrowser = Test.WebBrowser;
            toReturn.ScreenshotDir = journeyExecutor.Screenshot();
            return toReturn;
        }

        public void TestCompleted(int testNumber)
        {
            var Test = TestList[testNumber];
            string uri = @"Test/Message/" + Test.TestNumber;
            networkMessaging.SendStringInBody(apiPort, uri, "Log Message", "----------Test Ended----------", Test.TestNumber.ToString());

            //networkMessaging.SendStringInBody(apiPort, uri, "System Message", "Test Ended", Test.testNumber.ToString());

            TestsFinished++;
        }

        public int TestsToRun()
        {
            return TestList.Count;
        }

        public int SetNumberOfThreads(int numOfThreads)
        {
            NumberOfThreads = numOfThreads;
            return NumberOfThreads;
        }

        public Object PauseTest(int testNumber)
        {
            var Test = TestList[testNumber];
            string testName = Test.TestName + Test.WebBrowser;
            if (Test.MobileEmulation != null)
            {
                testName = testName + Test.MobileEmulation.Replace(" ", "");
            }
            else
            {
                testName = testName + "Desktop";
            }
            var journeyExecutor = ThreadMap[testName].Task;
            dynamic toReturn = new ExpandoObject();
            toReturn.TestName = Test.TestName;
            toReturn.WebBrowser = Test.WebBrowser;
            toReturn.Paused = journeyExecutor.PauseTest();

            string uri = @"Test/Message/" + Test.TestNumber;
            networkMessaging.SendStringInBody(apiPort, uri, "System Message", "Test Pause", Test.TestNumber.ToString());

            networkMessaging.SendStringInBody(apiPort, uri, "Log Message", "Test Paused: "+toReturn.Paused, Test.TestNumber.ToString());

            //Console.WriteLine("Pause Toggling: "+testName);
            return toReturn;
        }

        public Object StopQuit(int testNumber)
        {
            var Test = TestList[testNumber];
            string testName = Test.TestName + Test.WebBrowser;
            if (Test.MobileEmulation != null)
            {
                testName = testName + Test.MobileEmulation.Replace(" ", "");
            }
            else
            {
                testName = testName + "Desktop";
            }
            Thread testThread = ThreadMap[testName].TestThread;
            ThreadMap[testName].Task.StopQuit();
            while(!(ThreadMap[testName].Task.TestComplete))
            { }
            testThread.Abort();
            TestCompleted(Test.TestNumber);

            dynamic toReturn = new ExpandoObject();
            toReturn.TestName = Test.TestName;
            toReturn.Message = "Stopped Test and closed driver";
            ExecuteNextTest();
            return toReturn;
        }

        public Object StopTest(int testNumber)
        {
            var Test = TestList[testNumber];
            string testName = Test.TestName + Test.WebBrowser;
            if (Test.MobileEmulation != null)
            {
                testName = testName + Test.MobileEmulation.Replace(" ", "");
            }
            else
            {
                testName = testName + "Desktop";
            }
            Thread testThread = ThreadMap[testName].TestThread;
            ThreadMap[testName].Task.StopTest();

            //wait for test to report done.            
            TestCompleted(Test.TestNumber);

            Thread.Sleep(1000);            
            dynamic toReturn = new ExpandoObject();
            toReturn.TestName = Test.TestName;
            toReturn.Identifier = Test.TestNumber;
            toReturn.Message = "Stopped Test";

            testThread.Abort();
            ExecuteNextTest();
            return toReturn;
        }

        public dynamic CheckDefinition(int testNumber, string searchTerm)
        {
            dynamic toReturn = new ExpandoObject();
            //find the test it links to 
            var Test = TestList[testNumber];
            string testName = Test.TestName + Test.WebBrowser;
            if (Test.MobileEmulation != null)
            {
                testName = testName + Test.MobileEmulation.Replace(" ", "");
            }
            else
            {
                testName = testName + "Desktop";
            }
            var requiredExecutor = ThreadMap[testName].Task;

            //split searchTerm
            string searchTermMain = searchTerm.Split(new string[] { "---***---" }, StringSplitOptions.None)[0];
            string journey=searchTerm.Split(new string[] { "---***---" }, StringSplitOptions.None)[1];

            toReturn = requiredExecutor.executor.CheckDefinition(searchTermMain, journey);

            return toReturn;
        }

        public dynamic StartMouseTracking(int testNumber)
        {
            dynamic toReturn = new ExpandoObject();

            //find the test it links to 
            var Test = TestList[testNumber];
            string testName = Test.TestName + Test.WebBrowser;
            if (Test.MobileEmulation != null)
            {
                testName = testName + Test.MobileEmulation.Replace(" ", "");
            }
            else
            {
                testName = testName + "Desktop";
            }
            var requiredExecutor = ThreadMap[testName].Task;

            toReturn = requiredExecutor.StartStopMouseTracking();

            return toReturn;
        }

        public object FocusWindow(int testNumber)
        {
            dynamic toReturn = new ExpandoObject();

            //find the test it links to 
            var Test = TestList[testNumber];
            string testName = Test.TestName + Test.WebBrowser;
            if (Test.MobileEmulation != null)
            {
                testName = testName + Test.MobileEmulation.Replace(" ", "");
            }
            else
            {
                testName = testName + "Desktop";
            }
            var requiredExecutor = ThreadMap[testName].Task;

            toReturn = requiredExecutor.FocusWindow();

            return toReturn;
        }

        public dynamic TestFunction(int testNumber, List<XElement> magFunction)
        {
            dynamic toReturn = new ExpandoObject();
            //find the test it links to 
            var Test = TestList[testNumber];
            string testName = Test.TestName + Test.WebBrowser;
            if (Test.MobileEmulation != null)
            {
                testName = testName + Test.MobileEmulation.Replace(" ", "");
            }
            else
            {
                testName = testName + "Desktop";
            }
            var requiredExecutor = ThreadMap[testName].Task;

            requiredExecutor.executor.TestingFunction = true;
            toReturn = requiredExecutor.executor.ExecuteFunction(magFunction,"");
            requiredExecutor.executor.TestingFunction = false;
            return toReturn;
        }
    }
}
