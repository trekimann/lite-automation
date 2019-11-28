using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Selenium.testLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

namespace Selenium.htmlElementManager
{
    public class HtmlElementManager : IHtmlElementManager
    {
        private IWebDriver driver;
        private ITestLogger logger;
        private WebDriverWait wait;
        private ElementIdentifierDictionary elementdictionary;
        private int numberOfAttemptsToMake = 5;
        public bool stopTest { get; set; } = false;
        public string browser { get; set; }
        public List<string> PauseTargets { get; set; } = new List<string>();

        public Boolean Pause { get; set; } = false;
        public Boolean TestingFunction { get; set; } = false;

        private ReplayBuffer buffer;
        private int numberOfTextChecksToRun = 3;
        private Boolean elementSuccess = false;
        private readonly StringComparison ignoringCase = StringComparison.InvariantCultureIgnoreCase;

        public int getNumberOfTextChecksToRun()
        {
            return numberOfTextChecksToRun;
        }

        public void setNumberOfTextChecksToRun(int numberOfTextChecksToRun)
        {
            this.numberOfTextChecksToRun = numberOfTextChecksToRun;
        }

        public IWebDriver getDriver()
        {
            return driver;
        }

        public void setElementDictionary(ElementIdentifierDictionary elementdictionary)
        {
            this.elementdictionary = elementdictionary;
            elementdictionary.manager = this;
        }

        public HtmlElementManager(IWebDriver driver, ITestLogger logger)
        {
            this.driver = driver;
            this.logger = logger;
            buffer = new ReplayBuffer(this, logger, numberOfAttemptsToMake);
            specificBrowserWaits();
        }

        public void specificBrowserWaits()
        {
            if (logger.getBrowserType().Equals("firefox", ignoringCase))
            {
                wait = new WebDriverWait(this.driver, TimeSpan.FromSeconds(20));
            }
            else if (logger.getBrowserType().Equals("chrome", ignoringCase))
            {
                wait = new WebDriverWait(this.driver, TimeSpan.FromSeconds(15));
            }
            else if (logger.getBrowserType().Equals("Android", ignoringCase))
            {
                wait = new WebDriverWait(this.driver, TimeSpan.FromSeconds(60));
            }
        }

        public bool allChecks()
        {
            bool toReturn = false;
            WaitForPendingAjax();
            toReturn = checkForSorry();
            WaitForPendingAjax();
            return toReturn;
        }

        public bool checkForSorry()
        {
            Boolean sorryFound = driver.FindElements(By.Id("HEAD_CONN_ERROR")).Count > 0;
            if (sorryFound)
            {
                // DirtyPause(1000);
                // get logs here

                // pull sorry reason from edge $(QUE_2D9638BF3EA1B663235671).text()
                var reason = driver.FindElement(By.Id("QUE_2D9638BF3EA1B663235671")).Text;


                logger.recordOutcome("---------------Sorry Page Hit---------------", "Warning");
                logger.recordOutcome(reason, "Warning");
                Console.WriteLine("sorry Hit");
                logger.screenshot();
                //driver.Close();
                //driver.Quit();
                //try
                //{
                //    var chrome = Process.GetProcessesByName("chromedriver");
                //    var gecko = Process.GetProcessesByName("geckoDriver");

                //    foreach (Process p in chrome) { p.Kill(); }
                //    foreach (Process p in gecko) { p.Kill(); }
                //}
                //catch (Exception e)
                //{
                //    logger.recordOutcome(e.ToString());
                //}
                stopTest = true;
                return true;
            }
            return false;
        }

        public void DirtyPause(int millisecond)
        {
            Thread.Sleep(millisecond);
        }

        public void PauseTest()
        {//used to pause the test and keep it alive
            DirtyPause(1000);
            var TimeoutBtnFound = ElementAvailable("TimeoutOk_Btn");
            if (TimeoutBtnFound)
            {
                Pause = !Pause;
                logger.screenshot("Prevented Session From Timing Out");
                Click("TimeoutOk_Btn");
                Pause = !Pause;
            }
        }

