using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection.Emit;
using csfe.compilation;
using Microsoft.SqlServer.Server;
using NUnit.Framework;

namespace csfe.tests
{
    [TestFixture]
    public class test_compiler
    {
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
        
        
        const string SOURCE_FILENAME = "flowsource.txt";
        
        [Test]
        public void Compile() {
            File.WriteAllText(SOURCE_FILENAME, "toupper");

            var services = new Dictionary<string, ServiceInfo>{
                {
                    "toupper",
                    new ServiceInfo{Path = "testflow1/service2", Executable="mono", Arguments="TestServiceToUpper.exe"}
                }
            };
            
            var flow = Compiler.Compile(SOURCE_FILENAME, "testflow1", services, out string _);
            
            // Run
            flow.ProcessText("hello");
            var results = flow.Output;
            
            Assert.AreEqual(1, results.Length);
            Assert.AreEqual("HELLO", File.ReadAllText(results[0]));
        }
        
        
        [Test]
        public void Compile_with_error() {
            File.WriteAllText(SOURCE_FILENAME, "XYZ");

            var services = new Dictionary<string, ServiceInfo>{
                {
                    "toupper",
                    new ServiceInfo{Path = "testflow1/service2", Executable="mono", Arguments="TestServiceToUpper.exe"}
                }
            };
            
            var flow = Compiler.Compile(SOURCE_FILENAME, "testflow1", services, out string errors);
            
            Assert.IsNull(flow);
            Debug.Print(errors);
            Assert.IsTrue(errors.IndexOf("XYZ") > 0);
        }
    }
}