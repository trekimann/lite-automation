using Magrathea.ElementOperations;
using Selenium.testLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Magrathea.Executors
{
    public interface IMagratheaExecutor
    {
        ITestInformation TestInfo { get; }
        Boolean TestingFunction { get; set; }
        Boolean debugPhase { get; }
        ITestLogger Logger { get; }
        typeObjectPair DataValuesMap(String variable);
        Object GetPhaseValue(XElement element);
        Boolean switchPause();
    }
}
