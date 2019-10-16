using OpenQA.Selenium;
using Selenium.testLogger;
using System;


namespace Selenium.htmlElementManager
{
    public class ReplayBuffer
    {
        private ITestLogger logger;
        private IHtmlElementManager manager;

        private int bufferAttempts;
        private int numberOfAttemptsToMake = 5;
        private String lastGoodCommand;
        private String lastGoodElementId;
        private String lastGoodText;
        private Boolean replayCompleted = false;
        private Boolean bufferSuccess = false;
        public int consecutiveFails { get; set; }

        public ReplayBuffer(IHtmlElementManager manager, ITestLogger logger, int numberOfAttemptsToMake)
        {
            this.logger = logger;
            this.manager = manager;
            this.numberOfAttemptsToMake = numberOfAttemptsToMake;
            this.consecutiveFails = 0;
        }

        public void eventBufferRecorder(String elementId, String command, String text)
        {
            lastGoodCommand = command;
            lastGoodElementId = elementId;
            lastGoodText = text;
            bufferAttempts = numberOfAttemptsToMake;
            bufferSuccess = true;
        }

        public void eventBufferPlayer()
        {
            
            if (consecutiveFails >= 2)
            {
                logger.recordOutcome("2 consecutive elements falied to operate. Stopping test","Fail");
                manager.stopTest = true;
                consecutiveFails = 0;
                bufferSuccess = true;
                replayCompleted = true;
                manager.SetElementSuccess(true);
            }
            if (!manager.stopTest)
            {
                manager.allChecks();
                bufferSuccess = false;
                while (!bufferSuccess)
                {
                    if (bufferAttempts <= numberOfAttemptsToMake)
                    {
                        logger.recordOutcome("Attempting Last Good Command. Atempt " + bufferAttempts,"Warning");
                        if (lastGoodCommand.Equals("click"))
                        {
                            replayClick(lastGoodElementId);
                        }
                        else if (lastGoodCommand.Equals("sendKeys"))
                        {
                            replaySendKeys(lastGoodElementId, lastGoodText);
                        }
                        else if (lastGoodCommand.Equals("sendKeysTab"))
                        {
                            replaySendKeysTab(lastGoodElementId, lastGoodText);
                        }
                        else if (lastGoodCommand.Equals("sendKeysSlow"))
                        {
                            replaySendKeysSlow(lastGoodElementId, lastGoodText);
                        }
                        bufferAttempts--;
                        Console.WriteLine(bufferAttempts);
                    }
                    replayCompleted = true;
                    if (bufferAttempts <= 0)
                    {
                        logger.recordOutcome("Could not replay last command successfully. Attempting next command","Warning");
                        replayCompleted = true;
                        bufferAttempts = numberOfAttemptsToMake;
                        bufferSuccess = true;

                        consecutiveFails++;
                        Console.WriteLine(consecutiveFails + " replay fails");
                        manager.SetElementSuccess(true);
                    }                    
                }
            }
        }

        private void replayClick(String elementId)
        {
            Console.WriteLine("replay Click on " + elementId);
            manager.allChecks();
            try
            {
                manager.Find(elementId).Click();
                logger.recordOutcome(elementId + " clicked");
                bufferSuccess = true;
            }
            catch (Exception e)
            {
                logger.recordOutcome(elementId + " UNABLE TO CLICK.","Warning");
                logger.recordOutcome(e);
            }
        }

        private void replaySendKeys(String elementId, String text)
        {
            Console.WriteLine("replaySendKeys on " + elementId);
            IWebElement found;
            Boolean textMatch = false;
            manager.allChecks();
            try
            {
                while (!textMatch && manager.getNumberOfTextChecksToRun() > 0)
                {
                    found = manager.Find(elementId);
                    String foundId = found.GetAttribute("id");
                    found.SendKeys(text);
                    textMatch = manager.TextFieldCheck(elementId, text,true);
                }
                manager.setNumberOfTextChecksToRun(3);
                logger.recordOutcome(elementId + " '" + text + "' sent");
                bufferSuccess = true;
            }
            catch (Exception e)
            {
                logger.recordOutcome(elementId + " UNABLE TO SEND KEYS.","warning");
                logger.recordOutcome(e);
            }
        }

        private void replaySendKeysTab(String elementId, String text)
        {
            Console.WriteLine("replaySendKeysTab on " + elementId);
            Boolean textMatch = false;
            manager.allChecks();
            try
            {
                IWebElement found = manager.Find(elementId);
                while (!textMatch && manager.getNumberOfTextChecksToRun() > 0)
                {
                    found = manager.Find(elementId);
                    String foundId = found.GetAttribute("id");
                    found.SendKeys(text);
                    textMatch = manager.TextFieldCheck(elementId, text,true);
                }
                manager.setNumberOfTextChecksToRun(3);
                manager.DirtyPause(500);
                found.SendKeys(Keys.Tab);
                logger.recordOutcome(elementId + " '" + text + "' sent+TAB");
                bufferSuccess = true;
            }
            catch (Exception e)
            {
                logger.recordOutcome(elementId + " UNABLE TO SEND KEYS.");
                    logger.recordOutcome(e);
            }
        }

        private void replaySendKeysSlow(String elementId, String text)
        {
            Console.WriteLine("replaySendKeysSlow on " + elementId);
            manager.allChecks();
            Boolean textMatch = false;
            char[] brokenUpText = text.ToCharArray();
            try
            {
                IWebElement found = manager.Find(elementId);
                String foundId = found.GetAttribute("id");
                while (!textMatch && manager.getNumberOfTextChecksToRun() > 0)
                {
                    for (int i = 0; i < brokenUpText.Length; i++)
                    {
                        manager.DirtyPause(100);
                        found.SendKeys((brokenUpText[i]).ToString());
                    }
                    textMatch = manager.TextFieldCheck(elementId, text,true);
                }
                manager.setNumberOfTextChecksToRun(3);
                logger.recordOutcome(elementId + " '" + text + "' sent slowly");
                bufferSuccess = true;
            }
            catch (Exception e)
            {
                logger.recordOutcome(elementId + " UNABLE TO SEND KEYS.","Warning");
                logger.recordOutcome(e);
            }
        }

        public Boolean getBufferSuccess()
        {
            return bufferSuccess;
        }

        public void setBufferSuccess(Boolean bufferSuccess)
        {
            this.bufferSuccess = bufferSuccess;
        }

        public Boolean getReplayCompleted()
        {
            return replayCompleted;
        }

        public void setReplayCompleted(Boolean replayCompleted)
        {
            this.replayCompleted = replayCompleted;
        }

        public void setNumeberOfAttemptsToMake(int numeberOfAttemptsToMake)
        {
            this.numberOfAttemptsToMake = numeberOfAttemptsToMake;
        }
    }
}
