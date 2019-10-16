using Magrathea;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing.Magrathea.xmlParser
{
    [TestFixture]
    class XmlParser_Tests
    {
        XmlParser TestParser;
        String TempDirectory;

        String StringToSaveAsXml = "<ElementDictionary>\r\n  <tutorial>\r\n    <Definition>googleSearchBox<Id /><Name>q</Name><xPath>/html[1]/body[1]/div[1]/div[3]/form[1]/div[2]/div[1]/div[1]/div[1]/div[1]/input[1]</xPath></Definition>\r\n  </tutorial>\r\n</ElementDictionary>";

        [SetUp]
        public void SetUp()
        {
            TestParser = new XmlParser();
        }
        [TearDown]
        public void TearDown()
        {
            TestParser = null;
            removeTemporayFile();
        }

        public void makeXMLDictForTest(Boolean goodXml,String directory)
        {
            if(!goodXml)
            {//the ID element is not closed making it bad XML
                StringToSaveAsXml = "<ElementDictionary>\r\n  <tutorial>\r\n    <Definition>googleSearchBox<Id><Name>q</Name><xPath>/html[1]/body[1]/div[1]/div[3]/form[1]/div[2]/div[1]/div[1]/div[1]/div[1]/input[1]</xPath></Definition>\r\n  </tutorial>\r\n</ElementDictionary>";
            }

            TempDirectory = directory;
            File.WriteAllText(directory, StringToSaveAsXml);
        }

        private void removeTemporayFile()
        {
            File.Delete(TempDirectory);
        }

        //----------------------------------ParseXml----------------------------------

        [Test]
        public void ParseXml_returnsParsedXmlAsXDocument_Happy()
        {
            Boolean goodXml = true;
            String temporaryFileDirectory = Directory.GetCurrentDirectory()+ @"\ElementDictionary.xml";

            makeXMLDictForTest(goodXml, temporaryFileDirectory);

            var parsedData = TestParser.ParseXml(temporaryFileDirectory).ToString();
            
            Assert.AreEqual(StringToSaveAsXml, parsedData);            
        }

        [Test]
        public void ParseXml_ParsedXmlAsXDocument_BadXmlInDictionary_returnsNull()
        {
            Boolean goodXml = false;
            String temporaryFileDirectory = Directory.GetCurrentDirectory() + @"\ElementDictionary.xml";

            makeXMLDictForTest(goodXml, temporaryFileDirectory);

            var parsedData = TestParser.ParseXml(temporaryFileDirectory);

            Assert.AreEqual(null, parsedData);
        }

        [Test]
        public void ParseXml_ParsedXmlAsXDocument_BadXmlInTest_returnsFailedTestParsingXml()
        {
            Boolean goodXml = false;
            String temporaryFileDirectory = Directory.GetCurrentDirectory() + @"\USTestingDictionary.xml";

            makeXMLDictForTest(goodXml, temporaryFileDirectory);

            var parsedData = TestParser.ParseXml(temporaryFileDirectory);

            //get name of test
            var testName= parsedData.Attribute("name").Value;

            Assert.AreEqual("FAILED_TO_PARSE", testName);
        }

    }
}
