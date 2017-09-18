using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;

namespace csfe.tests
{
    [TestFixture]
    public class test_Runtime
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
        public void Acceptance()
        {
            File.WriteAllText(SOURCE_FILENAME, "toupper > service1(\"41\") > service1(\"1\")");
            
            var fex = new FlowExecutionEngine(SOURCE_FILENAME, "testflow1");
            
            Debug.Print(fex.Errors);
            Assert.IsTrue(fex.Errors == "");
            
            fex.ProcessText("hello");
            
            Assert.AreEqual("HELLO\n41\n1", File.ReadAllText(fex.Output[0]));
            File.Delete(fex.Output[0]);
        }
    }
}