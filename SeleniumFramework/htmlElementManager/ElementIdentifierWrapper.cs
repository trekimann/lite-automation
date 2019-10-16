using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selenium
{
    public class ElementIdentifierWrapper
    {
        public String elementId { get; private set; }
        public String elementName { get; private set; }
        public String elementXpath { get; private set; }
        public String journey { get; private set; }

        public String getJourney()
        {
            return journey;
        }

        public void SetJourney(String journey)
        {
            this.journey = journey;
        }

        public String GetElementId()
        {
            return elementId;
        }

        public void SetElementId(String elementId)
        {
            this.elementId = elementId;
        }

        public String GetElementName()
        {
            return elementName;
        }

        public void SetElementName(String elementName)
        {
            this.elementName = elementName;
        }

        public String GetElementXpath()
        {
            return elementXpath;
        }

        public void SetElementXpath(String elementXpath)
        {
            this.elementXpath = elementXpath;
        }

        public override String ToString()
        {
            String toReturn = "";

            toReturn = toReturn+ "elementId: " +elementId+"\n";
            toReturn = toReturn + "elementName: " + elementName + "\n";
            toReturn = toReturn + "elementXpath: " + elementXpath + "\n";
            //toReturn = toReturn + "journey: " + journey + "\n";
            return toReturn;
        }

    }
}
