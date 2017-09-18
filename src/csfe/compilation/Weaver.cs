using System;
using System.Collections.Generic;
using System.Diagnostics;
using csfe.execution;
using csfe.operations;

namespace csfe.compilation
{
    /*
        Weaving means connecting service operations to a bus as the backbone for message distribution.
        
        Service operations are not directly connected to each other. Rather they subscribe to events from the bus
        to receive input, and send output as events to the bus.
        
        The flow
        
            a > b > c
            
        will first be bracketed by an inbox and an outbox for communication with the environment:
        
            $inbox > a > b > c > $outbox
            
        Then the operations will be woven to the bus like this:
        
            $inbox i> bus i> a
            a 0> bus 0> b
            b 1> bus 1> c
            c o> bus o> $outbox
            
        "i", "0" etc. designate event names. That means:
        
        - $inbox is generating and even "i" which is sent to the bus.
        - operation a subscribes to an event "i".
        etc.
        
        Event names made up of an index plus an id for the operation sequence they occurr in. Hence they are
        local to an OperationList of the ServiceFlowLang definition.
            
    */
    class Weaver
    {
        private readonly string _flowPath;
        private readonly Dictionary<string, ServiceInfo> _services;
        
        
        public Weaver(string flowPath, Dictionary<string, ServiceInfo> services) {
            _flowPath = flowPath;
            _services = services;
        }
        
        
        public Flow Weave(ServiceFlowLangNode flowAST)
        {
            var bus = new ServiceBus();
            var inboxOp = new InboxOperation(_flowPath);
            const string inputEventname = "environment->";
            var outboxOp = new OutboxOperation(_flowPath);
            const string outputEventname = "->environment";

            Weave_to_environment();
            Weave_around_bus(bus, flowAST.Sequence, inputEventname, outputEventname);
            return new Flow(bus, inboxOp, outboxOp);

            void Weave_to_environment() {
                inboxOp.OnItemArrived += filename => bus.Send(inputEventname, filename);
                bus.Subscribe(outputEventname, outboxOp.AddFile);
            }
        }
        
        
        void Weave_around_bus(ServiceBus bus, OperationListNode opList, string inputEventname, string outputEventname) {
            var flowId = Guid.NewGuid().ToString();
            Debug.Print($"Weaving flow {flowId} with '{inputEventname}' / '{outputEventname}'");
            
            var ops = opList.Operations.ToArray();
            for(var i=0; i<ops.Length; i++) {
                var sourceEventname = i == 0 ? inputEventname : $"-{flowId}.{i - 1}>";
                var sinkEventname = i == ops.Length - 1 ? outputEventname : $"-{flowId}.{i}>";
                
                switch (ops[i]) {
                    case ServiceNode sn:
                        Weave_service(sn, sourceEventname, sinkEventname);
                        break;
                        
                    case SwitchNode swn:
                        Weave_switch(swn, sourceEventname, sinkEventname);
                        break;
                }
            }

            
            void Weave_service(ServiceNode sn, string sourceEventname, string sinkEventname) {
                Debug.Print($"  Service {sn.FullName} with '{sourceEventname}' / '{sinkEventname}'");
                        
                var service = _services[sn.Name];
                var arguments = service.Arguments.Replace("%argument", sn.Argument);
                var serviceOp = new ServiceOperation(service.Path,service.Executable,arguments);

                bus.Subscribe(sourceEventname, serviceOp.Enqueue);
                        
                serviceOp.OnOutput += filename => bus.Send(sinkEventname, filename);
            }

            
            void Weave_switch(SwitchNode swn, string sourceEventname, string sinkEventname) {
                Debug.Print($"  Switch with '{sourceEventname}' / '{sinkEventname}'");
                        
                var switchOp = new SwitchOperation();
                        
                bus.Subscribe(sourceEventname, switchOp.Switch);
                        
                foreach (var optn in swn.Options) {
                    var optionSinkEventname = $"{optn.Tag}/{sinkEventname}";
                    Debug.Print($"    tag '{optn.Tag}' ... / '{optionSinkEventname}'");
                            
                    switchOp.OnOutput[optn.Tag] = filename => bus.Send(optionSinkEventname, filename);
                            
                    Weave_around_bus(bus, optn.Sequence, optionSinkEventname, sinkEventname);
                }        
            }
        }
    }
}