using System.Collections.Generic;

namespace csfe.compilation
{
    class ASTNode {
    }

    class ServiceFlowLangNode : ASTNode {
        public OperationListNode Sequence = new OperationListNode();
    }
    
    class OperationListNode : ASTNode {
        public T Get<T>(int index) where T:OperationNode => (T) Operations[index];
        public List<OperationNode> Operations = new List<OperationNode>();
    }
    
    class OperationNode : ASTNode {}
    
    class ServiceNode : OperationNode {
        public string Name;
        public string Instance = "";
        public string FullName => Name + (Instance != "" ? "." + Instance : "");

        public string Argument = "";
    }


    class SwitchNode : OperationNode {
        public List<SwitchOptionNode> Options = new List<SwitchOptionNode>();
    }
    
    class SwitchOptionNode {
        public string Tag;
        public OperationListNode Sequence = new OperationListNode();
    }
}