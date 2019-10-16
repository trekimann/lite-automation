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
    class IntCompare_Tests
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
        //-----------------------condition lessThan------------------
        [Test]
        public void condition_lessThan_ValueFromMapLessThanIntToCompare_returnsTrue()
        {
            //valueFromMap is the value which is stored in the datamap
            int valueFromMap = 1;
            int valueToCompreAgainst = 100;

            var result = ifs.IntCompare("lessThan", valueFromMap, valueToCompreAgainst);

            Assert.AreEqual(true, result);
        }
        [Test]
        public void condition_lessThan_ValueFromMapMoreThanIntToCompare_returnsFalse()
        {
            //valueFromMap is the value which is stored in the datamap
            int valueFromMap = 100;
            int valueToCompreAgainst = 1;

            var result = ifs.IntCompare("lessThan", valueFromMap, valueToCompreAgainst);

            Assert.AreEqual(false, result);
        }
        [Test]
        public void condition_lessThan_ValueFromMapSameThanIntToCompare_returnsFalse()
        {
            //valueFromMap is the value which is stored in the datamap
            int valueFromMap = 100;
            int valueToCompreAgainst = 100;

            var result = ifs.IntCompare("lessThan", valueFromMap, valueToCompreAgainst);

            Assert.AreEqual(false, result);
        }
        //-----------------------condition lessThan------------------

        //-----------------------condition lessThanOrEqual------------------
        [Test]
        public void condition_lessThanOrEqual_ValueFromMapLessThanIntToCompare_returnsTrue()
        {
            //valueFromMap is the value which is stored in the datamap
            int valueFromMap = 1;
            int valueToCompreAgainst = 100;

            var result = ifs.IntCompare("lessThanOrEqual", valueFromMap, valueToCompreAgainst);

            Assert.AreEqual(true, result);
        }
        [Test]
        public void condition_lessThanOrEqual_ValueFromMapMoreThanIntToCompare_returnsFalse()
        {
            //valueFromMap is the value which is stored in the datamap
            int valueFromMap = 100;
            int valueToCompreAgainst = 1;

            var result = ifs.IntCompare("lessThanOrEqual", valueFromMap, valueToCompreAgainst);

            Assert.AreEqual(false, result);
        }
        [Test]
        public void condition_lessThanOrEqual_ValueFromMapSameThanIntToCompare_returnsFalse()
        {
            //valueFromMap is the value which is stored in the datamap
            int valueFromMap = 100;
            int valueToCompreAgainst = 100;

            var result = ifs.IntCompare("lessThanOrEqual", valueFromMap, valueToCompreAgainst);

            Assert.AreEqual(true, result);
        }
        //-----------------------condition lessThan------------------
        //-----------------------condition moreThan------------------
        [Test]
        public void condition_moreThan_ValueFromMapLessThanIntToCompare_returnsFalse()
        {
            //valueFromMap is the value which is stored in the datamap
            int valueFromMap = 1;
            int valueToCompreAgainst = 100;

            var result = ifs.IntCompare("moreThan", valueFromMap, valueToCompreAgainst);

            Assert.AreEqual(false, result);
        }
        [Test]
        public void condition_moreThan_ValueFromMapMoreThanIntToCompare_returnsTrue()
        {
            //valueFromMap is the value which is stored in the datamap
            int valueFromMap = 100;
            int valueToCompreAgainst = 1;

            var result = ifs.IntCompare("moreThan", valueFromMap, valueToCompreAgainst);

            Assert.AreEqual(true, result);
        }
        [Test]
        public void condition_moreThan_ValueFromMapSameAsIntToCompare_returnsFalse()
        {
            //valueFromMap is the value which is stored in the datamap
            int valueFromMap = 100;
            int valueToCompreAgainst = 100;

            var result = ifs.IntCompare("moreThan", valueFromMap, valueToCompreAgainst);

            Assert.AreEqual(false, result);
        }
        //-----------------------condition moreThan------------------
        //-----------------------condition moreThanOrEqual------------------
        [Test]
        public void condition_moreThanOrEqual_ValueFromMapLessThanIntToCompare_returnsFalse()
        {
            //valueFromMap is the value which is stored in the datamap
            int valueFromMap = 1;
            int valueToCompreAgainst = 100;

            var result = ifs.IntCompare("moreThanOrEqual", valueFromMap, valueToCompreAgainst);

            Assert.AreEqual(false, result);
        }
        [Test]
        public void condition_moreThanOrEqual_ValueFromMapMoreThanIntToCompare_returnsTrue()
        {
            //valueFromMap is the value which is stored in the datamap
            int valueFromMap = 100;
            int valueToCompreAgainst = 1;

            var result = ifs.IntCompare("moreThanOrEqual", valueFromMap, valueToCompreAgainst);

            Assert.AreEqual(true, result);
        }
        [Test]
        public void condition_moreThanOrEqual_ValueFromMapSameAsIntToCompare_returnsTrue()
        {
            //valueFromMap is the value which is stored in the datamap
            int valueFromMap = 100;
            int valueToCompreAgainst = 100;

            var result = ifs.IntCompare("moreThanOrEqual", valueFromMap, valueToCompreAgainst);

            Assert.AreEqual(true, result);
        }
        //-----------------------condition moreThanOrEqual------------------
        //-----------------------condition equalTo------------------
        [Test]
        public void condition_equalTo_ValueFromMapLessThanIntToCompare_returnsFalse()
        {
            //valueFromMap is the value which is stored in the datamap
            int valueFromMap = 1;
            int valueToCompreAgainst = 100;

            var result = ifs.IntCompare("equalTo", valueFromMap, valueToCompreAgainst);

            Assert.AreEqual(false, result);
        }
        [Test]
        public void condition_equalTo_ValueFromMapMoreThanIntToCompare_returnsFalse()
        {
            //valueFromMap is the value which is stored in the datamap
            int valueFromMap = 100;
            int valueToCompreAgainst = 1;

            var result = ifs.IntCompare("equalTo", valueFromMap, valueToCompreAgainst);

            Assert.AreEqual(false, result);
        }
        [Test]
        public void condition_equalTo_ValueFromMapSameAsIntToCompare_returnsTrue()
        {
            //valueFromMap is the value which is stored in the datamap
            int valueFromMap = 100;
            int valueToCompreAgainst = 100;

            var result = ifs.IntCompare("equalTo", valueFromMap, valueToCompreAgainst);

            Assert.AreEqual(true, result);
        }
        //-----------------------condition equalTo------------------
        //-----------------------condition notEqualTo------------------
        [Test]
        public void condition_notEqualTo_ValueFromMapLessThanIntToCompare_returnsTrue()
        {
            //valueFromMap is the value which is stored in the datamap
            int valueFromMap = 1;
            int valueToCompreAgainst = 100;

            var result = ifs.IntCompare("notEqualTo", valueFromMap, valueToCompreAgainst);

            Assert.AreEqual(true, result);
        }
        [Test]
        public void condition_notEqualTo_ValueFromMapMoreThanIntToCompare_returnsTrue()
        {
            //valueFromMap is the value which is stored in the datamap
            int valueFromMap = 100;
            int valueToCompreAgainst = 1;

            var result = ifs.IntCompare("notEqualTo", valueFromMap, valueToCompreAgainst);

            Assert.AreEqual(true, result);
        }
        [Test]
        public void condition_notEqualTo_ValueFromMapSameAsIntToCompare_returnsFalse()
        {
            //valueFromMap is the value which is stored in the datamap
            int valueFromMap = 100;
            int valueToCompreAgainst = 100;

            var result = ifs.IntCompare("notEqualTo", valueFromMap, valueToCompreAgainst);

            Assert.AreEqual(false, result);
        }
        //-----------------------condition equalTo------------------


    }
}