        public void WaitForPendingAjax()
        {
            int waitTime = 60;//in seconds
            int pauseBetweenLoops = 50;//in ms
            int loopsRequired = waitTime * (1000 / pauseBetweenLoops);
            var Js = (driver as IJavaScriptExecutor);
            for (var i = 0; i < loopsRequired; i++)
            {
                DirtyPause(pauseBetweenLoops);
                var pending = Js.ExecuteScript("return (window.Ext && window.Ext.Ajax && window.Ext.Ajax.isLoading()) || (window.AJAX_REQ_ID != undefined && window.AJAX_REQ_ID != 0);");
                var ajaxIsComplete = !(bool)pending;
                if (ajaxIsComplete)
                {
                    if (i > 0)
                    {
                        logger.recordOutcome("pending ajax request: " + pending, "Debug");
                        easteregg(Js);
                    }
                    return;
                }
                else
                {
                    if (i == 0)
                    {
                        logger.recordOutcome("pending ajax request: " + pending, "Debug");
                        easteregg(Js);
                    }
                }
            }
        }

        private void easteregg(IJavaScriptExecutor Js)
        {
            string date = DateTime.Today.Day.ToString() + ":" + DateTime.Today.Month.ToString();
            string imageSize = "75";
            //return;
            string target = "";
            if (date.Equals("1:4") && logger.username == "drepaus")
            {
                target = @"https://media.licdn.com/dms/image/C5603AQH-LnCvGY8Sbg/profile-displayphoto-shrink_800_800/0?e=1559174400&v=beta&t=G6DmQ0kJHiMYIE3pf3_xP8THG-RdR_smDL650neym2M";
                imageSize = "110";
            }//sohan
            else if (date.Equals("1:4") && logger.username == "manchel")
            {
                target = @"https://media.licdn.com/dms/image/C5103AQEHGkn9-sxQbw/profile-displayphoto-shrink_800_800/0?e=1559779200&v=beta&t=2FOubuZbFmpQCX-UDP1eESvfEvuqVibJGevMdtfhB2k";
            }
            else if (date.Equals("1:4"))
            {
                target = @"https://media.licdn.com/dms/image/C5603AQF2Fmf1MlN45A/profile-displayphoto-shrink_800_800/0?e=1559174400&v=beta&t=ODT7C-PQOaLbZ87JOdadtwAhOn23DJo-e8rF6KD2sRQ";//tim
                imageSize = "100";
            }
            else if (date.Equals("31:10"))
            {
                target = @"https://cdn.shopify.com/s/files/1/1061/1924/products/Pumpkin_Emoji_Icon_169be5a5-3354-4573-b327-5d69e23e8458_large.png";//halloween
            }
            else if (date.Equals("5:11") || date.Equals("1:1") || date.Equals("2:1") || date.Equals("3:1"))
            {//fireworks
                target = @"https://media.giphy.com/media/peAFQfg7Ol6IE/200w_d.gif";
                imageSize = "100";
            }
            else if (date.Equals("25:12") || date.Equals("24:12") || date.Equals("23:12") || date.Equals("22:12"))
            {
                target = @"http://clipart-library.com/img/944278.png";
            }

            if (target != "")
            {
                try
                {
                    Js.ExecuteScript(@"$('body').on('DOMNodeInserted', 'div', function(){$('.loadingoverlay_element').ready(function(){$('.loadingoverlay_element').css({'background-image': 'url(" + target + @")','width': '" + imageSize + "px', 'height': '" + imageSize + "px','border-radius':'50%','background-size': 'auto " + imageSize + "px'});});});");
                }
                catch (Exception e) { }
            }
        }

