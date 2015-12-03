using Planning.Util;
using StateSpaceSearchProject;
using System;
using System.Collections.Generic;

namespace HeuristicSearchPlannerSGW
{
    public class HeuristicQueue
    {
        public readonly StateHeuristic heuristic;
        private readonly PriorityQueue<HeuristicNode> queue;

        public HeuristicQueue(StateHeuristic heuristic, HeuristicComparator comparator)
        {
            this.heuristic = heuristic;
            this.queue = new PriorityQueue<HeuristicNode>();
        }

        public void clear()
        {
            queue.Clear();
        }

        public int size()
        {
            return queue.Size();
        }

        public HeuristicNode push(StateSpaceNode node)
        {
            HeuristicNode n = new HeuristicNode(node, heuristic.evaluate(node.state));
            queue.Enqueue(n,Convert.ToInt32( n.heuristic));
            return n;
        }

        public HeuristicNode peek()
        {
            return queue.Peek();
        }

        public double hPeek()
        {
            return queue.Peek().heuristic;
        }

        public StateSpaceNode pop()
        {
            return queue.Dequeue().state;
        }
    }
}