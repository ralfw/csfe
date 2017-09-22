using System;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace digitvalues.tests
{
    [TestFixture]
    public class Tests
    {
        [SetUp]
        public void Setup() {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;

            Directory.CreateDirectory("input");
            Directory.CreateDirectory("output");

            Delete_all_files("input");
            Delete_all_files("output");
        }
        
        
        [Test]
        public void Test1()
        {
            File.WriteAllText("input/test.txt", "IVXLCDM");
            
            digitvalues.Program.Main(null);

            var outputFilename = Directory.GetFiles("output").First();
            Assert.AreEqual(new[]{"1", "5", "10", "50", "100", "500", "1000"}, File.ReadAllLines(outputFilename));
        }


        void Delete_all_files(string directoryPath)
        {
            var filenames = Directory.GetFiles(directoryPath);
            foreach(var f in filenames) File.Delete(f);
        }
    }
}