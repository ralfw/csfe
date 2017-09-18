using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace csfe.execution
{
    class ServiceBus {
        private readonly List<(string eventname, Action<string> handler)> _subscriptions = new List<(string, Action<string>)>();
        
        
        public void Subscribe(string eventname, Action<string> handler) {
            _subscriptions.Add((eventname,handler));
        }

        
        public void Send(string eventname, string message) {
            Debug.Print($"About to send '{eventname}'");
            var subscribers = _subscriptions.Where(s => s.eventname == eventname).ToArray();
            
            if (subscribers.Length == 0) 
                throw new ApplicationException($"Cannot send message! No subscriptions for event '{eventname}'.");
            
            foreach(var s in subscribers) {
                Debug.Print($"  sending");
                s.handler(message);
            };
        }
    }
}