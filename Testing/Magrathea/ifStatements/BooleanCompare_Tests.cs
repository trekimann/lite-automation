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
    class BooleanCompare_Tests
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
        public void desiredCondition_True_actual_True_Returns_True()
        {            
            var desiredCondition = true;
            var actualValue = true;
            var result = ifs.BooleanCompare(desiredCondition, actualValue);

            Assert.AreEqual(true, result);
        }

        [Test]
        public void desiredCondition_false_actual_false_Returns_True()
        {
            var desiredCondition = false;
            var actualValue = false;
            var result = ifs.BooleanCompare(desiredCondition, actualValue);

            Assert.AreEqual(true, result);
        }

        [Test]
        public void desiredCondition_True_actual_false_Returns_false()
        {
            var desiredCondition = true;
            var actualValue = false;
            var result = ifs.BooleanCompare(desiredCondition, actualValue);

            Assert.AreEqual(false, result);
        }

        [Test]
        public void desiredCondition_false_actual_true_Returns_false()
        {
            var desiredCondition = true;
            var actualValue = false;
            var result = ifs.BooleanCompare(desiredCondition, actualValue);

            Assert.AreEqual(false, result);
        }
    }
}