        public Boolean TextFieldCheck(IWebElement element, String textToCheck, Boolean log)
        {
            if (textToCheck == null)
            {
                textToCheck = "";
            }
            allChecks();
            Boolean found = false;
            // Console.WriteLine("Text sent:"+textToCheck);
            while (!found)
            {
                IWebElement newFind = element;
                //var newFind = element;
                String type = newFind.GetAttribute("type");

                // allChecks();
                if (!(type.Equals("text", ignoringCase)) && !(type.Equals("password", ignoringCase))
                    && !(type.Equals("number", ignoringCase)))
                {
                    var select = new SelectElement(newFind);
                    newFind = select.SelectedOption;
                }
                // allChecks();
                String retrievedText = newFind.Text;

                if (textToCheck.Equals(""))
                {

                }
                else if (retrievedText.Equals("") || retrievedText == null)
                {
                    retrievedText = newFind.GetAttribute("value");
                }
                // Console.WriteLine("Text Found:"+retrievedText);
                if (retrievedText == textToCheck)
                {
                    found = true;
                    return true;
                }
                else
                {
                    if (log)
                    {
                        logger.recordOutcome("Looking for: " + textToCheck + ". Found: " + retrievedText);
                    }

                    // clear Box
                    if (type.Equals("text") || type.Equals("password"))
                    {
                        int textLength = retrievedText.Length;
                        for (int i = 0; i < textLength * 2; i++)
                        {
                            newFind.SendKeys(Keys.Backspace);
                        }
                    }
                    found = true;
                    numberOfTextChecksToRun--;
                    return false;
                }
            }
            return true;
        }

        public Boolean TextFieldCheck(String elementIdentifier, String textToCheck, Boolean log)
        {
            IWebElement newFind = Find(elementIdentifier);
            return TextFieldCheck(newFind, textToCheck, false);
        }

        public Boolean ElementAvailable(String elementIdentifier)
        {
            // mag check
            if (elementIdentifier.Equals("MYA_MtaQuoteContinue"))
            {
                Console.WriteLine("mag breakpoint");
            }

            List<By> identifiers = new List<By>();
            String id = elementdictionary.GetId(elementIdentifier);
            String xpath = elementdictionary.GetXpath(elementIdentifier);
            String name = elementdictionary.GetName(elementIdentifier);

            if (id != null && !id.Equals("") && !id.Equals(" "))
            {
                identifiers.Add(By.Id(id));
            }
            if (xpath != null && !xpath.Equals("") && !xpath.Equals(" "))
            {
                identifiers.Add(By.XPath(xpath));
            }
            if (name != null && !name.Equals("") && !name.Equals(" "))
            {
                identifiers.Add(By.Name(name));
            }
            allChecks();
            foreach (By by in identifiers)
            {
                List<IWebElement> found = driver.FindElements(by).ToList();
                if (found.Count > 0)
                {
                    IWebElement first = found[0];
                    elementdictionary.CheckDictionaryDefinition(elementIdentifier, first);
                    if (first.Displayed)
                    {
                        return true;
                    }
                    //else
                    //{
                    //    return false;
                    //}
                }
            }
            return false;
        }

        public void PauseOnTarget(String elementIdentifier)
        {
            foreach (string PauseTarget in PauseTargets)
            {
                if (elementIdentifier == PauseTarget)
                {
                    logger.recordOutcome("Test was indefinitely paused. This was triggered by: " + elementIdentifier);
                    logger.sendSystemMessage("Test Paused");
                    Pause = true;
                    break;
                }
            }
        }

