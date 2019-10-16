using System.Xml.Linq;

namespace Magrathea.Executors
{
    interface IElementManagerExecutor
    {
        object ElementExecution(XElement managerElement);
        string SendKeys(string target, string variable, string type);
    }
}