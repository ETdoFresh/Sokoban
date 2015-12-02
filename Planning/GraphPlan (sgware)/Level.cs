using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphPlan
{
    public class Level
    {

        public  int number;
        public  Level previous;
	protected  PlanGraph graph;
	
	protected Level(PlanGraph graph, int number)
        {
            this.graph = graph;
            this.number = number;
            this.previous = number == 0 ? null : graph.getLevel(number - 1);
        }

        public virtual void computeMutexes()
        {

        }
    }

}
