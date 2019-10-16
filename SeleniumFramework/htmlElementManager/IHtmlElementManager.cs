using System.Collections.Generic;
using System.Xml.Linq;
using OpenQA.Selenium;

namespace Selenium.htmlElementManager
{
    public interface IHtmlElementManager
    {
        bool Pause { get; set; }
        bool TestingFunction { get; set; }
        bool stopTest { get; set; }
        List<string> PauseTargets { get; set; }

        bool allChecks();
        bool checkForSorry();
        void Click(string elementIdentifier);
        void DirtyPause(int millisecond);
        void PauseTest();
        bool ElementAvailable(string elementIdentifier);
        IWebElement Find(string elementIdentifier);
        string FirstSelection(XElement managerElement);
        IWebDriver getDriver();
        int getNumberOfTextChecksToRun();
        string GetText(XElement managerElement);
        void SendKeys(string elementIdentifier, string text);
        void SendKeysClear(string elementIdentifier);
        void SendKeysNoCheck(string elementIdentifier, string text);
        void SendKeysSlow(string elementIdentifier, string text);
        void SendKeysTab(string elementIdentifier, string text);
        void setElementDictionary(ElementIdentifierDictionary elementdictionary);
        void SetElementSuccess(bool elementSuccess);
        void setNumberOfTextChecksToRun(int numberOfTextChecksToRun);
        void specificBrowserWaits();
        bool TextFieldCheck(IWebElement element, string textToCheck, bool log);
        bool TextFieldCheck(string elementIdentifier, string textToCheck, bool log);
        void WaitForPendingAjax();
        object ScrollTo(XElement managerElement);
        string getAttribute(XElement managerElement);
    }
}