using Magrathea.ifStatements;
using NUnit.Framework;
using System;

namespace Testing.Magrathea.ifStatements
{
    class StringCompare_Tests
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

            [Test]
            public void stringToMatch_SingleString_ConditionEquals_StringsMatch_RetursnTrue()
            {
                String condition = "equals";
                String value = "Match";
                String stringTomatch = "Match";

                var result = ifs.StringCompare(condition, value, stringTomatch);

                Assert.AreEqual(true, result);
            }
            [Test]
            public void stringToMatch_SingleString_ConditionEquals_StringsDontMatch_RetursnFalse()
            {
                String condition = "equals";
                String value = "Match";
                String stringTomatch = "DontMatch";

                var result = ifs.StringCompare(condition, value, stringTomatch);

                Assert.AreEqual(false, result);
            }

            [Test]
            public void stringToMatch_PipeSplit_ConditionEquals_EitherStringMatch_ReturnsTrue()
            {
                String condition = "equals";
                String value = "Match";
                String stringTomatch = "Match||Found";

                var result = ifs.StringCompare(condition, value, stringTomatch);

                Assert.AreEqual(true, result);
            }
            [Test]
            public void stringToMatch_PipeSplit_ConditionEquals_NeitherStringMatch_ReturnsFalse()
            {
                String condition = "equals";
                String value = "Match";
                String stringTomatch = "Not||Found";

                var result = ifs.StringCompare(condition, value, stringTomatch);

                Assert.AreEqual(false, result);
            }

            [Test]
            public void stringToMatch_PipeSplit_ConditionEquals_LastStringMatch_ReturnsTrue()
            {
                String condition = "equals";
                String value = "Found";
                String stringTomatch = "NotThisOne||OrThisOne||Found";

                var result = ifs.StringCompare(condition, value, stringTomatch);

                Assert.AreEqual(true, result);
            }

        }
    }
}