        public IWebElement Find(String elementIdentifier)
        {
            PauseOnTarget(elementIdentifier);
            //pausing
            if (Pause && !TestingFunction)
            {
                while (Pause)
                {
                    PauseTest();
                }
            }

            //magratheaDebug
            if (elementIdentifier.Equals("MyAccountBrand") || elementIdentifier.Equals("ATB_ExistingAccountPassword"))
            {
                Console.WriteLine("Mag debug point");
            }

            List<By> identifiers = new List<By>();
            String id = null;
            String name = null;
            String xpath = null;
            id = elementdictionary.GetId(elementIdentifier);
            name = elementdictionary.GetName(elementIdentifier);
            xpath = elementdictionary.GetXpath(elementIdentifier);

            if (id != null && id != "")
            {
                identifiers.Add(By.Id(id));
            }
            if (xpath != null && xpath != "")
            {
                identifiers.Add(By.XPath(xpath));
            }
            if (name != null && name != "")
            {
                identifiers.Add(By.Name(name));
            }
            foreach (By by in identifiers)
            {
                allChecks();
                List<IWebElement> found = new List<IWebElement>();
                try
                {
                    found = driver.FindElements(by).ToList();
                }
                catch (Exception ex)
                {
                    logger.recordOutcome("failed to find " + elementIdentifier + " using " + by, "Error");
                    logger.recordOutcome(ex);
                    if (!elementdictionary.FindException)
                    {
                        elementdictionary.SetNeedsRepair(true);
                    }
                }
                if (found.Count > 0)
                {
                    Console.WriteLine(elementIdentifier + " found " + by.ToString());
                    allChecks();
                    if (browser != "Android" && !elementdictionary.FindException)
                    {
                        elementdictionary.CheckDictionaryDefinition(elementIdentifier, found[0]);
                    }

                    IWebElement element = wait.Until(ExpectedConditions.ElementIsVisible(by));

                    //highlight element
                    //try
                    //{
                    //    var Js = (IJavaScriptExecutor)driver;
                    //    Js.ExecuteScript(@"$(arguments[0]).css({ ""border-width"" : ""4px"", ""border-style"" : ""solid"", ""border-color"" : ""red"" });", found);
                    //    easteregg(Js);
                    //}
                    //catch (Exception hi) { Console.WriteLine(hi); }

                    if (elementdictionary.FindException)
                    {
                        logger.recordOutcome("An element was found using a hard coded ID. This is not recomended for final tests, only during development", "warning");
                    }
                    //help stop stale elements
                    element = driver.FindElement(by);

                    return element;
                }
                if (!elementdictionary.FindException)
                {
                    elementdictionary.SetNeedsRepair(true);
                }
                logger.recordOutcome("failed to find " + elementIdentifier + " using " + by, "Error");
            }
            logger.recordOutcome(elementIdentifier + ": Not found using any method. Manual Repair Required", "Warning");
            return null;
        }

        public void Click(String elementIdentifier)
        {
            elementSuccess = false;

            while (!buffer.getReplayCompleted() || !elementSuccess)
            {
                try
                {
                    allChecks();
                    if (stopTest)
                    { return; }
                    var found = Find(elementIdentifier);
                    allChecks();
                    found.Click();
                    buffer.eventBufferRecorder(elementIdentifier, "click", "");
                    logger.recordOutcome(elementIdentifier + " clicked", "debug");
                    buffer.setReplayCompleted(true);
                    buffer.consecutiveFails = 0;
                    elementSuccess = true;
                }
                catch (NullReferenceException ne)
                {
                    logger.recordOutcome("No Web element was found", "Error");
                    logger.recordOutcome(ne);
                    elementSuccess = false;
                    buffer.eventBufferPlayer();
                    //break;
                }
                catch (Exception e)
                {
                    logger.recordOutcome(elementIdentifier + " unable to click element. Trying each corner", "Error");
                    logger.recordOutcome(e);
                    elementSuccess = ClickAllCorners(elementIdentifier);
                    buffer.setReplayCompleted(elementSuccess);
                }
            }
            buffer.setReplayCompleted(false);
            buffer.setBufferSuccess(false);
        }

        private Boolean ClickAllCorners(String elementIdentifier)
        {
            try
            {
                allChecks();
                var found = Find(elementIdentifier);
                //find element size
                int xSize = found.Size.Width;
                int ySize = found.Size.Height;
                Actions builder = new Actions(driver);
                //click top left
                try
                {
                    builder.MoveToElement(found, 1, 1).Click().Perform();
                    return true;
                }
                catch
                {
                    //click top right
                    try { builder.MoveToElement(found, 1, ySize - 1).Click().Perform(); return true; }
                    catch
                    {
                        //click bottom left
                        try { builder.MoveToElement(found, xSize - 1, 1).Click().Perform(); return true; }
                        catch
                        {
                            //click bottom right
                            try { builder.MoveToElement(found, xSize - 1, ySize - 1).Click().Perform(); return true; }
                            catch
                            {
                                logger.recordOutcome(elementIdentifier + " unable to click on any corner. Trying Javascript Click", "Error");
                                Javascriptclick(elementIdentifier);
                            }
                        }
                    }

                }
                return false;
            }
            catch (Exception e)
            {
                logger.recordOutcome(elementIdentifier + " unable to click on any corner. Trying Javascript Click", "Error");
                Javascriptclick(elementIdentifier);
                return true;
            }
        }

