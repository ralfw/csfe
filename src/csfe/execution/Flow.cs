using System;
using csfe.operations;

namespace csfe.execution
{
    class Flow {
        private readonly ServiceBus _bus;
        private readonly InboxOperation _inboxOp;
        private readonly OutboxOperation _outboxOp;

        
        public Flow(ServiceBus bus, InboxOperation inboxOp, OutboxOperation outboxOp)
        {
            _bus = bus;
            _inboxOp = inboxOp;
            _outboxOp = outboxOp;
            _outboxOp.OnItemArrived += filename => OnOutput(filename);
        }
        
        
        public void ProcessText(string text) { _inboxOp.AddText(text); }
        public void ProcessFile(string filename) { _inboxOp.AddFile(filename); }

        public string[] Output => _outboxOp.Items;
        public event Action<string> OnOutput = _ => { };
    }
}