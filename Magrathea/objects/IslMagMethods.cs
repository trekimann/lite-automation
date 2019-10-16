using IslServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Magrathea
{
    public class IslMagMethods
    {
        private MagratheaExecutor magratheaExecutor;
        private StringComparison ignoringCase = StringComparison.InvariantCultureIgnoreCase;
        private List<String> quoteNames = new List<string>();

        public IslMagMethods(MagratheaExecutor magratheaExecutor)
        {
            this.magratheaExecutor = magratheaExecutor;
        }

        public object Quote(XElement clone)
        {
            string quoteType = clone.Attribute("type").Value;
            string quoteBrand = clone.Attribute("brand").Value;
            string quoteTarget = clone.Attribute("target").Value;

            //find the values for the quote
            var dataValues = magratheaExecutor.InUsePhase.Element("Data").Elements().ToList();
            List<XElement> quoteElements = new List<XElement>();
            foreach (XElement element in dataValues)
            {
                if (element.Attribute("type").Value.Equals("quote", ignoringCase) && element.Attribute("name").Value.Equals(quoteTarget, ignoringCase))
                {
                    quoteElements = element.Elements().ToList();
                    break;
                }
            }
            //get a list of the quote variables
            List<string> quoteVariableNames = new List<string>();
            foreach (XElement element in quoteElements)
            {
                quoteVariableNames.Add(element.Attribute("name").Value);
            }                       

            //want the deeplink?
            var deeplinkPair = magratheaExecutor.DataValuesMap("QTB_Deeplink");
            Boolean deepLinkRequired = (Boolean)deeplinkPair.Value;

            if (quoteType.Equals("car", ignoringCase))
            {
                CarQuoteDetails carDetails = new CarQuoteDetails();
                var variables = typeof(CarQuoteDetails).GetProperties();

                foreach (string variableName in quoteVariableNames)
                {
                    Object value = magratheaExecutor.DataValuesMap(variableName).Value;
                    foreach (PropertyInfo property in variables)
                    {
                        if (property.Name.Equals("_" + variableName))
                        {
                            property.SetValue(carDetails, value);
                        }
                    }
                }

                IslCarQuote islCar = new IslCarQuote(carDetails);
                islCar.GetQuote();
                string toReturn = "";
                while (!islCar.quotesReturned) { }
                if (quoteBrand.Equals("HD", ignoringCase))
                {
                    if (deepLinkRequired)
                    {
                        toReturn = islCar.HDdeeplink;
                    }
                    else
                    {
                        toReturn = islCar.HDquoteNumber;
                    }
                }
                else if (quoteBrand.Equals("HE", ignoringCase))
                {
                    if (deepLinkRequired)
                    {
                        toReturn = islCar.HEdeeplink;
                    }
                    else
                    {
                        toReturn = islCar.HEquoteNumber;
                    }
                }
                else if (quoteBrand.Equals("HP", ignoringCase))
                {
                    if (deepLinkRequired)
                    {
                        toReturn = islCar.HPdeeplink;
                    }
                    else
                    {
                        toReturn = islCar.HPquoteNumber;
                    }
                }
                else if (quoteBrand.Equals("PC", ignoringCase))
                {
                    if (deepLinkRequired)
                    {
                        toReturn = islCar.PCdeeplink;
                    }
                    else
                    {
                        toReturn = islCar.PCquoteNumber;
                    }
                }
                else if (quoteBrand.Equals("IP", ignoringCase))
                {
                    if (deepLinkRequired)
                    {
                        toReturn = islCar.IPdeeplink;
                    }
                    else
                    {
                        toReturn = islCar.IPquoteNumber;
                    }
                }
                return toReturn;
            }
            else if (quoteType.Equals("van", ignoringCase))
            {
                VanQuoteDetails vanDetails = new VanQuoteDetails();
                var variables = typeof(VanQuoteDetails).GetProperties();

                foreach (string variableName in quoteVariableNames)
                {
                    Object value = magratheaExecutor.DataValuesMap(variableName).Value;
                    foreach (PropertyInfo property in variables)
                    {
                        if (property.Name.Equals("_" + variableName))
                        {
                            property.SetValue(vanDetails, value);
                        }
                    }
                }

                IslVanQuote islVan = new IslVanQuote(vanDetails);
                islVan.GetQuote();
                string toReturn = "";
                while (!islVan.QuotesReturned) { }
                if (quoteBrand.Equals("HD", ignoringCase))
                {
                    if (deepLinkRequired)
                    {
                        toReturn = islVan.HDdeeplink;
                    }
                    else
                    {
                        toReturn = islVan.HDquoteNumber;
                    }
                }                
                else if (quoteBrand.Equals("HP", ignoringCase))
                {
                    if (deepLinkRequired)
                    {
                        toReturn = islVan.HPdeeplink;
                    }
                    else
                    {
                        toReturn = islVan.HPquoteNumber;
                    }
                }
                return toReturn;
            }
            return null;
        }
    }
}