        private void Javascriptclick(String elementIdentifier)
        {//the reason we dont just use the find method is because that uses a wait which may be the reason the
         //normal click failed, so by not using Find we avoid that as a reason the click failed
            try
            {
                List<By> identifiers = new List<By>();
                String id = null;
                String name = null;
                String xpath = null;
                id = elementdictionary.GetId(elementIdentifier);
                name = elementdictionary.GetName(elementIdentifier);
                xpath = elementdictionary.GetXpath(elementIdentifier);

                if (id != null && id != "")
                {
                    identifiers.Add(By.Id(id));
                }
                if (xpath != null && xpath != "")
                {
                    identifiers.Add(By.XPath(xpath));
                }
                if (name != null && name != "")
                {
                    identifiers.Add(By.Name(name));
                }
                foreach (By by in identifiers)
                {
                    allChecks();
                    List<IWebElement> found = new List<IWebElement>();
                    try
                    {
                        found = driver.FindElements(by).ToList();
                    }
                    catch (Exception ex)
                    {
                        elementdictionary.SetNeedsRepair(true);
                        logger.recordOutcome("failed to find " + elementIdentifier + " using " + by, "Error");
                    }
                    if (found.Count > 0)
                    {
                        Console.WriteLine(elementIdentifier + " found " + by.ToString());
                        allChecks();
                        elementdictionary.CheckDictionaryDefinition(elementIdentifier, found[0]);

                        //highlight element
                        try
                        {
                            (driver as IJavaScriptExecutor).ExecuteScript(@"$(arguments[0]).css({ ""border-width"" : ""4px"", ""border-style"" : ""solid"", ""border-color"" : ""red"" });", found);
                            var elementId = found[0].GetAttribute("id");
                            //javascript click it with ID
                            (driver as IJavaScriptExecutor).ExecuteScript("document.getElementById(\"" + elementId + "\").click();");
                            logger.recordOutcome("Clicked on " + by.ToString() + " using Javascript Injection", "Debug");
                            return;
                        }

                        catch (Exception hi)
                        {
                            Console.WriteLine(hi);
                        }
                        elementdictionary.SetNeedsRepair(true);
                        logger.recordOutcome("failed to find " + elementIdentifier + " using " + by, "Error");
                    }
                    logger.recordOutcome(elementIdentifier + ": Not found using any method. Manual Repair Required", "Warning");
                    buffer.eventBufferPlayer();
                }
            }
            catch (Exception jsEx)
            {
                logger.recordOutcome("Javascript click failed.", "Error");
                logger.recordOutcome(jsEx);
            }
        }

