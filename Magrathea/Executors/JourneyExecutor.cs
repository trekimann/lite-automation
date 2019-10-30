using HttpWebServices;
using Magrathea.webDrivers;
using OpenQA.Selenium;
using Selenium;
using Selenium.htmlElementManager;
using Selenium.testLogger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Xml.Linq;

namespace Magrathea.Executors
{
    public class JourneyExecutor
    {
        private XmlParser parser;
        private XElement testToRun;
        public MagratheaExecutor executor { get; private set; }
        private ITestLogger logger;
        private Dictionary<String, ElementIdentifierWrapper> mappedIdData = new Dictionary<string, ElementIdentifierWrapper>();
        private StringComparison ignoringCase = StringComparison.InvariantCultureIgnoreCase;
        private MultiThreadedExecutor handler;
        private HttpWebOutgoing networkMessaging;
        private Boolean QuitOnComplete = false;
        public Boolean TestComplete { get; private set; } = false;
        private int apiPort { get; set; }

        public JourneyExecutor(MultiThreadedExecutor multiThreadedExecutor, HttpWebOutgoing networkMessaging)
        {
            this.handler = multiThreadedExecutor;
            this.networkMessaging = networkMessaging;
            this.apiPort = multiThreadedExecutor.apiPort;
        }

        public Object Screenshot()
        {
            return logger.screenshot();
        }

        public Boolean PauseTest()
        {
            return executor.switchPause();
        }

        public Object FocusWindow()
        {
            return executor.FocusWindow();
        }

        public int StopTest()
        {
            int toReturn = executor.StopTest();

            return toReturn;
        }

        public void StopQuit()
        {
            StopTest();
            QuitOnComplete = true;
        }

