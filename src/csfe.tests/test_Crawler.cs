using System;
using System.Diagnostics;
using csfe.adapters;
using NUnit.Framework;

namespace csfe.tests
{
    [TestFixture]
    public class test_Crawler
    {
        [SetUp]
        public void Setup() {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }
        
        
        [Test]
        public void Compile()
        {
            var services = ServiceCrawler.Compile_services("testflow1");
            
            Assert.AreEqual(4, services.Count);
            Assert.AreEqual("mono", services["service1"].Executable);
            Assert.AreEqual("testflow1/service2", services["toupper"].Path);            
            Assert.AreEqual("testforoccurrenceofargs0.exe %argument", services["check"].Arguments);
        }
    }
}