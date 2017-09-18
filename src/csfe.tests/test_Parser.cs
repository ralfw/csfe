using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using csfe.compilation;
using csfe.compilation.parsing;
using NUnit.Framework;

namespace csfe.tests
{
    [TestFixture]
    public class test_Parser
    {
        const string SOURCE_FILENAME = "flowsource.txt";
        
        
        [Test]
        public void Single_service() {
            File.WriteAllText(SOURCE_FILENAME, "a.i(\"1 2 3\")");
            
            var scanner = new Scanner(SOURCE_FILENAME);

            var parser = new Parser(scanner);
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            parser.errors.errorStream = sw;

            parser.Parse();

            if (parser.errors.count > 0) {
                Debug.Print("Error(s) detected:");
                Debug.Print(sb.ToString());
            }
            Assert.AreEqual(0, parser.errors.count);
            var sn = parser.ASTroot.Sequence.Get<ServiceNode>(0);
            Assert.AreEqual("a", sn.Name);
            Assert.AreEqual("i", sn.Instance);
            Assert.AreEqual("1 2 3", sn.Argument);
        }
        
        
        [Test]
        public void Service_sequence()
        {
            File.WriteAllText(SOURCE_FILENAME, "a > b > c");
            
            var scanner = new Scanner(SOURCE_FILENAME);

            var parser = new Parser(scanner);
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            parser.errors.errorStream = sw;

            parser.Parse();

            if (parser.errors.count > 0) {
                Debug.Print("Error(s) detected:");
                Debug.Print(sb.ToString());
            }
            Assert.AreEqual(0, parser.errors.count);

            var serviceNames = parser.ASTroot.Sequence.Operations.Select(op => ((ServiceNode) op).Name).ToArray();
            Assert.AreEqual(new[]{"a", "b", "c"}, serviceNames);            
        }
        
        
        [Test]
        public void Switch_in_sequence()
        {
            File.WriteAllText(SOURCE_FILENAME, "a > (S: b, F: c) > d");
            
            var scanner = new Scanner(SOURCE_FILENAME);

            var parser = new Parser(scanner);
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            parser.errors.errorStream = sw;

            parser.Parse();

            if (parser.errors.count > 0) {
                Debug.Print("Error(s) detected:");
                Debug.Print(sb.ToString());
            }
            Assert.AreEqual(0, parser.errors.count);

            Assert.AreEqual("a", parser.ASTroot.Sequence.Get<ServiceNode>(0).Name);
            Assert.AreEqual("d", parser.ASTroot.Sequence.Get<ServiceNode>(2).Name);

            var swtch = parser.ASTroot.Sequence.Get<SwitchNode>(1);
            var opt = swtch.Options[0];
            Assert.AreEqual("S", opt.Tag);
            Assert.AreEqual("b", opt.Sequence.Get<ServiceNode>(0).Name);
            opt = swtch.Options[1];
            Assert.AreEqual("F", opt.Tag);
            Assert.AreEqual("c", opt.Sequence.Get<ServiceNode>(0).Name);
        }
        
        
        [Test]
        public void Valid_service_names() {
            File.WriteAllText(SOURCE_FILENAME, "a > (S: b, F: c) > d");
            
            var scanner = new Scanner(SOURCE_FILENAME);

            var parser = new Parser(scanner);
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            parser.errors.errorStream = sw;
            parser.RegisteredServicenames = new[] {"a", "b", "c", "d"};

            parser.Parse();

            Assert.AreEqual(0, parser.errors.count);
        }
        
        
        [Test]
        public void Invalid_service_names() {
            File.WriteAllText(SOURCE_FILENAME, "a > (S: x, F: c) > y");
            
            var scanner = new Scanner(SOURCE_FILENAME);

            var parser = new Parser(scanner);
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            parser.errors.errorStream = sw;
            
            parser.RegisteredServicenames = new[] {"a", "c"};
            parser.Parse();

            Assert.AreEqual(2, parser.errors.count);
            Debug.Print("Errors due to missing service names:");
            Debug.Print(sb.ToString());
        }
    }
}