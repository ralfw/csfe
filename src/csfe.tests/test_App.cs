using System;
using System.IO;
using csfe.adapters;
using NUnit.Framework;

namespace csfe.tests
{
    [TestFixture]
    public class test_App
    {
        const string SOURCE_FILENAME = "flowsource.txt";
        
        [SetUp]
        public void Setup() {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;

            var ioDirs = Directory.GetDirectories(".", "*", SearchOption.AllDirectories);
            foreach (var d in ioDirs)
            {
                if (d.EndsWith("output") || d.EndsWith("input") || d.EndsWith("tmp"))
                    Directory.Delete(d, true);
            }
        }
        
        
        [Test]
        public void Run_with_params()
        {
            File.WriteAllText(SOURCE_FILENAME, "toupper > service1(\"b\")");
            
            App.Run(SOURCE_FILENAME, "testflow1", "testflow1/sampleInput");

            var outputFilenames = Directory.GetFiles("testflow1/output");
            Assert.AreEqual(1, outputFilenames.Length);
            Assert.AreEqual("A\nb", File.ReadAllText(outputFilenames[0]));
        }
        
        
        [Test]
        public void Run_with_args()
        {
            File.WriteAllText(SOURCE_FILENAME, "toupper > service1(\"b\")");
            
            App.Run(new[]{$"-s={SOURCE_FILENAME}", "-p=testflow1", "-i=testflow1/sampleInput"});

            var outputFilenames = Directory.GetFiles("testflow1/output");
            Assert.AreEqual(1, outputFilenames.Length);
            Assert.AreEqual("A\nb", File.ReadAllText(outputFilenames[0]));
        }


        [Test]
        public void Run_exe() {
            File.WriteAllText(SOURCE_FILENAME, "toupper > service1(\"b\")");
            
            var sut = new ServiceAdapter(Environment.CurrentDirectory, "mono", $"csfe.exe -s={SOURCE_FILENAME} -p=testflow1 -i=testflow1/sampleInput");
            sut.RunSync();
            
            var outputFilenames = Directory.GetFiles("testflow1/output");
            Assert.AreEqual(1, outputFilenames.Length);
            Assert.AreEqual("A\nb", File.ReadAllText(outputFilenames[0]));
        }
    }
}