using System;
using System.Collections.Generic;
using System.IO;
using csfe.compilation;
using NUnit.Framework;
using csfe.execution;

namespace csfe.tests
{
    [TestFixture]
    public class test_Weaver
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

        
        [Test]
        public void Weave_service_with_argument()
        {
            // Service discovery simulieren
            var services = new Dictionary<string, ServiceInfo>{
                {
                    "append",
                    new ServiceInfo{Path = "testflow1/service1", Executable="mono", Arguments="TestServiceAppendArgs0.exe %argument"}
                }
            };
            
            // Parser simulieren: append("42")
            var serviceAppendNode = new ServiceNode {Name = "append", Argument = "42"};
            var sequence = new OperationListNode();
            sequence.Operations.Add(serviceAppendNode);
            var rootNode = new ServiceFlowLangNode {Sequence = sequence};

            // Generate code
            var weaver = new Weaver("testflow1", services);
            var flow = weaver.Weave(rootNode);
            
            // Run
            flow.ProcessText("hello");
            var results = flow.Output;
            
            Assert.AreEqual(1, results.Length);
            Assert.AreEqual("hello\n42", File.ReadAllText(results[0]));
        }
        
        
        
        [Test]
        public void Sequence_weaving_with_flow()
        {
            // Service discovery simulieren
            var services = new Dictionary<string, ServiceInfo>{
                {
                    "toupper",
                    new ServiceInfo{Path = "testflow1/service2", Executable="mono", Arguments="TestServiceToUpper.exe"}
                },
                {
                    "append",
                    new ServiceInfo{Path = "testflow1/service1", Executable="mono", Arguments="TestServiceAppendArgs0.exe world"}
                }
            };
            
            // Parser simulieren: toupper > append
            var serviceToUpperNode = new ServiceNode { Name = "toupper" };
            var serviceAppendNode = new ServiceNode {Name = "append"};
            var sequence = new OperationListNode();
            sequence.Operations.Add(serviceToUpperNode);
            sequence.Operations.Add(serviceAppendNode);
            var rootNode = new ServiceFlowLangNode {Sequence = sequence};

            // Generate code
            var weaver = new Weaver("testflow1", services);
            var flow = weaver.Weave(rootNode);
            
            // Run
            flow.ProcessText("hello");
            var results = flow.Output;
            
            Assert.AreEqual(1, results.Length);
            Assert.AreEqual("HELLO\nworld", File.ReadAllText(results[0]));
        }
        
          
        [Test]
        public void Switch_weaving_with_flow()
        {
            // Service discovery simulieren
            var services = new Dictionary<string, ServiceInfo>{
                {
                    "toupper",
                    new ServiceInfo{Path = "testflow1/service2", Executable="mono", Arguments="TestServiceToUpper.exe"}
                },
                {
                    "append",
                    new ServiceInfo{Path = "testflow1/service1", Executable="mono", Arguments="TestServiceAppendArgs0.exe world"}
                },
                {
                    "check",
                    new ServiceInfo{Path = "testflow1/service3", Executable="mono", Arguments="TestForOccurrenceOfArgs0.exe 42"}
                }
            };
            
            
            // Parser simulieren: check > (SUCCESS: toupper, FAILURE: append)
            var serviceCheckNode = new ServiceNode {Name = "check"};
            
            var serviceToUpperNode = new ServiceNode { Name = "toupper" };
            var successSequences = new OperationListNode();
            successSequences.Operations.Add(serviceToUpperNode);

            var serviceAppendNode = new ServiceNode {Name = "append"};
            var failureSequence = new OperationListNode();
            failureSequence.Operations.Add(serviceAppendNode);
            
            var switchNode = new SwitchNode();
            switchNode.Options.Add(new SwitchOptionNode {
                Tag = "SUCCESS",
                Sequence = successSequences
            });
            switchNode.Options.Add(new SwitchOptionNode {
                Tag = "FAILURE",
                Sequence = failureSequence
            });
            
            var sequence = new OperationListNode();
            sequence.Operations.Add(serviceCheckNode);
            sequence.Operations.Add(switchNode);
            var rootNode = new ServiceFlowLangNode {Sequence = sequence};
            
            // Generate
            var weaver = new Weaver("testflow1", services);
            var flow = weaver.Weave(rootNode);
            
            // Run
            flow.ProcessText("failure");
            var results = flow.Output;
            Assert.AreEqual(1, results.Length);
            var resultText = File.ReadAllText(results[0]);
            Assert.IsTrue(resultText.IndexOf("failure") >= 0 && resultText.IndexOf("world") >= 0);
            File.Delete(results[0]);
            
            flow.ProcessText("success 42");
            results = flow.Output;
            Assert.AreEqual(1, results.Length);
            Assert.AreEqual("SUCCESS 42", File.ReadAllText(results[0]));
            File.Delete(results[0]);
        }
    }
}