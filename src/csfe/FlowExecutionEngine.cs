using System;
using csfe.adapters;
using csfe.compilation;
using csfe.execution;

namespace csfe
{
    public class FlowExecutionEngine
    {
        private readonly Flow _flow;
        
        public FlowExecutionEngine(string flowSourceFilename, string flowPath)
        {
            var services = ServiceCrawler.Compile_services(flowPath);
            _flow = Compiler.Compile(flowSourceFilename, flowPath, services, out string errors);
            
            Errors = "";
            if (_flow == null) { Errors = errors; return; }

            _flow.OnOutput += filename => OnOutput(filename);
        }

        public string Errors { get; }


        public void ProcessText(string text) => _flow.ProcessText(text);
        public void ProcessFile(string filename) => _flow.ProcessFile(filename);

        public string[] Output => _flow.Output;

        public event Action<string> OnOutput = _ => {};
    }
}