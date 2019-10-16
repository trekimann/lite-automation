using System.Collections.Generic;

namespace restClient_0
{
    internal class PhaseData
    {
        public string phase;
        public Dictionary<string, Dictionary<string, string>> phaseValue { get; set; } = new Dictionary<string, Dictionary<string, string>>();

        public void AddPhase(string phase, Dictionary<string,Dictionary<string,string>> phaseValue)
        {
            this.phase = phase;
            this.phaseValue = phaseValue;
        }

    }
}