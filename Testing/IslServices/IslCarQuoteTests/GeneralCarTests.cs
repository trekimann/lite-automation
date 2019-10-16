using IslServices;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing.IslServices.IslCarQuoteTests
{   
    [TestFixture]
    public class GeneralCarTests
    {
        [Test]
        public void DebuggingCarQuoteSOAP()
        {
            CarQuoteDetails details = new CarQuoteDetails();
            IslCarQuote quote = new IslCarQuote(details);

            quote.GetQuote();

            while (!quote.quotesReturned) { }

            var quotes = quote.HPquoteNumber;
        }
    }
}
