using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Planning.Logic;
using Planning.Util;
using Planning;

namespace HeuristicSearchPlanner
{
    public class FactGatherer
    {
        private ImmutableArray<Constant> _allObjects;
        private ImmutableArray<Operator> _operators;

        public FactGatherer(Problem problem)
        {
            _allObjects = problem.objects;
            _operators = problem.domain.operators;
        }

        public List<Literal> GetAllFacts()
        {
            List<Literal> allFacts = new List<Literal>();
            List<Literal> ungroundedFacts = GetAllUngroundedFacts();

            //ApplyObjectsToUngroundedFacts()
            foreach (Literal ungroundedFact in ungroundedFacts)
                allFacts.AddRange(GetGroundedFacts(ungroundedFact));

            return allFacts;
        }

        private IEnumerable<Literal> GetGroundedFacts(Literal ungroundedFact)
        {
            List<Literal> groundedFacts = new List<Literal>();
            List<Constant> objects = new List<Constant>();
            GetGroundedFacts(0, (Predication)ungroundedFact, objects, groundedFacts);
            objects.Clear();
            return groundedFacts;
        }

        private void GetGroundedFacts(int depth, Predication ungroundedFact, List<Constant> objects, List<Literal> groundedFacts)
        {
            if (depth >= ungroundedFact.terms.length)
            {
                HashSubstitution sub = new HashSubstitution();
                for (int i = 0; i < depth; i++)
                    sub.set(ungroundedFact.terms.get(i), objects[i]);

                groundedFacts.Add((Literal)ungroundedFact.Substitute(sub));
                groundedFacts.Add((NegatedLiteral)ungroundedFact.Substitute(sub).Negate());
                return;
            }
            foreach (Constant obj in _allObjects)
                if (obj.type == ungroundedFact.terms.get(depth).type)
                {
                    objects.Add(obj);
                    GetGroundedFacts(depth + 1, ungroundedFact, objects, groundedFacts);
                    objects.RemoveAt(depth);
                }
        }

        private List<Literal> GetAllUngroundedFacts()
        {
            List<Literal> ungroundedFacts = new List<Literal>();
            ungroundedFacts.AddRange(GetPrecondtions());
            ungroundedFacts.AddRange(GetEffects());

            //RemoveNegations()
            for (int i = ungroundedFacts.Count - 1; i >= 0; i--)
                if (ungroundedFacts[i] is Negation)
                    ungroundedFacts[i] = ungroundedFacts[i].Negate();

            //RemoveDuplicatePredicates()
            for (int i = ungroundedFacts.Count - 1; i >= 0; i--)
                for (int j = ungroundedFacts.Count - 1; j >= 0; j--)
                    if (i != j)
                        if (((Predication)ungroundedFacts[i]).predicate == 
                            ((Predication)ungroundedFacts[j]).predicate)
                        {
                            ungroundedFacts.RemoveAt(i);
                            break;
                        }

            return ungroundedFacts;
        }

        private IEnumerable<Literal> GetPrecondtions()
        {
            List<Literal> preconditionLiterals = new List<Literal>();
            foreach (Operator oper in _operators)
                preconditionLiterals.AddRange(Split(oper.precondition));
            return preconditionLiterals;
        }

        private IEnumerable<Literal> GetEffects()
        {
            List<Literal> effectLiterals = new List<Literal>();
            foreach (Operator oper in _operators)
                effectLiterals.AddRange(Split(oper.effect));
            return effectLiterals;
        }

        private IEnumerable<Literal> Split(Expression expression)
        {
            List<Literal> literals = new List<Literal>();
            expression = expression.ToCNF();
            NAryBooleanExpression NABExpression = (NAryBooleanExpression)expression;
            foreach (Expression argument in NABExpression.arguments)
                literals.Add((Literal)((Disjunction)argument).arguments.get(0));
            return literals;
        }
    }
}