        public void SendKeysClear(string elementIdentifier)
        {
            Boolean textMatch = false;
            elementSuccess = false;
            IWebElement found;
            allChecks();
            if (stopTest)
            { return; }
            while (!buffer.getReplayCompleted() || !elementSuccess)
            {
                try
                {
                    found = Find(elementIdentifier);
                    while (!textMatch && numberOfTextChecksToRun > 0)
                    {
                        //check if the item is a dropdown box.
                        String type = found.GetAttribute("type");
                        if (type.Equals("select-one", ignoringCase))
                        {

                        }
                        else
                        {
                            found.Clear();
                        }
                        textMatch = TextFieldCheck(found, "", false);
                    }
                    numberOfTextChecksToRun = 3;
                    buffer.eventBufferRecorder(elementIdentifier, "sendKeys", "");
                    elementSuccess = true;
                    buffer.setReplayCompleted(true);
                    buffer.consecutiveFails = 0;
                    logger.recordOutcome(elementIdentifier + "cleared", "debug");
                }
                catch (StaleElementReferenceException se)
                {
                    Console.WriteLine("caught Stale Element in Send keys");
                }
                catch (NullReferenceException ne)
                {
                    logger.recordOutcome("No Web element was found for " + elementIdentifier, "Error");
                    logger.recordOutcome(ne);
                    elementSuccess = false;
                    buffer.eventBufferPlayer();
                    //break;
                }
                catch (Exception e)
                {
                    buffer.eventBufferPlayer();
                    logger.recordOutcome(elementIdentifier + " UNABLE TO SEND KEYS", "Error");
                    logger.recordOutcome(e);
                }
            }
            buffer.setReplayCompleted(false);
        }

        public String FirstSelection(XElement managerElement)
        {//selects the first option in a dropdown box and returns its value
            String toReturn = "";

            //get target element
            String target = managerElement.Attribute("target").Value;
            IWebElement found = Find(target);

            //get element type
            String type = found.GetAttribute("type");

            if (type.Equals("select-one", ignoringCase))
            {
                SelectElement firstOption = new SelectElement(found);
                firstOption.SelectByIndex(1);
                try
                {
                    found = firstOption.SelectedOption;
                }
                catch (StaleElementReferenceException e)
                {
                    found = Find(target);
                    found = firstOption.SelectedOption;
                }
                toReturn = found.Text;

                logger.recordOutcome("Option Selected was " + toReturn, "Debug");
            }

            return toReturn;
        }

        public String GetText(XElement managerElement)
        {//gets the text in an element and returns it as a string.
            String toReturn = "";
            //get target element
            String target = managerElement.Attribute("target").Value;
            IWebElement found = Find(target);

            //get element type
            String type = found.GetAttribute("type");

            if (type != null && !(type.Equals("text", ignoringCase)) && !(type.Equals("password", ignoringCase))
                        && !(type.Equals("number", ignoringCase)))
            {
                SelectElement selected = new SelectElement(found);
                found = selected.SelectedOption;
            }
            toReturn = found.Text;
            if (toReturn.Equals("") || toReturn == null)
            {
                toReturn = found.GetAttribute("value");
            }

            logger.recordOutcome("Text found: " + Environment.NewLine + toReturn, "debug");
            return toReturn;
        }