        public Object RunTest(TestInformation testInfo)
        {
            // Create an XmlParser
            parser = new XmlParser();
            //testMagPhase();
            // translateDictionaryToXML();
            String xmlDirectory = testInfo.RootXmlDirectory;
            // load in the test
            testToRun = parser.ParseXml(testInfo.TestDirectory + @"\" + testInfo.TestName + ".xml");

            // extract test url from test
            testInfo.TestUrl = testToRun.Attribute("url").Value;

            String browser = testInfo.WebBrowser;
            // Create Driver
            IWebDriver driver = CreateDriver(testInfo);

            // driver get to landing page
            //driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(90);
            // Create Test Logger
            logger = new TestLogger(testInfo, networkMessaging, handler.Reporter);
            logger.SetTestPort(apiPort);
            //add driver to logger
            logger.driver = driver;

            // create element manager
            HtmlElementManager manager = new HtmlElementManager(driver, logger);
            manager.browser = browser;
            // create MagratheaExecutor
            executor = new MagratheaExecutor(parser, testInfo, xmlDirectory);
            executor.SetLogger(logger);
            executor.SetDriver(driver);
            executor.SetElementManager(manager);
            handler.Reporter.AddTest(testInfo.TestName + "_" + browser);
            
            try
            {
                string url = testInfo.TestUrl;
                driver.Navigate().GoToUrl(url);
            }
            catch (Exception e)
            {
                logger.recordOutcome("Could not navigate, trying again", "Warning");
                logger.recordOutcome(e);

                driver.Url = testInfo.TestUrl;
            }

            //resize window
            if (browser != "Android")
            {
                if (testInfo.WindowWidth != 0 && testInfo.WindowHeight != 0)
                {
                    driver.Manage().Window.Size = new Size(testInfo.WindowWidth, testInfo.WindowHeight);
                }
                else
                {
                    driver.Manage().Window.Maximize();
                }
            }

            // Make pretty
            SaveCopyOfTheTestToOutputDirectory(testInfo);
            // pass the requested journey to the executor
            logger.recordOutcome("--------------------Starting " + testInfo.TestName + "--------------------", "info");
            executor.StartJourney(testToRun);

            if (manager.stopTest)
            {
                logger.recordOutcome("--------------------Test " + testInfo.TestName + " Complete on " + browser + ". Test Was Stopped early. Check Log for details--------------------", "Warning");
                logger.sendSystemMessage("Test Aborted");
            }
            else
            {
                logger.recordOutcome("--------------------Test " + testInfo.TestName + " Complete on " + browser + "--------------------", "Pass");
                logger.sendSystemMessage("Test Success");
            }
            handler.TestCompleted(testInfo.TestNumber);

            if (QuitOnComplete)
            {
                driver.Quit();
            }

            if (handler.TestsToRun() == handler.TestsFinished)
            {
                Console.WriteLine("All Tests Complete. Killing processes");
                networkMessaging.SendStringInBody(apiPort, "", "System Message", "All Completed");
                handler.Reporter.EndReport();
                executor.WindowSwitching.OpenNewTab(logger.Reporter.FileLocation, "report");
                CmdProcessKill();
                handler.ClearThreadList();
            }
            else
            {
                handler.ExecuteNextTest();
            }

            //build expando response
            dynamic response = new ExpandoObject();
            response.TestName = testInfo.TestName;
            response.Result = !manager.stopTest;
            TestComplete = true;
            return response;
        }

        public void CmdProcessKill()
        {
            try
            {
                var chrome = Process.GetProcessesByName("chromedriver");
                var gecko = Process.GetProcessesByName("geckoDriver");

                foreach (Process p in chrome) { p.Kill(); }
                foreach (Process p in gecko) { p.Kill(); }
            }
            catch (Exception e)
            {
                logger.recordOutcome(e);
            }
        }

        public IWebDriver CreateDriver(TestInformation testInformation)
        {
            String driverType = testInformation.WebBrowser;

            if (driverType.Equals("firefox", ignoringCase))
            {
                var firefoxDriverCreater = new FirefoxWebdriver(testInformation.FirefoxDirectory);

                return firefoxDriverCreater.CreateWebdriver(); ;
            }

            else if (driverType.Equals("chrome", ignoringCase))
            {
                var chromeDriverCreator = new ChromeWebdriver(testInformation.ChromeDirectory, testInformation.MobileEmulation);

                return chromeDriverCreator.CreateWebdriver(); ;
            }

            else if (driverType.Equals("android", ignoringCase))
            {
                var androidDriverCreator = new AndroidWebdriver(testInformation.DesiredCapibilities);

                return androidDriverCreator.CreateWebdriver(); ;
            }
            return null;
        }

        public dynamic StartStopMouseTracking()
        {
            return executor.StartStopMouseTracking();
        }

        public void SaveCopyOfTheTestToOutputDirectory(TestInformation testInfo)
        {
            String destDirectory = testInfo.OutputDirectory + "\\" + testInfo.TestName + "\\";
            Directory.CreateDirectory(destDirectory);
            testToRun.Save(destDirectory + testInfo.TestName + ".xml");
        }
        //public void TestMagPhase()
        //{
        //    String xmlDirectory = "C:\\Users\\MANNN\\Desktop\\SeleniumLite_BETA\\DevFiles\\XML Experiment";
        //    executor = new MagratheaExecutor(parser,null, xmlDirectory);
        //    String targetPhase = "C:\\Users\\MANNN\\Desktop\\SeleniumLite_BETA\\DevFiles\\XML Experiment\\Journeys\\tutorial\\myFirstPhase.xml";
        //    XElement phase = parser.ParseXml(targetPhase);
        //    // get default data
        //    XElement dataElement = phase.Elements("Data").ToList()[0];
        //    List<XElement> dataItems = dataElement.Elements("phaseValue").ToList();
        //    // find any list items and add 0 to them
        //    foreach (XElement dataItem in dataItems)
        //    {
        //        if (dataItem.Attribute("type").Value.Equals("list", ignoringCase))
        //        {
        //            List<XElement> listItems = dataItem.Elements().ToList();
        //            // loop though each list item and add 0
        //            foreach (XElement listItem in listItems)
        //            {
        //                listItem.SetAttributeValue("name", listItem.Attribute("name").Value + "0");
        //            }
        //        }
        //    }
        //    executor.PopulateTestData(dataItems);
        //    // remove 0 from items
        //    foreach (XElement dataItem in dataItems)
        //    {
        //        if (dataItem.Attribute("type").Value.Equals("list", ignoringCase))
        //        {
        //            List<XElement> listItems = dataItem.Elements().ToList();
        //            // loop though each list item and add 0
        //            foreach (XElement listItem in listItems)
        //            {
        //                String removed = listItem.Attribute("name").Value.Substring(0,listItem.Attribute("name").Value.Length - 1);
        //                listItem.SetAttributeValue("name", removed);
        //            }
        //        }
        //    }

        //    // get function
        //    List<XElement> function = phase.Elements("Function").ToList()[0].Elements().ToList();

        //    // enter any temp variables to data map
        //    // executor.setDataValue("accidentMonth0", new typeObjectPair("string",
        //    // "January"));
        //    // executor.setDataValue("accidentYear", new typeObjectPair("string", "2017"));
        //    // executor.setDataValue("accidentType", new typeObjectPair("string",
        //    // "Accident"));
        //    // executor.setDataValue("claimFaultYesNo", new typeObjectPair("boolean",
        //    // true));
        //    // executor.setDataValue("noClaimsDiscountYesNo", new typeObjectPair("boolean",
        //    // true));
        //    // executor.setDataValue("anyInjuriesYesNo", new typeObjectPair("boolean",
        //    // true));

        //    // execute function
        //    executor.SetDebug(true, phase);
        //    executor.ExecuteFunction(function, "");

        //}

        // public void translateDictionaryToXML() {//repurpose for dictionary repair
        // //read in old csv
        // String csvFile =
        // "C:\\Users\\MANNN\\Desktop\\SeleniumLite_BETA\\DevFiles\\Common
        // Dictionary\\ElementDictionary.csv";
        // BufferedReader buffer = null;
        // String line = "";
        // String splitBy = ",";
        // int linecount = 0;
        //
        // try {
        // buffer = new BufferedReader(new FileReader(csvFile));
        // while ((line = buffer.readLine()) != null) {
        // String[] temp = line.split(splitBy);
        // ElementIdentifierWrapper elementDefinition = new ElementIdentifierWrapper();
        // elementDefinition.setElementId(temp[1]);
        // elementDefinition.setElementName(temp[2]);
        // elementDefinition.setElementXpath(temp[3]);
        //
        // mappedIdData.put(temp[0], elementDefinition);
        // linecount++;
        // }
        // } catch (Exception e) {
        // System.out.println(e);
        // }
        // System.out.println("Fields Read: " + linecount);
        // System.out.println("Dictionary CSV Parsed");
        //
        // //reorder to xml
        // //root element
        // Element dictionaryElement = new Element("ElementDictionary");
        // Document doc = new Document(dictionaryElement);
        // //journeyElement
        // Element journeyElement = new Element("directCar");
        //
        // //loop though map and convert to xml form
        // for (String Key : mappedIdData.keySet()) {
        // // Deffinition Element
        // Element deffinition = new Element("Definition");
        // deffinition.setText(Key);
        // //id Element
        // Element id = new Element("Id");
        // String idText = mappedIdData.get(Key).getElementId();
        // id.setText(idText);
        // deffinition.addContent(id);
        // //name Element
        // Element name = new Element("Name");
        // String nameText =mappedIdData.get(Key).getElementName();
        // name.setText(nameText);
        // deffinition.addContent(name);
        // //xPath element
        // Element xpath = new Element("xPath");
        // String xpathText = mappedIdData.get(Key).getElementXpath();
        // xpath.setText(xpathText);
        // deffinition.addContent(xpath);
        //
        // //add deffinition to journey
        // journeyElement.addContent(deffinition);
        // }
        // doc.getRootElement().addContent(journeyElement);
        //
        // XMLOutputter xmlOutput = new XMLOutputter();
        // xmlOutput.setFormat(Format.getPrettyFormat());
        // try {
        // FileWriter fileWriter = new
        // FileWriter("C:\\Users\\MANNN\\Desktop\\SeleniumLite_BETA\\DevFiles\\Common
        // Dictionary\\ElementDictionary.xml");
        // xmlOutput.output(doc,fileWriter);
        // } catch (IOException e) {
        // // TODO Auto-generated catch block
        // e.printStackTrace();
        // }
        //
        // }
    }
}
