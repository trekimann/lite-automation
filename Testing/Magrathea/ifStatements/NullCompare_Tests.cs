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
    class NullCompare_Tests
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
        public void desiredCondition_true_ObjectNull_ReturnsTrue()
        {
            Boolean condition = true;
            Object value = null;

            var result = ifs.NullCompare(condition, value);
            Assert.AreEqual(true, result);
        }
        [Test]
        public void desiredCondition_false_ObjectNull_ReturnsFalse()
        {
            Boolean condition = false;
            Object value = null;

            var result = ifs.NullCompare(condition, value);
            Assert.AreEqual(false, result);
        }
        [Test]
        public void desiredCondition_true_ObjectNotNull_ReturnsFalse()
        {
            Boolean condition = true;
            Object value = ifs;

            var result = ifs.NullCompare(condition, value);
            Assert.AreEqual(false, result);
        }
        [Test]
        public void desiredCondition_false_ObjectNotNull_ReturnsTrue()
        {
            Boolean condition = false;
            Object value = ifs;

            var result = ifs.NullCompare(condition, value);
            Assert.AreEqual(true, result);
        }

    }
}