        private void SelectDropDown(IWebElement element, String text)
        {
            SelectElement dropDown = new SelectElement(element);

            // find contents of dropdown
            var options = dropDown.Options;
            //foreach (var option in options)
            //{
            //    Console.WriteLine(option.Text);
            //}

            if (options.Count > 1)
            {
                try
                {
                    if (browser != "Android")
                    {
                        element.Click();
                    }
                    allChecks();
                    dropDown.SelectByValue(text);
                }
                catch (Exception e)
                {
                    try
                    {
                        dropDown.SelectByText(text);//select by text can be used to select by partial text match.
                                                    //Console.WriteLine(e.ToString());
                    }
                    catch (Exception e2)
                    {
                        //last resort just send keys
                        element.SendKeys(text);
                    }
                }
                allChecks();
                try
                {
                    if (browser != "Android")
                    {
                        element.Click();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("SelectDropDown click exception");
                }
            }
            else
            {
                //jsClick(element);// makes it click so the screenshot will show its empty
                //logReport(LogStatus.WARNING, element.getAttribute("id") + " was not populated with more than 1 option");
            }
        }

        public void SendKeys(String elementIdentifier, String text)
        {
            Boolean textMatch = false;
            elementSuccess = false;
            IWebElement found;
            allChecks();
            if (stopTest)
            { return; }
            while (!buffer.getReplayCompleted() || !elementSuccess)
            {
                try
                {
                    found = Find(elementIdentifier);
                    textMatch = TextFieldCheck(found, text, false);//check if the box already says what we want it to.
                    while (!textMatch && numberOfTextChecksToRun > 0)
                    {
                        //String foundId = found.GetAttribute("id");

                        //check if the item is a dropdown box.
                        String type = found.GetAttribute("type");
                        if (type.Equals("select-one", ignoringCase))
                        {
                            SelectDropDown(found, text);
                        }
                        else
                        {
                            found.SendKeys(text);
                        }

                        textMatch = TextFieldCheck(found, text, true);
                    }
                    numberOfTextChecksToRun = 3;
                    buffer.eventBufferRecorder(elementIdentifier, "sendKeys", text);
                    elementSuccess = true;
                    buffer.setReplayCompleted(true);
                    buffer.consecutiveFails = 0;
                    logger.recordOutcome(elementIdentifier + " '" + text + "' sent", "debug");
                }
                catch (StaleElementReferenceException se)
                {
                    Console.WriteLine("caught Stale Element in Send keys");
                }
                catch (NullReferenceException ne)
                {
                    logger.recordOutcome("No Web element was found for " + elementIdentifier, "Error");
                    logger.recordOutcome(ne);
                    elementSuccess = false;
                    buffer.eventBufferPlayer();
                    //break;
                }
                catch (Exception e)
                {
                    buffer.eventBufferPlayer();
                    logger.recordOutcome(elementIdentifier + " UNABLE TO SEND KEYS", "Error");
                    logger.recordOutcome(e);
                }
            }
            buffer.setReplayCompleted(false);
        }

        public void SendKeysNoCheck(string elementIdentifier, string text)
        {
            elementSuccess = false;
            IWebElement found;
            allChecks();
            if (stopTest)
            { return; }
            while (!buffer.getReplayCompleted() || !elementSuccess)
            {
                try
                {
                    found = Find(elementIdentifier);
                    //String foundId = found.GetAttribute("id");
                    String type = found.GetAttribute("type");
                    if (type.Equals("select-one", ignoringCase))
                    {
                        SelectDropDown(found, text);
                    }
                    else
                    {
                        found.SendKeys(text);
                    }
                    buffer.eventBufferRecorder(elementIdentifier, "sendKeysNoCheck", text);
                    elementSuccess = true;
                    buffer.setReplayCompleted(true);
                    buffer.consecutiveFails = 0;
                    logger.recordOutcome(elementIdentifier + " '" + text + "' sent without checking text", "Debug");
                }
                catch (StaleElementReferenceException se)
                {
                    Console.WriteLine("caught Stale Element in Send keys");
                }
                catch (NullReferenceException ne)
                {
                    logger.recordOutcome("No Web element was found for " + elementIdentifier, "Error");
                    logger.recordOutcome(ne);
                    elementSuccess = false;
                    buffer.eventBufferPlayer();
                    //break;
                }
                catch (Exception e)
                {
                    buffer.eventBufferPlayer();
                    logger.recordOutcome(elementIdentifier + " UNABLE TO SEND KEYS", "Error");
                    logger.recordOutcome(e);
                }
            }
            buffer.setReplayCompleted(false);
        }

        public void SendKeysTab(String elementIdentifier, String text)
        {
            if (elementIdentifier.Equals("MYA_MtaEditDriverPrimaryOccupationStatus"))
            {
                Console.WriteLine("MagDebugPoint");
            }

            allChecks();
            if (stopTest)
            { return; }
            Boolean textMatch = false;
            elementSuccess = false;
            while (!buffer.getReplayCompleted() || !elementSuccess)
            {
                try
                {
                    IWebElement found = Find(elementIdentifier);
                    textMatch = TextFieldCheck(found, text, false);
                    while (!textMatch && numberOfTextChecksToRun > 0)
                    {
                        String foundId = found.GetAttribute("id");
                        String type = found.GetAttribute("type");
                        if (type.Equals("select-one", ignoringCase))
                        {
                            SelectDropDown(found, text);
                        }
                        else
                        {
                            found.SendKeys(text);
                        }
                        textMatch = TextFieldCheck(found, text, true);
                    }
                    found.SendKeys(Keys.Tab);
                    numberOfTextChecksToRun = 3;
                    buffer.eventBufferRecorder(elementIdentifier, "sendKeysTab", text);
                    buffer.setReplayCompleted(true);
                    elementSuccess = true;
                    logger.recordOutcome(elementIdentifier + " '" + text + "' sent+TAB", "debug");
                    buffer.consecutiveFails = 0;
                    DirtyPause(500);
                }
                catch (StaleElementReferenceException se)
                {
                    Console.WriteLine("caught Stale Element in Send Keys Tab");
                }
                catch (NullReferenceException ne)
                {
                    logger.recordOutcome("No Web element was found", "Error");
                    logger.recordOutcome(ne);
                    elementSuccess = false;
                    buffer.eventBufferPlayer();
                    //break;
                }
                catch (Exception e)
                {
                    buffer.eventBufferPlayer();
                    logger.recordOutcome(elementIdentifier + " UNABLE TO SEND KEYS", "Error");
                    logger.recordOutcome(e);
                }
            }
            buffer.setReplayCompleted(false);
        }

        public void SendKeysSlow(String elementIdentifier, String text)
        {
            allChecks();
            if (stopTest)
            { return; }
            elementSuccess = false;
            Boolean textMatch = false;
            while (!buffer.getReplayCompleted() || !elementSuccess)
            {
                char[] brokenUpText = text.ToCharArray();
                try
                {
                    IWebElement found = Find(elementIdentifier);
                    textMatch = TextFieldCheck(found, text, false);
                    String foundId = found.GetAttribute("id");
                    while (!textMatch && numberOfTextChecksToRun > 0)
                    {
                        for (int i = 0; i < brokenUpText.Length; i++)
                        {
                            DirtyPause(100);
                            found.SendKeys((brokenUpText[i]).ToString());
                        }
                        textMatch = TextFieldCheck(found, text, true);
                        if (!textMatch)
                        {
                            // found.clear();
                        }
                    }
                    numberOfTextChecksToRun = 3;
                    buffer.eventBufferRecorder(elementIdentifier, "sendKeysSlow", text);
                    buffer.setReplayCompleted(true);
                    elementSuccess = true;
                    buffer.consecutiveFails = 0;
                    logger.recordOutcome(elementIdentifier + " '" + text + "' sent slowly", "Debug");
                }
                catch (StaleElementReferenceException se)
                {
                    Console.WriteLine("caught Stale Element in Send keys Slow");
                }
                catch (NullReferenceException ne)
                {
                    logger.recordOutcome("No Web element was found", "Error");
                    logger.recordOutcome(ne);
                    elementSuccess = false;
                    buffer.eventBufferPlayer();
                    //break;
                }
                catch (Exception e)
                {
                    buffer.eventBufferPlayer();
                    logger.recordOutcome(elementIdentifier + " UNABLE TO SEND KEYS", "Error");
                    logger.recordOutcome(e);
                }
            }
            buffer.setReplayCompleted(false);
        }

        public void SetElementSuccess(Boolean elementSuccess)
        {
            this.elementSuccess = elementSuccess;
        }

        public object ScrollTo(XElement managerElement)
        {
            Boolean completed = false;
            //find element
            var element = Find(managerElement.Attribute("target").Value);

            try
            {
                Actions actions = new Actions(driver);
                actions.MoveToElement(element);
                actions.Perform();
                completed = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return completed;
        }

        public string getAttribute(XElement managerElement)
        {
            string toReturn = "";
            string target = managerElement.Attribute("target").Value;
            string attribute = managerElement.Attribute("attribute").Value;

            var foundElement = Find(target);

            //get the text of the required attribute
            try
            {
                toReturn = foundElement.GetAttribute(attribute);
            }
            catch (Exception e)
            {
                logger.recordOutcome(e);
                logger.recordOutcome("Could not get Attribute. Check Spelling and case", "Warning");
                toReturn = "Could Not Get Desired Attribute";
            }
            return toReturn;
        }
    }
}
