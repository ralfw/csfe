using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using csfe.compilation.parsing;
using csfe.execution;

namespace csfe.compilation
{
    class Compiler
    {
        public static Flow Compile(string sourceFilename, string flowPath, Dictionary<string, ServiceInfo> services, out string errors)
        {
            errors = "";
            
            var parser = new Parser(new Scanner(sourceFilename)) {
                RegisteredServicenames = services.Keys.ToArray(),
                errors = {errorStream = new StringWriter(new StringBuilder())}
            };
            parser.Parse();

            if (Successfully_parsed()) {
                var weaver = new Weaver(flowPath, services);
                return weaver.Weave(parser.ASTroot);
            }

            errors = Summarize_errors();
            return null;
            
            
            bool Successfully_parsed() => parser.errors.errorStream.ToString() == "";
            string Summarize_errors() => $"Error(s) detected in flow source file '{sourceFilename}':\n{parser.errors.errorStream}";
        }
    }
}