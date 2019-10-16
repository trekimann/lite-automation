using System.Collections.Generic;

namespace restClient_0
{
    internal class JourneyData
    {
        public string journey { get; set; }
        public List<PhaseData> phaseData { get; set; } = new List<PhaseData>();

        public JourneyData() { }

        public JourneyData(string journey)
        {
            this.journey = journey;
        }

        public void addPhase(PhaseData phaseData)
        {
            this.phaseData.Add(phaseData);
        }

        
    }
}