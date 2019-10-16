using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IslServices;
using NUnit.Framework;

namespace Testing.IslServices.IslVanQuoteTests
{
    [TestFixture]
    public class GeneralVanTests
    {
        [Test]
        public void WhatDoesGetQuoteNeed()
        {
            VanQuoteDetails QuoteDetails = new VanQuoteDetails();

            IslVanQuote van = new IslVanQuote(QuoteDetails);
            van.GetQuote();

            while (!van.QuotesReturned) { }
            var quotes = van.HPquoteNumber;

        }
    }
}
