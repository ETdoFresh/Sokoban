using Planning;
using Planning.Logic;
using Planning.Util;
using StateSpaceSearchProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicSearchPlanner
{
    public class HSPlanner : StateSpaceSearch
    {
        protected readonly PriorityQueue<StateSpaceNode> queue;
        private ImmutableArray<Step> _allSteps;
        private List<Literal> _allLiterals;
        private Dictionary<Literal, HSPNode> _nodeLookup;
        private List<HSPNode> _goals;

        public HSPlanner(StateSpaceProblem problem)
            : base(problem)
        {
            GetAllSteps();
            FactGatherer factGatherer = new FactGatherer(problem);
            _allLiterals = factGatherer.GetAllFacts();

            CreateHSPNodes();

            _goals = new List<HSPNode>();
            foreach (Literal literal in ConversionUtil.expressionToLiterals(problem.goal))
                _goals.Add(_nodeLookup[literal]);

            queue = new PriorityQueue<StateSpaceNode>();
            queue.Enqueue(root, 0);
        }

        public override Plan findNextSolution()
        {
            while (queue.Count > 0)
            {
                StateSpaceNode node = queue.Dequeue();
                if (problem.goal.IsTrue(node.state))
                    return node.plan;
                node.expand();
                foreach (StateSpaceNode child in node.children)
                    //if (!Prune(child)) // Use Novelty? Uncomment to use
                        queue.Enqueue(child, GetCost(child));
            }
            return null;
        }

        private bool Prune(StateSpaceNode node)
        {
            int maxWidth = 2;
            for (int i = 1; i <= maxWidth; i++)
                if (Novelty.HasNovelty((StateSpaceProblem)problem, node, i, _allLiterals))
                    return false;
            return true;
        }

        public Dictionary<StateSpaceNode, int> GetNextStatesCosts(int depth = 1)
        {
            Dictionary<StateSpaceNode, int> statesCosts = new Dictionary<StateSpaceNode, int>();
            GetNextStateCosts(depth, root, statesCosts);
            return statesCosts;
        }

        private void GetNextStateCosts(int depth, StateSpaceNode parent, Dictionary<StateSpaceNode, int> statesCosts)
        {
            if (depth <= 0)
                return;

            parent.expand();
            foreach(StateSpaceNode child in parent.children)
            {
                GetNextStateCosts(depth - 1, child, statesCosts);
                statesCosts.Add(child, GetCost(child));
            }
        }

        private int GetCost(StateSpaceNode child)
        {
            ResetNodes();
            var Nodes = _nodeLookup;
            foreach (Literal literal in child.state.Literals)
                _nodeLookup[literal].SetCost(0);

            foreach (Literal literal in GetImpliedLiterals(child.state.Literals))
                _nodeLookup[literal].SetCost(0);

            Dictionary<HSPNode, int> previousCosts = new Dictionary<HSPNode, int>();
            while (previousCosts.Count == 0 || HasChanged(previousCosts))
            {
                previousCosts.Clear();
                foreach (HSPNode node in _nodeLookup.Values)
                    previousCosts.Add(node, node.GetCost());

                foreach (Step step in _allSteps)
                {
                    foreach (Literal literal in ConversionUtil.expressionToLiterals(step.effect))
                    {
                        int currentCost = _nodeLookup[literal].GetCost();
                        int newCost = 1;
                        foreach (Literal precondition in ConversionUtil.expressionToLiterals(step.precondition))
                        {
                            HSPNode HSPprecondition = _nodeLookup[precondition];
                            if (HSPprecondition.GetCost() == int.MaxValue)
                            {
                                newCost = int.MaxValue;
                                break;
                            }
                            newCost += HSPprecondition.GetCost();
                        }
                        _nodeLookup[literal].SetCost(Math.Min(currentCost, newCost));
                    }
                }
            }

            int cost = 0;
            foreach (Literal literal in ConversionUtil.expressionToLiterals(problem.goal))
                cost += _nodeLookup[literal].GetCost();

            return cost;
        }

        private void CreateHSPNodes()
        {
            _nodeLookup = new Dictionary<Literal, HSPNode>();
            foreach (Literal literal in _allLiterals)
                _nodeLookup.Add(literal, new HSPNode(literal, int.MaxValue));
        }

        private void ResetNodes()
        {
            foreach (HSPNode node in _nodeLookup.Values)
                node.SetCost(int.MaxValue);
        }

        private bool HasChanged(Dictionary<HSPNode, int> _previousCosts)
        {
            foreach(HSPNode node in _nodeLookup.Values)
                if (node.GetCost() != _previousCosts[node])
                    return true;

            return false;
        }

        private void GetAllSteps()
        {
            _allSteps = (problem as StateSpaceProblem).steps;
        }

        private List<Literal> GetImpliedLiterals(List<Literal> statedLiterals)
        {
            List<Literal> impliedLiterals = new List<Literal>();
            foreach (Literal literal in _nodeLookup.Keys)
                if (literal is NegatedLiteral)
                    if (!statedLiterals.Contains(literal.Negate()))
                        impliedLiterals.Add(literal);
            return impliedLiterals;
        }
    }
}
