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
    public class Novelty
    {
        public static bool HasNovelty(StateSpaceProblem problem, StateSpaceNode node, int size, ImmutableArray<Literal> literals)
        {
            return HasNovelty(literals, node, new Literal[size], 0, 0);
        }

        private static bool HasNovelty(ImmutableArray<Literal> literals, StateSpaceNode node, Literal[] conjunction, int index, int start)
        {
            if (index == conjunction.Length)
                return !Ever(node.parent, conjunction);
            else
            {
                for (int i = start; i < literals.length; i++)
                {
                    Literal literal = literals.get(i);
                    if (node.state.isTrue(literal))
                    {
                        conjunction[index] = literal;
                        if (HasNovelty(literals, node, conjunction, index + 1, i + 1))
                            return true;
                    }
                }
                return false;
            }
        }

        private static bool Ever(StateSpaceNode node, Literal[] conjunction)
        {
            if (node == null)
                return false;
            else if (Test(conjunction, node.state))
                return true;
            else
                return Ever(node.parent, conjunction);
        }

        private static bool Test(Literal[] conjunction, State state)
        {
            foreach (Literal literal in conjunction)
                if (!state.isTrue(literal))
                    return false;
            return true;
        }
    }
}
