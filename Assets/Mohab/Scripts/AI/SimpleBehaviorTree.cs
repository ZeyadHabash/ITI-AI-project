using System;
using System.Collections.Generic;

namespace AI
{
    public enum NodeState { Running, Success, Failure }

    public abstract class Node
    {
        public NodeState State { get; protected set; }
        public abstract NodeState Evaluate();
    }

    public class Selector : Node
    {
        private List<Node> nodes = new List<Node>();

        public Selector(List<Node> nodes) => this.nodes = nodes;

        public override NodeState Evaluate()
        {
            foreach (var node in nodes)
            {
                switch (node.Evaluate())
                {
                    case NodeState.Running:
                        State = NodeState.Running;
                        return State;
                    case NodeState.Success:
                        State = NodeState.Success;
                        return State;
                    case NodeState.Failure:
                        continue;
                }
            }
            State = NodeState.Failure;
            return State;
        }
    }

    public class Sequence : Node
    {
        private List<Node> nodes = new List<Node>();

        public Sequence(List<Node> nodes) => this.nodes = nodes;

        public override NodeState Evaluate()
        {
            bool anyChildIsRunning = false;
            foreach (var node in nodes)
            {
                switch (node.Evaluate())
                {
                    case NodeState.Running:
                        State = NodeState.Running;
                        return State;
                    case NodeState.Success:
                        continue;
                    case NodeState.Failure:
                        State = NodeState.Failure;
                        return State;
                }
            }
            State = NodeState.Success;
            return State;
        }
    }
    
    public class ActionNode : Node
    {
        private Func<NodeState> action;

        public ActionNode(Func<NodeState> action) => this.action = action;

        public override NodeState Evaluate()
        {
            State = action?.Invoke() ?? NodeState.Failure;
            return State;
        }
    }
}
