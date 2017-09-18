using System;
using System.Collections.Generic;
using System.Linq;

namespace csfe.operations
{
    public class SwitchOperation {
        OutputPorts _ports = new OutputPorts();
        
        public void Switch(string filename) {
            foreach(var portname in _ports.Names)
                if (filename.EndsWith(portname))
                    _ports[portname](filename);
        }

        public OutputPorts OnOutput => _ports;
        
        
        public class OutputPorts {
            private readonly Dictionary<string, Action<string>> _handlers = new Dictionary<string, Action<string>>();

            public string[] Names => _handlers.Keys.ToArray();
            
            public Action<string> this[string portname] {
                internal get => _handlers[portname];
                set => _handlers[portname] = value;
            }   
        }
    }
}