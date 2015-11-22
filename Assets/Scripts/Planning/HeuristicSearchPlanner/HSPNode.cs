using Planning.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicSearchPlanner
{
    class HSPNode
    {
        private Literal _literal;
        private int _cost;

        public HSPNode(Literal literal, int cost)
        {
            _literal = literal;
            _cost = cost;
        }

        public void SetCost(int cost)
        {
            _cost = cost;
        }

        public int GetCost()
        {
            return _cost;
        }

        public Literal GetLiteral()
        {
            return _literal;
        }

        public override string ToString()
        {
            return string.Format("{0} [{1}]", _literal, _cost);
        }
    }
}
