using System.Xml.Linq;

namespace Magrathea.Executors
{
    public interface ITestLoggerExecutor
    {
        bool debugPhase { get; set; }

        void Log(XElement loggerFunction);
        string LogMessage(XElement loggerFunction);
    }
}