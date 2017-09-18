using System;
using System.Collections.Generic;
using System.IO;
using csfe.operations;
using csfe.adapters;
using NUnit.Framework;

namespace csfe.tests
{
    [TestFixture()]
    public class Operations
    {
        [SetUp]
        public void Setup()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;

            var ioDirs = Directory.GetDirectories(".", "*", SearchOption.AllDirectories);
            foreach (var d in ioDirs) {
                if (d.EndsWith("output") || d.EndsWith("input") || d.EndsWith("tmp"))
                    Directory.Delete(d, true);
            }
        }


        [Test]
        public void Run_service_several_times_to_fetch_output_in_chron_order()
        {
            const string SERVICEDIR = "testflow1/service2"; 
            var sut = new Service(SERVICEDIR, "mono", "TestServiceToUpper.exe");
            File.WriteAllText("test.txt", "x");
            sut.AddInput("test.txt");
            sut.RunSync();
            
            sut = new Service(SERVICEDIR, "mono", "TestServiceToUpper.exe");
            File.WriteAllText("test.txt", "y");
            sut.AddInput("test.txt");
            sut.RunSync();

            sut = new Service(SERVICEDIR, "mono", "TestServiceToUpper.exe");
            File.WriteAllText("test.txt", "z");
            sut.AddInput("test.txt");
            sut.RunSync();
            
            var resultFilenames = sut.Output;

            Assert.AreEqual(3, resultFilenames.Length);
            Assert.AreEqual("X", File.ReadAllText(resultFilenames[0]));
            Assert.AreEqual("Y", File.ReadAllText(resultFilenames[1]));
            Assert.AreEqual("Z", File.ReadAllText(resultFilenames[2]));
        }


        [Test]
        public void Run_service_on_node()
        {
            const string SERVICEDIR = "testflow1/service2";
            var sut = new ServiceOperation(SERVICEDIR, "mono", "TestServiceToUpper.exe");

            var resultFilenames = new List<string>();
            sut.OnOutput += outputFilename => resultFilenames.Add(outputFilename);
            
            File.WriteAllText("test.txt", "hello");
            sut.Enqueue("test.txt");

            Assert.AreEqual(1, resultFilenames.Count);
            Assert.AreEqual("HELLO", File.ReadAllText(resultFilenames[0]));
        }

        
        
        [Test]
        public void Run_service_nodes_in_sequence()
        {
            var sutToUpper = new ServiceOperation("testflow1/service2", "mono", "TestServiceToUpper.exe");
            var sutAddArgs0 = new ServiceOperation("testflow1/service1", "mono", "TestServiceAppendArgs0.exe world");

            var results = new List<string>();
            sutToUpper.OnOutput += sutAddArgs0.Enqueue;
            sutAddArgs0.OnOutput += outputFilename => {
                results.Add(File.ReadAllText(outputFilename));
                File.Delete(outputFilename);
            };
            
            File.WriteAllText("test.txt", "the first hello");
            sutToUpper.Enqueue("test.txt");
            File.WriteAllText("test.txt", "the second hello");
            sutToUpper.Enqueue("test.txt");

            Assert.AreEqual(2, results.Count);
            Assert.AreEqual("THE FIRST HELLO\nworld", results[0]);
            Assert.AreEqual("THE SECOND HELLO\nworld", results[1]);
        }

        
        [Test]
        public void Run_flow_with_inbox_and_outbox()
        {
            var inbox = new InboxOperation("testflow1");
            var sutToUpper = new ServiceOperation("testflow1/service2", "mono", "TestServiceToUpper.exe");
            var sutAddArgs0 = new ServiceOperation("testflow1/service1", "mono", "TestServiceAppendArgs0.exe world");
            var outbox = new OutboxOperation("testflow1");
            
            var results = new List<string>();
            inbox.OnItemArrived += sutToUpper.Enqueue;
            sutToUpper.OnOutput += sutAddArgs0.Enqueue;
            sutAddArgs0.OnOutput += outbox.AddFile;
            outbox.OnItemArrived += outputFilename => {
                results.Add(File.ReadAllText(outputFilename));
                File.Delete(outputFilename);
            };
            
            inbox.AddText("the first hello");
            inbox.AddText("the second hello");

            Assert.AreEqual(2, results.Count);
            Assert.AreEqual("THE FIRST HELLO\nworld", results[0]);
            Assert.AreEqual("THE SECOND HELLO\nworld", results[1]);
        }

        
        [Test]
        public void Run_flow_with_switch()
        {
            var inbox = new InboxOperation("testflow1");
            var swtch = new SwitchOperation();
            var sutTest = new ServiceOperation("testflow1/service3", "mono", "TestForOccurrenceOfArgs0.exe 42");
            var sutToUpper = new ServiceOperation("testflow1/service2", "mono", "TestServiceToUpper.exe");
            var sutAddArgs0 = new ServiceOperation("testflow1/service1", "mono", "TestServiceAppendArgs0.exe world");
            var outboxToupper = new OutboxOperation("testflow1", "toupper-output");
            var outboxAppend = new OutboxOperation("testflow1", "append-output");
            
            var results = new List<string>();
            inbox.OnItemArrived += sutTest.Enqueue;
            sutTest.OnOutput += swtch.Switch;
            swtch.OnOutput["SUCCESS"] = sutToUpper.Enqueue;
            sutToUpper.OnOutput += outboxToupper.AddFile;
            swtch.OnOutput["FAILURE"] = sutAddArgs0.Enqueue;
            sutAddArgs0.OnOutput += outboxAppend.AddFile;
            
            inbox.AddText("the first hello 42 SUCCESS");
            inbox.AddText("the second hello FAILURE");

            var toupperFilenames = outboxToupper.Items;
            Assert.AreEqual(1, toupperFilenames.Length);
            Assert.AreEqual("THE FIRST HELLO 42 SUCCESS", File.ReadAllText(toupperFilenames[0]));

            var appendFilenames = outboxAppend.Items;
            Assert.AreEqual(1, appendFilenames.Length);
            var text = File.ReadAllText(appendFilenames[0]);
            Assert.IsTrue(text.IndexOf("not found") >= 0 && text.IndexOf("the second hello") >= 0);
        }
    }
}