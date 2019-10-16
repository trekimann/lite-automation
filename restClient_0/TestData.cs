using System.Collections.Generic;

namespace restClient_0
{
    internal class TestData
    {
        public string testName { get; set; }
        public string testUrl { get; set; }
        public string outputLocation { get; set; }
        public List<JourneyData> testData { get; set; } = new List<JourneyData>();

        public TestData(string TestName, string TestUrl, string outputLocation)
        {
            this.testName = TestName;
            this.testUrl = TestUrl;
            this.outputLocation = outputLocation;
        }

        public void addJourney(JourneyData journey)
        {
            testData.Add(journey);
        }
        
    }
}