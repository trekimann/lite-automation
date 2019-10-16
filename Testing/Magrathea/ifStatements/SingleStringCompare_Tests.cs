using Magrathea.ifStatements;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing.Magrathea.ifStatements
{
    [TestFixture]
    class SingleStringCompare_Tests
    {
        IfStatements ifs;

        [SetUp]
        public void init()
        {
            ifs = new IfStatements(null);
        }

        [TearDown]
        public void tear()
        {
            ifs = null;
        }

        //-----------------------condition tests------------------
        //-----------------------condition equals------------------
        [Test]
        public void condition_equals_valueAndStringMatch_ReturnsTrue()
        {
            String condition = "equals";
            String value = "Match";
            String stringToMatch = "Match";

            var result = ifs.SingleStringCompare(condition, value, stringToMatch);

            Assert.AreEqual(true, result);
        }

        [Test]
        public void condition_equals_valueAndStringDONTMatch_ReturnsFalse()
        {
            String condition = "equals";
            String value = "Match";
            String stringToMatch = "DontMatch";

            var result = ifs.SingleStringCompare(condition, value, stringToMatch);

            Assert.AreEqual(false, result);
        }
        //-----------------------condition equals------------------

        //-----------------------condition startsWith------------------
        [Test]
        public void condition_startsWith_valueAndStringStartTheSame_ReturnsTrue()
        {
            String condition = "startsWith";
            String value = "Match more text after";
            String stringToMatch = "Match";

            var result = ifs.SingleStringCompare(condition, value, stringToMatch);

            Assert.AreEqual(true, result);
        }

        [Test]
        public void condition_startsWith_valueAndStringDONTStartTheSame_ReturnsFalse()
        {
            String condition = "startsWith";
            String value = "Dont start the same";
            String stringToMatch = "Match";

            var result = ifs.SingleStringCompare(condition, value, stringToMatch);

            Assert.AreEqual(false, result);
        }
        //-----------------------condition startsWith------------------

        //-----------------------condition contains------------------
        [Test]
        public void condition_contains_valueContainsStringToMatch_ReturnsTrue()
        {
            String condition = "contains";
            String value = "This string has the word Match in it";
            String stringToMatch = "Match";

            var result = ifs.SingleStringCompare(condition, value, stringToMatch);

            Assert.AreEqual(true, result);
        }

        [Test]
        public void condition_contains_valueDoesNOTContainStringToMatch_ReturnsFalse()
        {
            String condition = "contains";
            String value = "Doesnt Have the word in it";
            String stringToMatch = "Match";

            var result = ifs.SingleStringCompare(condition, value, stringToMatch);

            Assert.AreEqual(false, result);
        }
        [Test]
        public void condition_contains_valueContainsStringToMatch_DifferentCase_ReturnsFalse()
        {
            String condition = "contains";
            String value = "This string has the word match in it but lowercase";
            String stringToMatch = "Match";

            var result = ifs.SingleStringCompare(condition, value, stringToMatch);

            Assert.AreEqual(false, result);
        }
        //-----------------------condition contains------------------

    }
}
