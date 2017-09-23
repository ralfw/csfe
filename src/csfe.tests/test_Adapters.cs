using System;
using System.Diagnostics;
using System.IO;
using csfe.adapters;
using NUnit.Framework;

namespace csfe.tests
{
    [TestFixture()]
    public class Adapters
    {
        [SetUp]
        public void Setup() {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;

            var ioDirs = Directory.GetDirectories(".", "*", SearchOption.AllDirectories);
            foreach (var d in ioDirs) {
                if (d.EndsWith("output") || d.EndsWith("input") || d.EndsWith("tmp"))
                    Directory.Delete(d, true);
            }
        }


        [Test]
        public void Run_service_adapter()
        {
            const string SERVICEDIR = "testflow1/service1";
            var sut = new ServiceAdapter(SERVICEDIR, "mono", "TestServiceAppendArgs0.exe b");

            File.WriteAllText("test.txt", "a");
            Filesystem.Move_file("test.txt", Path.Combine(SERVICEDIR, "input", "test.txt"));

            sut.RunSync();

            var resultFilenames = Directory.GetFiles(Path.Combine(SERVICEDIR, "output"));
            Assert.AreEqual(1, resultFilenames.Length);
            Assert.AreEqual("a\nb", File.ReadAllText(resultFilenames[0]));
        }


        [Test]
        public void Run_service()
        {
            const string SERVICEDIR = "testflow1/service1";
            var sut = new Service(SERVICEDIR, "mono", "TestServiceAppendArgs0.exe c");

            File.WriteAllText("test.txt", "b");

            sut.AddInput("test.txt");
            sut.RunSync();
            var resultFilenames = sut.Output;

            Assert.AreEqual(1, resultFilenames.Length);
            Assert.AreEqual("b\nc", File.ReadAllText(resultFilenames[0]));
        }
        
        
        [Test]
        public void Run_Go_service()
        {
            const string SERVICEDIR = "testflow1/service4_go";
            
            var sut = new Service(SERVICEDIR, "%path/negate_exe", "");

            File.WriteAllText("test.txt", "1\n5");

            sut.AddInput("test.txt");
            sut.RunSync();
            var resultFilenames = sut.Output;

            Assert.AreEqual(1, resultFilenames.Length);
            Assert.AreEqual("-1\n5\n", File.ReadAllText(resultFilenames[0]));
        }
        
        
        [Test]
        public void Exploration_Go()
        {
            const string SERVICEDIR = "testflow1/service4_go";

            Directory.CreateDirectory(SERVICEDIR + "/input");
            Directory.CreateDirectory(SERVICEDIR + "/output");
            
            File.WriteAllText(SERVICEDIR + "/input/test.txt", "1\n5");
            
            var pi = new ProcessStartInfo {
                FileName = SERVICEDIR + "/negate_exe",
                Arguments = "",
                WorkingDirectory = SERVICEDIR,
                UseShellExecute = true,
                CreateNoWindow = true
            };
            var p = Process.Start(pi);
           
            p.WaitForExit();
            Assert.AreEqual(0, p.ExitCode);
        }
    }
}